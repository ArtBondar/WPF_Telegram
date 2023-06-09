﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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
        public bool Tag
        {
            get
            {
                if (!String.IsNullOrEmpty(Photo))
                    return true;
                return false;
            }
        }
        public string PhotoText
        {
            get
            {
                if (UserName.Length >= 2)
                    return UserName.Substring(0, 2).ToUpper();
                return null;
            }
        }
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
        public Visibility VisibilityAddContact { get; set; } = Visibility.Visible;
        public User()
        {
            
        }
    }
}
