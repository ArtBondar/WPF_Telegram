using System.Collections.Generic;

namespace Telegram.Models
{
    public partial class Message
    {
        public int Id { get; set; }
        public int Viewed { get; set; }
        public DataMessage DataMessage { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public IList<GroupMessage> GroupMessages { get; set; }
        public IList<ChatMessage> ChatMessages { get; set; }
        public IList<ChannelMessage> ChannelMessages { get; set; }
    }
}
