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
using Telegram.ViewModels;

namespace Telegram
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string JwtToken { get; set; }
        public User LoginedUser { get; set; }
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
        public List<User> SelectedContacts { get; set; } = new List<User>();
        //
        public void RefreshUI()
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
                        LeftMenuEllipseText.Text = LoginedUser.UserName.Substring(0,2).ToUpper();
                    }
                    else
                        LeftMenuEllipse.Fill = new ImageBrush(LoginedUser.PhotoSource);
                    // Chats
                    Contact_ListView.ItemsSource = Chats.OrderByDescending(chat => chat.PublishTime);
                    if (SelectedChat != null)
                    {
                        List<Chat> list = Contact_ListView.Items.Cast<Chat>().ToList();
                        Contact_ListView.SelectedIndex = list.IndexOf(list.FirstOrDefault(chat => chat.Id == SelectedChat.Id));
                    }
                        
                    // Settings
                    if (LoginedUser.PhotoSource == null)
                    {
                        Ellipse_Avatar.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#B8ACFF"));
                        SettingsEllipseText.Text = LoginedUser.UserName.Substring(0, 2).ToUpper();
                    }
                    else
                        Ellipse_Avatar.Fill = new ImageBrush(LoginedUser.PhotoSource);
                    SettingsEditEmail_Lable.Content = SettingsEmail_Lable.Content = LoginedUser.Email;
                    SettingsEditUserName_Lable.Content = SettingsUserName_Lable.Content = LoginedUser.UserName;
                    if (!String.IsNullOrWhiteSpace(LoginedUser.AboutUser))
                    {
                        SettingsDescription_Lable.Text = LoginedUser.AboutUser;
                        SettingsDescription_Lable.Foreground = Brushes.White;
                    }
                    else {
                        SettingsDescription_Lable.Foreground = Brushes.Gray;
                        SettingsDescription_Lable.Text = "Description...";
                    }
                    // Create Group and Channel
                    ContactsList.ItemsSource = UserContacts;
                    if(SelectedContacts.Count > 0)
                    {
                        List<User> list = ContactsList.Items.Cast<User>().ToList();
                        var copyCollection = new List<User>(SelectedContacts);
                        foreach (User contact in copyCollection)
                        {
                            ContactsList.SelectedItems.Add(list.FirstOrDefault(user => user.Id == contact.Id));
                        }
                    }
                }
            });
        }
        public async void RefreshDate()
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
                if(result.user != null)
                {
                    LoginedUser = result.user;
                    Chats = result.chats;
                    SavedMessages = result.savedMessages;
                    UserContacts = result.contacts;
                }
            }
        }
        public MainWindow()
        {
            InitializeComponent();
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += OnTimerElapsed;
            timer.Start();
        }
        private void OnTimerElapsed(object sender, EventArgs e)
        {
            RefreshDate();
            RefreshUI();
        }
        private void Close_Left_Menu(object sender, MouseButtonEventArgs e)
        {
            Left_Menu_Grid.Width = 0;
        }
        private void ToogleButton_Notification_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.Source != ToogleButton_Notification)
            {
                ToogleButton_Notification.IsChecked = !ToogleButton_Notification.IsChecked;
                Notifications_Path.Data = Geometry.Parse(ToogleButton_Notification.IsChecked.Value ? "M24 6V42C17 42 11.7985 32.8391 11.7985 32.8391H6C4.89543 32.8391 4 31.9437 4 30.8391V17.0108C4 15.9062 4.89543 15.0108 6 15.0108H11.7985C11.7985 15.0108 17 6 24 6Z M32 15L32 15C32.6232 15.5565 33.1881 16.1797 33.6841 16.8588C35.1387 18.8504 36 21.3223 36 24C36 26.6545 35.1535 29.1067 33.7218 31.0893C33.2168 31.7885 32.6391 32.4293 32 33 M34.2359 41.1857C40.0836 37.6953 44 31.305 44 24C44 16.8085 40.2043 10.5035 34.507 6.97906" : "M40.7348,20.2858 L32.2495,28.7711 M32.2496,20.2858 L40.7349,28.7711 M24,6 V42 C17,42 11.7985,32.8391 11.7985,32.8391 H6 C4.89543,32.8391 4,31.9437 4,30.8391 V17.0108 C4,15.9062 4.89543,15.0108 6,15.0108 H11.7985 C11.7985,15.0108 17,6 24,6 Z");
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
            if(Select.Type == "Private")
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
            var result = JsonConvert.DeserializeAnonymousType(responseString, new { error = "", chat = new Chat(), messages = new List<UserMessageViewModel>(), members = new List<User>() });
            //
            if (result.chat == null) return;
            SelectedChat = result.chat;
            if (Select.PhotoSource == null)
            {
                SelectedEllipse.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#B8ACFF"));
                SelectedEllipseText.Text = Select.PhotoText;
            }
            else
                SelectedEllipse.Fill = new ImageBrush(Select.PhotoSource);
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
            //
            if (Select.Type == "Group")
            {
                ChatPanel_SecondInfo.Content = $"{Select.MembersCount} members";
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
                Info2_NameLable.Content = "Group name"; Info2_NameLable.Visibility = Visibility.Visible;
                Info2_Lable.Content = Select.ChatName; Info2_Lable.Visibility = Visibility.Visible;
                RigthInfo_Second.Content = $"{Select.MembersCount} members";
            }
            if (Select.Type == "Channel")
            {
                ChatPanel_SecondInfo.Content = $"{Select.MembersCount} members";
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
                RigthInfo_Second.Content = $"{Select.MembersCount} members";
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
            foreach (UserMessageViewModel message in result.messages)
            {
                if(message.Author.Id == LoginedUser.Id)
                    message.VisibilityDeleteMessage = Visibility.Visible;
                else
                    message.VisibilityDeleteMessage = Visibility.Collapsed;
            }
            Chat_ListView.ItemsSource = result.messages;

            // RigthInfo
            if (Select.PhotoSource == null)
            {
                RigthInfoEllipse.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#B8ACFF"));
                RigthInfoEllipseText.Text = Select.PhotoText;
            }
            else
                RigthInfoEllipse.Fill = new ImageBrush(Select.PhotoSource);
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
        private void Chat_ListView_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            // Получить ScrollViewer из ListView
            ScrollViewer scrollViewer = GetDescendantByType(sender as ListView, typeof(ScrollViewer)) as ScrollViewer;

            // Если ScrollViewer находится в нижней части, скрыть кнопку Button_To_Down
            if (scrollViewer.VerticalOffset == scrollViewer.ScrollableHeight)
            {
                Button_To_Down.Visibility = Visibility.Hidden;
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
                if(result.user != null)
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
        private void Open_EditPassword_ButtonUp(object sender, MouseButtonEventArgs e)
        {
            Menu_EditPassword_Grid.Visibility = Visibility.Visible;
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
                }
                catch (Exception ex) { MessageBox.Show($"Error: {ex.Message}"); }
                Ellipse_CreateGroup.Fill = new ImageBrush(image);
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
                }
                catch (Exception ex) { MessageBox.Show($"Error: {ex.Message}"); }
                Ellipse_CreateChannel.Fill = new ImageBrush(image);
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
                var data = JsonConvert.SerializeObject(new { authorId = LoginedUser.Id, chatImage = SelectedPhoto, chatName = GroupOrChannelName, shortMessage = "Group created...", publishTime = DateTime.Now, type = "Group" });
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
            Menu_CreateGroup_Grid.Visibility = Visibility.Hidden;
            Contacts_Create_Grid.Visibility = Visibility.Visible;
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
                var data = JsonConvert.SerializeObject(new { authorId = LoginedUser.Id, chatImage = SelectedPhoto, chatName = GroupOrChannelName, shortMessage = "Channel created...", publishTime = DateTime.Now, type = "Channel", chatInfo = ChannelDescription });
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
            Menu_CreateChanel_Grid.Visibility = Visibility.Hidden;
            Contacts_Create_Grid.Visibility = Visibility.Visible;
        }
        private void AddMembersClick(object sender, RoutedEventArgs e)
        {
            if (CreateGroupOrChannel == CreateFrag.Null)
            {
                return;
            }
            if (ContactsList.Items.Count != 0)
            {
                List<User> users = ContactsList.Items.Cast<User>().ToList();
                //...
            }
            if(CreateGroupOrChannel == CreateFrag.Group)
            {
                // Create group
            }
            if(CreateGroupOrChannel == CreateFrag.Channel)
            {
                // Create channel
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
                var data = JsonConvert.SerializeObject(new { userId = LoginedUser.Id, chatId = (Contact_ListView.SelectedItem as Chat).Id, text = thistextBox.Text, data = String.Empty }); // Add data
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
            Menu_Info_Grid.Visibility = Visibility.Visible;
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
                InfoEllipse.Fill = new ImageBrush(SelectedChat.PhotoSource);
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
    }
}
