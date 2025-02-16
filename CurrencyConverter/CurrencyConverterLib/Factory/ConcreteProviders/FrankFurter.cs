using System.Net.Http;
using CurrencyConverterLib.Models;

namespace CurrencyConverterLib.Factory.ConcreteProviders
{
    public class FrankFurter :  ICurrencyProvider
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public FrankFurter(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public Task<ExchangeRateResponse> GetExchangeRates()
        {
            return null;
        }
    }
}
