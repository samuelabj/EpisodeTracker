namespace EpisodeTracker.Core.Migrations {
	using System;
	using System.Data.Entity.Migrations;

	public partial class SeriesAliasIgnore : DbMigration {
		public override void Up() {
			CreateTable(
				"dbo.SeriesAliases",
				c => new {
					SeriesID = c.Int(nullable: false),
					Name = c.String(nullable: false, maxLength: 250),
				})
				.PrimaryKey(t => new { t.SeriesID, t.Name })
				.ForeignKey("dbo.Series", t => t.SeriesID, cascadeDelete: true)
				.Index(t => t.SeriesID)
				.Index(t => t.Name, unique: true);

			CreateTable(
				"dbo.SeriesIgnores",
				c => new {
					Name = c.String(nullable: false, maxLength: 250),
				})
				.PrimaryKey(t => t.Name);

		}

		public override void Down() {
			DropIndex("dbo.SeriesAliases", new[] { "SeriesID" });
			DropForeignKey("dbo.SeriesAliases", "SeriesID", "dbo.Series");
			DropTable("dbo.SeriesIgnores");
			DropTable("dbo.SeriesAliases");
		}
	}
}
