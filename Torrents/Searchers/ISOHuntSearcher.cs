using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel.Syndication;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace EpisodeTracker.Torrents.Searchers {
	public class ISOHuntSearcher : ITorrentSearcher {
		const string URLSearchFormat = "http://isohunt.com/js/rss/{0}?sort=seeds";

		public List<TorrentResult> Search(string text) {
			var uri = new Uri(String.Format(URLSearchFormat, HttpUtility.UrlEncode(text)));
			var request = WebRequest.Create(uri);
			var response = request.GetResponse();

			SyndicationFeed feed;
			using(var stream = response.GetResponseStream())
			using(var xr = XmlReader.Create(stream)) {
				feed = SyndicationFeed.Load(xr);
			}

			var results = feed.Items.Select(i => {
				var torrent = i.Links.Where(e => e.MediaType == "application/x-bittorrent").FirstOrDefault();
				if(torrent == null) return null;

				var slMatch = Regex.Match(i.Title.Text, @"^(.*)\[(\d+)/(\d+)\]$");
				long seeds = 0;
				long leechs = 0;
				var title = i.Title.Text;

				if(slMatch.Success) {
					title = slMatch.Groups[1].Value.Trim();
					seeds = long.Parse(slMatch.Groups[2].Value);
					leechs = long.Parse(slMatch.Groups[3].Value);
				}

				return new TorrentResult {
					Title = title,
					Published = i.PublishDate.DateTime,
					DownloadURL = torrent.Uri,
					Length = torrent.Length,
					Seeds = seeds,
					Leechs = leechs
				};
			})
			.Where(r => r != null);

			return results.ToList();
		}
	}
}
