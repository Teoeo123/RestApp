using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Newtonsoft.Json;
using System.Net.Http;
using RESTApp.Api;
using RESTApp.Api.ApiClasses;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;

namespace RESTApp.Views
{
    /// <summary>
    /// Logika interakcji dla klasy LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        ApiClient client = new ();

        public LoginWindow()
        {
            InitializeComponent();
        }

        public async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var login = UsernameTextBox.Text;
            var pass = PasswordBox.Password;
            var response = await client.LogIn(login, pass);
            if (response.IsSuccessStatusCode)
            {
                var token = JsonConvert.DeserializeObject<ApiToken>(response.Content.ReadAsStringAsync().Result ?? "");
                if(token == null)
                {
                    MessageBlock.Text = "Something went wrong!";
                    MessageBlock.Foreground = new SolidColorBrush(Colors.Red);
                    return;
                }
                token.GetTime = DateTime.Now;
                token.SaveAsSingleton();
                var MainWindow = new MainWindow(token);
                MainWindow.Show();
                Close();
            }
            else
            {
                var rp = JsonConvert.DeserializeObject<SimpleResponse>(response.Content.ReadAsStringAsync().Result ?? "") ?? new SimpleResponse() { detail = "" };
                MessageBlock.Foreground = new SolidColorBrush(Colors.Red);
                MessageBlock.Text = rp.detail;
            }
        }

        public async void SingInButton_Click(object sender, RoutedEventArgs e)
        {
            var login = UsernameTextBox.Text;
            var pass = PasswordBox.Password;
            var user = new ApiUser()
            {
                email = login,
                passwd = pass
            };
            var userJson = JsonConvert.SerializeObject(user);
            var response = await client.PostItemAsync(ApiInformations.epSignUp,userJson);
            SimpleResponse rp = JsonConvert.DeserializeObject<SimpleResponse>(response.Content.ReadAsStringAsync().Result ?? "") ?? new SimpleResponse() { detail = "" };
            if(response.IsSuccessStatusCode)
                MessageBlock.Foreground = new SolidColorBrush(Colors.Green);
            else
                MessageBlock.Foreground = new SolidColorBrush(Colors.Red);
            MessageBlock.Text = rp.detail;
        }
    }
}

