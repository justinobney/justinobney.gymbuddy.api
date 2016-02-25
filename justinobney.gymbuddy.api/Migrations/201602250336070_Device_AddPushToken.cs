namespace justinobney.gymbuddy.api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Device_AddPushToken : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Devices", "PushToken", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Devices", "PushToken");
        }
    }
}
