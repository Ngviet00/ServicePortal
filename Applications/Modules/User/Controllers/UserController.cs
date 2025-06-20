using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicePortal.Applications.Modules.User.DTO.Requests;
using ServicePortal.Applications.Modules.User.DTO.Responses;
using ServicePortal.Applications.Modules.User.Services;
using ServicePortal.Applications.Modules.User.Services.Interfaces;
using ServicePortal.Applications.Viclock.DTO.User;
using ServicePortal.Common;
using ServicePortal.Common.Filters;

namespace ServicePortal.Applications.Modules.User.Controllers
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

            var response = new BaseResponse<GetUserPersonalInfoResponse>(200, "Success", results);

            return Ok(response);
        }

        [HttpGet("get-all"), RoleAuthorize("HR", "HR_Manager")]
        public async Task<IActionResult> GetAll(GetAllUserRequestDto request)
        {
            var results = await _userService.GetAll(request);

            var response = new PageResponse<GetAllUserResponseDto>(
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

        [HttpGet("get-by-id/{id}"), RoleAuthorize("HR", "HR_Manager")]
        public async Task<IActionResult> GetById(Guid id)
        {
            UserResponseDto? userDTO = await _userService.GetById(id);

            return Ok(new BaseResponse<UserResponseDto>(200, "success", userDTO));
        }

        [HttpGet("get-by-code/{code}"), RoleAuthorize("HR", "HR_Manager")]
        public async Task<IActionResult> GetByCode(string code)
        {
            UserResponseDto? userDTO = await _userService.GetByCode(code);

            return Ok(new BaseResponse<UserResponseDto>(200, "success", userDTO));
        }

        [HttpDelete("delete/{id}"), RoleAuthorize("superadmin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _userService.ForceDelete(id);

            return Ok(new BaseResponse<UserResponseDto>(200, "Delete user successfully", null));
        }

        [HttpDelete("force-delete/{id}"), RoleAuthorize("superadmin")]
        public async Task<IActionResult> ForceDelete(Guid id)
        {
            await _userService.ForceDelete(id);

            return Ok(new BaseResponse<UserResponseDto>(200, "Delete user permanently successfully", null));
        }

        [HttpPost("update-user-role"), RoleAuthorize("superadmin")]
        public async Task<IActionResult> UpdateUserRole(UpdateUserRoleDto dto)
        {
            var result = await _userService.UpdateUserRole(dto);

            return Ok(new BaseResponse<bool>(200, "Success", result));
        }

        [HttpPost("reset-password"), RoleAuthorize("superadmin")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto request)
        {
            var result = await _userService.ResetPassword(request);

            return Ok(new BaseResponse<UserResponseDto>(200, "Success", result));
        }

        [HttpGet("org-chart"), RoleAuthorize("HR", "HR_Manager")]
        public async Task<IActionResult> GetUsersOrgChart([FromQuery(Name = "department_id")] int? departmentId)
        {
            var result = await _userService.BuildTree(departmentId);

            return Ok(new BaseResponse<OrgChartNode>(200, "Success", result));
        }

        [HttpGet("test"), AllowAnonymous]
        public async Task<IActionResult> Test()
        {
            return Ok();
            //var result = await _userService.GetEmailByUserCodeAndUserConfig(new List<string> { "22757" });

            //return Ok(new BaseResponse<List<GetEmailByUserCodeAndUserConfigResponse>>(200, "Success", result));
        }
    }
}