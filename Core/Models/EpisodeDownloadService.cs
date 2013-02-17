using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EpisodeTracker.Core.Data;
using System.Data.Entity;
using System.Net;
using System.Diagnostics;
using System.IO;
using System.Collections.ObjectModel;
using EpisodeTracker.Core.Logging;

namespace EpisodeTracker.Core.Models {
	public delegate void EpisodeDownloadServiceHandler(EpisodeDownloadService service, EpisodeDownloadServiceEventArgs args);
	public class EpisodeDownloadServiceEventArgs {
		public ReadOnlyCollection<Tuple<Episode, EpisodeTorrentSearcherResult>> Episodes { get; set; }
	}

	public class EpisodeDownloadService : IDisposable {
		Task checkTask;
		AutoResetEvent checkEvent = new AutoResetEvent(false);
		object downloadedLock = new object();

		public EpisodeDownloadService() {
			Logger = Logger.Get("EpisodeDownloadService");
		}

		public event EpisodeDownloadServiceHandler DownloadsFound;
		public event EpisodeDownloadServiceHandler DownloadFound;

		public Logger Logger { get; private set; }
		public bool Running { get { return checkTask != null; } }

		public void Start() {
			Logger.Info("Starting EpisodeDownloadService");

			checkTask = Task.Factory.StartNew(() => {
				var wait = TimeSpan.FromSeconds(60);

				do {
					try {
						Check();
					} catch(Exception e) {
						Logger.Error("Error while checking for downloads: " + e, e);
					}

				} while(!checkEvent.WaitOne(wait));
			});
		}

		public void Stop() {
			checkEvent.Set();
			checkTask.Wait();
			checkTask = null;

			Logger.Info("EpisodeDownloadService stopped");
		}

		public void Dispose() {
			if(Running) Stop();
		}

		void Check() {
			Logger.Debug("Checking");

			IEnumerable<Episode> episodes;
			using(var db = new EpisodeTrackerDBContext()) {
				episodes = db.Episodes.Where(ep =>
					ep.Series.DownloadAutomatically
					&& ep.Season != 0
					&& ep.FileName == null
					&& ep.Aired <= DateTime.Now
					&& !ep.DownloadLog.Any()
				)
				.Include(ep => ep.Series)
				.ToList();
			}

			Logger.Debug("Found episodes which need to be downloaded: " + episodes.Count());
			var found = new List<Tuple<Episode, EpisodeTorrentSearcherResult>>();

			Parallel.ForEach(episodes, episode => {
				Logger.Debug().Message("Searching for episode").Episode(episode.ID).Log();

				var searcher = new EpisodeTorrentSearcher {
					MinSeeds = episode.Series.DownloadMinSeeds,
					MinMB = episode.Series.DownloadMinMB,
					MaxMB = episode.Series.DownloadMaxMB,
					HD = episode.Series.DownloadHD,
					UseAbsoluteEpisodeFormat = episode.Series.DownloadUseAbsoluteEpisode
				};

				IEnumerable<EpisodeTorrentSearcherResult> results;
				List<EpisodeTorrentSearcherResult> misses;
				try {
					results = searcher.Search(episode, out misses);
				} catch(Exception e) {
					Logger.Error().Message("Error searching for episode download: " + e).Episode(episode.ID).Log();
					return;
				}

				if(!results.Any()) {
					Logger.Debug().Message("No results found").Episode(episode.ID).Log();
					Logger.Debug().Episode(episode.ID).Message("Results which did not match criteria - \n" + String.Join(", \n", misses.Select(r => DisplayResult(r)))).Log();
					return;
				}

				Logger.Debug().Message("Found results: " + String.Join(", \n", results.Select(r => DisplayResult(r)))).Episode(episode.ID).Log();

				EpisodeTorrentSearcherResult first;
				lock(downloadedLock) {
					using(var db = new EpisodeTrackerDBContext()) {
						var exclude = results.Where(r => db.EpisodeDownloadLog.Any(edl => edl.URL == r.DownloadURL.AbsolutePath)).ToArray();
						if(exclude.Any()) Logger.Debug().Message("Excluding: {0}\n{1}", exclude.Count(), String.Join(", \n", exclude.Select(e => DisplayResult(e)))).Episode(episode.ID).Log();
						results = results.Except(exclude);


						if(!results.Any()) {
							Logger.Debug().Episode(episode.ID).Message("All results excluded").Log();
							return;
						}

						first = results.First();
						Logger.Debug().Episode(episode.ID).Message("Downloading: " + DisplayResult(first)).Log();

						var ids = db.Episodes.Where(ep =>
							ep.SeriesID == episode.SeriesID
							&& (
								first.Match.Season.HasValue
								&& ep.Season == first.Match.Season
								&& (
									ep.Number == first.Match.Episode 
									|| first.Match.ToEpisode.HasValue 
									&& ep.Number > first.Match.Episode 
									&& ep.Number <= first.Match.ToEpisode.Value
								) 
								|| !first.Match.Season.HasValue 
								&& ep.AbsoluteNumber == first.Match.Episode
							)
						)
						.Select(ep => ep.ID)
						.ToArray();

						Logger.Debug().Episode(episode.ID).Message("Logging download for episode IDs: " + String.Join(", ", ids)).Log();

						foreach(var id in ids) {
							db.EpisodeDownloadLog.Add(new EpisodeDownloadLog {
								EpisodeID = id,
								Date = DateTime.Now,
								URL = first.DownloadURL.ToString()
							});
						}

						db.SaveChanges();
					}
				}

				var webClient = new CustomWebClient();
				if(!Directory.Exists("Torrents")) Directory.CreateDirectory("Torrents");

				var fileName = @"torrents\" + first.Title + ".torrent";

				try {
					webClient.DownloadFile(first.DownloadURL, fileName);
				} catch(Exception e) {
					Logger.Error().Episode(episode.ID).Message("Error downloading torrent: " + first.DownloadURL + " - " + e).Log();
					return;
				}

				Logger.Debug().Episode(episode.ID).Message("Starting process: " + fileName).Log();

				try {
					Process.Start(fileName);
				} catch(Exception e) {
					Logger.Error().Episode(episode.ID).Message("Error starting process for file: " + fileName + " - " + e).Log();
					return;
				}

				var f = Tuple.Create<Episode, EpisodeTorrentSearcherResult>(episode, first);
				lock(found) found.Add(f);

				if(DownloadFound != null) {
					DownloadFound(this, new EpisodeDownloadServiceEventArgs {
						Episodes = new ReadOnlyCollection<Tuple<Episode, EpisodeTorrentSearcherResult>>(new[] { f })
					});
				}
			});

			if(DownloadsFound != null && found.Any()) {
				DownloadsFound(this, new EpisodeDownloadServiceEventArgs {
					Episodes = new ReadOnlyCollection<Tuple<Episode,EpisodeTorrentSearcherResult>>(found)
				});
			}

			Logger.Debug("Finished checking");
		}

		string DisplayResult(EpisodeTorrentSearcherResult result) {
			return String.Format("{0} ({1}/{2}) {3}MB", result.Title, result.Seeds, result.Leechs, result.MB);
		}

		class CustomWebClient : WebClient {
			protected override WebRequest GetWebRequest(Uri address) {
				HttpWebRequest request = base.GetWebRequest(address) as HttpWebRequest;
				request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
				return request;
			}
		}
	}
}
