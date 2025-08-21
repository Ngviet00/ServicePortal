using ServicePortals.Domain.Entities;

namespace ServicePortals.Application.Common
{
    public static class TemplateEmail
    {
        #region Email Memo Notification
        public static string EmailSendMemoNotificationNeedApproval(string urlFrontEnd)
        {
            var urlWaitApprovalMemo = $"{urlFrontEnd}/memo-notify/wait-approval";

            return $@"<!DOCTYPE html>
                <html>
                      <head>
                            <meta charset=""UTF-8"" />
                            <title>Approval Needed</title>
                      </head>
                      <body style=""margin:0; padding:0; font-family:Arial, sans-serif; background-color:#f4f4f4;"">
                        <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""padding: 20px 0;"">
                          <tr>
                            <td align=""center"">
                              <table width=""600"" cellpadding=""0"" cellspacing=""0"" style=""background-color:#ffffff; border-radius:8px; box-shadow:0 0 5px rgba(0,0,0,0.05); overflow:hidden;"">
                                <tr>
                                  <td style=""background-color:#0051dd; color:#ffffff; padding:16px; text-align:center; font-size:20px;"">
                                    📢 New Approval Request
                                  </td>
                                </tr>
                                <tr>
                                  <td style=""padding:24px 30px; font-size:15px; color:#333;"">
                                    There's a new request waiting for your approval.
                                    <br /><br />
                                    Please check the system to review and take action.
                                  </td>
                                </tr>
                                <tr>
                                  <td style=""padding: 0 30px 30px 30px; text-align: center;"">
                                    <a href=""{urlWaitApprovalMemo}""
                                      style=""color:#0073e6; text-decoration:none; padding:10px 20px; border-radius:4px; font-weight:500; display:inline-block;text-decoration: underline;text-decoration-offset: 3px;"">
                                      Open Request
                                    </a>
                                  </td>
                                </tr>
                                <tr>
                                  <td style=""background-color:#f9f9f9; text-align:center; color:#888; font-size:12px; padding:16px;"">
                                    This email was sent automatically. No reply is needed.
                                  </td>
                                </tr>
                              </table>
                            </td>
                          </tr>
                        </table>
                      </body>
                </html>
            ";
        }

        public static string EmailSendMemoNotificationHasBeenCompletedOrReject(string urlFrontEnd, bool rejectOrApproved) //true approval, false reject
        {
            var urlWaitApprovalMemo = $"{urlFrontEnd}/memo-notify";
            string status = rejectOrApproved ? "Approved" : "Reject";

            return $@"<!DOCTYPE html>
                <html>
                    <head>
                    <meta charset=""UTF-8"" />
                    <title>Request Approved</title>
                    </head>
                    <body style=""margin:0; padding:0; font-family:Arial, sans-serif; background-color:#f4f4f4;"">
                    <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""padding: 20px 0;"">
                        <tr>
                        <td align=""center"">
                            <table width=""600"" cellpadding=""0"" cellspacing=""0"" style=""background-color:#ffffff; border-radius:8px; box-shadow:0 0 5px rgba(0,0,0,0.05); overflow:hidden;"">
                            <tr>
                                <td style=""background-color:#0051dd; color:#ffffff; padding:16px; text-align:center; font-size:20px;"">
                                    📢 Request {status}
                                </td>
                            </tr>
                            <tr>
                                <td style=""padding:24px 30px; font-size:15px; color:#333;"">
                                Your request create notification has been <strong>{status}</strong>.
                                <br /><br />
                                You can view the approved details in the system.
                                </td>
                            </tr>
                            <tr>
                                <td style=""padding: 0 30px 30px 30px; text-align: center;"">
                                <a href=""{urlWaitApprovalMemo}""
                                    style=""color:#0051dd; text-decoration:none; padding:10px 20px; border-radius:4px; font-weight:500; display:inline-block;text-decoration: underline"">
                                    View Details
                                </a>
                                </td>
                            </tr>
                            <tr>
                                <td style=""background-color:#f9f9f9; text-align:center; color:#888; font-size:12px; padding:16px;"">
                                This is an automated message from the system. No reply is needed.
                                </td>
                            </tr>
                            </table>
                        </td>
                        </tr>
                    </table>
                    </body>
                </html>
            ";
        }

        #endregion

        public static string EmailResetPassword(string password)
        {
            return $@"
                <h2>Your Password Has Been Reset</h2>
                <div style=""font-size: 18px;"">
                    An administrator has reset your password. You can now log in using the password below: <br/>
                </div>
                <div style=""font-size: 25px;margin-top: 10px; color: #e71a1a;letter-spacing: 1px"">
                    {password}
                </div>
                <div style=""font-size: 18px;margin-top: 10px;"">
                    For security reasons, please change your password after logging in. <br/> <br/>
                    Thanks, <br/><br/>
                    MIS/IT Team
                </div>
            ";
        }

        public static string EmailContentLeaveRequest(LeaveRequest? leaveRequest)
        {
            return $@"
                <table cellpadding=""10"" cellspacing=""0"" style=""border-collapse: collapse; width: 100%; font-family: Arial, sans-serif; border: 1px solid #ccc;"">
                    <tr>
                        <th colspan=""2"" style=""background-color: #f2f2f2; font-size: 20px; padding: 12px; text-align: center; border-bottom: 2px solid #ccc;"">
                            ĐƠN XIN NGHỈ PHÉP
                        </th>
                    </tr>

                    <tr style=""border-bottom: 1px solid #ddd;"">
                        <td style=""background-color: #f9f9f9;""><strong>Tên nhân viên:</strong></td>
                        <td>{leaveRequest?.UserNameRequestor}</td>
                    </tr>

                    <tr style=""border-bottom: 1px solid #ddd;"">
                        <td style=""background-color: #f9f9f9;""><strong>Mã nhân viên:</strong></td>
                        <td>{leaveRequest?.UserCodeRequestor}</td>
                    </tr>

                    <tr style=""border-bottom: 1px solid #ddd;"">
                        <td style=""background-color: #f9f9f9;""><strong>Phòng ban:</strong></td>
                        <td>{leaveRequest?.OrgUnit?.Name}</td>
                    </tr>

                    <tr style=""border-bottom: 1px solid #ddd;"">
                        <td style=""background-color: #f9f9f9;""><strong>Chức vụ:</strong></td>
                        <td>{leaveRequest?.Position}</td>
                    </tr>

                    <tr style=""border-bottom: 1px solid #ddd;"">
                        <td style=""background-color: #f9f9f9;""><strong>Ngày nghỉ từ:</strong></td>
                        <td>{leaveRequest?.FromDate}</td>
                    </tr>

                    <tr style=""border-bottom: 1px solid #ddd;"">
                        <td style=""background-color: #f9f9f9;""><strong>Đến ngày:</strong></td>
                        <td>{leaveRequest?.ToDate}</td>
                    </tr>

                    <tr style=""border-bottom: 1px solid #ddd;"">
                        <td style=""background-color: #f9f9f9;""><strong>Loại phép:</strong></td>
                        <td>{leaveRequest?.TypeLeave?.Name}</td>
                    </tr>

                    <tr style=""border-bottom: 1px solid #ddd;"">
                        <td style=""background-color: #f9f9f9;""><strong>Thời gian nghỉ:</strong></td>
                        <td>{leaveRequest?.TimeLeave?.Name}</td>
                    </tr>

                    <tr style=""border-bottom: 1px solid #ddd;"">
                        <td style=""background-color: #f9f9f9;""><strong>Lý do nghỉ:</strong></td>
                        <td>{leaveRequest?.Reason}</td>
                    </tr>
                </table>";
        }

        public static string EmailFormIT()
        {
            return $@"
                <table style=""width: 100%; border-collapse: collapse; margin-top: 15px; margin-bottom: 20px; font-family: Arial, sans-serif; font-size: 14px;"">
                    <tr>
                        <th style=""border: 1px solid #cccccc; padding: 8px; background-color: #f2f2f2; text-align: left;"">Thông tin</th>
                        <th style=""border: 1px solid #cccccc; padding: 8px; background-color: #f2f2f2; text-align: left;"">Chi tiết</th>
                    </tr>
                    <tr>
                        <td style=""border: 1px solid #cccccc; padding: 8px;"">Ngày yêu cầu</td>
                        <td style=""border: 1px solid #cccccc; padding: 8px;"">{{Ngày Yêu Cầu}}</td>
                    </tr>
                    <tr>
                        <td style=""border: 1px solid #cccccc; padding: 8px;"">Ngày hoàn thành dự kiến</td>
                        <td style=""border: 1px solid #cccccc; padding: 8px;"">{{Ngày Hoàn Thành}}</td>
                    </tr>
                    <tr>
                        <td style=""border: 1px solid #cccccc; padding: 8px;"">Loại IT</td>
                        <td style=""border: 1px solid #cccccc; padding: 8px;"">{{Danh sách IT Category}}</td>
                    </tr>
                    <tr>
                        <td style=""border: 1px solid #cccccc; padding: 8px;"">Mức ưu tiên</td>
                        <td style=""border: 1px solid #cccccc; padding: 8px;"">{{Mức ưu tiên}}</td>
                    </tr>
                    <tr>
                        <td style=""border: 1px solid #cccccc; padding: 8px;"">Lý do</td>
                        <td style=""border: 1px solid #cccccc; padding: 8px;"">{{Lý do chi tiết}}</td>
                    </tr>
                </table>
            ";
        }
    }
}
