namespace EpisodeTracker.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Genres : DbMigration
    {
        public override void Up()
        {
			CreateTable(
				"dbo.Genres",
				c => new {
					ID = c.Int(nullable: false, identity: true),
					Name = c.String(nullable: false, maxLength: 250),
				})
				.PrimaryKey(t => t.ID)
				.Index(t => t.Name, unique: true);
            
            CreateTable(
                "dbo.SeriesGenres",
                c => new
                    {
                        SeriesID = c.Int(nullable: false),
                        GenreID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.SeriesID, t.GenreID })
                .ForeignKey("dbo.Series", t => t.SeriesID, cascadeDelete: true)
                .ForeignKey("dbo.Genres", t => t.GenreID, cascadeDelete: true)
                .Index(t => t.SeriesID)
                .Index(t => t.GenreID);
            
            AddColumn("dbo.Series", "Rating", c => c.Double(nullable: true));
            AddColumn("dbo.Episodes", "Rating", c => c.Double(nullable: true));
        }
        
        public override void Down()
        {
            DropIndex("dbo.SeriesGenres", new[] { "GenreID" });
            DropIndex("dbo.SeriesGenres", new[] { "SeriesID" });
            DropForeignKey("dbo.SeriesGenres", "GenreID", "dbo.Genres");
            DropForeignKey("dbo.SeriesGenres", "SeriesID", "dbo.Series");
            DropColumn("dbo.Episodes", "Rating");
            DropColumn("dbo.Series", "Rating");
            DropTable("dbo.SeriesGenres");
            DropTable("dbo.Genres");
        }
    }
}
