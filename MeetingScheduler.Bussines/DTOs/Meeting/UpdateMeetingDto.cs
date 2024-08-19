namespace MeetingScheduler.Bussines.DTOs.Meeting
{
    public class UpdateMeetingDto
    {
        public Guid Id { get; set; }
        public string? MeetingTopic { get; set; }
        public string? MeetingRoomName { get; set; }
        public List<Guid>? EmployeesId { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? MeetingStartTime { get; set; }
        public DateTime? MeetingEndTime { get; set; }
    }
}
