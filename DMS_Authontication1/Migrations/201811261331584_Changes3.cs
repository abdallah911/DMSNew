namespace DMS_Authontication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Changes3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Doctors_Permissions", "permissionType", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Doctors_Permissions", "permissionType");
        }
    }
}
