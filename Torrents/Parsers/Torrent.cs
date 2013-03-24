using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpisodeTracker.Torrents.Parsers {
	public class Torrent {
		public Torrent() {
			Files = new List<TorrentInfoFile>();
		}

		public List<TorrentInfoFile> Files { get; private set; }
	}
}
