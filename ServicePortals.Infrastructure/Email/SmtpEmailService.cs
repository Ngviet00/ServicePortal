using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Serilog;
using ServicePortals.Infrastructure.Helpers;
using Microsoft.Extensions.Options;

namespace ServicePortals.Infrastructure.Email
{
    public class SmtpEmailService : IEmailService
    {
        private readonly SmtpOptions _settings;

        public SmtpEmailService (IOptions<SmtpOptions> settings)
        {
            _settings = settings.Value;
        }

        public async Task SendEmailFormIT(List<string>? to, List<string>? cc, string subject, string? body, List<(string, byte[])>? attachments, bool isHtml = true)
        {
            await SendAsync(to, cc, subject, body, attachments, isHtml);
        }

        public async Task SendEmailInternalMemoHr(List<string>? to, List<string>? cc, string subject, string? body, List<(string, byte[])>? attachments, bool isHtml = true)
        {
            await SendAsync(to, cc, subject, body, attachments, isHtml);
        }

        public async Task SendEmailLeaveRequest(List<string>? to, List<string>? cc, string subject, string? body, List<(string, byte[])>? attachments, bool isHtml = true)
        {
            await SendAsync(to, cc, subject, body, attachments, isHtml);
        }

        public async Task SendEmailMemoNotification(List<string>? to, List<string>? cc, string subject, string? body, List<(string, byte[])>? attachments, bool isHtml = true)
        {
            await SendAsync(to, cc, subject, body, attachments, isHtml);
        }

        public async Task SendEmailMissTimeKeeping(List<string>? to, List<string>? cc, string subject, string? body, List<(string, byte[])>? attachments, bool isHtml = true)
        {
            await SendAsync(to, cc, subject, body, attachments, isHtml);
        }

        public async Task SendEmailOverTime(List<string>? to, List<string>? cc, string subject, string? body, List<(string, byte[])>? attachments, bool isHtml = true)
        {
            await SendAsync(to, cc, subject, body, attachments, isHtml);
        }

        public async Task SendEmailPurchase(List<string>? to, List<string>? cc, string subject, string? body, List<(string, byte[])>? attachments, bool isHtml = true)
        {
            await SendAsync(to, cc, subject, body, attachments, isHtml);
        }

        public async Task SendEmailResetPassword(List<string>? to, List<string>? cc, string subject, string? body, List<(string, byte[])>? attachments, bool isHtml = true)
        {
            await SendAsync(to, cc, subject, body, attachments, isHtml);
        }

        public async Task SendEmailTimeKeepingToHR(List<string>? to, List<string>? cc, string subject, string? body, List<(string, byte[])>? attachments, bool isHtml = false)
        {
            await SendAsync(to, cc, subject, body, attachments, isHtml);
        }

        private async Task SendAsync(List<string>? to, List<string>? cc, string subject, string? body, List<(string FileName, byte[] FileBytes)>? attachments = null, bool isHtml = true)
        {
            try
            {
                var message = new MimeMessage();

                message.From.Add(new MailboxAddress(
                    "System",
                    _settings.From ?? "vsit@vsvn.com.vn"));

                if (to != null && to.Count > 0)
                {
                    foreach (var email in to)
                    {
                        message.To.Add(new MailboxAddress("", email));
                    }
                }

                if (cc != null && cc.Count > 0)
                {
                    foreach (var email in cc)
                    {
                        message.Cc.Add(new MailboxAddress("", email));
                    }
                }

                message.Cc.Add(new MailboxAddress("", Global.EmailDefault));

                message.Subject = subject;

                var builder = new BodyBuilder
                {
                    HtmlBody = isHtml ? body : null,
                    TextBody = !isHtml ? body : null
                };

                if (attachments != null)
                {
                    foreach (var (fileName, fileBytes) in attachments)
                    {
                        builder.Attachments.Add(fileName, fileBytes);
                    }
                }

                message.Body = builder.ToMessageBody();

                using var client = new SmtpClient();
                await client.ConnectAsync(
                    _settings.Host ?? "smtp.office365.com",
                    _settings.Port ?? 587,
                    _settings.EnableSsl ?? true
                        ? SecureSocketOptions.StartTls
                        : SecureSocketOptions.None);

                await client.AuthenticateAsync(
                    _settings.Username ?? "vsit@vsvn.com.vn",
                    _settings.Password ?? "Mis789456.");

                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                Log.Error($@"Send email error, message: {ex}, info: {to}, {cc}, {subject}, {body}");
            }
        }
    }

    public class SmtpOptions
    {
        public string? Host { get; set; }
        public int? Port { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? From { get; set; }
        public bool? EnableSsl { get; set; }
    }
}
