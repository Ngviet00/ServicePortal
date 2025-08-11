using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicePortals.Application;
using ServicePortals.Application.Dtos.OrgUnit;
using ServicePortals.Application.Dtos.OrgUnit.Requests;
using ServicePortals.Application.Interfaces.OrgUnit;
using ServicePortals.Shared.SharedDto;

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

        [HttpGet("get-all-departments")]
        public async Task<IActionResult> GetAllDepartments()
        {
            var results = await _orgUnitService.GetAllDepartments();

            return Ok(new BaseResponse<List<OrgUnitDto>>(200, "success", results));
        }

        [HttpGet("get-all-dept-of-orgunit")]
        public async Task<IActionResult> GetAllDeptOfOrgUnit()
        {
            var results = await _orgUnitService.GetAllDeptOfOrgUnit();

            return Ok(new BaseResponse<List<TreeCheckboxResponse>>(200, "success", results));
        }

        [HttpGet("get-all-dept-and-first-org-unit")]
        public async Task<IActionResult> GetAllDepartmentAndFirstOrgUnit()
        {
            var results = await _orgUnitService.GetAllDepartmentAndFirstOrgUnit();

            return Ok(new BaseResponse<dynamic>(200, "success", results));
        }

        [HttpGet("get-orgunit-team-and-user-not-set-orgunit-with-dept")]
        public async Task<IActionResult> GetOrgUnitTeamAndUserNotSetOrgUnitWithDept(int departmentId)
        {
            var result = await _orgUnitService.GetOrgUnitTeamAndUserNotSetOrgUnitWithDept(departmentId);

            return Ok(new BaseResponse<object>(200, "success", result));
        }

        [HttpGet("get-orgunit-user-by-with-dept")]
        public async Task<IActionResult> GetOrgUnitUserWithDept(int departmentId)
        {
            var result = await _orgUnitService.GetOrgUnitUserWithDept(departmentId);

            return Ok(new BaseResponse<List<OrgUnitDto>>(200, "success", result));
        }

        [HttpPost("save-change-org-unit-user")]
        public async Task<IActionResult> SaveChangeOrgUnitUser(SaveChangeOrgUnitUserRequest request)
        {
            var result = await _orgUnitService.SaveChangeUserOrgUnit(request);

            return Ok(new BaseResponse<bool>(200, "success", result));
        }
    }
}
