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
using MediaReign.Core;

namespace EpisodeTracker.Core.Monitors {
	public delegate void MonitoredFileHandler(ProcessMonitor monitor, MonitoredFileEventArgs args);
	public class MonitoredFileEventArgs {
		public string Filename { get; set; }
		public string FriendlyName { get; set; }
		public bool Watched { get; set; }
	}

	public class ProcessMonitor : IDisposable {

		class MonitoredFile {
			public DateTime Start { get; set; }
			public string FileName { get; set; }
			public TvMatch TvMatch { get; set; }
			public Series Series { get; set; }
			public IEnumerable<Episode> Episodes { get; set; }
			public int? TrackedFileID { get; set; }
			public TimeSpan Length { get; set; }
			public bool Tracking { get; set; }
			public int PreviousTrackedSeconds { get; set; }
			public bool Watched { get; set; }
			public string FriendlyName {
				get {
					if(Episodes != null && Episodes.Any()) {
						return String.Format(
							"{0} - S{1:00}E{2:00}{3} - {4}",
							Series.Name,
							Episodes.First().Season,
							Episodes.First().Number,
							Episodes.Count() > 1 ? String.Format("-E{0:00}", Episodes.Last().Number) : null,
							String.Join(" + ", Episodes.Select(ep => ep.Name))
						);
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

		void CheckUnmonitoredFile(string fileName) {
			Logger.Debug("File is not monitored: " + fileName);
			var mon = new MonitoredFile {
				FileName = fileName,
				Start = DateTime.Now
			};

			using(var info = new MediaInfo(fileName)) {
				mon.Length = info.Length;
			}

			var match = new TvMatcher().Match(fileName);
			if(match != null) {
				Logger.Debug("Found episode info - name: " + match.Name + ", season: " + match.Season + ", episode: " + match.Episode);
				
				mon.TvMatch = match;
				
				// Try and look it up
				// TODO: movies
				Series series;
				using(var db = new EpisodeTrackerDBContext()) {
					series = db.Series.SingleOrDefault(s => s.Name == match.Name || s.Aliases.Any(a => a.Name == match.Name));

					int? tvdbid = null;
					if(series == null) {
						var results = new TVDBRequest().Search(match.Name);
						var first = results.FirstOrDefault();
						if(first != null) {
							Logger.Debug("Found TVDB result: " + first.Name);
							series = db.Series.SingleOrDefault(s => s.TVDBID == first.ID || s.Name == first.Name || s.Aliases.Any(a => a.Name == first.Name));
							tvdbid = first.ID;
						}
					}else {
						tvdbid = series.TVDBID;
					}

					if(tvdbid.HasValue) {
						if(series == null || series.Updated <= DateTime.Now.AddDays(-7)) {
							var syncer = new TVDBSeriesSyncer {
								TVDBID = tvdbid.Value,
								Name = match.Name,
								DownloadBannersAsync = true
							};
							syncer.Sync();
						}

						// Pull out series again as it might have been updated
						series = db.Series
							.Include(s => s.Episodes)
							.Single(s => s.TVDBID == tvdbid.Value);

						mon.Series = series;

						if(match.Season.HasValue) {
							var eps = series.Episodes.Where(ep => ep.Season == match.Season.Value);
							if(match.ToEpisode.HasValue) {
								mon.Episodes = eps.Where(ep => ep.Number >= match.Episode && ep.Number <= match.ToEpisode.Value);
							} else {
								mon.Episodes = eps.Where(ep => ep.Number == match.Episode);
							}
						} else {
							mon.Episodes = series.Episodes.Where(ep => ep.AbsoluteNumber == match.Episode);
						}

						if(mon.Episodes != null) Logger.Debug("Found TVDB episodes: " + String.Join(" + ", mon.Episodes.Select(e => e.Name)));
					}
				}
			}

			monitored.Add(mon);
		}

		void CheckMonitoredFile(MonitoredFile mon) {
			Logger.Trace("File is monitored");

			if(!mon.Watched) {
				using(var db = new EpisodeTrackerDBContext()) {
					Logger.Trace("Seconds since started monitoring: " + DateTime.Now.Subtract(mon.Start).TotalSeconds);

					// check if it's been monitored for a while before doing anything with file
					if(mon.Start <= DateTime.Now.AddMinutes(-.0)) {
						var tracked = GetTrackedFile(db, mon);

						if(!mon.Tracking) {
							Logger.Debug("Begin tracking file: " + mon.FileName);
							if(tracked == null) {
								Logger.Debug("Recording file/episode as tracked");
								tracked = NewTrackedFile(db, mon);
							} else {
								Logger.Debug("This file has been tracked before");
								mon.PreviousTrackedSeconds = tracked.TrackedSeconds;
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
						tracked.Stop = DateTime.Now;
						foreach(var te in tracked.Episodes) te.Updated = DateTime.Now;

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

					// Double check - sometimes the file seems to be released from the process
					// Might need to delay this a tiny bit
					if(GetMediaFiles().Any(f => f.Equals(mon.FileName, StringComparison.OrdinalIgnoreCase))) {
						CheckMonitoredFile(mon);
						return;
					}

					// Not open anymore
					if(mon.Tracking) {
						TrackedFile tracked;
						bool watched = false;

						using(var db = new EpisodeTrackerDBContext()) {
							tracked = GetTrackedFile(db, mon);
							if(tracked.TrackedSeconds >= (mon.Length.TotalSeconds * .66)) {
								Logger.Debug("Monitored file has probably been watched: " + mon.FileName);
								watched = true;
								foreach(var ep in tracked.Episodes) {
									ep.Watched = true;
									ep.Updated = DateTime.Now;
								}
								db.SaveChanges();
							}
						}

						if(FileRemoved != null) {
							FileRemoved(this, new MonitoredFileEventArgs {
								Filename = mon.FileName,
								FriendlyName = mon.FriendlyName,
								Watched = watched
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
			if(mon.TrackedFileID.HasValue) return db.TrackedFile.Single(f => f.ID == mon.TrackedFileID.Value);

			if(file == null) {
				file = db.TrackedFile.SingleOrDefault(f => f.FileName == mon.FileName);
			}

			if(file == null && mon.Episodes != null) {
				var episodeIDs = mon.Episodes.Select(e => e.ID);
				file = db.TrackedFile
					.FirstOrDefault(f => f.Episodes.Any() && f.Episodes.All(te => episodeIDs.Contains(te.Episode.ID)));
			}

			if(file == null && mon.TvMatch != null) {
				file = db.TrackedFile
					.FirstOrDefault(f =>
						f.Episodes.Any()
						&& f.Episodes.All(te => 
							te.Episode.Series.Name == mon.TvMatch.Name
							&& te.Episode.Season == mon.TvMatch.Season
							&& (
								!mon.TvMatch.ToEpisode.HasValue && te.Episode.Number == mon.TvMatch.Episode
								|| mon.TvMatch.ToEpisode.HasValue && te.Episode.Number >= mon.TvMatch.Episode && te.Episode.Number <= mon.TvMatch.ToEpisode.Value
							)
						)
					);
			}

			if(file != null) mon.TrackedFileID = file.ID;

			return file;
		}

		TrackedFile NewTrackedFile(EpisodeTrackerDBContext db, MonitoredFile mon) {
			var tracked = new TrackedFile {
				FileName = mon.FileName,
				Start = DateTime.Now,
				Stop = DateTime.Now,
				LengthSeconds = mon.Length.TotalSeconds
			};
			db.TrackedFile.Add(tracked);

			if(mon.TvMatch != null) {
				var seriesQuery = db.Series.Include(s => s.Episodes);
				Series series = null;
				if(mon.Series != null) {
					series = db.Series.SingleOrDefault(s => s.ID == mon.Series.ID);
				} else {
					// Series is only null when no TVDB match was found
					series = seriesQuery.SingleOrDefault(s => s.Name == mon.TvMatch.Name);
				}

				if(series == null) {
					series = new Series {
						Name = mon.TvMatch.Name,
						Added = DateTime.Now
					};

					db.Series.Add(series);
				}

				IEnumerable<Episode> episodes = null;
				if(mon.Episodes != null) {
					var ids = mon.Episodes.Select(e => e.ID);
					episodes = series.Episodes
						.Where(ep => ids.Contains(ep.ID));

					foreach(var ep in episodes) {
						tracked.Episodes.Add(new TrackedEpisode { 
							Episode = ep,
 							Added = DateTime.Now,
							Updated = DateTime.Now
						});
					}
				} else {
					// Check for loose reference
					episodes = series.Episodes
						.Where(ep =>
							ep.Season == mon.TvMatch.Season
							&& (
								!mon.TvMatch.ToEpisode.HasValue && ep.Number == mon.TvMatch.Episode
								|| mon.TvMatch.ToEpisode.HasValue && ep.Number >= mon.TvMatch.Episode && ep.Number <= mon.TvMatch.ToEpisode.Value
							)
						);

					for(var i = mon.TvMatch.Episode; i < (mon.TvMatch.ToEpisode ?? mon.TvMatch.Episode); i++) {
						var episode = series.Episodes.SingleOrDefault(ep => ep.Season == mon.TvMatch.Season && ep.Number == i);
						if(episode == null) {
							episode = new Episode {
								Season = mon.TvMatch.Season ?? 0,
								Number = i,
								Added = DateTime.Now
							};

							series.Episodes.Add(episode);
						}

						episode.Updated = DateTime.Now;
						tracked.Episodes.Add(new TrackedEpisode { 
							Episode = episode,
							Added = DateTime.Now,
							Updated = DateTime.Now
						});
					}

					series.Updated = DateTime.Now;
				}			

				mon.Series = series;
				mon.Episodes = tracked.Episodes.Select(te => te.Episode);
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
						// Remove weird mup prefix if it exists
						var mupPrefix = @"\Device\Mup\";
						var fileName = f;
						if(f.StartsWith(mupPrefix, StringComparison.OrdinalIgnoreCase)) {
							fileName = @"\\" + fileName.Remove(0, mupPrefix.Length);
							Logger.Debug("Removed mup prefix: " + fileName);
						}

						videoFiles.Add(fileName);
					}
				}
			}
			return videoFiles;
		}

		public void Dispose() {
			if(Running) Stop();
		}
	}
}
