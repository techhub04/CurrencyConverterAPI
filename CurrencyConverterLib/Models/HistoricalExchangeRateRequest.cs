using CurrencyConverterLib.Factory;

namespace CurrencyConverterLib.Models
{
    public class HistoricalExchangeRateRequest : ExchangeRateRequest
    {
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
    }
}
