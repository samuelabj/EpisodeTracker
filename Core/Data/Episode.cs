using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpisodeTracker.Core.Data {
	public class Episode {
		[Key, Required, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int ID { get; set; }
		
		public int SeriesID { get; set; }
		public int Season { get; set; }
		public int Number { get; set; }
		public string Name { get; set; }
		public DateTime? Aired { get; set; }
		public string Overview { get; set; }
		public int? TVDBID { get; set; }
		public DateTime Added { get; set; }
		public DateTime Updated { get; set; }
		public int? AbsoluteNumber { get; set; }

		[ForeignKey("ID")]
		public virtual Series Series { get; set; }
		[ForeignKey("EpisodeID")]
		public virtual ICollection<TrackedEpisode> Tracked { get; set; }
	}
}
