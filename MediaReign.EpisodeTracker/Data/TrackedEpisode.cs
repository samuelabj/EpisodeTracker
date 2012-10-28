using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaReign.EpisodeTracker.Data {
	public class TrackedEpisode : ITrackedItem {
		[Key, Required, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int ID { get; set; }
		
		public int TrackedSeriesID { get; set; }
		public int Season { get; set; }
		public int Number { get; set; }
		public string FileName { get; set; }
		public DateTime Added { get; set; }
		public DateTime LastTracked { get; set; }
		public bool Watched { get; set; }
		public bool ProbablyWatched { get; set; }
		public int TrackedSeconds { get; set; }

		public virtual TrackedSeries TrackedSeries { get; set; }
	}
}
