namespace justinobney.gymbuddy.api.Notifications
{
    public class NofiticationTypes
    {
        public static string CreateAppointment { get; } = "CreateAppointment";
        public static string ConfirmAppointment { get; } = "ConfirmAppointment";
        public static string AddAppointmentGuest { get; } = "AddAppointmentGuest";
        public static string RemoveAppointmentGuest { get; } = "RemoveAppointmentGuest";
    }
}