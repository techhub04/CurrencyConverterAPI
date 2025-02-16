using CurrencyConverterLib.Factory;

namespace CurrencyConverterLib.Models
{
    public class CurrencyConversionRequest:ExchangeRateRequest
    {
        public decimal Amount { get; set; }     
    }
}
