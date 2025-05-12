using Serilog;
using ServicePortal.Common.Helpers;

namespace ServicePortal.Infrastructure.BackgroundServices
{
    public class LogCleanupService : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    FileHelper.DeleteOldFiles(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs"), DateTime.Now, 7);
                }
                catch (Exception ex)
                {
                    Log.Error($"Error can not clean log file, ex:{ex.Message}");
                }
                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
        }
    }
}
