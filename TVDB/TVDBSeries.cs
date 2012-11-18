using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MediaReign.TVDB {
	public class TVDBSeries {

		public int ID { get; private set; }
		public string[] Actors { get; private set; }
		public DayOfWeek AirsDay { get; private set; }
		public DateTime AirsTime { get; private set; }
		public string ContentRating { get; private set; }
		public DateTime FirstAired { get; private set; }
		public string[] Genre { get; private set; }
		public string IMDbId { get; private set; }
		public string Language { get; private set; }
		public string Network { get; private set; }
		public string Overview { get; private set; }
		public double Rating { get; private set; }
		public double Runtime { get; private set; }
		public string TvDotComId { get; private set; }
		public string Name { get; private set; }
		public string Status { get; private set; }
		public string BannerPath { get; private set; }
		public string FanartPath { get; private set; }
		public DateTime LastUpdated { get; private set; }
		public string PosterPath { get; private set; }
		public string Zap2ItId { get; private set; }

		public List<TVDBEpisode> Episodes { get; private set; }
		public XDocument Xml { get; private set; }

		public TVDBSeries(XDocument xml) {
			Xml = xml;
			var series = xml.Descendants("Series").Single();

			ID = series.GetInt("id");
			Actors = series.Split("Actors");
			AirsDay = series.Get<DayOfWeek>("Airs_DayOfWeek");
			AirsTime = series.GetDateTime("Airs_Time");
			ContentRating = series.Get("ContentRating");
			FirstAired = series.GetDateTime("FirstAired");
			Genre = series.Split("Genre");
			IMDbId = series.Get("IMDB_ID");
			Language = series.Get("Language");
			Network = series.Get("Network");
			Overview = series.Get("Overview");
			Rating = series.GetDouble("Rating");
			Runtime = series.GetDouble("Runtime");
			TvDotComId = series.Get("SeriesID");
			Name = series.Get("SeriesName");
			Status = series.Get("Status");
			BannerPath = series.Get("banner");
			FanartPath = series.Get("fanart");
			LastUpdated = series.GetUnixDateTime("lastupdated");
			PosterPath = series.Get("poster");
			Zap2ItId = series.Get("zap2it_id");

			Episodes = (from ep in xml.Descendants("Episode")
						where ep.HasElements
						select new TVDBEpisode {
							ID = ep.GetInt("id"),
							Directors = ep.Split("Director"),
							Name = ep.Get("EpisodeName"),
							Number = ep.GetInt("EpisodeNumber"),
							Aired = ep.GetDateTime("FirstAired"),
							GuestStars = ep.Split("GuestStars"),
							ImDbId = ep.Get("IMDB_ID"),
							Language = ep.Get("Language"),
							Overview = ep.Get("Overview"),
							Season = ep.GetInt("SeasonNumber"),
							Writers = ep.Split("Writer"),
							AbsoluteNumber = ep.GetInt("absolute_number"),
							Filename = ep.Get("filename"),
							LastUpdated = ep.GetUnixDateTime("lastupdated"),
							SeasonId = ep.GetInt("seasonid"),
							SeriesId = ep.GetInt("seriesId")
						}).ToList();
		}
	}
}
