namespace justinobney.gymbuddy.api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Appointment_AddKudos : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AppointmentKudos",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserId = c.Long(nullable: false),
                        PostId = c.Long(nullable: false),
                        Appointment_Id = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Appointments", t => t.Appointment_Id)
                .Index(t => t.Appointment_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AppointmentKudos", "Appointment_Id", "dbo.Appointments");
            DropIndex("dbo.AppointmentKudos", new[] { "Appointment_Id" });
            DropTable("dbo.AppointmentKudos");
        }
    }
}
