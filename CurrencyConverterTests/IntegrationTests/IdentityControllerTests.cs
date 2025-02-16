using CurrencyConverterAPI.Controllers;
using CurrencyConverterLib.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using System.IdentityModel.Tokens.Jwt;

namespace SCMReportTests.IntegrationTests
{
    public class IdentityControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        public IdentityControllerTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetTokenAsync_With_OK_Admin_Role_Result()
        {
            var userId = "adminUser";
            var response = await _client.GetAsync($"/api/v1/Identity/GetToken/{userId}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
                       
            var tokenHandler = new JwtSecurityTokenHandler();

            var jwtToken = tokenHandler.ReadJwtToken(content);
            jwtToken.Should().NotBeNull();
            jwtToken.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == userId);
            jwtToken.Claims.Should().Contain(c => c.Type == "ClientId" && c.Value.Contains(userId));
            jwtToken.Claims.Should().Contain(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role" && c.Value == "Admin");
        }

        [Fact]
        public async Task GetTokenAsync_With_OK_Admin_Guest_Result()
        {
            var userId = "guest";
            var response = await _client.GetAsync($"/api/v1/Identity/GetToken/{userId}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            var tokenHandler = new JwtSecurityTokenHandler();

            var jwtToken = tokenHandler.ReadJwtToken(content);
            jwtToken.Should().NotBeNull();
            jwtToken.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == userId);
            jwtToken.Claims.Should().Contain(c => c.Type == "ClientId" && c.Value.Contains(userId));
            jwtToken.Claims.Should().Contain(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role" && c.Value == "Guest");
        }

        
    }
}
