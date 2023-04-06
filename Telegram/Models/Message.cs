using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;

namespace Telegram.Models
{
    public partial class Message
    {
        public int Id { get; set; }
        public bool Viewed { get; set; }
        public Visibility VisibilityViewed { get; set; } = Visibility.Collapsed;
        public string Data { get; set; }
        public ImageSource PhotoSource
        {
            get
            {
                if (!String.IsNullOrEmpty(Data))
                {
                    string x = Data.Substring(Data.IndexOf("base64,") + 7);
                    byte[] bytes;
                    try
                    {
                        bytes = System.Convert.FromBase64String(x);
                        MemoryStream ms = new MemoryStream(bytes);
                        return BitmapFrame.Create(ms);
                    }
                    catch
                    {
                        return null;
                    }
                }
                return null;
            }
        }
        public bool DeliveryStatus { get; set; }
        public DateTime SendTime { get; set; }
        public string Text { get; set; }
        public User Author { get; set; }
        public Visibility? VisibilityDeleteMessage { get; set; } = null;
    }
}
