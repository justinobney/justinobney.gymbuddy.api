namespace justinobney.gymbuddy.api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Domain_Rebuild : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Appointments",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserId = c.Long(nullable: false),
                        GymId = c.Long(),
                        Location = c.String(),
                        Title = c.String(),
                        Description = c.String(),
                        Status = c.Int(nullable: false),
                        ConfirmedTime = c.DateTime(),
                        CreatedAt = c.DateTime(),
                        ModifiedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.UserId)
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
            
            CreateTable(
                "dbo.GymUsers",
                c => new
                    {
                        Gym_Id = c.Long(nullable: false),
                        User_Id = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.Gym_Id, t.User_Id })
                .ForeignKey("dbo.Gyms", t => t.Gym_Id, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.User_Id, cascadeDelete: true)
                .Index(t => t.Gym_Id)
                .Index(t => t.User_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Appointments", "UserId", "dbo.Users");
            DropForeignKey("dbo.AppointmentTimeSlots", "AppointmentId", "dbo.Appointments");
            DropForeignKey("dbo.GymUsers", "User_Id", "dbo.Users");
            DropForeignKey("dbo.GymUsers", "Gym_Id", "dbo.Gyms");
            DropForeignKey("dbo.Devices", "UserId", "dbo.Users");
            DropForeignKey("dbo.UserAppointments", "Appointment_Id", "dbo.Appointments");
            DropForeignKey("dbo.UserAppointments", "User_Id", "dbo.Users");
            DropIndex("dbo.GymUsers", new[] { "User_Id" });
            DropIndex("dbo.GymUsers", new[] { "Gym_Id" });
            DropIndex("dbo.UserAppointments", new[] { "Appointment_Id" });
            DropIndex("dbo.UserAppointments", new[] { "User_Id" });
            DropIndex("dbo.AppointmentTimeSlots", new[] { "AppointmentId" });
            DropIndex("dbo.Devices", new[] { "UserId" });
            DropIndex("dbo.Appointments", new[] { "UserId" });
            DropTable("dbo.GymUsers");
            DropTable("dbo.UserAppointments");
            DropTable("dbo.AppointmentTimeSlots");
            DropTable("dbo.Gyms");
            DropTable("dbo.Devices");
            DropTable("dbo.Users");
            DropTable("dbo.Appointments");
        }
    }
}
