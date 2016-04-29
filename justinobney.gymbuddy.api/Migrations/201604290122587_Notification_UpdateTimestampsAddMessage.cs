namespace justinobney.gymbuddy.api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Notification_UpdateTimestampsAddMessage : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Notifications", "Message", c => c.String());
            AddColumn("dbo.Notifications", "Title", c => c.String());
            AddColumn("dbo.Notifications", "CreatedAt", c => c.DateTime());
            AddColumn("dbo.Notifications", "ProcessedAt", c => c.DateTime());
            DropColumn("dbo.Notifications", "TimeStamp");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Notifications", "TimeStamp", c => c.DateTime());
            DropColumn("dbo.Notifications", "ProcessedAt");
            DropColumn("dbo.Notifications", "CreatedAt");
            DropColumn("dbo.Notifications", "Title");
            DropColumn("dbo.Notifications", "Message");
        }
    }
}
