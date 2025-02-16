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
    public class FrankFurter : ICurrencyProvider
    {
        private string excludedCurrencies = "TRY,PLN,THB,MXN";
        private readonly HttpClient _httpClient;
        public FrankFurter(IHttpClientFactory httpClientFactory=null)
        {
            _httpClient = httpClientFactory.CreateClient();
        }
        public async Task<ExchangeRateResponse> GetExchangeRatesAsync(string baseCurrency)
        {
            var response = await _httpClient.GetAsync($"https://api.frankfurter.app/latest?base={baseCurrency}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var exchangeRate = JsonConvert.DeserializeObject<ExchangeRateResponse>(content);
                
                // Get today's date
                var todayDate = DateTime.Now;
                DateOnly today = new DateOnly(todayDate.Year, todayDate.Month, todayDate.Day);

                string formattedDate = today.ToString("yyyy-MM-dd");
                exchangeRate.Date = formattedDate;
                return exchangeRate;              
            }
           
            throw new Exception("Failed to fetch data from frankfurter api");

        }
        public async Task<CurrencyConversionResponse> ConvertAmountAsync(string baseCurrency, decimal amount)
        {
            var response = await _httpClient.GetAsync($"https://api.frankfurter.app/latest?base={baseCurrency}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                var exchangeRate = JsonConvert.DeserializeObject<CurrencyConversionResponse>(content);

                var excludedCurrencyArray = excludedCurrencies.Split(',');
                    
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
            throw new Exception("Failed to fetch data from frankfurter api");
        }

        public async Task<HistoricalExchangeRateResponse> GetHistoricalExchangeRatesAsync
            (string baseCurrency, DateOnly startDate, DateOnly endDate)
        {
            //https://api.frankfurter.dev/v1/2000-01-01..2000-12-31 
            var endPointAddress = $"https://api.frankfurter.dev/v1/{startDate.ToString("yyyy-MM-dd")}..{endDate.ToString("yyyy-MM-dd")}";
            var response = await _httpClient.GetAsync(endPointAddress);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var exchangeRates = JsonConvert.DeserializeObject<HistoricalExchangeRateResponse>(content);

                
                var newRates = new Dictionary<DateOnly, Dictionary<string, decimal>>();

                foreach (var rate in exchangeRates.Rates)
                {
                    if (rate.Key >= startDate && rate.Key <= endDate)
                    {
                        newRates.Add(rate.Key, rate.Value);
                    }
                }

                exchangeRates.Rates = newRates;
                exchangeRates.StartDate = startDate;
                exchangeRates.EndDate = endDate;

                return exchangeRates;
            }
            throw new Exception("Failed to fetch data from frankfurter api");
        }
    }
}

