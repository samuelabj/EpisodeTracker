using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MediaReign.TVDB {
	public class TVDBEpisode {

		public int ID { get; internal set; }
		public string[] Directors { get; internal set; }
		public string Name { get; internal set; }
		public int Number { get; internal set; }
		public DateTime Aired { get; internal set; }
		public string[] GuestStars { get; internal set; }
		public string ImDbId { get; internal set; }
		public string Language { get; internal set; }
		public string Overview { get; internal set; }
		public double Rating { get; internal set; }
		public int Season { get; internal set; }
		public string[] Writers { get; internal set; }
		public int AbsoluteNumber { get; internal set; }
		public string Filename { get; internal set; }
		public DateTime LastUpdated { get; internal set; }
		public int SeasonId { get; internal set; }
		public int SeriesId { get; internal set; }
	}
}
