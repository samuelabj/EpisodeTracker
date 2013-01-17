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
			Episodes = new List<TrackedEpisode>();
		}

		[Key, Required, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int ID { get; set; }
		[MaxLength(500)]
		public string FileName { get; set; }
		public DateTime Start { get; set; }
		public DateTime Stop { get; set; }
		public int TrackedSeconds { get; set; }
		public double? DurationSeconds { get; set; }

		[ForeignKey("TrackedFileID")]
		public virtual ICollection<TrackedEpisode> Episodes { get; set; }
	}
}
