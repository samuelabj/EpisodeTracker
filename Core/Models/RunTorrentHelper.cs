using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EpisodeTracker.Core.Logging;

namespace EpisodeTracker.Core.Models {
	public class RunTorrentHelper {
		public static void Run(EpisodeTorrentSearcherResult torrent) {
			var fileName = @"torrents\" + torrent.Title + ".torrent";
			Logger.Get("General").Build().Message("Starting process: " + fileName).Debug();

			Process.Start(fileName);	
		}
	}
}
