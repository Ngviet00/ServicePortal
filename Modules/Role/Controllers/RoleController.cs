using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicePortal.Common;
using ServicePortal.Common.Filters;
using ServicePortal.Modules.Role.Interfaces;
using ServicePortal.Modules.Role.Requests;

namespace ServicePortal.Modules.Role.Controllers
{
    [ApiController, Route("api/role"), Authorize]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet("get-all"), Authorize, RoleAuthorize("HR", "HR_Manager")]
        public async Task<IActionResult> GetAll([FromQuery] SearchRoleRequest request)
        {
            var results = await _roleService.GetAll(request);

            var response = new PageResponse<Domain.Entities.Role>(200, "Success", results.Data, results.TotalPages, request.Page, request.PageSize, results.TotalItems);

            return Ok(response);
        }

        [HttpGet("get-by-id/{id}"), RoleAuthorize("superadmin")]
        public async Task<IActionResult> GetById(int id)
        {
            var role = await _roleService.GetById(id);

            return Ok(new BaseResponse<Domain.Entities.Role>(200, "success", role));
        }

        [HttpPost("create"), RoleAuthorize("superadmin")]
        public async Task<IActionResult> Create([FromBody] CreateRoleRequest request)
        {
            var role = await _roleService.Create(request);

            return Ok(new BaseResponse<Domain.Entities.Role>(200, "success", role));
        }

        [HttpPut("update/{id}"), RoleAuthorize("superadmin")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateRoleRequest request)
        {
            var role = await _roleService.Update(id, request);

            return Ok(new BaseResponse<Domain.Entities.Role>(200, "success", role));
        }

        [HttpDelete("delete/{id}"), RoleAuthorize("superadmin")]
        public async Task<IActionResult> Delete(int id)
        {
            var role = await _roleService.Delete(id);

            return Ok(new BaseResponse<Domain.Entities.Role>(200, "success", role));
        }
    }
}
