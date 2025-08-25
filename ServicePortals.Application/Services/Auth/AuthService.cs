using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ServicePortal.Infrastructure.Cache;
using ServicePortals.Application.Dtos.Auth.Requests;
using ServicePortals.Application.Interfaces.Auth;
using ServicePortals.Application.Interfaces.User;
using ServicePortals.Domain.Entities;
using ServicePortals.Infrastructure.Data;
using ServicePortals.Infrastructure.Helpers;
using ServicePortals.Infrastructure.Services.Auth;
using ServicePortals.Shared.Exceptions;

namespace ServicePortals.Application.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly JwtService _jwtService;
        private readonly IUserService _userService;
        private readonly IConfiguration _config;
        private readonly ICacheService _cacheService;

        public AuthService(
            JwtService jwtService, 
            ApplicationDbContext context, 
            IConfiguration config, 
            IUserService userService,
            ICacheService cacheService
        )
        {
            _cacheService = cacheService;
            _jwtService = jwtService;
            _context = context;
            _config = config;
            _userService = userService;
        }

        /// <summary>
        /// 
        /// Hàm đăng ký, user phải tồn tại trên viclock thì mới đăng được
        /// 
        /// </summary>
        public async Task<LoginResponse> Register(CreateUserRequest request)
        {
            dynamic? userFromViclock = await _userService.GetCustomColumnUserViclockByUserCode(
                request.UserCode ?? "",
                "NVEmail, NVDienThoai, NVNgaySinh"
            );

            if (userFromViclock == null)
            {
                throw new ValidationException("User is not exists, contact to HR!");
            }

            var userFromWebSystem = await _context.Users.FirstOrDefaultAsync(e => e.UserCode == request.UserCode);

            if (userFromWebSystem != null)
            {
                throw new ValidationException("User is exists!");
            }

            var newUser = new Domain.Entities.User
            {
                UserCode = request.UserCode,
                IsActive = 1,
                IsChangePassword = 0,
                Email = !string.IsNullOrWhiteSpace(userFromViclock.NVEmail) ? userFromViclock.NVEmail : null,
                Phone = !string.IsNullOrWhiteSpace(userFromViclock.NVDienThoai) ? userFromViclock.NVDienThoai : null,
                DateOfBirth = userFromViclock.NVNgaySinh ?? null
            };

            _context.Users.Add(newUser);

            var role = await _context.Roles.FirstOrDefaultAsync(e => e.Code == "USER");

            _context.UserRoles.Add(new UserRole
            {
                UserCode = request.UserCode,
                RoleId = role?.Id,
            });

            await _context.SaveChangesAsync();

            var roleAndPermissions = await _userService.GetRoleAndPermissionByUser(request.UserCode ?? "");
            var formatRoleAndPermission = _userService.FormatRoleAndPermissionByUser(roleAndPermissions);

            var claims = new List<Claim> { new("user_code", request?.UserCode ?? "") };

            claims.AddRange(formatRoleAndPermission.Roles.Select(roleName => new Claim(ClaimTypes.Role, roleName)));
            claims.AddRange(formatRoleAndPermission.Permissions.Select(permissionName => new Claim("permission", permissionName)));

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

            var result = new LoginResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                UserInfo = new
                {
                    newUser?.UserCode,
                    newUser?.IsChangePassword,
                    newUser?.Email,
                    roles = formatRoleAndPermission.Roles,
                    permissions = formatRoleAndPermission.Permissions
                },
                ExpiresAt = refreshTokenEntity.ExpiresAt
            };

            return result;
        }

        /// <summary>
        /// 
        /// Hàm đăng nhập
        /// 
        /// </summary>
        public async Task<LoginResponse> Login(LoginRequest request)
        {
            var userFromWebSystem = await _context.Users.FirstOrDefaultAsync(e => e.UserCode == request.UserCode) ?? throw new ValidationException("User not found!");

            if (!Helper.VerifyString(userFromWebSystem?.Password ?? "", request?.Password ?? ""))
            {
                throw new ValidationException("Password is incorrect!");
            }

            var claims = new List<Claim> { new("user_code", userFromWebSystem?.UserCode ?? "") };

            var roleAndPermissions = await _userService.GetRoleAndPermissionByUser(userFromWebSystem?.UserCode ?? "");
            var formatRoleAndPermission = _userService.FormatRoleAndPermissionByUser(roleAndPermissions);

            claims.AddRange(formatRoleAndPermission.Roles.Select(roleName => new Claim(ClaimTypes.Role, roleName)));
            claims.AddRange(formatRoleAndPermission.Permissions.Select(permissionName => new Claim("permission", permissionName)));

            var accessToken = _jwtService.GenerateAccessToken(claims);
            var refreshToken = _jwtService.GenerateRefreshToken();

            var refreshTokenEntity = new RefreshToken
            {
                Token = refreshToken,
                UserCode = userFromWebSystem?.UserCode ?? "",
                ExpiresAt = DateTimeOffset.UtcNow.AddDays(double.Parse(_config["Jwt:RefreshTokenExpirationDays"] ?? Global.DefaultExpirationDaysRefreshToken)),
                IsRevoked = false,
                CreatedAt = DateTimeOffset.UtcNow
            };

            _context.RefreshTokens.Add(refreshTokenEntity);
            await _context.SaveChangesAsync();

            var result = new LoginResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                UserInfo = new
                {
                    userFromWebSystem?.UserCode,
                    userFromWebSystem?.IsChangePassword,
                    userFromWebSystem?.Email,
                    roles = formatRoleAndPermission.Roles,
                    permissions = formatRoleAndPermission.Permissions
                },
                ExpiresAt = refreshTokenEntity.ExpiresAt
            };

            return result;
        }

        /// <summary>
        /// 
        /// Hàm tạo mới access token, đang set là 15 phút sẽ tự động gọi lấy lại. hàm này khi lấy sẽ bao gồm usercode, role và permission
        /// lưu vào claims để thuận tiện cho việc check role và permission (RoleAuthorize và RoleOrPermissionAuththorize)
        /// 
        /// </summary>
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

            var claims = new List<Claim> {new("user_code", user?.UserCode ?? "")};

            claims.AddRange(userRoleAndPermissions.Roles.Select(roleName => new Claim(ClaimTypes.Role, roleName)));
            claims.AddRange(userRoleAndPermissions.Permissions.Select(permissionName => new Claim("permission", permissionName)));

            var newAccessToken = _jwtService.GenerateAccessToken(claims);

            return newAccessToken;
        }

        /// <summary>
        /// 
        /// Đổi mật khẩu
        /// 
        /// </summary>
        public async Task ChangePassword(ChangePasswordRequest request, string userCode)
        {
            var user = await _context.Users.FirstOrDefaultAsync(e => e.UserCode == userCode) ?? throw new NotFoundException("User not found!");

            user.Password = Helper.HashString(request.NewPassword);
            user.IsChangePassword = 1;

            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                user.Email = request.Email;
                _cacheService.Remove($"user_info_{userCode}");
            }

            _context.Users.Update(user);

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// 
        /// Mỗi khi logout, set refresh token invoke = 1 hết hạn
        /// 
        /// </summary>
        public async Task UpdateRefreshTokenWhenLogout(string refreshToken)
        {
            var token = await _context.RefreshTokens.FirstOrDefaultAsync(x => x.Token == refreshToken) ?? throw new NotFoundException("Refresh Token not found!");

            token.IsRevoked = true;

            _context.RefreshTokens.Update(token);

            _cacheService.Remove($"user_info_{token.UserCode}");

            await _context.SaveChangesAsync();
        }
    }
}
