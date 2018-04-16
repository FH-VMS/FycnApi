using System;
using System.Collections.Generic;
using System.Text;

namespace Fycn.Utility
{
    public class WebCacheHelper
    {
        private static RedisHelper _redisHelper2;

        private static RedisHelper redisHelper2
        {
            get
            {
                if (_redisHelper2 == null)
                {
                    _redisHelper2 = new RedisHelper(2);
                }
                return _redisHelper2;
            }
            set
            {
                _redisHelper2 = value;
            }
        }

        //缓存parent and child ids  无过期时间
        public static void CacheParentAndChildIds(string clientId, string val)
        {
            redisHelper2.StringSet(clientId + "-" + "cp", val);
        }
        //取parent and child ids
        public static string GetParentAndChildIds(string clientId)
        {
            return redisHelper2.StringGet(clientId + "-" + "cp");
        }

        //缓存child ids  无过期时间
        public static void CacheChildIds(string clientId, string val)
        {
            redisHelper2.StringSet(clientId + "-" + "c", val);
        }

        //取child ids
        public static string GetChildIds(string clientId)
        {
            return redisHelper2.StringGet(clientId + "-" + "c");
        }

        //清除ids缓存
        public static void ClearIds(string clientId)
        {
            redisHelper2.KeyDelete(clientId + "-" + "cp");
            redisHelper2.KeyDelete(clientId + "-" + "c");
        }
    }
}
