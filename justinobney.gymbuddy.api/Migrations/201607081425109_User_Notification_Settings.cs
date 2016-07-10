namespace justinobney.gymbuddy.api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class User_Notification_Settings : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "NewGymWorkoutNotifications", c => c.Boolean(nullable: false));
            AddColumn("dbo.Users", "NewSquadWorkoutNotifications", c => c.Boolean(nullable: false));
            AddColumn("dbo.Users", "SilenceAllNotifications", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "SilenceAllNotifications");
            DropColumn("dbo.Users", "NewSquadWorkoutNotifications");
            DropColumn("dbo.Users", "NewGymWorkoutNotifications");
        }
    }
}
