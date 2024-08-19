using AutoMapper;
using MeetingScheduler.Bussines.DTOs.MeetingRoom;
using MeetingScheduler.Bussines.Services.Interfaces;
using MeetingScheduler.Infrastructure.Models;
using MeetingScheduler.Infrastructure.Repositories.Interfaces;
using StudentRecords.Bussines.Exceptions;

namespace MeetingScheduler.Bussines.Services
{
    public class MeetingRoomService(
        IMeetingRoomRepository meetingRoomRepository,
        IMapper mapper) : IMeetingRoomService
    {
        private readonly IMeetingRoomRepository _meetingRoomRepository = meetingRoomRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<List<MeetingRoomDto>> GetAllMeetingRooms()
        {
            return _mapper.Map<List<MeetingRoomDto>>(await _meetingRoomRepository.GetAllMeetingRooms());
        }

        public async Task<MeetingRoomDto> GetMeetingRoomById(Guid roomId)
        {
            var meetingRoom = await _meetingRoomRepository.GetMeetingRoomById(roomId);

            ApiExceptionHandler.ObjectNotFound(meetingRoom, $"Meeting Room {roomId}");

            return _mapper.Map<MeetingRoomDto>(meetingRoom);
        }

        public async Task<MeetingRoomDto> CreateMeetingRoom(CreateMeetingRoomDto createMeetingRoomDto)
        {
            return _mapper.Map<MeetingRoomDto>(await _meetingRoomRepository.AddMeetingRoom(_mapper.Map<MeetingRoom>(createMeetingRoomDto)));
        }

        public async Task<MeetingRoomDto> UpdateMeetingRoom(UpdateMeetingRoomDto updateMeetingRoomDto)
        {
            var meetingRoom = await _meetingRoomRepository.GetMeetingRoomByName(updateMeetingRoomDto.RoomName);

            ApiExceptionHandler.ObjectNotFound(meetingRoom, $"Meeting Room {meetingRoom.RoomName}");

            return _mapper.Map<MeetingRoomDto>(await _meetingRoomRepository.UpdateMeetingRoom(_mapper.Map<MeetingRoom>(updateMeetingRoomDto)));
        }

        public async Task<bool> DeleteMeetingRoom(string roomName)
        {
            var meetingRoom = await _meetingRoomRepository.GetMeetingRoomByName(roomName);

            ApiExceptionHandler.ObjectNotFound(meetingRoom, $"Meeting Room {meetingRoom.RoomName}");

            return await _meetingRoomRepository.DeleteMeetingRoom(roomName);
        }
    }
}
