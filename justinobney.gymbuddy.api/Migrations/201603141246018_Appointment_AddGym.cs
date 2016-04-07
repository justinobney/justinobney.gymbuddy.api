namespace justinobney.gymbuddy.api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Appointment_AddGym : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.Appointments", "GymId");
            AddForeignKey("dbo.Appointments", "GymId", "dbo.Gyms", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Appointments", "GymId", "dbo.Gyms");
            DropIndex("dbo.Appointments", new[] { "GymId" });
        }
    }
}
