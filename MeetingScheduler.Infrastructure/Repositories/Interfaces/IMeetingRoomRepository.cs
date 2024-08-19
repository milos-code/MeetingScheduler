using MeetingScheduler.Infrastructure.Models;

namespace MeetingScheduler.Infrastructure.Repositories.Interfaces
{
    public interface IMeetingRoomRepository
    {
        Task<List<MeetingRoom>> GetAllMeetingRooms();
        Task<MeetingRoom> GetMeetingRoomById(Guid id);
        Task<MeetingRoom> GetMeetingRoomByName(string roomName);
        Task<MeetingRoom> AddMeetingRoom(MeetingRoom meetingRoom);
        Task<MeetingRoom> UpdateMeetingRoom(MeetingRoom meetingRoom);
        Task<bool> DeleteMeetingRoom(string roomName);
    }
}
