namespace EpisodeTracker.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MoreTrackedEpisodeData : SQLCompactMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TrackedEpisodes", "Added", c => c.DateTime(nullable: false));
            AddColumn("dbo.TrackedEpisodes", "Updated", c => c.DateTime(nullable: false));
            AddColumn("dbo.TrackedEpisodes", "Watched", c => c.Boolean(nullable: false));
	
			var trackedEpisodes = Query(@"
select 
	te.ID
	,coalesce(tf.Start, te.datewatched) as Start
	,coalesce(tf.Stop, te.datewatched) as Stop
	,te.DateWatched 
from trackedEpisodes te 
left join trackedFiles tf on tf.id = te.trackedFileID");

			foreach(var te in trackedEpisodes) {
				Sql("update trackedEpisodes set added = '" + te.Start.ToString("yyyy-MM-dd hh:mm:ss") + "', updated = '" + te.Stop.ToString("yyyy-MM-dd hh:mm:ss") + "', watched = " + (te.DateWatched != null ? 1 : 0) + " where id = " + te.ID);
			}

            DropColumn("dbo.TrackedEpisodes", "DateWatched");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TrackedEpisodes", "DateWatched", c => c.DateTime());
            DropColumn("dbo.TrackedEpisodes", "Watched");
            DropColumn("dbo.TrackedEpisodes", "Updated");
            DropColumn("dbo.TrackedEpisodes", "Added");
        }
    }
}
