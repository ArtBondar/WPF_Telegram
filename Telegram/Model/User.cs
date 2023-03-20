using System.Collections.Generic;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Telegram.Models
{
    public class User
    {
        public int Id { get; set; }
        public int? Age { get; set; }
        public bool EmailConfirmed { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string UserName { get; set; }
        public byte[] Photo { get; set; }
        public int? MemberId { get; set; }
        public Member Member { get; set; }
        public ICollection<Chat> Chats { get; set; }
        public ICollection<SavedMessage> SavedMessages { get; set; }
        public ICollection<UserContact> UserContacts { get; set; }
        public IList<UserSetting> UserSettings { get; set; }
        public IList<UserRole> UserRoles { get; set; }
        public IList<UserGroup> UserGroups { get; set; }
        public IList<UserChannel> UserChannels { get; set; }

        public User()
        {
            
        }
    }
}
