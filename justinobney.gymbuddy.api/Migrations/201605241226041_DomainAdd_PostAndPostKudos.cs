namespace justinobney.gymbuddy.api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DomainAdd_PostAndPostKudos : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Posts",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserId = c.Long(nullable: false),
                        Title = c.String(nullable: false),
                        Type = c.Int(nullable: false),
                        ContentJson = c.String(),
                        SkipPush = c.Boolean(nullable: false),
                        CreatedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PostKudos",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserId = c.Long(nullable: false),
                        PostId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Posts", t => t.PostId, cascadeDelete: true)
                .Index(t => new { t.UserId, t.PostId }, unique: true, name: "IX_UserPostKudos");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PostKudos", "PostId", "dbo.Posts");
            DropIndex("dbo.PostKudos", "IX_UserPostKudos");
            DropTable("dbo.PostKudos");
            DropTable("dbo.Posts");
        }
    }
}
