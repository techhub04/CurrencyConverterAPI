using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CurrencyConverterLib.Models;

namespace CurrencyConverterLib.Services
{
    public interface ICurrencyService
    {
        Task<ExchangeRateResponse> GetExchangeRatesAsync(ExchangeRateRequest request);
        Task<CurrencyConversionResponse> ConvertCurrencyAsync(CurrencyConversionRequest request);
        Task<HistoricalExchangeRateResponse> GetHistoricalExchangeRatesAsync(HistoricalExchangeRateRequest request);
    }
}
