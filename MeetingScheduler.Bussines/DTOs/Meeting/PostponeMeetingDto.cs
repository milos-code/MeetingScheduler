namespace MeetingScheduler.Bussines.DTOs.Meeting
{
    public class PostponeMeetingDto
    {
        public Guid Id { get; set; }
        public DateTime MeetingStartTime { get; set; }
        public DateTime MeetingEndTime { get; set; }
        public string ReasonForPostponment { get; set; }
    }
}
