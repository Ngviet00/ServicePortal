using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicePortals.Application;
using ServicePortals.Application.Dtos.Approval.Request;
using ServicePortals.Application.Dtos.Approval.Response;
using ServicePortals.Application.Interfaces.Approval;
using ServicePortals.Domain.Enums;

namespace ServicePortal.Controllers.Approval
{
    [ApiController, Route("api/approval")]
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

        [HttpPost("approval")]
        public async Task<IActionResult> Approval([FromBody] ApprovalRequest request)
        {
            if (!HasApprovalRolePermission(request))
            {
                return Forbid("Bạn không có quyền duyệt loại đơn này");
            }

            var result = await _approvalService.Approval(request);
            return Ok(new BaseResponse<object>(200, "success", result));
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
        public async Task<IActionResult> ListAssigned()
        {
            return Ok();
        }

        [HttpGet("list-history-approval-or-processed"), AllowAnonymous]
        public async Task<IActionResult> ListHistoryApprovalOrProcessed([FromQuery] ListHistoryApprovalProcessedRequest request)
        {
            var results = await _approvalService.ListHistoryApprovedOrProcessed(request);

            var response = new PageResponse<HistoryApprovalProcessResponse>(
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

        private bool HasApprovalRolePermission(ApprovalRequest request)
        {
            var userClaims = HttpContext.User;
            var roleClaims = userClaims?.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToHashSet(StringComparer.OrdinalIgnoreCase);
            var permissionClaims = userClaims?.Claims.Where(c => c.Type == "permission").Select(c => c.Value).ToHashSet(StringComparer.OrdinalIgnoreCase);

            if (request.RequestTypeId == (int)RequestTypeEnum.CREATE_MEMO_NOTIFICATION)
            {
                bool checkCanApprovalMemo = roleClaims?
                    .Any(r => new[] { "HR", "IT", "union" }
                    .Contains(r, StringComparer.OrdinalIgnoreCase)) == true || permissionClaims?.Contains("memo_notification.create") == true;

                return checkCanApprovalMemo;
            }

            return true;
        }
    }
}
