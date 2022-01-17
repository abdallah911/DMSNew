namespace DMS_Authontication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Changes : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Doctors_Permissions", "PermissionId", c => c.Int(nullable: false));
            AddColumn("dbo.Doctors_Permissions", "DailyPermission_Id", c => c.Int());
            AddColumn("dbo.Doctors_Permissions", "MonthlyPermission_Id", c => c.Int());
            CreateIndex("dbo.Doctors_Permissions", "DailyPermission_Id");
            CreateIndex("dbo.Doctors_Permissions", "MonthlyPermission_Id");
            AddForeignKey("dbo.Doctors_Permissions", "DailyPermission_Id", "dbo.DailyPermissions", "Id");
            AddForeignKey("dbo.Doctors_Permissions", "MonthlyPermission_Id", "dbo.MonthlyPermissions", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Doctors_Permissions", "MonthlyPermission_Id", "dbo.MonthlyPermissions");
            DropForeignKey("dbo.Doctors_Permissions", "DailyPermission_Id", "dbo.DailyPermissions");
            DropIndex("dbo.Doctors_Permissions", new[] { "MonthlyPermission_Id" });
            DropIndex("dbo.Doctors_Permissions", new[] { "DailyPermission_Id" });
            DropColumn("dbo.Doctors_Permissions", "MonthlyPermission_Id");
            DropColumn("dbo.Doctors_Permissions", "DailyPermission_Id");
            DropColumn("dbo.Doctors_Permissions", "PermissionId");
        }
    }
}
