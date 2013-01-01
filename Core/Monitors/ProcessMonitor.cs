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
using EpisodeTracker.Core.Data;
using EpisodeTracker.Core.Models;
using MediaReign.TVDB;
using NLog;
using System.Data.Entity;
using System.Data.SqlTypes;

namespace EpisodeTracker.Core.Monitors {
	public delegate void MonitoredFileHandler(ProcessMonitor monitor, MonitoredFileEventArgs args);
	public class MonitoredFileEventArgs {
		public string Filename { get; set; }
		public string FriendlyName { get; set; }
		public bool ProbablyWatched { get; set; }
	}

	public class ProcessMonitor : IDisposable {

		class MonitoredFile {
			public DateTime Start { get; set; }
			public string FileName { get; set; }
			public TvMatch TvMatch { get; set; }
			public Series Series { get; set; }
			public Episode Episode { get; set; }
			public int? TrackedFileID { get; set; }
			public TimeSpan? Length { get; set; }
			public bool Tracking { get; set; }
			public int PreviousTrackedSeconds { get; set; }
			public bool Watched { get; set; }
			public int MissingStrikes { get; set; }
			public string FriendlyName {
				get {
					if(Episode != null) {
						return String.Format("{0} - S{1:00}E{2:00} - {3}", Series.Name, Episode.Season, Episode.Number, Episode.Name);
					}

					return Path.GetFileName(FileName);
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
						throw new ApplicationException("Error while checking processes: " + e.Message, e);
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
				var mon = monitored.SingleOrDefault(m => m.FileName.Equals(f, StringComparison.OrdinalIgnoreCase));
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
				FileName = filename,
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
					
					using(var db = new EpisodeTrackerDBContext()) {
						var series = db.Series.SingleOrDefault(s => s.TVDBID == first.ID);
						if(series == null || series.Updated <= DateTime.Now.AddDays(-7)) {
							TVDBSeriesSyncer.Sync(first.ID, match.Name);
						}

						// Pull out series again as it might have been updated
						series = db.Series.Include(s => s.Episodes).Single(s => s.TVDBID == first.ID);
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
					}					

					if(mon.Episode != null) Logger.Debug("Found TVDB episode: " + mon.Episode.Name);
				}
			}

			mon.TvMatch = match;
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
						var tracked = GetTrackedFile(db, mon);

						if(!mon.Tracking) {
							Logger.Debug("Begin tracking file: " + mon.FileName);
							if(tracked == null) {
								Logger.Debug("Recording file/episode as tracked");
								tracked = NewTrackedFile(db, mon);
							} else {
								Logger.Debug("This file has been tracked before");
								mon.PreviousTrackedSeconds = tracked.TrackedSeconds;
								mon.Watched = tracked.Watched;
							}
							mon.Tracking = true;
							if(FileAdded != null) {
								FileAdded(this, new MonitoredFileEventArgs {
									Filename = mon.FileName,
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

				if(!files.Contains(mon.FileName)) {
					Logger.Debug("Monitored file is no longer open: " + mon.FileName);
					//Logger.Debug("Process output: " + processOutput);

					if(mon.MissingStrikes++ < 1) continue;

					// not open anymore
					if(mon.Tracking) {
						TrackedFile tracked;
						using(var db = new EpisodeTrackerDBContext()) {
							tracked = GetTrackedFile(db, mon);
							if(mon.Length.HasValue && tracked.TrackedSeconds >= (mon.Length.Value.TotalSeconds * .66)) {
								Logger.Debug("Monitored file has probably been watched: " + mon.FileName);
								tracked.ProbablyWatched = true;
								db.SaveChanges();
							}
						}
						if(FileRemoved != null) {
							FileRemoved(this, new MonitoredFileEventArgs {
								Filename = mon.FileName,
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

		TrackedFile GetTrackedFile(EpisodeTrackerDBContext db, MonitoredFile mon) {
			TrackedFile file = null;
			if(mon.TrackedFileID.HasValue) return db.TrackedFiles.Single(f => f.ID == mon.TrackedFileID.Value);

			if(mon.Episode != null) {
				file = db.TrackedFiles.SingleOrDefault(f => f.Episode.TVDBID == mon.Episode.ID);
				if(file == null) {
					file = db.TrackedFiles
						.FirstOrDefault(f =>
							f.Episode.Series.Name == mon.Series.Name
							&& f.Episode.Season == mon.Episode.Season
							&& f.Episode.Number == mon.Episode.Number
						);
				}
			}

			if(file == null && mon.TvMatch != null) {
				file = db.TrackedFiles
					.FirstOrDefault(f =>
						f.FileName == mon.FileName
						|| f.Episode.Series.Name == mon.TvMatch.Name
						&& f.Episode.Season == mon.TvMatch.Season
						&& f.Episode.Number == mon.TvMatch.Episode
					);
			}

			if(file == null) {
				file = db.TrackedFiles.SingleOrDefault(f => f.FileName == mon.FileName);
			}

			if(file != null) mon.TrackedFileID = file.ID;

			return file;
		}

		TrackedFile NewTrackedFile(EpisodeTrackerDBContext db, MonitoredFile mon) {
			var tracked = new TrackedFile {
				FileName = mon.FileName,
				Added = DateTime.Now,
				LastTracked = DateTime.Now,
				SecondsLength = mon.Length.HasValue ? mon.Length.Value.TotalSeconds : default(double?)
			};
			db.TrackedFiles.Add(tracked);

			if(mon.TvMatch != null) {
				var series = mon.Series;
				
				// Series is only null when no TVDB match was found
				if(series == null) {
					var seriesQuery = db.Series.Include(s => s.Episodes);
					series = seriesQuery.SingleOrDefault(s => s.Name == mon.TvMatch.Name);
				}

				if(series == null) {
					series = new Series {
						Name = mon.TvMatch.Name,
						Added = DateTime.Now
					};

					db.Series.Add(series);
				}

				Episode episode = null;
				if(mon.Episode != null) {
					episode = series.Episodes
						.Single(ep => ep.TVDBID == mon.Episode.ID);
				} else {
					// check for loose reference
					episode = series.Episodes
						.SingleOrDefault(ep =>
							ep.Number == mon.TvMatch.Episode
							&& ep.Season == mon.TvMatch.Season
						);

					if(episode == null) {
						episode = new Episode {
							Season = mon.TvMatch.Season ?? 0,
							Number = mon.TvMatch.Episode,
							Added = DateTime.Now
						};
						
						series.Episodes.Add(episode);
					}

					episode.Updated = DateTime.Now;
					series.Updated = DateTime.Now;
				}			

				tracked.Episode = episode;
				mon.Series = series;
				mon.Episode = episode;
			}

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
