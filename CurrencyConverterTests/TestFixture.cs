using System;
using Castle.Core.Configuration;
using CurrencyConverterLib.Factory;
using CurrencyConverterLib.Services;
using CurrencyConverterLib.Services.Implementation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CurrencyConverterTests
{
    public class TestFixture : IDisposable
    {
        public IServiceProvider ServiceProvider { get; private set; }
        private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;

        public TestFixture()
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())  // Set base path
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true) // Load JSON config
            .AddEnvironmentVariables(); // Load environment variables if needed

            _configuration = builder.Build();

            var serviceCollection = new ServiceCollection();

            serviceCollection.AddSingleton<Microsoft.Extensions.Configuration.IConfiguration>(_configuration);

            // Add services to the container.
            serviceCollection.AddHttpClient();
            serviceCollection.AddDistributedMemoryCache();
            serviceCollection.AddScoped<CurrencyProviderFactory>();

            serviceCollection.AddScoped<ICurrencyService, CurrencyService>();
            serviceCollection.AddScoped<ITokenService, TokenService>();
            serviceCollection.AddScoped<ICacheService, CacheService>();

            // Build the service provider
            ServiceProvider = serviceCollection.BuildServiceProvider();
        }

        

        public void Dispose()
        {
            // Clean up any resources after each test runs
        }
    }
}
