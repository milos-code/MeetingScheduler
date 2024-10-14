using MeetingScheduler.Bussines.DTOs.JwtToken;
using MeetingScheduler.Bussines.DTOs.User;
using MeetingScheduler.Bussines.Services.JwtRefreshTokenGenerator;
using MeetingScheduler.Infrastructure.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using StudentRecords.Bussines.Exceptions;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace MeetingScheduler.Bussines.Services.JwtTokenGenerator
{
    public class TokenGenerator(IConfiguration configuration, IRefreshTokenService refreshTokenService) : ITokenGenerator
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly IRefreshTokenService _refreshTokenService = refreshTokenService;

        public async Task<LogInUserResponse> CreateToken(JwtCreationToken user)
        {
            var expiration = DateTime.UtcNow.AddMinutes(int.Parse(_configuration.GetSection("Jwt:Duration").Value));

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            foreach (var userRole in user.RoleNames)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole.ToString()));
            }

            var authLogInKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretForKey"]));

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                authClaims,
                expires: expiration,
                signingCredentials: new SigningCredentials(authLogInKey, SecurityAlgorithms.HmacSha256)
            );

            var tokenHandler = new JwtSecurityTokenHandler();

            var refreshToken = GenerateRefreshToken();

            // Store the refresh token
            await _refreshTokenService.CreateAsync(new JwtRefreshToken
            {
                Token = refreshToken,
                UserId = user.UserName,
                ExpiryDate = DateTime.UtcNow.AddDays(7) // Set an appropriate expiry time
            });

            return new LogInUserResponse
            {
                Token = "Bearer " + tokenHandler.WriteToken(token),
                RefreshToken = refreshToken,
                Expiration = expiration
            };
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        public async Task<LogInUserResponse> RefreshToken(string token, string refreshToken)
        {
            var principal = GetPrincipalFromExpiredToken(token);
            var username = principal.Identity.Name;

            var storedRefreshToken = await _refreshTokenService.GetByTokenAsync(refreshToken);

            if (storedRefreshToken == null || storedRefreshToken.ExpiryDate <= DateTime.UtcNow)
            {
                throw new SecurityTokenException("Invalid refresh token");
            }

            var newJwtToken = await CreateToken(new JwtCreationToken
            {
                UserName = username,
                RoleNames = principal.Claims
                    .Where(c => c.Type == ClaimTypes.Role)
                    .Select(c => c.Value)
                    .ToList()
            });

            // Delete the old refresh token
            await _refreshTokenService.DeleteAsync(storedRefreshToken.Id);

            return newJwtToken;
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretForKey"])),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                ApiExceptionHandler.ThrowApiException(HttpStatusCode.BadRequest, "Invalid token");

            return principal;
        }
    }
}
