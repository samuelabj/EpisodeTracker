namespace EpisodeTracker.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

	public partial class AllowNullableEpisodeProps : SQLCompactMigration
    {
        public override void Up()
        {
			AddColumn("dbo.Series", "FirstAired", c => c.DateTime());
			Sql("ALTER TABLE [Episodes] ALTER COLUMN [Aired] [datetime] null");
			Sql("ALTER TABLE [Episodes] ALTER COLUMN [AbsoluteNumber] [int] null");
			Sql("ALTER TABLE [Series] ALTER COLUMN [AirsDay] [int] null");
			Sql("ALTER TABLE [Series] ALTER COLUMN [AirsTime] [datetime] null");
        }
        
        public override void Down()
        {
			DropColumn("dbo.Series", "FirstAired");
			AlterColumnSQLComp("dbo.Episodes", "AbsoluteNumber", c => c.Int(nullable: false));
			AlterColumnSQLComp("dbo.Episodes", "Aired", c => c.DateTime(nullable: false));
			AlterColumnSQLComp("dbo.Series", "AirsDay", c => c.Int(nullable: false));
			AlterColumnSQLComp("dbo.Series", "AirsTime", c => c.DateTime(nullable: false));
        }
    }
}
