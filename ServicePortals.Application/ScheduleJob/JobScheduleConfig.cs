using Hangfire;
using ServicePortals.Infrastructure.Helpers;
using ServicePortals.Infrastructure.Services.Auth;

namespace ServicePortals.Application.ScheduleJob
{
    public static class JobScheduleConfig
    {
        public static void Configure()
        {
            //job auto delete token every week
            RecurringJob.AddOrUpdate<JwtService>(
                "DeleteOldRefreshTokensDaily",
                (job) => job.DeleteOldRefreshToken(),
                Cron.Weekly
            );

            //job auto delete old file every week
            RecurringJob.AddOrUpdate(
                "DeleteOldFiles",
                () => Helper.DeleteOldFiles(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs"), DateTime.Now, 7),
                Cron.Weekly
            );
        }
    }
}
