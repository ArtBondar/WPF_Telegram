using System.Collections.Generic;

namespace Telegram.Models
{
    public partial class Setting
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IList<UserSetting> UserSettings { get; set; }
    }
}
