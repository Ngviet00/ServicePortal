using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ServicePortals.Application.Interfaces.Department;
using ServicePortals.Application;
using ServicePortals.Application.Dtos.Department.Responses;

namespace ServicePortal.Controllers.Department
{
    [ApiController, Route("api/department"), Authorize]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _departmentService.GetAll();

            return Ok(new BaseResponse<List<GetAllDepartmentResponse>>(200, "success", result));
        }
    }
}
