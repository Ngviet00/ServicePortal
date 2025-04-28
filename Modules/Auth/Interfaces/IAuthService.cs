using ServicePortal.Application.DTOs.Auth.Requests;
using ServicePortal.Application.DTOs.Auth.Responses;
using ServicePortal.Modules.Auth.Requests;
using ServicePortal.Modules.User.DTO;

namespace ServicePortal.Modules.Auth.Interfaces
{
    public interface IAuthService
    {
        Task<UserDTO> Register(CreateUserRequest request);
        Task<LoginResponse> Login(Application.DTOs.Auth.Requests.LoginRequest request);
        Task<string> RefreshAccessToken(string refreshToken);
        Task ChangePassword(ChangePasswordRequest request, string userCode);
        Task UpdateRefreshTokenWhenLogout(string refreshToken);
    }
}
