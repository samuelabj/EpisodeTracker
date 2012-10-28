namespace MediaReign.EpisodeTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TrackedSeries",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Added = c.DateTime(nullable: false),
                        LastTracked = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.TrackedEpisodes",
                c => new
                    {
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
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.TrackedSeries", t => t.TrackedSeriesID, cascadeDelete: true)
                .Index(t => t.TrackedSeriesID);
            
            CreateTable(
                "dbo.TrackedOthers",
                c => new
                    {
                        FileName = c.String(nullable: false, maxLength: 128),
                        Added = c.DateTime(nullable: false),
                        LastTracked = c.DateTime(nullable: false),
                        Watched = c.Boolean(nullable: false),
                        ProbablyWatched = c.Boolean(nullable: false),
                        TrackedSeconds = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.FileName);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.TrackedEpisodes", new[] { "TrackedSeriesID" });
            DropForeignKey("dbo.TrackedEpisodes", "TrackedSeriesID", "dbo.TrackedSeries");
            DropTable("dbo.TrackedOthers");
            DropTable("dbo.TrackedEpisodes");
            DropTable("dbo.TrackedSeries");
        }
    }
}
