namespace justinobney.gymbuddy.api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class User_AddGymNavigation : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Users", "Gym_Id", "dbo.Gyms");
            DropIndex("dbo.Users", new[] { "Gym_Id" });
            CreateTable(
                "dbo.UserGyms",
                c => new
                    {
                        User_Id = c.Long(nullable: false),
                        Gym_Id = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.User_Id, t.Gym_Id })
                .ForeignKey("dbo.Users", t => t.User_Id, cascadeDelete: true)
                .ForeignKey("dbo.Gyms", t => t.Gym_Id, cascadeDelete: true)
                .Index(t => t.User_Id)
                .Index(t => t.Gym_Id);
            
            DropColumn("dbo.Users", "Gym_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Users", "Gym_Id", c => c.Long());
            DropForeignKey("dbo.UserGyms", "Gym_Id", "dbo.Gyms");
            DropForeignKey("dbo.UserGyms", "User_Id", "dbo.Users");
            DropIndex("dbo.UserGyms", new[] { "Gym_Id" });
            DropIndex("dbo.UserGyms", new[] { "User_Id" });
            DropTable("dbo.UserGyms");
            CreateIndex("dbo.Users", "Gym_Id");
            AddForeignKey("dbo.Users", "Gym_Id", "dbo.Gyms", "Id");
        }
    }
}
