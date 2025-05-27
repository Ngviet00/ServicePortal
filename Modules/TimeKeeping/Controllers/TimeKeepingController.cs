using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicePortal.Common;
using ServicePortal.Modules.TimeKeeping.DTO.Requests;
using ServicePortal.Modules.TimeKeeping.DTO.Responses;
using ServicePortal.Modules.TimeKeeping.Services.Interfaces;
using ServicePortal.Modules.User.DTO.Responses;

namespace ServicePortal.Modules.TimeKeeping.Controllers
{
    [Authorize]
    [ApiController, Route("api/time-keeping")]
    public class TimeKeepingController : ControllerBase
    {
        private readonly ITimeKeepingService _timeKeepingService;

        public TimeKeepingController(ITimeKeepingService timeKeepingService)
        {
            _timeKeepingService = timeKeepingService;
        }

        [HttpGet("get-personal-time-keeping")]
        public async Task<IActionResult> GetPersonalTimeKeeping([FromQuery] GetPersonalTimeKeepingDto request)
        {
            var results = await _timeKeepingService.GetPersonalTimeKeeping(request);

            var response = new BaseResponse<object>(200, "Success", results);

            return Ok(response);
        }

        [HttpGet("get-management-time-keeping")]
        public async Task<IActionResult> GetMngTimeKeeping([FromQuery] GetManagementTimeKeepingDto request)
        {
            var results = await _timeKeepingService.GetManagementTimeKeeping(request);
            var response = new BaseResponse<ManagementTimeKeepingResponseDto>(200, "Success", results);

            return Ok(response);
        }

        [HttpPost("confirm-time-keeping-to-hr")]
        public async Task<IActionResult> ConfirmTimeKeeping([FromBody] GetManagementTimeKeepingDto request)
        {
            var results = await _timeKeepingService.ConfirmTimeKeeping(request);
            var response = new BaseResponse<object>(200, "Success", results);

            return Ok(response);
        }

        [HttpGet("get-list-user-to-choose-manage-time-keeping")]
        public async Task<IActionResult> GetListUserToChooseManageTimeKeeping([FromQuery] GetUserManageTimeKeepingDto request)
        {
            var results = await _timeKeepingService.GetListUserToChooseManageTimeKeeping(request);
            var response = new PageResponse<UserResponseDto>(
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

        [HttpPost("save-manage-time-keeping")]
        public async Task<IActionResult> SaveManageTimeKeeping([FromBody] SaveManageTimeKeepingDto dto)
        {
            var results = await _timeKeepingService.SaveManageTimeKeeping(dto);
            var response = new BaseResponse<object>(200, "Success", results);

            return Ok(response);
        }
    }
}
