using Microsoft.AspNetCore.Mvc;
using LotteryPredictionApi.DTOs;
using LotteryPredictionApi.Services;

namespace LotteryPredictionApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService) => _authService = authService;

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var result = await _authService.LoginAsync(loginDto);
            if (result == null) return Unauthorized(new { message = "Invalid email or password" });
            return Ok(result);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(CreateUserDto createUserDto)
        {
            try
            {
                var user = await _authService.RegisterAsync(createUserDto);
                return CreatedAtAction(null, user);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(RefreshTokenDto refreshTokenDto)
        {
            var resp = await _authService.RefreshTokenAsync(refreshTokenDto.RefreshToken);
            if (resp == null) return Unauthorized(new { message = "Invalid refresh token" });
            return Ok(resp);
        }
    }
}
