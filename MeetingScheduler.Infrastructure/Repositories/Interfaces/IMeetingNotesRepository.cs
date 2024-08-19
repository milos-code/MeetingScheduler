using MeetingScheduler.Infrastructure.Models;

namespace MeetingScheduler.Infrastructure.Repositories.Interfaces
{
    public interface IMeetingNotesRepository
    {
        Task<List<MeetingNotes>> GetAllMeetingNotes();
        Task<MeetingNotes> GetMeetingNotesById(Guid notesId);
        Task<MeetingNotes> AddMeetingNotes(MeetingNotes meetingNotes);
        Task<MeetingNotes> UpdateMeetingNotes(MeetingNotes meetingNotes);
        Task<bool> DeleteMeetingNotes(Guid notesId);
    }
}
