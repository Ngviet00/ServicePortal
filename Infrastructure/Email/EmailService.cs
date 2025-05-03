using System.Net.Mail;
using System.Net;
using ServicePortal.Domain.Entities;
using Serilog;
using ServicePortal.Common.Helpers;
using ServicePortal.Domain.Enums;

namespace ServicePortal.Infrastructure.Email
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailLeaveRequest(string to, LeaveRequest leaveRequest)
        {
            try
            {
                var smtp = _config.GetSection("SmtpSettings");
                using var smtpClient = new SmtpClient(smtp["Host"])
                {
                    Port = int.Parse(smtp["Port"]),
                    Credentials = new NetworkCredential(smtp["Username"], smtp["Password"]),
                    EnableSsl = bool.Parse(smtp["EnableSsl"])
                };

                string content = FormatContentMailLeaveRequest(leaveRequest);

                string subject = $"Đơn xin nghỉ phép - {leaveRequest.Name}";

                var message = new MailMessage
                {
                    From = new MailAddress(smtp["From"]),
                    Subject = subject,
                    Body = content,
                    IsBodyHtml = true
                };

                message.To.Add(to);

                await smtpClient.SendMailAsync(message);
            }
            catch (Exception ex)
            {
                Log.Error($"Error send mail failed, error: {ex.Message}");
            }
        }

        public string FormatContentMailLeaveRequest(LeaveRequest leaveRequest)
        {
            return $@"
                    <h4>
                        <span>Duyệt đơn: </span>
                        <a href=""http://localhost:5173/leave/wait-approval"">http://localhost:5173/leave/wait-approval</a>
                    </h4>
                    <table cellpadding=""10"" cellspacing=""0"" style=""border-collapse: collapse; width: 100%; font-family: Arial, sans-serif; border: 1px solid #ccc;"">
                          <tr>
                            <th colspan=""2"" style=""background-color: #f2f2f2; font-size: 20px; padding: 12px; text-align: center; border-bottom: 2px solid #ccc;"">
                              ĐƠN XIN NGHỈ PHÉP
                            </th>
                          </tr>
                          <tr style=""border-bottom: 1px solid #ddd;"">
                            <td style=""background-color: #f9f9f9;""><strong>Mã nhân viên:</strong></td>
                            <td>{leaveRequest.UserCode}</td>
                          </tr>
                          <tr style=""border-bottom: 1px solid #ddd;"">
                            <td style=""background-color: #f9f9f9;""><strong>Họ tên:</strong></td>
                            <td>{leaveRequest.Name}</td>
                          </tr>
                          <tr style=""border-bottom: 1px solid #ddd;"">
                            <td style=""background-color: #f9f9f9;""><strong>Phòng ban:</strong></td>
                            <td>{leaveRequest.Deparment}</td>
                          </tr>
                          <tr style=""border-bottom: 1px solid #ddd;"">
                            <td style=""background-color: #f9f9f9;""><strong>Chức vụ:</strong></td>
                            <td>{leaveRequest.Position}</td>
                          </tr>
                          <tr style=""border-bottom: 1px solid #ddd;"">
                            <td style=""background-color: #f9f9f9;""><strong>Ngày nghỉ từ:</strong></td>
                            <td>{leaveRequest.FromDate}</td>
                          </tr>
                          <tr style=""border-bottom: 1px solid #ddd;"">
                            <td style=""background-color: #f9f9f9;""><strong>Đến ngày:</strong></td>
                            <td>{leaveRequest.ToDate}</td>
                          </tr>
                          <tr style=""border-bottom: 1px solid #ddd;"">
                            <td style=""background-color: #f9f9f9;""><strong>Loại phép:</strong></td>
                            <td>{Helper.GetDescriptionFromValue<TypeLeaveEnum>((int)leaveRequest.TypeLeave)}</td>
                          </tr>
                          <tr style=""border-bottom: 1px solid #ddd;"">
                            <td style=""background-color: #f9f9f9;""><strong>Thời gian nghỉ:</strong></td>
                            <td>{Helper.GetDescriptionFromValue<TimeLeaveEnum>((int)leaveRequest.TimeLeave)}</td>
                          </tr>
                          <tr style=""border-bottom: 1px solid #ddd;"">
                            <td style=""background-color: #f9f9f9;""><strong>Lý do nghỉ:</strong></td>
                            <td>{leaveRequest.Reason}</td>
                          </tr>
                    </table>";
        }
    }
}
