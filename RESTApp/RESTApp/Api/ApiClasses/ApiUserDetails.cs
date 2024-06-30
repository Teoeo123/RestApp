using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RESTApp.Api.ApiClasses
{
    public class ApiUserDetails
    {
        [JsonPropertyName("nickName")]
        public string? Nickname { get; set; }

        [JsonPropertyName("bio")]
        public string? Bio { get; set; }

        [JsonPropertyName("profpicID")]
        public int? ProfpicID { get; set; }
    }
}
