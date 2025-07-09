using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicePortals.Application;
using ServicePortals.Application.Interfaces.OrgUnit;

namespace ServicePortal.Controllers.OrgUnit
{
    [ApiController, Route("api/org-unit"), Authorize]
    public class OrgUnitController : ControllerBase
    {
        private readonly IOrgUnitService _orgUnitService;
        public OrgUnitController(IOrgUnitService orgUnitService)
        {
            _orgUnitService = orgUnitService;
        }

        [HttpGet("get-all-dept-in-org-unit")]
        public async Task<IActionResult> GetAllDepartmentInOrgUnit()
        {
            var results = await _orgUnitService.GetAllDepartmentInOrgUnit();

            return Ok(new BaseResponse<List<object>>(200, "success", results));
        }

        [HttpGet("get-org-unit-by-dept")]
        public async Task<IActionResult> GetOrgUnitByDept(int departmentId)
        {
            var results = await _orgUnitService.GetOrgUnitByDept(departmentId);

            return Ok(new BaseResponse<List<object>>(200, "success", results));
        }
    }
}
