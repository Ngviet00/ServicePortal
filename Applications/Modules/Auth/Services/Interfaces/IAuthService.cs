﻿using ServicePortal.Applications.Modules.Auth.DTO.Requests;
using ServicePortal.Applications.Modules.Auth.DTO.Responses;

namespace ServicePortal.Applications.Modules.Auth.Services.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDto> Register(CreateUserDto request);
        Task<LoginResponseDto> Login(LoginRequestDto request);
        Task<string> RefreshAccessToken(string refreshToken);
        Task ChangePassword(ChangePasswordRequestDto request, string userCode);
        Task UpdateRefreshTokenWhenLogout(string refreshToken);
    }
}
