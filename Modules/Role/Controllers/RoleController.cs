using Microsoft.AspNetCore.Mvc;
using Serilog;
using ServicePortal.Common;
using ServicePortal.Common.Helpers;
using ServicePortal.Modules.Role.Interfaces;

namespace ServicePortal.Modules.Role.Controllers
{
    [ApiController, Route("role")]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
        {
            var roles = await _roleService.GetAll();

            //Log.Information("test log ok {roles}", Helper.ConvertObjToString(roles));

            return Ok(new BaseResponse<List<Domain.Entities.Role>>(200, "success", roles));
        }

        [HttpGet("get-by-id/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var role = await _roleService.GetById(id);

            return Ok(new BaseResponse<Domain.Entities.Role>(200, "success", role));
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create(string name)
        {
            var role = await _roleService.Create(name);

            return Ok(new BaseResponse<Domain.Entities.Role>(200, "success", role));
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, string name)
        {
            var role = await _roleService.Update(id, name);

            return Ok(new BaseResponse<Domain.Entities.Role>(200, "success", role));
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var role = await _roleService.Delete(id);

            return Ok(new BaseResponse<Domain.Entities.Role>(200, "success", role));
        }
    }
}
