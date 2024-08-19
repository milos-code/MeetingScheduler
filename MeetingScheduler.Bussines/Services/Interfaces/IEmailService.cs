using MeetingScheduler.Bussines.DTOs.Email;

namespace MeetingScheduler.Bussines.Services.Interfaces
{
    public interface IEmailService
    {
        Task<bool> SendEmail(List<string> usersEmails, string subject, string body);
    }
}
