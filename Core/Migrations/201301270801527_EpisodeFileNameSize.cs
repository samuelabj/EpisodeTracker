namespace EpisodeTracker.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EpisodeFileNameSize : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Settings",
                c => new
                    {
                        Name = c.String(nullable: false, maxLength: 200),
                        Value = c.String(),
                    })
                .PrimaryKey(t => t.Name);
            
            AlterColumn("dbo.Series", "Status", c => c.String(maxLength: 200));
            AlterColumn("dbo.Episodes", "Name", c => c.String(maxLength: 200));
            AlterColumn("dbo.Episodes", "FileName", c => c.String(maxLength: 2000));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Episodes", "FileName", c => c.String(maxLength: 500));
            AlterColumn("dbo.Episodes", "Name", c => c.String());
            AlterColumn("dbo.Series", "Status", c => c.String());
            DropTable("dbo.Settings");
        }
    }
}
