using Microsoft.IdentityModel.Tokens;
using SoftwareProject.API.Entites;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SoftwareProject.API.Authentication
{
    public class TokenGenerator
    {
        private readonly IConfiguration configuration;

        public TokenGenerator(IConfiguration configuration)
        {
            this.configuration = configuration ??
                throw new ArgumentNullException(nameof(configuration));
        }

        public string? generateToken(User user)
        {
            // 1- Generate SecurityKey
            var securityKey = new SymmetricSecurityKey(
                Encoding.ASCII.GetBytes(configuration["Jwt:Key"])
            );

            // 2- Generate ClaimsArrey
            var authClaims = new List<Claim>()
            {
                new Claim("id", user.UserId.ToString()),           
                new Claim("name", user.Name.ToString()),           
                new Claim("role", user.Role.ToString()),
            };

            // 3- Generate new instance from jwtSecurityToken 
            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                expires: DateTime.UtcNow.AddHours(12),
                claims: authClaims,
                signingCredentials: new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256)
            );

            // 4- Generate Token string
            var encryptedToken = new JwtSecurityTokenHandler().WriteToken(token);

            // 5- return token string
            return encryptedToken;
        }
    }
}
