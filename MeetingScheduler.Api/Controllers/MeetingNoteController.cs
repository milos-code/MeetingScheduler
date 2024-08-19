using MeetingScheduler.Bussines.DTOs.MeetingNotes;
using MeetingScheduler.Bussines.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MeetingScheduler.Api.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles = "PeopleManager")]
    [ApiController]
    public class MeetingNoteController(IMeetingNotesService meetingNotesService) : ControllerBase
    {
        private readonly IMeetingNotesService _meetingNotesService = meetingNotesService;

        [HttpGet("GetAllMeetingNotes")]
        public async Task<ActionResult<List<MeetingNotesDto>>> GetAllMeetingNotes()
        {
            return await _meetingNotesService.GetAllMeetingNotes();
        }

        [HttpGet("GetMeetingNoteById")]
        public async Task<ActionResult<MeetingNotesDto>> GetMeetingNoteById(Guid noteId)
        {
            return await _meetingNotesService.GetMeetingNotesById(noteId);
        }

        [HttpPost("CreateMeetingNote")]
        public async Task<ActionResult<MeetingNotesDto>> CreateMeetingNote(CreateMeetingNotesDto createMeetingNotesDto)
        {
            return await _meetingNotesService.CreateMeetingNote(createMeetingNotesDto);
        }

        [HttpPut("UpdateMeetingNote")]
        public async Task<ActionResult<MeetingNotesDto>> UpdateMeetingNote(UpdateMeetingNoteDto updateMeetingNoteDto)
        {
            return await _meetingNotesService.UpdateMeetingNote(updateMeetingNoteDto);
        }

        [HttpDelete("DeleteMeetingNote")]
        public async Task<ActionResult> DeleteMeetingNote(Guid noteId)
        {
            await _meetingNotesService.DeleteMeetingNote(noteId);

            return Ok("Meeting note has been deleted.");
        }
    }
}
