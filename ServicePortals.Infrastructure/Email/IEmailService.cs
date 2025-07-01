using Hangfire;

namespace ServicePortals.Infrastructure.Email
{
    public interface IEmailService
    {
        [AutomaticRetry(Attempts = 10)]
        Task SendEmailAsync(List<string>? to, List<string>? cc, string subject, string? body, List<(string, byte[])>? attachments, bool isHtml = true);

        [AutomaticRetry(Attempts = 10)]
        Task SendEmailForgotPassword(List<string>? to, List<string>? cc, string subject, string? body, List<(string, byte[])>? attachments, bool isHtml = true);
    }
}
