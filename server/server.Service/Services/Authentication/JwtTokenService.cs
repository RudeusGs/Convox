using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using server.Domain.Entities;
using server.Service.Interfaces.Authentication;
using server.Service.Models.Authenticate;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace server.Service.Services.Authentication
{
    /// <summary>
    /// JwtTokenService: Tạo và quản lý JWT token.
    /// </summary>
    public class JwtTokenService : IJwtTokenService
    {
        private readonly IConfiguration _configuration;

        public JwtTokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Tạo JWT token cho user.
        /// </summary>
        public UserToken GenerateToken(User user, IEnumerable<string> roles)
        {
            var secret = _configuration["JWT:Secret"];
            if (string.IsNullOrWhiteSpace(secret))
                throw new InvalidOperationException("Missing JWT:Secret in configuration.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var now = DateTime.UtcNow;
            var expires = now.AddDays(7);

            var claims = BuildClaims(user, roles);

            var jwt = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                claims: claims,
                notBefore: now,
                expires: expires,
                signingCredentials: creds
            );

            var token = new JwtSecurityTokenHandler().WriteToken(jwt);

            return new UserToken
            {
                UserId = user.Id,
                Username = user.UserName,
                Email = user.Email,
                FullName = user.FullName,
                Token = token,
                Expires = expires
            };
        }

        /// <summary>
        /// Build claims cho JWT token.
        /// </summary>
        private static IEnumerable<Claim> BuildClaims(User user, IEnumerable<string> roles)
        {
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.UserName ?? string.Empty),
                new(ClaimTypes.Email, user.Email ?? string.Empty),
            };

            foreach (var r in roles.Distinct())
                claims.Add(new Claim(ClaimTypes.Role, r));

            return claims;
        }
    }
}