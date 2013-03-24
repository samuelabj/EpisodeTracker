using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BencodeLibrary;

namespace EpisodeTracker.Torrents.Parsers {
	public class TorrentFileParser {
		public static Torrent Parse(string path) {
			var dict = BencodingUtils.DecodeFile(path) as BDict;
			var info = dict["info"] as BDict;
			var files = info["files"] as BList;

			var torrent = new Torrent();
			torrent.Files.AddRange(files.Cast<BDict>().Select(f => new TorrentInfoFile {
				Path = f["path"].ToString(),
				Length = (f["length"] as BInt).Value
			}));

			return torrent;
		}
	}
}
