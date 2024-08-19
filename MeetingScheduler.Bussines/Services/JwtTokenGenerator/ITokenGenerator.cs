using MeetingScheduler.Bussines.DTOs.JwtToken;
using MeetingScheduler.Bussines.DTOs.User;

namespace MeetingScheduler.Bussines.Services.JwtTokenGenerator
{
    public interface ITokenGenerator
    {
        Task<LogInUserResponse> CreateToken(JwtCreationToken user);
    }
}
