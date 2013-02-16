using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using EpisodeTracker.Core.Data;
using EpisodeTracker.Torrents.Searchers;
using MediaReign.Core.TvMatchers;

namespace EpisodeTracker.Core.Models {
	public class EpisodeTorrentSearcher {
		public int? MinMB { get; set; }
		public int? MaxMB { get; set; }
		public int? MinSeeds { get; set; }
		public bool UseAbsoluteEpisodeFormat { get; set; }
		public bool? HD { get; set; }

		public List<EpisodeTorrentSearcherResult> Search(Episode episode, out List<EpisodeTorrentSearcherResult> misses) {
			var searchers = new ITorrentSearcher[] {
				new KickAssTorrentsSeacher(),
				new ISOHuntSearcher()
			};

			string text;
			var series = Regex.Replace(episode.Series.Name, @"['\-_]", "");
			if(!UseAbsoluteEpisodeFormat) {
				text = String.Format("{0} S{1:00}E{2:00}", series, episode.Season, episode.Number);
			} else {
				text = String.Format("{0} {1}", series, episode.AbsoluteNumber);
			}

			var matcher = new TvMatcher();

			var results = searchers
				.AsParallel()
				.SelectMany(s => s.Search(text))
				.Select(r => new {
					Match = matcher.Match(r.Title),
					Torrent = r
				})
				.ToList();
			
			var minBytes = MinMB.HasValue ? MinMB * 1024 * 1024 : (default(int?));
			var maxBytes = MaxMB.HasValue ? MaxMB * 1024 * 1024 : (default(int?));

			var matches = results
				.Where(r =>
					r.Match != null
					&& (
						r.Match.Name.IndexOf(series, StringComparison.OrdinalIgnoreCase) > -1 
						|| r.Match.Name.IndexOf(episode.Series.Name, StringComparison.OrdinalIgnoreCase) > -1
					) && (
						UseAbsoluteEpisodeFormat
						&& !r.Match.Season.HasValue
						&& r.Match.Episode == episode.AbsoluteNumber
						|| !UseAbsoluteEpisodeFormat
						&& r.Match.Season == episode.Season
						&& r.Match.Episode == episode.Number
					)
					&& r.Torrent.Seeds >= MinSeeds.GetValueOrDefault(int.MinValue)
					&& r.Torrent.Length >= minBytes.GetValueOrDefault(int.MinValue)
					&& r.Torrent.Length <= maxBytes.GetValueOrDefault(int.MaxValue)
					&& (!HD.HasValue || r.Torrent.Title.Contains("720p") == HD.Value)
				);

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

			return matches.Select(r => new EpisodeTorrentSearcherResult {
				Title = r.Torrent.Title,
				DownloadURL = r.Torrent.DownloadURL,
				MB = r.Torrent.Length / 1024 / 1024,
				Published = r.Torrent.Published,
				Seeds = r.Torrent.Seeds,
				Leechs = r.Torrent.Leechs,
				Match = r.Match
			})
			.OrderByDescending(r => r.Leechs)
			.OrderByDescending(r => r.Seeds)
			.ToList();
		}

		public List<EpisodeTorrentSearcherResult> Search(Episode episode) {
			List<EpisodeTorrentSearcherResult> misses;
			return Search(episode, out misses);
		}

		public Task<List<EpisodeTorrentSearcherResult>> SearchAsync(Episode episode) {
			return Task.Factory.StartNew(() => Search(episode));
		}
	}
}
