using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpisodeTracker.Core.Data {
	public class TrackedFile {

		[Key, Required, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int ID { get; set; }
		[MaxLength(500)]
		public string FileName { get; set; }
		public DateTime Added { get; set; }
		public DateTime LastTracked { get; set; }
		public double? SecondsLength { get; set; }
		public bool Watched { get; set; }
		public bool ProbablyWatched { get; set; }
		public int TrackedSeconds { get; set; }
		public int? EpisodeID { get; set; }


		public virtual Episode Episode { get; set; }
	}
}
