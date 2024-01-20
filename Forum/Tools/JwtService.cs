using Forum.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Forum.Tools
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;
        private readonly JwtSecurityTokenHandler _handler;
        public JwtService(IConfiguration configuration, JwtSecurityTokenHandler handler )
        {
            _configuration = configuration;
            _handler = handler;
        }

        public string GenerateToken(IEnumerable<Claim> claims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Secret"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
         
            claims: claims,
            expires: DateTime.Now.AddMinutes(60),
            audience: "http://localhost:5109/",
            issuer: "http://localhost:5109/",
            signingCredentials: creds
            );
            return _handler.WriteToken(token);
        }
    }
}
