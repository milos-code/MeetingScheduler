using MeetingScheduler.Bussines.DTOs.User;
using MeetingScheduler.Infrastructure.Models.Enums;

namespace MeetingScheduler.Bussines.DTOs.Meeting
{
    public class MeetingDto
    {
        public Guid Id { get; set; }
        public string MeetingTopic { get; set; }
        public string RoomName { get; set; }
        public List<UserDto> Users { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime MeetingStartTime { get; set; }
        public DateTime MeetingEndTime { get; set; }
        public MeetingStatus Status { get; set; }
    }
}
