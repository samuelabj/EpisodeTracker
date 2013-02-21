namespace EpisodeTracker.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DownloadSeasonEpisodeRestrictions : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Series", "DownloadFromSeason", c => c.Int());
            AddColumn("dbo.Series", "DownloadFromEpisode", c => c.Int());
            AddColumn("dbo.Episodes", "IgnoreDownload", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Episodes", "IgnoreDownload");
            DropColumn("dbo.Series", "DownloadFromEpisode");
            DropColumn("dbo.Series", "DownloadFromSeason");
        }
    }
}
