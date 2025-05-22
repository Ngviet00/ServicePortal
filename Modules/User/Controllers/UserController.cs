using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicePortal.Common;
using ServicePortal.Common.Filters;
using ServicePortal.Infrastructure.Data;
using ServicePortal.Modules.User.DTO.Requests;
using ServicePortal.Modules.User.DTO.Responses;
using ServicePortal.Modules.User.Services;
using ServicePortal.Modules.User.Services.Interfaces;

namespace ServicePortal.Modules.User.Controllers
{
    [Authorize]
    [ApiController, Route("api/user"), RoleAuthorize("HR", "HR_Manager")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        private readonly ApplicationDbContext _context;

        public UserController(IUserService userService, ApplicationDbContext context)
        {
            _userService = userService;
            _context = context;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            var currentUserCode = User.FindFirst("user_code")?.Value;

            var results = await _userService.GetMe(currentUserCode ?? "");

            var response = new BaseResponse<UserResponseDto>(200, "Success", results);

            return Ok(response);
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll(GetAllUserRequestDto request)
        {
            var results = await _userService.GetAll(request);

            var response = new PageResponse<UserResponseDto>(
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
            UserResponseDto? userDTO = await _userService.GetById(id);

            return Ok(new BaseResponse<UserResponseDto>(200, "success", userDTO));
        }

        [HttpGet("get-by-code/{code}")]
        public async Task<IActionResult> GetByCode(string code)
        {
            UserResponseDto? userDTO = await _userService.GetByCode(code);

            return Ok(new BaseResponse<UserResponseDto>(200, "success", userDTO));
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _userService.ForceDelete(id);

            return Ok(new BaseResponse<UserResponseDto>(200, "Delete user successfully", null));
        }

        [HttpDelete("force-delete/{id}")]
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

        [HttpGet("org-chart")]
        public async Task<IActionResult> GetUsersOrgChart([FromQuery(Name = "department_id")] int? departmentId)
        {

            var result = await _userService.BuildTree(departmentId);

            return Ok(new BaseResponse<OrgChartNode>(200, "Success", result));
        }   
    }
}