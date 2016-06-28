namespace justinobney.gymbuddy.api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class User_AddFacebookUserId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "FacebookUserId", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "FacebookUserId");
        }
    }
}
