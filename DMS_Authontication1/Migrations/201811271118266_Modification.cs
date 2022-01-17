namespace DMS_Authontication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Modification : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Doctors_Permissions", "MonthlyPermission_Id", "dbo.MonthlyPermissions");
            DropIndex("dbo.Doctors_Permissions", new[] { "MonthlyPermission_Id" });
            RenameColumn(table: "dbo.Doctors_Permissions", name: "MonthlyPermission_Id", newName: "MonthlyPermissionId");
            AlterColumn("dbo.Doctors_Permissions", "MonthlyPermissionId", c => c.Int(nullable: false));
            CreateIndex("dbo.Doctors_Permissions", "MonthlyPermissionId");
            AddForeignKey("dbo.Doctors_Permissions", "MonthlyPermissionId", "dbo.MonthlyPermissions", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Doctors_Permissions", "MonthlyPermissionId", "dbo.MonthlyPermissions");
            DropIndex("dbo.Doctors_Permissions", new[] { "MonthlyPermissionId" });
            AlterColumn("dbo.Doctors_Permissions", "MonthlyPermissionId", c => c.Int());
            RenameColumn(table: "dbo.Doctors_Permissions", name: "MonthlyPermissionId", newName: "MonthlyPermission_Id");
            CreateIndex("dbo.Doctors_Permissions", "MonthlyPermission_Id");
            AddForeignKey("dbo.Doctors_Permissions", "MonthlyPermission_Id", "dbo.MonthlyPermissions", "Id");
        }
    }
}
