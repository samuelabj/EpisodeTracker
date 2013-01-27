using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpisodeTracker.Core.Data {
	public class Setting {
		[MaxLength(200)]
		[Key]
		public string Name { get; set; }
		public string Value { get; set; }
	}
}
