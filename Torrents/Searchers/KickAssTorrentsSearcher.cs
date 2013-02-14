using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using System.Xml.Linq;

namespace EpisodeTracker.Torrents.Searchers {
	public class KickAssTorrentsSeacher : ITorrentSearcher {
		const string URLSearchFormat = "http://kat.ph/usearch/{0}/?rss=1";
		
		public List<TorrentResult> Search(string text) {
			var uri = new Uri(String.Format(URLSearchFormat, HttpUtility.UrlEncode(text)));
			var request = WebRequest.Create(uri);

			WebResponse response = null;
			try {
				response = request.GetResponse();
			} catch(WebException e) {
				if(e.Response != null && ((HttpWebResponse)e.Response).StatusCode == HttpStatusCode.NotFound) {
					return new List<TorrentResult>();
				}
			}

			SyndicationFeed feed;
			using(var stream = response.GetResponseStream())
			using(var gzip = new GZipStream(stream, CompressionMode.Decompress))
			using(var xr = XmlReader.Create(gzip)) {
				feed = SyndicationFeed.Load(xr);
			}

			var results = feed.Items.Select(i => {
				var torrent = i.Links.Where(e => e.MediaType == "application/x-bittorrent").FirstOrDefault();
				if(torrent == null) return null;

				var seeds = long.Parse(
					i.ElementExtensions.Single(e => 
						e.OuterName.Equals("seeds", StringComparison.OrdinalIgnoreCase)
					)
					.GetObject<XElement>().Value
				);

				var leechs = long.Parse(
					i.ElementExtensions.Single(e => 
						e.OuterName.Equals("leechs", StringComparison.OrdinalIgnoreCase)
					)
					.GetObject<XElement>().Value
				);

				return new TorrentResult {
					Title = i.Title.Text,
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
