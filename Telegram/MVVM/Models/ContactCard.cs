using System.Collections.ObjectModel;
using System.Linq;

namespace Telegram.Models
{
    public class ContactCard
    {
        public string UserName { get; set; }
        public string ImageSource { get; set; }
        public ObservableCollection<MessageModel> Messages { get; set; }
        public string LastMessage => Messages.Last().Message;
        public string LastMessageTime => Messages.Last().Time.ToShortTimeString();
        public int NewMessageCount => Messages.Count;
    }
}
