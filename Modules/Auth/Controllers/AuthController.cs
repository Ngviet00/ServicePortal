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
    [ApiController, Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IConfiguration _config;

        public AuthController(IAuthService authService, IConfiguration config)
        {
            _authService = authService;
            _config = config;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var result = await _authService.Login(request);

            Response.Cookies.Append("access_token", result.AccessToken ?? "", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.Now.AddMinutes(_config.GetValue<int>("Jwt:AccessTokenExpirationMinutes")),
            });

            Response.Cookies.Append("refresh_token", result.RefreshToken ?? "", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = result.ExpiresAt
            });

            return Ok(new { accessToken = result?.AccessToken, user = result?.UserInfo });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(CreateUserRequest request)
        {
            UserDTO? userDTO = await _authService.Register(request);

            return Ok(new BaseResponse<UserDTO>(200, "Register user successfully", userDTO));
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var refreshToken = Request.Cookies["refresh_token"];
            var accessToken = Request.Cookies["access_token"];

            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return Ok(new BaseResponse<string>(200, "Logout successfully", null));
            }

            await _authService.UpdateRefreshTokenWhenLogout(refreshToken);

            Response.Cookies.Delete("refresh_token");
            Response.Cookies.Delete("access_token");

            return Ok(new BaseResponse<string>(200, "Logout successfully", null));
        }


        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var employeeCode = User.FindFirstValue("user_code");

            if (string.IsNullOrWhiteSpace(employeeCode))
            {
                return Unauthorized(new BaseResponse<string>(401, "Unauthorized exception", null));
            }

            await _authService.ChangePassword(request, employeeCode);

            return Ok(new BaseResponse<string>(200, "Change password successfully", null));
        }

        [HttpGet("refresh-token")]
        public async Task<IActionResult> RefreshAccessToken()
        {
            var refreshToken = Request.Cookies["refresh_token"] ?? "";

            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return Unauthorized(new BaseResponse<string>(401, "No refresh token!", null));
            }

            var newAccessToken = await _authService.RefreshAccessToken(refreshToken);

            Response.Cookies.Append("access_token", newAccessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.Now.AddMinutes(_config.GetValue<int>("Jwt:AccessTokenExpirationMinutes"))
            });

            return Ok(new BaseResponse<string>(200, "success", newAccessToken));
        }
    }
}