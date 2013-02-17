namespace EpisodeTracker.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DownloadAndLog : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EpisodeDownloadLog",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        EpisodeID = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        URL = c.String(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Episodes", t => t.EpisodeID, cascadeDelete: true)
                .Index(t => t.EpisodeID);

			CreateTable(
				"dbo.Log",
				c => new {
					ID = c.Int(nullable: false, identity: true),
					Key = c.String(nullable: false, maxLength: 100),
					Level = c.Byte(nullable: false),
					Date = c.DateTime(nullable: false),
					Message = c.String(nullable: false),
					EpisodeID = c.Int(),
				})
				.PrimaryKey(t => t.ID)
				.ForeignKey("dbo.Episodes", t => t.EpisodeID)
				.Index(t => t.EpisodeID)
				.Index(t => t.Key);
            
            AddColumn("dbo.Series", "DownloadAutomatically", c => c.Boolean(nullable: false));
            AddColumn("dbo.Series", "DownloadUseAbsoluteEpisode", c => c.Boolean(nullable: false));
            AddColumn("dbo.Series", "DownloadMaxMB", c => c.Int());
            AddColumn("dbo.Series", "DownloadMinMB", c => c.Int());
            AddColumn("dbo.Series", "DownloadMinSeeds", c => c.Int());
            AddColumn("dbo.Series", "DownloadHD", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropIndex("dbo.Log", new[] { "EpisodeID" });
            DropIndex("dbo.EpisodeDownloadLog", new[] { "EpisodeID" });
            DropForeignKey("dbo.Log", "EpisodeID", "dbo.Episodes");
            DropForeignKey("dbo.EpisodeDownloadLog", "EpisodeID", "dbo.Episodes");
            DropColumn("dbo.Series", "DownloadHD");
            DropColumn("dbo.Series", "DownloadMinSeeds");
            DropColumn("dbo.Series", "DownloadMinMB");
            DropColumn("dbo.Series", "DownloadMaxMB");
            DropColumn("dbo.Series", "DownloadUseAbsoluteEpisode");
            DropColumn("dbo.Series", "DownloadAutomatically");
            DropTable("dbo.Log");
            DropTable("dbo.EpisodeDownloadLog");
        }
    }
}
