namespace jwt.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateAccountInfoTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AccountInfoes", "PhoneNumber", c => c.String());
            DropColumn("dbo.AccountInfoes", "Password");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AccountInfoes", "Password", c => c.String());
            DropColumn("dbo.AccountInfoes", "PhoneNumber");
        }
    }
}
