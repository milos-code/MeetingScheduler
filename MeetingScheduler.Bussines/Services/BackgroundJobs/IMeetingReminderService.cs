namespace MeetingScheduler.Bussines.Services.BackgroundJobs
{
    public interface IMeetingReminderService
    {
        Task ProcessMeetingBackgroundJob();
    }
}
