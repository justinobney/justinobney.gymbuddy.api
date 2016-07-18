namespace justinobney.gymbuddy.api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class renameapiclientversion : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Devices", "ClientVersion", c => c.String());
            DropColumn("dbo.Devices", "ApiVersion");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Devices", "ApiVersion", c => c.String());
            DropColumn("dbo.Devices", "ClientVersion");
        }
    }
}
