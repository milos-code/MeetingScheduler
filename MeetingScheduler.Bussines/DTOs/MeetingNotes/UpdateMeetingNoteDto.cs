namespace MeetingScheduler.Bussines.DTOs.MeetingNotes
{
    public class UpdateMeetingNoteDto
    {
        public Guid Id { get; set; }
        public DateTime? Date { get; set; }
        public Guid? MeetingId { get; set; }
        public string? MeetingNote { get; set; }
        public Guid? EmployeeId { get; set; }
    }
}
