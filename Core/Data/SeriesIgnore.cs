using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpisodeTracker.Core.Data {
	public class SeriesIgnore {
		[Key, MaxLength(250)]
		public string Name { get; set; }
	}
}
