using Microsoft.AspNetCore.Mvc;
using ServicePortals.Application.Dtos.Role.Requests;
using ServicePortals.Application.Interfaces.Role;
using ServicePortals.Application;
using ServicePortal.Filters;
using Entities = ServicePortals.Domain.Entities;

namespace ServicePortal.Controllers.Role
{
    [ApiController, Route("api/permission")]
    public class PermissionController : ControllerBase
    {
        private readonly IPermissionService _permissionService;

        public PermissionController(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] SearchPermissionRequest request)
        {
            var results = await _permissionService.GetAll(request);

            var response = new PageResponse<Entities.Permission>(
                200,
                "Success",
                results.Data,
                results.TotalPages,
                request.Page,
                request.PageSize,
                results.TotalItems
            );

            return Ok(response);
        }

        [HttpGet("{id}"), RoleAuthorize("superadmin")]
        public async Task<IActionResult> GetById(int id)
        {
            var role = await _permissionService.GetById(id);

            return Ok(new BaseResponse<Entities.Permission>(200, "success", role));
        }

        [HttpPost, RoleAuthorize("superadmin")]
        public async Task<IActionResult> Create([FromBody] CreatePermissionRequest request)
        {
            var role = await _permissionService.Create(request);

            return Ok(new BaseResponse<Entities.Permission>(200, "success", role));
        }

        [HttpPut("{id}"), RoleAuthorize("superadmin")]
        public async Task<IActionResult> Update(int id, [FromBody] CreatePermissionRequest request)
        {
            var role = await _permissionService.Update(id, request);

            return Ok(new BaseResponse<Entities.Permission>(200, "success", role));
        }

        [HttpDelete("{id}"), RoleAuthorize("superadmin")]
        public async Task<IActionResult> Delete(int id)
        {
            var role = await _permissionService.Delete(id);

            return Ok(new BaseResponse<Entities.Permission>(200, "success", role));
        }
    }
}
