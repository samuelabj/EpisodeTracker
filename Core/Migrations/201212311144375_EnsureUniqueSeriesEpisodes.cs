namespace EpisodeTracker.Core.Migrations {
	using System;
	using System.Data.Entity.Migrations;

	public partial class EnsureUniqueSeriesEpisodes : SQLCompactMigration {
		public override void Up() {
			AlterColumnSQLComp("dbo.Series", "Name", c => c.String(maxLength: 250));
			AlterColumnSQLComp("dbo.TrackedFiles", "FileName", c => c.String(maxLength: 500));

			AddColumn("dbo.Episodes", "AbsoluteNumber", c => c.Int(nullable: false));
			CreateIndex("dbo.Episodes", new[] { "SeriesID", "Season", "Number" }, true);
			CreateIndex("dbo.Series", new[] { "Name" }, true);
			CreateIndex("dbo.TrackedFiles", new[] { "FileName" });
		}

		public override void Down() {
			DropColumn("dbo.Episodes", "AbsoluteNumber");
			DropIndex("dbo.Episodes", new[] { "SeriesID", "Season", "Number" });
			DropIndex("dbo.Series", new[] { "Name" });
			DropIndex("dbo.TrackedFiles", new[] { "FileName" });
			AlterColumnSQLComp("dbo.Series", "Name", c => c.String(maxLength: 4000));
			AlterColumnSQLComp("dbo.TrackedFiles", "FileName", c => c.String(maxLength: 4000));
		}
	}
}
