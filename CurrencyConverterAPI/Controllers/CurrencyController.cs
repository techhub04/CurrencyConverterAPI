using CurrencyConverterLib.Factory;
using CurrencyConverterLib.Models;
using CurrencyConverterLib.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyConverterAPI.Controllers
{
    [ApiController]
    [Authorize(Roles = "Admin")]
    [ApiVersion("1.0")]
    public class CurrencyController : ControllerBase
    {
        private readonly ICurrencyService _currencyService;
        private readonly ICacheService _cacheService;

        public CurrencyController(ICurrencyService currencyService, ICacheService cacheService)
        {
            _currencyService = currencyService;
            _cacheService = cacheService;
        }

        [HttpGet("api/latest/{provider}/{baseCurrency}")]
        public async Task<IActionResult> GetlatestExchangeRatesAsync(CurrencyProviderType provider, string baseCurrency)
        {
            string cachKey = Enum.GetName(typeof(CurrencyProviderType), provider);
            var cachedData = await _cacheService.GetCacheAsync<ExchangeRateResponse>(cachKey);

            if (cachedData is null)
            {
                var response = await _currencyService.GetExchangeRatesAsync(new ExchangeRateRequest
                { CurrencyProvider = provider, BaseCurrency = baseCurrency });

                await _cacheService.SetCacheAsync<ExchangeRateResponse>(response, cachKey);

                return Ok(new { Version = "v1", data = response });
            }
            return Ok(new { Version = "v1", data = cachedData });


        }

        [HttpGet("api/convert/{provider}/{baseCurrency}/{amount}")]
        public async Task<IActionResult> Get(CurrencyProviderType provider, string baseCurrency, decimal amount)
        {
            try
            {
                string cacheKey = string.Concat(Enum.GetName(typeof(CurrencyProviderType), provider), baseCurrency, amount);

                var cachedData = await _cacheService.GetCacheAsync<ExchangeRateResponse>(cacheKey);

                if (cachedData is null)
                {
                    var response = await _currencyService.ConvertCurrencyAsync
                    (new CurrencyConversionRequest { CurrencyProvider = provider, BaseCurrency = baseCurrency, Amount = amount });

                    await _cacheService.SetCacheAsync<ExchangeRateResponse>(response, cacheKey);

                    return Ok(new { Version = "v1", data = response });
                }
                return Ok(new { Version = "v1", data = cachedData });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }


        [HttpGet("api/{provider}/{startDate}/{endDate}/{baseCurrency}")]
        public async Task<IActionResult> Get
            (CurrencyProviderType provider, DateOnly startDate, DateOnly endDate, string baseCurrency)
        {
            string cachKey = string.Concat(Enum.GetName(typeof(CurrencyProviderType), provider), startDate, endDate, baseCurrency);

            var cachedData = await _cacheService.GetCacheAsync<HistoricalExchangeRateResponse>(cachKey);

            if (cachedData is null)
            {
                var response = await _currencyService.GetHistoricalExchangeRatesAsync
                (new HistoricalExchangeRateRequest
                {
                    CurrencyProvider = provider,
                    StartDate = startDate,
                    EndDate = endDate,
                    BaseCurrency = baseCurrency
                });

                await _cacheService.SetCacheAsync<HistoricalExchangeRateResponse>(response, cachKey);
                return Ok(new { Version = "v1", data = response });
            }
            return Ok(new { Version = "v1", data = cachedData });

        }
    }
}
