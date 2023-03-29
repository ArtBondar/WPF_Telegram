using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Telegram.Model;

namespace Telegram.Models
{
    public class User
    {
        public int Id { get; set; }
        public int? Age { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string UserName { get; set; }
        public string AboutUser { get; set; }
        public DateTime? LastOnline { get; set; }
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
        public Member Member { get; set; }
        public ICollection<Chat> Chats { get; set; }
        public ICollection<UserContact> UserContacts { get; set; }
        public IList<UserSetting> UserSettings { get; set; }
        public IList<UserRole> UserRoles { get; set; }
        public IList<UserChat> UserChats { get; set; }

        public User()
        {
            
        }
    }
}
