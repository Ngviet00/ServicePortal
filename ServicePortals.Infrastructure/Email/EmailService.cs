using System.Net.Mail;
using System.Net;
using Serilog;
using Hangfire;
using System.Net.Mime;
using ServicePortals.Domain.Entities;
using Microsoft.Extensions.Configuration;
using ServicePortals.Infrastructure.Helpers;
using ServicePortals.Domain.Enums;

//Task SendEmailAsync(string to, string subject, string body, bool isHtml = true);
//Task SendEmailWithCcAsync(string to, List<string> cc, string subject, string body, bool isHtml = true);

////email when complete or rejected
//Task SendEmaiLeaveRequestMySelfStatus(string email, LeaveRequest request, string? UrlFrontEnd, string? comment, bool status);

////email notify have been sent
//Task SendEmaiLeaveRequestMySelf(string email, LeaveRequest request, string UrlFrontEnd);

////email send to next user approve leave request
//Task SendEmailLeaveRequest(List<string>? toEmail, List<string>? ccEmail, List<LeaveRequest>? request, string? UrlFrontEnd);

////email reset password
//Task SendEmailResetPassword(string email, string password);

////email send from user confirm timekeeping of user to HR
//Task SendEmailConfirmTimeKeepingToHr(byte[] fileBytes, GetManagementTimeKeepingRequest request);
namespace ServicePortals.Infrastructure.Email
{
    //public class EmailService : IEmailService
    //{
    //    private readonly IConfiguration _config;
    //    private readonly IHRManagementService _hrManagementService;

    //    public EmailService(IConfiguration config, IHRManagementService hrManagementService)
    //    {
    //        _config = config;
    //        _hrManagementService = hrManagementService;
    //    }

    //    public (IConfigurationSection smtpSection, SmtpClient smtpClient) GetEmailConfig()
    //    {
    //        var smtp = _config.GetSection("SmtpSettings");

    //        var smtpClient = new SmtpClient(smtp["Host"])
    //        {
    //            Port = int.Parse(smtp["Port"] ?? ""),
    //            Credentials = new NetworkCredential(smtp["Username"], smtp["Password"]),
    //            EnableSsl = bool.Parse(smtp["EnableSsl"] ?? "")
    //        };

    //        return (smtp, smtpClient);
    //    }

    //    public MailMessage SetMessageEmail(IConfigurationSection smtp, string? subject, string? content, bool? IsBodyHtml = true)
    //    {
    //        var message = new MailMessage
    //        {
    //            From = new MailAddress(smtp["From"] ?? ""),
    //            Subject = subject,
    //            Body = content,
    //            IsBodyHtml = IsBodyHtml ?? true
    //        };

    //        return message;
    //    }

    //    [AutomaticRetry(Attempts = 10)]
    //    public async Task SendEmaiLeaveRequestMySelf(string email, LeaveRequest leaveRequest, string? UrlFrontEnd)
    //    {
    //        try
    //        {
    //            var (smtp, smtpClient) = GetEmailConfig();

    //            string subject = $"Đơn xin nghỉ phép của bạn đã được gửi!";

    //            string content = FormatContentMailLeaveRequest(leaveRequest);

    //            var message = SetMessageEmail(smtp, subject, content);

    //            message.To.Add(email.Trim());

    //            await smtpClient.SendMailAsync(message);
    //        }
    //        catch (Exception ex)
    //        {
    //            Log.Error($"Error send mail your self failed, error: {ex.Message}");
    //        }
    //    }

    //    [AutomaticRetry(Attempts = 10)]
    //    public async Task SendEmaiLeaveRequestMySelfStatus(string email, LeaveRequest leaveRequest, string? UrlFrontEnd, string? comment, bool status)
    //    {
    //        try
    //        {
    //            var (smtp, smtpClient) = GetEmailConfig();

    //            string subject = "";
    //            string content = "";

    //            if (status)
    //            {
    //                subject = $"Đơn xin nghỉ phép của bạn đã đăng ký thành công!";
    //                content = FormatContentMailLeaveRequest(leaveRequest);
    //            }
    //            else
    //            {
    //                subject = $"Đơn xin nghỉ phép của bạn đã bị từ chối!";

    //                content = $@"<h4>
    //                    <span style=""color:red"">Lý do từ chối: {comment}</span>
    //                </h4>"
    //                + FormatContentMailLeaveRequest(leaveRequest);
    //            }

    //            var message = SetMessageEmail(smtp, subject, content);

    //            message.To.Add(email.Trim());

    //            await smtpClient.SendMailAsync(message);
    //        }
    //        catch (Exception ex)
    //        {
    //            Log.Error($"Error send mail your self failed, error: {ex.Message}");
    //        }
    //    }

    //    [AutomaticRetry(Attempts = 10)]
    //    public async Task SendEmailLeaveRequest(List<string>? toEmails, List<string>? ccEmails, List<LeaveRequest>? leaveRequest, string? UrlFrontEnd)
    //    {
    //        try
    //        {
    //            var (smtp, smtpClient) = GetEmailConfig();

    //            string? requester = leaveRequest?.Count == 1 ? leaveRequest?.FirstOrDefault()?.RequesterUserCode : $"{leaveRequest?.Count} người";

    //            string subject = $"Đơn xin nghỉ phép - {requester}";
    //            string urlWaitApproval = $"{UrlFrontEnd}/leave/wait-approval";

    //            string content = $@"
    //                <h4>
    //                    <span>Duyệt đơn: </span>
    //                    <a href={urlWaitApproval}>{urlWaitApproval}</a>
    //                </h4>";

    //            if (leaveRequest != null && leaveRequest.Count > 0)
    //            {
    //                foreach (var request in leaveRequest)
    //                {
    //                    content += FormatContentMailLeaveRequest(request) + "<br/>";
    //                }
    //            }

    //            var message = SetMessageEmail(smtp, subject, content);

    //            if (toEmails != null && toEmails.Count > 0)
    //            {
    //                foreach (var email in toEmails)
    //                {
    //                    if (!string.IsNullOrWhiteSpace(email))
    //                    {
    //                        message.To.Add(email.Trim());
    //                    }
    //                }
    //            }

    //            if (ccEmails != null && ccEmails.Count > 0)
    //            {
    //                foreach (var email in ccEmails)
    //                {
    //                    if (!string.IsNullOrWhiteSpace(email))
    //                    {
    //                        message.CC.Add(email.Trim());
    //                    }
    //                }
    //            }

    //            await smtpClient.SendMailAsync(message);
    //        }
    //        catch (Exception ex)
    //        {
    //            Log.Error($"Error send mail failed, error: {ex.Message}");
    //        }
    //    }

    //    [AutomaticRetry(Attempts = 10)]
    //    public async Task SendEmailResetPassword(string email, string password)
    //    {
    //        try
    //        {
    //            var (smtp, smtpClient) = GetEmailConfig();

    //            string subject = $"Reset password";

    //            string content = $@"
    //                        <h2>Your Password Has Been Reset</h2>
    //                        <div style=""font-size: 18px;"">
	   //                         An administrator has reset your password. You can now log in using the password below: <br/>
    //                        </div>
    //                        <div style=""font-size: 25px;margin-top: 10px; color: #e71a1a;font-family: monospace;letter-spacing: 1px"">
	   //                         {password}
    //                        </div>
    //                        <div style=""font-size: 18px;margin-top: 10px;"">
	   //                         For security reasons, please change your password after logging in. <br/> <br/>
    //                            Thanks, <br/><br/>
	   //                         MIS/IT Team
    //                        </div>";

    //            var message = SetMessageEmail(smtp, subject, content);

    //            message.To.Add(email.Trim());

    //            await smtpClient.SendMailAsync(message);
    //        }
    //        catch (Exception ex)
    //        {
    //            Log.Error($"Error send mail failed, error: {ex.Message}");
    //        }
    //    }

    //    [AutomaticRetry(Attempts = 10)]
    //    public async Task SendEmailConfirmTimeKeepingToHr(byte[] fileBytes, GetManagementTimeKeepingRequest request)
    //    {
    //        try
    //        {
    //            var (smtp, smtpClient) = GetEmailConfig();

    //            string subject = $"Production - Confirm Attendance List [{request.Month} - {request.Year}]";

    //            string content = $@"Dear HR Team, Please find attached the excel file containing the staff attendance list [{request.Month} - {request.Year}]";

    //            var message = SetMessageEmail(smtp, subject, content, false);

    //            string fileName = $"Production_Confirm_Attendance_T{request.Month} - {request.Year}.xlsx";

    //            var attachment = new Attachment(new MemoryStream(fileBytes), fileName, MediaTypeNames.Application.Octet);

    //            message.Attachments.Add(attachment);

    //            var hrMngTimekeeping = await _hrManagementService.GetHrManagementsByType("MANAGE_TIMEKEEPING");

    //            foreach (var hr in hrMngTimekeeping)
    //            {
    //                message.To.Add(Global.EmailDefault);
    //            }

    //            message.CC.Add(request?.EmailSender?.Trim() ?? Global.EmailDefault);

    //            await smtpClient.SendMailAsync(message);

    //        }
    //        catch (Exception ex)
    //        {
    //            Log.Error($"Error can not send email confirm timekeeping to HR, error: {ex.Message}");
    //        }
    //    }

    //    public string FormatContentMailLeaveRequest(LeaveRequest leaveRequest)
    //    {
    //        string typeLeaveDescription = leaveRequest.TypeLeave != null
    //            ? Helper.GetDescriptionFromValue<TypeLeaveEnum>((int)leaveRequest.TypeLeave)
    //            : "";

    //        string timeLeaveDescription = leaveRequest.TimeLeave != null
    //            ? Helper.GetDescriptionFromValue<TimeLeaveEnum>((int)leaveRequest.TimeLeave)
    //            : "";

    //        return $@"
    //                <table cellpadding=""10"" cellspacing=""0"" style=""border-collapse: collapse; width: 100%; font-family: Arial, sans-serif; border: 1px solid #ccc;"">
    //                    <tr>
    //                        <th colspan=""2"" style=""background-color: #f2f2f2; font-size: 20px; padding: 12px; text-align: center; border-bottom: 2px solid #ccc;"">
    //                            ĐƠN XIN NGHỈ PHÉP
    //                        </th>
    //                    </tr>

    //                    <tr style=""border-bottom: 1px solid #ddd;"">
    //                        <td style=""background-color: #f9f9f9;""><strong>Tên nhân viên:</strong></td>
    //                        <td>{leaveRequest.Name}</td>
    //                    </tr>

    //                    <tr style=""border-bottom: 1px solid #ddd;"">
    //                        <td style=""background-color: #f9f9f9;""><strong>Mã nhân viên:</strong></td>
    //                        <td>{leaveRequest.RequesterUserCode}</td>
    //                    </tr>

    //                    <tr style=""border-bottom: 1px solid #ddd;"">
    //                        <td style=""background-color: #f9f9f9;""><strong>Phòng ban:</strong></td>
    //                        <td>{leaveRequest.Department}</td>
    //                    </tr>

    //                    <tr style=""border-bottom: 1px solid #ddd;"">
    //                        <td style=""background-color: #f9f9f9;""><strong>Chức vụ:</strong></td>
    //                        <td>{leaveRequest.Position}</td>
    //                    </tr>

    //                    <tr style=""border-bottom: 1px solid #ddd;"">
    //                        <td style=""background-color: #f9f9f9;""><strong>Ngày nghỉ từ:</strong></td>
    //                        <td>{leaveRequest.FromDate}</td>
    //                    </tr>

    //                    <tr style=""border-bottom: 1px solid #ddd;"">
    //                        <td style=""background-color: #f9f9f9;""><strong>Đến ngày:</strong></td>
    //                        <td>{leaveRequest.ToDate}</td>
    //                    </tr>

    //                    <tr style=""border-bottom: 1px solid #ddd;"">
    //                        <td style=""background-color: #f9f9f9;""><strong>Loại phép:</strong></td>
    //                        <td>{typeLeaveDescription}</td>
    //                    </tr>

    //                    <tr style=""border-bottom: 1px solid #ddd;"">
    //                        <td style=""background-color: #f9f9f9;""><strong>Thời gian nghỉ:</strong></td>
    //                        <td>{timeLeaveDescription}</td>
    //                    </tr>

    //                    <tr style=""border-bottom: 1px solid #ddd;"">
    //                        <td style=""background-color: #f9f9f9;""><strong>Lý do nghỉ:</strong></td>
    //                        <td>{leaveRequest.Reason}</td>
    //                    </tr>
    //                </table>";
    //    }
    //}
}