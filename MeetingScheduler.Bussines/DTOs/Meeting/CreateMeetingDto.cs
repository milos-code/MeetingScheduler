namespace MeetingScheduler.Bussines.DTOs.Meeting
{
    public class CreateMeetingDto
    {
        public string MeetingTopic { get; set; }
        public string RoomName { get; set; }
        public List<string>? EmployeesEmails { get; set; }
        public DateTime MeetingStartTime { get; set; }
        public DateTime MeetingEndTime { get; set; }
    }
}
