namespace justinobney.gymbuddy.api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Domain_AddNotification : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Notifications",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserId = c.Long(nullable: false),
                        Type = c.String(nullable: false),
                        JsonPayload = c.String(),
                        NotificationKey = c.String(),
                        Seen = c.Boolean(nullable: false),
                        SkipPush = c.Boolean(nullable: false),
                        TimeStamp = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Notifications");
        }
    }
}
