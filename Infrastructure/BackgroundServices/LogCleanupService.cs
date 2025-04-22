using ServicePortal.Common.Helpers;
using ServicePortal.Domain.Enums;

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
                    FileHelper.CleanupOldLogs();
                }
                catch (Exception ex)
                {
                    FileHelper.WriteLog(TypeErrorEnum.ERROR, $"Error clean log file, error: {ex.Message}");
                }

                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
        }
    }
}
