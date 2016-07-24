namespace justinobney.gymbuddy.api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Domain_AsyncJob_InitialCreation : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AsyncJobs",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Status = c.Int(nullable: false),
                        StatusUrl = c.String(),
                        ContentUrl = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.AsyncJobs");
        }
    }
}
