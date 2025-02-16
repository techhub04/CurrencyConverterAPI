using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CurrencyConverterLib.Factory;
using CurrencyConverterLib.Models;

namespace CurrencyConverterLib.Services
{
    public class CurrencyService : ICurrencyService
    {
        private readonly CurrencyProviderFactory _currencyProviderFactory;
        public CurrencyService(CurrencyProviderFactory currencyProviderFactory)
        {
            _currencyProviderFactory = currencyProviderFactory;
        }
        public async Task<ExchangeRateResponse> GetExchangeRates(ExchangeRateRequest request)
        {
            var currentProvider = _currencyProviderFactory.GetCurrencyProvider(request.CurrencyProvider);
            var apiResponse =  await currentProvider.GetExchangeRates();
            return null;
        }
    }
}
