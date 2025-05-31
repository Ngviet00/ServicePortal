using ServicePortal.Domain.Entities;
using ServicePortal.Modules.TimeKeeping.DTO.Requests;

namespace ServicePortal.Infrastructure.Email
{
    public interface IEmailService
    {
        //email when approved or rejected
        Task SendEmaiLeaveRequestMySelfStatus(string email, LeaveRequest request, string UrlFrontEnd, string? comment, bool status);

        //email notify have been sent
        Task SendEmaiLeaveRequestMySelf(string email, LeaveRequest request, string UrlFrontEnd);

        //email send to next user approve leave request
        Task SendEmailLeaveRequest(List<string> listEmail, LeaveRequest request, string UrlFrontEnd);

        //email reset password
        Task SendEmailResetPassword(string email, string password);

        //email send from user confirm timekeeping of user to HR
        Task SendEmailConfirmTimeKeepingToHr(string email, byte[] fileBytes, GetManagementTimeKeepingDto request);
    }
}
