using CurrencyConverterLib.Factory;
using CurrencyConverterLib.Factory.ConcreteProviders;
using CurrencyConverterLib.Models;
using CurrencyConverterLib.Services;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace CurrencyConverterTests.UnitTests
{
    public class CurrencyServiceTests : IClassFixture<TestFixture>
    {
        private readonly ICurrencyService _service;
        public CurrencyServiceTests(TestFixture fixture)
        {
            _service = fixture.ServiceProvider.GetRequiredService<ICurrencyService>();
        }

        [Fact]
        public async Task Get_Exchange_Rates_Test()
        {
            var entity = await _service.GetExchangeRatesAsync
                (
                    new ExchangeRateRequest
                    { CurrencyProvider = CurrencyProviderType.XChangeNow, BaseCurrency = "EUR" });

            entity.Should().NotBeNull();
            entity.Base.Equals("EUR");
            entity.Rates.Should().HaveCountGreaterThan(0);
        }

        [Fact]
        public async Task Convert_Exchange_Rates_Test()
        {
            var entity = await _service.ConvertCurrencyAsync
                (
                    new CurrencyConversionRequest { CurrencyProvider = CurrencyProviderType.XChangeNow, BaseCurrency = "USD", Amount = 120.0m }
                );

            entity.Should().NotBeNull();
            entity.Base.Equals("Base");
            entity.Rates.Should().HaveCountGreaterThan(0);
            entity.Rates.First().Key.Equals("EUR");
            entity.Rates.First().Value.Equals(116.246350);
        }

        [Fact]
        public async Task Get_Historical_Exchange_Rates_Test()
        {

            var payload = new HistoricalExchangeRateRequest
            {
                CurrencyProvider = CurrencyProviderType.XChangeNow,
                StartDate = new DateOnly(2000, 01, 04),
                EndDate = new DateOnly(2001, 01, 05),
                BaseCurrency = "EUR"
            };
            var response = await _service.GetHistoricalExchangeRatesAsync
            (
                payload
            );

            response.Should().NotBeNull();
            response.BaseCurrency.Equals("EUR");
            response.Rates.Should().HaveCountGreaterThan(0);
        }


        //FrankFuter API tests
        [Fact]
        public async Task Get_Exchange_Rates_FrankFurter_Test()
        {
            var entity = await _service.GetExchangeRatesAsync
                (
                    new ExchangeRateRequest
                    { CurrencyProvider = CurrencyProviderType.FrankFuterApi, BaseCurrency = "EUR" });

            entity.Should().NotBeNull();
            entity.Base.Equals("EUR");
            entity.Rates.Should().HaveCountGreaterThan(0);
        }

        [Fact]
        public async Task Convert_Exchange_Rates_FrankFurter_Test()
        {
            var entity = await _service.ConvertCurrencyAsync
                (
                    new CurrencyConversionRequest 
                    { CurrencyProvider = CurrencyProviderType.FrankFuterApi, BaseCurrency = "USD", Amount = 120.0m }
                );

            entity.Should().NotBeNull();
            entity.Base.Equals("Base");
            entity.Rates.Should().HaveCountGreaterThan(0);
            entity.Rates.First().Key.Equals("EUR");
            entity.Rates.First().Value.Equals(116.246350);
        }

        [Fact]
        public async Task Get_Historical_Exchange_Rates_FrankFurter_Test()
        {

            var payload = new HistoricalExchangeRateRequest
            {
                CurrencyProvider = CurrencyProviderType.FrankFuterApi,
                StartDate = new DateOnly(2000, 01, 04),
                EndDate = new DateOnly(2001, 01, 05),
                BaseCurrency = "EUR"
            };
            var response = await _service.GetHistoricalExchangeRatesAsync
            (
                payload
            );

            response.Should().NotBeNull();
            response.BaseCurrency.Equals("EUR");
            response.Rates.Should().HaveCountGreaterThan(0);
        }



        //Future Provider that is waiting for implementation
        [Fact]
        public async Task ConvertAmountAsync_ShouldThrowNotImplementedException()
        {
            Func<Task> response = async () => await _service.ConvertCurrencyAsync
            (new CurrencyConversionRequest { CurrencyProvider = CurrencyProviderType.XChangeFuture }); 

            await response.Should().ThrowAsync<NotImplementedException>();
        }

        [Fact]
        public async Task GetExchangeRatesAsync_ShouldThrowNotImplementedException()
        {
            Func<Task> response = async () => await _service.GetExchangeRatesAsync
            (new ExchangeRateRequest { CurrencyProvider = CurrencyProviderType.XChangeFuture });

            await response.Should().ThrowAsync<NotImplementedException>();
        }

        [Fact]
        public async Task GetHistoricalExchangeRatesAsync_ShouldThrowNotImplementedException()
        {
            Func<Task> response = async () => await _service.GetHistoricalExchangeRatesAsync
             (new HistoricalExchangeRateRequest { CurrencyProvider = CurrencyProviderType.XChangeFuture });

            await response.Should().ThrowAsync<NotImplementedException>();
        }
    }
}
