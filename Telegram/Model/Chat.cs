using System.Collections.Generic;

namespace Telegram.Models
{
    public partial class Chat
    {
        public int Id { get; set; }
        public int OpponentId { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public IList<ChatMessage> ChatMessages { get; set; }
    }
}
