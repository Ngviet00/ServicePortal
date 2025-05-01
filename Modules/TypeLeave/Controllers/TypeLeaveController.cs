using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicePortal.Common;
using ServicePortal.Modules.TypeLeave.DTO;
using ServicePortal.Modules.TypeLeave.DTO.Requests;
using ServicePortal.Modules.TypeLeave.Interfaces;

namespace ServicePortal.Modules.TypeLeave.Controllers
{
    [ApiController, Route("api/type-leave"), Authorize]
    public class TypeLeaveController : ControllerBase
    {
        private readonly ITypeLeaveService _typeLeaveService;

        public TypeLeaveController(ITypeLeaveService typeLeaveService)
        {
            _typeLeaveService = typeLeaveService;
        }

        [HttpGet("get-all"), Authorize]
        public async Task<IActionResult> GetAll([FromQuery] GetAllTypeLeaveRequest request)
        {
            var results = await _typeLeaveService.GetAll(request);

            var response = new PageResponse<Domain.Entities.TypeLeave>(200, "Success", results.Data, results.TotalPages, request.Page, request.PageSize, results.TotalItems);

            return Ok(response);
        }

        [HttpGet("get-by-id/{id}"), AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _typeLeaveService.GetById(id);

            return Ok(new BaseResponse<Domain.Entities.TypeLeave>(200, "success", result));
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] TypeLeaveDTO dto)
        {
            var result = await _typeLeaveService.Create(dto);

            return Ok(new BaseResponse<Domain.Entities.TypeLeave>(200, "success", result));
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] TypeLeaveDTO dto)
        {
            var result = await _typeLeaveService.Update(id, dto);

            return Ok(new BaseResponse<Domain.Entities.TypeLeave>(200, "success", result));
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _typeLeaveService.Delete(id);

            return Ok(new BaseResponse<Domain.Entities.TypeLeave>(200, "success", result));
        }
    }
}
