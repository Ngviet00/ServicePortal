using System.Security.Claims;
using ServicePortals.Application.Dtos.Approval.Request;
using ServicePortals.Application.Dtos.Approval.Response;

namespace ServicePortals.Application.Interfaces.Approval
{
    public interface IApprovalService
    {
        Task<object> Approval();
        Task<CountWaitApprovalAndAssignedInSidebarResponse> CountWaitAprrovalAndAssignedInSidebar(CountWaitAprrovalAndAssignedInSidebarRequest request, ClaimsPrincipal userClaims);
        Task<PagedResults<PendingApproval>> ListWaitApprovals(ListWaitApprovalRequest request, ClaimsPrincipal userClaims);
        Task<object> ListAssigned();
        Task<object> ListHistoryApprovedOrProcessed();
    }
}
