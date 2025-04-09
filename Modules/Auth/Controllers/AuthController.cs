using Microsoft.AspNetCore.Mvc;
using LoginRequest = ServicePortal.Application.DTOs.Auth.Requests.LoginRequest;
using ServicePortal.Application.DTOs.Auth.Requests;
using System.Security.Claims;
using ServicePortal.Common;
using ServicePortal.Modules.Auth.Requests;
using ServicePortal.Modules.User.DTO;
using ServicePortal.Modules.Auth.Interfaces;

namespace ServicePortal.Modules.Auth.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("/login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var result = await _authService.Login(request);

            Response.Cookies.Append("refreshToken", result.RefreshToken ?? "", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = result.ExpiresAt
            });

            return Ok(new { accessToken = result?.AccessToken, user = result?.UserInfo });
        }

        [HttpPost("/register")]
        public async Task<IActionResult> Register(CreateUserRequest request)
        {
            UserDTO? userDTO = await _authService.Register(request);

            return Ok(new BaseResponse<UserDTO>(200, "Register user successfully", userDTO));
        }

        [HttpPost("/logout")]
        public async Task<IActionResult> Logout()
        {
            var refreshToken = Request.Cookies["refreshToken"];

            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return Ok(new BaseResponse<string>(200, "Logout successfully", null));
            }

            await _authService.UpdateRefreshTokenWhenLogout(refreshToken);

            Response.Cookies.Delete("refreshToken");

            return Ok(new BaseResponse<string>(200, "Logout successfully", null));
        }

        [HttpPost("/change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var employeeCode = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0";

            if (string.IsNullOrWhiteSpace(employeeCode))
            {
                return Ok(new BaseResponse<string>(401, "Unauthorized exception", null));
            }

            await _authService.ChangePassword(request, employeeCode);

            return Ok(new BaseResponse<string>(200, "Change password successfully", null));
        }

        [HttpGet("/refresh-token")]
        public async Task<IActionResult> RefreshAccessToken()
        {
            var refreshToken = Request.Cookies["refreshToken"] ?? "";

            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return Ok(new BaseResponse<string>(401, "Missing refresh token!", null));
            }

            var newAccessToken = await _authService.RefreshAccessToken(refreshToken);

            return Ok(new BaseResponse<string>(200, "success", newAccessToken));
        }
    }
}