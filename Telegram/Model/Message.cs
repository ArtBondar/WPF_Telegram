using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Telegram.Models
{
    public partial class Message
    {
        public int Id { get; set; }
        public bool Viewed { get; set; }
        public byte[] Data { get; set; }
        [DefaultValue(false)]
        public bool DeliveryStatus { get; set; }
        public DateTime SendTime { get; set; }
        public string Text { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public IList<ChatMessage> ChatMessages { get; set; }
    }
}
