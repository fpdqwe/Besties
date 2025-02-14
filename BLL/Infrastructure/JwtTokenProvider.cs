using BLL.Abstractions;
using Domain.Entities;
using Domain.Enums;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text;

namespace BLL.Infrastructure
{
    public class JwtTokenProvider(IOptions<JwtOptions> options) : IJwtTokenProvider
    {
        private readonly JwtOptions _options = options.Value;

        public string GenerateToken(User user)
        {
            Claim[] claims = [new("userId", user.Id.ToString()),
                new("role", role.ToString())];

            return GenerateToken(claims);
        }

        public string GenerateToken()
        {
            Claim[] claims = [new("role", UserRole.Guest.ToString())];

            return GenerateToken(claims);
        }

        private string GenerateToken(Claim[] claims)
        {
            var signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey)),
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                signingCredentials: signingCredentials,
                expires: DateTime.UtcNow.AddHours(_options.ExpiresHours)
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
