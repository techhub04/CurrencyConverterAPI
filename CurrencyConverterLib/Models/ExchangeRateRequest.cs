using CurrencyConverterLib.Factory;

namespace CurrencyConverterLib.Models
{
    public class ExchangeRateRequest 
    {
        public CurrencyProviderType CurrencyProvider { get; set; }
        public string BaseCurrency { get; set; }
    }
}
