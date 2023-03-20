using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Telegram.Models;

namespace Telegram.Model.UI
{
    public class UIChat
    {
        public string Name { get; set; }
        public string LastMessage { get; set; }
        public ImageSource ImageSource { get; set; }
        public DateTime Time { get; set; }
        public UIChat()
        {

        }
        public UIChat(Chat chat)
        {
            Name = chat.User.UserName;
            LastMessage = Encoding.Default.GetString(chat.ChatMessages.Last().Message.DataMessage.Data);
            Time = DateTime.Now;
            ImageSource = BitmapFrame.Create(new MemoryStream(chat.User.Photo));
        }
        public UIChat(Channel channel)
        {
            Name = channel.ChannelName;
            LastMessage = Encoding.Default.GetString(channel.ChannelMessages.Last().Message.DataMessage.Data);
            Time = DateTime.Now;
            ImageSource = BitmapFrame.Create(new MemoryStream(channel.ChannelImage));
        }
        public UIChat(Group group)
        {
            Name = group.GroupName;
            LastMessage = Encoding.Default.GetString(group.GroupMessages.Last().Message.DataMessage.Data);
            Time = DateTime.Now;
            ImageSource = BitmapFrame.Create(new MemoryStream(group.GroupImage));
        }
    }
}
