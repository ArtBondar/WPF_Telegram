using System.Collections.Generic;

namespace Telegram.Models
{
    public partial class Group
    {
        public int Id { get; set; }
        public int? PinnedMessage { get; set; }
        public string AboutGroup { get; set; }
        public byte[] GroupImage { get; set; }
        public string GroupLink { get; set; }
        public string GroupName { get; set; }
        public bool NotificationStatus { get; set; }
        public IList<UserGroup> UserGroups { get; set; }
        public IList<GroupMessage> GroupMessages { get; set; }
    }
}
