namespace MeetingScheduler.Bussines.DTOs.JwtToken
{
    public class JwtCreationToken
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public List<string> RoleNames { get; set; }
    }
}
