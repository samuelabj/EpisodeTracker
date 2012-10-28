using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaReign.EpisodeTracker.Data {
	public interface ITrackedItem {
		int TrackedSeconds { get; set; }
		bool Watched { get; set; }
		bool ProbablyWatched { get; set; }
		DateTime LastTracked { get; set; }
		DateTime Added { get; set; }
	}
}
