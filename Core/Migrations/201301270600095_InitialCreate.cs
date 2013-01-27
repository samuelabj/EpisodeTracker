namespace EpisodeTracker.Core.Migrations {
	using System;
	using System.Data.Entity.Migrations;

	public partial class InitialCreate : DbMigration {
		public override void Up() {
			CreateTable(
				"dbo.Series",
				c => new {
					ID = c.Int(nullable: false, identity: true),
					Name = c.String(maxLength: 250),
					Added = c.DateTime(nullable: false),
					Updated = c.DateTime(nullable: false),
					Status = c.String(),
					FirstAired = c.DateTime(),
					AirsDay = c.Int(),
					AirsTime = c.DateTime(),
					LengthMinutes = c.Double(),
					Overview = c.String(),
					TVDBID = c.Int(),
					Rating = c.Double(),
				})
				.PrimaryKey(t => t.ID)
				.Index(t => t.Name, unique: true);

			CreateTable(
				"dbo.Episodes",
				c => new {
					ID = c.Int(nullable: false, identity: true),
					SeriesID = c.Int(nullable: false),
					Season = c.Int(nullable: false),
					Number = c.Int(nullable: false),
					Name = c.String(),
					Aired = c.DateTime(),
					Overview = c.String(),
					TVDBID = c.Int(),
					Added = c.DateTime(nullable: false),
					Updated = c.DateTime(nullable: false),
					AbsoluteNumber = c.Int(),
					Rating = c.Double(),
					FileName = c.String(maxLength: 500),
				})
				.PrimaryKey(t => t.ID)
				.ForeignKey("dbo.Series", t => t.SeriesID, cascadeDelete: true)
				.Index(t => t.SeriesID)
				.Index(t => new { t.SeriesID, t.Season, t.Number }, unique: true);

			CreateTable(
				"dbo.TrackedEpisodes",
				c => new {
					ID = c.Int(nullable: false, identity: true),
					EpisodeID = c.Int(nullable: false),
					UserID = c.Int(),
					Added = c.DateTime(nullable: false),
					Updated = c.DateTime(nullable: false),
					Watched = c.Boolean(nullable: false),
					TrackedFileID = c.Int(),
				})
				.PrimaryKey(t => t.ID)
				.ForeignKey("dbo.Episodes", t => t.EpisodeID, cascadeDelete: true)
				.ForeignKey("dbo.TrackedFiles", t => t.TrackedFileID)
				.ForeignKey("dbo.Users", t => t.UserID)
				.Index(t => t.EpisodeID)
				.Index(t => t.TrackedFileID)
				.Index(t => t.UserID);

			CreateTable(
				"dbo.TrackedFiles",
				c => new {
					ID = c.Int(nullable: false, identity: true),
					FileName = c.String(maxLength: 500),
					Start = c.DateTime(nullable: false),
					Stop = c.DateTime(nullable: false),
					TrackedSeconds = c.Int(nullable: false),
					LengthSeconds = c.Double(),
				})
				.PrimaryKey(t => t.ID);

			CreateTable(
				"dbo.Users",
				c => new {
					ID = c.Int(nullable: false, identity: true),
					Name = c.String(maxLength: 100),
					Added = c.DateTime(nullable: false),
				})
				.PrimaryKey(t => t.ID);

			CreateTable(
				"dbo.SeriesGenres",
				c => new {
					SeriesID = c.Int(nullable: false),
					GenreID = c.Int(nullable: false),
				})
				.PrimaryKey(t => new { t.SeriesID, t.GenreID })
				.ForeignKey("dbo.Series", t => t.SeriesID, cascadeDelete: true)
				.ForeignKey("dbo.Genres", t => t.GenreID, cascadeDelete: true)
				.Index(t => t.SeriesID)
				.Index(t => t.GenreID);

			CreateTable(
				"dbo.Genres",
				c => new {
					ID = c.Int(nullable: false, identity: true),
					Name = c.String(nullable: false, maxLength: 250),
				})
				.PrimaryKey(t => t.ID)
				.Index(t => t.Name, unique: true);

		}

		public override void Down() {
			DropIndex("dbo.SeriesGenres", new[] { "GenreID" });
			DropIndex("dbo.SeriesGenres", new[] { "SeriesID" });
			DropIndex("dbo.TrackedEpisodes", new[] { "UserID" });
			DropIndex("dbo.TrackedEpisodes", new[] { "TrackedFileID" });
			DropIndex("dbo.TrackedEpisodes", new[] { "EpisodeID" });
			DropIndex("dbo.Episodes", new[] { "SeriesID" });
			DropForeignKey("dbo.SeriesGenres", "GenreID", "dbo.Genres");
			DropForeignKey("dbo.SeriesGenres", "SeriesID", "dbo.Series");
			DropForeignKey("dbo.TrackedEpisodes", "UserID", "dbo.Users");
			DropForeignKey("dbo.TrackedEpisodes", "TrackedFileID", "dbo.TrackedFiles");
			DropForeignKey("dbo.TrackedEpisodes", "EpisodeID", "dbo.Episodes");
			DropForeignKey("dbo.Episodes", "SeriesID", "dbo.Series");
			DropTable("dbo.Genres");
			DropTable("dbo.SeriesGenres");
			DropTable("dbo.Users");
			DropTable("dbo.TrackedFiles");
			DropTable("dbo.TrackedEpisodes");
			DropTable("dbo.Episodes");
			DropTable("dbo.Series");
		}
	}
}
