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
		public DbSet<TrackedFile> TrackedFile { get; set; }
		public DbSet<TrackedEpisode> TrackedEpisodes { get; set; }
		public DbSet<User> Users { get; set; }
		public DbSet<Genre> Genres { get; set; }
		public DbSet<SeriesGenre> SeriesGenres { get; set; }
		public DbSet<Setting> Settings { get; set; }
		public DbSet<SeriesAlias> SeriesAliases { get; set; }
		public DbSet<SeriesIgnore> SeriesIgnore { get; set; }
		public DbSet<EpisodeDownloadLog> EpisodeDownloadLog { get; set; }

		protected override void OnModelCreating(DbModelBuilder modelBuilder) {
			base.OnModelCreating(modelBuilder);
		}
	}
}
