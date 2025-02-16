using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyConverterLib.Models
{    
    public class ExchangeRateResponse
    {
        public string Base { get; set; }
        public decimal Amount { get; set; }
        public string Date { get; set; }

        public Dictionary<string,decimal> Rates { get; set; } = new Dictionary<string, decimal>();
    }
}
