using CurrencyConverterLib.Factory.ConcreteProviders;

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
                case CurrencyProviderType.XChangeNow:
                    return new XChangeNow();
                case CurrencyProviderType.XChangeFuture:
                    return new XChangeFuture();
                case CurrencyProviderType.FrankFuterApi:
                    return new FrankFurter(_httpClientFactory);
            }

            throw new Exception("Currency ProviderType is not available");
        }
    }
}
