using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using Telegram.Model;
using System.Runtime.InteropServices.ComTypes;
using System.Linq;
using System.ComponentModel;
using System.Windows.Markup;

namespace Telegram.Models
{
    public partial class Chat
    {
        public int Id { get; set; }
        public string ChatImage { get; set; }
        public ImageSource PhotoSource
        {
            get
            {
                if (!String.IsNullOrEmpty(ChatImage))
                {
                    string x = ChatImage.Substring(ChatImage.IndexOf("base64,") + 7);
                    byte[] bytes;
                    try
                    {
                        bytes = System.Convert.FromBase64String(x);
                        MemoryStream ms = new MemoryStream(bytes);
                        return BitmapFrame.Create(ms);
                    } catch
                    {
                        return null;
                    }
                }
                return null;
            }
        }
        public bool Tag
        {
            get
            {
                if (!String.IsNullOrEmpty(ChatImage))
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
                if ((DateTime.Now - PublishTime).TotalDays >= 1)
                    return PublishTime.ToString("dd.MM.yyyy");
                else
                    return PublishTime.ToString("HH:mm");
            }
        }
        public int CountNotViwedMessages
        {
            get
            {
                return 1;
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
