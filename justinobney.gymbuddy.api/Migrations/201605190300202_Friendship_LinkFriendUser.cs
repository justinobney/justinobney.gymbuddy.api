namespace justinobney.gymbuddy.api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Friendship_LinkFriendUser : DbMigration
    {
        public override void Up()
        {
            AddForeignKey("dbo.Friendships", "FriendId", "dbo.Users", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Friendships", "FriendId", "dbo.Users");
        }
    }
}
