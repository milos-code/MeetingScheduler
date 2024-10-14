using MeetingScheduler.Infrastructure.AppContext;
using MeetingScheduler.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly MeetingSchedulerContext _context;

    public RefreshTokenRepository(MeetingSchedulerContext context)
    {
        _context = context;
    }

    public async Task<JwtRefreshToken> GetByTokenAsync(string token)
    {
        return await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == token);
    }

    public async Task CreateAsync(JwtRefreshToken refreshToken)
    {
        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var refreshToken = await _context.RefreshTokens.FindAsync(id);
        if (refreshToken != null)
        {
            _context.RefreshTokens.Remove(refreshToken);
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteAllForUserAsync(string userId)
    {
        var userTokens = await _context.RefreshTokens.Where(rt => rt.UserId == userId).ToListAsync();
        _context.RefreshTokens.RemoveRange(userTokens);
        await _context.SaveChangesAsync();
    }
}
