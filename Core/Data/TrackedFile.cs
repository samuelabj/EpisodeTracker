using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpisodeTracker.Core.Data {
	public class TrackedFile {

		public TrackedFile() {
			TrackedEpisodes = new List<TrackedEpisode>();
		}

		[Key, Required, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int ID { get; set; }
		[MaxLength(500)]
		public string FileName { get; set; }
		public DateTime Added { get; set; }
		public DateTime LastTracked { get; set; }
		public double? DurationSeconds { get; set; }
		public bool Watched { get; set; }
		public bool ProbablyWatched { get; set; }
		public int TrackedSeconds { get; set; }

		[ForeignKey("TrackedFileID")]
		public virtual ICollection<TrackedEpisode> TrackedEpisodes { get; set; }
	}
}
