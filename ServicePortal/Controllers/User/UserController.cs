using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicePortal.Filters;
using ServicePortals.Application;
using ServicePortals.Application.Dtos.User.Requests;
using ServicePortals.Application.Dtos.User.Responses;
using ServicePortals.Application.Interfaces.User;

namespace ServicePortal.Controllers.User
{
    [Authorize]
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

        [HttpGet, RoleAuthorize("HR", "HR_Manager")]
        public async Task<IActionResult> GetAll([FromQuery] GetAllUserRequest request)
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

        [HttpGet("{id}")]
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

        [HttpDelete("{id}"), RoleAuthorize("superadmin")]
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

        [HttpPut("{userCode}")]
        public async Task<IActionResult> Update(string userCode, [FromBody] UpdatePersonalInfoRequest request)
        {
            var result = await _userService.Update(userCode, request);

            return Ok(new BaseResponse<UserResponse>(200, "Success", result));
        }

        [HttpGet("org-chart"), AllowAnonymous]
        public async Task<IActionResult> BuildOrgChart(int departmentId)
        {
            var result = await _userService.BuildOrgTree(departmentId);

            return Ok(new BaseResponse<List<TreeNode>>(200, "Success", result));
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

        [HttpGet("search-user-combine-viclock-and-web-system"), AllowAnonymous]
        public async Task<IActionResult> SearchUserCombineViClockAndWebSystem([FromQuery] string userCode)
        {
            var result = await _userService.SearchUserCombineViClockAndWebSystem(userCode);

            return Ok(new BaseResponse<PersonalInfoResponse>(200, "Success", result));
        }

        [HttpGet("get-multiple-user-by-org-position-id/{orgPositionId}")]
        public async Task<IActionResult> GetMultipleUserViclockByOrgPositionId(int orgPositionId)
        {
            var results = await _userService.GetMultipleUserViclockByOrgPositionId(orgPositionId);

            return Ok(new BaseResponse<List<GetMultiUserViClockByOrgPositionIdResponse>>(200, "Success", results));
        }
    }
}