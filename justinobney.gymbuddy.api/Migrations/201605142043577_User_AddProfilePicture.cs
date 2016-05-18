namespace justinobney.gymbuddy.api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class User_AddProfilePicture : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "ProfilePictureUrl", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "ProfilePictureUrl");
        }
    }
}
