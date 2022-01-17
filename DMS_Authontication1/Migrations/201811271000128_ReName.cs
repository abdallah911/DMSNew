namespace DMS_Authontication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReName : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Doctors_Permissions", "DailyPermission_Id", "dbo.DailyPermissions");
            DropIndex("dbo.Doctors_Permissions", new[] { "DailyPermission_Id" });
            RenameColumn(table: "dbo.Doctors_Permissions", name: "DailyPermission_Id", newName: "DailyPermissionId");
            AlterColumn("dbo.Doctors_Permissions", "DailyPermissionId", c => c.Int(nullable: false));
            CreateIndex("dbo.Doctors_Permissions", "DailyPermissionId");
            AddForeignKey("dbo.Doctors_Permissions", "DailyPermissionId", "dbo.DailyPermissions", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Doctors_Permissions", "DailyPermissionId", "dbo.DailyPermissions");
            DropIndex("dbo.Doctors_Permissions", new[] { "DailyPermissionId" });
            AlterColumn("dbo.Doctors_Permissions", "DailyPermissionId", c => c.Int());
            RenameColumn(table: "dbo.Doctors_Permissions", name: "DailyPermissionId", newName: "DailyPermission_Id");
            CreateIndex("dbo.Doctors_Permissions", "DailyPermission_Id");
            AddForeignKey("dbo.Doctors_Permissions", "DailyPermission_Id", "dbo.DailyPermissions", "Id");
        }
    }
}
