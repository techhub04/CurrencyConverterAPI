using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyConverterLib.Services.Implementation
{
    public class CacheService : ICacheService
    {
        private readonly IDistributedCache _cache;
        public CacheService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task SetCacheAsync<T>(T source, string cacheKey)
        {
            // Cache the data for 60 minutes
            await _cache.SetStringAsync(cacheKey, JsonConvert.SerializeObject(source), new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60)
            });
        }
        public async Task<T?> GetCacheAsync<T>(string cacheKey)
        {
            var cachedData = await _cache.GetStringAsync(cacheKey);
            if (cachedData is null)
                return default;

            var data = JsonConvert.DeserializeObject<T>(cachedData);

            return await Task.FromResult(data);
        }
    }
}
