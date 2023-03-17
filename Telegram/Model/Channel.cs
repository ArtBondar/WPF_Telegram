using System.Collections.Generic;

namespace Telegram.Models
{
    public partial class Channel
    {
        public int Id { get; set; }
        public string AboutChannel { get; set; }
        public byte[] ChannelImage { get; set; }
        public string ChannelLink { get; set; }
        public string ChannelName { get; set; }
        public bool NotificationStatus { get; set; }
        public IList<UserChannel> UserChannels { get; set; }
        public IList<ChannelMessage> ChannelMessages { get; set; }
    }
}
