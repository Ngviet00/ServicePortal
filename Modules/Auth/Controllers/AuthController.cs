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

            return Ok(new 
            {
                user = result?.UserInfo,
                accessToken = result?.AccessToken,
                refreshToken = result?.RefreshToken,
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(CreateUserDto request)
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