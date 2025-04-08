namespace ServicePortal.Modules.Auth.Controllers
{
    public class AuthController
    {
    }
}
//using Microsoft.AspNetCore.Mvc;
//using ServicePortal.Application.DTOs.User.Responses;
//using ServicePortal.Application.DTOs;
//using ServicePortal.Application.Services;
//using ServicePortal.Domains.Models;
//using LoginRequest = ServicePortal.Application.DTOs.Auth.Requests.LoginRequest;
//using ServicePortal.Application.DTOs.Auth.Requests;
//using System.Security.Claims;

//namespace ServicePortal.Presentation.Controllers
//{
//    [ApiController]
//    [Route("[controller]")]
//    public class AuthController : ControllerBase
//    {
//        private readonly AuthService _authService;
//        public AuthController(AuthService authService)
//        {
//            _authService = authService;
//        }

//        [HttpPost("/login")]
//        public async Task<IActionResult> Login(LoginRequest request)
//        {
//            var result = await _authService.Login(request);

//            Response.Cookies.Append("refreshToken", result.RefreshToken ?? "", new CookieOptions
//            {
//                HttpOnly = true,
//                Secure = true,
//                Expires = result.ExpiresAt
//            });

//            return Ok(new { accessToken = result?.AccessToken, user = result?.UserInfo });
//        }

//        [HttpPost("/register")]
//        public async Task<IActionResult> Register(User userEntity)
//        {
//            UserResponse? userResponse = await _authService.Register(userEntity);

//            return Ok(new BaseResponse<UserResponse>(200, "Register user successfully", userResponse));
//        }

//        [HttpPost("/logout")]
//        public async Task<IActionResult> Logout()
//        {
//            if (!Request.Cookies.TryGetValue("refreshToken", out var refreshToken))
//            {
//                return Ok(new BaseResponse<string>(200, "Logout successfully", null));
//            }

//            await _authService.UpdateRefreshTokenWhenLogout(refreshToken);

//            Response.Cookies.Delete("refreshToken");

//            return Ok(new BaseResponse<string>(200, "Logout successfully", null));
//        }

//        [HttpPost("/change-password")]
//        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
//        {
//            var employeeCode = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";

//            if (string.IsNullOrWhiteSpace(employeeCode))
//            {
//                return Ok(new BaseResponse<string>(401, "Unauthorized exception", null));
//            }

//            await _authService.ChangePassword(request, employeeCode);

//            return Ok(new BaseResponse<string>(200, "Change password successfully", null));
//        }

//        [HttpGet("/refresh-token")]
//        public async Task<IActionResult> RefreshAccessToken()
//        {
//            Request.Cookies.TryGetValue("refreshToken", out var refreshToken);

//            if (string.IsNullOrWhiteSpace(refreshToken))
//            {
//                return Ok(new BaseResponse<string>(401, "Missing refresh token!", null));
//            }

//            var newAccessToken = await _authService.RefreshAccessToken(refreshToken);

//            return Ok(new BaseResponse<string>(200, "success", newAccessToken));
//        }
//    }
//}