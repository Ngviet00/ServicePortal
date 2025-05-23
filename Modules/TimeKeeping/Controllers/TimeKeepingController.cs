using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicePortal.Common;
using ServicePortal.Modules.TimeKeeping.DTO.Requests;
using ServicePortal.Modules.TimeKeeping.DTO.Responses;
using ServicePortal.Modules.TimeKeeping.Services.Interfaces;

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

            var response = new BaseResponse<List<ManagementTimeKeepingResponseDto>>(200, "Success", results);

            return Ok(response);
        }

        [HttpPost("confirm-time-keeping-to-hr")]
        public async Task<IActionResult> ConfirmTimeKeeping([FromBody] GetManagementTimeKeepingDto request)
        {
            var results = await _timeKeepingService.ConfirmTimeKeeping(request);

            var response = new BaseResponse<object>(200, "Success", results);

            return Ok(response);
        }
    }
}
