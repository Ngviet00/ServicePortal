namespace ServicePortal.Modules.Auth.Responses
{
    public class AuthService
    {
    }
}


//using ServicePortal.Application.DTOs.User.Responses;
//using ServicePortal.Domains.Models;
//using Microsoft.EntityFrameworkCore;
//using ServicePortal.Infrastructure.Data;
//using AutoMapper;
//using ServicePortal.Application.DTOs.Auth.Requests;
//using ServicePortal.Application.Exceptions;
//using System.Security.Claims;
//using ServicePortal.Application.DTOs.Auth.Responses;
//using ServicePortal.Domain.Models;

//namespace ServicePortal.Application.Services
//{
//    public class AuthService
//    {
//        private readonly ApplicationDbContext _context;
//        private readonly JwtService _jwtService;
//        private readonly UserService _userService;
//        private readonly IMapper _mapper;

//        public AuthService(JwtService jwtService, UserService userService, ApplicationDbContext context, IMapper mapper)
//        {
//            _jwtService = jwtService;
//            _userService = userService;
//            _context = context;
//            _mapper = mapper;
//        }

//        //create new user
//        public async Task<UserResponse> Register(User user)
//        {
//            if (await _context.User.AnyAsync(u => u.Email == user.Email || u.Name == user.Name || u.EmployeeCode == user.EmployeeCode))
//            {
//                throw new ValidationException("Email already in use!");
//            }

//            user.Password = Helper.Hash(user.Password);
//            user.CreatedAt = DateTime.UtcNow;

//            _context.User.Add(user);

//            await _context.SaveChangesAsync();

//            user.Password = null;

//            return _mapper.Map<UserResponse>(user);
//        }


//        public async Task<LoginResponse> Login(LoginRequest request)
//        {
//            var user = await _context.User.FirstOrDefaultAsync(u => u.EmployeeCode == request.EmployeeCode) ?? throw new NotFoundException("User not found!");

//            if (!Helper.Verify(user?.Password ?? "", request?.Password ?? ""))
//            {
//                throw new ValidationException("Password is incorrect!");
//            }

//            var claims = new List<Claim> {
//                new(ClaimTypes.NameIdentifier, user?.EmployeeCode ?? "") 
//            };

//            var accessToken = _jwtService.GenerateAccessToken(claims);

//            var refreshToken = _jwtService.GenerateRefreshToken();

//            var refreshTokenEntity = new RefreshToken
//            {
//                Token = refreshToken,
//                EmployeeCode = user?.EmployeeCode ?? "",
//                ExpiresAt = DateTime.UtcNow.AddDays(7)
//            };

//            await _context.RefreshTokens.AddAsync(refreshTokenEntity);
//            await _context.SaveChangesAsync();

//            user.Password = null;

//            var result = new LoginResponse
//            {
//                AccessToken = accessToken,
//                RefreshToken = refreshToken,
//                UserInfo = user,
//                ExpiresAt = refreshTokenEntity.ExpiresAt
//            };

//            return result;
//        }

//        public async Task<string> RefreshAccessToken(string refreshToken)
//        {
//            var token = await _context.RefreshTokens.FirstOrDefaultAsync(x => x.Token == refreshToken && !x.IsRevoked && x.ExpiresAt > DateTime.UtcNow);

//            if (token == null)
//            {
//                throw new UnauthorizedException("Invalid refresh token");
//            }

//            var claims = new List<Claim> {
//                new(ClaimTypes.NameIdentifier, token.EmployeeCode)
//            };

//            var newAccessToken = _jwtService.GenerateAccessToken(claims);

//            return newAccessToken;
//        }

//        public async Task ChangePassword(ChangePasswordRequest request, string employeeCode)
//        {
//            var user = await _context.User.FirstOrDefaultAsync(e => e.EmployeeCode == employeeCode) ?? throw new NotFoundException("Not found user!");

//            user.Password = Helper.Hash(request.NewPassword);

//            _context.User.Update(user);
//            await _context.SaveChangesAsync();
//        }

//        public async Task UpdateRefreshTokenWhenLogout(string refreshToken)
//        {
//            var token = await _context.RefreshTokens.FirstOrDefaultAsync(x => x.Token == refreshToken);

//            if (token is null)
//            {
//                throw new NotFoundException("Refresh Token not found!");
//            }

//            token.IsRevoked = true;
//            await _context.SaveChangesAsync();
//        }
//    }
//}
