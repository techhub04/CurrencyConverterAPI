using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CurrencyConverterLib.Models
{
    public class HistoricalExchangeRateResponse
    {
        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }

        [JsonPropertyName("base")]
        [JsonProperty("base")]
        public string BaseCurrency { get; set; }

        [JsonProperty("start_date")]
        [JsonPropertyName("start_date")]
        public DateOnly StartDate { get; set; }

        [JsonProperty("end_date")]
        [JsonPropertyName("end_date")]
        public DateOnly EndDate { get; set; }

        [JsonPropertyName("rates")]
        public Dictionary<DateOnly, Dictionary<string, decimal>> Rates { get; set; }
    }
}
