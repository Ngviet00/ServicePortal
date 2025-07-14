using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicePortals.Application;
using ServicePortals.Application.Interfaces.OrgUnit;

namespace ServicePortal.Controllers.OrgUnit
{
    [Authorize]
    [ApiController, Route("api/org-unit")]
    public class OrgUnitController : ControllerBase
    {
        private readonly IOrgUnitService _orgUnitService;
        public OrgUnitController(IOrgUnitService orgUnitService)
        {
            _orgUnitService = orgUnitService;
        }

        [HttpGet("get-all-dept-and-first-org-unit")]
        public async Task<IActionResult> GetAllDepartmentAndFirstOrgUnit()
        {
            var results = await _orgUnitService.GetAllDepartmentAndFirstOrgUnit();

            return Ok(new BaseResponse<dynamic>(200, "success", results));
        }
    }
}
