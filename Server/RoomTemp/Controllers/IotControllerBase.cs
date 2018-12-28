using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using RoomTemp.Data;

namespace RoomTemp.Controllers
{
    public class IotControllerBase : ControllerBase
    {
        private readonly IMemoryCache _cache;
        private readonly TemperatureContext _temperatureContext;


        public IotControllerBase(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null) throw new ArgumentNullException(nameof(serviceProvider));

            _cache = serviceProvider.GetService<IMemoryCache>();
            _temperatureContext = serviceProvider.GetService<TemperatureContext>();
        }

        protected async Task<Device> GetAuthorizedDevice()
        {
            Guid? apiKey = ExtractApiKey();
            if (apiKey == null)
            {
                return null;
            }

            var device = await GetCachedValue($"DeviceByApiKey.{apiKey.Value}",
                async () => await _temperatureContext.Device.Where(x => x.Key == apiKey).FirstOrDefaultAsync(),
                TimeSpan.FromHours(6));

            return device;
        }

        protected async Task<T> GetCachedValue<T>(string key, Func<Task<T>> func, TimeSpan expireIn,
            Func<T, bool> shouldCacheResult = null)
        {
            if (_cache.TryGetValue(key, out T cacheEntry))
                return cacheEntry;

            var valueToCache = await func();
            if (shouldCacheResult?.Invoke(valueToCache) ?? true)
            {
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(expireIn);
                _cache.Set(key, valueToCache, cacheEntryOptions);
            }

            return valueToCache;
        }

        protected void ResetCache(string key)
        {
            _cache.Remove(key);
        }

        private Guid? ExtractApiKey()
        {
            const string iotApiKeyParameterName = "IoTApiKey";
            if (!Request.Headers.ContainsKey(iotApiKeyParameterName))
            {
                return null;
            }

            StringValues values = Request.Headers[iotApiKeyParameterName];
            if (values.Count != 1)
            {
                return null;
            }

            if (Guid.TryParse(values.First(), out var result))
            {
                return result;
            }

            return null;
        }
    }
}