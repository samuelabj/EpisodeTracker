using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpisodeTracker.Core.Data {
	[Table("SeriesAliases")]
	public class SeriesAlias {
		[Key, Column(Order=1)]
		public int SeriesID { get; set; }

		[Key, Column(Order=2), MaxLength(250)]
		public string Name { get; set; }

		[ForeignKey("SeriesID")]
		public virtual Series Series { get; set; }
	}
}
