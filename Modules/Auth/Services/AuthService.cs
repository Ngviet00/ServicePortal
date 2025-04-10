using ServicePortal.Application.DTOs.Auth.Requests;
using ServicePortal.Application.DTOs.Auth.Responses;
using ServicePortal.Application.Services;
using ServicePortal.Common.Helpers;
using ServicePortal.Common;
using ServicePortal.Domain.Entities;
using ServicePortal.Infrastructure.Data;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using ServicePortal.Modules.Auth.Requests;
using ServicePortal.Modules.User.DTO;
using ServicePortal.Modules.Auth.Interfaces;
using ServicePortal.Domain.Enums;
using ServicePortal.Common.Mappers;

namespace ServicePortal.Modules.Auth.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly JwtService _jwtService;
        private readonly IConfiguration _config;

        public AuthService(JwtService jwtService, ApplicationDbContext context, IConfiguration config)
        {
            _jwtService = jwtService;
            _context = context;
            _config = config;
        }

        public async Task<UserDTO> Register(CreateUserRequest request)
        {
            if (await _context.Users.AnyAsync(u => u.Code == request.Code))
            {
                throw new ValidationException("User is exists!");
            }

            if (!string.IsNullOrWhiteSpace(request.Email) && await _context.Users.AnyAsync(u => u.Email == request.Email))
            {
                throw new ValidationException("Email already in use!");
            }

            var newUser = new ServicePortal.Domain.Entities.User
            {
                Code = request.Code,
                Name = request.Name,
                Password = Helper.HashString(request.Password),
                Email = request.Email ?? null,
                RoleId = request.RoleId,
                IsActive = true,
                DateJoinCompany = request.DateJoinCompany ?? null,
                CreatedAt = DateTime.Now
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            var positionDeparment = new PositionDeparment
            {
                DeparmentId = request.DeparmentId,
                PositionId = request.PositionId,
                PositionDeparmentLevel = request.PositionDeparmentLevel,
            };
            _context.PositionDeparments.Add(positionDeparment);
            await _context.SaveChangesAsync();

            var userAssignment = new UserAssignment
            {
                UserCode = request.Code,
                PositionDeparmentId = request.PositionId,
            };

            _context.UserAssignments.Add(userAssignment);
            await _context.SaveChangesAsync();

            return UserMapper.ToDto(newUser);
        }

        public async Task<LoginResponse> Login(LoginRequest request)
        {
            //get info user include role
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Code == request.UserCode) ?? throw new NotFoundException("User not found!");

            if (!Helper.VerifyString(user?.Password ?? "", request?.Password ?? ""))
            {
                throw new UnauthorizedException("Password is incorrect!");
            }

            var claims = new List<Claim> {
                new(ClaimTypes.Name, user?.Code ?? ""),
                new("role", RoleEnum.SuperAdmin.ToString()), //fake
            };

            var accessToken = _jwtService.GenerateAccessToken(claims);

            var refreshToken = _jwtService.GenerateRefreshToken();

            var refreshTokenEntity = new RefreshToken
            {
                Token = refreshToken,
                UserCode = user?.Code ?? "",
                ExpiresAt = DateTime.Now.AddDays(_config.GetValue<int>("Jwt:RefreshTokenExpirationDays")),
                IsRevoked = false,
                CreatedAt = DateTime.Now
            };

            await _context.RefreshTokens.AddAsync(refreshTokenEntity);
            await _context.SaveChangesAsync();

            user!.Password = null;

            var result = new LoginResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                UserInfo = user,
                ExpiresAt = (DateTime)refreshTokenEntity.ExpiresAt
            };

            return result;
        }

        public async Task<string> RefreshAccessToken(string refreshToken)
        {
            var token = await _context.RefreshTokens.FirstOrDefaultAsync(x => x.Token == refreshToken && x.IsRevoked == false && x.ExpiresAt > DateTime.UtcNow);

            if (token == null)
            {
                throw new UnauthorizedException("Invalid refresh token");
            }

            var claims = new List<Claim> {
                new(ClaimTypes.NameIdentifier, token.UserCode ?? "")
            };

            var newAccessToken = _jwtService.GenerateAccessToken(claims);

            return newAccessToken;
        }

        public async Task ChangePassword(ChangePasswordRequest request, string userCode)
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
