namespace DMS_Authontication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class typeid : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "TypeId", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "TypeId");
        }
    }
}
