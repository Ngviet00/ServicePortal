using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicePortals.Application.Interfaces.OrgUnit;
using ServicePortals.Application;
using ServicePortals.Domain.Entities;

namespace ServicePortal.Controllers.OrgUnit
{
    [Authorize]
    [ApiController, Route("api/position")]
    public class PositionController : ControllerBase
    {
        private readonly IPositionService _positionService;
        public PositionController(IPositionService positionService)
        {
            _positionService = positionService;
        }

        [HttpGet("get-positions-by-department-id")]
        public async Task<IActionResult> GetPositionsByDepartmentId([FromQuery] int departmentId)
        {
            var results = await _positionService.GetPositionsByDepartmentId(departmentId);

            return Ok(new BaseResponse<List<Position>>(200, "success", results));
        }
    }
}
