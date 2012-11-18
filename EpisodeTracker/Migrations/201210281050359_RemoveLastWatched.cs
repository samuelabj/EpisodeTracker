namespace MediaReign.EpisodeTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveLastWatched : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.TrackedSeries", "LastTracked");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TrackedSeries", "LastTracked", c => c.DateTime(nullable: false));
        }
    }
}
