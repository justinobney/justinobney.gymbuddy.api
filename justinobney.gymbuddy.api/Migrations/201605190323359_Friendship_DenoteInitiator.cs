namespace justinobney.gymbuddy.api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Friendship_DenoteInitiator : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Friendships", "Initiator", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Friendships", "Initiator");
        }
    }
}
