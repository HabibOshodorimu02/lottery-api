using LotteryPredictionApi.DTOs;
using System.Threading.Tasks;

namespace LotteryPredictionApi.Services
{
    public interface IAuthService
    {
        Task<LoginResponseDto?> LoginAsync(LoginDto loginDto);
        Task<UserDto> RegisterAsync(CreateUserDto createUserDto);
        Task<LoginResponseDto?> RefreshTokenAsync(string refreshToken);
        string GenerateJwtToken(UserDto user);
        string GenerateRefreshToken();
    }
}
