using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EpisodeTracker.Core.Models;
using Xunit;

namespace EpisodeTracker.Tests {
	public class EpisodeFileSearcherFacts {

		[Fact]
		public void FindsFiles() {
			var path = @"D:\Downloads\Media";
			var searcher = new EpisodeFileSearcher();
			var results = searcher.Search(path);
		}
	}
}
