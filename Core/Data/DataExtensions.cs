using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaReign.Core.TvMatchers;

namespace EpisodeTracker.Core.Data {
	public static class DataExtensions {
		public static IQueryable<Episode> WhereTVMatch(this IQueryable<Episode> source, TvMatch match) {
			return source.Where(Episode.EqualsMatchExpression(match)); 
		}
	}
}
