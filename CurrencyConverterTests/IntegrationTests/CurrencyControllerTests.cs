using CurrencyConverterLib.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;


namespace SCMReportTests.IntegrationTests
{
    public class CurrencyControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        public CurrencyControllerTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient(); 
        }

        [Fact]
        public async Task Not_Autorized_Test()
        {
            var response = await _client.GetAsync("/api/latest/1/AED");
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GetExchangeRatesAsync_OK_Test()
        {
            await SetTokenAsync();

            var response = await _client.GetAsync("/api/latest/1/AED");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            JObject jContent = JObject.Parse(content);
            var data = jContent["data"]?.ToString();

            var result = JsonConvert.DeserializeObject<ExchangeRateResponse>(data);

            result?.Base.Should().Be("AED");
            result?.Amount.Should().Be(1);
            result?.Rates.Count().Should().BeGreaterThan(0);
        }

         
        [Fact]
        public async Task ConvertAmountAsyncAsync_With_OK_Result()
        {
            await SetTokenAsync();

            var response = await _client.GetAsync("/api/convert/1/USD/120.50");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            JObject jContent = JObject.Parse(content);
            var data = jContent["data"]?.ToString();

            var result = JsonConvert.DeserializeObject<ExchangeRateResponse>(data);
            var todayDate = DateTime.Now;
            DateOnly today = new DateOnly(todayDate.Year, todayDate.Month, todayDate.Day);

            string formattedDate = today.ToString("yyyy-MM-dd");

            result?.Base.Should().Be("USD");
            result?.Amount.Should().Be(1);
            result?.Date.Should().Be(formattedDate);
            result?.Rates.Count().Should().BeGreaterThan(0);
            result?.Rates.First().Value.Should().BeGreaterThan(0);

        }

        [Fact]
        public async Task ConvertAmountAsyncAsync_With_BadResult()
        {
            await SetTokenAsync();

            var response = await _client.GetAsync("/api/convert/1/GBP/120.50");
            
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetHistoricalExchangeRatesAsync_With_OK_Result()
        {
            await SetTokenAsync();

            var response = await _client.GetAsync("/api/1/2000-01-04/2001-01-05/EUR");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            JObject jContent = JObject.Parse(content);
            var data = jContent["data"]?.ToString();

            var result = JsonConvert.DeserializeObject<HistoricalExchangeRateResponse>(data);
                     
            result?.Amount.Equals(1.0);

            result?.StartDate.Should().Be( new DateOnly(2000,01,04));
            result?.EndDate.Should().Be(new DateOnly(2001, 01, 05));

            result?.Amount.Should().Be(1);
            result?.Rates.Count().Should().BeGreaterThan(0);

        }


        /// <summary>
        /// FrankFurter API
        /// </summary>
        /// <returns></returns>
        /// 

        [Fact]
        public async Task GetExchangeRatesAsync_FrankFurter_OK_Test()
        {
            await SetTokenAsync();

            var response = await _client.GetAsync("/api/latest/3/EUR");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            JObject jContent = JObject.Parse(content);
            var data = jContent["data"]?.ToString();

            var result = JsonConvert.DeserializeObject<ExchangeRateResponse>(data);

            result?.Base.Should().Be("EUR");
            result?.Amount.Should().Be(1);
            result?.Rates.Count().Should().BeGreaterThan(0);
        }


        [Fact]
        public async Task ConvertAmountAsyncAsync_FrankFurter_With_OK_Result()
        {
            await SetTokenAsync();

            var response = await _client.GetAsync("/api/convert/3/USD/120.50");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            
            JObject jContent = JObject.Parse(content);
            var data = jContent["data"]?.ToString();

            var result = JsonConvert.DeserializeObject<ExchangeRateResponse>(data);
            var todayDate = DateTime.Now;
            DateOnly today = new DateOnly(todayDate.Year, todayDate.Month, todayDate.Day);

            string formattedDate = today.ToString("yyyy-MM-dd");

            result?.Base.Should().Be("USD");
            result?.Amount.Should().Be(1);
            result?.Date.Should().Be(formattedDate);
            result?.Rates.Count().Should().BeGreaterThan(0);
            result?.Rates.First().Value.Should().BeGreaterThan(0);

        }

        [Fact]
        public async Task GetHistoricalExchangeRatesAsync_FrankFurter_With_OK_Result()
        {
            await SetTokenAsync();

            var response = await _client.GetAsync("/api/1/2000-01-04/2001-01-05/EUR");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            JObject jContent = JObject.Parse(content);
            var data = jContent["data"]?.ToString();

            var result = JsonConvert.DeserializeObject<HistoricalExchangeRateResponse>(data);

            result?.Amount.Equals(1.0);

            result?.StartDate.Should().Be(new DateOnly(2000, 01, 04));
            result?.EndDate.Should().Be(new DateOnly(2001, 01, 05));

            result?.Amount.Should().Be(1);
            result?.Rates.Count().Should().BeGreaterThan(0);

        }

        private async Task SetTokenAsync()
        { 
            var response = await _client.GetAsync("/api/v1/Identity/GetToken/adminUser");
            response.EnsureSuccessStatusCode();

            var token = await response.Content.ReadAsStringAsync();

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);             
        }

    }
}
