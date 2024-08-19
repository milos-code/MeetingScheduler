namespace MeetingScheduler.Bussines.DTOs.Meeting
{
    public class EmployeeMeetingsDto
    {
        public string MeetingTopic { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime MeetingStartTime { get; set; }
        public DateTime MeetingEndTime { get; set; }
        public string Username { get; set; }
    }
}
