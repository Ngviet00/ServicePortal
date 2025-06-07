using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicePortal.Applications.Modules.LeaveRequest.DTO;
using ServicePortal.Applications.Modules.LeaveRequest.DTO.Requests;
using ServicePortal.Applications.Modules.LeaveRequest.DTO.Responses;
using ServicePortal.Applications.Modules.LeaveRequest.Services.Interfaces;
using ServicePortal.Common;
using ServicePortal.Common.Filters;

namespace ServicePortal.Applications.Modules.LeaveRequest.Controllers
{
    [ApiController, Route("api/leave-request"), Authorize]
    public class LeaveRequestController : ControllerBase
    {
        private readonly ILeaveRequestService _leaveRequestService;

        public LeaveRequestController(ILeaveRequestService leaveRequestService)
        {
            _leaveRequestService = leaveRequestService;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll(GetAllLeaveRequestDto request)
        {
            var results = await _leaveRequestService.GetAll(request);

            var response = new PageResponse<LeaveRequestDto>(
                200,
                "Success",
                results.Data,
                results.TotalPages,
                request.Page,
                request.PageSize,
                results.TotalItems,
                results.CountPending,
                results.CountInProcess
            );

            return Ok(response);
        }

        [HttpGet("get-leave-request-wait-approval")]
        public async Task<IActionResult> GetWaitApproval(GetAllLeaveRequestWaitApprovalDto request)
        {
            var userClaim = HttpContext.User;

            var results = await _leaveRequestService.GetAllWaitApproval(request, userClaim);

            var response = new PageResponse<LeaveRequestDto>
                (
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

        [HttpGet("get-by-id/{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _leaveRequestService.GetById(id);

            return Ok(new BaseResponse<LeaveRequestDto>(200, "success", result));
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] LeaveRequestDto dto)
        {
            var result = await _leaveRequestService.Create(dto);

            return Ok(new BaseResponse<LeaveRequestDto>(200, "success", result));
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(Guid id, LeaveRequestDto dto)
        {
            var result = await _leaveRequestService.Update(id, dto);

            return Ok(new BaseResponse<LeaveRequestDto>(200, "success", result));
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _leaveRequestService.Delete(id);

            return Ok(new BaseResponse<LeaveRequestDto>(200, "success", result));
        }

        [HttpPost("approval")]
        [RoleAuthorize("leave_request.approval", "HR", "HR_Manager")]
        public async Task<IActionResult> Approval(ApprovalDto request)
        {
            var currentUserCode = User.FindFirst("user_code")?.Value;

            var result = await _leaveRequestService.Approval(request, currentUserCode ?? "");

            return Ok(new BaseResponse<LeaveRequestDto>(200, "success", result));
        }

        [HttpGet("count-wait-approval")]
        public async Task<IActionResult> CountWaitApproval(GetAllLeaveRequestWaitApprovalDto request)
        {
            var userClaim = HttpContext.User;

            var result = await _leaveRequestService.CountWaitApproval(request, userClaim);

            return Ok(new BaseResponse<long>(200, "success", result));
        }

        [HttpPost("hr-register-all-leave-rq")]
        public async Task<IActionResult> HrRegisterAllLeave([FromBody] HrRegisterAllLeaveRqDto request)
        {
            var result = await _leaveRequestService.HrRegisterAllLeave(request);

            return Ok(new BaseResponse<string>(200, "success", result));
        }

        [HttpGet("history-approval"), RoleAuthorize("HR", "HR_Manager")]
        public async Task<IActionResult> GetHistoryLeaveRequestApproval([FromQuery] GetAllLeaveRequestDto request)
        {
            var result = await _leaveRequestService.GetHistoryLeaveRequestApproval(request);

            var response = new PageResponse<HistoryLeaveRequestApprovalResponse>(
                200,
                "Success",
                result.Data,
                result.TotalPages,
                request.Page,
                request.PageSize,
                result.TotalItems
            );

            return Ok(response);
        }
    }
}
