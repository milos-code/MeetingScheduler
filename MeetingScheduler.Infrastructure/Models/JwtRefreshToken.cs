namespace MeetingScheduler.Infrastructure.Models
{
    public class JwtRefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public string UserId { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}
