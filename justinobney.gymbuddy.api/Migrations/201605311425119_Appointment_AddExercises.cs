namespace justinobney.gymbuddy.api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Appointment_AddExercises : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AppointmentExercises",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        AppointmentId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Appointments", t => t.AppointmentId, cascadeDelete: true)
                .Index(t => t.AppointmentId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AppointmentExercises", "AppointmentId", "dbo.Appointments");
            DropIndex("dbo.AppointmentExercises", new[] { "AppointmentId" });
            DropTable("dbo.AppointmentExercises");
        }
    }
}
