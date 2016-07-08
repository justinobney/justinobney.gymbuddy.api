namespace justinobney.gymbuddy.api.Requests.Users.Dtos
{
    public class NotificationSettingDto
    {
        public bool NewGymWorkoutNotifications { get; set; }
        public bool NewSquadWorkoutNotifications { get; set; }
        public bool SilenceAllNotifications { get; set; }
    }
}