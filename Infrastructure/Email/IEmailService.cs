using ServicePortal.Domain.Entities;

namespace ServicePortal.Infrastructure.Email
{
    public interface IEmailService
    {
        Task SendEmailLeaveRequest(List<string> listEmail, LeaveRequest request, string UrlFrontEnd);
    }
}
