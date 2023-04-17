using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Telegram.Models;

namespace Telegram
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Criptic : Window
    {
        public string JwtToken { get; set; }
        public User LoginedUser { get; set; }
        // Updated values
        public List<User> UserContacts { get; set; }
        public List<Chat> Chats { get; set; } = new List<Chat>();
        public Chat SelectedChat { get; set; }
        public List<SavedMessage> SavedMessages { get; set; }
        // Temp date
        public enum CreateFrag { Null, Group, Channel }
        public CreateFrag CreateGroupOrChannel { get; set; }
        public string GroupOrChannelName { get; set; }
        public string DescriptionChannel { get; set; }
        public string SelectedPhoto { get; set; }
        public Chat CreatedLastChat { get; set; }
        public List<User> SelectedContacts { get; set; } = new List<User>();
        public string codeString { get; set; }
        // Temp date

        // Refresh UI
        public void RefreshUI()
        {
            RefreshUILoginedUser();
            RefreshUIChats();
            RefreshUIContacts();
        }
        public void RefreshUILoginedUser()
        {
            Dispatcher.Invoke(() =>
            {
                if (LoginedUser != null)
                {
                    Username_Lable_LeftMenu.Content = LoginedUser.UserName;
                    Email_Lable_LeftMenu.Content = LoginedUser.Email;
                    if (LoginedUser.PhotoSource == null)
                    {
                        LeftMenuEllipse.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#B8ACFF"));
                        LeftMenuEllipseText.Text = LoginedUser.UserName.Substring(0, 2).ToUpper();
                    }
                    else
                    {
                        LeftMenuEllipse.Fill = new ImageBrush(LoginedUser.PhotoSource);
                        LeftMenuEllipseText.Text = "";
                    }
                    // Settings
                    if (LoginedUser.PhotoSource == null)
                    {
                        Ellipse_Avatar.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#B8ACFF"));
                        SettingsEllipseText.Text = LoginedUser.UserName.Substring(0, 2).ToUpper();
                    }
                    else
                    {
                        Ellipse_Avatar.Fill = new ImageBrush(LoginedUser.PhotoSource);
                        SettingsEllipseText.Text = "";
                    }
                    // Settings
                    SettingsEditEmail_Lable.Content = SettingsEmail_Lable.Content = LoginedUser.Email;
                    SettingsEditUserName_Lable.Content = SettingsUserName_Lable.Content = LoginedUser.UserName;
                    if (!String.IsNullOrWhiteSpace(LoginedUser.AboutUser))
                    {
                        SettingsDescription_Lable.Text = LoginedUser.AboutUser;
                        SettingsDescription_Lable.Foreground = Brushes.White;
                    }
                    else
                    {
                        SettingsDescription_Lable.Foreground = Brushes.Gray;
                        SettingsDescription_Lable.Text = "Description...";
                    }
                }
            });
        }
        public void RefreshUIChats()
        {
            Dispatcher.Invoke(() =>
            {
                if (LoginedUser != null)
                {
                    // Chats
                    Contact_ListView.ItemsSource = Chats.OrderByDescending(chat => chat.PublishTime);
                    if (SelectedChat != null)
                    {
                        List<Chat> list = Contact_ListView.Items.Cast<Chat>().ToList();
                        Contact_ListView.SelectedIndex = list.IndexOf(list.FirstOrDefault(chat => chat.Id == SelectedChat.Id));
                    }
                }
            });
        }
        public void RefreshUIContacts()
        {
            Dispatcher.Invoke(() =>
            {
                if (LoginedUser != null)
                {
                    // Contacts
                    ContactsList.ItemsSource = UserContacts;
                    if (SelectedContacts.Count > 0)
                    {
                        List<User> list = ContactsList.Items.Cast<User>().ToList();
                        var copyCollection = new List<User>(SelectedContacts);
                        foreach (User contact in copyCollection)
                            ContactsList.SelectedItems.Add(list.FirstOrDefault(user => user.Id == contact.Id));
                    }
                }
            });
        }
        // Refresh UI
        public async void RefreshDate(object sender, EventArgs e)
        {
            if (JwtToken != null && LoginedUser.UserName != null)
            {
                var client = new HttpClient();
                var data = JsonConvert.SerializeObject(new { token = JwtToken, login = LoginedUser.UserName });
                var content = new StringContent(data, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("https://localhost:7195/api/Users/updateinfo", content);
                var responseString = await response.Content.ReadAsStringAsync();
                if (responseString == null)
                {
                    MessageBox.Show("Server error...");
                    this.Close();
                    return;
                }
                var result = JsonConvert.DeserializeAnonymousType(responseString, new { user = new User(), chats = new List<Chat>(), savedMessages = new List<SavedMessage>(), contacts = new List<User>() });
                if (result.user != null)
                {
                    if (JsonConvert.SerializeObject(LoginedUser) != JsonConvert.SerializeObject(result.user))
                    {
                        LoginedUser = result.user;
                        RefreshUILoginedUser();
                    }
                    if (JsonConvert.SerializeObject(Chats) != JsonConvert.SerializeObject(result.chats) || JsonConvert.SerializeObject(SavedMessages) != JsonConvert.SerializeObject(result.savedMessages))
                    {
                        Chats = result.chats;
                        SavedMessages = result.savedMessages;
                        RefreshUIChats();
                    }
                    if (JsonConvert.SerializeObject(UserContacts) != JsonConvert.SerializeObject(result.contacts))
                    {
                        UserContacts = result.contacts;
                        RefreshUIContacts();
                    }
                }
            }
        }
        public Criptic()
        {
            InitializeComponent();
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += RefreshDate;
            timer.Start();
        }
        private void Close_Left_Menu(object sender, MouseButtonEventArgs e)
        {
            Left_Menu_Grid.Width = 0;
        }
        private async void ToogleButton_Notification_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.Source != ToogleButton_Notification)
            {
                ToogleButton_Notification.IsChecked = !ToogleButton_Notification.IsChecked;
                Notifications_Path.Data = Geometry.Parse(ToogleButton_Notification.IsChecked.Value ? "M24 6V42C17 42 11.7985 32.8391 11.7985 32.8391H6C4.89543 32.8391 4 31.9437 4 30.8391V17.0108C4 15.9062 4.89543 15.0108 6 15.0108H11.7985C11.7985 15.0108 17 6 24 6Z M32 15L32 15C32.6232 15.5565 33.1881 16.1797 33.6841 16.8588C35.1387 18.8504 36 21.3223 36 24C36 26.6545 35.1535 29.1067 33.7218 31.0893C33.2168 31.7885 32.6391 32.4293 32 33 M34.2359 41.1857C40.0836 37.6953 44 31.305 44 24C44 16.8085 40.2043 10.5035 34.507 6.97906" : "M1.0107,0.976807 L19.3955,19.3616 M10.3036,3.4165 V18.4165 C7.38692,18.4165 5.21963,14.5995 5.21963,14.5995 H2.80359 C2.34335,14.5995 1.97026,14.2264 1.97026,13.7661 V8.00434 C1.97026,7.54409 2.34335,7.171 2.80359,7.171 H5.21963 C5.21963,7.171 7.38692,3.4165 10.3036,3.4165 Z M13.6369,7.1665 C13.8966,7.39838 14.132,7.65805 14.3386,7.941 C14.9447,8.77084 15.3036,9.8008 15.3036,10.9165 C15.3036,12.0225 14.9509,13.0443 14.3543,13.8704 C14.1439,14.1617 13.9032,14.4287 13.6369,14.6665 M14.5686,18.0772 C17.0051,16.6229 18.6369,13.9603 18.6369,10.9165 C18.6369,7.92006 17.0554,5.29298 14.6815,3.82446");
                var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", JwtToken);
                var response = await client.SendAsync(new HttpRequestMessage { Method = new HttpMethod("PATCH"), RequestUri = new Uri($"https://localhost:7195/api/Chats/notifications/{SelectedChat.Id}") });
                var responseString = await response.Content.ReadAsStringAsync();
            }
        }
        private void ToogleButton_DarkWhite_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.Source != ToogleButton_DarkWhite)
            {
                ToogleButton_DarkWhite.IsChecked = !ToogleButton_DarkWhite.IsChecked;
                ResourceDictionary resource = new ResourceDictionary();
                string uri = (ToogleButton_DarkWhite.IsChecked == true) ? $"Themes/MainWindow/Skins/White.xaml" : $"Themes/MainWindow/Skins/Dark.xaml";
                resource.Source = new Uri(uri, UriKind.Relative);
                this.Resources = resource;
            }
        }
        private void ToogleButton_DarkWhite_Click(object sender, RoutedEventArgs e)
        {
            ResourceDictionary resource = new ResourceDictionary();
            string uri = (ToogleButton_DarkWhite.IsChecked == true) ? $"Themes/MainWindow/Skins/White.xaml" : $"Themes/MainWindow/Skins/Dark.xaml";
            resource.Source = new Uri(uri, UriKind.Relative);
            this.Resources = resource;
        }
        private void OpenLeftMenu_Click(object sender, RoutedEventArgs e)
        {
            Left_Menu_Grid.Width = Double.NaN;
        }
        private void OpenRigthMenu_Click(object sender, RoutedEventArgs e)
        {
            if (RigthInfoMenu.Width.Value == 0)
                RigthInfoMenu.Width = new GridLength(250);
            else
                RigthInfoMenu.Width = new GridLength(0);
        }
        private void ThreePointMenuOpen_Click(object sender, RoutedEventArgs e)
        {
            ThreePointMenu.IsSubmenuOpen = true;
        }
        private async void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Chat Select = (sender as ListView).SelectedItem as Chat;
            if (Select == null) return;
            ChatGrid.Visibility = Visibility.Visible;
            //
            var client = new HttpClient();
            string additionalChatName = null;
            if (Select.Type == "Private")
                additionalChatName = Select.ChatName;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", JwtToken);
            var data = JsonConvert.SerializeObject(new { chatName = Select.ChatName, authorId = Select.AuthorId, additionalChatName });
            var content = new StringContent(data, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("https://localhost:7195/api/Chats/openchat", content);
            var responseString = await response.Content.ReadAsStringAsync();
            if (responseString == null)
            {
                MessageBox.Show("Server error...");
                this.Close();
                return;
            }
            var result = JsonConvert.DeserializeAnonymousType(responseString, new { error = "", chat = new Chat(), messages = new List<Message>(), members = new List<User>() });
            //
            if (result.chat == null) return;
            SelectedChat = result.chat;
            if (Select.PhotoSource == null)
            {
                SelectedEllipse.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#B8ACFF"));
                SelectedEllipseText.Text = Select.PhotoText;
            }
            else
            {
                SelectedEllipse.Fill = new ImageBrush(Select.PhotoSource);
                SelectedEllipseText.Text = "";
            }
            ChatPanel_Name.Content = Select.ChatName;
            //
            InfoPath.Visibility = Visibility.Visible;
            RigthInfo_Second.Content = "";
            ChatPanel_SecondInfo.Content = "";
            Info1_NameLable.Content = ""; Info1_NameLable.Visibility = Visibility.Collapsed;
            Info1_Lable.Content = ""; Info1_Lable.Visibility = Visibility.Collapsed;
            Info2_NameLable.Content = ""; Info2_NameLable.Visibility = Visibility.Collapsed;
            Info2_Lable.Content = ""; Info2_Lable.Visibility = Visibility.Collapsed;
            Info3_NameLable.Content = ""; Info3_NameLable.Visibility = Visibility.Collapsed;
            Info3_Lable.Content = ""; Info3_Lable.Visibility = Visibility.Collapsed;
            WriteMessageBox.Visibility = Visibility.Visible;
            //
            Edit_ThreePointInfoSelectedChat.Visibility = Visibility.Collapsed;
            //
            if (Select.Type == "Group" || Select.Type == "Channel")
            {
                ChatPanel_SecondInfo.Content = $"{result.members.Count} members";
                MembersList.ItemsSource = result.members;
                if (!String.IsNullOrWhiteSpace(Select.ChatInfo))
                {
                    Info1_NameLable.Content = "About"; Info1_NameLable.Visibility = Visibility.Visible;
                    Info1_Lable.Content = Select.ChatInfo; Info1_Lable.Visibility = Visibility.Visible;
                }
                else
                {
                    Info1_NameLable.Content = "";
                    Info1_Lable.Content = "";
                }
                Info2_NameLable.Content = $"{Select.Type} name"; Info2_NameLable.Visibility = Visibility.Visible;
                Info2_Lable.Content = Select.ChatName; Info2_Lable.Visibility = Visibility.Visible;
                RigthInfo_Second.Content = $"{result.members.Count} members";
                if (Select.AuthorId == LoginedUser.Id)
                    Edit_ThreePointInfoSelectedChat.Visibility = Visibility.Visible;
                if (Select.Type == "Channel" && Select.AuthorId != LoginedUser.Id)
                    WriteMessageBox.Visibility = Visibility.Collapsed;
            }
            if (Select.Type == "Private")
            {
                RigthInfo_Name.Content = Select.ChatName;
                RigthInfo_Second.Content = "?";
                ChatPanel_SecondInfo.Content = Select.PublishTime;
                if (!String.IsNullOrWhiteSpace(Select.ChatInfo))
                {
                    Info1_NameLable.Content = "About"; Info1_NameLable.Visibility = Visibility.Visible;
                    Info1_Lable.Content = Select.ChatInfo; Info1_Lable.Visibility = Visibility.Visible;
                }
                Info2_NameLable.Content = "Username"; Info2_NameLable.Visibility = Visibility.Visible;
                Info2_Lable.Content = Select.ChatName; Info2_Lable.Visibility = Visibility.Visible;
            }
            if (Select.Type == "Favorite")
            {
                InfoPath.Visibility = Visibility.Collapsed;
                // Favorite
            }
            // Messages
            foreach (Message message in result.messages)
            {
                if (message.Author.Id == LoginedUser.Id)
                {
                    message.VisibilityDeleteMessage = Visibility.Visible;
                    if (message.Viewed)
                        message.VisibilityViewed = Visibility.Visible;
                }
                else
                    message.VisibilityDeleteMessage = Visibility.Collapsed;
            }
            Chat_ListView.ItemsSource = result.messages;
            // To end
            ScrollViewer scrollViewer = GetDescendantByType(Chat_ListView, typeof(ScrollViewer)) as ScrollViewer;
            scrollViewer?.ScrollToEnd();
            //

            // RigthInfo
            if (Select.PhotoSource == null)
            {
                RigthInfoEllipse.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#B8ACFF"));
                RigthInfoEllipseText.Text = Select.PhotoText;
            }
            else
            {
                RigthInfoEllipse.Fill = new ImageBrush(Select.PhotoSource);
                RigthInfoEllipseText.Text = "";
            }
            RigthInfo_Name.Content = Select.ChatName;
            ToogleButton_Notification.IsChecked = !Select.MuteStatus;
        }
        private void Close_Settings_Menu(object sender, MouseButtonEventArgs e)
        {
            if (e.Source == Menu_Settings_Grid)
                Menu_Settings_Grid.Visibility = Visibility.Hidden;
        }
        private void Menu_Settings_Open(object sender, MouseButtonEventArgs e)
        {
            Menu_Settings_Grid.Visibility = Visibility.Visible;
            Left_Menu_Grid.Width = 0;
        }
        private void MenuSettingsGridClose_Click(object sender, RoutedEventArgs e)
        {
            Menu_Settings_Grid.Visibility = Visibility.Hidden;
        }
        private void Button_To_Down_Messages(object sender, RoutedEventArgs e)
        {
            if (Chat_ListView.Items.Count == 0)
            {
                Button_To_Down.Visibility = Visibility.Hidden;
                return;
            }
            var lastItem = Chat_ListView.Items[Chat_ListView.Items.Count - 1];
            Chat_ListView.ScrollIntoView(lastItem);
            Button_To_Down.Visibility = Visibility.Hidden;
        }
        private DependencyObject GetDescendantByType(DependencyObject element, Type type)
        {
            if (element == null) return null;
            if (element.GetType() == type) return element;

            DependencyObject foundElement = null;
            if (element is FrameworkElement)
            {
                (element as FrameworkElement).ApplyTemplate();
            }
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                DependencyObject visual = VisualTreeHelper.GetChild(element, i);
                foundElement = GetDescendantByType(visual, type);
                if (foundElement != null) break;
            }
            return foundElement;
        }
        private async void Chat_ListView_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            // Получить ScrollViewer из ListView
            ScrollViewer scrollViewer = GetDescendantByType(sender as ListView, typeof(ScrollViewer)) as ScrollViewer;

            // Если ScrollViewer находится в нижней части, скрыть кнопку Button_To_Down
            if (scrollViewer.VerticalOffset == scrollViewer.ScrollableHeight)
            {
                Button_To_Down.Visibility = Visibility.Hidden;
                // Read messages
                if (SelectedChat != null)
                {
                    var client = new HttpClient();
                    var data = JsonConvert.SerializeObject(new { chatId = SelectedChat.Id, userId = LoginedUser.Id });
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", JwtToken);
                    var content = new StringContent(data, Encoding.UTF8, "application/json");
                    await client.PostAsync("https://localhost:7195/api/Messages/readmessages", content);
                }
            }
            // Если ScrollViewer не находится в нижней части, проверить его позицию
            else
            {
                Button_To_Down.Visibility = Visibility.Visible;
            }
        }
        private void Close_CreateGroup_Menu(object sender, MouseButtonEventArgs e)
        {
            if (e.Source == Menu_CreateGroup_Grid)
                Menu_CreateGroup_Grid.Visibility = Visibility.Hidden;
        }
        private void Menu_CreateGroup_Open(object sender, MouseButtonEventArgs e)
        {
            Menu_CreateGroup_Grid.Visibility = Visibility.Visible;
            Left_Menu_Grid.Width = 0;
            Menu_CreateGroup_Grid.Focus();
        }
        private void Close_CreateGroup_Menu_Click(object sender, RoutedEventArgs e)
        {
            Menu_CreateGroup_Grid.Visibility = Visibility.Hidden;
        }
        private void Close_CreateChanel_Menu_Click(object sender, RoutedEventArgs e)
        {
            Menu_CreateChanel_Grid.Visibility = Visibility.Hidden;
        }
        private void Menu_CreateChanel_Open(object sender, MouseButtonEventArgs e)
        {
            Menu_CreateChanel_Grid.Visibility = Visibility.Visible;
            Left_Menu_Grid.Width = 0;
            Menu_CreateChanel_Grid.Focus();
        }
        private async void Menu_Settings_Edit_Avatar(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg|All files (*.*)|*.*",
                RestoreDirectory = true
            };
            if (openFileDialog.ShowDialog() == true)
            {
                byte[] bytes = File.ReadAllBytes(openFileDialog.FileName);
                string extension = Path.GetExtension(openFileDialog.SafeFileName);
                string photo = $"data:image/{extension.Substring(1)};base64," + Convert.ToBase64String(bytes);
                if (!String.IsNullOrWhiteSpace(photo))
                {
                    var client = new HttpClient();
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", JwtToken);
                    var data = JsonConvert.SerializeObject(new { id = LoginedUser.Id, photo });
                    var content = new StringContent(data, Encoding.UTF8, "application/json");
                    var response = await client.SendAsync(new HttpRequestMessage { Method = new HttpMethod("PATCH"), RequestUri = new Uri("https://localhost:7195/api/Users/patchuser"), Content = content });
                    var responseString = await response.Content.ReadAsStringAsync();
                    if (responseString == null)
                    {
                        MessageBox.Show("Server error...");
                        return;
                    }
                    var result = JsonConvert.DeserializeAnonymousType(responseString, new { error = "", user = new User() });
                    if (result.error != null)
                    {
                        MessageBox.Show($"{result.error}");
                        return;
                    }
                    if (result.user != null)
                    {
                        LoginedUser = result.user;
                        Menu_EditDescription_Grid.Visibility = Visibility.Hidden;
                    }
                }
            }
        }
        private void Close_EditUserName_Menu_Click(object sender, RoutedEventArgs e)
        {
            Menu_EditUserName_Grid.Visibility = Visibility.Hidden;
        }
        private void Open_EditUserName_ButtonUp(object sender, MouseButtonEventArgs e)
        {
            Menu_EditUserName_Grid.Visibility = Visibility.Visible;
            ((TextBox)NewUsernameEdit_TextBox.Template.FindName("MainTextBox", NewUsernameEdit_TextBox)).Text = LoginedUser.UserName;
        }
        private void Close_EditUserName_Menu(object sender, MouseButtonEventArgs e)
        {
            if (e.Source == Menu_EditUserName_Grid)
                Menu_EditUserName_Grid.Visibility = Visibility.Hidden;
        }
        private void Menu_CreateChanel_Grid_ButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (e.Source == Menu_CreateChanel_Grid)
                Menu_CreateChanel_Grid.Visibility = Visibility.Hidden;
        }
        private void Close_EditEmail_Menu_Click(object sender, RoutedEventArgs e)
        {
            Menu_EditEmail_Grid.Visibility = Visibility.Hidden;
        }
        private void Close_EditEmail_Menu(object sender, MouseButtonEventArgs e)
        {
            if (e.Source == Menu_EditEmail_Grid)
                Menu_EditEmail_Grid.Visibility = Visibility.Hidden;
        }
        private void Open_EditEmail_ButtonUp(object sender, MouseButtonEventArgs e)
        {
            Menu_EditEmail_Grid.Visibility = Visibility.Visible;
            ((TextBox)NewEmailEdit_TextBox.Template.FindName("MainTextBox", NewEmailEdit_TextBox)).Text = LoginedUser.Email;
        }
        private async void Edit_NewUsername_Click(object sender, RoutedEventArgs e)
        {
            string userName = ((TextBox)NewUsernameEdit_TextBox.Template.FindName("MainTextBox", NewUsernameEdit_TextBox)).Text;
            if (!String.IsNullOrWhiteSpace(userName))
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", JwtToken);
                var data = JsonConvert.SerializeObject(new { id = LoginedUser.Id, userName });
                var content = new StringContent(data, Encoding.UTF8, "application/json");
                var response = await client.SendAsync(new HttpRequestMessage { Method = new HttpMethod("PATCH"), RequestUri = new Uri("https://localhost:7195/api/Users/patchuser"), Content = content });
                var responseString = await response.Content.ReadAsStringAsync();
                if (responseString == null)
                {
                    MessageBox.Show("Server error...");
                    return;
                }
                var result = JsonConvert.DeserializeAnonymousType(responseString, new { error = "", user = new User() });
                if (result.error != null)
                {
                    MessageBox.Show($"{result.error}");
                    return;
                }
                if (result.user != null)
                {
                    LoginedUser = result.user;
                    Menu_EditUserName_Grid.Visibility = Visibility.Hidden;
                }
            }
        }
        private async void Edit_NewEmail_Click(object sender, RoutedEventArgs e)
        {
            string email = ((TextBox)NewEmailEdit_TextBox.Template.FindName("MainTextBox", NewEmailEdit_TextBox)).Text;
            if (!String.IsNullOrWhiteSpace(email))
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", JwtToken);
                var data = JsonConvert.SerializeObject(new { id = LoginedUser.Id, email });
                var content = new StringContent(data, Encoding.UTF8, "application/json");
                var response = await client.SendAsync(new HttpRequestMessage { Method = new HttpMethod("PATCH"), RequestUri = new Uri("https://localhost:7195/api/Users/patchuser"), Content = content });
                var responseString = await response.Content.ReadAsStringAsync();
                if (responseString == null)
                {
                    MessageBox.Show("Server error...");
                    return;
                }
                var result = JsonConvert.DeserializeAnonymousType(responseString, new { error = "", user = new User() });
                if (result.error != null)
                {
                    MessageBox.Show($"{result.error}");
                    return;
                }
                if (result.user != null)
                {
                    LoginedUser = result.user;
                    Menu_EditEmail_Grid.Visibility = Visibility.Hidden;
                }
            }
        }
        private void OpenPointMenuSettings_Click(object sender, RoutedEventArgs e)
        {
            ThreePointsSettings.IsSubmenuOpen = true;
        }
        private void Logout_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            JwtToken = null;
            LoginedUser = null;
            Chats = null;
            SavedMessages = null;
            if (File.Exists("login.txt"))
                File.Delete("login.txt");
            SingUp mainForm = new SingUp();
            mainForm.Show();
            this.Close();
        }
        private void SettingsPassword1_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PlaceHolderPasswordSettings1.Visibility = (sender as PasswordBox).Password.Length == 0 ? Visibility.Visible : Visibility.Hidden;
        }
        private void SettingsPassword2_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PlaceHolderPasswordSettings2.Visibility = (sender as PasswordBox).Password.Length == 0 ? Visibility.Visible : Visibility.Hidden;
        }
        private void Close_EditPassword_Menu(object sender, MouseButtonEventArgs e)
        {
            if (e.Source == Menu_EditPassword_Grid)
                Menu_EditPassword_Grid.Visibility = Visibility.Hidden;
        }
        private void Close_EditPassword_Click(object sender, RoutedEventArgs e)
        {
            PasswordBoxSettings1.Password = "";
            PasswordBoxSettings2.Password = "";
            Menu_EditPassword_Grid.Visibility = Visibility.Hidden;
        }
        private async void Edit_EditPassword_Click(object sender, RoutedEventArgs e)
        {
            if (PasswordBoxSettings1.Password == PasswordBoxSettings2.Password)
            {
                string newPassword = PasswordBoxSettings1.Password;
                if (!String.IsNullOrWhiteSpace(newPassword))
                {
                    var client = new HttpClient();
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", JwtToken);
                    var data = JsonConvert.SerializeObject(new { email = LoginedUser.Email, newPassword });
                    var content = new StringContent(data, Encoding.UTF8, "application/json");
                    var response = await client.SendAsync(new HttpRequestMessage { Method = new HttpMethod("PATCH"), RequestUri = new Uri("https://localhost:7195/api/Users/setpassword"), Content = content });
                    var responseString = await response.Content.ReadAsStringAsync();
                    if (responseString == null)
                    {
                        MessageBox.Show("Server error...");
                        return;
                    }
                    var result = JsonConvert.DeserializeAnonymousType(responseString, new { result = "", user = new User() });
                    if (result.result == "success")
                    {
                        LoginedUser = result.user;
                        Menu_EditPassword_Grid.Visibility = Visibility.Hidden;
                    }
                }
            }
            else
            {
                PasswordBorder1.BorderBrush = Brushes.Red;
                PasswordBorder2.BorderBrush = Brushes.Red;
            }
        }
        private async void Open_EditPassword_ButtonUp(object sender, MouseButtonEventArgs e)
        {
            Menu_EditPasswordCode_Grid.Visibility = Visibility.Visible;
            var client = new HttpClient();
            var data = JsonConvert.SerializeObject(new { email = LoginedUser.Email });
            var content = new StringContent(data, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("https://localhost:7195/api/Email/sendcode", content);
            var responseString = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeAnonymousType(responseString, new { code = "" });
            codeString = result?.code;
        }
        private void EditImageCreateGroup(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg|All files (*.*)|*.*",
                RestoreDirectory = true
            };

            if (openFileDialog.ShowDialog() == true)
            {
                byte[] tbytes = File.ReadAllBytes(openFileDialog.FileName);
                string extension = Path.GetExtension(openFileDialog.SafeFileName);
                SelectedPhoto = $"data:image/{extension.Substring(1)};base64," + Convert.ToBase64String(tbytes);
                //
                string imagePath = openFileDialog.FileName;
                BitmapImage bitmap = new BitmapImage(new Uri(imagePath, UriKind.RelativeOrAbsolute));
                byte[] bytes;
                using (MemoryStream ms = new MemoryStream())
                {
                    PngBitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(bitmap));
                    encoder.Save(ms);
                    bytes = ms.ToArray();
                }
                ImageSource image = null;
                try
                {
                    image = BitmapFrame.Create(new MemoryStream(bytes));
                    Ellipse_CreateGroup.Fill = new ImageBrush(image);
                    PhotoPath_CreateGroup.Visibility = Visibility.Collapsed;
                }
                catch (Exception ex) { MessageBox.Show($"Error: {ex.Message}"); }
            }
        }
        private void EditImageCreateChannel(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg|All files (*.*)|*.*",
                RestoreDirectory = true
            };

            if (openFileDialog.ShowDialog() == true)
            {
                byte[] tbytes = File.ReadAllBytes(openFileDialog.FileName);
                string extension = Path.GetExtension(openFileDialog.SafeFileName);
                SelectedPhoto = $"data:image/{extension.Substring(1)};base64," + Convert.ToBase64String(tbytes);
                //
                string imagePath = openFileDialog.FileName;
                BitmapImage bitmap = new BitmapImage(new Uri(imagePath, UriKind.RelativeOrAbsolute));
                byte[] bytes;
                using (MemoryStream ms = new MemoryStream())
                {
                    PngBitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(bitmap));
                    encoder.Save(ms);
                    bytes = ms.ToArray();
                }
                ImageSource image = null;
                try
                {
                    image = BitmapFrame.Create(new MemoryStream(bytes));
                    Ellipse_CreateChannel.Fill = new ImageBrush(image);
                    PhotoPath_CreateChannel.Visibility = Visibility.Collapsed;
                }
                catch (Exception ex) { MessageBox.Show($"Error: {ex.Message}"); }

            }
        }
        private void Close_Contacts_Menu(object sender, MouseButtonEventArgs e)
        {
            if (e.Source == Contacts_Create_Grid)
            {
                Contacts_Create_Grid.Visibility = Visibility.Hidden;
                ContactsList.SelectedItems.Clear();
            }
        }
        private void Close_ContactsCreate_Click(object sender, RoutedEventArgs e)
        {
            Contacts_Create_Grid.Visibility = Visibility.Hidden;
            ContactsList.SelectedItems.Clear();
        }
        private async void CreateGroup_Menu_Click(object sender, RoutedEventArgs e)
        {
            CreateGroupOrChannel = CreateFrag.Group;
            GroupOrChannelName = ((TextBox)GroupCreateTextBox.Template.FindName("MainTextBox", GroupCreateTextBox)).Text;
            // SelectedChat
            if (!String.IsNullOrWhiteSpace(GroupOrChannelName))
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", JwtToken);
                var data = JsonConvert.SerializeObject(new { chatImage = SelectedPhoto, chatName = GroupOrChannelName, shortMessage = "Group created", publishTime = DateTime.Now, Type = "Group", authorId = LoginedUser.Id });
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", JwtToken);
                var content = new StringContent(data, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("https://localhost:7195/api/Chats/createchat", content);
                var responseString = await response.Content.ReadAsStringAsync();
                if (responseString == null)
                {
                    MessageBox.Show("Server error...");
                    return;
                }
                var result = JsonConvert.DeserializeAnonymousType(responseString, new { error = "", result = "", chat = new Chat() });
                if (result == null)
                {
                    MessageBox.Show($"{result.error}");
                    return;
                }
                if (result.result != null)
                {
                    Menu_CreateGroup_Grid.Visibility = Visibility.Hidden;
                    Contacts_Create_Grid.Visibility = Visibility.Visible;
                    CreatedLastChat = result.chat;
                    SelectedPhoto = null;
                    GroupOrChannelName = null;
                }
            }
        }
        private async void CreateChanel_Click(object sender, RoutedEventArgs e)
        {
            CreateGroupOrChannel = CreateFrag.Channel;
            GroupOrChannelName = ((TextBox)ChannelName.Template.FindName("MainTextBox", ChannelName)).Text;
            DescriptionChannel = ((TextBox)ChannelDescription.Template.FindName("MainTextBox", ChannelDescription)).Text;
            //
            if (!String.IsNullOrWhiteSpace(GroupOrChannelName))
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", JwtToken);
                var data = JsonConvert.SerializeObject(new { chatImage = SelectedPhoto, chatName = GroupOrChannelName, shortMessage = "Channel created", publishTime = DateTime.Now, Type = "Channel", authorId = LoginedUser.Id });
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", JwtToken);
                var content = new StringContent(data, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("https://localhost:7195/api/Chats/createchat", content);
                var responseString = await response.Content.ReadAsStringAsync();
                if (responseString == null)
                {
                    MessageBox.Show("Server error...");
                    return;
                }
                var result = JsonConvert.DeserializeAnonymousType(responseString, new { error = "", result = "", chat = new Chat() });
                if (result == null)
                {
                    MessageBox.Show($"{result.error}");
                    return;
                }
                if (result.result != null)
                {
                    Menu_CreateChanel_Grid.Visibility = Visibility.Hidden;
                    Contacts_Create_Grid.Visibility = Visibility.Visible;
                    CreatedLastChat = result.chat;
                    SelectedPhoto = null;
                    GroupOrChannelName = null;
                }
            }
        }
        private async void AddMembersClick(object sender, RoutedEventArgs e)
        {
            List<User> users = new List<User>();
            if (CreateGroupOrChannel == CreateFrag.Null) return;
            if (ContactsList.SelectedItems.Count != 0)
            {
                users = ContactsList.SelectedItems.Cast<User>().ToList();
                List<int> members = users.Select(u => u.Id).ToList();
                var client = new HttpClient();
                var data = JsonConvert.SerializeObject(new { id = CreatedLastChat.Id, members });
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", JwtToken);
                var content = new StringContent(data, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("https://localhost:7195/api/Chats/editpublicchat", content);
                var responseString = await response.Content.ReadAsStringAsync();
                if (responseString == null)
                {
                    MessageBox.Show("Server error...");
                    this.Close();
                    return;
                }
                ContactsList.SelectedItems.Clear();
            }
            Contacts_Create_Grid.Visibility = Visibility.Hidden;
        }
        private void Menu_Settings_Edit_Description(object sender, MouseButtonEventArgs e)
        {
            ((TextBox)NewEditDescription_TextBox.Template.FindName("MainTextBox", NewEditDescription_TextBox)).Text = LoginedUser.AboutUser;
            Menu_EditDescription_Grid.Visibility = Visibility.Visible;
        }
        private void Close_EditDescription_Menu_Click(object sender, RoutedEventArgs e)
        {
            Menu_EditDescription_Grid.Visibility = Visibility.Hidden;
        }
        private void Close_EditDescription_Menu(object sender, MouseButtonEventArgs e)
        {
            if (e.Source == Menu_EditDescription_Grid)
                Menu_EditDescription_Grid.Visibility = Visibility.Hidden;
        }
        private async void Edit_Description_Click(object sender, RoutedEventArgs e)
        {
            string about = ((TextBox)NewEditDescription_TextBox.Template.FindName("MainTextBox", NewEditDescription_TextBox)).Text;
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", JwtToken);
            var data = JsonConvert.SerializeObject(new { id = LoginedUser.Id, about });
            var content = new StringContent(data, Encoding.UTF8, "application/json");
            var response = await client.SendAsync(new HttpRequestMessage { Method = new HttpMethod("PATCH"), RequestUri = new Uri("https://localhost:7195/api/Users/patchuser"), Content = content });
            var responseString = await response.Content.ReadAsStringAsync();
            if (responseString == null)
            {
                MessageBox.Show("Server error...");
                return;
            }
            var result = JsonConvert.DeserializeAnonymousType(responseString, new { error = "", user = new User() });
            if (result.error != null)
            {
                MessageBox.Show($"{result.error}");
                return;
            }
            if (result.user != null)
            {
                LoginedUser = result.user;
                Menu_EditDescription_Grid.Visibility = Visibility.Hidden;
            }
        }
        private void Menu_Favorites_LeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            List<Chat> myList = Contact_ListView.Items.Cast<Chat>().ToList();
            Contact_ListView.SelectedIndex = myList.IndexOf(myList.FirstOrDefault(chat => chat.Type == "Favorite"));
            Left_Menu_Grid.Width = 0;
        }
        private async void MessageBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Contact_ListView.SelectedItem == null) return;
            var thistextBox = (TextBox)(sender as TextBox).Template.FindName("MainTextBox", sender as TextBox);
            if (e.Key == Key.Enter && !string.IsNullOrEmpty(thistextBox.Text))
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", JwtToken);
                var data = JsonConvert.SerializeObject(new { userId = LoginedUser.Id, chatId = (Contact_ListView.SelectedItem as Chat).Id, text = thistextBox.Text });
                var content = new StringContent(data, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("https://localhost:7195/api/Messages/sendmessage", content);
                var responseString = await response.Content.ReadAsStringAsync();
                if (responseString == null)
                {
                    MessageBox.Show("Server error...");
                    return;
                }
                var result = JsonConvert.DeserializeAnonymousType(responseString, new { deliveryStatus = false });
                if (result.deliveryStatus)
                {
                    thistextBox.Text = "";
                }
            }
        }
        private void ContactsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (User user in (sender as ListBox).SelectedItems)
            {
                if (!SelectedContacts.Contains(user))
                {
                    SelectedContacts.Add(user);
                }
            }
        }
        private async void AddFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg|All files (*.*)|*.*",
                RestoreDirectory = true
            };
            if (openFileDialog.ShowDialog() == true)
            {
                byte[] bytes = File.ReadAllBytes(openFileDialog.FileName);
                string extension = Path.GetExtension(openFileDialog.SafeFileName);
                string photo = $"data:image/{extension.Substring(1)};base64," + Convert.ToBase64String(bytes);
                if (!String.IsNullOrWhiteSpace(photo))
                {
                    var client = new HttpClient();
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", JwtToken);
                    var data = JsonConvert.SerializeObject(new { userId = LoginedUser.Id, chatId = (Contact_ListView.SelectedItem as Chat).Id, data = photo }); // Add data
                    var content = new StringContent(data, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync("https://localhost:7195/api/Messages/sendmessage", content);
                    var responseString = await response.Content.ReadAsStringAsync();
                    if (responseString == null)
                    {
                        MessageBox.Show("Server error...");
                        return;
                    }
                }
            }
        }
        private void CopyTextMessageMenu_Click(object sender, RoutedEventArgs e)
        {
            string text = (sender as MenuItem).Tag.ToString();
            Clipboard.SetText(text);
        }
        private async void DeleteMessageMenu_Click(object sender, RoutedEventArgs e)
        {
            int messageId = (int)(sender as MenuItem).Tag;
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", JwtToken);
            var data = JsonConvert.SerializeObject(new { userId = LoginedUser.Id, messageId });
            var content = new StringContent(data, Encoding.UTF8, "application/json");
            var response = await client.SendAsync(new HttpRequestMessage { Method = new HttpMethod("DELETE"), RequestUri = new Uri("https://localhost:7195/api/Messages/deletemessage"), Content = content });
            var responseString = await response.Content.ReadAsStringAsync();
            if (responseString == null)
            {
                MessageBox.Show("Server error...");
                return;
            }
        }
        private void OpenSelectedChatmenuInfo(object sender, MouseButtonEventArgs e)
        {
            if (e.Source == BorderChatInfoWithButtons)
            {
                Menu_Info_Grid.Visibility = Visibility.Visible;
                ToogleButton_Notification_Info.IsChecked = !SelectedChat.MuteStatus;
                Info_Name.Content = SelectedChat.ChatName;
                Info_Second.Content = $"{SelectedChat.MembersCount} members";
                Lable_Members.Visibility = Visibility.Collapsed;
                ChatPanel_SecondInfo.Content = "";
                Info1_NameLable_Info.Content = ""; Info1_NameLable_Info.Visibility = Visibility.Collapsed;
                Info1_Lable_Info.Content = ""; Info1_Lable_Info.Visibility = Visibility.Collapsed;
                Info2_NameLable_Info.Content = ""; Info2_NameLable_Info.Visibility = Visibility.Collapsed;
                Info2_Lable_Info.Content = ""; Info2_Lable_Info.Visibility = Visibility.Collapsed;
                Info3_NameLable_Info.Content = ""; Info3_NameLable_Info.Visibility = Visibility.Collapsed;
                Info3_Lable_Info.Content = ""; Info3_Lable_Info.Visibility = Visibility.Collapsed;
                InfoPath_Info.Visibility = Visibility.Collapsed;
                UpInfoBorder.Visibility = Visibility.Collapsed;
                MembersList.Visibility = Visibility.Collapsed;
                if (SelectedChat.Type == "Private")
                {
                    Info_Name.Content = SelectedChat.ChatName;
                    Info_Second.Content = SelectedChat.PublishTime;
                    ChatPanel_SecondInfo.Content = SelectedChat.PublishTime;
                    if (!String.IsNullOrWhiteSpace(SelectedChat.ChatInfo))
                    {
                        Info1_NameLable_Info.Content = "About"; Info1_NameLable_Info.Visibility = Visibility.Visible;
                        Info1_Lable_Info.Content = SelectedChat.ChatInfo; Info1_Lable_Info.Visibility = Visibility.Visible;
                    }
                    Info2_NameLable_Info.Content = "Username"; Info2_NameLable_Info.Visibility = Visibility.Visible;
                    Info2_Lable_Info.Content = SelectedChat.ChatName; Info2_Lable_Info.Visibility = Visibility.Visible;
                    InfoPath_Info.Visibility = Visibility.Visible;
                    UpInfoBorder.Visibility = Visibility.Visible;
                }
                if (SelectedChat.Type == "Group" || SelectedChat.Type == "Channel")
                {
                    MembersList.Visibility = Visibility.Visible;
                    Lable_Members.Visibility = Visibility.Visible;
                }
                if (SelectedChat.PhotoSource == null)
                {
                    InfoEllipse.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#B8ACFF"));
                    InfoEllipseText.Text = SelectedChat.PhotoText;
                }
                else
                {
                    InfoEllipse.Fill = new ImageBrush(SelectedChat.PhotoSource);
                    InfoEllipseText.Text = "";
                }
            }
        }
        private void Close_Info_Menu(object sender, MouseButtonEventArgs e)
        {
            if (e.Source == Menu_Info_Grid)
            {
                Menu_Info_Grid.Visibility = Visibility.Hidden;
            }
        }
        private void MenuInfoGridClose_Click(object sender, RoutedEventArgs e)
        {
            Menu_Info_Grid.Visibility = Visibility.Hidden;
        }
        private async void DeleteChat_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedChat.Type == "Channel" || SelectedChat.Type == "Group")
            {
                var client = new HttpClient();
                var data = JsonConvert.SerializeObject(new { userName = LoginedUser.UserName, chatName = SelectedChat.ChatName });
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", JwtToken);
                var content = new StringContent(data, Encoding.UTF8, "application/json");
                await client.PostAsync("https://localhost:7195/api/Chats/leavepublicchat", content);
            }
            else
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", JwtToken);
               await client.DeleteAsync($"https://localhost:7195/api/Chats/{SelectedChat.Id}");
            }
            SelectedChat = null;
            RigthInfoMenu.Width = new GridLength(0);
            ChatGrid.Visibility = Visibility.Hidden;
        }
        private async void ToogleButton_NotificationInfo_Click(object sender, MouseButtonEventArgs e)
        {
            if (e.Source != ToogleButton_DarkWhite)
            {
                ToogleButton_Notification_Info.IsChecked = !ToogleButton_Notification_Info.IsChecked;
                Notifications_Path_Info.Data = Geometry.Parse((ToogleButton_Notification_Info.IsChecked == true) ? "M10.9976 2.5V17.5C8.0809 17.5 5.9136 13.683 5.9136 13.683H3.49756C3.03733 13.683 2.66423 13.3099 2.66423 12.8496V7.08783C2.66423 6.62758 3.03733 6.2545 3.49756 6.2545H5.9136C5.9136 6.2545 8.0809 2.5 10.9976 2.5Z M14.3309 6.25C14.5906 6.48188 14.8259 6.74154 15.0326 7.0245C15.6387 7.85433 15.9976 8.88429 15.9976 10C15.9976 11.106 15.6448 12.1278 15.0483 12.9539C14.8379 13.2452 14.5972 13.5122 14.3309 13.75 M15.2625 17.1607C17.6991 15.7064 19.3309 13.0438 19.3309 10C19.3309 7.00356 17.7493 4.37648 15.3755 2.90796" : "M1.0107,0.976807 L19.3955,19.3616 M10.3036,3.4165 V18.4165 C7.38692,18.4165 5.21963,14.5995 5.21963,14.5995 H2.80359 C2.34335,14.5995 1.97026,14.2264 1.97026,13.7661 V8.00434 C1.97026,7.54409 2.34335,7.171 2.80359,7.171 H5.21963 C5.21963,7.171 7.38692,3.4165 10.3036,3.4165 Z M13.6369,7.1665 C13.8966,7.39838 14.132,7.65805 14.3386,7.941 C14.9447,8.77084 15.3036,9.8008 15.3036,10.9165 C15.3036,12.0225 14.9509,13.0443 14.3543,13.8704 C14.1439,14.1617 13.9032,14.4287 13.6369,14.6665 M14.5686,18.0772 C17.0051,16.6229 18.6369,13.9603 18.6369,10.9165 C18.6369,7.92006 17.0554,5.29298 14.6815,3.82446");
                //
                var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", JwtToken);
                var response = await client.SendAsync(new HttpRequestMessage { Method = new HttpMethod("PATCH"), RequestUri = new Uri($"https://localhost:7195/api/Chats/notifications/{SelectedChat.Id}") });
                var responseString = await response.Content.ReadAsStringAsync();
            }
        }
        private async void ToogleButton_MuteInfo_Click(object sender, RoutedEventArgs e)
        {
            ToogleButton_Notification_Info.IsChecked = !ToogleButton_Notification_Info.IsChecked;
            Notifications_Path_Info.Data = Geometry.Parse((ToogleButton_Notification_Info.IsChecked == true) ? "M10.9976 2.5V17.5C8.0809 17.5 5.9136 13.683 5.9136 13.683H3.49756C3.03733 13.683 2.66423 13.3099 2.66423 12.8496V7.08783C2.66423 6.62758 3.03733 6.2545 3.49756 6.2545H5.9136C5.9136 6.2545 8.0809 2.5 10.9976 2.5Z M14.3309 6.25C14.5906 6.48188 14.8259 6.74154 15.0326 7.0245C15.6387 7.85433 15.9976 8.88429 15.9976 10C15.9976 11.106 15.6448 12.1278 15.0483 12.9539C14.8379 13.2452 14.5972 13.5122 14.3309 13.75 M15.2625 17.1607C17.6991 15.7064 19.3309 13.0438 19.3309 10C19.3309 7.00356 17.7493 4.37648 15.3755 2.90796" : "M1.0107,0.976807 L19.3955,19.3616 M10.3036,3.4165 V18.4165 C7.38692,18.4165 5.21963,14.5995 5.21963,14.5995 H2.80359 C2.34335,14.5995 1.97026,14.2264 1.97026,13.7661 V8.00434 C1.97026,7.54409 2.34335,7.171 2.80359,7.171 H5.21963 C5.21963,7.171 7.38692,3.4165 10.3036,3.4165 Z M13.6369,7.1665 C13.8966,7.39838 14.132,7.65805 14.3386,7.941 C14.9447,8.77084 15.3036,9.8008 15.3036,10.9165 C15.3036,12.0225 14.9509,13.0443 14.3543,13.8704 C14.1439,14.1617 13.9032,14.4287 13.6369,14.6665 M14.5686,18.0772 C17.0051,16.6229 18.6369,13.9603 18.6369,10.9165 C18.6369,7.92006 17.0554,5.29298 14.6815,3.82446");
            //
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", JwtToken);
            var response = await client.SendAsync(new HttpRequestMessage { Method = new HttpMethod("PATCH"), RequestUri = new Uri($"https://localhost:7195/api/Chats/notifications/{SelectedChat.Id}") });
            var responseString = await response.Content.ReadAsStringAsync();
        }
        private async void ToogleButton_Notification_Click(object sender, RoutedEventArgs e)
        {
            Notifications_Path.Data = Geometry.Parse(ToogleButton_Notification.IsChecked.Value ? "M24 6V42C17 42 11.7985 32.8391 11.7985 32.8391H6C4.89543 32.8391 4 31.9437 4 30.8391V17.0108C4 15.9062 4.89543 15.0108 6 15.0108H11.7985C11.7985 15.0108 17 6 24 6Z M32 15L32 15C32.6232 15.5565 33.1881 16.1797 33.6841 16.8588C35.1387 18.8504 36 21.3223 36 24C36 26.6545 35.1535 29.1067 33.7218 31.0893C33.2168 31.7885 32.6391 32.4293 32 33 M34.2359 41.1857C40.0836 37.6953 44 31.305 44 24C44 16.8085 40.2043 10.5035 34.507 6.97906" : "M1.0107,0.976807 L19.3955,19.3616 M10.3036,3.4165 V18.4165 C7.38692,18.4165 5.21963,14.5995 5.21963,14.5995 H2.80359 C2.34335,14.5995 1.97026,14.2264 1.97026,13.7661 V8.00434 C1.97026,7.54409 2.34335,7.171 2.80359,7.171 H5.21963 C5.21963,7.171 7.38692,3.4165 10.3036,3.4165 Z M13.6369,7.1665 C13.8966,7.39838 14.132,7.65805 14.3386,7.941 C14.9447,8.77084 15.3036,9.8008 15.3036,10.9165 C15.3036,12.0225 14.9509,13.0443 14.3543,13.8704 C14.1439,14.1617 13.9032,14.4287 13.6369,14.6665 M14.5686,18.0772 C17.0051,16.6229 18.6369,13.9603 18.6369,10.9165 C18.6369,7.92006 17.0554,5.29298 14.6815,3.82446");
            //
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", JwtToken);
            var response = await client.SendAsync(new HttpRequestMessage { Method = new HttpMethod("PATCH"), RequestUri = new Uri($"https://localhost:7195/api/Chats/notifications/{SelectedChat.Id}") });
            var responseString = await response.Content.ReadAsStringAsync();
        }
        private async void ReadAllMessage_Click(object sender, RoutedEventArgs e)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", JwtToken);
            var data = JsonConvert.SerializeObject(new { chatId = SelectedChat.Id, userId = LoginedUser.Id });
            var content = new StringContent(data, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("https://localhost:7195/api/Messages/readmessages/", content);
            var responseString = await response.Content.ReadAsStringAsync();
            if (responseString == null)
            {
                MessageBox.Show("Server error...");
                this.Close();
                return;
            }
        }
        private async void SearchTextChanged(object sender, TextChangedEventArgs e)
        {
            string text = ((TextBox)(sender as TextBox).Template.FindName("MainTextBox", sender as TextBox)).Text;
            var client = new HttpClient();
            var data = JsonConvert.SerializeObject(new { text });
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", JwtToken);
            var content = new StringContent(data, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("https://localhost:7195/api/Chats/findchats", content);
            var responseString = await response.Content.ReadAsStringAsync();
            if (responseString == null)
            {
                MessageBox.Show("Server error...");
                this.Close();
                return;
            }
            var result = JsonConvert.DeserializeAnonymousType(responseString, new { users = new List<User>(), chats = new List<Chat>() });
            //
            Contact_Search_Users.Items.Clear();
            Contact_Search.Items.Clear();
            //
            bool flag = false;
            if (result?.users != null)
            {
                if (result.users.Count > 0)
                {
                    flag = true;
                    foreach (var user in result.users)
                    {
                        if (UserContacts.FirstOrDefault(cont => cont.UserName == user.UserName) != null)
                        {
                            user.VisibilityAddContact = Visibility.Collapsed;
                        }
                        Contact_Search_Users.Items.Add(user);
                    }
                }
            }
            if (result?.chats != null)
            {
                if (result.chats.Count > 0)
                {
                    flag = true;
                    foreach (var chat in result.chats)
                    {
                        Contact_Search.Items.Add(chat);
                    }
                }
            }
            Contact_ListView.Visibility = (flag) ? Visibility.Collapsed : Visibility.Visible;
        }
        private async void SearchChat_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Contact_Search.SelectedItems.Count > 0)
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", JwtToken);
                var data = JsonConvert.SerializeObject(new { userName = LoginedUser.UserName, chatName = (Contact_Search.SelectedItem as Chat).ChatName });
                var content = new StringContent(data, Encoding.UTF8, "application/json");
                await client.PostAsync("https://localhost:7195/api/Chats/enterpublicchat", content);
                // Select new chat
                List<Chat> myList = Contact_ListView.Items.Cast<Chat>().ToList();
                Contact_ListView.SelectedIndex = myList.IndexOf(myList.FirstOrDefault(chat => chat.ChatName == (Contact_Search.SelectedItem as Chat).ChatName));
            }
            // Clear search lists
            ((TextBox)SearchTextBox.Template.FindName("MainTextBox", SearchTextBox)).Text = "";
        }
        private async void SearchUser_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Contact_Search_Users.SelectedItems.Count > 0)
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", JwtToken);
                var data = JsonConvert.SerializeObject(new { userName = LoginedUser.UserName, opponentName = (Contact_Search_Users.SelectedItem as User).UserName });
                var content = new StringContent(data, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("https://localhost:7195/api/Chats/enterprivatechat", content);
                var responseString = await response.Content.ReadAsStringAsync();
                if (responseString == null)
                {
                    MessageBox.Show("Server error...");
                    this.Close();
                    return;
                }
                // Select new chat
                List<Chat> myList = Contact_ListView.Items.Cast<Chat>().ToList();
                Contact_ListView.SelectedIndex = myList.IndexOf(myList.FirstOrDefault(chat => chat.ChatName == (Contact_Search_Users.SelectedItem as User).UserName));
            }
            // Clear search lists
            ((TextBox)SearchTextBox.Template.FindName("MainTextBox", SearchTextBox)).Text = "";
        }
        private void Close_RigthMenu_Click(object sender, RoutedEventArgs e)
        {
            RigthInfoMenu.Width = new GridLength(0);
        }
        private void OpenPointMenuInfo_Click(object sender, RoutedEventArgs e)
        {
            ThreePointsInfo.IsSubmenuOpen = true;
        }
        private async void Leave_SelectedInfo_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (SelectedChat.Type == "Channel" || SelectedChat.Type == "Group")
            {
                var client = new HttpClient();
                var data = JsonConvert.SerializeObject(new { userName = LoginedUser.UserName, chatName = SelectedChat.ChatName });
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", JwtToken);
                var content = new StringContent(data, Encoding.UTF8, "application/json");
                await client.PostAsync("https://localhost:7195/api/Chats/leavepublicchat", content);
            }
            else
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", JwtToken);
                await client.DeleteAsync($"https://localhost:7195/api/Chats/{SelectedChat.Id}");
            }
            SelectedChat = null;
            RigthInfoMenu.Width = new GridLength(0);
            // Close info
            Menu_Info_Grid.Visibility = Visibility.Hidden;
            // Close selected message
            ChatGrid.Visibility = Visibility.Hidden;
        }
        private void Edit_SelectedInfo_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Edit_Info_Grid.Visibility = Visibility.Visible;
            EditInfoEllipse.Fill = new ImageBrush(SelectedChat.PhotoSource);
            if (SelectedChat.PhotoSource == null)
            {
                PhotoPath.Visibility = Visibility.Visible;
            }
            ((TextBox)Channel_Name.Template.FindName("MainTextBox", Channel_Name as TextBox)).Text = SelectedChat.ChatName;
            ((TextBox)Channel_Description.Template.FindName("MainTextBox", Channel_Description as TextBox)).Text = SelectedChat.ChatInfo;
        }
        private void EditImageEditChannel(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg|All files (*.*)|*.*",
                RestoreDirectory = true
            };
            if (openFileDialog.ShowDialog() == true)
            {
                byte[] tbytes = File.ReadAllBytes(openFileDialog.FileName);
                string extension = Path.GetExtension(openFileDialog.SafeFileName);
                SelectedPhoto = $"data:image/{extension.Substring(1)};base64," + Convert.ToBase64String(tbytes);
                //
                string imagePath = openFileDialog.FileName;
                BitmapImage bitmap = new BitmapImage(new Uri(imagePath, UriKind.RelativeOrAbsolute));
                byte[] bytes;
                using (MemoryStream ms = new MemoryStream())
                {
                    PngBitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(bitmap));
                    encoder.Save(ms);
                    bytes = ms.ToArray();
                }
                ImageSource image = null;
                try
                {
                    image = BitmapFrame.Create(new MemoryStream(bytes));
                    PhotoPath.Visibility = Visibility.Hidden;
                    EditInfoEllipse.Fill = new ImageBrush(image);
                }
                catch (Exception ex) { MessageBox.Show($"Error: {ex.Message}"); }
            }
        }
        private void Close_EditChat_Menu_Click(object sender, RoutedEventArgs e)
        {
            Edit_Info_Grid.Visibility = Visibility.Hidden;
        }
        private async void Edit_Chat_Click(object sender, RoutedEventArgs e)
        {
            string chatImage = SelectedPhoto;
            string chatName = ((TextBox)Channel_Name.Template.FindName("MainTextBox", Channel_Name as TextBox)).Text;
            string chatInfo = ((TextBox)Channel_Description.Template.FindName("MainTextBox", Channel_Description as TextBox)).Text;
            var client = new HttpClient();
            var data = JsonConvert.SerializeObject(new { id = SelectedChat.Id, chatImage, chatName, chatInfo });
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", JwtToken);
            var content = new StringContent(data, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("https://localhost:7195/api/Chats/editpublicchat", content);
            var responseString = await response.Content.ReadAsStringAsync();
            if (responseString == null)
            {
                MessageBox.Show("Server error...");
                this.Close();
                return;
            }
            Edit_Info_Grid.Visibility = Visibility.Hidden;
            Menu_Info_Grid.Visibility = Visibility.Hidden;
        }
        private void Close_Edit_Menu(object sender, MouseButtonEventArgs e)
        {
            if (e.Source == Edit_Info_Grid)
                Edit_Info_Grid.Visibility = Visibility.Hidden;
        }
        private void Menu_Contacts_Open(object sender, MouseButtonEventArgs e)
        {
            ContactsList_Settings.ItemsSource = UserContacts;
            Contacts_Settings_Grid.Visibility = Visibility.Visible;
        }
        private void Close_ContactsSettings_Menu(object sender, MouseButtonEventArgs e)
        {
            if (e.Source == Contacts_Settings_Grid)
                Contacts_Settings_Grid.Visibility = Visibility.Hidden;
        }
        private async void DeleteContact_Click(object sender, RoutedEventArgs e)
        {
            if (ContactsList_Settings.SelectedItem is User Selectedcontact)
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", JwtToken);
                var data = JsonConvert.SerializeObject(new { currentUserLogin = LoginedUser.UserName, contactUserName = Selectedcontact.UserName });
                var content = new StringContent(data, Encoding.UTF8, "application/json");
                var response = await client.SendAsync(new HttpRequestMessage { Method = new HttpMethod("DELETE"), RequestUri = new Uri("https://localhost:7195/api/UserContacts/deletecontact"), Content = content });
                var responseString = await response.Content.ReadAsStringAsync();
                if (responseString == null)
                {
                    MessageBox.Show("Server error...");
                    return;
                }
            }
        }
        private void UserSearch_ContextOpen(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            var element = sender as FrameworkElement;
            var menu = element.ContextMenu;
            if (menu != null)
            {
                menu.PlacementTarget = element;
                menu.IsOpen = true;
            }
        }
        private async void Addcontact_Click(object sender, RoutedEventArgs e)
        {
            int.TryParse((sender as MenuItem).Tag.ToString(), out int id);
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", JwtToken);
            var response = await client.GetAsync($"https://localhost:7195/api/Users/{id}");
            var responseString = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeAnonymousType(responseString, new { user = new User() });
            //
            var data = JsonConvert.SerializeObject(new { currentUserLogin = LoginedUser.UserName, contactUserName = result.user.UserName });
            var content = new StringContent(data, Encoding.UTF8, "application/json");
            await client.PostAsync("https://localhost:7195/api/UserContacts/createcontact", content);
        }
        private void TextBoxCode_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back) return;
            if (e.Key == Key.Delete) return;
            try
            {
                if (!char.IsDigit((sender as TextBox).Text[0]))
                {
                    (sender as TextBox).Text = "";
                    e.Handled = true;
                }
                else
                {
                    var textBox = sender as TextBox;
                    if (char.IsDigit(textBox.Text[0]))
                    {
                        if (textBox.Tag == null) return;
                        int.TryParse(textBox.Tag.ToString(), out int index);
                        if (index != 0)
                        {
                            if (FindName("TextBoxCode" + (++index)) is TextBox nextTextBox)
                            {
                                nextTextBox.Focus();
                                nextTextBox.SelectAll();
                                e.Handled = true;
                            }
                        }
                    }
                }
            }
            catch { }
        }
        private void Close_EditPasswordCode_Menu(object sender, MouseButtonEventArgs e)
        {
            if (e.Source == Menu_EditPasswordCode_Grid)
                Menu_EditPasswordCode_Grid.Visibility = Visibility.Hidden;
        }
        private void Close_EditPasswordCode_Click(object sender, RoutedEventArgs e)
        {
            Menu_EditPasswordCode_Grid.Visibility = Visibility.Hidden;
        }
        private void Check_EditPasswordCode_Click(object sender, RoutedEventArgs e)
        {
            string code = (TextBoxCode1.Text + TextBoxCode2.Text + TextBoxCode3.Text + TextBoxCode4.Text + TextBoxCode5.Text);
            if (code.Length == 5 && code == codeString)
            {
                Menu_EditPassword_Grid.Visibility = Visibility.Visible;
                Menu_EditPasswordCode_Grid.Visibility = Visibility.Hidden;
            }
        }
        private void BlockClipboard_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key == Key.V) && ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control))
            {
                e.Handled = true;
            }
        }
        private void Theme_Checked(object sender, RoutedEventArgs e)
        {
            string name = (sender as RadioButton).Name;
            ResourceDictionary resource = new ResourceDictionary();
            string uri = $"Themes/MainWindow/Skins/{name}.xaml";
            resource.Source = new Uri(uri, UriKind.Relative);
            this.Resources = resource;
        }
        private void Piker_SelectionChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            ((TextBox)(TextBox_MessageBox).Template.FindName("MainTextBox", TextBox_MessageBox)).AppendText(Piker.Selection);
        }
    }
}
