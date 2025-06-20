using Microsoft.AspNetCore.Mvc;
using ServicePortal.Common;
using Microsoft.AspNetCore.Authorization;
using ServicePortal.Applications.Modules.Department.Services.Interfaces;
using ServicePortal.Applications.Modules.Department.DTO.Responses;

namespace ServicePortal.Applications.Modules.Department.Controllers
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
