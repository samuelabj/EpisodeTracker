// <auto-generated />
namespace EpisodeTracker.Core.Migrations
{
    using System.Data.Entity.Migrations;
    using System.Data.Entity.Migrations.Infrastructure;
    using System.Resources;
    
    public sealed partial class TrackedEpisodeCleanup : IMigrationMetadata
    {
        private readonly ResourceManager Resources = new ResourceManager(typeof(TrackedEpisodeCleanup));
        
        string IMigrationMetadata.Id
        {
            get { return "201301220144479_TrackedEpisodeCleanup"; }
        }
        
        string IMigrationMetadata.Source
        {
            get { return null; }
        }
        
        string IMigrationMetadata.Target
        {
            get { return Resources.GetString("Target"); }
        }
    }
}
