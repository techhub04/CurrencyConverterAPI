using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CurrencyConverterLib.Models;

namespace CurrencyConverterLib.Factory
{
    public interface ICurrencyProvider
    {
        Task<ExchangeRateResponse> GetExchangeRatesAsync(string baseCurrency);
        Task<CurrencyConversionResponse> ConvertAmountAsync(string baseCurrency, decimal amount);
        Task<HistoricalExchangeRateResponse> GetHistoricalExchangeRatesAsync
            (string baseCurrency, DateOnly startDate, DateOnly endDate);
    }
}
