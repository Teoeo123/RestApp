using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Xml.Serialization;
using Newtonsoft.Json; // For JSON serialization/deserialization
using RESTApp.Api.ApiClasses;
using static RESTApp.Api.ApiInformations;
using System.IO;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace RESTApp.Api
{
    public class ApiClient
    {
        private  HttpClient _client;
        public ApiToken? tokenInfo;

        public ApiClient(ApiToken? token = null)
        {
            _client = new HttpClient { BaseAddress = new Uri(ApiIp) };
            _client.Timeout = new TimeSpan(0, 0, 5);
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            tokenInfo = token;
        }

        private void ResetClient()
        {
            _client = new HttpClient { BaseAddress = new Uri(ApiIp) };
            _client.Timeout = new TimeSpan(0, 0, 5);
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<T?> GetAsync<T>(string endpoint)
        {
            ResetClient();
            var response = await _client.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(content);
        }

        public async Task<HttpResponseMessage> LogIn(string email, string passwd)
        {
            ResetClient();
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("username", email),
                new KeyValuePair<string, string>("password", passwd)
            });
            var response = await _client.PostAsync(epLogin, content);
            return response;
        }


        public async Task<HttpResponseMessage> PostItemAsync(string endpoint, string RawJson = "", KeyValuePair<String, String>[]? header = null)
        {
            ResetClient();
            if (header != null)
            {
                foreach (var i in header) _client.DefaultRequestHeaders.Add(i.Key, i.Value);
            }

            var content = new StringContent(RawJson, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(endpoint, content);
            return response; 
        }

        public async Task<HttpResponseMessage?> GetProfileInfo()
        {
            ResetClient();
            if (tokenInfo == null) return null; 
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenInfo.access_token);
            var response = await _client.GetAsync(epGetUserInfo).ConfigureAwait(false);
            return response;
        }

        public async Task<String> GetPictureById(int id, string savePath)
        {
            ResetClient();
            //_client.DefaultRequestHeaders.Add("photoId", $"{id}");
            var response = await _client.GetAsync(epDownloadPicture + $"?photoId={id}").ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                var contentDispositionHeader = response.Content.Headers.ContentDisposition;
                string filename = contentDispositionHeader!.FileName!.Trim('"');
                filename = id + filename.Substring(filename.LastIndexOf('.'));
                var content = await response.Content.ReadAsByteArrayAsync();

                await File.WriteAllBytesAsync(savePath + $"\\{filename}", content);

                return filename;
            }
            else
            {
                return "";
            }
        }
        public async Task<HttpResponseMessage?> SendUserDetails(string nickName, string bio, string ProfPicPath)
        {
            ResetClient();
            if (tokenInfo == null) return null;
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenInfo.access_token);
            var multipartFormDataContent = new MultipartFormDataContent();
            multipartFormDataContent.Add(new StringContent(nickName), "nick");
            multipartFormDataContent.Add(new StringContent(bio), "bio");
            var extension = ProfPicPath.Substring(ProfPicPath.LastIndexOf(".") + 1);
            if (!string.IsNullOrEmpty(ProfPicPath) && File.Exists(ProfPicPath))
            {
                var fileStreamContent = new StreamContent(File.OpenRead(ProfPicPath));
                fileStreamContent.Headers.ContentType = new MediaTypeHeaderValue($"image/{extension}"); // Based on your example image type
                multipartFormDataContent.Add(fileStreamContent, name: "file", fileName: Path.GetFileName(ProfPicPath));
            }
            //    here you can specify boundary if you need---^
            var uri = epSendDetails + $"/?nick={nickName}&bio={bio}";
            var response = _client.PostAsync(uri, multipartFormDataContent);
            return response.Result;
        }

        public async Task<HttpResponseMessage?> SendPost(string TxtContent, string PicPath)
        {
            ResetClient();
            if (tokenInfo == null) return null;
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenInfo.access_token);
            var multipartFormDataContent = new MultipartFormDataContent();
            multipartFormDataContent.Add(new StringContent(TxtContent), "content");
            if (!string.IsNullOrEmpty(PicPath) && File.Exists(PicPath))
            {
                var extension = PicPath.Substring(PicPath.LastIndexOf(".") + 1);
                var fileStreamContent = new StreamContent(File.OpenRead(PicPath));
                fileStreamContent.Headers.ContentType = new MediaTypeHeaderValue($"image/{extension}"); // Based on your example image type
                multipartFormDataContent.Add(fileStreamContent, name: "file", fileName: Path.GetFileName(PicPath));
            }
            //    here you can specify boundary if you need---^
            var uri = epPost + $"/?content={TxtContent}";
            var response = _client.PostAsync(uri, multipartFormDataContent);
            return response.Result;
        }

        public async Task<HttpResponseMessage> GetPosts(DateTime? Date, int count)
        {
            ResetClient();
            var uri = epPost + $"/?count={count}";
            if (Date != null) uri += $"&fromDate ={Date}";
            var response = await _client.GetAsync(uri).ConfigureAwait(false);
            return response;
        }
    }
}
