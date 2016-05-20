namespace justinobney.gymbuddy.api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Friendship_InitialDomainCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Friendships",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserId = c.Long(nullable: false),
                        FriendId = c.Long(nullable: false),
                        FriendshipKey = c.String(nullable: false),
                        Status = c.Int(nullable: false),
                        FriendshipDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => new { t.UserId, t.FriendId }, unique: true, name: "IX_UserFriend");
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.Friendships", "IX_UserFriend");
            DropTable("dbo.Friendships");
        }
    }
}
