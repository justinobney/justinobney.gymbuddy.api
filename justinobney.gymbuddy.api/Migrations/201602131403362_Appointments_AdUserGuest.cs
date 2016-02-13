namespace justinobney.gymbuddy.api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Appointments_AdUserGuest : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.UserAppointments", "User_Id", "dbo.Users");
            DropForeignKey("dbo.UserAppointments", "Appointment_Id", "dbo.Appointments");
            DropForeignKey("dbo.Appointments", "UserId", "dbo.Users");
            DropIndex("dbo.UserAppointments", new[] { "User_Id" });
            DropIndex("dbo.UserAppointments", new[] { "Appointment_Id" });
            CreateTable(
                "dbo.AppointmentGuests",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserId = c.Long(nullable: false),
                        AppointmentId = c.Long(nullable: false),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.UserId)
                .ForeignKey("dbo.Appointments", t => t.AppointmentId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.AppointmentId);
            
            AddForeignKey("dbo.Appointments", "UserId", "dbo.Users", "Id", cascadeDelete: true);
            DropTable("dbo.UserAppointments");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.UserAppointments",
                c => new
                    {
                        User_Id = c.Long(nullable: false),
                        Appointment_Id = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.User_Id, t.Appointment_Id });
            
            DropForeignKey("dbo.Appointments", "UserId", "dbo.Users");
            DropForeignKey("dbo.AppointmentGuests", "AppointmentId", "dbo.Appointments");
            DropForeignKey("dbo.AppointmentGuests", "UserId", "dbo.Users");
            DropIndex("dbo.AppointmentGuests", new[] { "AppointmentId" });
            DropIndex("dbo.AppointmentGuests", new[] { "UserId" });
            DropTable("dbo.AppointmentGuests");
            CreateIndex("dbo.UserAppointments", "Appointment_Id");
            CreateIndex("dbo.UserAppointments", "User_Id");
            AddForeignKey("dbo.Appointments", "UserId", "dbo.Users", "Id");
            AddForeignKey("dbo.UserAppointments", "Appointment_Id", "dbo.Appointments", "Id", cascadeDelete: true);
            AddForeignKey("dbo.UserAppointments", "User_Id", "dbo.Users", "Id", cascadeDelete: true);
        }
    }
}
