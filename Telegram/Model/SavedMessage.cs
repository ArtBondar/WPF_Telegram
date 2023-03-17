using System;

namespace Telegram.Models
{
    public partial class SavedMessage
    {
        public int Id { get; set; }
        public DateTime CreationTime { get; set; }
        public bool IsViewed { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
