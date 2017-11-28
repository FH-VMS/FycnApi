using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fycn.Utility
{
    public static class CacheHandler<T>
    {
        private static ICacheStrategy instance;
        private static ICacheStrategy cache
        {
            get
            {
                if (instance == null)
                {
                    instance = ReflectionHandler.GetClass<ICacheStrategy>(ConfigurationManager.AppSettings["ProjectName"]+".Utility." + ConfigHandler.CacheStrategy);
                }
                return instance;
            }
        }

        #region With two params for Function
        public static T GetObject<TM, TN>(Func<TM, TN, T> f, TM m, TN n)
        {
            var method = f.Method;
            return GetObject(method.ReflectedType + method.Name + m + n, f, m, n);
        }
        public static T GetObject<TM, TN>(Func<TM, TN, T> f, TM m, TN n, int expireTime, DateTime absoluteTimeOut)
        {
            var method = f.Method;
            return GetObject(method.ReflectedType + method.Name + m, expireTime, absoluteTimeOut, f, m, n);
        }
        public static T GetObject<TM, TN>(string key, Func<TM, TN, T> f, TM m, TN n)
        {
            return GetObject(key, -1, DateTime.MaxValue, f, m, n);
        }

        public static T GetObject<TM, TN>(string key, int expireTime, DateTime absoluteTimeOut, Func<TM, TN, T> f, TM m, TN n)
        {
            if (cache.CheckExistValueByKey(key))
            {
                return (T)cache.RetrieveObject(key);
            }
            var temp = f(m, n);
            SetObject(key, expireTime == -1 ? TimeSpan.MaxValue : new TimeSpan(expireTime), absoluteTimeOut, temp);
            return temp;
        }
        #endregion

        #region With only one params for Function.
        public static T GetObject<TM>(Func<TM, T> f, TM m)
        {
            var method = f.Method;
            return GetObject(method.ReflectedType + method.Name + m, f, m);
        }
        public static T GetObject<TM>(Func<TM, T> f, TM m, int expireTime, DateTime absoluteTimeOut)
        {
            var method = f.Method;
            return GetObject(method.ReflectedType + method.Name + m, expireTime, absoluteTimeOut, f, m);
        }

        public static T GetObject<TM>(string key, Func<TM, T> f, TM m)
        {
            return GetObject(key, -1, DateTime.MaxValue, f, m);
        }

        public static T GetObject<TM>(string key, int expireTime, DateTime absoluteTimeOut, Func<TM, T> f, TM m)
        {
            if (cache.CheckExistValueByKey(key))
            {
                return (T)cache.RetrieveObject(key);
            }
            var temp = f(m);
            SetObject(key, expireTime == -1 ? TimeSpan.MaxValue : new TimeSpan(expireTime), absoluteTimeOut, temp);
            return temp;
        }
        #endregion

        #region With no params for Function.
        public static T GetObject(Func<T> f)
        {
            var m = f.Method;
            return GetObject(m.ReflectedType + m.Name, f);
        }
        public static T GetObject(Func<T> f, int expireTime, DateTime absoluteTimeOut)
        {
            var m = f.Method;
            return GetObject(m.ReflectedType + m.Name, expireTime, absoluteTimeOut, f);
        }
        public static T GetObject(string key, Func<T> f)
        {
            var m = f.Method;
            return GetObject(key + m.Name, 0, DateTime.MaxValue, f);
        }

        public static T GetObject(string key, int expireTime, DateTime absoluteTimeOut, Func<T> f)
        {
            if (cache.CheckExistValueByKey(key))
            {
                return (T)cache.RetrieveObject(key);
            }
            var temp = f();
            SetObject(key, expireTime == -1 ? TimeSpan.MaxValue : new TimeSpan(expireTime), absoluteTimeOut, temp);
            return temp;
        }
        #endregion

        /// <summary>
        /// Get Object by the key of cache
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T GetObject(string key)
        {
            return (T)cache.RetrieveObject(key);
        }

        /// <summary>
        /// Set Object with key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="t"></param>
        public static void SetObject(string key, T t)
        {
            SetObject(key, TimeSpan.MaxValue, DateTime.MaxValue, t);
        }

        /// <summary>
        /// Set Object with the expire time.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="expireTime"></param>
        /// <param name="absoluteTimeOut"></param>
        /// <param name="t"></param>
        public static void SetObject(string key, TimeSpan expireTime, DateTime absoluteTimeOut, T t)
        {
            if (Equals(t, default(T)))
            {
                return;
            }
            if (expireTime == TimeSpan.MaxValue)
            {
                expireTime = ConfigHandler.TimeOutSpan;
            }
            if (absoluteTimeOut == DateTime.MaxValue)
            {
                absoluteTimeOut = ConfigHandler.AbsoluteTime;
            }
            cache.TimeOutSpan = expireTime;
            cache.AbsoluteTime = absoluteTimeOut;
            cache.AddObject(key, t);
        }

        #region Clear cache
        /*
        public static void ClearAllCache()
        {
            var cacheEnum = cache.GetKeyValue();
            while (cacheEnum.MoveNext())
            {
                cache.RemoveObject(cacheEnum.Key.ToString());

            }
        }
        */
        /// <summary>
        /// Clear cache by the key
        /// </summary>
        /// <param name="key"></param>
        public static void ClearCache(string key)
        {
            cache.RemoveObject(key);
        }
        #endregion
    }
}
