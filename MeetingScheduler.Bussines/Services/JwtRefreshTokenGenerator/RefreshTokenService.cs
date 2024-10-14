using MeetingScheduler.Infrastructure.Models;

namespace MeetingScheduler.Bussines.Services.JwtRefreshTokenGenerator
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public RefreshTokenService(IRefreshTokenRepository refreshTokenRepository)
        {
            _refreshTokenRepository = refreshTokenRepository;
        }

        public async Task<JwtRefreshToken> GetByTokenAsync(string token)
        {
            return await _refreshTokenRepository.GetByTokenAsync(token);
        }

        public async Task CreateAsync(JwtRefreshToken refreshToken)
        {
            await _refreshTokenRepository.CreateAsync(refreshToken);
        }

        public async Task DeleteAsync(int id)
        {
            await _refreshTokenRepository.DeleteAsync(id);
        }

        public async Task DeleteAllForUserAsync(string userId)
        {
            await _refreshTokenRepository.DeleteAllForUserAsync(userId);
        }
    }
}
