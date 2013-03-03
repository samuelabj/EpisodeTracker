using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Web;
using System.Xml.Linq;
using ICSharpCode.SharpZipLib.Zip;
using System.IO;
using System.Diagnostics;

namespace MediaReign.TVDB {
	public class TVDBRequest {
		private string mirror;
		private const string language = "en";
		private const string api = "A1DA4CF74415C72E";

		public TVDBRequest() {
			API = api;
		}

		public string API { get; private set; }

		private string Mirror {
			get {
				if(mirror == null) {
					mirror = "http://www.thetvdb.com/";
				}
				return mirror;
			}
		}

		public LinkedList<TVDBSearchResult> Search(string name) {
			var xml = DownloadXml("GetSeries.php?seriesname={0}&language={1}", name, language);

			var results = from series in xml.Descendants("Series")
						  where series.HasElements
						  select new TVDBSearchResult {
							  ID = series.GetInt("seriesid").Value,
							  FirstAired = series.GetDateTime("FirstAired"),
							  Language = series.Get("language"),
							  Overview = series.Get("Overview"),
							  Name = series.Get("SeriesName"),
							  IMDbID = series.Get("IMDB_ID"),
							  BannerPath = series.Get("banner")
						  };

			return new LinkedList<TVDBSearchResult>(results);
		}

		public TVDBSeries Series(int id, bool zip = true) {
			var xml = DownloadXml(zip, "{0}/series/{1}/all/{2}", API, id, language);
			return new TVDBSeries(xml);			
		}

		public TVDBUpdates Updates(DateTime from, bool zip = true) {
			string timeframe;
			var diff = DateTime.Now - from;
			if(diff.TotalDays == 1) timeframe = "day";
			if(diff.TotalDays == 7) timeframe = "week";
			if(diff.TotalDays == 30) timeframe = "month";
			else timeframe = "all";

			var xml = DownloadXml(zip, "{0}/updates/updates_{1}", API, timeframe);
			return new TVDBUpdates(xml);
		}
		
		public void DownloadBanner(string path, string fileName) {
			new WebClient().DownloadFile(Mirror + "banners/" + path.TrimStart('/'), fileName);
		}

		private XDocument DownloadXml(bool zip, string request, params object[] args) {
			if(zip) {
				var data = DownloadZip(request + ".zip", args);
				return XDocument.Parse(Encoding.UTF8.GetString(data));
			} else {
				var url = BuildRequestPath(request, args);
				var data = new WebClient().DownloadString(url);
				return XDocument.Parse(data);
			}
		}

		private XDocument DownloadXml(string request, params object[] args) {
			return DownloadXml(false, request, args);
		}

		private byte[] DownloadZip(string request, params object[] args) {
			var url = BuildRequestPath(request, args);
			var data = new WebClient().DownloadData(url);

			using(var zip = new ZipInputStream(new MemoryStream(data))) {
				zip.GetNextEntry();
				var buffer = new byte[zip.Length];
				zip.Read(buffer, 0, buffer.Length);

				return buffer;
			}
		}

		private string BuildRequestPath(string request, params object[] args) {
			return Mirror + "api/" + String.Format(request, args.Select(a => (object)Uri.EscapeDataString(a.ToString())).ToArray());
		}
	}
}
