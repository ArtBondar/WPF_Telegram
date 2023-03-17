namespace Telegram.Models
{
    public partial class UserContact
    {
        public int Id { get; set; }
        public bool NotificationStatus { get; set; }
        public bool OnlineStatus { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
