namespace MeetingScheduler.Bussines.DTOs.MeetingNotes
{
    public class CreateMeetingNotesDto
    {
        public DateTime Date { get; set; }
        public Guid MeetingId { get; set; }
        public string Note { get; set; }
        public Guid EmployeeId { get; set; }
    }
}
