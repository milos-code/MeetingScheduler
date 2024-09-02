using MeetingScheduler.Bussines.DTOs.JwtToken;
using MeetingScheduler.Bussines.DTOs.User;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MeetingScheduler.Bussines.Services.JwtTokenGenerator
{
    public class TokenGenerator(IConfiguration configuration) : ITokenGenerator
    {
        private readonly IConfiguration _configuration = configuration;

        public async Task<LogInUserResponse> CreateToken(JwtCreationToken user)
        {
            var expiration = DateTime.UtcNow.AddMinutes(int.Parse(_configuration.GetSection("Jwt:Duration").Value));

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, new Guid().ToString())
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

            return new LogInUserResponse
            {
                Token = "Bearer " + tokenHandler.WriteToken(token),
                Expiration = expiration
            };
        }
    }
}
