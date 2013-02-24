using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpisodeTracker.Torrents.Searchers {
	public class TorrentResult {
		public string Title { get; set; }
		public Uri DownloadURL { get; set; }
		public long Length { get; set; }
		public DateTime Published { get; set; }
		public long Seeds { get; set; }
		public long Leechs { get; set; }

		public override string ToString() {
			return String.Format("{0} ({1}/{2}) {3}MB - {4}", Title, Seeds, Leechs, Length, DownloadURL);
		}
	}
}
