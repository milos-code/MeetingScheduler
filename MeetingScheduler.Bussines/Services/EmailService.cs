using FluentEmail.Core;
using FluentEmail.Core.Models;
using MeetingScheduler.Bussines.DTOs.Email;
using MeetingScheduler.Bussines.Services.Interfaces;

namespace MeetingScheduler.Bussines.Services
{
    public class EmailService(IFluentEmailFactory fluentEmail) : IEmailService
    {
        private readonly IFluentEmailFactory _fluentEmail = fluentEmail;

        public async Task<bool> SendEmail(List<string> usersEmails, string subject, string body)
        {
            bool isSuccessful = true;

            foreach (var userEmail in usersEmails)
            {
                var emailToSend = new EmailMetadata(userEmail, subject, body);

                isSuccessful = await Send(emailToSend) && isSuccessful;
            }

            return isSuccessful;
        }

        private async Task<bool> Send(EmailMetadata emailMetadata)
        {
            SendResponse response;
            try
            {
                response = await _fluentEmail
                    .Create()
                    .To(emailMetadata.ToAddress)
                    .Subject(emailMetadata.Subject)
                    .Body(emailMetadata.Body)
                    .SendAsync();
            }
            catch (Exception)
            {
                return false;
            }

            return response.Successful;
        }
    }
}
