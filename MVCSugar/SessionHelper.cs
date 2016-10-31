using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace RedWall.MVCSugar
{
    public static class SessionHelper
    {
        public static void Add(string key, object value)
        {
            HttpContext.Current.Session[key] = value;
        }

        public static T Get<T>(string key)
        {
            return Get<T>(key, null);
        }
        public static T Get<T>(string key, Func<T> getIfEmpty)
        {
            var sessionValue = HttpContext.Current.Session[key];

            if (sessionValue == null && getIfEmpty != null)
            {
                var tmpVal = getIfEmpty();
                if (tmpVal != null)
                {
                    Add(key, tmpVal);
                    return tmpVal;
                }
            }

            return (T)sessionValue;
        }
    }

}
