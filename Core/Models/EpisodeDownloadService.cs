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
		public Tuple<Episode, EpisodeTorrentSearcherResult> Result { get; set; }
	}

	public class EpisodeDownloadService : IDisposable {
		Task checkTask;
		AutoResetEvent checkEvent = new AutoResetEvent(false);
		object downloadedLock = new object();

		public EpisodeDownloadService() {
			Logger = Logger.Get("EpisodeDownloadService");
		}

		public event EpisodeDownloadServiceHandler DownloadFound;

		public Logger Logger { get; private set; }
		public bool IsRunning { get { return checkTask != null; } }

		public void Start() {
			Logger.Debug("Starting EpisodeDownloadService");

			checkTask = Task.Factory.StartNew(() => {
				var wait = TimeSpan.FromMinutes(30);

				do {
					try {
						Check();
					} catch(Exception e) {
						Logger.Error("Error while checking for downloads: " + e, e);
					}

				} while(!checkEvent.WaitOne(wait));
			});

			Logger.Info("Episode download service started");
		}

		public void Stop() {
			checkEvent.Set();
			checkTask.Wait();
			checkTask = null;

			Logger.Info("Episode download service stopped");
		}

		public void Dispose() {
			if(IsRunning) Stop();
		}

		void Check() {
			Logger.Debug("Checking");

			IEnumerable<Episode> episodes;
			using(var db = new EpisodeTrackerDBContext()) {
				episodes = db.Episodes.Where(ep =>
					ep.Series.DownloadAutomatically
					&& !ep.IgnoreDownload
					&& ep.Season != 0
					&& ep.FileName == null
					&& ep.Aired <= DateTime.Now
					&& !ep.DownloadLog.Any()
					&& (
						!ep.Series.DownloadFromSeason.HasValue
						|| ep.Season >= ep.Series.DownloadFromSeason
						&& ep.Number >= ep.Series.DownloadFromEpisode
					)
				)
				.Include(ep => ep.Series)
				.ToList();
			}

			Logger.Debug("Found episodes which need to be downloaded: " + episodes.Count());

			Download(episodes);

			Logger.Debug("Finished checking");
		}

		void Download(IEnumerable<Episode> episodes) {
			var found = new List<Tuple<Episode, EpisodeTorrentSearcherResult>>();

			Parallel.ForEach(episodes, episode => {
				var downloader = new EpisodeDownloader(episode);
				var result = downloader.Download();
				if(result == null) return;

				Logger.Build()
					.Message("Found new download: {0}", result)
					.Episode(episode)
					.Info();

				if(DownloadFound != null) {
					var f = Tuple.Create<Episode, EpisodeTorrentSearcherResult>(episode, result);
					DownloadFound(this, new EpisodeDownloadServiceEventArgs {
						Result = f
					});
				}
			});
		}
	}
}
