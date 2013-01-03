using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpisodeTracker.Core.Data {
	public class TrackedEpisode {
		[Key, Column(Order = 1), Required]
		public int TrackedFileID { get; set; }
		[Key, Column(Order = 2), Required]
		public int EpisodeID { get; set; }

		[ForeignKey("TrackedFileID")]
		public virtual TrackedFile TrackedFile { get; set; }
		[ForeignKey("EpisodeID")]
		public virtual Episode Episode { get; set; }
	}
}
