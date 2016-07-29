namespace justinobney.gymbuddy.api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Device_StoreClientVersion : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Devices", "ClientVersion", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Devices", "ClientVersion");
        }
    }
}
