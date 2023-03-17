namespace Telegram.Models
{
    public partial class UserSetting
    {
        public int UserId { get; set; }
        public User User { get; set; }
        public int SettingId { get; set; }
        public Setting Setting { get; set; }
    }
}
