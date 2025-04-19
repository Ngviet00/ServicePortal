using Microsoft.AspNetCore.Mvc;
using ServicePortal.Common;
using ServicePortal.Modules.PositionDeparment.DTO;
using ServicePortal.Modules.PositionDeparment.Interfaces;

namespace ServicePortal.Modules.PositionDeparment.Controllers
{
    [ApiController, Route("position-department")]
    public class PositionDepartmentController : ControllerBase
    {
        private readonly IPositionDepartmentService _positionDeparmentService;

        public PositionDepartmentController(IPositionDepartmentService positionDeparmentService)
        {
            _positionDeparmentService = positionDeparmentService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create(PositionDepartmentDTO dto)
        {
            var positionDeparmentDto = await _positionDeparmentService.Create(dto);

            return Ok(new BaseResponse<PositionDepartmentDTO>(200, "success", positionDeparmentDto));
        }
    }
}
