namespace DMS_Authontication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class applicationUserNotificationChange : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.ProposalNotificationApplicationUsers", "Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ProposalNotificationApplicationUsers", "Id", c => c.Int(nullable: false));
        }
    }
}
