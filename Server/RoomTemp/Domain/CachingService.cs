using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace RoomTemp.Domain
{
    internal class CachingService : ICachingService
    {
        private readonly IMemoryCache _cache;

        public CachingService(IMemoryCache cache)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        public async Task<T> GetCachedValue<T>(string key, Func<Task<T>> func, Func<T, TimeSpan> expireIn,
            Func<T, bool> shouldCacheResult = null)
        {
            if (_cache.TryGetValue(key, out T cacheEntry))
                return cacheEntry;

            var valueToCache = await func();
            if (shouldCacheResult?.Invoke(valueToCache) ?? true)
            {
                var expireInValue = expireIn.Invoke(valueToCache);
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(expireInValue);
                _cache.Set(key, valueToCache, cacheEntryOptions);
            }

            return valueToCache;
        }

        public T GetCachedValue<T>(string key, Func<T> func, Func<T, TimeSpan> expireIn, Func<T, bool> shouldCacheResult = null)
        {
            if (_cache.TryGetValue(key, out T cacheEntry))
                return cacheEntry;

            var valueToCache = func();
            if (shouldCacheResult?.Invoke(valueToCache) ?? true)
            {
                var expireInValue = expireIn.Invoke(valueToCache);
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(expireInValue);
                _cache.Set(key, valueToCache, cacheEntryOptions);
            }

            return valueToCache;
        }

        public T GetCachedValue<T>(string key, Func<T> func, TimeSpan expireIn, Func<T, bool> shouldCacheResult = null)
        {
            if (_cache.TryGetValue(key, out T cacheEntry))
                return cacheEntry;

            var valueToCache = func();
            if (shouldCacheResult?.Invoke(valueToCache) ?? true)
            {
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(expireIn);
                _cache.Set(key, valueToCache, cacheEntryOptions);
            }

            return valueToCache;
        }
        
        public void ResetCache(string key)
        {
            _cache.Remove(key);
        }
    }
}