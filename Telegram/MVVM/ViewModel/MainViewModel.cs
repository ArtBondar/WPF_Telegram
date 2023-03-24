using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Telegram.Core;
using Telegram.Models;
using Telegram.MVVM.ViewModel;

namespace Telegram.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        // public User LoginedUser { get; set; }
        public ObservableCollection<Chat> Chats { get; set; }
        public ObservableCollection<SavedMessage> SavedMessages { get; set; }
        //
        public ObservableCollection<MessageModel> Messages { get; set; }
        public ObservableCollection<ContactCard> Contacts { get; set; }
        //
        //public async void RefreshDate(string token)
        //{
        //    var client = new HttpClient();
        //    var data = JsonConvert.SerializeObject(new { token, login = LoginedUser.UserName });
        //    var content = new StringContent(data, Encoding.UTF8, "application/json");
        //    var response = await client.PostAsync("https://localhost:7195/api/Users/updateinfo", content);
        //    var responseString = await response.Content.ReadAsStringAsync();
        //    if (responseString == null)
        //    {
        //        MessageBox.Show("Server error...");
        //        return;
        //    }
        //    var result = JsonConvert.DeserializeAnonymousType(responseString, new { user = new Models.User(), });
        //    if (LoginedUser != result.user)
        //    {
        //        LoginedUser = result.user;
        //    }
        //}

        // Commands
        public RelayCommand SendCommand { get; set; }

        private ContactCard _selectedContact;

        public ContactCard SelectedContact
        {
            get { return _selectedContact; }
            set { _selectedContact = value; OnPropertyChanged(); }
        }

        private string _message;

        public string Message
        {
            get { return _message; }
            set { _message = value; OnPropertyChanged(); }
        }

        public MainViewModel()
        {
            Messages = new ObservableCollection<MessageModel>();
            Contacts = new ObservableCollection<ContactCard>();

            SendCommand = new RelayCommand(o =>
            {
                Messages.Add(new MessageModel
                {
                    UserName = "Me",
                    ImageSource = "https://i.imgur.com/yMWvLXd.png",
                   Message = Message,
                   Time = DateTime.Now,
                    IsNativeOriogin = false,
                    FirstMessage = true
                });
                Message = "";
            });

            // Refresh date

            for (int i = 0; i < 5; i++)
            {
                Messages.Add(new MessageModel
                {
                    UserName = "Allis",
                    ImageSource = "https://i.imgur.com/yMWvLXd.png",
                    Message = "Test",
                    Time = DateTime.Now,
                    IsNativeOriogin = false,
                    FirstMessage = true
                });
            }
            for (int i = 0; i < 5; i++)
            {
                Messages.Add(new MessageModel
                {
                    UserName = "Bunny",
                    ImageSource = "https://i.imgur.com/yMWvLXd.png",
                    Message = "Test...",
                    Time = DateTime.Now,
                    IsNativeOriogin = true
                });
            }
            for (int i = 0; i < 13; i++)
            {
                Contacts.Add(new ContactCard
                {
                    UserName = $"Allis {i}",
                    ImageSource = "https://i.imgur.com/yMWvLXd.png",
                    Messages = Messages
                });
            }
        }
    }
}
