namespace ServicePortal.Modules.Auth.Services
{
    public class JwtService
    {
    }
}


//using System.IdentityModel.Tokens.Jwt;
//using System.Security.Claims;
//using Microsoft.IdentityModel.Tokens;
//using System.Text;
//using System.Security.Cryptography;

//namespace ServicePortal.Application.Services
//{
//    public class JwtService
//    {
//        private readonly IConfiguration _config;

//        public JwtService(IConfiguration config)
//        {
//            _config = config;
//        }

//        public string GenerateAccessToken(List<Claim> claims)
//        {
//            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
//            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

//            var token = new JwtSecurityToken(
//                claims: claims,
//                expires: DateTime.UtcNow.AddMinutes(15),
//                signingCredentials: creds
//            );

//            return new JwtSecurityTokenHandler().WriteToken(token);
//        }

//        public string GenerateRefreshToken()
//        {
//            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
//        }
//    }
//}