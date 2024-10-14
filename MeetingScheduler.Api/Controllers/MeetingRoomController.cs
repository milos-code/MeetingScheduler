using MeetingScheduler.Bussines.DTOs.MeetingRoom;
using MeetingScheduler.Bussines.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MeetingScheduler.Api.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class MeetingRoomController(IMeetingRoomService meetingRoomService) : ControllerBase
    {
        private readonly IMeetingRoomService _meetingRoomService = meetingRoomService;

        [Authorize(Roles = ("Admin, PeopleManager, Employee"))]
        [HttpGet("GetAllMeetingRooms")]
        public async Task<ActionResult<List<MeetingRoomDto>>> GetAllMeetingRooms()
        {
            return Ok(await _meetingRoomService.GetAllMeetingRooms());
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("GetMeetingRoomById")]
        public async Task<ActionResult<MeetingRoomDto>> GetMeetingRoomById(Guid meetingRoomId)
        {
            return Ok(await _meetingRoomService.GetMeetingRoomById(meetingRoomId));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("CreateMeetingRoom")]
        public async Task<ActionResult<MeetingRoomDto>> CreateMeetingRoom(CreateMeetingRoomDto createMeetingRoomDto)
        {
            return await _meetingRoomService.CreateMeetingRoom(createMeetingRoomDto);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("UpdateMeetingRoom")]
        public async Task<ActionResult<MeetingRoomDto>> UpdateMeetingRoom(UpdateMeetingRoomDto updateMeetingRoomDto)
        {
            return await _meetingRoomService.UpdateMeetingRoom(updateMeetingRoomDto);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("DeleteMeetingRoom")]
        public async Task<ActionResult> DeleteMeetingRoom(string roomId)
        {
            await _meetingRoomService.DeleteMeetingRoom(roomId);

            return Ok("Meeting room has been deleted.");
        }

    }
}
