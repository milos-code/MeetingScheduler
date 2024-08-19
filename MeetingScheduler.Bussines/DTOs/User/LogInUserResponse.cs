namespace MeetingScheduler.Bussines.DTOs.User
{
    public class LogInUserResponse
    {
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
    }
}
