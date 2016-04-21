namespace justinobney.gymbuddy.api.Notifications
{
    public class NofiticationTypes
    {
        public static string CreateAppointment { get; } = "CreateAppointment";
        public static string ConfirmAppointment { get; } = "ConfirmAppointment";
        public static string AddAppointmentGuest { get; } = "AddAppointmentGuest";
        public static string RemoveAppointmentGuest { get; } = "RemoveAppointmentGuest";
        public static string ConfirmAppointmentGuest { get; set; } = "ConfirmAppointmentGuest";
        public static string CancelAppointment { get; set; } = "CancelAppointment";
        public static string AppointmentOnMyWay { get; set; } = "AppointmentOnMyWay";
        public static string AddComment { get; set; } = "AddComment";
    }
}