using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaReign.Core.TvMatchers;

namespace EpisodeTracker.Core.Models {
	public class EpisodeFileSearchResult {
		public string FileName { get; set; }
		public TvMatch Match { get; set; }
	}
}
