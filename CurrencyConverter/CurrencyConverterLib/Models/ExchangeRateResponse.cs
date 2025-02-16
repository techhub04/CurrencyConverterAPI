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
        public double Amount { get; set; }
        public DateOnly Date { get; set; }

        public IEnumerable<ExchangeRate> Rates { get; set; } = new List<ExchangeRate>();
    }

    public class ExchangeRate
    {
        public string Currency { get; set; }
        public double Rate { get; set; }
    }
}
