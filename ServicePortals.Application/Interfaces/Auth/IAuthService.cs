using ServicePortals.Application.Dtos.Auth.Requests;

namespace ServicePortals.Application.Interfaces.Auth
{
    public interface IAuthService
    {
        Task<LoginResponse> Register(CreateUserRequest request);
        Task<LoginResponse> Login(LoginRequest request);
        Task<string> RefreshAccessToken(string refreshToken);
        Task ChangePassword(ChangePasswordRequest request, string userCode);
        Task UpdateRefreshTokenWhenLogout(string refreshToken);
    }
}
