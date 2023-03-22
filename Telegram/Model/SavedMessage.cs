using System;

namespace Telegram.Models
{
    public partial class SavedMessage
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int MessageId { get; set; }
    }
}
