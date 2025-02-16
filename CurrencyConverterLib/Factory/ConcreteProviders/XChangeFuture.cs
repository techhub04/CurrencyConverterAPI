using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CurrencyConverterLib.Models;

namespace CurrencyConverterLib.Factory.ConcreteProviders
{
    public class XChangeFuture : ICurrencyProvider
    {
        public async Task<CurrencyConversionResponse> ConvertAmountAsync(string baseCurrency, decimal amount)
        {
            throw new NotImplementedException();
        }

        public async Task<ExchangeRateResponse> GetExchangeRatesAsync(string baseCurrency)
        {
            throw new NotImplementedException();
        }

        public async Task<HistoricalExchangeRateResponse> GetHistoricalExchangeRatesAsync(string baseCurrency, DateOnly startDate, DateOnly endDate)
        {
            throw new NotImplementedException();
        }
    }
}

