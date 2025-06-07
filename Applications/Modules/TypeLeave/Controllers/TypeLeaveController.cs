using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicePortal.Applications.Modules.TypeLeave.DTO;
using ServicePortal.Applications.Modules.TypeLeave.DTO.Requests;
using ServicePortal.Applications.Modules.TypeLeave.Services.Interfaces;
using ServicePortal.Common;
using ServicePortal.Common.Filters;

namespace ServicePortal.Applications.Modules.TypeLeave.Controllers
{
    [ApiController, Route("api/type-leave"), Authorize]
    public class TypeLeaveController : ControllerBase
    {
        private readonly ITypeLeaveService _typeLeaveService;

        public TypeLeaveController(ITypeLeaveService typeLeaveService)
        {
            _typeLeaveService = typeLeaveService;
        }

        [HttpGet("get-all"), Authorize, RoleAuthorize("HR", "HR_Manager", "user")]
        public async Task<IActionResult> GetAll([FromQuery] GetAllTypeLeaveRequestDto request)
        {
            var results = await _typeLeaveService.GetAll(request);

            var response = new BaseResponse<List<Domain.Entities.TypeLeave>>(200, "Success", results);

            return Ok(response);
        }

        [HttpGet("get-by-id/{id}"), RoleAuthorize("HR", "HR_Manager")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _typeLeaveService.GetById(id);

            return Ok(new BaseResponse<Domain.Entities.TypeLeave>(200, "success", result));
        }

        [HttpPost("create"), RoleAuthorize("HR", "HR_Manager")]
        public async Task<IActionResult> Create([FromBody] TypeLeaveDto dto)
        {
            var result = await _typeLeaveService.Create(dto);

            return Ok(new BaseResponse<Domain.Entities.TypeLeave>(200, "success", result));
        }

        [HttpPut("update/{id}"), RoleAuthorize("HR", "HR_Manager")]
        public async Task<IActionResult> Update(int id, [FromBody] TypeLeaveDto dto)
        {
            var result = await _typeLeaveService.Update(id, dto);

            return Ok(new BaseResponse<Domain.Entities.TypeLeave>(200, "success", result));
        }

        [HttpDelete("delete/{id}"), RoleAuthorize("HR", "HR_Manager")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _typeLeaveService.Delete(id);

            return Ok(new BaseResponse<Domain.Entities.TypeLeave>(200, "success", result));
        }
    }
}
