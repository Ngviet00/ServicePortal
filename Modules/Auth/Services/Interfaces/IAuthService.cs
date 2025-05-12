using ServicePortal.Modules.Auth.DTO.Requests;
using ServicePortal.Modules.Auth.DTO.Responses;
using ServicePortal.Modules.User.DTO.Responses;

namespace ServicePortal.Modules.Auth.Services.Interfaces
{
    public interface IAuthService
    {
        Task<UserResponseDto> Register(CreateUserDto request);
        Task<LoginResponseDto> Login(LoginRequestDto request);
        Task<string> RefreshAccessToken(string refreshToken);
        Task ChangePassword(ChangePasswordRequestDto request, string userCode);
        Task UpdateRefreshTokenWhenLogout(string refreshToken);
    }
}
