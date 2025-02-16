using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyConverterLib.Services
{
    public interface ITokenService
    {
        Task<string> GetTokenAsync(string userId);
    }
}
