using MeetingScheduler.Infrastructure.AppContext;
using MeetingScheduler.Infrastructure.Models;
using MeetingScheduler.Infrastructure.Models.Enums;
using MeetingScheduler.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MeetingScheduler.Infrastructure.Repositories
{
    public class MeetingUserRepository(MeetingSchedulerContext context,
        IMeetingRepository meetingRepository) : IMeetingUserRepository
    {
        private readonly MeetingSchedulerContext _dbContext = context;
        private readonly IMeetingRepository _meetingRepository = meetingRepository;

        public async Task<UserMeetingStatus> GetMeetingUserStatus(Guid userId, Guid meetingId)
        {
            return await _dbContext.UserMeetingStatuses.FirstOrDefaultAsync(n => n.UserId == userId  && n.MeetingId == meetingId);
        }

        public async Task UpdateMeetingStatus(UserMeetingStatus userMeetingStatus)
        {
            _dbContext.Update(userMeetingStatus);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<UserMeetingStatus>> GetAllUsersForMeeting(Guid meetingId)
        {
            return await _dbContext.UserMeetingStatuses
                .Include(u => u.User)
                .Where(m => m.MeetingId == meetingId)
                .ToListAsync();
        }
    }
}
