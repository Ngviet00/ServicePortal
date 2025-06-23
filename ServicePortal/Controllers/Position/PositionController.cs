using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ServicePortals.Application.Interfaces.Position;
using ServicePortals.Application;
using ServicePortals.Application.Dtos.Position.Responses;

namespace ServicePortal.Controllers.Position
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
