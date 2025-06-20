using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicePortal.Applications.Modules.TimeKeeping.DTO.Requests;
using ServicePortal.Applications.Modules.TimeKeeping.Services.Interfaces;
using ServicePortal.Common;

namespace ServicePortal.Applications.Modules.TimeKeeping.Controllers
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
            return Ok(new BaseResponse<object>(200, "Success", results));
        }

        [HttpGet("get-management-time-keeping"), AllowAnonymous]
        public async Task<IActionResult> GetMngTimeKeeping([FromQuery] GetManagementTimeKeepingRequest request)
        {
            var results = await _timeKeepingService.GetManagementTimeKeeping(request);
            return Ok(new BaseResponse<IEnumerable<dynamic>>(200, "Success", results));
        }

        [HttpPost("confirm-timekeeping-to-hr")]
        public async Task<IActionResult> ConfirmTimeKeepingToHr([FromBody] GetManagementTimeKeepingRequest request)
        {
            var results = await _timeKeepingService.ConfirmTimeKeepingToHr(request);
            return Ok(new BaseResponse<object>(200, "Success", results));
        }
    }
}
