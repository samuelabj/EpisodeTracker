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
using NLog;

namespace MediaReign.EpisodeTracker.Monitors {
	public class ProcessMonitor {
		class MonitoredFile {
			public DateTime Start { get; set; }
			public string Filename { get; set; }
			public TvMatch Match { get; set; }
			public TimeSpan? Length { get; set; }
			public bool Tracked { get; set; }
			public int PreviousTrackedSeconds { get; set; }
			public bool Watched { get; set; }
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

		public List<string> ApplicationNames { get; private set; }
		public Logger Logger { get; private set; }
		public bool Running { get { return checkTask != null; } }

		public void Start() {
			monitored = new List<MonitoredFile>();
			checkTask = Task.Factory.StartNew(() => {
				var wait = TimeSpan.FromSeconds(30);

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

			foreach(var f in files) {
				Logger.Trace("Found file: " + f);
				var mon = monitored.SingleOrDefault(m => m.Filename.Equals(f, StringComparison.OrdinalIgnoreCase));
				if(mon == null) {
					Logger.Debug("File is not monitored: " + f);
					var match = new TvMatcher().Match(f);
					if(match != null) {
						Logger.Debug("Found episode info - name: " + match.Name + ", season: " + match.Season + ", episode: " + match.Episode);
					}

					mon = new MonitoredFile {
						Filename = f,
						Start = DateTime.Now,
						Match = match,
						Length = GetVideoLength(f)
					};
					monitored.Add(mon);

					// check if it's been tracked before
					using(var db = new EpisodeTrackerDBContext()) {
						var tracked = GetTrackedItem(db, mon);
						if(tracked != null) {
							Logger.Debug("This file has been tracked before");
							mon.Tracked = true;
							mon.PreviousTrackedSeconds = tracked.TrackedSeconds;
							mon.Watched = tracked.Watched;
						}
					}
					
				} else {
					Logger.Trace("File is monitored");

					if(!mon.Watched) {
						using(var db = new EpisodeTrackerDBContext()) {
							ITrackedItem tracked = null;
							Logger.Trace("Seconds since started monitoring: " + DateTime.Now.Subtract(mon.Start).TotalSeconds);

							// check if it's been monitored for a while before doing anything with file
							if(mon.Start <= DateTime.Now.AddMinutes(-1)) {				
								if(!mon.Tracked) {
									Logger.Debug("Recording file/episode as tracked: " + f);
									tracked = NewTrackedItem(db, mon);
									mon.Tracked = true;
								}


								if(tracked == null) tracked = GetTrackedItem(db, mon);
								tracked.TrackedSeconds = (int)DateTime.Now.Subtract(mon.Start).TotalSeconds + mon.PreviousTrackedSeconds;
								tracked.LastTracked = DateTime.Now;
								Logger.Trace("Total tracked seconds: " + tracked.TrackedSeconds);

								//if(mon.Length.HasValue && tracked.TrackedSeconds >= (mon.Length.Value.TotalSeconds * .75)) {
								//	Logger.Debug("Monitored file has probably been watched: " + mon.Filename);
								//	tracked.ProbablyWatched = true;
								//}
								db.SaveChanges();
							}
						}
					}
				}
			}

			for(var i = 0; i < monitored.Count; i++) {
				var mon = monitored[i];

				if(!files.Contains(mon.Filename)) {
					Logger.Debug("Monitored file is no longer open and will be removed: " + mon.Filename);

					// not open anymore
					if(mon.Tracked) {
						using(var db = new EpisodeTrackerDBContext()) {
							var tracked = GetTrackedItem(db, mon);
							if(mon.Length.HasValue && tracked.TrackedSeconds >= (mon.Length.Value.TotalSeconds * .75)) {
								Logger.Debug("Monitored file has probably been watched: " + mon.Filename);
								tracked.ProbablyWatched = true;
								db.SaveChanges();
							}
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
				// ignore multi episode files for now
				return db.TrackedEpisodes.FirstOrDefault(e => e.FileName == mon.Filename|| e.TrackedSeries.Name == match.Name && e.Number == match.Episode);
			} else {
				return db.TrackedOthers.SingleOrDefault(o => o.FileName == mon.Filename);
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

		List<string> GetMediaFiles() {
			var videoFiles = new List<string>();

			var helper = new HandleHelper();
			var processes = helper.GetProcesses();

			foreach(var process in processes) {
				if(!ApplicationNames.Contains(process.Name)) continue;

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
	}
}
