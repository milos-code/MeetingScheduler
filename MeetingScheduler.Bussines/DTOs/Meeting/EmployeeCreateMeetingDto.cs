namespace MeetingScheduler.Bussines.DTOs.Meeting
{
    public class EmployeeCreateMeetingDto
    {
        public string MeetingTopic { get; set; }
        public string RoomName { get; set; }
        public DateTime MeetingStartTime { get; set; }
        public DateTime MeetingEndTime { get; set; }
    }
}
