namespace justinobney.gymbuddy.api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Appointment_AddComments : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AppointmentComments",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserId = c.Long(nullable: false),
                        AppointmentId = c.Long(nullable: false),
                        Text = c.String(),
                        CreatedAt = c.DateTime(),
                        ModifiedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.UserId)
                .ForeignKey("dbo.Appointments", t => t.AppointmentId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.AppointmentId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AppointmentComments", "AppointmentId", "dbo.Appointments");
            DropForeignKey("dbo.AppointmentComments", "UserId", "dbo.Users");
            DropIndex("dbo.AppointmentComments", new[] { "AppointmentId" });
            DropIndex("dbo.AppointmentComments", new[] { "UserId" });
            DropTable("dbo.AppointmentComments");
        }
    }
}
