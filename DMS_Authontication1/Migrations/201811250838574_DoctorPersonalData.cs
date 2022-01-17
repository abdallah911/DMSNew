namespace DMS_Authontication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DoctorPersonalData : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DoctorPersonalDatas",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        TypeId = c.String(),
                        WorkType = c.String(),
                        Speciality = c.String(),
                        Image = c.String(),
                        StampImage = c.String(),
                        BirthData = c.DateTime(),
                        Religious = c.String(),
                        Gender = c.Boolean(),
                        Nationality = c.String(),
                        SocialStatus = c.String(),
                        KidsNumbers = c.Int(),
                        MalitaryStatus = c.String(),
                        DataOfEndMalitary = c.DateTime(),
                        IdType = c.String(),
                        IdNumber = c.String(),
                        IdStartData = c.DateTime(),
                        IdExpiredData = c.DateTime(),
                        IdPlace = c.String(),
                        BloodType = c.String(),
                        User_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.UserId)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id)
                .Index(t => t.User_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DoctorPersonalDatas", "User_Id", "dbo.AspNetUsers");
            DropIndex("dbo.DoctorPersonalDatas", new[] { "User_Id" });
            DropTable("dbo.DoctorPersonalDatas");
        }
    }
}
