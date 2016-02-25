namespace justinobney.gymbuddy.api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Device_AddPlatform : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Devices", "Platform", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Devices", "Platform");
        }
    }
}
