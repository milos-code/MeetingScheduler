using MeetingScheduler.Infrastructure.Models.Enums;

namespace MeetingScheduler.Infrastructure.Models
{
    public class UserMeetingStatus
    {
        public Guid UserId { get; set; }
        public Guid MeetingId { get; set; }
        public virtual Meeting Meeting { get; set; }
        public virtual User User { get; set; }
        public MeetingStatus? Status { get; set; }
        public string? DelayOrCancelationNote { get; set; }
    }
}
