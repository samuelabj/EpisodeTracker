namespace EpisodeTracker.Core.Migrations {
	using System;
	using System.Data.Entity.Migrations;

	public abstract partial class SQLCompactMigration : DbMigration {
		protected void AlterColumnSQLComp(string table, string name, Func<System.Data.Entity.Migrations.Builders.ColumnBuilder, System.Data.Entity.Migrations.Model.ColumnModel> columnAction) {
			RenameColumnSQLComp(table, name, name + "Temp", columnAction);
			RenameColumnSQLComp(table, name + "Temp", name, columnAction);
		}

		protected void RenameColumnSQLComp(string table, string name, string newName, Func<System.Data.Entity.Migrations.Builders.ColumnBuilder, System.Data.Entity.Migrations.Model.ColumnModel> columnAction) {
			AddColumn(table, newName, columnAction);
			Sql("update " + table.Replace("dbo.", "") + " set " + newName + " = " + name);
			DropColumn(table, name);
		}
	}
}
