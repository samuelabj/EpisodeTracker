using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpisodeTracker.Core.Data {
	public class Series {
		public Series() {
			Episodes = new List<Episode>();
		}

		[Key, Required, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int ID { get; set; }
		[MaxLength(250)]
		public string Name { get; set; }
		public DateTime Added { get; set; }
		public DateTime Updated { get; set; }
		public string Status { get; set; }
		public DateTime? FirstAired { get; set; }
		public DayOfWeek? AirsDay { get; set; }
		public DateTime? AirsTime { get; set; }
		public int? TVDBID { get; set; }

		public virtual ICollection<Episode> Episodes { get; set; }
	}
}
