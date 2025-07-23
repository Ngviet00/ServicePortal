using Microsoft.AspNetCore.Mvc;
using ServicePortals.Application.Dtos.Role.Requests;
using ServicePortals.Application.Interfaces.Role;
using ServicePortals.Application;

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

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll([FromQuery] SearchRoleRequest request)
        {
            var results = await _permissionService.GetAll(request);

            var response = new PageResponse<ServicePortals.Domain.Entities.Permission>(
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
    }
}
