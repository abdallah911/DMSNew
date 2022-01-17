namespace DMS_Authontication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Delete : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Doctors_Permissions", "PermissionId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Doctors_Permissions", "PermissionId", c => c.Int(nullable: false));
        }
    }
}
