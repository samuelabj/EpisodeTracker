namespace EpisodeTracker.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

	public partial class AllowNullableEpisodeProps : SQLCompactMigration
    {
        public override void Up()
        {
			Sql("ALTER TABLE [Episodes] ALTER COLUMN [Aired] [datetime] null");
			Sql("ALTER TABLE [Episodes] ALTER COLUMN [AbsoluteNumber] [int] null");
        }
        
        public override void Down()
        {
			AlterColumnSQLComp("dbo.Episodes", "AbsoluteNumber", c => c.Int(nullable: false));
			AlterColumnSQLComp("dbo.Episodes", "Aired", c => c.DateTime(nullable: false));
        }
    }
}
