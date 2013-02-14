using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpisodeTracker.Core.Data {
	[Table("dbo.EpisodeDownloadLog")]
	public class EpisodeDownloadLog {
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int ID { get; set; }
		public int EpisodeID { get; set; }
		public DateTime Date { get; set; }
		public string URL { get; set; }

		[ForeignKey("EpisodeID")]
		public virtual Episode Episode { get; set; }
	}
}
