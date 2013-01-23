using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpisodeTracker.Core.Data {
	public class SeriesGenre {
		[Key, Column(Order=1)]
		public int SeriesID { get; set; }
		[Key, Column(Order = 2)]
		public int GenreID { get; set; }

		[ForeignKey("SeriesID")]
		public virtual Series Series { get; set; }
		[ForeignKey("GenreID")]
		public virtual Genre Genre { get; set; }
	}
}
