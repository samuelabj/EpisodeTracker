using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaReign.Core.TvMatchers;
using Xunit;

namespace EpisodeTracker.Tests {
	public class TvMatcherFacts {
		[Fact]
		public void Finds_Name_With_Allowed_Punctuation() {
			var file = @"Z:\downloaded\[WhyNot] Steins;Gate [BD 720p AAC]\[WhyNot] Steins;Gate - 01 [BD 720p AAC][5CFFC1C7].mkv";
			var name = Path.GetFileName(file);

			var matcher = new TvMatcher();
			var match = matcher.Match(name);

			Assert.NotNull(match);
			Assert.Equal("Steins;Gate", match.Name);
		}
	}
}
