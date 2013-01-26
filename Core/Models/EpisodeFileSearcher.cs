using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaReign.Core.TvMatchers;

namespace EpisodeTracker.Core.Models {
	public class EpisodeFileSearcher {
		static readonly string[] VideoExtensions = new[] {
			"avi"
			,"divx"
			,"dvr"
			,"mkv"
			,"mp4"
			,"mpeg"
			,"mpeg4"
			,"mpg"
			,"ogm"
			,"wmv"
		};

		TvMatcher matcher = new TvMatcher();

		public LinkedList<EpisodeFileSearchResult> Search(string path) {
			var results = new LinkedList<EpisodeFileSearchResult>();
			Search(path, results);
			return results;
		}

		Task Search(string path, LinkedList<EpisodeFileSearchResult> results) {
			return Task.Factory.StartNew(() => {
				var files = Directory.GetFiles(path);
				foreach(var file in files) {
					var ext = System.IO.Path.GetExtension(file);
					if(ext.Length > 1) ext = ext.Substring(1);
					if(VideoExtensions.Contains(ext)) {
						var match = matcher.Match(System.IO.Path.GetFileName(file));
						if(match != null) {
							results.AddLast(new EpisodeFileSearchResult {
								FileName = file,
								Match = match
							});
						}
					}
				}

				var children = Directory.GetDirectories(path);
				var tasks = new List<Task>();
				foreach(var child in children) {
					var task = Search(child, results);
					tasks.Add(task);
				}

				Task.WaitAll(tasks.ToArray());

			});
		}
	}
}
