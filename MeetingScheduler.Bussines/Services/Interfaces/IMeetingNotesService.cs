using MeetingScheduler.Bussines.DTOs.MeetingNotes;

namespace MeetingScheduler.Bussines.Services.Interfaces
{
    public interface IMeetingNotesService
    {
        Task<List<MeetingNotesDto>> GetAllMeetingNotes();
        Task<MeetingNotesDto> GetMeetingNotesById(Guid noteId);
        Task<MeetingNotesDto> CreateMeetingNote(CreateMeetingNotesDto createMeetingNotesDto);
        Task<MeetingNotesDto> UpdateMeetingNote(UpdateMeetingNoteDto updateMeetingNoteDto);
        Task<bool> DeleteMeetingNote(Guid noteId);
    }
}
