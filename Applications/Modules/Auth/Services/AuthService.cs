using ServicePortal.Common.Helpers;
using ServicePortal.Common;
using ServicePortal.Domain.Entities;
using ServicePortal.Infrastructure.Data;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using ServicePortal.Common.Mappers;
using ServicePortal.Applications.Modules.Auth.DTO.Requests;
using ServicePortal.Applications.Modules.Auth.DTO.Responses;
using ServicePortal.Applications.Modules.Auth.Services.Interfaces;
using ServicePortal.Applications.Modules.User.Services.Interfaces;

namespace ServicePortal.Applications.Modules.Auth.Services
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

        public async Task<LoginResponseDto> Register(CreateUserDto request)
        {
            if (string.IsNullOrWhiteSpace(request.UserCode))
            {
                throw new ValidationException("Usercode is required");
            }

            //check exist in viclock
            if (await _userService.CheckUserIsExistsInViClock(request.UserCode ?? "") == false)
            {
                throw new ValidationException("User is not exists, contact to HR!");
            }

            var currentUser = await _context.Users
                .Where(e => e.UserCode == request.UserCode)
                .FirstOrDefaultAsync();

            if (currentUser != null)
            {
                throw new ValidationException("User is exists!");
            }

            var newUser = new Domain.Entities.User
            {
                UserCode = request.UserCode,
                Password = Helper.HashString(request.Password),
                IsActive = 1, //true
                IsChangePassword = 0, //false
            };

            _context.Users.Add(newUser);

            //role default user
            var roleUser = await _context.Roles.FirstOrDefaultAsync(e => e.Code == "user");

            _context.UserRoles.Add(new UserRole
            {
                UserCode = request.UserCode,
                RoleId = roleUser?.Id,
            });

            await _context.SaveChangesAsync();

            var createdUser = await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.UserCode == request.UserCode);

            var claims = new List<Claim> { new("user_code", createdUser?.UserCode ?? "") };


            if (createdUser != null)
            {
                claims.AddRange(
                    createdUser.UserRoles.Select(ur =>
                        new Claim(ClaimTypes.Role, ur.Role?.Code ?? "")
                    )
                );
            }

            var accessToken = _jwtService.GenerateAccessToken(claims);

            var refreshToken = _jwtService.GenerateRefreshToken();

            var refreshTokenEntity = new RefreshToken
            {
                Token = refreshToken,
                UserCode = createdUser?.UserCode ?? "",
                ExpiresAt = DateTimeOffset.UtcNow.AddDays(_config.GetValue<int>("Jwt:RefreshTokenExpirationDays")),
                IsRevoked = false,
                CreatedAt = DateTimeOffset.UtcNow
            };

            _context.RefreshTokens.Add(refreshTokenEntity);

            createdUser!.Password = null;

            await _context.SaveChangesAsync();

            var result = new LoginResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                UserInfo = UserMapper.ToDto(createdUser),
                ExpiresAt = refreshTokenEntity.ExpiresAt
            };

            return result;
        }

        public async Task<LoginResponseDto> Login(LoginRequestDto request)
        {
            var user = await _context.Users
                .Include(ur => ur.UserRoles)
                    .ThenInclude(r => r.Role)
                .Where(e => e.UserCode == request.UserCode)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                throw new ValidationException("User not found!");
            }

            if (!Helper.VerifyString(user?.Password ?? "", request?.Password ?? ""))
            {
                throw new ValidationException("Password is incorrect!");
            }

            var claims = new List<Claim> { new("user_code", user?.UserCode ?? "")};

            claims.AddRange(user!.UserRoles.Select(ur => new Claim(ClaimTypes.Role, ur.Role?.Code ?? "")));

            var accessToken = _jwtService.GenerateAccessToken(claims);

            var refreshToken = _jwtService.GenerateRefreshToken();

            var refreshTokenEntity = new RefreshToken
            {
                Token = refreshToken,
                UserCode = user?.UserCode ?? "",
                ExpiresAt = DateTimeOffset.UtcNow.AddDays(_config.GetValue<int>("Jwt:RefreshTokenExpirationDays")),
                IsRevoked = false,
                CreatedAt = DateTimeOffset.UtcNow
            };

            await _context.RefreshTokens.AddAsync(refreshTokenEntity);
            await _context.SaveChangesAsync();

            user!.Password = null;

            var result = new LoginResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                UserInfo = UserMapper.ToDto(user),
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

            var user = await _context.Users
                .Include(ur => ur.UserRoles)
                    .ThenInclude(r => r.Role)
                .Where(e => e.UserCode == token.UserCode)
                .FirstOrDefaultAsync();

            var claims = new List<Claim> {new("user_code", user?.UserCode ?? "")};

            claims.AddRange(user!.UserRoles.Select(r => new Claim(ClaimTypes.Role, r.Role?.Code ?? "")));

            var newAccessToken = _jwtService.GenerateAccessToken(claims);

            return newAccessToken;
        }

        public async Task ChangePassword(ChangePasswordRequestDto request, string userCode)
        {
            var user = await _context.Users.FirstOrDefaultAsync(e => e.UserCode == userCode) ?? throw new NotFoundException("User not found!");

            user.Password = Helper.HashString(request.NewPassword);
            user.IsChangePassword = 1;

            _context.Users.Update(user);

            await _context.SaveChangesAsync();
        }

        public async Task UpdateRefreshTokenWhenLogout(string refreshToken)
        {
            var token = await _context.RefreshTokens.FirstOrDefaultAsync(x => x.Token == refreshToken);

            if (token is null)
            {
                throw new NotFoundException("Refresh Token not found!");
            }

            token.IsRevoked = true;

            _context.RefreshTokens.Update(token);

            await _context.SaveChangesAsync();
        }
    }
}
