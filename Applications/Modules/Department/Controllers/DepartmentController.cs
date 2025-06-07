using Microsoft.AspNetCore.Mvc;
using ServicePortal.Common;
using ServicePortal.Applications.Viclock.Queries;
using ServicePortal.Applications.Viclock.DTO.Department;
using Microsoft.AspNetCore.Authorization;

namespace ServicePortal.Applications.Modules.Department.Controllers
{
    [ApiController, Route("api/department"), Authorize]
    public class DepartmentController : ControllerBase
    {
        private readonly IViClockDepartmentQuery _viClockDepartmentQuery;

        public DepartmentController(IViClockDepartmentQuery viClockDepartmentQuery)
        {
            _viClockDepartmentQuery = viClockDepartmentQuery;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _viClockDepartmentQuery.GetAll();

            return Ok(new BaseResponse<List<DepartmentViclockDtos>>(200, "success", result));
        }
    }
}
