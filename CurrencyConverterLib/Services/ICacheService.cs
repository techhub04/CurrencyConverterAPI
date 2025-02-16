using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyConverterLib.Services
{
    public interface ICacheService
    {
        Task SetCacheAsync<T>(T source, string key);
        Task<T> GetCacheAsync<T>(string cacheKey);
    }
}
