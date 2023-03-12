using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Core;
using Telegram.Models;

namespace Telegram.ViewModel
{
    public class MainViewModel : ObservableObject
    {
        public ObservableCollection<MessageModel> Messages { get; set; }
        public ObservableCollection<ContactCard> Contacts { get; set; }

        // Commands
        public RelayCommand SendCommand { get; set; }

        private ContactCard _selectedContact;

        public ContactCard SelectedContact
        {
            get { return _selectedContact; }
            set { _selectedContact = value; OnPropertyChanged();  }
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
