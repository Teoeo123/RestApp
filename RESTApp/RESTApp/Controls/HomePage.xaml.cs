using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
using RESTApp.Models;
using RESTApp.Api.ApiClasses;
using RESTApp.Api;
using Newtonsoft.Json;
using static RESTApp.Classes.PhotoDownloader;

namespace RESTApp.Controls
{
    /// <summary>
    /// Logika interakcji dla klasy HomePage.xaml
    /// </summary>
    public partial class HomePage : UserControl
    {
        List<PostModel> PostList = new();
        DateTime? LastPostDate = null;
        string format = "yyyy-MM-ddTHH:mm:ss";
        String CachePath;
        ApiClient apiClient = new();
        public HomePage()
        {
            InitializeComponent();
            LoadPostsFromServer();
            
        }
        private void LoadPostsFromServer()
        {
            var response = apiClient.GetPosts(LastPostDate, 100);
            response.Wait();
            if (response.IsCompletedSuccessfully)
            {
                var postList = JsonConvert.DeserializeObject<ApiPost[]>(response.Result.Content.ReadAsStringAsync().Result);
                if (postList == null) return;
                foreach (var post in postList)
                {
                    string imagebuf;
                    if (post.photo_id == null) imagebuf = "";
                    else imagebuf = LoadPhoto(post.photo_id ?? -1);
                    PostList.Add(new PostModel()
                    {
                        Nickname = post.user_nick ?? "Anon",
                        PostText = post.content ?? "",
                        UserPicture = LoadPhoto(post.user_photo_id ?? -1),
                        PostImage = imagebuf
                    });
                    LastPostDate = post.date_added;
                }
                PostsListView.ItemsSource = PostList ?? new List<PostModel>();
            }
        }
        private void LoadMore(object sender, RoutedEventArgs e)
        {
            if (LastPostDate == null) return;
            LoadPostsFromServer(); 

        }
    }
}
//C:\Users\Teoeo\Desktop\ProjektowanieObiektowe\PomysłDwa\RESTApp\RESTApp\obj\Debug\net6.0-windows\Cache\ProfPics\prof1.jpg
//C:\Users\Teoeo\Desktop\ProjektowanieObiektowe\PomysłDwa\RESTApp\RESTApp\bin\Debug\net6.0-windows\Cache\ProfPics\prof1.jpg
