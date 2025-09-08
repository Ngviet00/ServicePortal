using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicePortals.Application;
using ServicePortals.Application.Dtos.OrgUnit.Requests;
using ServicePortals.Application.Interfaces.OrgUnit;
using ServicePortals.Domain.Enums;
using Entities = ServicePortals.Domain.Entities;

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

            return Ok(new BaseResponse<List<Entities.OrgUnit>>(200, "success", results));
        }

        [HttpGet("get-team-by-department-id-and-user-not-set-org-position-id-by-department-name")]
        public async Task<IActionResult> GetTeamByDeptIdAndUserNotSetOrgPositionId(int departmentId)
        {
            var result = await _orgUnitService.GetTeamByDeptIdAndUserNotSetOrgPositionId(departmentId);

            return Ok(new BaseResponse<object>(200, "success", result));
        }

        [HttpGet("get-list-user-by-team-id")]
        public async Task<IActionResult> GetListUserByTeamId(int teamId)
        {
            var result = await _orgUnitService.GetListUserByTeamId(teamId);

            return Ok(new BaseResponse<object>(200, "success", result));
        }

        [HttpPost("save-change-org-unit-user")]
        public async Task<IActionResult> SaveChangeOrgUnitUser(SaveChangeOrgUnitUserRequest request)
        {
            var result = await _orgUnitService.SaveChangeUserOrgUnit(request);

            return Ok(new BaseResponse<bool>(200, "success", result));
        }

        [HttpGet("get-department-and-children-team")]
        public async Task<IActionResult> GetDepartmentAndChildrenTeam()
        {
            var results = await _orgUnitService.GetDepartmentAndChildrenTeam();

            return Ok(new BaseResponse<dynamic>(200, "success", results));
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
        {
            var results = await _orgUnitService.GetAll();

            return Ok(new BaseResponse<List<Entities.OrgUnit>>(200, "success", results));
        }

        [HttpGet("get-all-team")]
        public async Task<IActionResult> GetAllTeam(int? departmentId)
        {
            var results = await _orgUnitService.GetAll(e => e.UnitId == (int)UnitEnum.Team, departmentId);

            return Ok(new BaseResponse<List<Entities.OrgUnit>>(200, "success", results));
        }

        [HttpGet("get-all-without-team")]
        public async Task<IActionResult> GetAllWithOutTeam()
        {
            var results = await _orgUnitService.GetAll(e => e.UnitId != (int)UnitEnum.Team);

            return Ok(new BaseResponse<List<Entities.OrgUnit>>(200, "success", results));
        }
    }
}
