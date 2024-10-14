using MeetingScheduler.Bussines.DTOs.User;

namespace MeetingScheduler.Bussines.Services.Authentication
{
    public interface IAuthenticationService
    {
        Task<LogInUserResponse> LogInUser(LogInUserDto logInUser);
        Task LogOut();
        Task<LogInUserResponse> RefreshToken(string token, string refreshToken);
    }
}
