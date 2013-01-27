using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaReign.TVDB;

namespace EpisodeTracker.Core.Models {
	public class TVDBSeriesSearcher {
		class SyncInfo {
			public TVDBSearchResult Result { get; set; }
			public object Lock { get; set; }
		}

		static Dictionary<string, SyncInfo> searching = new Dictionary<string, SyncInfo>(StringComparer.OrdinalIgnoreCase);

		public static TVDBSearchResult Search(string name) {
			SyncInfo info;
			lock(searching) {		
				if(searching.TryGetValue(name, out info)) {
					lock(info.Lock) {
						return info.Result;
					}
				} else {
					searching.Add(name, info = new SyncInfo {
						Lock = new object()
					});
				}
			}

			lock(info.Lock) {
				var results = new TVDBRequest().Search(name);
				info.Result = results.FirstOrDefault();
				return info.Result;
			}
		}
	}
}
