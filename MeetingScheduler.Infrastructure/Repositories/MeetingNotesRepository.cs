using MeetingScheduler.Infrastructure.AppContext;
using MeetingScheduler.Infrastructure.Models;
using MeetingScheduler.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MeetingScheduler.Infrastructure.Repositories
{
    public class MeetingNotesRepository(MeetingSchedulerContext context) : IMeetingNotesRepository
    {
        private readonly MeetingSchedulerContext _context = context;

        public async Task<List<MeetingNotes>> GetAllMeetingNotes()
        {
            return await _context.MeetingNotes.ToListAsync();
        }

        public async Task<MeetingNotes> GetMeetingNotesById(Guid notesId)
        {
            return await _context.MeetingNotes.FirstOrDefaultAsync(n => n.Id == notesId);
        }

        public async Task<MeetingNotes> AddMeetingNotes(MeetingNotes meetingNotes)
        {
            var result = await _context.MeetingNotes.AddAsync(meetingNotes);
            await _context.SaveChangesAsync();

            return result.Entity;
        }

        public async Task<MeetingNotes> UpdateMeetingNotes(MeetingNotes meetingNotes)
        {
            _context.MeetingNotes.Update(meetingNotes);
            await _context.SaveChangesAsync();

            return meetingNotes;
        }

        public async Task<bool> DeleteMeetingNotes(Guid notesId)
        {
            var notes = await _context.MeetingNotes.FirstOrDefaultAsync(n => n.Id == notesId);
            if(notes == null) { return false; }

            _context.MeetingNotes.Remove(notes);

            return await _context.SaveChangesAsync() > 0;
        }

    }
}
