using System.Security.Claims;
using DocumentFormat.OpenXml.Office2021.DocumentTasks;
using ServicePortals.Application.Dtos.Approval.Request;
using ServicePortals.Application.Dtos.Approval.Response;
using ServicePortals.Application.Dtos.LeaveRequest.Requests;
using ServicePortals.Application.Interfaces.Approval;
using ServicePortals.Application.Interfaces.LeaveRequest;
using ServicePortals.Application.Interfaces.MemoNotification;
using ServicePortals.Infrastructure.Data;

namespace ServicePortals.Application.Services.Approval
{
    public class ApprovalService : IApprovalService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILeaveRequestService _leaveRequestService;
        private readonly IMemoNotificationService _memoNotificationService;
        
        public ApprovalService
        (
            ApplicationDbContext context,
            ILeaveRequestService leaveRequestService,
            IMemoNotificationService memoNotificationService
        )
        {
            _context = context;
            _leaveRequestService = leaveRequestService;
            _memoNotificationService = memoNotificationService;
        }

        public async Task<PagedResults<PendingApproval>> ListWaitApprovals(ListWaitApprovalRequest request, ClaimsPrincipal userClaims)
        {
            int page = request.Page;
            int pageSize = request.PageSize;

            var tasks = new List<Task<PendingApprovalList>>
            {
                _memoNotificationService.WaitApproval(request, userClaims)
            };

            var results = await System.Threading.Tasks.Task.WhenAll(tasks);

            var allData = results.SelectMany(r => r.Data).ToList();
            var totalCount = results.Sum(r => r.TotalCount);

            var finalResults = new PagedResults<PendingApproval>
            {
                Data = allData,
                TotalItems = totalCount,
                TotalPages = 0
            };

            return finalResults;
        }

        public Task<object> Approval()
        {
            throw new NotImplementedException();
        }

        public async Task<CountWaitApprovalAndAssignedInSidebarResponse> CountWaitAprrovalAndAssignedInSidebar
        (
            CountWaitAprrovalAndAssignedInSidebarRequest request,
            ClaimsPrincipal userClaims
        )
        {
            CountWaitApprovalAndAssignedInSidebarResponse results = new CountWaitApprovalAndAssignedInSidebarResponse();

            var countWaitNoti = await _memoNotificationService.CountWaitApprovalMemoNotification(request?.OrgUnitId ?? -9999);
            var countWaitLeaveRq = await _leaveRequestService.CountWaitApproval(new GetAllLeaveRequestWaitApprovalRequest
            {
                UserCode = request?.UserCode,
                OrgUnitId = request?.OrgUnitId
            }, userClaims);


            results.TotalWaitApproval = countWaitNoti + countWaitLeaveRq;
            results.TotalAssigned = 0;

            return results;
        }

        public Task<object> ListAssigned()
        {
            throw new NotImplementedException();
        }

        public Task<object> ListHistoryApprovedOrProcessed()
        {
            throw new NotImplementedException();
        }
    }
}
