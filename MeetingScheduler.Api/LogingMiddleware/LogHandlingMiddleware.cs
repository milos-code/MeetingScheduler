using Serilog;

namespace MeetingScheduler.Api.LogingMiddleware
{
    public static class LogHandlingMiddleware
    {
        public static void LogError(Exception ex, string message = "")
        {
            string loggingPath = Path.Combine(AppContext.BaseDirectory, "Logs");
            string logMessage = !String.IsNullOrWhiteSpace(message) ? message + " " + ex.Message + "\n" : ex.Message + "\n";

            if (!Directory.Exists(loggingPath)) { Directory.CreateDirectory(loggingPath); }

            Log.Logger = new LoggerConfiguration().WriteTo.File(Path.Combine(loggingPath, "log-.txt"), rollingInterval: RollingInterval.Day).CreateLogger();
            Log.Error(logMessage);
            Log.CloseAndFlush();
        }
    }
}
