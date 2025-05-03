using ServicePortal.Domain.Entities;

namespace ServicePortal.Infrastructure.Email
{
    public interface IEmailService
    {
        Task SendEmailLeaveRequest(string to, LeaveRequest request);
    }
}
