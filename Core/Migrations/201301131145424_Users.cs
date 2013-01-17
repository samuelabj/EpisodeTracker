namespace EpisodeTracker.Core.Migrations
{
	using System;
	using System.Collections.Generic;
	using System.Configuration;
	using System.Data.Entity.Migrations;
	using System.Data.SqlServerCe;
	using System.Text;
	using EpisodeTracker.Core.Data;
    
    public partial class Users : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TrackedEpisodes", "TrackedFileID", "dbo.TrackedFiles");
            DropIndex("dbo.TrackedEpisodes", new[] { "TrackedFileID" });
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 100),
                        Added = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            AddColumn("dbo.TrackedEpisodes", "ID", c => c.Int(nullable: false, identity: true));
            AddColumn("dbo.TrackedEpisodes", "UserID", c => c.Int());
            AddColumn("dbo.TrackedEpisodes", "DateWatched", c => c.DateTime());
            AddColumn("dbo.TrackedFiles", "Start", c => c.DateTime(nullable: false));
            AddColumn("dbo.TrackedFiles", "Stop", c => c.DateTime(nullable: false));
            AlterColumn("dbo.TrackedEpisodes", "TrackedFileID", c => c.Int());
            DropPrimaryKey("dbo.TrackedEpisodes", new[] { "TrackedFileID", "EpisodeID" });
            AddPrimaryKey("dbo.TrackedEpisodes", "ID");
            AddForeignKey("dbo.TrackedEpisodes", "TrackedFileID", "dbo.TrackedFiles", "ID");
            AddForeignKey("dbo.TrackedEpisodes", "UserID", "dbo.Users", "ID");
            CreateIndex("dbo.TrackedEpisodes", "TrackedFileID");
            CreateIndex("dbo.TrackedEpisodes", "UserID");

			var trackedFiles = new List<Tuple<int, DateTime>>();
			//Sql("--" + ConfigurationManager.ConnectionStrings["EpisodeTrackerDBContext"].ConnectionString);
			using(var sqlConn = new SqlCeConnection(ConfigurationManager.ConnectionStrings["EpisodeTrackerDBContext"].ConnectionString)) {
				sqlConn.Open();

				using(var cmd = new SqlCeCommand("select id, lastTracked from trackedFiles where probablyWatched = 1 and lastTracked is not null", sqlConn))
				using(var reader = cmd.ExecuteReader()) {
					while(reader.Read()) {
						trackedFiles.Add(Tuple.Create<int, DateTime>((int)reader["id"], (DateTime)reader["lastTracked"]));
					}
				}
			}

			foreach(var tf in trackedFiles) {
				Sql("update trackedEpisodes set dateWatched = '" + tf.Item2.ToString("yyyy-MM-dd hh:mm:ss") + "' where trackedFileID = " + tf.Item1);
			}

			Sql(@"update trackedFiles set start = added, stop = lastTracked");

            DropColumn("dbo.TrackedFiles", "Added");
            DropColumn("dbo.TrackedFiles", "LastTracked");
            DropColumn("dbo.TrackedFiles", "Watched");
            DropColumn("dbo.TrackedFiles", "ProbablyWatched");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TrackedFiles", "ProbablyWatched", c => c.Boolean(nullable: false));
            AddColumn("dbo.TrackedFiles", "Watched", c => c.Boolean(nullable: false));
            AddColumn("dbo.TrackedFiles", "LastTracked", c => c.DateTime(nullable: false));
            AddColumn("dbo.TrackedFiles", "Added", c => c.DateTime(nullable: false));
            DropIndex("dbo.TrackedEpisodes", new[] { "UserID" });
            DropIndex("dbo.TrackedEpisodes", new[] { "TrackedFileID" });
            DropForeignKey("dbo.TrackedEpisodes", "UserID", "dbo.Users");
            DropForeignKey("dbo.TrackedEpisodes", "TrackedFileID", "dbo.TrackedFiles");
            DropPrimaryKey("dbo.TrackedEpisodes", new[] { "ID" });
            AddPrimaryKey("dbo.TrackedEpisodes", new[] { "TrackedFileID", "EpisodeID" });
            AlterColumn("dbo.TrackedEpisodes", "TrackedFileID", c => c.Int(nullable: false));
            DropColumn("dbo.TrackedFiles", "Stop");
            DropColumn("dbo.TrackedFiles", "Start");
            DropColumn("dbo.TrackedEpisodes", "DateWatched");
            DropColumn("dbo.TrackedEpisodes", "UserID");
            DropColumn("dbo.TrackedEpisodes", "ID");
            DropTable("dbo.Users");
            CreateIndex("dbo.TrackedEpisodes", "TrackedFileID");
            AddForeignKey("dbo.TrackedEpisodes", "TrackedFileID", "dbo.TrackedFiles", "ID", cascadeDelete: true);
        }
    }
}
