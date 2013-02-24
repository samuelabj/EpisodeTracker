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

		public override string ToString() {
			return String.Format("{0} {1}E{2:00}{3}", 
				Name, 
				Season.HasValue ? String.Format("S{0:00}", Season) : null, 
				Episode, 
				ToEpisode.HasValue ? String.Format("-{0:00}", ToEpisode) : null);
		}
	}
}
