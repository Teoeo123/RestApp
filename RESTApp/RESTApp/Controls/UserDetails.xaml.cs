using Microsoft.Win32;
using RESTApp.Api.ApiClasses;
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
using static System.Collections.Specialized.BitVector32;
using RESTApp.Api;
using Newtonsoft.Json;
using RESTApp.Classes;

namespace RESTApp.Controls
{
    /// <summary>
    /// Logika interakcji dla klasy UserDetails.xaml
    /// </summary>
    public partial class UserDetails : UserControl
    {
        ApiToken token;
        ApiClient apiClient;
        ApiUserDetails userDetails;
        BitmapImage bitmapImage;
        string newImageUri = "";
        public UserDetails(ApiToken token)
        {
            this.token = token;
            apiClient = new(token);            
            InitializeComponent();
            LoadUserDetails();
        }

        private void LoadUserDetails()
        {
            var r = apiClient.GetProfileInfo().Result;
            if (r != null)
            {
                userDetails = JsonConvert.DeserializeObject<ApiUserDetails>(r.Content.ReadAsStringAsync().Result) ?? new ApiUserDetails();
                txtNickname.Text = userDetails.Nickname ?? "";
                txtBio.Text = userDetails.Bio;
                var uriStr = PhotoDownloader.LoadPhoto(userDetails.ProfpicID ?? -1);
                bitmapImage = new BitmapImage(new Uri(uriStr));
                imgProfilePicture.Source = bitmapImage;
            }
            else userDetails = null;
        }
        private void btnSelectPicture_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif";

            if (openFileDialog.ShowDialog() == true)
            {
                newImageUri = openFileDialog.FileName;
                imgProfilePicture.Source = new BitmapImage(new Uri(newImageUri));
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            var newDet = new ApiUserDetails() {
                Nickname = "",
                Bio = "",
                ProfpicID = null
            };
            newDet.Nickname = txtNickname.Text;
            newDet.Bio = txtBio.Text;
            var response = apiClient.SendUserDetails(newDet.Nickname, newDet.Bio, newImageUri);
            if (!response.IsCompletedSuccessfully)
            {
                throw new Exception("something went wrong");
            }
            newImageUri = "";
        }
    }
}
