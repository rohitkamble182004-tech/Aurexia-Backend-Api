using Fashion.Api.Core.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Fashion.Api.Helpers
{
    public class JwtTokenGenerator
    {
        private readonly IConfiguration _config;

        public JwtTokenGenerator(IConfiguration config)
        {
            _config = config;
        }

        public string Generate(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var key = _config["Jwt:Key"]
                ?? throw new InvalidOperationException("JWT Key is missing");

            var claims = new List<Claim>
            {
                // Unique user identifier
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),

                // ASP.NET identity identifier
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),

                // Email
                new Claim(JwtRegisteredClaimNames.Email, user.Email),

                // Role (lowercase for consistency with controllers)
                new Claim(ClaimTypes.Role, user.Role.ToString().ToLower())
            };

            var securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(key)
            );

            var credentials = new SigningCredentials(
                securityKey,
                SecurityAlgorithms.HmacSha256
            );

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}