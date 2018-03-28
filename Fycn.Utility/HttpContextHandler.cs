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
            object wiiCookies = HttpContext.Current.Request.Headers["FH-COOKIES"];
            if(wiiCookies == null){
                return string.Empty;
            }
            if (string.IsNullOrEmpty(wiiCookies.ToString())) return string.Empty;
            var cookies = JsonHandler.GetObjectFromJson<Dictionary<string, object>>(wiiCookies.ToString());
            return cookies.ContainsKey(key) ? cookies[key] : string.Empty;
        }


       
    }
}
