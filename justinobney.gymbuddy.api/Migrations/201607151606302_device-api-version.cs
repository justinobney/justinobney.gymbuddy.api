namespace justinobney.gymbuddy.api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class deviceapiversion : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Devices", "ApiVersion", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Devices", "ApiVersion");
        }
    }
}
