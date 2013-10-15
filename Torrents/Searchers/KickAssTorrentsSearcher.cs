using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.ServiceModel.Syndication;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using System.Xml.Linq;

namespace EpisodeTracker.Torrents.Searchers {
	public class KickAssTorrentsSeacher : ITorrentSearcher {
		const string URLSearchFormat = "http://kat.ph/usearch/{0}/?rss=1&field=seeders&sorder=desc";
		
		public List<TorrentResult> Search(string text) {
			var uri = new Uri(String.Format(URLSearchFormat, HttpUtility.UrlEncode(text)));

			var request = (HttpWebRequest)WebRequest.Create(uri);
			request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;

			WebResponse response = null;
			try {
				response = request.GetResponse();
			} catch(WebException e) {
				var r = e.Response as HttpWebResponse;
				if(r != null & r.StatusCode == HttpStatusCode.NotFound) {
					return new List<TorrentResult>();
				}
				throw;
			}

			SyndicationFeed feed;
			using(var stream = response.GetResponseStream())
			using(var sr = new StreamReader(stream)) {
				var content = sr.ReadToEnd();
				content = Regex.Replace(content, "&(?!amp;)", "&amp;");
				using(var str = new StringReader(content))
				using(var xr = XmlReader.Create(str)) {
					feed = SyndicationFeed.Load(xr);
				}
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
						e.OuterName.Equals("peers", StringComparison.OrdinalIgnoreCase)
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
