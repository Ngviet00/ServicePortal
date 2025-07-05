using ServicePortals.Domain.Entities;

namespace ServicePortals.Application.Common
{
    public static class TemplateEmail
    {
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

        public static string EmailContentLeaveRequest(LeaveRequest? leaveRequest, TypeLeave? typeLeave, TimeLeave? timeLeave)
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
                        <td>{leaveRequest?.Name}</td>
                    </tr>

                    <tr style=""border-bottom: 1px solid #ddd;"">
                        <td style=""background-color: #f9f9f9;""><strong>Mã nhân viên:</strong></td>
                        <td>{leaveRequest?.RequesterUserCode}</td>
                    </tr>

                    <tr style=""border-bottom: 1px solid #ddd;"">
                        <td style=""background-color: #f9f9f9;""><strong>Phòng ban:</strong></td>
                        <td>{leaveRequest?.Department}</td>
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
                        <td>{typeLeave?.Name}</td>
                    </tr>

                    <tr style=""border-bottom: 1px solid #ddd;"">
                        <td style=""background-color: #f9f9f9;""><strong>Thời gian nghỉ:</strong></td>
                        <td>{timeLeave?.Description}</td>
                    </tr>

                    <tr style=""border-bottom: 1px solid #ddd;"">
                        <td style=""background-color: #f9f9f9;""><strong>Lý do nghỉ:</strong></td>
                        <td>{leaveRequest?.Reason}</td>
                    </tr>
                </table>";
        }
    }
}
