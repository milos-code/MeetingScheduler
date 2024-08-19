using MeetingScheduler.Infrastructure.AppContext;
using MeetingScheduler.Infrastructure.Models;
using MeetingScheduler.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MeetingScheduler.Infrastructure.Repositories
{
    public class MeetingRoomRepository(MeetingSchedulerContext context) : IMeetingRoomRepository
    {
        public MeetingSchedulerContext _context  = context;

        public async Task<List<MeetingRoom>> GetAllMeetingRooms()
        {
            return await _context.MeetingRooms.ToListAsync();
        }

        public async Task<MeetingRoom> GetMeetingRoomById(Guid id)
        {
            return await _context.MeetingRooms.FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<MeetingRoom> GetMeetingRoomByName(string roomName)
        {
            return await _context.MeetingRooms.FirstOrDefaultAsync(m => m.RoomName == roomName);
        }

        public async Task<MeetingRoom> AddMeetingRoom(MeetingRoom meetingRoom)
        {
            var result = await _context.MeetingRooms.AddAsync(meetingRoom);
            await _context.SaveChangesAsync();

            return result.Entity;
        }

        public async Task<MeetingRoom> UpdateMeetingRoom(MeetingRoom meetingRoom)
        {
            _context.MeetingRooms.Update(meetingRoom);
            await _context.SaveChangesAsync();

            return meetingRoom;
        }

        public async Task<bool> DeleteMeetingRoom(string roomName)
        {
            var meetingRoom = await _context.MeetingRooms.FirstOrDefaultAsync(m => m.RoomName == roomName);
            if(meetingRoom == null) { return false; }

            _context.MeetingRooms.Remove(meetingRoom);

            return await _context.SaveChangesAsync() > 0;
        }       
    }
}
