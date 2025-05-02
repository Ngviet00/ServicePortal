using System.Net.Mail;
using System.Net;

namespace ServicePortal.Infrastructure.Email
{
    public class EmailService
    {
        private readonly SmtpClient _smtpClient;
        private readonly string _from;

        public EmailService()
        {
            _from = "nguyenviet@vsvn.com.vn";
            _smtpClient = new SmtpClient("smtp.office365.com") // Thay bằng SMTP của công ty
            {
                Port = 587, // Hoặc 587 nếu dùng TLS
                EnableSsl = false, // Đặt true nếu SMTP yêu cầu SSL/TLS
                UseDefaultCredentials = false,
                //Credentials = new NetworkCredential("your_email@yourcompany.com", "your_password") // Nếu cần xác thực
            };
        }

        public void SendEmail(string to, string subject, string body)
        {
            var mailMessage = new MailMessage(_from, to, subject, body)
            {
                IsBodyHtml = true // Đặt true nếu gửi email HTML
            };

            _smtpClient.Send(mailMessage);
        }
    }
}
