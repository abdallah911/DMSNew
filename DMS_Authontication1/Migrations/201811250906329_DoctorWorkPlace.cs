namespace DMS_Authontication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DoctorWorkPlace : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DoctorWorkPlaces",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(maxLength: 128),
                        TypeId = c.String(),
                        WorkPlace = c.String(),
                        Address = c.String(),
                        PermenantExpenditure = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DoctorWorkPlaces", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.DoctorWorkPlaces", new[] { "UserId" });
            DropTable("dbo.DoctorWorkPlaces");
        }
    }
}
