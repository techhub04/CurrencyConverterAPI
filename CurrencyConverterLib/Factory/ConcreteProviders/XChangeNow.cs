using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CurrencyConverterLib.Helpers;
using CurrencyConverterLib.Models;
using Newtonsoft.Json;

namespace CurrencyConverterLib.Factory.ConcreteProviders
{
    public class XChangeNow : ICurrencyProvider
    {
        private string excludedCurrencies = "TRY,PLN,THB,MXN";
        public XChangeNow()
        {

        }
        public async Task<ExchangeRateResponse> GetExchangeRatesAsync(string baseCurrency)
        {
            var latestExchangeRates = await FileHelper.ReadAsync("Latest_Exchange_Rates.json");
            var exchangeRates = JsonConvert.DeserializeObject<List<ExchangeRateResponse>>(latestExchangeRates);
            var exchangeRate = exchangeRates.Find(x => x.Base == baseCurrency);
            // Get today's date
            var todayDate = DateTime.Now;
            DateOnly today = new DateOnly(todayDate.Year, todayDate.Month, todayDate.Day);

            string formattedDate = today.ToString("yyyy-MM-dd");
            exchangeRate.Date = formattedDate;
            return exchangeRate;

        }
        public async Task<CurrencyConversionResponse> ConvertAmountAsync(string baseCurrency, decimal amount)
        {
            var latestExchangeRates = await FileHelper.ReadAsync("Latest_Exchange_Rates.json");
            var exchangeRates = JsonConvert.DeserializeObject<List<CurrencyConversionResponse>>(latestExchangeRates);

            var excludedCurrencyArray = excludedCurrencies.Split(',');

            var exchangeRate = exchangeRates.Find(x => x.Base == baseCurrency);

            var keys = string.Join(",", exchangeRate.Rates.Keys);

            if (keys.Contains(excludedCurrencies))
            {
                throw new Exception($"{excludedCurrencies} are not supported");
            }

            var convertedAmounts = new Dictionary<string, decimal>();

            foreach (var rate in exchangeRate.Rates)
            {
                var convertedAmount = rate.Value * amount;
                convertedAmounts.Add(rate.Key, convertedAmount);
            }
            exchangeRate.Rates = convertedAmounts;

            var todayDate = DateTime.Now;
            DateOnly today = new DateOnly(todayDate.Year, todayDate.Month, todayDate.Day);

            string formattedDate = today.ToString("yyyy-MM-dd");

            exchangeRate.Date = formattedDate;
            return exchangeRate;
        }

        public async Task<HistoricalExchangeRateResponse> GetHistoricalExchangeRatesAsync
            (string baseCurrency, DateOnly startDate, DateOnly endDate)
        {
            var rates = await FileHelper.ReadAsync("Historical_Exchange_Rates.json");
            var exchangeRates = JsonConvert.DeserializeObject<List<HistoricalExchangeRateResponse>>(rates);

            var filteredExchangeRate = exchangeRates
             .Where(x => x.BaseCurrency == baseCurrency)
             .FirstOrDefault();

            var newRates = new Dictionary<DateOnly, Dictionary<string, decimal>>();

            foreach (var rate in filteredExchangeRate.Rates)
            {
                if (rate.Key >= startDate && rate.Key <= endDate)
                {
                    newRates.Add(rate.Key, rate.Value);
                }
            }

            filteredExchangeRate.Rates = newRates;
            filteredExchangeRate.StartDate = startDate;
            filteredExchangeRate.EndDate = endDate;

            return filteredExchangeRate;
        }
    }
}

