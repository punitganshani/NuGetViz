using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Threading.Tasks;
using System.Web;

namespace NuGetViz.Core
{
    public class InMemoryCacheStore
    { 
        private static readonly ObjectCache Cache; 
        static InMemoryCacheStore()
        {
            Cache = MemoryCache.Default;
        } 

        public T Get<T>(string region, string key)
        {
            string cacheKey = CreateUniqueKey<T>(region, key);
            if (Cache.Contains(cacheKey))
            {
                return (T)Cache[cacheKey];
            }

            return default(T);
        }

        public T Get<T>(string region, string key, Func<T> func)
        {
#if DEBUG
            return func();
#endif
            // Cache is enabled, so check in cache
            T newObject;
            string cacheKey = CreateUniqueKey<T>(region, key);

            if (Cache.Contains(cacheKey))
            {
                newObject = (T)Cache[cacheKey];
            }
            else
            {
                newObject = func();
                Add<T>(cacheKey, newObject);                
            }

            return newObject;
        }

        public void Remove<T>(string region, string key)
        {
            string cacheKey = CreateUniqueKey<T>(region, key);

            if (Cache.Contains(cacheKey))
                Cache.Remove(cacheKey);
        }

        private bool Add<T>(string uniqueKey, T value)
        {
            return Cache.Add(new CacheItem(uniqueKey, value), GetPolicy());
        }

        private static CacheItemPolicy GetPolicy()
        {
            return new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.Now.Add(TimeSpan.FromMinutes(30)) };
        }
        private static string CreateUniqueKey<T>(string region, string key)
        {
            return String.Format("[{0}|key={1}|Type={2}]", region, key, typeof(T).FullName);
        }

        internal bool IsInCache<T>(string region, string key)
        {
            string cacheKey = CreateUniqueKey<T>(region, key);
            return Cache.Contains(cacheKey);
        }
    }
}