namespace justinobney.gymbuddy.api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Post_UserUpdate : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.Posts", "UserId");
            AddForeignKey("dbo.Posts", "UserId", "dbo.Users", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Posts", "UserId", "dbo.Users");
            DropIndex("dbo.Posts", new[] { "UserId" });
        }
    }
}
