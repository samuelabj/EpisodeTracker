using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaReign.TVDB;
using Xunit;

namespace EpisodeTracker.Tests {
	public class TvDBFacts {
		[Fact]
		public void Finds_Steins_Gate() {
			var tvdb = new TVDBRequest();
			var results = tvdb.Search("Steins;Gate");

			Assert.NotEmpty(results);
		}
	}
}
