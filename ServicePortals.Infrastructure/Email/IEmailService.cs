using Hangfire;

namespace ServicePortals.Infrastructure.Email
{
    public interface IEmailService
    {
        [AutomaticRetry(Attempts = 10)]
        Task SendEmailResetPassword(List<string>? to, List<string>? cc, string subject, string? body, List<(string, byte[])>? attachments, bool isHtml = true);


        [AutomaticRetry(Attempts = 10)]
        Task SendEmailLeaveRequest(List<string>? to, List<string>? cc, string subject, string? body, List<(string, byte[])>? attachments, bool isHtml = true);


        [AutomaticRetry(Attempts = 10)]
        Task SendEmailMemoNotification(List<string>? to, List<string>? cc, string subject, string? body, List<(string, byte[])>? attachments, bool isHtml = true);


        [AutomaticRetry(Attempts = 10)]
        Task SendEmailTimeKeepingToHR(List<string>? to, List<string>? cc, string subject, string? body, List<(string, byte[])>? attachments, bool isHtml = false);


        [AutomaticRetry(Attempts = 10)]
        Task SendEmailFormIT(List<string>? to, List<string>? cc, string subject, string? body, List<(string, byte[])>? attachments, bool isHtml = true);


        [AutomaticRetry(Attempts = 10)]
        Task SendEmailPurchase(List<string>? to, List<string>? cc, string subject, string? body, List<(string, byte[])>? attachments, bool isHtml = true);


        [AutomaticRetry(Attempts = 10)]
        Task SendEmailInternalMemoHr(List<string>? to, List<string>? cc, string subject, string? body, List<(string, byte[])>? attachments, bool isHtml = true);


        [AutomaticRetry(Attempts = 10)]
        Task SendEmailMissTimeKeeping(List<string>? to, List<string>? cc, string subject, string? body, List<(string, byte[])>? attachments, bool isHtml = true);


        [AutomaticRetry(Attempts = 10)]
        Task SendEmailOverTime(List<string>? to, List<string>? cc, string subject, string? body, List<(string, byte[])>? attachments, bool isHtml = true);
    }
}
