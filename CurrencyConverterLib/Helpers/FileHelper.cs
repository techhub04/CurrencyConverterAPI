using System.Reflection;

namespace CurrencyConverterLib.Helpers
{
    public  class FileHelper
    {
        public async static Task<string> ReadAsync(string fileName)
        {
            string executingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location); 
            var filePath = Path.Combine(executingDirectory, "ExchangeRates\\",fileName);
            return await File.ReadAllTextAsync(filePath);
        }
    }
}
