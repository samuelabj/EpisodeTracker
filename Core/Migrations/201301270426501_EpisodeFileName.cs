namespace EpisodeTracker.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EpisodeFileName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Episodes", "FileName", c => c.String(maxLength: 500));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Episodes", "FileName");
        }
    }
}
