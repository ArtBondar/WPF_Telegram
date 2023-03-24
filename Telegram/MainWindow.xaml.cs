using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
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
    public partial class MainWindow : Window
    {
        public string JwtToken { get; set; }
        public User LoginedUser { get; set; }
        public List<Chat> Chats { get; set; }
        public List<SavedMessage> SavedMessages { get; set; }
        public void RefreshUI()
        {
            Dispatcher.Invoke(() =>
            {
                if (LoginedUser != null)
                {
                    Username_Lable_LeftMenu.Content = LoginedUser.UserName.ToString();
                    Email_Lable_LeftMenu.Content = LoginedUser.Email;
                    Photo_ImageBrush_LeftMenu.ImageSource = LoginedUser.PhotoSource;
                    // Chats
                    Contact_ListView.ItemsSource = Chats.OrderByDescending(chat => chat.MuteStatus);
                    // Settings
                    SettingsImageBrush.ImageSource = LoginedUser.PhotoSource;
                    SettingsEditEmail_Lable.Content = SettingsEmail_Lable.Content = LoginedUser.Email;
                    SettingsEditUserName_Lable.Content = SettingsUserName_Lable.Content = LoginedUser.UserName;
                    SettingsDescription_Lable.Content = LoginedUser.AboutUser;
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
                    return;
                }
                var result = JsonConvert.DeserializeAnonymousType(responseString, new { user = new Models.User(), chats = new List<Models.Chat>(), savedMessages = new List<SavedMessage>() });
                if(result.user != null)
                {
                    LoginedUser = result.user;
                    Chats = result.chats;
                    SavedMessages = result.savedMessages;
                }
            }
        }
        public MainWindow()
        {
            InitializeComponent();
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(3);
            timer.Tick += OnTimerElapsed;
            timer.Start();
        }
        private void OnTimerElapsed(object sender, EventArgs e)
        {
            RefreshDate();
            RefreshUI();
        }
        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
                WindowState = WindowState.Normal;
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }
        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        private void WindowsStateButton_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState != WindowState.Maximized)
                WindowState = WindowState.Maximized;
            else
                WindowState = WindowState.Normal;
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
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
        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ChatGrid.Visibility = Visibility.Visible;
            Chat Select = (sender as ListView).SelectedItem as Chat;
            if (Select == null) return;
            ChatPanel_Image.ImageSource = Select.PhotoSource;
            ChatPanel_Name.Content = Select.ChatName;
            if (Select.Type == "Group")
            {
                ChatPanel_SecondInfo.Content = $"{Select.MembersCount} members";
                if (!String.IsNullOrWhiteSpace(Select.ChatInfo))
                {
                    Info1_NameLable.Content = "About";
                    Info1_Lable.Content = Select.ChatInfo;
                }
                else
                {
                    Info1_NameLable.Content = "";
                    Info1_Lable.Content = "";
                }
                Info2_NameLable.Content = "Username";
                Info2_Lable.Content = Select.ChatName;
                Info3_NameLable.Content = "";
                Info3_Lable.Content = "";
                RigthInfo_Second.Content = $"{Select.MembersCount} members";
            }
            if (Select.Type == "Channel")
            {
                ChatPanel_SecondInfo.Content = $"{Select.MembersCount} members";
                if (!String.IsNullOrWhiteSpace(Select.ChatInfo))
                {
                    Info1_NameLable.Content = "About";
                    Info1_Lable.Content = Select.ChatInfo;
                }
                else
                {
                    Info1_NameLable.Content = "";
                    Info1_Lable.Content = "";
                }
                Info2_NameLable.Content = "";
                Info2_Lable.Content = "";
                Info3_NameLable.Content = "";
                Info3_Lable.Content = "";
                RigthInfo_Second.Content = $"{Select.MembersCount} members";
            }
            if (Select.Type == "Private")
            {
                RigthInfo_Second.Content = "d";
                ChatPanel_SecondInfo.Content = Select.PublishTime;
                Info1_NameLable.Content = "About";
                Info1_Lable.Content = Select.ChatInfo;
                Info2_NameLable.Content = "Username";
                Info2_Lable.Content = Select.ChatName;
                Info3_NameLable.Content = "";
                Info3_Lable.Content = "";
            }
            if (Select.Type == "Favorite")
            {
                Info1_NameLable.Content = "";
                Info1_Lable.Content = "";
                ChatPanel_SecondInfo.Content = "";
                Info2_NameLable.Content = "";
                Info2_Lable.Content = "";
                Info3_NameLable.Content = "";
                Info3_Lable.Content = "";
            }
            // Messages
            Chat_ListView.ItemsSource = Select.ChatMessages;
            // RigthInfo
            RigthInfoImage.ImageSource = Select.PhotoSource;
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
            var lastItem = Chat_ListView.Items[Chat_ListView.Items.Count - 1];
            Chat_ListView.ScrollIntoView(lastItem);
            Button_To_Down.Visibility = Visibility.Hidden;
        }
        private void Chat_ListView_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.VerticalOffset == 0)
                Button_To_Down.Visibility = Visibility.Visible;
            else
                Button_To_Down.Visibility = Visibility.Hidden;
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
        private void Menu_Settings_Edit_Avatar(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg|All files (*.*)|*.*",
                RestoreDirectory = true
            };

            if (openFileDialog.ShowDialog() == true)
            {
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
                Ellipse_Avatar.Fill = new ImageBrush(image);
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
    }
}
