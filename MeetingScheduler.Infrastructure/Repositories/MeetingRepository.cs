using MeetingScheduler.Infrastructure.AppContext;
using MeetingScheduler.Infrastructure.Models;
using MeetingScheduler.Infrastructure.Models.Enums;
using MeetingScheduler.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MeetingScheduler.Infrastructure.Repositories
{
    public class MeetingRepository(MeetingSchedulerContext context) : IMeetingRepository
    {
        public MeetingSchedulerContext _context = context;

        public async Task<List<Meeting>> GetAllMeetings()
        {
            return await _context.Meetings.ToListAsync();
        }

        public async Task<Meeting> GetMeetingById(Guid meetingId)
        {
            return await _context.Meetings
                .Include(u => u.Users)
                .Include(r => r.MeetingRoom)
                .Where(u => u.Id == meetingId)
                .FirstOrDefaultAsync();
        }

        public async Task<Meeting> GetMeetingByDate(DateTime date)
        {
            return await _context.Meetings.FirstOrDefaultAsync(m => m.MeetingStartTime == date);
        }

        public async Task<List<Meeting>> GetAllMeetingsByRoomNameOrStartTime(DateOnly? meetingStartTime, string? meetingRoomName)
        {
            return await _context.Meetings
                .Where(m => m.MeetingRoom.RoomName == meetingRoomName || meetingRoomName == null)
                .Where(m => DateOnly.FromDateTime(m.MeetingStartTime) == meetingStartTime || meetingStartTime == null)
                .Include(m => m.MeetingRoom)
                .Include(u => u.Users)
                .ToListAsync();
        }


        public async Task<List<Meeting>> GetAllMeetingsByStatus(List<MeetingStatus> statuses)
        {
            return await _context.Meetings
                .Where(meeting => statuses.Contains(meeting.Status))
                .ToListAsync();
        }

        public async Task<Meeting> AddMeeting(Meeting meeting)
        {
            var result = await _context.Meetings.AddAsync(meeting);
            await _context.SaveChangesAsync();

            return result.Entity;
        }

        public async Task<Meeting> UpdateMeeting(Meeting meeting)
        {
            _context.Meetings.Update(meeting);
            await _context.SaveChangesAsync();

            return meeting;
        }

        public async Task<List<Meeting>> GetAllMeetingsForEmployeeWithPeopleManager(Guid userId, Guid peopleManagerId)
        {
            return await _context.Meetings
                .Include(u => u.Users)
                .Where(u => u.Users.Any(u => u.Id == userId))
                .Where(u => u.Users.Any(p => p.PeopleManagerId == peopleManagerId))
                .Where(m => m.Status == MeetingStatus.Scheduled || m.Status == MeetingStatus.Completed)
                .ToListAsync();
        }

        public async Task<List<Meeting>> GetAllMeetingsWithAnEmployee(string userEmail)
        {
            return await _context.Meetings
                 .Include(u => u.Users)
                 .Where(u => u.Users.Any(e => e.Email == userEmail))
                 .ToListAsync();
        }
    }
}
