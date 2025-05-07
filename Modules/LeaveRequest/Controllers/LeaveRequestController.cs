using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicePortal.Common;
using ServicePortal.Infrastructure.Data;
using ServicePortal.Modules.LeaveRequest.DTO;
using ServicePortal.Modules.LeaveRequest.Interfaces;
using ServicePortal.Modules.LeaveRequest.Requests;

namespace ServicePortal.Modules.LeaveRequest.Controllers
{
    [ApiController, Route("api/leave-request"), Authorize]
    public class LeaveRequestController : ControllerBase
    {
        private readonly ILeaveRequestService _leaveRequestService;

        private readonly ApplicationDbContext _context;

        public LeaveRequestController(ILeaveRequestService leaveRequestService, ApplicationDbContext context)
        {
            _leaveRequestService = leaveRequestService;
            _context = context;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll(GetAllLeaveRequest request)
        {
            var results = await _leaveRequestService.GetAll(request);

            var response = new PageResponse<LeaveRequestDTO>(200, "Success", results.Data, results.TotalPages, request.Page, request.PageSize, results.TotalItems, results.CountPending, results.CountInProcess);

            return Ok(response);
        }

        [HttpGet("get-leave-request-wait-approval")]
        public async Task<IActionResult> GetWaitApproval(GetAllLeaveRequestWaitApproval request)
        {
            var results = await _leaveRequestService.GetAllWaitApproval(request);

            var response = new PageResponse<LeaveRequestDTO>(200, "Success", results.Data, results.TotalPages, request.Page, request.PageSize, results.TotalItems);

            return Ok(response);
        }


        [HttpGet("get-by-id/{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _leaveRequestService.GetById(id);

            return Ok(new BaseResponse<LeaveRequestDTO>(200, "success", result));
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create(LeaveRequestDTO dto)
        {
            var result = await _leaveRequestService.Create(dto);

            return Ok(new BaseResponse<LeaveRequestDTO>(200, "success", result));
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(Guid id, LeaveRequestDTO dto)
        {
            var result = await _leaveRequestService.Update(id, dto);

            return Ok(new BaseResponse<LeaveRequestDTO>(200, "success", result));
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _leaveRequestService.Delete(id);

            return Ok(new BaseResponse<LeaveRequestDTO>(200, "success", result));
        }

        [HttpPost("approval")]
        public async Task<IActionResult> Approval(ApprovalDTO request)
        {
            var currentUserCode = User.FindFirst("user_code")?.Value;
            
            var result = await _leaveRequestService.Approval(request, currentUserCode ?? "");

            return Ok(new BaseResponse<LeaveRequestDTO>(200, "success", result));
        }

        [HttpGet("count-wait-approval")]
        public async Task<IActionResult> CountWaitApproval(GetAllLeaveRequestWaitApproval request)
        {
            var result = await _leaveRequestService.CountWaitApproval(request);

            return Ok(new BaseResponse<long>(200, "success", result));
        }

        [HttpGet("test"), AllowAnonymous]
        public async Task<IActionResult> Test()
        {
            return Ok(new BaseResponse<object>(200, "success", null));
        }
    }
}
