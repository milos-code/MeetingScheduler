using MeetingScheduler.Bussines.DTOs.Meeting;
using MeetingScheduler.Infrastructure.Models.Enums;

namespace MeetingScheduler.Bussines.Services.Interfaces
{
    public interface IMeetingService
    {
        Task<List<MeetingDto>> GetAllMeetings();
        Task<MeetingDto> GetMeetingById(Guid meetingId);
        Task<MeetingDto> GetMeetingByDate(DateTime date);
        Task<List<MeetingDto>> GetAllMeetingsByRoomNameOrStartTime(DateOnly? meetingStartTime, string? meetingRoomName);
        Task<MeetingDto> CreateMeeting(CreateMeetingDto createMeetingDto);
        Task<MeetingDto> EmployeeCreateMeeting(EmployeeCreateMeetingDto employeeCreateMeetingDto);
        Task<MeetingDto> PostponeMeetinng(PostponeMeetingDto postponeMeetingDto);
        Task<MeetingDto> UpdateMeeting(UpdateMeetingDto updateMeetingDto);
        Task<MeetingDto> CancelMeeting(Guid meetingId, string reasonForCancelation);
        Task UpdateUserMeetingStatus(Guid userId, Guid meetingId, MeetingStatus status, string? note);
        Task<List<EmployeeMeetingsDto>> GetAllMeetingsForAnEmployee(string userEmail);
        Task<List<EmployeeMeetingsDto>> GetAllMeetingsForEmployeeWithPeopleManager();
    }
}
