namespace EpisodeTracker.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Stuff : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Series", "LengthMinutes", c => c.Double());
            AddColumn("dbo.TrackedFiles", "LengthSeconds", c => c.Double());
			Sql("update series set lengthMinutes = durationMinutes");
			Sql("update trackedFiles set durationSeconds = lengthSeconds");
            DropColumn("dbo.Series", "DurationMinutes");
            DropColumn("dbo.TrackedFiles", "DurationSeconds");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TrackedFiles", "DurationSeconds", c => c.Double());
            AddColumn("dbo.Series", "DurationMinutes", c => c.Double());
            DropColumn("dbo.TrackedFiles", "LengthSeconds");
            DropColumn("dbo.Series", "LengthMinutes");
        }
    }
}
