using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using Telegram.Model;
using System.Runtime.InteropServices.ComTypes;

namespace Telegram.Models
{
    public partial class Chat
    {
        public int Id { get; set; }
        public byte[] ChatImage { get; set; }
        public ImageSource PhotoSource
        {
            get
            {
                if (ChatImage != null)
                    return BitmapFrame.Create(new MemoryStream(ChatImage));
                return null;
            }
        }
        public bool Tag
        {
            get
            {
                if (ChatImage != null)
                    return true;
                return false;
            }
        }
        public string PhotoText
        {
            get
            {
                if(ChatName.Length > 2)
                    return ChatName.Substring(0, 2).ToUpper();
                return null;
            }
        }
        public string ViewTime
        {
            get
            {
                if ((DateTime.Now - PublishTime).TotalDays > 1)
                    return PublishTime.ToString("d");
                else
                    return PublishTime.ToString("t");
            }
        }
        public string ChatName { get; set; }
        public string ShortMessage { get; set; }
        public DateTime PublishTime { get; set; }
        public bool MuteStatus { get; set; }
        public string Type { get; set; }
        public int MembersCount { get; set; }
        public int PinnedMessageId { get; set; }
        public string ChatInfo { get; set; }
        public int AuthorId { get; set; }
        public IList<ChatMessage> ChatMessages { get; set; }
        public IList<UserChat> UserChats { get; set; }
    }
}
