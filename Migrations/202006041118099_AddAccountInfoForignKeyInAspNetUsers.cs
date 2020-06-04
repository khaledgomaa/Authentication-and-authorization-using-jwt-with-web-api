namespace jwt.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAccountInfoForignKeyInAspNetUsers : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "ClientId", c => c.Int(nullable: false));
            CreateIndex("dbo.AspNetUsers", "ClientId");
            AddForeignKey("dbo.AspNetUsers", "ClientId", "dbo.AccountInfoes", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUsers", "ClientId", "dbo.AccountInfoes");
            DropIndex("dbo.AspNetUsers", new[] { "ClientId" });
            DropColumn("dbo.AspNetUsers", "ClientId");
        }
    }
}
