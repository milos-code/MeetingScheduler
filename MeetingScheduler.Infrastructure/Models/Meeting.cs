using MeetingScheduler.Infrastructure.Models.Enums;

namespace MeetingScheduler.Infrastructure.Models
{
    public class Meeting
    {
        public Guid Id { get; set; }
        public string MeetingTopic { get; set; }
        public MeetingRoom MeetingRoom { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime MeetingStartTime { get; set; }
        public DateTime MeetingEndTime { get; set; }
        public MeetingStatus Status { get; set; } = MeetingStatus.Scheduled;
        public List<User> Users { get; set; } = new List<User>();
        public List<MeetingNotes>? MeetingNotes { get; set; }
        public string? DelayOrCancelationNote { get; set; }
    }
}
