using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaReign.Core.TvMatchers;

namespace EpisodeTracker.Core.Models {
	public class EpisodeFileSearcher {
		public delegate void EpisodeFileSearcherResultHandler(EpisodeFileSearcher sender, EpisodeFilesSearchResultEventArgs e);

		public class EpisodeFilesSearchResultEventArgs : EventArgs {
			public int Results { get; set; }
		}

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

		public event EpisodeFileSearcherResultHandler FilesFound;

		TvMatcher matcher = new TvMatcher();

		public List<EpisodeFileSearchResult> Search(string path) {
			var task = SearchAsync(path);
			return task.Result;
		}

		public Task<List<EpisodeFileSearchResult>> SearchAsync(string path) {
			return Task.Factory.StartNew(() => {
				var results = new List<EpisodeFileSearchResult>();
				var files = Directory.GetFiles(path);

				foreach(var file in files) {
					var ext = System.IO.Path.GetExtension(file);
					if(ext.Length > 1) ext = ext.Substring(1);
					if(VideoExtensions.Contains(ext)) {
						var match = matcher.Match(System.IO.Path.GetFileName(file));
						if(match != null) {
							results.Add(new EpisodeFileSearchResult {
								FileName = file,
								Match = match
							});
						}
					}
				}

				if(FilesFound != null) FilesFound(this, new EpisodeFilesSearchResultEventArgs { Results = results.Count });

				var children = Directory.GetDirectories(path);
				var tasks = new List<Task<List<EpisodeFileSearchResult>>>();

				foreach(var child in children) {
					var info = new DirectoryInfo(child);
					if(info.Attributes.HasFlag(FileAttributes.Hidden) || info.Attributes.HasFlag(FileAttributes.System)) continue;
					var task = SearchAsync(child);
					tasks.Add(task);
				}

				Task.WaitAll(tasks.ToArray());
				results = results.Concat(tasks.SelectMany(t => t.Result)).ToList();

				return results;
			});
		}
	}
}
