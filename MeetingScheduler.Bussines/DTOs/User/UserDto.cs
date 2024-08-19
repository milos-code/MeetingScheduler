namespace MeetingScheduler.Bussines.DTOs.User
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public Guid PeopleManagerId { get; set; }
        public string Position { get; set; }
        public DateTime BirthDate { get; set; }
        public List<string> RoleNames { get; set; }
    }
}
