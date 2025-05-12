using ServicePortal.Application.Services;
using ServicePortal.Common.Helpers;
using ServicePortal.Common;
using ServicePortal.Domain.Entities;
using ServicePortal.Infrastructure.Data;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using ServicePortal.Common.Mappers;
using ServicePortal.Modules.User.Services.Interfaces;
using ServicePortal.Modules.User.DTO.Responses;
using ServicePortal.Modules.Auth.Services.Interfaces;
using ServicePortal.Modules.Auth.DTO.Responses;
using ServicePortal.Modules.Auth.DTO.Requests;

namespace ServicePortal.Modules.Auth.Services
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

        public async Task<UserResponseDto> Register(CreateUserDto request)
        {
            if (await _context.Users.AnyAsync(u => u.Code == request.Code && u.DeletedAt == null))
            {
                throw new ValidationException("User is exists!");
            }

            var newUser = new ServicePortal.Domain.Entities.User
            {
                Code = request.Code,
                Name = request.Name,
                Password = Helper.HashString(request.Password),
                Email = request.Email ?? null,
                IsActive = true,
                DateJoinCompany = request.DateJoinCompany ?? null,
                DateOfBirth = request.DateOfBirth ?? null,
                Phone = request.Phone ?? null,
                Sex = request.Sex ?? null,
                DepartmentId = request.DepartmentId,
                Level = request.Level,
                LevelParent = request.LevelParent,
                Position = request.Position,
                CreatedAt = DateTime.Now
            };

            _context.Users.Add(newUser);

            _context.UserRoles.Add(new UserRole
            {
                UserCode = request.Code,
                RoleId = request.RoleId,
                DepartmentId = request.DepartmentId
            });

            await _context.SaveChangesAsync();

            return UserMapper.ToDto(newUser);
        }

        public async Task<LoginResponseDto> Login(LoginRequestDto request)
        {
            var query = _userService.GetUserQueryLogin();

            var user = await query.Where(e => e.Code == request.UserCode).FirstOrDefaultAsync();

            if (user == null)
            {
                throw new ValidationException("User not found!");
            }

            if (!Helper.VerifyString(user?.Password ?? "", request?.Password ?? ""))
            {
                throw new ValidationException("Password is incorrect!");
            }

            var claims = new List<Claim> {
                new("user_code", user?.Code ?? ""),
            };

            claims.AddRange(user!.Roles.Select(r => new Claim(ClaimTypes.Role, r.Code ?? "")));

            var accessToken = _jwtService.GenerateAccessToken(claims);

            var refreshToken = _jwtService.GenerateRefreshToken();

            var refreshTokenEntity = new RefreshToken
            {
                Token = refreshToken,
                UserCode = user?.Code ?? "",
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
                UserInfo = user,
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

            var query = _userService.GetUserQueryLogin();

            var user = await query.Where(e => e.Code == token.UserCode).FirstOrDefaultAsync();

            var claims = new List<Claim> {
                new("user_code", token?.UserCode ?? ""),
            };

            claims.AddRange(user!.Roles.Select(r => new Claim(ClaimTypes.Role, r.Code ?? "")));

            var newAccessToken = _jwtService.GenerateAccessToken(claims);

            return newAccessToken;
        }

        public async Task ChangePassword(ChangePasswordRequestDto request, string userCode)
        {
            var user = await _context.Users.FirstOrDefaultAsync(e => e.Code == userCode) ?? throw new NotFoundException("Not found user!");

            user.Password = Helper.HashString(request.NewPassword);

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
