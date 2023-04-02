using System;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace Telegram.ViewModels
{
    public class UserViewModel
    {
        public int Id { get; set; }
        public int Age { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string AboutUser { get; set; }
        public DateTime LastOnline { get; set; }
        public string Photo { get; set; }
        public ImageSource PhotoSource
        {
            get
            {
                if (!String.IsNullOrEmpty(Photo))
                {
                    string x = Photo.Substring(Photo.IndexOf("base64,") + 7);
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
        public int? MemberId { get; set; }
    }
}
