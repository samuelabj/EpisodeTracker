namespace EpisodeTracker.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MultipleTrackedEpisodes : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TrackedFiles", "EpisodeID", "dbo.Episodes");
            DropIndex("dbo.TrackedFiles", new[] { "EpisodeID" });
            CreateTable(
                "dbo.TrackedEpisodes",
                c => new
                    {
                        TrackedFileID = c.Int(nullable: false),
                        EpisodeID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.TrackedFileID, t.EpisodeID })
                .ForeignKey("dbo.TrackedFiles", t => t.TrackedFileID, cascadeDelete: true)
                .ForeignKey("dbo.Episodes", t => t.EpisodeID, cascadeDelete: true)
                .Index(t => t.TrackedFileID)
                .Index(t => t.EpisodeID);
            
            AddColumn("dbo.Series", "DurationMinutes", c => c.Double());
            AddColumn("dbo.Series", "Overview", c => c.String(maxLength: 4000));
            AddColumn("dbo.TrackedFiles", "DurationSeconds", c => c.Double());
			Sql("update trackedFiles set durationSeconds = secondsLength");
            DropColumn("dbo.TrackedFiles", "SecondsLength");
			Sql("insert trackedEpisodes (trackedFileID, episodeID) select id, episodeID from trackedFiles where episodeID is not null");
            DropColumn("dbo.TrackedFiles", "EpisodeID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TrackedFiles", "EpisodeID", c => c.Int());
            AddColumn("dbo.TrackedFiles", "SecondsLength", c => c.Double());
            DropIndex("dbo.TrackedEpisodes", new[] { "EpisodeID" });
            DropIndex("dbo.TrackedEpisodes", new[] { "TrackedFileID" });
            DropForeignKey("dbo.TrackedEpisodes", "EpisodeID", "dbo.Episodes");
            DropForeignKey("dbo.TrackedEpisodes", "TrackedFileID", "dbo.TrackedFiles");
            DropColumn("dbo.TrackedFiles", "DurationSeconds");
            DropColumn("dbo.Series", "Overview");
            DropColumn("dbo.Series", "DurationMinutes");
            DropTable("dbo.TrackedEpisodes");
            CreateIndex("dbo.TrackedFiles", "EpisodeID");
            AddForeignKey("dbo.TrackedFiles", "EpisodeID", "dbo.Episodes", "ID");
        }
    }
}
