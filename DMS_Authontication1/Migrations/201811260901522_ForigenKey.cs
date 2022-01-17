namespace DMS_Authontication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ForigenKey : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Doctors_Permissions", "UserId", c => c.String(maxLength: 128));
            CreateIndex("dbo.Doctors_Permissions", "UserId");
            AddForeignKey("dbo.Doctors_Permissions", "UserId", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Doctors_Permissions", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.Doctors_Permissions", new[] { "UserId" });
            AlterColumn("dbo.Doctors_Permissions", "UserId", c => c.String());
        }
    }
}
