namespace MeetingScheduler.Bussines.DTOs.Meeting
{
    public class CreateMeetingDto
    {
        public string MeetingTopic { get; set; }
        public Guid RoomId { get; set; }
        public List<string>? EmployeesIds { get; set; }
        public DateTime MeetingStartTime { get; set; }
        public DateTime MeetingEndTime { get; set; }
    }
}
