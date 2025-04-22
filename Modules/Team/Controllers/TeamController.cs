using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicePortal.Common;
using ServicePortal.Modules.Team.DTO;
using ServicePortal.Modules.Team.Interfaces;
using ServicePortal.Modules.Team.Requests;

namespace ServicePortal.Modules.Team.Controllers
{
    [Authorize]
    [ApiController, Route("api/team")]
    public class TeamController : ControllerBase
    {
        private readonly ITeamService _teamService;

        public TeamController(ITeamService teamService)
        {
            _teamService = teamService;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll(GetAllTeamRequest request)
        {
            var results = await _teamService.GetAll(request);

            var response = new PageResponse<TeamDTO>(200, "Success", results.Data, results.TotalPages, request.Page, request.PageSize, results.TotalItems);

            return Ok(response);
        }

        [HttpGet("get-by-id/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var team = await _teamService.GetById(id);

            return Ok(new BaseResponse<Domain.Entities.Team>(200, "success", team));
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create(TeamDTO dto)
        {
            var team = await _teamService.Create(dto);

            return Ok(new BaseResponse<Domain.Entities.Team>(200, "success", team));
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, TeamDTO dto)
        {
            var team = await _teamService.Update(id, dto);

            return Ok(new BaseResponse<Domain.Entities.Team>(200, "success", team));
        }

        //[RoleAuthorize(RoleEnum.SuperAdmin)]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var team = await _teamService.Delete(id);

            return Ok(new BaseResponse<Domain.Entities.Team>(200, "success", team));
        }
    }
}
