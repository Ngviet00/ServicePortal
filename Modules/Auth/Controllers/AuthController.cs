using Microsoft.AspNetCore.Mvc;
using LoginRequestDto = ServicePortal.Modules.Auth.DTO.Requests.LoginRequestDto;
using System.Security.Claims;
using ServicePortal.Common;
using Microsoft.AspNetCore.Authorization;
using ServicePortal.Modules.Auth.Services.Interfaces;
using ServicePortal.Modules.Auth.DTO.Requests;

namespace ServicePortal.Modules.Auth.Controllers
{
    [ApiController, Route("api/auth")]
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
        public async Task<IActionResult> Login(LoginRequestDto request)
        {
            var result = await _authService.Login(request);

            Response.Cookies.Append("access_token", result.AccessToken ?? "", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddMinutes(_config.GetValue<int>("Jwt:AccessTokenExpirationMinutes")),
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
        public async Task<IActionResult> Register(CreateUserDto request)
        {
            var result = await _authService.Register(request);

            Response.Cookies.Append("access_token", result.AccessToken ?? "", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddMinutes(_config.GetValue<int>("Jwt:AccessTokenExpirationMinutes")),
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

        [HttpPost("logout"), Authorize]
        public async Task<IActionResult> Logout()
        {
            var refreshToken = Request.Cookies["refresh_token"];
            var accessToken = Request.Cookies["access_token"];

            Response.Cookies.Delete("access_token", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = "/",
            });

            Response.Cookies.Delete("refresh_token", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = "/",
            });

            if (string.IsNullOrWhiteSpace(refreshToken) || string.IsNullOrWhiteSpace(accessToken))
            {
                return Ok(new BaseResponse<string>(200, "Logout successfully", null));
            }

            await _authService.UpdateRefreshTokenWhenLogout(refreshToken);

            return Ok(new BaseResponse<string>(200, "Logout successfully", null));
        }


        [HttpPost("change-password"), Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto request)
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
                Expires = DateTimeOffset.UtcNow.AddMinutes(_config.GetValue<int>("Jwt:AccessTokenExpirationMinutes"))
            });

            return Ok(new BaseResponse<string>(200, "success", newAccessToken));
        }
    }
}