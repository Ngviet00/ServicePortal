using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicePortal.Filters;
using ServicePortals.Application;
using ServicePortals.Application.Dtos.TypeLeave;
using ServicePortals.Application.Dtos.TypeLeave.Requests;
using ServicePortals.Application.Interfaces.TypeLeave;

namespace ServicePortal.Controllers.TypeLeave
{
    [ApiController, Route("api/type-leave"), Authorize]
    public class TypeLeaveController : ControllerBase
    {
        private readonly ITypeLeaveService _typeLeaveService;

        public TypeLeaveController(ITypeLeaveService typeLeaveService)
        {
            _typeLeaveService = typeLeaveService;
        }

        [HttpGet, RoleAuthorize("HR", "HR_Manager", "user")]
        public async Task<IActionResult> GetAll([FromQuery] GetAllTypeLeaveRequest request)
        {
            var results = await _typeLeaveService.GetAll(request);

            var response = new BaseResponse<List<ServicePortals.Domain.Entities.TypeLeave>>(200, "Success", results);

            return Ok(response);
        }

        [HttpGet("{id}"), RoleAuthorize("HR", "HR_Manager")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _typeLeaveService.GetById(id);

            return Ok(new BaseResponse<ServicePortals.Domain.Entities.TypeLeave>(200, "success", result));
        }

        [HttpPost, RoleAuthorize("HR", "HR_Manager")]
        public async Task<IActionResult> Create([FromBody] TypeLeaveDto dto)
        {
            var result = await _typeLeaveService.Create(dto);

            return Ok(new BaseResponse<ServicePortals.Domain.Entities.TypeLeave>(200, "success", result));
        }

        [HttpPut("{id}"), RoleAuthorize("HR", "HR_Manager")]
        public async Task<IActionResult> Update(int id, [FromBody] TypeLeaveDto dto)
        {
            var result = await _typeLeaveService.Update(id, dto);

            return Ok(new BaseResponse<ServicePortals.Domain.Entities.TypeLeave>(200, "success", result));
        }

        [HttpDelete("{id}"), RoleAuthorize("HR", "HR_Manager")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _typeLeaveService.Delete(id);

            return Ok(new BaseResponse<ServicePortals.Domain.Entities.TypeLeave>(200, "success", result));
        }
    }
}
