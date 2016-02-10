namespace justinobney.gymbuddy.api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Appointment_InitialDomainCreate : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.UserGyms", newName: "GymUsers");
            DropPrimaryKey("dbo.GymUsers");
            CreateTable(
                "dbo.Appointments",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserId = c.Long(nullable: false),
                        GymId = c.Long(),
                        Location = c.String(),
                        Status = c.Int(nullable: false),
                        ConfirmedTime = c.DateTime(),
                        CreatedAt = c.DateTime(),
                        ModifiedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AppointmentTimeSlots",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        AppointmentId = c.Long(nullable: false),
                        Time = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Appointments", t => t.AppointmentId, cascadeDelete: true)
                .Index(t => t.AppointmentId);
            
            CreateTable(
                "dbo.UserAppointments",
                c => new
                    {
                        User_Id = c.Long(nullable: false),
                        Appointment_Id = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.User_Id, t.Appointment_Id })
                .ForeignKey("dbo.Users", t => t.User_Id, cascadeDelete: true)
                .ForeignKey("dbo.Appointments", t => t.Appointment_Id, cascadeDelete: true)
                .Index(t => t.User_Id)
                .Index(t => t.Appointment_Id);
            
            AddPrimaryKey("dbo.GymUsers", new[] { "Gym_Id", "User_Id" });
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AppointmentTimeSlots", "AppointmentId", "dbo.Appointments");
            DropForeignKey("dbo.UserAppointments", "Appointment_Id", "dbo.Appointments");
            DropForeignKey("dbo.UserAppointments", "User_Id", "dbo.Users");
            DropIndex("dbo.UserAppointments", new[] { "Appointment_Id" });
            DropIndex("dbo.UserAppointments", new[] { "User_Id" });
            DropIndex("dbo.AppointmentTimeSlots", new[] { "AppointmentId" });
            DropPrimaryKey("dbo.GymUsers");
            DropTable("dbo.UserAppointments");
            DropTable("dbo.AppointmentTimeSlots");
            DropTable("dbo.Appointments");
            AddPrimaryKey("dbo.GymUsers", new[] { "User_Id", "Gym_Id" });
            RenameTable(name: "dbo.GymUsers", newName: "UserGyms");
        }
    }
}
