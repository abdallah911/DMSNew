namespace DMS_Authontication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Modification2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Doctors_Permissions", "DailyPermissionId", "dbo.DailyPermissions");
            DropForeignKey("dbo.Doctors_Permissions", "MonthlyPermissionId", "dbo.MonthlyPermissions");
            DropIndex("dbo.Doctors_Permissions", new[] { "DailyPermissionId" });
            DropIndex("dbo.Doctors_Permissions", new[] { "MonthlyPermissionId" });
            AlterColumn("dbo.Doctors_Permissions", "DailyPermissionId", c => c.Int());
            AlterColumn("dbo.Doctors_Permissions", "MonthlyPermissionId", c => c.Int());
            CreateIndex("dbo.Doctors_Permissions", "DailyPermissionId");
            CreateIndex("dbo.Doctors_Permissions", "MonthlyPermissionId");
            AddForeignKey("dbo.Doctors_Permissions", "DailyPermissionId", "dbo.DailyPermissions", "Id");
            AddForeignKey("dbo.Doctors_Permissions", "MonthlyPermissionId", "dbo.MonthlyPermissions", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Doctors_Permissions", "MonthlyPermissionId", "dbo.MonthlyPermissions");
            DropForeignKey("dbo.Doctors_Permissions", "DailyPermissionId", "dbo.DailyPermissions");
            DropIndex("dbo.Doctors_Permissions", new[] { "MonthlyPermissionId" });
            DropIndex("dbo.Doctors_Permissions", new[] { "DailyPermissionId" });
            AlterColumn("dbo.Doctors_Permissions", "MonthlyPermissionId", c => c.Int(nullable: false));
            AlterColumn("dbo.Doctors_Permissions", "DailyPermissionId", c => c.Int(nullable: false));
            CreateIndex("dbo.Doctors_Permissions", "MonthlyPermissionId");
            CreateIndex("dbo.Doctors_Permissions", "DailyPermissionId");
            AddForeignKey("dbo.Doctors_Permissions", "MonthlyPermissionId", "dbo.MonthlyPermissions", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Doctors_Permissions", "DailyPermissionId", "dbo.DailyPermissions", "Id", cascadeDelete: true);
        }
    }
}
