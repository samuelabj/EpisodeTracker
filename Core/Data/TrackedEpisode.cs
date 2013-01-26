using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpisodeTracker.Core.Data {
	public class TrackedEpisode {
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int ID { get; set; }
		public int EpisodeID { get; set; }
		public int? UserID { get; set; }
		public DateTime Added { get; set; }
		public DateTime Updated { get; set; }
		public bool Watched { get; set; }
		public int? TrackedFileID { get; set; }

		[ForeignKey("EpisodeID")]
		public virtual Episode Episode { get; set; }

		[ForeignKey("TrackedFileID")]
		public virtual TrackedFile TrackedFile { get; set; }

		[ForeignKey("UserID")]
		public virtual User User { get; set; }
	}
}