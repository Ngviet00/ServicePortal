using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicePortal.Common;
using ServicePortal.Modules.LeaveRequest.DTO;
using ServicePortal.Modules.LeaveRequest.Interfaces;
using ServicePortal.Modules.LeaveRequest.Requests;

namespace ServicePortal.Modules.LeaveRequest.Controllers
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
        public async Task<IActionResult> GetAll(GetAllLeaveRequest request)
        {
            var results = await _leaveRequestService.GetAll(request);

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
    }
}
