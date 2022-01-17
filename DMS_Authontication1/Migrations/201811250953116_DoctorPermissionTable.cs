namespace DMS_Authontication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DoctorPermissionTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Doctors_Permissions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(maxLength: 128),
                        PermissionId = c.Int(nullable: false),
                        Status = c.Boolean(nullable: false),
                        permissionType = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            AddColumn("dbo.DailyPermissions", "PermissionId", c => c.Int(nullable: false));
            AddColumn("dbo.MonthlyPermissions", "PermissionId", c => c.Int(nullable: false));
            CreateIndex("dbo.DailyPermissions", "PermissionId");
            CreateIndex("dbo.MonthlyPermissions", "PermissionId");
            AddForeignKey("dbo.DailyPermissions", "PermissionId", "dbo.Doctors_Permissions", "Id", cascadeDelete: true);
            AddForeignKey("dbo.MonthlyPermissions", "PermissionId", "dbo.Doctors_Permissions", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MonthlyPermissions", "PermissionId", "dbo.Doctors_Permissions");
            DropForeignKey("dbo.DailyPermissions", "PermissionId", "dbo.Doctors_Permissions");
            DropForeignKey("dbo.Doctors_Permissions", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.MonthlyPermissions", new[] { "PermissionId" });
            DropIndex("dbo.Doctors_Permissions", new[] { "UserId" });
            DropIndex("dbo.DailyPermissions", new[] { "PermissionId" });
            DropColumn("dbo.MonthlyPermissions", "PermissionId");
            DropColumn("dbo.DailyPermissions", "PermissionId");
            DropTable("dbo.Doctors_Permissions");
        }
    }
}
