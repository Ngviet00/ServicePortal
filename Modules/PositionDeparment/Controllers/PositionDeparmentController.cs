using Microsoft.AspNetCore.Mvc;
using ServicePortal.Common;
using ServicePortal.Modules.PositionDeparment.DTO;
using ServicePortal.Modules.PositionDeparment.Interfaces;

namespace ServicePortal.Modules.PositionDeparment.Controllers
{
    [ApiController, Route("position-deparment")]
    public class PositionDeparmentController : ControllerBase
    {
        private readonly IPositionDeparmentService _positionDeparmentService;

        public PositionDeparmentController(IPositionDeparmentService positionDeparmentService)
        {
            _positionDeparmentService = positionDeparmentService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create(PositionDeparmentDTO dto)
        {
            var positionDeparmentDto = await _positionDeparmentService.Create(dto);

            return Ok(new BaseResponse<PositionDeparmentDTO>(200, "success", positionDeparmentDto));
        }
    }
}
