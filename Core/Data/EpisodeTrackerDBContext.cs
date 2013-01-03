using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpisodeTracker.Core.Data {
	public class EpisodeTrackerDBContext : DbContext {
		public DbSet<Series> Series { get; set; }
		public DbSet<Episode> Episodes { get; set; }
		public DbSet<TrackedFile> TrackedFiles { get; set; }

		protected override void OnModelCreating(DbModelBuilder modelBuilder) {
			//modelBuilder.Entity<Episode>()
			//	.HasRequired(e => e.Series)
			//	.WithMany(s => s.Episodes)
			//	.HasForeignKey(e => e.SeriesID);

			//modelBuilder.Entity<TrackedFile>()
			//	.HasMany(f => f.TrackedEpisodes)
			//	.WithRequired(te => te.TrackedFile)
			//	.HasForeignKey(te => te.TrackedFileID);

			//modelBuilder.Entity<TrackedEpisode>()
			//	.HasRequired(te => te.Episode)
			//	.WithMany(e => e.TrackedEpisodes)
			//	.HasForeignKey(te => te.EpisodeID);

			base.OnModelCreating(modelBuilder);
		}
	}
}
