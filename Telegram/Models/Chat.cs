using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Runtime.InteropServices.ComTypes;
using System.Linq;
using System.ComponentModel;
using System.Windows.Markup;
using System.Windows;

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
                if(ChatName.Length >= 2)
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
        public string MutePath
        {
            get
            {
                if (MuteStatus)
                    return "M40.7348,20.2858 L32.2495,28.7711 M32.2496,20.2858 L40.7349,28.7711 M24,6 V42 C17,42 11.7985,32.8391 11.7985,32.8391 H6 C4.89543,32.8391 4,31.9437 4,30.8391 V17.0108 C4,15.9062 4.89543,15.0108 6,15.0108 H11.7985 C11.7985,15.0108 17,6 24,6 Z";
                else
                    return "";
            }
        }
        public string Type { get; set; }
        public Visibility ChannelIconVisibility
        {
            get
            {
                if (Type == "Channel")
                    return Visibility.Visible;
                return Visibility.Collapsed;
            }
        }
        public int MembersCount { get; set; }
        public int PinnedMessageId { get; set; }
        public string ChatInfo { get; set; }
        public int AuthorId { get; set; }
        public int NotViewedCounter { get; set; }
    }
}
