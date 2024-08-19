using Hangfire;
using MeetingScheduler.Bussines.Services.Interfaces;
using MeetingScheduler.Infrastructure.Models;
using MeetingScheduler.Infrastructure.Models.Enums;
using MeetingScheduler.Infrastructure.Repositories.Interfaces;
using StudentRecords.Bussines.Exceptions;
using System.Net;

namespace MeetingScheduler.Bussines.Services.BackgroundJobs
{
    public class MeetingReminderService(IEmailService emailService,
        IMeetingUserRepository meetingUserRepository,
        IMeetingRepository meetingRepository,
        IBackgroundJobClient backgroundJobClient) : IMeetingReminderService
    {
        private readonly IEmailService _emailService = emailService;
        private readonly IMeetingUserRepository _meetingUserRepository = meetingUserRepository;
        private readonly IMeetingRepository _meetingRepository = meetingRepository;
        private readonly IBackgroundJobClient _backgroundJobClient = backgroundJobClient;

        public async Task ProcessMeetingBackgroundJob()
        {
            var meetingsList = await _meetingRepository.GetAllMeetingsByStatus([MeetingStatus.Scheduled, MeetingStatus.Postponed]);

            foreach (var meeting in meetingsList)
            {
                var userStatuses = await _meetingUserRepository.GetAllUsersForMeeting(meeting.Id);

                var userMeetingStatuses = userStatuses
                    .GroupBy(ums => ums.Meeting.Id)
                    .Select(g => new
                    {
                        TotalUsers = g.Count(),
                        CancledUsers = g.Count(ums => ums.Status == MeetingStatus.Canceled),
                        ConfirmedUsers = g.Count(ums => ums.Status == MeetingStatus.Confirmed),
                        UsersNonResponse = g.Count(ums => ums.Status == null)
                    })
                    .FirstOrDefault();

                var usersEmails = meeting.Users
                            .Select(e => e.Email)
                            .ToList();

                var nonResposeUsers = (double)userMeetingStatuses.UsersNonResponse / userMeetingStatuses.TotalUsers;
                if (nonResposeUsers > 0.50)
                {
                    return;
                }

                var cancelationPercent = (double)userMeetingStatuses.CancledUsers / userMeetingStatuses.TotalUsers;
                if (cancelationPercent > 0.50)
                {
                    await CancelMeetingAndSendEmail(meeting, usersEmails);
                }

                var confirmationPercent = (double)userMeetingStatuses.ConfirmedUsers / userMeetingStatuses.TotalUsers;
                if (confirmationPercent >= 0.50)
                {
                    await ConfirmMeetingAndSendReminderEmail(meeting, usersEmails);
                }
            }
        }

        private async Task ConfirmMeetingAndSendReminderEmail(Meeting meeting, List<string?> usersEmails)
        {
            meeting.Status = MeetingStatus.Confirmed;
            await _meetingRepository.UpdateMeeting(meeting);

            await SendEmailReminders(usersEmails,
                                    "Meeting Reminder",
                                    $"This is a reminder for your meeting scheduled on {meeting.MeetingStartTime}.",
                                    meeting.MeetingStartTime);
        }

        private async Task CancelMeetingAndSendEmail(Meeting meeting, List<string?> usersEmails)
        {
            meeting.Status = MeetingStatus.Canceled;
            await _meetingRepository.UpdateMeeting(meeting);

            var emailResult = await _emailService.SendEmail(
                usersEmails,
                "Meeting Canceled",
                $"The meeting {meeting.MeetingStartTime} has been canceled.");
            if (emailResult is false)
            {
                ApiExceptionHandler.ThrowApiException(HttpStatusCode.BadRequest, "Error sending confirmation token via email.");
            }
        }

        private async Task SendEmailReminders(List<string> emails, string subject, string body, DateTime meetingStartTime)
        {
            var reminderTime = meetingStartTime.AddMinutes(-30);
            var delay = reminderTime - DateTime.UtcNow;

            if (delay > TimeSpan.Zero)
            {
                _backgroundJobClient.Schedule(() => _emailService.SendEmail(emails, subject, body), delay);
            }
        }
    }
}
