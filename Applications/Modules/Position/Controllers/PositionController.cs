using Microsoft.AspNetCore.Mvc;
using ServicePortal.Common;
using Microsoft.AspNetCore.Authorization;
using ServicePortal.Applications.Modules.Position.Services.Interfaces;
using ServicePortal.Applications.Modules.Position.DTO.Responses;

namespace ServicePortal.Applications.Modules.Position.Controllers
{
    [ApiController, Route("api/position"), Authorize]
    public class PositionController : ControllerBase
    {
        private readonly IPositionService _positionService;

        public PositionController(IPositionService positionService)
        {
            _positionService = positionService;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _positionService.GetAll();

            return Ok(new BaseResponse<List<GetAllPositionResponse>>(200, "success", result));
        }
    }
}
