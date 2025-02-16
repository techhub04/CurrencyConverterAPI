using CurrencyConverterLib.Services;
using CurrencyConverterLib.Services.Implementation;
using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;

namespace CurrencyConverterTests.UnitTests
{
    public class CacheServiceTests:IClassFixture<TestFixture>
    {     
        private readonly ICacheService _service;
        private readonly Mock<IDistributedCache> _mockCache;
        public CacheServiceTests(TestFixture fixture)
        {
            _mockCache = new Mock<IDistributedCache>();
            _service = fixture.ServiceProvider.GetRequiredService<ICacheService>();
        }

        [Fact]
        public async Task Get_Set_CacheAsync_ShouldGetSetCacheSuccessfully()
        {
            var cacheKey = "mockTestKey";
            var mockTestData = "Mock test cache data";

            await _service.SetCacheAsync(mockTestData, cacheKey);

            var cacheData = await _service.GetCacheAsync<string>(cacheKey);

            cacheData.Should().NotBeNull();
            cacheData.Equals("Mock test cache data");
        }

        [Fact]
        public async Task GetCacheAsync_ShouldReturnNull_WhenCacheIsNotPresent()
        {
            var cacheKey = "mockTestKey";
            var mockTestData = "Mock test cache data";
            var nonExistsTestKey = "nonExistsTestKey";

            await _service.SetCacheAsync(mockTestData, cacheKey);

            var cacheData = await _service.GetCacheAsync<string>(nonExistsTestKey);

            cacheData.Should().BeNull();
            
        }
    }
}
