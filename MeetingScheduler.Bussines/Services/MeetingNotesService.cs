using AutoMapper;
using MeetingScheduler.Bussines.DTOs.MeetingNotes;
using MeetingScheduler.Bussines.Services.Interfaces;
using MeetingScheduler.Infrastructure.Models;
using MeetingScheduler.Infrastructure.Repositories.Interfaces;
using StudentRecords.Bussines.Exceptions;

namespace MeetingScheduler.Bussines.Services
{
    public class MeetingNotesService(
        IMeetingNotesRepository meetingNotesRepository,
        IMapper mapper) : IMeetingNotesService
    {
        private readonly IMeetingNotesRepository _meetingNotesRepository = meetingNotesRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<List<MeetingNotesDto>> GetAllMeetingNotes()
        {
            return _mapper.Map<List<MeetingNotesDto>>(await _meetingNotesRepository.GetAllMeetingNotes());
        }

        public async Task<MeetingNotesDto> GetMeetingNotesById(Guid noteId)
        {
            var meetingNote = await _meetingNotesRepository.GetMeetingNotesById(noteId);

            ApiExceptionHandler.ObjectNotFound(meetingNote, $"Meeting Note {meetingNote.Id}");

            return _mapper.Map<MeetingNotesDto>(meetingNote);
        }

        public async Task<MeetingNotesDto> CreateMeetingNote(CreateMeetingNotesDto createMeetingNotesDto)
        {
            return _mapper.Map<MeetingNotesDto>(await _meetingNotesRepository.AddMeetingNotes(_mapper.Map<MeetingNotes>(createMeetingNotesDto)));
        }

        public async Task<MeetingNotesDto> UpdateMeetingNote(UpdateMeetingNoteDto updateMeetingNoteDto)
        {
            var meetingNote = await _meetingNotesRepository.GetMeetingNotesById(updateMeetingNoteDto.Id);

            ApiExceptionHandler.ObjectNotFound(meetingNote, $"Meeting Note {meetingNote.Id}");

            await _meetingNotesRepository.UpdateMeetingNotes(meetingNote);

            return _mapper.Map<MeetingNotesDto>(meetingNote);
        }

        public async Task<bool> DeleteMeetingNote(Guid noteId)
        {
            var meetingNote = await _meetingNotesRepository.GetMeetingNotesById(noteId);

            ApiExceptionHandler.ObjectNotFound(meetingNote, $"Meeting Note {meetingNote.Id}");

            return await _meetingNotesRepository.DeleteMeetingNotes(noteId);
        }
    }
}
