using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Telegram.Models;

namespace Telegram
{
    /// <summary>
    /// Interaction logic for SingUp.xaml
    /// </summary>
    public partial class SingUp : Window
    {
        private bool flag_singup = true;
        private bool flag_registration = true;
        private bool flag_newPassword = true;
        private string codeString = "";
        private string emailString = "";
        
        public SingUp()
        {
            InitializeComponent();
        }

        private void SingUp_Start(object sender, RoutedEventArgs e)
        {
            StartStackPanel.Visibility = Visibility.Hidden;
            SingUpStackPanel.Visibility = Visibility.Visible;
        }

        private void ButtonShowPassword_Click(object sender, RoutedEventArgs e)
        {
            flag_singup = !flag_singup;
            TextBoxPasswordLogin.Visibility = (flag_singup) ? Visibility.Hidden : Visibility.Visible;
            PasswordBoxLogin.Visibility = (flag_singup) ? Visibility.Visible : Visibility.Hidden;
            TextBoxPasswordLogin.Text = PasswordBoxLogin.Password;
        }

        private void pbPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PlaceHolderTextBlock.Visibility = (sender as PasswordBox).Password.Length == 0 ? Visibility.Visible : Visibility.Hidden;
        }

        private void TextBoxPassword_TextChanged(object sender, TextChangedEventArgs e)
        {
            PasswordBoxLogin.Password = (sender as TextBox).Text;
        }

        private void ButtonShowPassword_Registration_Click(object sender, RoutedEventArgs e)
        {
            flag_registration = !flag_registration;
            TextBoxPasswordRegistration.Visibility = (flag_registration) ? Visibility.Hidden : Visibility.Visible;
            PasswordBoxRegistration.Visibility = (flag_registration) ? Visibility.Visible : Visibility.Hidden;
            TextBoxPasswordRegistration.Text = PasswordBoxRegistration.Password;
        }

        private void PasswordForgotPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PlaceHolderTextBlockRegistration.Visibility = (sender as PasswordBox).Password.Length == 0 ? Visibility.Visible : Visibility.Hidden;
        }

        private void TextBoxPasswordForgotPassword_TextChanged(object sender, TextChangedEventArgs e)
        {
            PasswordBoxRegistration.Password = (sender as TextBox).Text;
        }

        private void TextBlock_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            SingUpStackPanel.Visibility = Visibility.Hidden;
            ForgotPasswordStackPanelEmail.Visibility = Visibility.Visible;
        }

        private async void ButtonSendEmail_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(((TextBox)EmailSendCode_TextBox.Template.FindName("MainTextBox", EmailSendCode_TextBox)).Text))
            {
                var client = new HttpClient();
                string email = ((TextBox)EmailSendCode_TextBox.Template.FindName("MainTextBox", EmailSendCode_TextBox)).Text;
                var data = JsonConvert.SerializeObject(new { email });
                var content = new StringContent(data, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("https://localhost:7195/api/Email/sendcode", content);
                var responseString = await response.Content.ReadAsStringAsync();
                if (responseString == "The specified string is not in the form required for an e-mail address.")
                {
                    ((TextBox)TextBoxEmailLogin.Template.FindName("MainTextBox", EmailSendCode_TextBox)).Foreground = Brushes.Red;
                    return;
                }
                var result = JsonConvert.DeserializeAnonymousType(responseString, new { code = "", email = "" });
                codeString = result?.code;
                emailString = result?.email;
            }
            ForgotPasswordStackPanelEmail.Visibility = Visibility.Hidden;
            ForgotPasswordStackPanel.Visibility = Visibility.Visible;
            TextBoxCode1.Focus(); // Focus Code
        }

        private void TextBoxPasswordRegistration_TextChanged(object sender, TextChangedEventArgs e)
        {
            PasswordBoxRegistration.Password = (sender as TextBox).Text;
            (sender as TextBox).Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#C6BDFF"));
        }

        private void PasswordBoxRegistration_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PlaceHolderTextBlockRegistration.Visibility = (sender as PasswordBox).Password.Length == 0 ? Visibility.Visible : Visibility.Hidden;
            (sender as PasswordBox).Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#C6BDFF"));
        }


        private void RegistrationPassword2_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PlaceHolderTextBlockRegistration2.Visibility = (sender as PasswordBox).Password.Length == 0 ? Visibility.Visible : Visibility.Hidden;
            (sender as PasswordBox).Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#C6BDFF"));
        }

        private void OpenRegistrationMenuButton_Click(object sender, RoutedEventArgs e)
        {
            SingUpStackPanel.Visibility = Visibility.Hidden;
            RegistrationStackPanel.Visibility = Visibility.Visible;
        }

        private void OnlyNumberPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            bool flag = regex.IsMatch(e.Text);
            e.Handled = flag;
        }
        private void TextBoxCode_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back) return;
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

        private async void ButtonSingUp_Click(object sender, RoutedEventArgs e)
        {
            var client = new HttpClient();
            string login = ((TextBox)TextBoxEmailLogin.Template.FindName("MainTextBox", TextBoxEmailLogin)).Text;
            string password = PasswordBoxLogin.Password;
            var data = JsonConvert.SerializeObject(new { login, password });
            var content = new StringContent(data, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("https://localhost:7195/api/Users/login", content);
            var responseString = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject(responseString);
            // Open Main Form
        }

        private async void Registration_Button_Click(object sender, RoutedEventArgs e)
        {
            if (TextBoxPasswordRegistration2.Text != PasswordBoxRegistration2.Password.ToString())
            {
                PasswordBoxRegistration2.Foreground = Brushes.Red;
                PasswordBoxRegistration.Foreground = Brushes.Red;
                TextBoxPasswordRegistration.Foreground = Brushes.Red;
                return;
            }
            var client = new HttpClient();
            string userName = ((TextBox)RegistrationEmail_TextBox.Template.FindName("MainTextBox", RegistrationEmail_TextBox)).Text;
            string email = ((TextBox)RegistrationUserName_TextBox.Template.FindName("MainTextBox", RegistrationUserName_TextBox)).Text;
            string password = PasswordBoxRegistration.Password;
            var data = JsonConvert.SerializeObject(new { userName, email, password });
            var content = new StringContent(data, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("https://localhost:7195/api/Users/register", content);
            var responseString = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject(responseString);
            // Open Main Form
        }

        private void TextBoxTextSetForeground(object sender, TextChangedEventArgs e)
        {
            ((TextBox)(sender as TextBox).Template.FindName("MainTextBox", (sender as TextBox))).Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#C6BDFF"));
        }

        private void ExaminationCode_Click(object sender, RoutedEventArgs e)
        {
            string code = (TextBoxCode1.Text + TextBoxCode2.Text + TextBoxCode3.Text + TextBoxCode4.Text + TextBoxCode5.Text);
            if(code.Length == 5 && code == codeString)
            {
                NewPasswordStackPanel.Visibility = Visibility.Visible;
                ForgotPasswordStackPanel.Visibility = Visibility.Hidden;
            }
            else
            {
                TextBoxCode1.Foreground = Brushes.Red;
                TextBoxCode2.Foreground = Brushes.Red;
                TextBoxCode3.Foreground = Brushes.Red;
                TextBoxCode4.Foreground = Brushes.Red;
                TextBoxCode5.Foreground = Brushes.Red;
            }
        }

        private void TextBoxCodeSetForeground_TextChanged(object sender, TextChangedEventArgs e)
        {
            (sender as TextBox).Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#C6BDFF"));
        }

        private void PasswordBoxNewPassword2_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PlaceHolderTextBlockNewPassword2.Visibility = (sender as PasswordBox).Password.Length == 0 ? Visibility.Visible : Visibility.Hidden;
        }

        private void PasswordBoxNewPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PlaceHolderTextBlockNewPassword.Visibility = (sender as PasswordBox).Password.Length == 0 ? Visibility.Visible : Visibility.Hidden;
            (sender as PasswordBox).Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#C6BDFF"));
        }
        
        private void ButtonShowPassword_NewPassword_Click(object sender, RoutedEventArgs e)
        {
            flag_newPassword = !flag_newPassword;
            TextBoxNewPassword.Visibility = (flag_newPassword) ? Visibility.Hidden : Visibility.Visible;
            PasswordBoxNewPassword.Visibility = (flag_newPassword) ? Visibility.Visible : Visibility.Hidden;
            TextBoxNewPassword.Text = PasswordBoxNewPassword.Password;
        }

        private void TextBoxNewPassword_TextChanged(object sender, TextChangedEventArgs e)
        {
            (sender as TextBox).Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#C6BDFF"));
            PasswordBoxNewPassword.Password = (sender as TextBox).Text;
        }

        private async void ButtonEditPassword_Click(object sender, RoutedEventArgs e)
        {
            if (TextBoxNewPassword.Text == PasswordBoxNewPassword2.Password.ToString())
            {
                // Api with emailString edit password
                var client = new HttpClient();
                var data = JsonConvert.SerializeObject(new { email = emailString, password = TextBoxNewPassword.Text });
                var content = new StringContent(data, Encoding.UTF8, "application/json");
                var response = await client.GetAsync($"https://localhost:7195/api/Users/username/id/{emailString}");
                var responseString = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject(responseString);
                // Open Main Form

            }
            else
            {
                PasswordBoxNewPassword2.Foreground = Brushes.Red;
                PasswordBoxNewPassword.Foreground = Brushes.Red;
                TextBoxNewPassword.Foreground = Brushes.Red;
            }
        }
    }
}
