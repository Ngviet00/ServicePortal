using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServicePortal.Filters;
using ServicePortals.Application;
using ServicePortals.Application.Dtos.User.Requests;
using ServicePortals.Application.Dtos.User.Responses;
using ServicePortals.Application.Interfaces.User;

namespace ServicePortal.Controllers.User
{
    //[Authorize]
    [ApiController, Route("api/user")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
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

        [HttpPost("update-user-role"), RoleAuthorize("superadmin")]
        public async Task<IActionResult> UpdateUserRole(UpdateUserRoleRequest request)
        {
            var result = await _userService.UpdateUserRole(request);

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

            return Ok(new BaseResponse<List<OrgUnitNode>>(200, "Success", result));
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

        [HttpPost("update-user-have-permission-mng-timekeeping")]
        public async Task<IActionResult> UpdateUserHavePermissionMngTimeKeeping([FromBody] List<string> userCodes)
        {
            var results = await _userService.UpdateUserHavePermissionMngTimeKeeping(userCodes);

            return Ok(new BaseResponse<object>(200, "success", results));
        }

        [HttpGet("get-user-have-permission-mng-timekeeping")]
        public async Task<IActionResult> GetUserHavePermissionMngTimeKeeping()
        {
            var results = await _userService.GetUserHavePermissionMngTimeKeeping();

            return Ok(new BaseResponse<object>(200, "success", results));
        }

        [HttpPost("update-user-mng-timekeeping")]
        public async Task<IActionResult> UpdateUserMngTimeKeeping(UpdateUserMngTimeKeepingRequest request)
        {
            var results = await _userService.UpdateUserMngTimeKeeping(request);

            return Ok(new BaseResponse<object>(200, "success", results));
        }

        [HttpGet("test"), AllowAnonymous]
        public async Task<IActionResult> Test()
        {
            return Ok(await _userService.Test());
        }
    }
}