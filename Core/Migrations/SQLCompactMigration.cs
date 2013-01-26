namespace EpisodeTracker.Core.Migrations {
	using System;
	using System.Collections.Generic;
	using System.Configuration;
	using System.Data.Entity.Migrations;
	using System.Data.SqlServerCe;
	using System.Dynamic;

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

		protected List<dynamic> Query(string query) {
			var results = new List<dynamic>();
			using(var sqlConn = new SqlCeConnection(ConfigurationManager.ConnectionStrings["EpisodeTrackerDBContext"].ConnectionString)) {
				sqlConn.Open();

				using(var cmd = new SqlCeCommand(query, sqlConn))
				using(var reader = cmd.ExecuteReader()) {
					while(reader.Read()) {
						var fields = new Dictionary<string, object>();
						for(var i = 0; i < reader.FieldCount; i++) {
							fields.Add(reader.GetName(i), reader[i]);
						}
						results.Add(new SQLDynamicResult(fields));
					}
				}
			}
			return results;
		}

		public class SQLDynamicResult : DynamicObject {
			Dictionary<string, object> fields;

			public SQLDynamicResult(Dictionary<string, object> fields) {
				this.fields = fields;
			}

			public override bool TryGetMember(GetMemberBinder binder, out object result) {
				if(fields.ContainsKey(binder.Name)) {
					result = fields[binder.Name];
					if(result is DBNull) result = null;
					return true;
				} else {
					return base.TryGetMember(binder, out result);
				}
			}
		}
	}
}
