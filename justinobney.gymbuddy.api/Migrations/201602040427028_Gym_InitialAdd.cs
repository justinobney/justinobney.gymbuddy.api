namespace justinobney.gymbuddy.api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Gym_InitialAdd : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Gyms",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Lat = c.Double(),
                        Lng = c.Double(),
                        Zipcode = c.String(),
                        CreatedAt = c.DateTime(),
                        ModifiedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Users", "Gym_Id", c => c.Long());
            CreateIndex("dbo.Users", "Gym_Id");
            AddForeignKey("dbo.Users", "Gym_Id", "dbo.Gyms", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Users", "Gym_Id", "dbo.Gyms");
            DropIndex("dbo.Users", new[] { "Gym_Id" });
            DropColumn("dbo.Users", "Gym_Id");
            DropTable("dbo.Gyms");
        }
    }
}
