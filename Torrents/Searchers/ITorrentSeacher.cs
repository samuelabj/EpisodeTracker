using System;
using System.Collections.Generic;
namespace EpisodeTracker.Torrents.Searchers {
	public interface ITorrentSeacher {
		List<TorrentResult> Search(string text);
	}
}
