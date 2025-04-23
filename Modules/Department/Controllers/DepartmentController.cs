using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicePortal.Common;
using ServicePortal.Modules.Deparment.DTO;
using ServicePortal.Modules.Deparment.Interfaces;
using ServicePortal.Modules.Deparment.Requests;
using ServicePortal.Modules.Department.DTO;

namespace ServicePortal.Modules.Deparment.Controllers
{
    [Authorize]
    [ApiController, Route("api/department")]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentService _deparmentService;

        public DepartmentController(IDepartmentService deparmentService)
        {
            _deparmentService = deparmentService;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll(GetAllDepartmentRequest request)
        {
            var results = await _deparmentService.GetAll(request);

            var response = new PageResponse<DepartmentDTO>(200, "Success", results.Data, results.TotalPages, request.Page, request.PageSize, results.TotalItems);

            return Ok(response);
        }

        [HttpGet("get-parent-department")]
        public async Task<IActionResult> GetParentDepartment()
        {
            var deparments = await _deparmentService.GetParentDepartment();

            return Ok(new BaseResponse<List<Domain.Entities.Department>>(200, "success", deparments));
        }

        [HttpGet("get-department-with-children-department-and-position")]
        public async Task<IActionResult> GetDepartmentWithChildrenDepartmentAndPosition()
        {
            var results = await _deparmentService.GetDepartmentWithChildrenDepartmentAndPosition();

            return Ok(new BaseResponse<List<DepartmentTreeDTO>>(200, "success", results));
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

        //[RoleAuthorize(RoleEnum.SuperAdmin)]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deparment = await _deparmentService.Delete(id);

            return Ok(new BaseResponse<Domain.Entities.Department>(200, "success", deparment));
        }
    }
}
