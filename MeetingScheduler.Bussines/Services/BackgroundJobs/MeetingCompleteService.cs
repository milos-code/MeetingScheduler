using MeetingScheduler.Bussines.Services.Interfaces;
using MeetingScheduler.Infrastructure.Models.Enums;
using MeetingScheduler.Infrastructure.Repositories.Interfaces;

namespace MeetingScheduler.Bussines.Services.BackgroundJobs
{
    public class MeetingCompleteService(IMeetingUserRepository meetingUserRepository,
        IMeetingRepository meetingRepository,
        IEmailService emailService) : IMeetingCompleteService
    {
        private readonly IMeetingUserRepository _meetingUserRepository = meetingUserRepository;
        private readonly IMeetingRepository _meetingRepository = meetingRepository;
        private readonly IEmailService _emailService = emailService;

        public async Task CompleteMeetingBackgroundJob()
        {
            var meetingsList = await _meetingRepository.GetAllMeetingsByStatus([MeetingStatus.Scheduled, MeetingStatus.Postponed, MeetingStatus.Confirmed]);

            foreach (var meeting in meetingsList)
            {
                if (meeting.MeetingEndTime < DateTime.UtcNow)
                {
                    meeting.Status = MeetingStatus.Completed;
                    await _meetingRepository.UpdateMeeting(meeting);
                }
            }
        }
    }
}
