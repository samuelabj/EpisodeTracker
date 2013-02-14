using System;
using System.Collections.Generic;
namespace EpisodeTracker.Torrents.Searchers {
	public interface ITorrentSearcher {
		List<TorrentResult> Search(string text);
	}
}
