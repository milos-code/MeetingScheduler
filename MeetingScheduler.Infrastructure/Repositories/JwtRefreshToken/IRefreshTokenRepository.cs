using MeetingScheduler.Infrastructure.Models;

public interface IRefreshTokenRepository
{
    Task<JwtRefreshToken> GetByTokenAsync(string token);
    Task CreateAsync(JwtRefreshToken refreshToken);
    Task DeleteAsync(int id);
    Task DeleteAllForUserAsync(string userId);
}