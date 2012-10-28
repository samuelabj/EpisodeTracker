using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaReign.EpisodeTracker.Data {
	public class TrackedOther : ITrackedItem {
		[Key]
		public string FileName { get; set; }
		public DateTime Added { get; set; }
		public DateTime LastTracked { get; set; }
		public bool Watched { get; set; }
		public bool ProbablyWatched { get; set; }
		public int TrackedSeconds { get; set; }
	}
}
