using System.Collections.Generic;

namespace Telegram.Models
{
    public partial class Member
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public Role Role { get; set; }
        public ICollection<User> User { get; set; }
    }
}
