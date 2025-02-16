using CurrencyConverterLib.Factory;
using CurrencyConverterLib.Models;

namespace CurrencyConverterLib.Services.Implementation
{
    public class CurrencyService : ICurrencyService
    {
        private readonly CurrencyProviderFactory _currencyProviderFactory;
        public CurrencyService(CurrencyProviderFactory currencyProviderFactory)
        {
            _currencyProviderFactory = currencyProviderFactory;
        }
        public async Task<ExchangeRateResponse> GetExchangeRatesAsync(ExchangeRateRequest request)
        {
            var currentProvider = _currencyProviderFactory.GetCurrencyProvider(request.CurrencyProvider);
            return await currentProvider.GetExchangeRatesAsync(request.BaseCurrency);
        }
        public async Task<CurrencyConversionResponse> ConvertCurrencyAsync(CurrencyConversionRequest request)
        {
            var currentProvider = _currencyProviderFactory.GetCurrencyProvider(request.CurrencyProvider);
            return await currentProvider.ConvertAmountAsync(request.BaseCurrency, request.Amount);            
        }

        public async Task<HistoricalExchangeRateResponse> GetHistoricalExchangeRatesAsync(HistoricalExchangeRateRequest request)
        {
            var currentProvider = _currencyProviderFactory.GetCurrencyProvider(request.CurrencyProvider);
            return await currentProvider.GetHistoricalExchangeRatesAsync
                (request.BaseCurrency, request.StartDate, request.EndDate);
        }
    }
}
