using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections;

namespace Fycn.Utility
{
    internal class HttpCache  : ICacheStrategy
    {
        private static readonly MemoryCache cache = new MemoryCache(new MemoryCacheOptions());

        public void AddObject(string key, object o)
        {
            if (key != null)
            {
                cache.Set(key, o, new MemoryCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromHours(1)
                });
            }
        }

        public void AddObjectWithFileChange(string key, object o, string[] files)
        {

        }

        public void AddObjectWithDepend(string key, object o, string[] dependKey)
        {

        }

        public void RemoveObject(string key)
        {
            cache.Remove(key);
        }

        public object RetrieveObject(string key)
        {
            object val = null;
            if (key != null && cache.TryGetValue(key, out val))
            {
                return val;
            }
            else
            {
                return default(object);
            }
        }
    

        public bool CheckExistValueByKey(string key)
        {
            object val = null;
            if (key != null && cache.TryGetValue(key, out val))
            {
                return true;
            }
            return false;
        }
        

        public TimeSpan TimeOutSpan { get; set; }

        public DateTime AbsoluteTime { get; set; }

        
    }
}
