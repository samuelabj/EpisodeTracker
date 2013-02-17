using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EpisodeTracker.Core.Logging;

namespace EpisodeTracker.Core.Data {
	[Table("Log")]
	public class LogEntry {
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int ID { get; set; }
		[Required, MaxLength(100)]
		public string Key { get; set; }
		public LogLevel Level { get; set; }
		public DateTime Date { get; set; }
		[Required]
		public string Message { get; set; }
		public int? EpisodeID { get; set; }

		[ForeignKey("EpisodeID")]
		public virtual Episode Episode { get; set; }
	}
}
