namespace justinobney.gymbuddy.api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Appointment_KudosPropertyRename : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AppointmentKudos", "Appointment_Id", "dbo.Appointments");
            DropIndex("dbo.AppointmentKudos", new[] { "Appointment_Id" });
            RenameColumn(table: "dbo.AppointmentKudos", name: "Appointment_Id", newName: "AppointmentId");
            AlterColumn("dbo.AppointmentKudos", "AppointmentId", c => c.Long(nullable: false));
            CreateIndex("dbo.AppointmentKudos", "AppointmentId");
            AddForeignKey("dbo.AppointmentKudos", "AppointmentId", "dbo.Appointments", "Id", cascadeDelete: true);
            DropColumn("dbo.AppointmentKudos", "PostId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AppointmentKudos", "PostId", c => c.Long(nullable: false));
            DropForeignKey("dbo.AppointmentKudos", "AppointmentId", "dbo.Appointments");
            DropIndex("dbo.AppointmentKudos", new[] { "AppointmentId" });
            AlterColumn("dbo.AppointmentKudos", "AppointmentId", c => c.Long());
            RenameColumn(table: "dbo.AppointmentKudos", name: "AppointmentId", newName: "Appointment_Id");
            CreateIndex("dbo.AppointmentKudos", "Appointment_Id");
            AddForeignKey("dbo.AppointmentKudos", "Appointment_Id", "dbo.Appointments", "Id");
        }
    }
}
