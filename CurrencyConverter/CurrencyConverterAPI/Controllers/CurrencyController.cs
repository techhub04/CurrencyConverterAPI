using CurrencyConverterLib.Factory;
using CurrencyConverterLib.Models;
using CurrencyConverterLib.Services;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyConverterAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]   
    public class CurrencyController : ControllerBase
    {
        private readonly ILogger<CurrencyController> _logger;
        private readonly ICurrencyService _currencyService;     

        public CurrencyController(ILogger<CurrencyController> logger, ICurrencyService currencyService)
        {
            _logger = logger;
            _currencyService = currencyService;
        }

        [HttpGet("GetExchangeRates/{provider}")]
        public async Task<IActionResult> GetExchangeRatesAsync(CurrencyProviderType provider)
        {
            var response = await _currencyService.GetExchangeRates(new ExchangeRateRequest { CurrencyProvider = provider });
            return Ok(response);
        }
    }
}
