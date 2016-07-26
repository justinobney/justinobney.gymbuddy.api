namespace justinobney.gymbuddy.api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Post_UpdateSchemaDesign : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PostComments",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        PostId = c.Long(nullable: false),
                        UserId = c.Long(nullable: false),
                        Value = c.String(),
                        Timestamp = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Posts", t => t.PostId, cascadeDelete: true)
                .Index(t => t.PostId);
            
            CreateTable(
                "dbo.PostContents",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        PostId = c.Long(nullable: false),
                        Type = c.Int(nullable: false),
                        Value = c.String(),
                        MetaJson = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Posts", t => t.PostId, cascadeDelete: true)
                .Index(t => t.PostId);
            
            AddColumn("dbo.Posts", "Timestamp", c => c.DateTime());
            DropColumn("dbo.Posts", "Title");
            DropColumn("dbo.Posts", "Type");
            DropColumn("dbo.Posts", "ContentJson");
            DropColumn("dbo.Posts", "SkipPush");
            DropColumn("dbo.Posts", "CreatedAt");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Posts", "CreatedAt", c => c.DateTime());
            AddColumn("dbo.Posts", "SkipPush", c => c.Boolean(nullable: false));
            AddColumn("dbo.Posts", "ContentJson", c => c.String());
            AddColumn("dbo.Posts", "Type", c => c.Int(nullable: false));
            AddColumn("dbo.Posts", "Title", c => c.String(nullable: false));
            DropForeignKey("dbo.PostContents", "PostId", "dbo.Posts");
            DropForeignKey("dbo.PostComments", "PostId", "dbo.Posts");
            DropIndex("dbo.PostContents", new[] { "PostId" });
            DropIndex("dbo.PostComments", new[] { "PostId" });
            DropColumn("dbo.Posts", "Timestamp");
            DropTable("dbo.PostContents");
            DropTable("dbo.PostComments");
        }
    }
}
