using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicePortal.Filters;
using ServicePortals.Application;
using ServicePortals.Application.Dtos.User.Requests;
using ServicePortals.Application.Dtos.User.Responses;
using ServicePortals.Application.Interfaces.User;
using ServicePortals.Domain.Entities;
using ServicePortals.Infrastructure.Excel;

namespace ServicePortal.Controllers.User
{
    [Authorize]
    [ApiController, Route("api/user")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ExcelService _excelService;

        public UserController(
            IUserService userService,
            ExcelService excelService
        )
        {
            _userService = userService;
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

        [HttpPost("import-excel-test-insert-new-user"), AllowAnonymous]
        public async Task<IActionResult> ImportExcelTestInsertNewUser(IFormFile file)
        {
            await _excelService.InsertFromExcelAsync(file);
            return Ok("Nhập dữ liệu thành công");
        }

        [HttpGet("search-user-combine-viclock-and-web-system"), AllowAnonymous]
        public async Task<IActionResult> SearchUserCombineViClockAndWebSystem([FromQuery] string userCode)
        {
            var result = await _userService.SearchUserCombineViClockAndWebSystem(userCode);

            return Ok(new BaseResponse<PersonalInfoResponse>(200, "Success", result));
        }

        [HttpGet("test"), AllowAnonymous]
        public async Task<IActionResult> Test()
        {
            var requestIds = new[] { 6 };

            // Giả sử dbSteps là list từ DB
            var dbSteps = new List<ApprovalFlow>
            {
                new ApprovalFlow { Id = 9, Condition = "{ \"it_category\": [ { \"id\": 1 }, { \"id\": 2 }, { \"id\": 3 } ] }", ToOrgPositionId = 5 },
                new ApprovalFlow { Id = 11, Condition = null, ToOrgPositionId = 7 }
            };

            var finalStep = dbSteps
                .FirstOrDefault(step => !string.IsNullOrEmpty(step.Condition) &&
                    JsonDocument.Parse(step.Condition)
                        .RootElement.GetProperty("it_category")
                        .EnumerateArray()
                        .Select(e => e.GetProperty("id").GetInt32())
                        .Any(id => requestIds.Contains(id)))
                ?? dbSteps.FirstOrDefault(step => step.Condition == null);

            // Nếu không match, lấy step condition null
            //var finalStep = matchedStep ?? dbSteps.FirstOrDefault(s => s.Condition == null);
            //var requestIds = new[] { 1, 3 };

            ////var results = await _userService.Test();

            //List<ServicePortals.Domain.Entities.ApprovalFlow> approvalFlows = await _userService.Test();

            //foreach (var step in approvalFlows)
            //{
            //    if (string.IsNullOrEmpty(step.Condition)) continue;

            //    var jsonDoc = JsonDocument.Parse(step.Condition);
            //    if (!jsonDoc.RootElement.TryGetProperty("it_category", out var itCategories)) continue;

            //    var idsInStep = itCategories.EnumerateArray()
            //        .Select(e => e.GetProperty("id").GetInt32())
            //        .ToList();

            //    if (requestIds.Any(id => idsInStep.Contains(id)))
            //    {
            //        matchedStep = step;
            //        break;
            //    }
            //}

            //// Nếu không match, lấy step condition null
            //var finalStep = matchedStep ?? dbSteps.FirstOrDefault(s => s.Condition == null);

            return Ok(finalStep);
        }
    }
}