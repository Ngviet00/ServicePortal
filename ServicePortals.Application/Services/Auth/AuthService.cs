using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ServicePortals.Application.Dtos.Auth.Requests;
using ServicePortals.Application.Dtos.User.Responses;
using ServicePortals.Application.Interfaces.Auth;
using ServicePortals.Application.Interfaces.User;
using ServicePortals.Domain.Entities;
using ServicePortals.Infrastructure.Data;
using ServicePortals.Infrastructure.Helpers;
using ServicePortals.Shared.Exceptions;

namespace ServicePortals.Infrastructure.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly JwtService _jwtService;
        private readonly IUserService _userService;
        private readonly IConfiguration _config;

        public AuthService(JwtService jwtService, ApplicationDbContext context, IConfiguration config, IUserService userService)
        {
            _jwtService = jwtService;
            _context = context;
            _config = config;
            _userService = userService;
        }

        public async Task<LoginResponse> Register(CreateUserRequest request)
        {
            GetDetailInfoUserResponse detailUser = await _userService.GetDetailUserWithRoleAndPermission(request.UserCode ?? "");
            
            if (detailUser.InfoFromViclock == null)
            {
                throw new ValidationException("User is not exists, contact to HR!");
            }

            if (detailUser.User != null)
            {
                throw new ValidationException("User is exists!");
            }

            var newUser = new Domain.Entities.User
            {
                UserCode = request.UserCode,
                Password = Helper.HashString(request.Password),
                IsActive = 1,
                IsChangePassword = 0,
                Email = !string.IsNullOrWhiteSpace(detailUser.InfoFromViclock.NVEmail) ? detailUser.InfoFromViclock.NVEmail : null,
                Phone = !string.IsNullOrWhiteSpace(detailUser.InfoFromViclock.NVDienThoai) ? detailUser.InfoFromViclock.NVDienThoai : null,
                DateOfBirth = detailUser.InfoFromViclock.NVNgaySinh != null ? detailUser.InfoFromViclock.NVNgaySinh : null
            };

            _context.Users.Add(newUser);

            var role = await _context.Roles.FirstOrDefaultAsync(e => e.Code == "USER");

            _context.UserRoles.Add(new UserRole
            {
                UserCode = request.UserCode,
                RoleId = role?.Id,
            });

            await _context.SaveChangesAsync();

            var claims = new List<Claim> { new("user_code", request?.UserCode ?? "") };

            claims.AddRange(detailUser.Roles.Select(roleName => new Claim(ClaimTypes.Role, roleName)));
            claims.AddRange(detailUser.Permissions.Select(permissionName => new Claim("permission", permissionName)));

            var accessToken = _jwtService.GenerateAccessToken(claims);
            var refreshToken = _jwtService.GenerateRefreshToken();

            var refreshTokenEntity = new RefreshToken
            {
                Token = refreshToken,
                UserCode = newUser?.UserCode ?? "",
                ExpiresAt = DateTimeOffset.UtcNow.AddDays(double.Parse(_config["Jwt:RefreshTokenExpirationDays"] ?? Global.DefaultExpirationDaysRefreshToken)),
                IsRevoked = false,
                CreatedAt = DateTimeOffset.UtcNow
            };

            _context.RefreshTokens.Add(refreshTokenEntity);

            await _context.SaveChangesAsync();

            if (detailUser.User != null)
            {
                detailUser.User.Password = null;
            }

            var result = new LoginResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                UserInfo = detailUser,
                ExpiresAt = refreshTokenEntity.ExpiresAt
            };

            return result;
        }

        public async Task<LoginResponse> Login(LoginRequest request)
        {
            GetDetailInfoUserResponse detailInfoUser = new();

            detailInfoUser = await _userService.GetDetailUserWithRoleAndPermission(request.UserCode ?? "");

            if (detailInfoUser.User == null)
            {
                throw new ValidationException("User not found!");
            }

            var user = detailInfoUser.User;

            if (!Helper.VerifyString(user?.Password ?? "", request?.Password ?? ""))
            {
                throw new ValidationException("Password is incorrect!");
            }

            var claims = new List<Claim> { new("user_code", user?.UserCode ?? "") };

            claims.AddRange(detailInfoUser.Roles.Select(roleName => new Claim(ClaimTypes.Role, roleName)));
            claims.AddRange(detailInfoUser.Permissions.Select(permissionName => new Claim("permission", permissionName)));

            var accessToken = _jwtService.GenerateAccessToken(claims);
            var refreshToken = _jwtService.GenerateRefreshToken();

            var refreshTokenEntity = new RefreshToken
            {
                Token = refreshToken,
                UserCode = user?.UserCode ?? "",
                ExpiresAt = DateTimeOffset.UtcNow.AddDays(double.Parse(_config["Jwt:RefreshTokenExpirationDays"] ?? Global.DefaultExpirationDaysRefreshToken)),
                IsRevoked = false,
                CreatedAt = DateTimeOffset.UtcNow
            };

            await _context.RefreshTokens.AddAsync(refreshTokenEntity);
            await _context.SaveChangesAsync();

            detailInfoUser.User.Password = null;

            var result = new LoginResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                UserInfo = detailInfoUser,
                ExpiresAt = refreshTokenEntity.ExpiresAt
            };

            return result;
        }

        public async Task<string> RefreshAccessToken(string refreshToken)
        {
            var token = await _context.RefreshTokens.FirstOrDefaultAsync(x => x.Token == refreshToken && x.IsRevoked == false && x.ExpiresAt > DateTimeOffset.UtcNow);

            if (token == null)
            {
                throw new UnauthorizedException("Invalid refresh token");
            }

            var user = await _userService.GetRoleAndPermissionByUser(token.UserCode ?? "");

            if (user == null)
            {
                return "";
            }

            var userRoleAndPermissions = _userService.FormatRoleAndPermissionByUser(user);

            user!.Password = null;

            var claims = new List<Claim> {new("user_code", user?.UserCode ?? "")};

            claims.AddRange(userRoleAndPermissions.Roles.Select(roleName => new Claim(ClaimTypes.Role, roleName)));
            claims.AddRange(userRoleAndPermissions.Permissions.Select(permissionName => new Claim("permission", permissionName)));

            var newAccessToken = _jwtService.GenerateAccessToken(claims);

            return newAccessToken;
        }

        public async Task ChangePassword(ChangePasswordRequest request, string userCode)
        {
            var user = await _context.Users.FirstOrDefaultAsync(e => e.UserCode == userCode) ?? throw new NotFoundException("User not found!");

            user.Password = Helper.HashString(request.NewPassword);
            user.IsChangePassword = 1;

            _context.Users.Update(user);

            await _context.SaveChangesAsync();
        }

        public async Task UpdateRefreshTokenWhenLogout(string refreshToken)
        {
            var token = await _context.RefreshTokens.FirstOrDefaultAsync(x => x.Token == refreshToken) ?? throw new NotFoundException("Refresh Token not found!");

            token.IsRevoked = true;

            _context.RefreshTokens.Update(token);

            await _context.SaveChangesAsync();
        }
    }
}
