using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MediaReign.TVDB {
	public class TVDBEpisode {

		public int ID { get; internal set; }
		public string[] Directors { get; internal set; }
		public string Name { get; internal set; }
		public int Number { get; internal set; }
		public DateTime? Aired { get; internal set; }
		public string[] GuestStars { get; internal set; }
		public string IMDbID { get; internal set; }
		public string Language { get; internal set; }
		public string Overview { get; internal set; }
		public double? Rating { get; internal set; }
		public int Season { get; internal set; }
		public string[] Writers { get; internal set; }
		public int? AbsoluteNumber { get; internal set; }
		public string Filename { get; internal set; }
		public DateTime LastUpdated { get; internal set; }
		public int SeasonID { get; internal set; }
		public int SeriesID { get; internal set; }

		public TVDBEpisode(XElement element) {
			ID = element.GetInt("id").Value;
			Directors = element.Split("Director");
			Name = element.Get("EpisodeName");
			Number = element.GetInt("EpisodeNumber").Value;
			Aired = element.GetDateTime("FirstAired");
			GuestStars = element.Split("GuestStars");
			IMDbID = element.Get("IMDB_ID");
			Language = element.Get("Language");
			Overview = element.Get("Overview");
			Season = element.GetInt("SeasonNumber").Value;
			Writers = element.Split("Writer");
			AbsoluteNumber = element.GetInt("absolute_number");
			Filename = element.Get("filename");
			LastUpdated = element.GetUnixDateTime("lastupdated");
			SeasonID = element.GetInt("seasonid").Value;
			SeriesID = element.GetInt("seriesid").Value;
			Rating = element.GetDouble("Rating");
		}
	}
}
