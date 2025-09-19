using System.Text;
using ServicePortals.Domain.Entities;

namespace ServicePortals.Application.Common
{
    public static class TemplateEmail
    {
        public static string SendContentEmail(string title, string url, string code)
        {
            return $@"
                <!DOCTYPE html>
                <html lang=""vi"">
                    <head>
                        <meta charset=""UTF-8"">
                        <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                        <title>Detail</title>
                    </head>
                    <body style=""font-family: Arial, sans-serif;margin: 0; padding: 20px;"">
                        <div>
                            <h3 style=""color: #333333; text-align: left;"">{title}</h2>
                            <p style=""color: #555555; line-height: 1.6; font-size: 14px; font-weight: normal"">
                                Click here to view details:
                                <a href=""{url}"" style=""font-weight: bold; color: #007bff; text-decoration: underline;"">
                                    {code}
                                </a>
                            </p>
                        </div>
                    </body>
                </html>
            ";
        }

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
                            Leave application form
                        </th>
                    </tr>

                    <tr style=""border-bottom: 1px solid #ddd;"">
                        <td style=""background-color: #f9f9f9;""><strong>Usercode:</strong></td>
                        <td>{leaveRequest?.UserCode}</td>
                    </tr>

                    <tr style=""border-bottom: 1px solid #ddd;"">
                        <td style=""background-color: #f9f9f9;""><strong>Username:</strong></td>
                        <td>{leaveRequest?.UserName}</td>
                    </tr>

                    <tr style=""border-bottom: 1px solid #ddd;"">
                        <td style=""background-color: #f9f9f9;""><strong>Department:</strong></td>
                        <td>{leaveRequest?.OrgUnit?.Name}</td>
                    </tr>

                    <tr style=""border-bottom: 1px solid #ddd;"">
                        <td style=""background-color: #f9f9f9;""><strong>Position:</strong></td>
                        <td>{leaveRequest?.Position}</td>
                    </tr>

                    <tr style=""border-bottom: 1px solid #ddd;"">
                        <td style=""background-color: #f9f9f9;""><strong>From date:</strong></td>
                        <td>{leaveRequest?.FromDate}</td>
                    </tr>

                    <tr style=""border-bottom: 1px solid #ddd;"">
                        <td style=""background-color: #f9f9f9;""><strong>To date:</strong></td>
                        <td>{leaveRequest?.ToDate}</td>
                    </tr>

                    <tr style=""border-bottom: 1px solid #ddd;"">
                        <td style=""background-color: #f9f9f9;""><strong>Type leave:</strong></td>
                        <td>{leaveRequest?.TypeLeave?.Name}</td>
                    </tr>

                    <tr style=""border-bottom: 1px solid #ddd;"">
                        <td style=""background-color: #f9f9f9;""><strong>Time:</strong></td>
                        <td>{leaveRequest?.TimeLeave?.Name}</td>
                    </tr>

                    <tr style=""border-bottom: 1px solid #ddd;"">
                        <td style=""background-color: #f9f9f9;""><strong>Reason:</strong></td>
                        <td>{leaveRequest?.Reason}</td>
                    </tr>
                </table>";
        }

        public static string EmailFormIT(ITForm? itForm)
        {
            return $@"
                <table style=""width: 100%; border-collapse: collapse; margin-top: 15px; margin-bottom: 20px; font-family: Arial, sans-serif; font-size: 14px;"">
                    <tr>
                        <th style=""border: 1px solid #cccccc; padding: 8px; background-color: #f2f2f2; text-align: left;"">Information</th>
                        <th style=""border: 1px solid #cccccc; padding: 8px; background-color: #f2f2f2; text-align: left;"">Detail</th>
                    </tr>
                    <tr>
                        <td style=""border: 1px solid #cccccc; padding: 8px;"">UserCode</td>
                        <td style=""border: 1px solid #cccccc; padding: 8px;"">{itForm?.ApplicationFormItem?.ApplicationForm?.UserCodeCreatedBy}</td>
                    </tr>
                    <tr>
                        <td style=""border: 1px solid #cccccc; padding: 8px;"">UserName</td>
                        <td style=""border: 1px solid #cccccc; padding: 8px;"">{itForm?.ApplicationFormItem?.ApplicationForm?.CreatedBy}</td>
                    </tr>
                    <tr>
                        <td style=""border: 1px solid #cccccc; padding: 8px;"">Date request</td>
                        <td style=""border: 1px solid #cccccc; padding: 8px;"">{itForm?.RequestDate?.ToString("yyyy-MM-dd")}</td>
                    </tr>
                    <tr>
                        <td style=""border: 1px solid #cccccc; padding: 8px;"">IT Category</td>
                        <td style=""border: 1px solid #cccccc; padding: 8px;"">{string.Join(", ", itForm != null ? itForm.ItFormCategories.Select(e => e.ITCategory?.Name ?? "").ToList() : [])}</td>
                    </tr>
                    <tr>
                        <td style=""border: 1px solid #cccccc; padding: 8px;"">Reason</td>
                        <td style=""border: 1px solid #cccccc; padding: 8px;"">{itForm?.Reason}</td>
                    </tr>
                    <tr>
                        <td style=""border: 1px solid #cccccc; padding: 8px;"">Priority</td>
                        <td style=""border: 1px solid #cccccc; padding: 8px;"">{itForm?.Priority?.NameE}</td>
                    </tr>
                </table>
            ";
        }

        public static string EmailPurchase(Purchase? purchase)
        {
            StringBuilder sb = new StringBuilder();

            if (purchase != null && purchase.PurchaseDetails != null)
            {
                foreach (var item in purchase.PurchaseDetails)
                {
                    sb.AppendLine($@"<li style=""margin-bottom: 5px;""><strong>Item name:</strong> {item.ItemName} - <strong>Qty:</strong> {item.Quantity} - <strong>Unit:</strong> {item.UnitMeasurement}</li>");
                }
            }

            return $@"
                <body style=""font-family: Arial, sans-serif; margin: 0; padding: 0; color: #333;"">
                    <div>
                        <div style=""padding: 20px 0; line-height: 1.6;"">
                            <div>
                                <ul style=""list-style: none; padding: 0;"">
                                    <li style=""margin-bottom: 5px;""><strong>Code:</strong> {purchase?.ApplicationFormItem?.ApplicationForm?.Code}</li>
                                    <li style=""margin-bottom: 5px;""><strong>UserName:</strong> {purchase?.ApplicationFormItem?.ApplicationForm?.CreatedBy}</li>
                                    <li style=""margin-bottom: 5px;""><strong>Department:</strong> {purchase?.OrgUnit?.Name}</li>
                                    <li style=""margin-bottom: 5px;""><strong>Date Created:</strong> {purchase?.CreatedAt}</li>
                                    <li style=""margin-bottom: 5px;""><strong>Order Summary:</strong>
                                        <ul style=""list-style: none; padding: 0; margin-top: 5px;"">
                                            {sb?.ToString()}
                                        </ul>
                                    </li>
                                </ul>
                            </div>
                        </div>
                        <div style=""text-align: center; padding-top: 20px; margin-top: 20px; border-top: 1px solid #ddd; font-size: 12px; color: #999;"">
                            <p style=""margin: 0 0 10px;"">This is an automated notification. Please do not reply to this email.</p>
                        </div>
                    </div>
                </body>
            ";
        }
    }
}
