namespace DMS_Authontication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class applicationUserNotification : DbMigration
    {
        public override void Up()
        {
          
            
            CreateTable(
                "dbo.ProposalNotificationApplicationUsers",
                c => new
                    {
                        ApplicationUserId = c.String(nullable: false, maxLength: 128),
                        ProposalNotificationId = c.Int(nullable: false),
                        Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ApplicationUserId, t.ProposalNotificationId })
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUserId, cascadeDelete: true)
                .ForeignKey("dbo.ProposalNotifications", t => t.ProposalNotificationId, cascadeDelete: true)
                .Index(t => t.ApplicationUserId)
                .Index(t => t.ProposalNotificationId);

            
        
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProposalNotificationApplicationUsers", "ProposalNotificationId", "dbo.ProposalNotifications");
            DropForeignKey("dbo.ProposalNotificationApplicationUsers", "ApplicationUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ProposalNotificationApplicationUser1", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.ProposalNotificationApplicationUser1", "ProposalNotification_Id", "dbo.ProposalNotifications");
            DropForeignKey("dbo.ProposalOutsideMedicalAuthorities", "PricesOutsideMedicalAuthorityId", "dbo.PricesOutsideMedicalAuthorities");
            DropIndex("dbo.ProposalNotificationApplicationUser1", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.ProposalNotificationApplicationUser1", new[] { "ProposalNotification_Id" });
            DropIndex("dbo.ProposalNotificationApplicationUsers", new[] { "ProposalNotificationId" });
            DropIndex("dbo.ProposalNotificationApplicationUsers", new[] { "ApplicationUserId" });
            DropIndex("dbo.ProposalOutsideMedicalAuthorities", new[] { "PricesOutsideMedicalAuthorityId" });
            AlterColumn("dbo.Brokers", "TaxCardId", c => c.String(nullable: false));
            AlterColumn("dbo.Brokers", "MembershipNo", c => c.String(nullable: false));
            AlterColumn("dbo.ProposalMains", "AdministratorPhone", c => c.Long(nullable: false));
            AlterColumn("dbo.ProposalMains", "CompanyTelePhone", c => c.Long(nullable: false));
            DropColumn("dbo.ProposalStepTwoes", "Death");
            DropColumn("dbo.ProposalStepTwoes", "Accidents");
            DropColumn("dbo.ProposalOutsideMedicalAuthorities", "PricesOutsideMedicalAuthorityId");
            DropColumn("dbo.ProposalOutsideMedicalAuthorities", "DoctorsExaminationValue");
            DropColumn("dbo.CompanyActivities", "IsDeleted");
            DropTable("dbo.ProposalNotificationApplicationUser1");
            DropTable("dbo.ProposalNotificationApplicationUsers");
            DropTable("dbo.PricesOutsideMedicalAuthorities");
        }
    }
}
