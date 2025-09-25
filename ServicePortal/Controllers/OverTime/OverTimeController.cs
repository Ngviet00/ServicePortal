using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicePortals.Application;
using ServicePortals.Application.Dtos.OverTime.Requests;
using ServicePortals.Application.Interfaces.OverTime;
using ServicePortals.Domain.Entities;

namespace ServicePortal.Controllers.OverTime
{
    [Route("api/overtime"), Authorize]
    [ApiController]
    public class OverTimeController : ControllerBase
    {
        private readonly IOverTimeService _overTimeService;

        public OverTimeController (IOverTimeService overTimeService)
        {
            _overTimeService = overTimeService;
        }

        [HttpGet("type-overtime")]
        public async Task<IActionResult> GetAllTypeOverTime()
        {
            var results = await _overTimeService.GetAllTypeOverTime();

            return Ok(new BaseResponse<List<TypeOverTime>>(200, "success", results));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateOverTimeRequest request)
        {
            var result = await _overTimeService.Create(request);

            return Ok(new BaseResponse<object>(200, "success", result));
        }
    }
}
