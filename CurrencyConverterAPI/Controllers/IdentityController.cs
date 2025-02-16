using CurrencyConverterLib.Factory;
using CurrencyConverterLib.Models;
using CurrencyConverterLib.Services;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyConverterAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class IdentityController : ControllerBase
    {       
        private readonly ITokenService _tokenService;

        public IdentityController(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        [HttpGet("GetToken/{userId}")]
        public async Task<IActionResult> GetTokenAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequest("User not valid");
            }
            var response = await _tokenService.GetTokenAsync(userId);
            return Ok(response);
        }
    }
}
