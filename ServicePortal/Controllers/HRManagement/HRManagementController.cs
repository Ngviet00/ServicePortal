using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicePortals.Application;
using ServicePortals.Application.Dtos.HRManagement.Requests;
using ServicePortals.Application.Interfaces.HRManagement;
using ServicePortals.Domain.Entities;

namespace ServicePortal.Controllers.HRManagement
{
    [ApiController, Route("api/hr-management"), Authorize]
    public class HRController : ControllerBase
    {
        //private readonly IHRManagementService _hrService;
        //public HRController(IHRManagementService hrManagementServices)
        //{
        //    _hrService = hrManagementServices;
        //}

        //[HttpGet("get-all-assign-attendance-user")]
        //public async Task<IActionResult> GetAllAssignableAttendanceUsersRequest([FromQuery] GetAssignableAttendanceUsersRequest request)
        //{
        //    var results = await _hrService.GetAssignableAttendanceUsersRequest(request);

        //    return Ok(new PageResponse<object>(
        //        200,
        //        "Success",
        //        results.Data,
        //        results.TotalPages,
        //        request.Page,
        //        request.PageSize,
        //        results.TotalItems
        //    ));
        //}

        //[HttpGet("get-all-hr")]
        //public async Task<IActionResult> GetAllHR()
        //{
        //    var results = await _hrService.GetAllHR();
        //    return Ok(new BaseResponse<object>(200, "Success", results));
        //}

        //[HttpGet("attendance-managers")]
        //public async Task<IActionResult> GetUsersWithAttendanceManagers()
        //{
        //    var results = await _hrService.GetUsersWithAttendanceManagers();
        //    return Ok(new BaseResponse<object>(200, "Success", results));
        //}

        //[HttpPost("assign-attendance-manager-to-user")]
        //public async Task<IActionResult> HrAssignAttendanceManagers(HrAssignAttendanceManagersRequest request)
        //{
        //    var results = await _hrService.HrAssignAttendanceManagers(request);
        //    return Ok(new BaseResponse<object>(200, "Success", results));
        //}

        //[HttpPost("attendance-multiple-people-to-attendance-manager")]
        //public async Task<IActionResult> AssignMultiplePeopleToAttendanceManager([FromBody] AssignMultiplePeopleToAttendanceManagerRequest request)
        //{
        //    var results = await _hrService.AssignMultiplePeopleToAttendanceManager(request);
        //    return Ok(new BaseResponse<object>(200, "Success", results));
        //}

        //[HttpPost("save-hr-management")]
        //public async Task<IActionResult> SaveHrManagement([FromBody] SaveHRManagementRequest request)
        //{
        //    var results = await _hrService.SaveHrManagement(request);
        //    return Ok(new BaseResponse<object>(200, "Success", results));
        //}

        //[HttpPost("change-manage-attendance"), AllowAnonymous]
        //public async Task<IActionResult> ChangeManageAttendance([FromBody] ChangeManageAttendanceRequest request)
        //{
        //    var results = await _hrService.ChangeManageAttendance(request);
        //    return Ok(new BaseResponse<object>(200, "Success", results));
        //}

        //[HttpGet("get-hr-managements")]
        //public async Task<IActionResult> GetHrManagements()
        //{
        //    var results = await _hrService.GetHrManagements();
        //    return Ok(new BaseResponse<List<HrManagements>>(200, "Success", results));
        //}
    }
}
