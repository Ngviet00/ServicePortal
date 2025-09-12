using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicePortals.Application.Interfaces.OrgUnit;
using ServicePortals.Application;
using ServicePortals.Domain.Entities;

namespace ServicePortal.Controllers.OrgUnit
{
    [Authorize]
    [ApiController, Route("api/org-position")]
    public class OrgPositionController : ControllerBase
    {
        private readonly IOrgPositionService _orgPositionService;
        public OrgPositionController(IOrgPositionService orgPositionService)
        {
            _orgPositionService = orgPositionService;
        }

        [HttpGet("get-org-positions-by-department-id")]
        public async Task<IActionResult> GetOrgPositionsByDepartmentId([FromQuery] int? departmentId)
        {
            var results = await _orgPositionService.GetOrgPositionsByDepartmentId(departmentId);

            return Ok(new BaseResponse<List<OrgPosition>>(200, "success", results));
        }
    }
}
