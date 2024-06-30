using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RESTApp.Api
{
    public static class ApiInformations
    {
        public const string ApiIp = "http://127.0.0.1:8000";
        public const string epLogin = "/token";
        public const string epSignUp = "/auth/signup";
        public const string epGetUserInfo = "/users/me/";
        public const string epDownloadPicture = "/users/photo";
        public const string epSendDetails = "/social/profil";
        public const string epPost = "/social/post";
    }
}
