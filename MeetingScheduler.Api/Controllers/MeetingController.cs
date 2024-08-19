using MeetingScheduler.Bussines.DTOs.Meeting;
using MeetingScheduler.Bussines.Services.Interfaces;
using MeetingScheduler.Infrastructure.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MeetingScheduler.Api.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class MeetingController(IMeetingService meetingService) : ControllerBase
    {
        private readonly IMeetingService _meetingService = meetingService;

        [HttpGet("GetAllMeeting")]
        public async Task<ActionResult<List<MeetingDto>>> GetAllMeetingNotes()
        {
            return await _meetingService.GetAllMeetings();
        }

        [HttpGet("GetMeetingById")]
        public async Task<ActionResult<MeetingDto>> GetMeetingById(Guid meetingId)
        {
            return await _meetingService.GetMeetingById(meetingId);
        }

        [HttpGet("GetMeetingByDate")]
        public async Task<ActionResult<MeetingDto>> GetMeetingByDate(DateTime date)
        {
            return await _meetingService.GetMeetingByDate(date);
        }

        [HttpGet("GetAllMeetingsByRoomNameOrStartTime")]
        public async Task<ActionResult<List<MeetingDto>>> GetAllMeetingsByRoomNameOrStartTime(DateOnly? meetingStartTime, string? meetingRoomName)
        {
            return Ok(await _meetingService.GetAllMeetingsByRoomNameOrStartTime(meetingStartTime, meetingRoomName));
        }

        [Authorize(Roles = "PeopleManager")]
        [HttpPost("CreateMeeting")]
        public async Task<ActionResult<MeetingDto>> CreateMeeting(CreateMeetingDto createMeetingDto)
        {
            return await _meetingService.CreateMeeting(createMeetingDto);
        }

        [Authorize(Roles = "Employee")]
        [HttpPost("EmployeeCreateMeeting")]
        public async Task<ActionResult<MeetingDto>> EmployeeCreateMeeting(EmployeeCreateMeetingDto employeeCreateMeetingDto)
        {
            return await _meetingService.EmployeeCreateMeeting(employeeCreateMeetingDto);
        }

        [Authorize(Roles = "PeopleManager")]
        [HttpPost("PostponeMeeting")]
        public async Task<ActionResult<MeetingDto>> PostponeMeeting(PostponeMeetingDto postponeMeeting)
        {
            await _meetingService.PostponeMeetinng(postponeMeeting);

            return Ok("Meeting postponed");
        }

        [Authorize(Roles = "PeopleManager")]
        [HttpPost("CancelMeeting")]
        public async Task<ActionResult> CancelMeeting(Guid id, string reasonForCancelation)
        {
            await _meetingService.CancelMeeting(id, reasonForCancelation);

            return Ok("Meeting has been cancled.");
        }

        [Authorize(Roles = "PeopleManager")]
        [HttpPut("UpdateMeeting")]
        public async Task<ActionResult<MeetingDto>> UpdateMeeting(UpdateMeetingDto updateMeetingNoteDto)
        {
            return await _meetingService.UpdateMeeting(updateMeetingNoteDto);
        }

        [Authorize(Roles = "Employee")]
        [HttpPost("UserConfirmMeeting")]
        public async Task<ActionResult> UserConfirmMeeting(Guid userId, Guid meetingId)
        {
            await _meetingService.UpdateUserMeetingStatus(userId, meetingId, MeetingStatus.Confirmed, "");

            return Ok("Meeting has been confirmed.");
        }

        [Authorize(Roles = "Employee")]
        [HttpPost("UserRejectMeeting")]
        public async Task<ActionResult> UserRejectMeeting(Guid userId, Guid meetingId, string note)
        {
            await _meetingService.UpdateUserMeetingStatus(userId, meetingId,MeetingStatus.Canceled, note);

            return Ok("Meeting has been rejected.");
        }

        [Authorize(Roles = "PeopleManager")]
        [HttpPost("GetAllMeetingsForAnEmployee")]
        public async Task<ActionResult<List<EmployeeMeetingsDto>>> GetAllMeetingsForAnEmployee(string userEmail)
        {
            return Ok(await _meetingService.GetAllMeetingsForAnEmployee(userEmail));
        }

        [Authorize(Roles = "Employee")]
        [HttpPost("GetAllMeetingsForEmployeeWithPeopleManager")]
        public async Task<ActionResult<List<EmployeeMeetingsDto>>> GetAllMeetingsForEmployeeWithPeopleManager()
        {
            return Ok(await _meetingService.GetAllMeetingsForEmployeeWithPeopleManager());
        }
    }
}
