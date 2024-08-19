using MeetingScheduler.Infrastructure.Models;

namespace MeetingScheduler.Infrastructure.Repositories.Interfaces
{
    public interface IMeetingUserRepository
    {
        //Task<List<UserMeetingStatus>> GetAllMeetingsWithAnEmployee(string userEmail);
        //Task<List<UserMeetingStatus>> GetAllMeetingsForEmployeeWithPeopleManager(Guid userId, Guid peopleManagerId);
        Task<UserMeetingStatus> GetMeetingUserStatus(Guid userId, Guid meetingId);
        Task<List<UserMeetingStatus>> GetAllUsersForMeeting(Guid meetingId);
        Task UpdateMeetingStatus(UserMeetingStatus userMeetingStatus);
    }
}
