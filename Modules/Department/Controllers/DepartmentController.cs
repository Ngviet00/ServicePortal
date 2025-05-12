using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicePortal.Common;
using ServicePortal.Common.Filters;
using ServicePortal.Modules.Deparment.DTO;
using ServicePortal.Modules.Department.DTO.Requests;
using ServicePortal.Modules.Department.Services.Interfaces;

namespace ServicePortal.Modules.Deparment.Controllers
{
    [Authorize]
    [ApiController, Route("api/department"), RoleAuthorize("HR", "HR_Manager")]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentService _deparmentService;

        public DepartmentController(IDepartmentService deparmentService)
        {
            _deparmentService = deparmentService;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll([FromQuery] GetAllDepartmentRequestDto request)
        {
            var results = await _deparmentService.GetAll(request);

            var response = new PageResponse<DepartmentDTO>(
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

        [HttpGet("get-parent-department")]
        public async Task<IActionResult> GetParentDepartment()
        {
            var deparments = await _deparmentService.GetParentDepartment();

            return Ok(new BaseResponse<List<Domain.Entities.Department>>(200, "success", deparments));
        }

        [HttpGet("get-by-id/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var deparment = await _deparmentService.GetById(id);

            return Ok(new BaseResponse<Domain.Entities.Department>(200, "success", deparment));
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create(DepartmentDTO dto)
        {
            var deparment = await _deparmentService.Create(dto);

            return Ok(new BaseResponse<Domain.Entities.Department>(200, "success", deparment));
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, DepartmentDTO dto)
        {
            var deparment = await _deparmentService.Update(id, dto);

            return Ok(new BaseResponse<Domain.Entities.Department>(200, "success", deparment));
        }

        [HttpDelete("delete/{id}"), RoleAuthorize("superadmin")]
        public async Task<IActionResult> Delete(int id)
        {
            var deparment = await _deparmentService.Delete(id);

            return Ok(new BaseResponse<Domain.Entities.Department>(200, "success", deparment));
        }
    }
}
