using System;

namespace Telegram.ViewModels
{
    public class MessageViewModel
    {
        public int Id { get; set; }
        public bool Viewed { get; set; }
        public string Data { get; set; }
        public bool DeliveryStatus { get; set; }
        public DateTime SendTime { get; set; }
        public string Text { get; set; }
        public int UserId { get; set; }
    }
}
