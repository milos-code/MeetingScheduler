using Microsoft.AspNetCore.Identity;

namespace MeetingScheduler.Infrastructure.Models
{
    public class User : IdentityUser<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public virtual User? PeopleManager { get; set; }
        public Guid? PeopleManagerId { get; set; }
        public string? Position { get; set; }
        public DateTime BirthDate { get; set; }
        public List<Meeting> Meetings { get; set; }
    }
}
