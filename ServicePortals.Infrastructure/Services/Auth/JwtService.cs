using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Cryptography;
using Serilog;
using Microsoft.Extensions.Configuration;
using ServicePortals.Infrastructure.Data;

namespace ServicePortals.Infrastructure.Services.Auth
{
    public class JwtService
    {
        private readonly IConfiguration _config;

        private readonly ApplicationDbContext _context;

        public JwtService(IConfiguration config, ApplicationDbContext context)
        {
            _config = config;
            _context = context;
        }

        public string GenerateAccessToken(List<Claim> claims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"] ?? "service-portal-management-system"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var identity = new ClaimsIdentity(claims, "jwt");

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: identity.Claims,
                expires: DateTime.Now.AddMinutes(double.Parse(_config["Jwt:AccessTokenExpirationMinutes"] ?? "0")),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }

        //cronjob auto delete daily, token have expired 20 day
        public void DeleteOldRefreshToken()
        {
            try
            {
                var thresholdDate = DateTime.Now.AddDays(-30);
                const int batchSize = 1000;

                while (true)
                {
                    var tokensToDelete = _context.RefreshTokens
                        .Where(t => t.CreatedAt < thresholdDate)
                        .Take(batchSize)
                        .ToList();

                    if (!tokensToDelete.Any())
                        break;

                    _context.RefreshTokens.RemoveRange(tokensToDelete);
                    _context.SaveChanges();

                    Log.Information($"{tokensToDelete.Count} tokens deleted.");
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Error occurred while deleting old tokens: {ex.Message}");
            }
        }
    }
}