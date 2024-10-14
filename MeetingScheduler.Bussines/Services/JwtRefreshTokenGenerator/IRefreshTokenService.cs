using MeetingScheduler.Infrastructure.Models;

namespace MeetingScheduler.Bussines.Services.JwtRefreshTokenGenerator
{
    public interface IRefreshTokenService
    {
        Task<JwtRefreshToken> GetByTokenAsync(string token);
        Task CreateAsync(JwtRefreshToken refreshToken);
        Task DeleteAsync(int id);
        Task DeleteAllForUserAsync(string userId);
    }
}
