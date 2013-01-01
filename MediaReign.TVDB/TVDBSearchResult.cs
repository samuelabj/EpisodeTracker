using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MediaReign.TVDB {
	public class TVDBSearchResult {

		public int ID { get; internal set; }
		public string Language { get; internal set; }
		public string Name { get; internal set; }
		public string BannerPath { get; internal set; }
		public string Overview { get; internal set; }
		public DateTime? FirstAired { get; internal set; }
		public string IMDbID { get; internal set; }
	}
}
