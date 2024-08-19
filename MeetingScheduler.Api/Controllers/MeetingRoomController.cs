using MeetingScheduler.Bussines.DTOs.MeetingRoom;
using MeetingScheduler.Bussines.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MeetingScheduler.Api.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    [ApiController]
    public class MeetingRoomController(IMeetingRoomService meetingRoomService) : ControllerBase
    {
        private readonly IMeetingRoomService _meetingRoomService = meetingRoomService;

        [Authorize(Roles =("PeopleManager, Employee"))]
        [HttpGet("GetAllMeetingRooms")]
        public async Task<ActionResult<List<MeetingRoomDto>>> GetAllMeetingRooms()
        {
            return Ok(await _meetingRoomService.GetAllMeetingRooms());
        }

        [HttpGet("GetMeetingRoomById")]
        public async Task<ActionResult<MeetingRoomDto>> GetMeetingRoomById(Guid meetingRoomId)
        {
            return Ok(await _meetingRoomService.GetMeetingRoomById(meetingRoomId));
        }

        [HttpPost("CreateMeetingRoom")]
        public async Task<ActionResult<MeetingRoomDto>> CreateMeetingRoom(CreateMeetingRoomDto createMeetingRoomDto)
        {
            return await _meetingRoomService.CreateMeetingRoom(createMeetingRoomDto);
        }

        [HttpPut("UpdateMeetingRoom")]
        public async Task<ActionResult<MeetingRoomDto>> UpdateMeetingRoom(UpdateMeetingRoomDto updateMeetingRoomDto)
        {
            return await _meetingRoomService.UpdateMeetingRoom(updateMeetingRoomDto);
        }

        [HttpDelete("DeleteMeetingRoom")]
        public async Task<ActionResult> DeleteMeetingRoom(string roomName)
        {
            await _meetingRoomService.DeleteMeetingRoom(roomName);

            return Ok("Meeting room has been deleted.");
        }

    }
}
