using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Telegram.Models;

namespace Telegram
{
    /// <summary>
    /// Interaction logic for StartLoading.xaml
    /// </summary>
    public partial class StartLoading : Window
    {
        public StartLoading()
        {
            InitializeComponent();
        }
        // Защита пароля
        private static readonly byte[] entropy = Encoding.Unicode.GetBytes("@Criptic/1028490275bbcc");

        // Загрузка логина и пароля из файла
        public static bool LoadCredentialsFromFile(string fileName, out string login, out string password)
        {
            try
            {
                // Чтение логина и пароля из файла
                using (StreamReader reader = new StreamReader(fileName))
                {
                    login = reader.ReadLine();
                    byte[] encryptedPassword = Convert.FromBase64String(reader.ReadLine());

                    // Расшифровка пароля
                    byte[] decryptedPassword = ProtectedData.Unprotect(encryptedPassword, entropy, DataProtectionScope.CurrentUser);
                    password = Encoding.Unicode.GetString(decryptedPassword);
                }
                return true;
            }
            catch
            {
                login = null;
                password = null;
                return false;
            }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await Task.Run(async () =>
            {
                Thread.Sleep(3000);
                string loadedLogin, loadedPassword;
                if (LoadCredentialsFromFile("login.txt", out loadedLogin, out loadedPassword))
                {
                    Console.WriteLine($"Логин: {loadedLogin}");
                    Console.WriteLine($"Пароль: {loadedPassword}");
                }
                else
                {
                    SingUp singUp = new SingUp();
                    singUp.Show();
                    this.Close();
                }
                var client = new HttpClient();
                var data = JsonConvert.SerializeObject(new { login = loadedLogin, password = loadedPassword });
                var content = new StringContent(data, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("https://localhost:7195/api/Users/login", content);
                var responseString = await response.Content.ReadAsStringAsync();
                if (responseString == null)
                {
                    MessageBox.Show("Server error...");
                    return;
                }
                var result = JsonConvert.DeserializeAnonymousType(responseString, new { jwtToken = "", user = new Models.User(), chats = new List<Models.Chat>(), contacts = new List<User>() });
                if (!String.IsNullOrWhiteSpace(result.jwtToken))
                {
                    // Open Main Form
                    Dispatcher.Invoke(() =>
                    {
                        Criptic mainForm = new Criptic();
                        mainForm.JwtToken = result.jwtToken;
                        mainForm.LoginedUser = result.user;
                        mainForm.Chats = result.chats;
                        mainForm.UserContacts = result.contacts;
                        mainForm.RefreshUI();
                        mainForm.Show();
                        this.Close();
                    });
                }
            });
        }
    }
}
