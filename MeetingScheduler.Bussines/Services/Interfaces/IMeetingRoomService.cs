using MeetingScheduler.Bussines.DTOs.MeetingRoom;

namespace MeetingScheduler.Bussines.Services.Interfaces
{
    public interface IMeetingRoomService
    {
        Task<List<MeetingRoomDto>> GetAllMeetingRooms();
        Task<MeetingRoomDto> GetMeetingRoomById(Guid roomId);
        Task<MeetingRoomDto> CreateMeetingRoom(CreateMeetingRoomDto createMeetingRoomDto);
        Task<MeetingRoomDto> UpdateMeetingRoom(UpdateMeetingRoomDto updateMeetingRoomDto);
        Task<bool> DeleteMeetingRoom(string roomName);
    }
}
