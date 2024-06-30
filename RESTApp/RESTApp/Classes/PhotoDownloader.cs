using RESTApp.Api;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RESTApp.Classes
{
    public static class PhotoDownloader
    {
        private static string CacheSavePath = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\Cache";
        private static ApiClient apiClient = new ApiClient();
        public static string PhotoFromCache(int photoId)
        {
            if (!Directory.Exists(CacheSavePath))
            {
                throw new DirectoryNotFoundException($"Directory '{CacheSavePath}' not found.");
            }

            var matchingFiles = Directory.GetFiles(CacheSavePath, $"{photoId}.*", SearchOption.TopDirectoryOnly);

            if (matchingFiles.Length > 0)
            {
                return matchingFiles.First();
            }
            else
            {
                return string.Empty;
            }
        }
        public static string LoadPhoto(int photoId)
        {
            if (photoId == -1)
            {
                return CacheSavePath + "\\emptyUser.jpg";
            }
            var CachePhoto = PhotoFromCache(photoId);
            if (CachePhoto == string.Empty)
            {
                return CacheSavePath + '\\'+ apiClient.GetPictureById(photoId, CacheSavePath).Result;
            }
            else return CachePhoto;
        }
    }
}
