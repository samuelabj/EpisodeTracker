﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaReign.Core.TvMatchers;

namespace EpisodeTracker.Core.Models {
	public class EpisodeTorrentSearcherResult {
		public string Title { get; set; }
		public Uri DownloadURL { get; set; }
		public long MB { get; set; }
		public DateTime Published { get; set; }
		public long Seeds { get; set; }
		public long Leechs { get; set; }
		public TvMatch Match { get; set; }

		public override string ToString() {
			return String.Format("{0} ({1}/{2}) {3}MB - {4}", Title, Seeds, Leechs, MB, DownloadURL);
		}
	}
}
