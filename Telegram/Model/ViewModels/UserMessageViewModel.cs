using System;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace Telegram.ViewModels
{
    public class UserMessageViewModel
    {
        public int Id { get; set; }
        public bool Viewed { get; set; }
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
        public UserViewModel Author{ get; set; }
    }
}
