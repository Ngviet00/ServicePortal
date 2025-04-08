using AutoMapper;
using ServicePortal.Application.DTOs.Auth.Requests;
using ServicePortal.Application.DTOs.Auth.Responses;
using ServicePortal.Application.Services;
using ServicePortal.Common.Helpers;
using ServicePortal.Common;
using ServicePortal.Domain.Entities;
using ServicePortal.Infrastructure.Data;
using ServicePortal.Modules.User.Responses;
using ServicePortal.Modules.User.Services;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace ServicePortal.Modules.Auth.Services
{
    public class AuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly JwtService _jwtService;
        private readonly UserService _userService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;

        public AuthService(JwtService jwtService, UserService userService, ApplicationDbContext context, IMapper mapper, IConfiguration config)
        {
            _jwtService = jwtService;
            _userService = userService;
            _context = context;
            _mapper = mapper;
            _config = config;
        }

        public async Task<UserResponse> Register(ServicePortal.Domain.Entities.User user)
        {
            if (await _context.Users.AnyAsync(u => u.Email == user.Email || u.Name == user.Name || u.Code == user.Code))
            {
                throw new ValidationException("Email already in use!");
            }

            user.Password = Helper.HashString(user.Password);
            user.CreatedAt = DateTime.UtcNow;

            _context.Users.Add(user);

            await _context.SaveChangesAsync();

            user.Password = null;

            return _mapper.Map<UserResponse>(user);
        }

        public async Task<LoginResponse> Login(LoginRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Code == request.UserCode) ?? throw new NotFoundException("User not found!");

            if (!Helper.VerifyString(user?.Password ?? "", request?.Password ?? ""))
            {
                throw new ValidationException("Password is incorrect!");
            }

            var claims = new List<Claim> {
                new(ClaimTypes.NameIdentifier, user?.Code ?? "")
            };

            var accessToken = _jwtService.GenerateAccessToken(claims);

            var refreshToken = _jwtService.GenerateRefreshToken();

            var refreshTokenEntity = new RefreshToken
            {
                Token = refreshToken,
                UserCode = user?.Code ?? "",
                ExpiresAt = DateTime.UtcNow.AddDays(_config.GetValue<int>("Jwt:RefreshTokenExpirationDays"))
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
            var token = await _context.RefreshTokens.FirstOrDefaultAsync(x => x.Token == refreshToken && x.IsRevoked == true && x.ExpiresAt > DateTime.UtcNow);

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

            await _context.SaveChangesAsync();
        }
    }
}
