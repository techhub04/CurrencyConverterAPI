using CurrencyConverterLib.Factory;
using CurrencyConverterLib.Models;
using CurrencyConverterLib.Services;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System.Data;
using System.IdentityModel.Tokens.Jwt;

namespace CurrencyConverterTests.UnitTests
{
    public class TokenServiceTests : IClassFixture<TestFixture>
    {
        private readonly ITokenService _service;
        public TokenServiceTests(TestFixture fixture)
        {
            _service = fixture.ServiceProvider.GetRequiredService<ITokenService>();
        }

        [Theory]
        [InlineData("adminUser")]
        public async Task Get_Token_Successfull_Test(string userId)
        {
            var token = await _service.GetTokenAsync(userId);
            var tokenHandler = new JwtSecurityTokenHandler();

            var jwtToken = tokenHandler.ReadJwtToken(token);
            jwtToken.Should().NotBeNull();
            jwtToken.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == userId);
            jwtToken.Claims.Should().Contain(c => c.Type == "ClientId" && c.Value.Contains(userId));
            jwtToken.Claims.Should().Contain(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role" && c.Value == "Admin");
            
        }
        [Theory]
        [InlineData("adminUser")] 
        public async Task JwtToken_Should_Expire_After_Specified_Time(string userId)
        {           
            var token = await _service.GetTokenAsync(userId);
            var tokenHandler = new JwtSecurityTokenHandler();

            var jwtToken = tokenHandler.ReadJwtToken(token);
                        
            jwtToken.ValidTo.Should().BeCloseTo(DateTime.UtcNow.AddMinutes(60), precision: TimeSpan.FromSeconds(5));
        }
    }
}
