using System;
using System.Configuration;
using System.Runtime.Caching;

namespace CnC.Core.Caching
{
    public class CachingProvider : ICacheManager
    {
        protected ObjectCache Cache
        {
            get
            {
                return MemoryCache.Default;
            }
        }
        public virtual T Get<T>(string key)
        {
            return (T)Cache[key];
        }
        public virtual void Remove(string key)
        {
            Cache.Remove(key);
        }
        public virtual void Set(string key, object data)
        {
            if (data == null)
                return;
            int cacheTime = Convert.ToInt32(ConfigurationManager.AppSettings["CacheTime"]);
            Cache.Add(key, data, DateTimeOffset.UtcNow.AddMinutes(cacheTime));
        }
        public virtual bool IsSet(string key)
        {
            return (Cache.Contains(key));
        }
        public virtual void Clear()
        {
            foreach (var item in Cache)
                Remove(item.Key);
        }
        public virtual void Dispose()
        {
        }

    }
}
