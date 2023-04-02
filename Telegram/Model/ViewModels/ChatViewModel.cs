using System;

namespace Telegram.ViewModels
{
    public class ChatViewModel
    {
        public int Id { get; set; }
        public string ChatImage { get; set; }
        public string ChatName { get; set; }
        public string ShortMessage { get; set; }
        public DateTime PublishTime { get; set; }
        public bool MuteStatus { get; set; }
        public string Type { get; set; }
        public int MembersCount { get; set; }
        public int PinnedMessageId { get; set; }
        public string ChatInfo { get; set; }
        public int AuthorId { get; set; }
        public int NotViewedCounter { get; set; }
    }
}
