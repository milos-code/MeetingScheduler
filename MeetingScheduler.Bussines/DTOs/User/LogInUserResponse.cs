namespace MeetingScheduler.Bussines.DTOs.User
{
    public class LogInUserResponse
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public DateTime Expiration { get; set; }
    }
}
