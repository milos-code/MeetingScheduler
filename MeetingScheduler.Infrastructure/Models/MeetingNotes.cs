namespace MeetingScheduler.Infrastructure.Models
{
    public class MeetingNotes
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public virtual Meeting Meeting { get; set; }
        public string Note { get; set; }
        public User Employee { get; set; }
    }
}
