namespace DMS_Authontication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Changes2 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Doctors_Permissions", "PermissionId");
            DropColumn("dbo.Doctors_Permissions", "permissionType");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Doctors_Permissions", "permissionType", c => c.String());
            AddColumn("dbo.Doctors_Permissions", "PermissionId", c => c.Int(nullable: false));
        }
    }
}
