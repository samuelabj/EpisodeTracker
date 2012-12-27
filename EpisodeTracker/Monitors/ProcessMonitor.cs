using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediaReign.Core.TvMatchers;
using MediaReign.EpisodeTracker.Data;
using MediaReign.EpisodeTracker.Models;
using MediaReign.TVDB;
using NLog;

namespace MediaReign.EpisodeTracker.Monitors {
	public delegate void MonitoredFileHandler(ProcessMonitor monitor, MonitoredFileEventArgs args);
	public class MonitoredFileEventArgs {
		public string Filename { get; set; }
		public string FriendlyName { get; set; }
		public bool ProbablyWatched { get; set; }
	}

	public class ProcessMonitor : IDisposable {

		class MonitoredFile {
			public DateTime Start { get; set; }
			public string Filename { get; set; }
			public TvMatch Match { get; set; }
			public TVDBSeries Series { get; set; }
			public TVDBEpisode Episode { get; set; }
			public TimeSpan? Length { get; set; }
			public bool Tracking { get; set; }
			public int PreviousTrackedSeconds { get; set; }
			public bool Watched { get; set; }
			public int MissingStrikes { get; set; }
			public string FriendlyName {
				get {
					var m = Match;
					if(m != null) {
						var series = Series != null ? Series.Name : m.Name;
						var epName = Episode != null ? " - " + Episode.Name : null;
						return String.Format("{0} - S{1:00}E{2:00}{3}", series, m.Season, m.Episode, epName);
					}

					return Path.GetFileName(Filename);
				}
			}
		}

		static readonly string[] VideoExtensions = new[] {
			"avi"
			,"divx"
			,"dvr"
			,"mkv"
			,"mp4"
			,"mpeg"
			,"mpeg4"
			,"mpg"
			,"ogm"
			,"wmv"
		};

		List<MonitoredFile> monitored;
		Task checkTask;
		AutoResetEvent checkEvent = new AutoResetEvent(false);

		public ProcessMonitor(Logger logger) {
			ApplicationNames = new List<string>();
			ApplicationNames.Add("PotPlayerMini64");
			Logger = logger;
		}

		public event MonitoredFileHandler FileAdded;
		public event MonitoredFileHandler FileRemoved;

		public List<string> ApplicationNames { get; private set; }
		public Logger Logger { get; private set; }
		public bool Running { get { return checkTask != null; } }

		public void Start() {
			monitored = new List<MonitoredFile>();
			checkTask = Task.Factory.StartNew(() => {
				var wait = TimeSpan.FromSeconds(5);

				do {
					try {
						Check();
					} catch(Exception e) {
						Logger.Error("Error while checking processes: " + e, e);
					}

				} while(!checkEvent.WaitOne(wait));
			});
		}

		public void Stop() {
			checkEvent.Set();
			checkTask.Wait();
			checkTask = null;
		}

		void Check() {
			Logger.Trace("Checking for open files");

			var files = GetMediaFiles();
			Logger.Trace("Files found: " + files.Count());

			CheckFiles(files);
			CheckMissing(files);			
		}

		void CheckFiles(IEnumerable<string> files) {
			foreach(var f in files) {
				Logger.Trace("Found file: " + f);
				var mon = monitored.SingleOrDefault(m => m.Filename.Equals(f, StringComparison.OrdinalIgnoreCase));
				if(mon == null) {
					CheckUnmonitoredFile(f);
				} else {
					CheckMonitoredFile(mon);
				}
			}
		}

		void CheckUnmonitoredFile(string filename) {
			Logger.Debug("File is not monitored: " + filename);
			var mon = new MonitoredFile {
				Filename = filename,
				Start = DateTime.Now,
				Length = GetVideoLength(filename)
			};

			var match = new TvMatcher().Match(filename);
			if(match != null) {
				Logger.Debug("Found episode info - name: " + match.Name + ", season: " + match.Season + ", episode: " + match.Episode);
				// try and look it up
				// do movies later
				var results = new TVDBRequest().Search(match.Name);
				var first = results.FirstOrDefault();

				if(first != null) {
					Logger.Debug("Found TVDB result: " + first.Name);

					var series = new TVDBRequest().Series(first.ID, true);
					mon.Series = series;
					mon.Episode = series.Episodes.FirstOrDefault(ep => 
						(
							match.Season.HasValue 
							&& ep.Season == match.Season 
							&& ep.Number == match.Episode
						)
						|| 
						(
							!match.Season.HasValue 
							&& ep.AbsoluteNumber == match.Episode
						)
					);

					if(mon.Episode != null) Logger.Debug("Found TVDB episode: " + mon.Episode.Name);
				}
			}

			mon.Match = match;
			monitored.Add(mon);
		}

		void CheckMonitoredFile(MonitoredFile mon) {
			Logger.Trace("File is monitored");
			mon.MissingStrikes = 0;

			if(!mon.Watched) {
				using(var db = new EpisodeTrackerDBContext()) {
					Logger.Trace("Seconds since started monitoring: " + DateTime.Now.Subtract(mon.Start).TotalSeconds);

					// check if it's been monitored for a while before doing anything with file
					if(mon.Start <= DateTime.Now.AddMinutes(-.5)) {
						var tracked = GetTrackedItem(db, mon);

						if(!mon.Tracking) {
							Logger.Debug("Begin tracking file: " + mon.Filename);
							if(tracked == null) {
								Logger.Debug("Recording file/episode as tracked");
								tracked = NewTrackedItem(db, mon);
							} else {
								Logger.Debug("This file has been tracked before");
								mon.PreviousTrackedSeconds = tracked.TrackedSeconds;
								mon.Watched = tracked.Watched;
							}
							mon.Tracking = true;
							if(FileAdded != null) {
								FileAdded(this, new MonitoredFileEventArgs {
									Filename = mon.Filename,
									FriendlyName = mon.FriendlyName
								});
							}
						}

						tracked.TrackedSeconds = (int)DateTime.Now.Subtract(mon.Start).TotalSeconds + mon.PreviousTrackedSeconds;
						tracked.LastTracked = DateTime.Now;
						Logger.Trace("Total tracked seconds: " + tracked.TrackedSeconds);
						db.SaveChanges();
					}
				}
			}
		}

		void CheckMissing(IEnumerable<string> files) {
			for(var i = 0; i < monitored.Count; i++) {
				var mon = monitored[i];

				if(!files.Contains(mon.Filename)) {
					Logger.Debug("Monitored file is no longer open: " + mon.Filename);
					Logger.Debug("Process output: " + processOutput);

					if(mon.MissingStrikes++ < 1) continue;

					// not open anymore
					if(mon.Tracking) {
						ITrackedItem tracked;
						using(var db = new EpisodeTrackerDBContext()) {
							tracked = GetTrackedItem(db, mon);
							if(mon.Length.HasValue && tracked.TrackedSeconds >= (mon.Length.Value.TotalSeconds * .75)) {
								Logger.Debug("Monitored file has probably been watched: " + mon.Filename);
								tracked.ProbablyWatched = true;
								db.SaveChanges();
							}
						}
						if(FileRemoved != null) {
							FileRemoved(this, new MonitoredFileEventArgs {
								Filename = mon.Filename,
								FriendlyName = mon.FriendlyName,
								ProbablyWatched = tracked.ProbablyWatched
							});
						}
					}

					monitored.RemoveAt(i);
					i--;
					continue;
				}
			}
		}

		ITrackedItem GetTrackedItem(EpisodeTrackerDBContext db, MonitoredFile mon) {
			if(mon.Match != null) {
				var match = mon.Match;
				var seriesName = match.Name;
				var episode = match.Episode;
				var season = match.Season;

				if(mon.Episode != null) {
					seriesName = mon.Series.Name;
					episode = mon.Episode.Number;
					season = mon.Episode.Season;
				}

				// ignore multi episode files for now
				return db.TrackedEpisodes
					.FirstOrDefault(e => 
						e.FileName == mon.Filename 
						|| e.TrackedSeries.Name == seriesName 
						&& e.Season == season 
						&& e.Number == episode
					);
			} else {
				return db.TrackedOthers
					.SingleOrDefault(o => o.FileName == mon.Filename);
			}
		}

		ITrackedItem NewTrackedItem(EpisodeTrackerDBContext db, MonitoredFile mon) {
			ITrackedItem tracked = null;
			if(mon.Match != null) {
				var series = db.TrackedSeries.SingleOrDefault(s => s.Name == mon.Match.Name);
				if(series == null) {
					series = new TrackedSeries {
						Name = mon.Match.Name,
						Added = DateTime.Now
					};
					db.TrackedSeries.Add(series);
				}

				tracked = new TrackedEpisode {
					FileName = mon.Filename,
					Season = mon.Match.Season ?? 0,
					Number = mon.Match.Episode
				};
				series.TrackedEpisodes.Add((TrackedEpisode)tracked);
			} else {
				tracked = new TrackedOther {
					FileName = mon.Filename
				};
				db.TrackedOthers.Add((TrackedOther)tracked);
			}
			tracked.Added = DateTime.Now;
			tracked.LastTracked = tracked.Added;

			db.SaveChanges();
			return tracked;
		}

		string processOutput;
		List<string> GetMediaFiles() {
			var videoFiles = new List<string>();

			var helper = new HandleHelper();
			var processes = helper.GetProcesses();
			processOutput = helper.Output;

			foreach(var process in processes) {
				if(!ApplicationNames.Contains(process.ProcessName)) continue;

				foreach(var f in process.Files) {
					var ext = Path.GetExtension(f);
					if(ext.Length > 1) ext = ext.Substring(1);
					if(VideoExtensions.Contains(ext.ToLower()) && !videoFiles.Contains(f, StringComparer.OrdinalIgnoreCase)) {
						videoFiles.Add(f);
					}
				}
			}
			return videoFiles;
		}

		TimeSpan? GetVideoLength(string file) {
			var fi = new FileInfo(file);
			var shellAppType = Type.GetTypeFromProgID("Shell.Application");

			// hack to fix exception - Unable to cast COM object of type 'System.__ComObject' to interface type 'Shell32.Shell'
			dynamic shell = Activator.CreateInstance(shellAppType);
			var folder = (Shell32.Folder)shell.NameSpace(fi.DirectoryName);
			var item = folder.ParseName(fi.Name);

			for(int i = 0; i < short.MaxValue; i++) {
				var key = folder.GetDetailsOf(null, i);
				if(key != "Length") continue;
				var val = folder.GetDetailsOf(item, i);
				TimeSpan length;
				if(TimeSpan.TryParse(val, out length)) {
					return length;
				}
			}

			return null;
		}

		public void Dispose() {
			if(Running) Stop();
		}
	}
}
