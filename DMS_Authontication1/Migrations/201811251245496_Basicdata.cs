namespace DMS_Authontication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Basicdata : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Doctors_Permissions", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.Doctors_Permissions", new[] { "UserId" });
            CreateTable(
                "dbo.BASIC_DATA",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BS_CODE = c.Int(nullable: false),
                        SOURCE_MOD = c.String(),
                        COMP_ID = c.Int(nullable: false),
                        BRANCH_CODE = c.Int(nullable: false),
                        BS_CODE_UP = c.String(),
                        BS_ANAME = c.String(),
                        BS_ENAME = c.String(),
                        BS_TYPE = c.String(),
                        NOTES = c.String(),
                        ACTIVE = c.String(),
                        SERV_CODE = c.String(),
                        PHONE = c.String(),
                        EMAIL = c.String(),
                        AMT = c.Int(),
                        START_DATE = c.DateTime(),
                        END_DATE = c.DateTime(),
                        CREATED_DATE = c.DateTime(),
                        CREATED_BY = c.String(),
                        UPDATE_BY = c.String(),
                        UPDATE_DATE = c.DateTime(),
                        ISSUE_QTY = c.Int(),
                        DELV_QTY = c.Int(),
                        ADJUST_TYP = c.String(),
                        ADJUST_PERIOD = c.Int(),
                        MAX_COLLECT_TIME = c.Int(),
                        USED_TYP = c.String(),
                        USED_VAL = c.Int(),
                        USED_PERIOD = c.Int(),
                        NO_RETURN_MONTH = c.Int(),
                        USED_TYP_ADD = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AlterColumn("dbo.Doctors_Permissions", "UserId", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Doctors_Permissions", "UserId", c => c.String(maxLength: 128));
            DropTable("dbo.BASIC_DATA");
            CreateIndex("dbo.Doctors_Permissions", "UserId");
            AddForeignKey("dbo.Doctors_Permissions", "UserId", "dbo.AspNetUsers", "Id");
        }
    }
}
