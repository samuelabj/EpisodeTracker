using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BencodeLibrary;

namespace EpisodeTracker.Torrents.Parsers {
	public class TorrentParser {

		public static Torrent ParseContent(string content) {
			var dict = BencodingUtils.Decode(content) as BDict;
			return Parse(dict);
		}

		public static Torrent ParseFile(string path) {
			var dict = BencodingUtils.DecodeFile(path) as BDict;
			return Parse(dict);
		}

		static Torrent Parse(BDict dict) {
			var info = dict["info"] as BDict;	
			var torrent = new Torrent();

			if(info.ContainsKey("files")) {
				var files = info["files"] as BList;

				torrent.Files.AddRange(files.Cast<BDict>().Select(f => new TorrentInfoFile {
					Path = ((f["path"] as BList).First() as BString).Value,
					Length = (f["length"] as BInt).Value
				}));
			} else {
				torrent.Files.Add(new TorrentInfoFile {
					Path = (info["name"] as BString).Value,
					Length = (info["length"] as BInt).Value
				});
			}

			return torrent;
		}
	}
}
