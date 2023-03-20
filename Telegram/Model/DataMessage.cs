namespace Telegram.Models
{
    public partial class DataMessage
    {
        public int Id { get; set; }
        public byte[] Data { get; set; }
        public string Text { get; set; }
        public int MessageId { get; set; }
        public Message Message { get; set; }
    }
}
