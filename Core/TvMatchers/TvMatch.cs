using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaReign.Core.TvMatchers {
	public class TvMatch {
		public string Name { get; set; }
		public int? Season { get; set; }
		public int Episode { get; set; }
		public int? ToEpisode { get; set; }
	}
}
