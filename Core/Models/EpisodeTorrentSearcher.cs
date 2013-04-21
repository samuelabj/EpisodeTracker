using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using EpisodeTracker.Core.Data;
using EpisodeTracker.Core.Logging;
using EpisodeTracker.Torrents.Searchers;
using MediaReign.Core.TvMatchers;

namespace EpisodeTracker.Core.Models {
	public class EpisodeTorrentSearcher {

		public EpisodeTorrentSearcher() {
			Logger = Logger.Get("EpisodeTorrentSearcher");
		}

		public int? MinMB { get; set; }
		public int? MaxMB { get; set; }
		public int? MinSeeds { get; set; }
		public bool UseAbsoluteEpisodeFormat { get; set; }
		public bool? HD { get; set; }

		Logger Logger;


		public List<EpisodeTorrentSearcherResult> Search(Episode episode, out List<EpisodeTorrentSearcherResult> misses) {
			var searchers = new ITorrentSearcher[] {
				new KickAssTorrentsSeacher()
				//,new ISOHuntSearcher()
			};

			Logger.Build().Episode(episode).Message("Beginning search").Debug();

			string text;
			var series = Regex.Replace(episode.Series.Name, @"['\-_()]", "");
			if(!UseAbsoluteEpisodeFormat) {
				text = String.Format("{0} S{1:00}E{2:00}", series, episode.Season, episode.Number);
			} else {
				text = String.Format("{0} {1}", series, episode.AbsoluteNumber);
			}

			Logger.Build().Episode(episode).Message("Search text: " + text).Debug();

			var matcher = new TvMatcher();

			var results = searchers
				.AsParallel()
				.SelectMany(s => {
					try {
						return s.Search(text);
					} catch(Exception e) {
						Logger.Build().Episode(episode).Message("Error searching " + s.GetType().Name + ": " + e).Error();
						return new List<TorrentResult>();
					}
				})
				.Select(r => new {
					Match = matcher.Match(r.Title),
					Torrent = r
				})
				.ToList()
				.OrderByDescending(r => r.Torrent.Leechs)
				.OrderByDescending(r => r.Torrent.Seeds);
			
			var minBytes = MinMB.HasValue ? MinMB * 1024 * 1024 : (default(int?));
			var maxBytes = MaxMB.HasValue ? MaxMB * 1024 * 1024 : (default(int?));

			var matches = results
				.Where(r => {
					var entry = Logger.Build().Episode(episode);
					var msg = "Result " + r.Torrent + " {0}: {1}";

					if(r.Match == null) {
						entry.Message(msg, "does not match TV episode format", r.Torrent.Title).Debug();
						return false;
					}

					if(r.Match.Name.IndexOf(series, StringComparison.OrdinalIgnoreCase) == -1
						&& r.Match.Name.IndexOf(episode.Series.Name, StringComparison.OrdinalIgnoreCase) == -1) {
							entry.Message(msg, "does not contain series name", r.Match).Debug();
							return false;
					}

					if(!episode.Equals(r.Match, UseAbsoluteEpisodeFormat)) {
						entry.Message(msg, "match does not match episode", r.Match).Debug();
						return false;
					}

					if(r.Torrent.Seeds < MinSeeds.GetValueOrDefault(int.MinValue)) {
						entry.Message(msg, "does not match min seeds", MinSeeds).Debug();
						return false;
					}

					if(r.Torrent.Length < minBytes.GetValueOrDefault(int.MinValue)) {
						entry.Message(msg, "does not match min bytes", minBytes).Debug();
						return false;
					}

					if(r.Torrent.Length > maxBytes.GetValueOrDefault(int.MaxValue)) {
						entry.Message(msg, "does not match max bytes", maxBytes).Debug();
						return false;
					}

					if(HD.HasValue && (r.Torrent.Title.Contains("720p") || r.Torrent.Title.Contains("1080p")) != HD.Value) {
						entry.Message(msg, "does not match HD value", HD).Debug();
						return false;
					}

					return true;
				});

			misses = results
				.Except(matches)
				.Select(r => new EpisodeTorrentSearcherResult {
					Title = r.Torrent.Title,
					DownloadURL = r.Torrent.DownloadURL,
					MB = r.Torrent.Length / 1024 / 1024,
					Published = r.Torrent.Published,
					Seeds = r.Torrent.Seeds,
					Leechs = r.Torrent.Leechs,
					Match = r.Match
				})
				.ToList();

			Logger.Build()
				.Episode(episode)
				.Message("Found download results: {0} matches, {1} misses", matches.Count(), misses.Count())
				.Debug();

			return matches.Select(r => new EpisodeTorrentSearcherResult {
				Title = r.Torrent.Title,
				DownloadURL = r.Torrent.DownloadURL,
				MB = r.Torrent.Length / 1024 / 1024,
				Published = r.Torrent.Published,
				Seeds = r.Torrent.Seeds,
				Leechs = r.Torrent.Leechs,
				Match = r.Match
			})
			.ToList();
		}
	}
}
