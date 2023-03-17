namespace Telegram.Models
{
    public partial class ChannelMessage
    {
        public int ChannelId { get; set; }
        public Channel Channel { get; set; }
        public int MessageId { get; set; }
        public Message Message { get; set; }
    }
}
