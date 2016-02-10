namespace justinobney.gymbuddy.api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Appointment_AddDescription : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Appointments", "Title", c => c.String());
            AddColumn("dbo.Appointments", "Description", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Appointments", "Description");
            DropColumn("dbo.Appointments", "Title");
        }
    }
}
