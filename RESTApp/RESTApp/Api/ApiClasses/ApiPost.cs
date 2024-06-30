using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RESTApp.Api.ApiClasses
{
    class ApiPost
    {
        public int id;
        public string? user_nick;
        public int? user_photo_id;
        public int? photo_id;
        public string? content;
        public DateTime date_added;
    }
}
