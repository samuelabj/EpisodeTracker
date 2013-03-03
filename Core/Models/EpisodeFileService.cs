using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EpisodeTracker.Core.Data;
using EpisodeTracker.Core.Logging;

namespace EpisodeTracker.Core.Models {
	public class EpisodeFileService : Service {
		public EpisodeFileService()
			: base(TimeSpan.FromMinutes(60)) {
				Logger = Logger.Get("EpisodeFileService");
		}

		Logger Logger;

		protected override void OnInterval() {
			try {
				Check();
			} catch(Exception e) {
				Logger.Error(e.ToString());
				throw;
			}
		}

		void Check() {
			var searcher = new EpisodeFileSearcher();

			var results = Core.Models.Settings.Default.Libraries.Take(1).AsParallel()
				.Select(p => {
					try {
						return searcher.Search(p);
					} catch(Exception e) {
						Logger.Error("Error searching for episode files: " + e.ToString());
						return null;
					}
				})
				.Where(r => r != null)
				.ToList();

			var groups = results
					.SelectMany(r => r)
					.GroupBy(r => r.Match.Name, StringComparer.OrdinalIgnoreCase)
					.Select(g => new {
						SeriesName = g.Key,
						Results = g.ToList()
					})
					.OrderBy(g => g.SeriesName);

			var para = groups
				.AsParallel()
				.WithDegreeOfParallelism(5);

			para.ForAll(info => {
				using(var db = new EpisodeTrackerDBContext()) {
					var seriesName = info.SeriesName;

					if(db.SeriesIgnore.Any(s => s.Name == seriesName)) {
						return;
					}

					var series = db.Series.SingleOrDefault(s => s.Name == seriesName || s.Aliases.Any(a => a.Name == seriesName));
					if(series == null) return;

					var episodes = series.Episodes.ToList();
					foreach(var r in info.Results) {
						var eps = episodes.WhereTVMatch(r.Match);
						eps.ToList().ForEach(ep => ep.FileName = r.FileName);
					}

					db.SaveChanges();
				}
			});
		}
	}
}
