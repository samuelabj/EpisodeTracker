namespace EpisodeTracker.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ConvertToTrackedFile : DbMigration
    {
		public override void Up() {
			DropForeignKey("dbo.TrackedEpisodes", "TrackedSeriesID", "dbo.TrackedSeries");
			DropIndex("dbo.TrackedEpisodes", new[] { "TrackedSeriesID" });

			CreateTable(
				"dbo.TrackedFiles",
				c => new {
					ID = c.Int(nullable: false, identity: true),
					FileName = c.String(maxLength: 4000),
					Added = c.DateTime(nullable: false),
					LastTracked = c.DateTime(nullable: false),
					SecondsLength = c.Double(),
					Watched = c.Boolean(nullable: false),
					ProbablyWatched = c.Boolean(nullable: false),
					TrackedSeconds = c.Int(nullable: false),
					EpisodeID = c.Int(),
				})
				.PrimaryKey(t => t.ID)
				.ForeignKey("dbo.Episodes", t => t.EpisodeID)
				.Index(t => t.EpisodeID);

			Sql(@"insert trackedFiles (
	fileName
	,added
	,lastTracked
	,watched
	,probablyWatched
	,trackedSeconds
	,episodeID
)
select
	fileName
	,added
	,lastTracked
	,watched
	,probablyWatched
	,trackedSeconds
	,ID
from trackedEpisodes");

			Sql(@"insert trackedFiles (
	fileName
	,added
	,lastTracked
	,watched
	,probablyWatched
	,trackedSeconds
)
select
	fileName
	,added
	,lastTracked
	,watched
	,probablyWatched
	,trackedSeconds
from trackedOthers");

			DropTable("dbo.TrackedOthers");

			// series
			RenameTable("dbo.TrackedSeries", "Series");
			AddColumn("dbo.Series", "Status", c => c.String(maxLength: 4000));
			AddColumn("dbo.Series", "AirsDay", c => c.Int());
			AddColumn("dbo.Series", "AirsTime", c => c.DateTime());
			AddColumn("dbo.Series", "TVDBID", c => c.Int());
			AddColumn("dbo.Series", "Updated", c => c.DateTime(nullable: false));

			// episodes
			RenameTable("dbo.TrackedEpisodes", "Episodes");
			AddColumn("dbo.Episodes", "SeriesID", c => c.Int(nullable: false));
			Sql("update episodes set seriesID = trackedSeriesID");
			DropColumn("dbo.Episodes", "TrackedSeriesID");
			CreateIndex("dbo.Episodes", "SeriesID");
			AddForeignKey("dbo.Episodes", "SeriesID", "dbo.Series", "ID", cascadeDelete: true);
			AddColumn("dbo.Episodes", "Name", c => c.String(maxLength: 4000));
			AddColumn("dbo.Episodes", "Aired", c => c.DateTime(nullable: false));
			AddColumn("dbo.Episodes", "Overview", c => c.String(maxLength: 4000));
			AddColumn("dbo.Episodes", "TVDBID", c => c.Int());
			AddColumn("dbo.Episodes", "Updated", c => c.DateTime(nullable: false));
			DropColumn("dbo.Episodes", "FileName");
			DropColumn("dbo.Episodes", "LastTracked");
			DropColumn("dbo.Episodes", "Watched");
			DropColumn("dbo.Episodes", "ProbablyWatched");
			DropColumn("dbo.Episodes", "TrackedSeconds");
		}

		public override void Down() {
			CreateTable(
				"dbo.TrackedOthers",
				c => new {
					FileName = c.String(nullable: false, maxLength: 128),
					Added = c.DateTime(nullable: false),
					LastTracked = c.DateTime(nullable: false),
					Watched = c.Boolean(nullable: false),
					ProbablyWatched = c.Boolean(nullable: false),
					TrackedSeconds = c.Int(nullable: false),
				})
				.PrimaryKey(t => t.FileName);

			CreateTable(
				"dbo.TrackedEpisodes",
				c => new {
					ID = c.Int(nullable: false, identity: true),
					TrackedSeriesID = c.Int(nullable: false),
					Season = c.Int(nullable: false),
					Number = c.Int(nullable: false),
					FileName = c.String(),
					Added = c.DateTime(nullable: false),
					LastTracked = c.DateTime(nullable: false),
					Watched = c.Boolean(nullable: false),
					ProbablyWatched = c.Boolean(nullable: false),
					TrackedSeconds = c.Int(nullable: false),
				})
				.PrimaryKey(t => t.ID);

			CreateTable(
				"dbo.TrackedSeries",
				c => new {
					ID = c.Int(nullable: false, identity: true),
					Name = c.String(),
					Added = c.DateTime(nullable: false),
				})
				.PrimaryKey(t => t.ID);

			DropIndex("dbo.TrackedFiles", new[] { "EpisodeID" });
			DropIndex("dbo.Episodes", new[] { "SeriesID" });
			DropForeignKey("dbo.TrackedFiles", "EpisodeID", "dbo.Episodes");
			DropForeignKey("dbo.Episodes", "SeriesID", "dbo.Series");
			DropTable("dbo.TrackedFiles");
			DropTable("dbo.Episodes");
			DropTable("dbo.Series");
			CreateIndex("dbo.TrackedEpisodes", "TrackedSeriesID");
			AddForeignKey("dbo.TrackedEpisodes", "TrackedSeriesID", "dbo.TrackedSeries", "ID", cascadeDelete: true);
		}
    }
}
