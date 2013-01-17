namespace EpisodeTracker.Core.Migrations {
	using System;
	using System.Collections.Generic;
	using System.Configuration;
	using System.Data.Entity.Migrations;
	using System.Data.SqlServerCe;
	using System.Linq;

	public partial class EnsureUniqueSeriesEpisodes : SQLCompactMigration {
		public override void Up() {
			AlterColumnSQLComp("dbo.Series", "Name", c => c.String(maxLength: 250));
			AlterColumnSQLComp("dbo.TrackedFiles", "FileName", c => c.String(maxLength: 500));

			var dups = new List<Tuple<int, int, int, int>>();
			using(var sqlConn = new SqlCeConnection(ConfigurationManager.ConnectionStrings["EpisodeTrackerDBContext"].ConnectionString)) {
				sqlConn.Open();

				using(var cmd = new SqlCeCommand("select id, seriesid, season, number from episodes e where exists (select null from episodes e2 where e2.seriesid = e.seriesid and e2.season = e.season and e2.number = e.number group by seriesid, season, number having count(*) > 1)", sqlConn))
				using(var reader = cmd.ExecuteReader()) {
					while(reader.Read()) {
						dups.Add(Tuple.Create<int, int, int, int>((int)reader["id"], (int)reader["seriesid"], (int)reader["season"], (int)reader["number"]));
					}
				}
			}

			var grouped = dups.GroupBy(d => new { d.Item2, d.Item3, d.Item4 });
			foreach(var grp in grouped) {
				foreach(var d in grp.Skip(1)) {
					Sql("update trackedFiles set episodeID = " + grp.First().Item1 + " where episodeID = " + d.Item1);
					Sql("delete from episodes where id = " + d.Item1);
				}
			}

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
