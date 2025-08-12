using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using LotteryPredictionApi.Data;
using LotteryPredictionApi.DTOs;
using LotteryPredictionApi.Models;

namespace LotteryPredictionApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly LotteryDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(LotteryDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginDto loginDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);
            if (user == null) return null;

            var verified = BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password);
            if (!verified) return null;

            var userDto = new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                NumberOfPredictions = user.NumberOfPredictions,
                TotalPoints = user.TotalPoints
            };

            var token = GenerateJwtToken(userDto);
            var refreshToken = GenerateRefreshToken();

            // NOTE: For production, persist refresh tokens in DB with expiry, rotation etc.
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return new LoginResponseDto
            {
                Token = token,
                User = userDto,
                ExpiresAt = DateTime.UtcNow.AddHours(24)
            };
        }

        public async Task<UserDto> RegisterAsync(CreateUserDto createUserDto)
        {
            if (await _context.Users.AnyAsync(u => u.Email == createUserDto.Email))
                throw new InvalidOperationException("Email already exists");

            var user = new User
            {
                FirstName = createUserDto.FirstName,
                LastName = createUserDto.LastName,
                Email = createUserDto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(createUserDto.Password),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Also create a PredictionStats row so one-to-one relation exists
            var stats = new PredictionStats
            {
                UserId = user.Id,
                TotalPredictions = 0,
                CorrectOutcomes = 0,
                CorrectScores = 0,
                TotalPoints = 0,
                UpdatedAt = DateTime.UtcNow
            };

            _context.PredictionStats.Add(stats);
            await _context.SaveChangesAsync();

            return new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                NumberOfPredictions = user.NumberOfPredictions,
                TotalPoints = user.TotalPoints
            };
        }

        public async Task<LoginResponseDto?> RefreshTokenAsync(string refreshToken)
        {
            // Placeholder: implement DB-backed refresh token validation if needed
            await Task.CompletedTask;
            return null;
        }

        public string GenerateJwtToken(UserDto user)
        {
            var secret = _configuration["Jwt:SecretKey"];
            if (string.IsNullOrWhiteSpace(secret))
                throw new InvalidOperationException("JWT SecretKey not configured (Jwt:SecretKey)");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.GivenName, user.FirstName),
                new Claim(ClaimTypes.Surname, user.LastName)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(24),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}
