namespace EpisodeTracker.Core.Migrations {
	using System;
	using System.Data.Entity.Migrations;

	public partial class EnsureUniqueSeriesEpisodes : DbMigration {
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

		void AlterColumnSQLComp(string table, string name, Func<System.Data.Entity.Migrations.Builders.ColumnBuilder, System.Data.Entity.Migrations.Model.ColumnModel> columnAction) {
			RenameColumnSQLComp(table, name, name + "Temp", columnAction);
			RenameColumnSQLComp(table, name + "Temp", name, columnAction);
		}

		void RenameColumnSQLComp(string table, string name, string newName, Func<System.Data.Entity.Migrations.Builders.ColumnBuilder, System.Data.Entity.Migrations.Model.ColumnModel> columnAction) {
			AddColumn(table, newName, columnAction);
			Sql("update " + table.Replace("dbo.", "") + " set " + newName + " = " + name);
			DropColumn(table, name);
		}
	}
}
