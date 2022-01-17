namespace DMS_Authontication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DailyandMonthly : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.DailyPermissions", "PermissionId", "dbo.Doctors_Permissions");
            DropForeignKey("dbo.MonthlyPermissions", "PermissionId", "dbo.Doctors_Permissions");
            DropIndex("dbo.DailyPermissions", new[] { "PermissionId" });
            DropIndex("dbo.MonthlyPermissions", new[] { "PermissionId" });
            DropColumn("dbo.DailyPermissions", "PermissionId");
            DropColumn("dbo.MonthlyPermissions", "PermissionId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.MonthlyPermissions", "PermissionId", c => c.Int(nullable: false));
            AddColumn("dbo.DailyPermissions", "PermissionId", c => c.Int(nullable: false));
            CreateIndex("dbo.MonthlyPermissions", "PermissionId");
            CreateIndex("dbo.DailyPermissions", "PermissionId");
            AddForeignKey("dbo.MonthlyPermissions", "PermissionId", "dbo.Doctors_Permissions", "Id", cascadeDelete: true);
            AddForeignKey("dbo.DailyPermissions", "PermissionId", "dbo.Doctors_Permissions", "Id", cascadeDelete: true);
        }
    }
}
