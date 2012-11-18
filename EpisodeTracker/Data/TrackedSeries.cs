using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaReign.EpisodeTracker.Data {
	public class TrackedSeries {
		public TrackedSeries() {
			TrackedEpisodes = new List<TrackedEpisode>();
		}

		[Key, Required, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int ID { get; set; }
		public string Name { get; set; }
		public DateTime Added { get; set; }

		public virtual ICollection<TrackedEpisode> TrackedEpisodes { get; set; }
	}
}
