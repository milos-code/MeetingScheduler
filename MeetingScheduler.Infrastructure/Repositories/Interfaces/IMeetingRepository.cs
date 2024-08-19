using MeetingScheduler.Infrastructure.Models;
using MeetingScheduler.Infrastructure.Models.Enums;

namespace MeetingScheduler.Infrastructure.Repositories.Interfaces
{
    public interface IMeetingRepository
    {
        Task<List<Meeting>> GetAllMeetings();
        Task<Meeting> GetMeetingById(Guid meetingId);
        Task<Meeting> GetMeetingByDate(DateTime date);
        Task<List<Meeting>> GetAllMeetingsByRoomNameOrStartTime(DateOnly? meetingStartTime, string? meetingRoomName);
        Task<List<Meeting>> GetAllMeetingsByStatus(List<MeetingStatus> statuses);
        Task<Meeting> AddMeeting(Meeting meeting);
        Task<Meeting> UpdateMeeting(Meeting meeting);
        Task<List<Meeting>> GetAllMeetingsForEmployeeWithPeopleManager(Guid userId, Guid peopleManagerId);
        Task<List<Meeting>> GetAllMeetingsWithAnEmployee(string userEmail);
    }
}
