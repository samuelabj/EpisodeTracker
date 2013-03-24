using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using EpisodeTracker.Core.Data;
using EpisodeTracker.Core.Logging;
using EpisodeTracker.Torrents.Parsers;

namespace EpisodeTracker.Core.Models {
	public class EpisodeDownloader {

		Logger Logger;
		static object DownloadedLock = new object();

		public EpisodeDownloader(Episode episode) {
			Logger = Logger.Get("EpisodeDownloader");
			Episode = episode;
		}

		public Episode Episode { get; private set; }

		public EpisodeTorrentSearcherResult Download() {
			Logger.Build()
				.Message("Searching for episode")
				.Episode(Episode.ID)
				.Debug();

			IEnumerable<EpisodeTorrentSearcherResult> results;
			if(!Search(Episode, out results)) return null;

			Logger.Build()
				.Message("Found results: " + String.Join(", \n", results.Select(r => DisplayResult(r))))
				.Episode(Episode.ID)
				.Debug();

			var result = GetResult(Episode, results);
			if(result == null) return null;

			return result;
		}

		private EpisodeTorrentSearcherResult GetResult(Episode episode, IEnumerable<EpisodeTorrentSearcherResult> results) {
			lock(DownloadedLock) {
				using(var db = new EpisodeTrackerDBContext()) {
					var exclude = results.Where(r => db.EpisodeDownloadLog.Any(edl => edl.URL == r.DownloadURL.OriginalString)).ToArray();

					if(exclude.Any()) {
						Logger.Build()
							.Message("Excluding: {0}\n{1}", exclude.Count(), String.Join(", \n", exclude.Select(e => DisplayResult(e))))
							.Episode(episode.ID)
							.Debug();
					}

					results = results.Except(exclude);

					foreach(var r in results) {
						var torrent = Download(episode, r);
						LogDownload(db, episode, r);

						if(!torrent.Files.Any(f => f.Path.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))) {
							Logger.Build()
								.Episode(episode.ID)
								.Message("Using result: " + DisplayResult(r))
								.Debug();

							return r;
						}
					}

					Logger.Build()
						.Episode(episode.ID)
						.Message("All results excluded")
						.Debug();

					return null;
				}
			}
		}

		bool Search(Episode episode, out IEnumerable<EpisodeTorrentSearcherResult> results) {
			var searcher = new EpisodeTorrentSearcher {
				MinSeeds = episode.Series.DownloadMinSeeds,
				MinMB = episode.Series.DownloadMinMB,
				MaxMB = episode.Series.DownloadMaxMB,
				HD = episode.Series.DownloadHD,
				UseAbsoluteEpisodeFormat = episode.Series.DownloadUseAbsoluteEpisode
			};

			results = null;
			List<EpisodeTorrentSearcherResult> misses;
			try {
				results = searcher.Search(episode, out misses);
			} catch(Exception e) {
				Logger.Build()
					.Message("Error searching for episode download: " + e)
					.Episode(episode.ID)
					.Error();
				return false;
			}

			if(misses.Any()) {
				Logger.Build()
					.Episode(episode.ID)
					.Message("Results which did not match criteria - \n" + String.Join(", \n", misses.Select(r => DisplayResult(r))))
					.Debug();
			}

			if(!results.Any()) {
				Logger.Build()
					.Message("No results found")
					.Episode(episode.ID)
					.Debug();
				return false;
			}

			return true;
		}

		void LogDownload(EpisodeTrackerDBContext db, Episode episode, EpisodeTorrentSearcherResult result) {
			var ids = db.Episodes.Where(ep => ep.SeriesID == episode.SeriesID)
				.WhereTVMatch(result.Match)
				.Select(ep => ep.ID)
				.ToArray();

			Logger.Build()
				.Episode(episode.ID)
				.Message("Logging download for episode IDs: " + String.Join(", ", ids))
				.Debug();

			foreach(var id in ids) {
				db.EpisodeDownloadLog.Add(new EpisodeDownloadLog {
					EpisodeID = id,
					Date = DateTime.Now,
					URL = result.DownloadURL.OriginalString
				});
			}

			db.SaveChanges();
		}

		Torrent Download(Episode episode, EpisodeTorrentSearcherResult result) {
			var webClient = new CustomWebClient();
			if(!Directory.Exists("Torrents")) Directory.CreateDirectory("Torrents");

			var fileName = @"torrents\" + result.Title + ".torrent";

			try {
				webClient.DownloadFile(result.DownloadURL, fileName);
			} catch(Exception e) {
				Logger.Build().Episode(episode.ID).Message("Error downloading torrent: " + result.DownloadURL + " - " + e).Error();
				return null;
			}

			Torrent torrent;
			try {
				torrent = TorrentFileParser.Parse(fileName);
			} catch(Exception e) {
				Logger.Build().Episode(Episode.ID).Message("Error parsing torrent file: " + fileName + "-->" + e).Error();
				return null;
			}

			return torrent;
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
