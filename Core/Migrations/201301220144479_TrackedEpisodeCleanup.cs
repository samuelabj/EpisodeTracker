namespace EpisodeTracker.Core.Migrations {
	using System;
	using System.Data.Entity.Migrations;

	public partial class TrackedEpisodeCleanup : SQLCompactMigration {
		public override void Up() {
			DropForeignKey("dbo.TrackedEpisodes", "TrackedFileID", "dbo.TrackedFiles", "ID");
			DropIndex("dbo.TrackedEpisodes", new [] { "TrackedFileID" });
			AlterColumnSQLComp("dbo.TrackedEpisodes", "TrackedFileID", c => c.Int(nullable: true));
			AddForeignKey("dbo.TrackedEpisodes", "TrackedFileID", "dbo.TrackedFiles", "ID");
			CreateIndex("dbo.TrackedEpisodes", "TrackedFileID");
		}

		public override void Down() {
			DropForeignKey("dbo.TrackedEpisodes", "TrackedFileID", "dbo.TrackedFiles", "ID");
			DropIndex("dbo.TrackedEpisodes", new[] { "TrackedFileID" });
			AlterColumnSQLComp("dbo.TrackedEpisodes", "TrackedFileID", c => c.Int(nullable: false));
			AddForeignKey("dbo.TrackedEpisodes", "TrackedFileID", "dbo.TrackedFiles", "ID");
			CreateIndex("dbo.TrackedEpisodes", "TrackedFileID");
		}
	}
}
