using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace TaskManagementApi.Tests
{
    public static class TestTokens
    {
        public static string Create(ApiFactory factory, int userId, string role,
        DateTime? expires = null,
        string? signingKey = null,
        string? issuer = null,
        string? audience = null)
        {
            var config = factory.Services.GetRequiredService<IConfiguration>();

            var keyText = signingKey ?? config["Jwt:SigningKey"]!;

            var exp = expires ?? DateTime.UtcNow.AddMinutes(30);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyText));

            var descriptor = new SecurityTokenDescriptor
            {
                Issuer = issuer ?? config["Jwt:Issuer"],
                Audience = audience ?? config["Jwt:Audience"],
                Expires = exp,
                NotBefore = exp.AddMinutes(-30),
                IssuedAt = exp.AddMinutes(-30),
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("sub", userId.ToString()),
                    new Claim("role", role)
                }),
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            };

            return new JsonWebTokenHandler().CreateToken(descriptor);
        }
    }
}
