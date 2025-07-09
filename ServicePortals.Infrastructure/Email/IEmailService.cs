using Hangfire;

namespace ServicePortals.Infrastructure.Email
{
    public interface IEmailService
    {
        [AutomaticRetry(Attempts = 10)]
        Task SendEmailAsync(List<string>? to, List<string>? cc, string subject, string? body, List<(string, byte[])>? attachments, bool isHtml = true);

        [AutomaticRetry(Attempts = 10)]
        Task SendEmailResetPassword(List<string>? to, List<string>? cc, string subject, string? body, List<(string, byte[])>? attachments, bool isHtml = true);

        [AutomaticRetry(Attempts = 10)]
        Task SendEmailRequestHasBeenSent(List<string>? to, List<string>? cc, string subject, string? body, List<(string, byte[])>? attachments, bool isHtml = true);

        [AutomaticRetry(Attempts = 10)]
        Task SendEmailRejectLeaveRequest(List<string>? to, List<string>? cc, string subject, string? body, List<(string, byte[])>? attachments, bool isHtml = true);

        [AutomaticRetry(Attempts = 10)]
        Task SendEmailForNextUserApproval(List<string>? to, List<string>? cc, string subject, string? body, List<(string, byte[])>? attachments, bool isHtml = true);

        [AutomaticRetry(Attempts = 10)]
        Task SendEmailSuccessLeaveRequest(List<string>? to, List<string>? cc, string subject, string? body, List<(string, byte[])>? attachments, bool isHtml = true);
    }
}
