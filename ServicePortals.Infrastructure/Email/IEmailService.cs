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

        [AutomaticRetry(Attempts = 10)]
        Task SendEmailManyPeopleLeaveRequest(List<string>? to, List<string>? cc, string subject, string? body, List<(string, byte[])>? attachments, bool isHtml = true);


        #region Email memo notification approval

        [AutomaticRetry(Attempts = 10)]
        Task EmailSendMemoNotification(List<string>? to, List<string>? cc, string subject, string? body, List<(string, byte[])>? attachments, bool isHtml = true);

        [AutomaticRetry(Attempts = 10)]
        Task EmailSendMemoNotificationHasBeenCompletedOrReject(List<string>? to, List<string>? cc, string subject, string? body, List<(string, byte[])>? attachments, bool isHtml = true);

        #endregion

        #region Email gửi chấm công đến HR
        [AutomaticRetry(Attempts = 10)]
        Task EmailSendTimeKeepingToHR(List<string>? to, List<string>? cc, string subject, string? body, List<(string, byte[])>? attachments, bool isHtml = false);
        #endregion

        [AutomaticRetry(Attempts = 10)]
        Task SendEmailFormIT(List<string>? to, List<string>? cc, string subject, string? body, List<(string, byte[])>? attachments, bool isHtml = true);

        [AutomaticRetry(Attempts = 10)]
        Task SendEmailPurchase(List<string>? to, List<string>? cc, string subject, string? body, List<(string, byte[])>? attachments, bool isHtml = true);
    }
}
