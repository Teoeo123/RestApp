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
using System.Windows.Navigation;
using System.Windows.Shapes;
using RESTApp.Controls;
using RESTApp.Api.ApiClasses;

namespace RESTApp.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ApiToken tokenInfo;
        public MainWindow(ApiToken tokenInfo)
        {
            StaticApiToken.Token = null;
            InitializeComponent();
            GbCurrentModule.Content = new HomePage();
            this.tokenInfo = tokenInfo;
        }
        public void AfterPost()
        {
            GbCurrentModule.Content = new HomePage();
        }
        public void HomePageClick(object sender, RoutedEventArgs e)
        {
            GbCurrentModule.Content = new HomePage();
        }
        public void EditProfileClick(object sender, RoutedEventArgs e)
        {
            GbCurrentModule.Content = new UserDetails(tokenInfo);

        }
        public void AddPostClick(object sender, RoutedEventArgs e)
        {
            GbCurrentModule.Content = new AddPost(tokenInfo, AfterPost);
        }
        public void LogOutClick(object sender, RoutedEventArgs e)
        {
            var loginPage = new LoginWindow();
            loginPage.Show();
            Close();
        }
    }
}
