namespace justinobney.gymbuddy.api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialUser : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Devices",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserId = c.Long(nullable: false),
                        DeviceId = c.String(nullable: false),
                        CreatedAt = c.DateTime(),
                        ModifiedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(),
                        FitnessLevel = c.Int(nullable: false),
                        FilterFitnessLevel = c.Int(nullable: false),
                        Gender = c.Int(nullable: false),
                        FilterGender = c.Int(nullable: false),
                        CreatedAt = c.DateTime(),
                        ModifiedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Devices", "UserId", "dbo.Users");
            DropIndex("dbo.Devices", new[] { "UserId" });
            DropTable("dbo.Users");
            DropTable("dbo.Devices");
        }
    }
}
