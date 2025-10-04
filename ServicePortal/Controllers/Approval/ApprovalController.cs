using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicePortals.Application;
using ServicePortals.Application.Dtos.Approval.Request;
using ServicePortals.Application.Dtos.Approval.Response;
using ServicePortals.Application.Interfaces.Approval;

namespace ServicePortal.Controllers.Approval
{
    [ApiController, Route("api/approval"), Authorize]
    public class ApprovalController : ControllerBase
    {
        private readonly IApprovalService _approvalService;
        public ApprovalController(IApprovalService approvalService)
        {
            _approvalService = approvalService;
        }

        [HttpGet("count-wait-approval-and-assigned-in-sidebar")]
        public async Task<IActionResult> CountWaitAprrovalAndAssignedInSidebar([FromQuery] CountWaitAprrovalAndAssignedInSidebarRequest request)
        {
            var results = await _approvalService.CountWaitAprrovalAndAssignedInSidebar(request);
            
            return Ok(new BaseResponse<CountWaitApprovalAndAssignedInSidebarResponse>(200, "success", results));
        }

        [HttpGet("list-wait-approvals")]
        public async Task<IActionResult> ListWaitApprovals([FromQuery] ListWaitApprovalRequest request)
        {
            var results = await _approvalService.ListWaitApprovals(request);

            var response = new PageResponse<PendingApproval>(
                200,
                "Success",
                results.Data,
                results.TotalPages,
                request.Page,
                request.PageSize,
                results.TotalItems
            );

            return Ok(response);
        }

        [HttpGet("list-assigned")]
        public async Task<IActionResult> ListAssigned([FromQuery] ListAssignedTaskRequest request)
        {
            var results = await _approvalService.ListAssigned(request);

            var response = new PageResponse<PendingApproval>(
                200,
                "Success",
                results.Data,
                results.TotalPages,
                request.Page,
                request.PageSize,
                results.TotalItems
            );

            return Ok(response);
        }

        [HttpGet("list-history-approval")]
        public async Task<IActionResult> ListHistoryApprovalOrProcessed([FromQuery] ListHistoryApprovalRequest request)
        {
            var results = await _approvalService.ListHistoryApprovedOrProcessed(request);

            var response = new PageResponse<HistoryApprovalResponse>(
                200,
                "Success",
                results.Data,
                results.TotalPages,
                request.Page,
                request.PageSize,
                results.TotalItems
            );

            return Ok(response);
        }
    }
}
