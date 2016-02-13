namespace justinobney.gymbuddy.api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AppointmentGuest_AddTimeInfo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AppointmentGuests", "AppointmentTimeSlotId", c => c.Long(nullable: false));
            CreateIndex("dbo.AppointmentGuests", "AppointmentTimeSlotId");
            AddForeignKey("dbo.AppointmentGuests", "AppointmentTimeSlotId", "dbo.AppointmentTimeSlots", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AppointmentGuests", "AppointmentTimeSlotId", "dbo.AppointmentTimeSlots");
            DropIndex("dbo.AppointmentGuests", new[] { "AppointmentTimeSlotId" });
            DropColumn("dbo.AppointmentGuests", "AppointmentTimeSlotId");
        }
    }
}
