using CurrencyConverterLib.Factory.ConcreteProviders;
using Microsoft.Extensions.DependencyInjection;

namespace CurrencyConverterLib.Factory
{
    public class CurrencyProviderFactory
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public CurrencyProviderFactory(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public ICurrencyProvider GetCurrencyProvider(CurrencyProviderType currencyProviderType)
        {
            switch (currencyProviderType)
            {
                case CurrencyProviderType.FrankFurter:
                    return  new FrankFurter(_httpClientFactory);
               // case CurrencyProviderType.Nasdaq:
                    //return _serviceProvider.GetRequiredService<Nasdaq>();
               // case CurrencyProviderType.UAEExchange:
                    //return _serviceProvider.GetRequiredService<UAEExchange>();
            }

            return null;
        }
    }
}
