using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using ServicePortals.Application.Interfaces.Auth;
using ServicePortals.Application.Dtos.Auth.Requests;
using ServicePortals.Application;

namespace ServicePortal.Controllers.Auth
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
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var result = await _authService.Login(request);

            return Ok(new 
            {
                user = result?.UserInfo,
                accessToken = result?.AccessToken,
                refreshToken = result?.RefreshToken,
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(CreateUserRequest request)
        {
            var result = await _authService.Register(request);

            return Ok(new
            {
                accessToken = result?.AccessToken,
                refreshToken = result?.RefreshToken,
                user = result?.UserInfo
            });
        }

        [HttpPost("logout"), Authorize]
        public async Task<IActionResult> Logout([FromBody] LogoutRequest request)
        {
            var refreshToken = request.RefreshToken;

            var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (string.IsNullOrWhiteSpace(refreshToken) || string.IsNullOrWhiteSpace(accessToken))
            {
                return Ok(new BaseResponse<string>(200, "Logout successfully", null));
            }

            await _authService.UpdateRefreshTokenWhenLogout(refreshToken);

            return Ok(new BaseResponse<string>(200, "Logout successfully", null));
        }


        [HttpPost("change-password"), Authorize]
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

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshAccessToken([FromBody] RefreshTokenRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.RefreshToken))
            {
                return Unauthorized(new BaseResponse<string>(401, "No refresh token!", null));
            }

            var newAccessToken = await _authService.RefreshAccessToken(request.RefreshToken);

            return Ok(new BaseResponse<string>(200, "success", newAccessToken));
        }
    }
}