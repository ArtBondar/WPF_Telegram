using Newtonsoft.Json;
using System;
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
        public string API_STRING = "https://jumedoo-001-site1.atempurl.com/api";
        public StartLoading()
        {
            InitializeComponent();
        }
        private static readonly byte[] entropy = Encoding.Unicode.GetBytes("@Criptic/1028490275bbcc");

        // Loading from file
        public static bool LoadDateFromFile(string fileName, out string login, out string password)
        {
            try
            {
                using (StreamReader reader = new StreamReader(fileName))
                {
                    login = reader.ReadLine();
                    byte[] encryptedPassword = Convert.FromBase64String(reader.ReadLine());

                    
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
                Thread.Sleep(1000);
                if (LoadDateFromFile("login.txt", out string loadedLogin, out string loadedPassword))
                {
                    try
                    {
                        var client = new HttpClient();
                        var data = JsonConvert.SerializeObject(new { login = loadedLogin, password = loadedPassword });
                        var content = new StringContent(data, Encoding.UTF8, "application/json");
                        var response = await client.PostAsync($"{API_STRING}/Users/login", content);
                        var responseString = await response.Content.ReadAsStringAsync();
                        if (responseString == null) return;
                        var result = JsonConvert.DeserializeAnonymousType(responseString, new { jwtToken = "", user = new User() });
                        if (!String.IsNullOrWhiteSpace(result.jwtToken))
                        {
                            // Open Main Form
                            Dispatcher.Invoke(() =>
                            {
                                Criptic mainForm = new Criptic
                                {
                                    JwtToken = result.jwtToken,
                                    LoginedUser = result.user
                                };
                                mainForm.Show();
                                this.Close();
                            });
                        }
                    }
                    catch { }
                }
                else
                {
                    SingUp singUp = new SingUp();
                    singUp.Show();
                    this.Close();
                }
            });
        }
    }
}
