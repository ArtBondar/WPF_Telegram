using System.Collections.Generic;

namespace Telegram.Models
{
    public partial class Role
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Member> Members { get; set; }
        public IList<UserRole> UserRoles { get; set; }
    }
}
