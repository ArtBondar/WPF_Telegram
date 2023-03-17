using System;

namespace Telegram.Models
{
    public class BugReport
    {
        public int Id { get; set; }
        public string UserName { get; set; } = null;
        public string BugTitle { get; set; }
        public string BugDescription { get; set; }
        public DateTime Date { get; set; }
    }
}
