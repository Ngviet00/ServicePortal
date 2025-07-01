using ServicePortals.Domain.Entities;

namespace ServicePortals.Application.Dtos.LeaveRequest.Responses
{
    public class LeaveRequestWithApprovalResponse
    {
        public required Domain.Entities.LeaveRequest LeaveRequest { get; set; }
        public required ApplicationForm ApprovalRequest { get; set; }
    }
}
