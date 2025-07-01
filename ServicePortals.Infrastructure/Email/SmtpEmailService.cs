using System.Net.Mail;
using System.Net;
using Serilog;
using ServicePortals.Infrastructure.Helpers;
using Microsoft.Extensions.Options;
using Hangfire;

namespace ServicePortals.Infrastructure.Email
{
    public class SmtpEmailService : IEmailService
    {
        private readonly SmtpOptions _settings;

        public SmtpEmailService (IOptions<SmtpOptions> settings)
        {
            _settings = settings.Value;
        }

        public async Task SendEmailAsync(List<string>? to, List<string>? cc, string subject, string? body, List<(string, byte[])>? attachments, bool isHtml = true)
        {
            await SendAsync(to, cc, subject, body, attachments, isHtml);
        }

        public async Task SendEmailForgotPassword(List<string>? to, List<string>? cc, string subject, string? body, List<(string, byte[])>? attachments, bool isHtml = true)
        {
            await SendAsync(to, cc, subject, body, attachments, isHtml);
        }

        private async Task SendAsync(List<string>? to, List<string>? cc, string subject, string? body, List<(string FileName, byte[] FileBytes)>? attachments = null, bool isHtml = true)
        {
            try
            {
                var message = new MailMessage
                {
                    From = new MailAddress(_settings.From ?? "vsit@vsvn.com.vn"),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = isHtml
                };

                if (to != null && to.Count > 0)
                {
                    foreach (var email in to)
                    {
                        message.To.Add(!string.IsNullOrWhiteSpace(email) ? email : Global.EmailDefault);
                    }
                }

                if (cc != null && cc.Count > 0)
                {
                    foreach (var email in cc)
                    {
                        message.CC.Add(!string.IsNullOrWhiteSpace(email) ? email : Global.EmailDefault);
                    }
                }

                if (attachments != null)
                {
                    foreach (var (fileName, fileBytes) in attachments)
                    {
                        var stream = new MemoryStream(fileBytes);
                        var attachment = new Attachment(stream, fileName);
                        message.Attachments.Add(attachment);
                    }
                }

                var smtp = new SmtpClient(_settings.Host ?? "smtp.office365.com")
                {
                    Port = _settings.Port ?? 587,
                    Credentials = new NetworkCredential(
                        _settings.Username ?? "vsit@vsvn.com.vn",
                        _settings.Password ?? "Mis789456."
                    ),
                    EnableSsl = _settings.EnableSsl ?? true,
                };

                await smtp.SendMailAsync(message);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to send email to {Recipients}", string.Join(", ", to));
                throw;
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
