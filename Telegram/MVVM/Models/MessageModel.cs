﻿using System;

namespace Telegram.Models
{
    public class MessageModel
    {
        public string UserName { get; set; }
        public string ImageSource { get; set; }
        public string Message { get; set; }
        public DateTime Time { get; set; }
        public bool IsNativeOriogin { get; set; }
        public bool? FirstMessage { get; set; }
    }
}
