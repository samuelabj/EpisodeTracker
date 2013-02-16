using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EpisodeTracker.Core.Data;
using NLog;
using System.Data.Entity;
using System.Net;
using System.Diagnostics;
using System.IO;

namespace EpisodeTracker.Core.Models {
	public class EpisodeDownloadService : IDisposable {
		Task checkTask;
		AutoResetEvent checkEvent = new AutoResetEvent(false);

		public EpisodeDownloadService() {
			Logger = LogManager.GetLogger("EpisodeDownloadService");
		}

		public Logger Logger { get; private set; }
		public bool Running { get { return checkTask != null; } }

		public void Start() {
			checkTask = Task.Factory.StartNew(() => {
				var wait = TimeSpan.FromSeconds(5);

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
					&& ep.FileName == null
					&& ep.Aired <= DateTime.Now
					&& !ep.DownloadLog.Any()
				)
				.Include(ep => ep.Series)
				.ToList();
			}

			Logger.Debug("Found episodes to download: " + episodes.Count());

			Parallel.ForEach(episodes, episode => {
				Logger.Debug("Searching for episode: " + String.Format("{0} - S{1:00}E{2:00}", episode.Name, episode.Season, episode.Number));

				var searcher = new EpisodeTorrentSearcher {
					MinSeeds = episode.Series.DownloadMinSeeds,
					MinMB = episode.Series.DownloadMinMB,
					MaxMB = episode.Series.DownloadMaxMB,
					HD = episode.Series.DownloadHD,
					UseAbsoluteEpisodeFormat = episode.Series.DownloadUseAbsoluteEpisode
				};

				var results = searcher.Search(episode);

				Logger.Debug("Found results: " + String.Join(", \n", results.Select(r => DisplayResult(r))));

				using(var db = new EpisodeTrackerDBContext()) {
					var exclude = results.Where(r => db.EpisodeDownloadLog.Any(edl => edl.URL == r.DownloadURL.AbsolutePath)).ToArray();
					if(exclude.Any()) Logger.Debug("Excluding: {0}\n{1}", exclude.Count(), String.Join(", \n", exclude.Select(e => DisplayResult(e))));
				}

				if(results.Any()) {
					var first = results.First();
					Logger.Debug("Downloading: " + DisplayResult(first));

					var webClient = new CustomWebClient();
					if(!Directory.Exists("torrents")) Directory.CreateDirectory("torrents");

					var fileName = @"torrents\" + first.Title + ".torrent";

					try {
						webClient.DownloadFile(first.DownloadURL, fileName);
					} catch(Exception e) {
						Logger.Error("Error downloading torrent: " + first.DownloadURL + " - " + e.ToString());
						return;
					}

					Logger.Debug("Starting process: " + fileName);

					Process.Start(fileName);

					using(var db = new EpisodeTrackerDBContext()) {
						db.EpisodeDownloadLog.Add(new EpisodeDownloadLog {
							EpisodeID = episode.ID,
							Date = DateTime.Now,
							URL = first.DownloadURL.ToString()
						});
						db.SaveChanges();
					}
				} else {
					Logger.Debug("No results found");
				}
			});

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
