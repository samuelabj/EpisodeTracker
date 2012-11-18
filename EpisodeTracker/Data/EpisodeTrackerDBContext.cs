using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaReign.EpisodeTracker.Data {
	public class EpisodeTrackerDBContext : DbContext {
		public DbSet<TrackedSeries> TrackedSeries { get; set; }
		public DbSet<TrackedEpisode> TrackedEpisodes { get; set; }
		public DbSet<TrackedOther> TrackedOthers { get; set; }

		protected override void OnModelCreating(DbModelBuilder modelBuilder) {
			modelBuilder.Entity<TrackedEpisode>()
				.HasRequired(e => e.TrackedSeries)
				.WithMany(s => s.TrackedEpisodes)
				.HasForeignKey(e => e.TrackedSeriesID);

			base.OnModelCreating(modelBuilder);
		}
	}
}
