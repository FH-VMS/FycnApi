using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Fycn.Utility
{
    public class HttpContextHandler
    {

        public static object GetHeaderObj(string key)
        {
            var wiiCookies = HttpContext.Current.Request.Headers["FH-COOKIES"].ToString();
            if (string.IsNullOrEmpty(wiiCookies)) return null;
            var cookies = JsonHandler.GetObjectFromJson<Dictionary<string, object>>(wiiCookies);
            return cookies.ContainsKey(key) ? cookies[key] : null;
        }


       
    }
}
