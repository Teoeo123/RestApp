using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RESTApp.Api.ApiClasses
{
    public static class StaticApiToken
    {
        public static string? Token;
        public static string? Type;
        public static DateTime? GetTime;
    }

    public class ApiToken
    {
        public string? access_token;
        public string? token_type;
        public DateTime? GetTime;
        public void SaveAsSingleton()
        {
            StaticApiToken.Token = access_token;
            StaticApiToken.Type = token_type;
            StaticApiToken.GetTime = DateTime.Now;
        }
    }
}
