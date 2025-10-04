using ServicePortals.Application.Dtos.Approval.Request;
using ServicePortals.Application.Dtos.Approval.Response;

namespace ServicePortals.Application.Interfaces.Approval
{
    public interface IApprovalService
    {
        Task<CountWaitApprovalAndAssignedInSidebarResponse> CountWaitAprrovalAndAssignedInSidebar(CountWaitAprrovalAndAssignedInSidebarRequest request);
        Task<PagedResults<PendingApproval>> ListWaitApprovals(ListWaitApprovalRequest request);
        Task<PagedResults<PendingApproval>> ListAssigned(ListAssignedTaskRequest request);
        Task<PagedResults<HistoryApprovalResponse>> ListHistoryApprovedOrProcessed(ListHistoryApprovalRequest request);
    }
}
