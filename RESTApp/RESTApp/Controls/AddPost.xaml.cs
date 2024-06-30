using Microsoft.Win32;
using Newtonsoft.Json;
using RESTApp.Api;
using RESTApp.Api.ApiClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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

namespace RESTApp.Controls
{
    /// <summary>
    /// Logika interakcji dla klasy AddPost.xaml
    /// </summary>
    public partial class AddPost : UserControl
    {
        public delegate void VoidMethodDelegate();
        string ImageUri;
        ApiClient apiClient;
        VoidMethodDelegate AfterPostMethod;
        public AddPost(ApiToken token, VoidMethodDelegate AfterPostMethod)
        {
            InitializeComponent();
            apiClient = new(token);
            this.AfterPostMethod = AfterPostMethod;
        }
        private void btnSelectPicture_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif";

            if (openFileDialog.ShowDialog() == true)
            {
                ImageUri = openFileDialog.FileName;
                imgPostPicture.Source = new BitmapImage(new Uri(ImageUri));
            }
        }
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var strContent = txtPost.Text;
            var response = apiClient.SendPost(strContent, ImageUri);
            response.Wait();
            if (!response.Result.IsSuccessStatusCode)
            {
                SimpleResponse rp = JsonConvert.DeserializeObject<SimpleResponse>(response.Result.Content.ReadAsStringAsync().Result ?? "") ?? new SimpleResponse() { detail = "" };
                ErrMess.Text = rp.detail;
                return;
            }
            AfterPostMethod();
        }

    }
}
