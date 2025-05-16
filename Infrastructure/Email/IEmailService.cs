using ServicePortal.Domain.Entities;

namespace ServicePortal.Infrastructure.Email
{
    public interface IEmailService
    {
        Task SendEmaiLeaveRequestMySelfStatus(string email, LeaveRequest request, string UrlFrontEnd, string? comment, bool status);
        Task SendEmaiLeaveRequestMySelf(string email, LeaveRequest request, string UrlFrontEnd);
        Task SendEmailLeaveRequest(List<string> listEmail, LeaveRequest request, string UrlFrontEnd);
    }
}
