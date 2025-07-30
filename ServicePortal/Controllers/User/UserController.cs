using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicePortal.Filters;
using ServicePortals.Application;
using ServicePortals.Application.Dtos.LeaveRequest.Requests;
using ServicePortals.Application.Dtos.User.Requests;
using ServicePortals.Application.Dtos.User.Responses;
using ServicePortals.Application.Interfaces.LeaveRequest;
using ServicePortals.Application.Interfaces.MemoNotification;
using ServicePortals.Application.Interfaces.User;
using ServicePortals.Infrastructure.Excel;

namespace ServicePortal.Controllers.User
{
    [Authorize]
    [ApiController, Route("api/user")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILeaveRequestService _leaveRequestService;
        private readonly IMemoNotificationService _memoNotificationService;
        private readonly ExcelService _excelService;

        public UserController(
            IUserService userService,
            ILeaveRequestService leaveRequestService,
            IMemoNotificationService memoNotificationService,
            ExcelService excelService
        )
        {
            _userService = userService;
            _leaveRequestService = leaveRequestService;
            _memoNotificationService = memoNotificationService;
            _excelService = excelService;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            var Usercode = User.FindFirst("user_code")?.Value;

            var results = await _userService.GetMe(Usercode ?? "");

            var response = new BaseResponse<PersonalInfoResponse>(200, "Success", results);

            return Ok(response);
        }

        [HttpGet("get-all"), RoleAuthorize("HR", "HR_Manager")]
        public async Task<IActionResult> GetAll(GetAllUserRequest request)
        {
            var results = await _userService.GetAll(request);

            var response = new PageResponse<GetAllUserResponse>(
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

        [HttpGet("get-by-id/{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            UserResponse? userDto = await _userService.GetById(id);

            return Ok(new BaseResponse<UserResponse>(200, "success", userDto));
        }

        [HttpGet("get-by-code/{code}")]
        public async Task<IActionResult> GetByUserCode(string code)
        {
            UserResponse? userDto = await _userService.GetByUserCode(code);

            return Ok(new BaseResponse<UserResponse>(200, "success", userDto));
        }

        [HttpDelete("delete/{id}"), RoleAuthorize("superadmin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _userService.ForceDelete(id);

            return Ok(new BaseResponse<UserResponse>(200, "Delete user successfully", null));
        }

        [HttpDelete("force-delete/{id}"), RoleAuthorize("superadmin")]
        public async Task<IActionResult> ForceDelete(Guid id)
        {
            await _userService.ForceDelete(id);

            return Ok(new BaseResponse<UserResponse>(200, "Delete user permanently successfully", null));
        }

        [HttpGet("get-role-permission-user")]
        public async Task<IActionResult> GetDetailUserWithRoleAndPermission([FromQuery] string userCode)
        {
            var result = await _userService.GetDetailUserWithRoleAndPermission(userCode);

            return Ok(new BaseResponse<DetailUserWithRoleAndPermissionResponse>(200, "Success", result));
        }

        [HttpPost("update-user-role"), RoleAuthorize("superadmin")]
        public async Task<IActionResult> UpdateUserRole(UpdateUserRoleRequest request)
        {
            var result = await _userService.UpdateUserRole(request);

            return Ok(new BaseResponse<bool>(200, "Success", result));
        }

        [HttpPost("update-user-permission"), RoleAuthorize("superadmin")]
        public async Task<IActionResult> UpdateUserPermission(UpdateUserRoleRequest request)
        {
            var result = await _userService.UpdateUserPermission(request);

            return Ok(new BaseResponse<bool>(200, "Success", result));
        }

        [HttpPost("reset-password"), RoleAuthorize("superadmin")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var result = await _userService.ResetPassword(request);

            return Ok(new BaseResponse<UserResponse>(200, "Success", result));
        }

        [HttpPut("update/{userCode}")]
        public async Task<IActionResult> Update(string userCode, [FromBody] UpdatePersonalInfoRequest request)
        {
            var result = await _userService.Update(userCode, request);

            return Ok(new BaseResponse<UserResponse>(200, "Success", result));
        }

        [HttpGet("org-chart"), AllowAnonymous]
        public async Task<IActionResult> BuildOrgChart(int departmentId)
        {
            var result = await _userService.BuildOrgTree(departmentId);

            return Ok(new BaseResponse<List<OrgUnitNode_1>>(200, "Success", result));
        }

        [HttpGet("get-user-by-parent-org-unit-id")]
        public async Task<IActionResult> GetUserByParentOrgUnit(int orgUnitId)
        {
            var results = await _userService.GetUserByParentOrgUnit(orgUnitId);

            return Ok(new BaseResponse<List<object>>(200, "success", results));
        }

        [HttpGet("search-all-user-from-viclock")]
        public async Task<IActionResult> SearchAllUserFromViClock([FromQuery] SearchAllUserFromViclockRequest request)
        {
            var results = await _userService.SearchAllUserFromViClock(request);

            return Ok(new PageResponse<object>(
                200,
                "Success",
                results.Data,
                results.TotalPages,
                request.Page,
                request.PageSize,
                results.TotalItems
            ));
        }

        [HttpGet("count-wait-approval-in-sidebar")]
        public async Task<IActionResult> CountWaitApprovalInSidebar([FromQuery] CountWaitAprrovalSidebarRequest request)
        {
            var userClaim = HttpContext.User;

            GetAllLeaveRequestWaitApprovalRequest requestLeaveRq = new()
            {
                UserCode = request.UserCode,
                OrgUnitId = request.OrgUnitId
            };

            CountWaitApprovalInSidebarResponse results = new();

            results.CountWaitNotification = await _memoNotificationService.CountWaitApprovalMemoNotification(request?.OrgUnitId ?? -9999);
            results.CountWaitLeaveRequest = await _leaveRequestService.CountWaitApproval(requestLeaveRq, userClaim);

            return Ok(new BaseResponse<CountWaitApprovalInSidebarResponse>(200, "Success", results));
        }

        [HttpPost("import-excel-test-insert-new-user"), AllowAnonymous]
        public async Task<IActionResult> ImportExcelTestInsertNewUser(IFormFile file)
        {
            await _excelService.InsertFromExcelAsync(file);
            return Ok("Nhập dữ liệu thành công");
        }

        [HttpGet("test"), AllowAnonymous]
        public async Task<IActionResult> Test()
        {
            return Ok(await _userService.Test());
        }
    }
}