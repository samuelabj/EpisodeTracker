namespace EpisodeTracker.Core.Migrations {
	using System;
	using System.Data.Entity.Migrations;

	public partial class LogIndexes : DbMigration {
		public override void Up() {
			CreateIndex("dbo.Log", "Level");
			CreateIndex("dbo.Log", "Date");
		}

		public override void Down() {
			DropIndex("dbo.Log", new[] { "Level" });
			DropIndex("dbo.Log", new[] { "Date" });
		}
	}
}
