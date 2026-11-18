using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace TaskManagementApi.Controllers
{
    [ApiController]
    [Route("api/dev/token")]
    [AllowAnonymous]
    public class DevTokenController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _env;

        public DevTokenController(IConfiguration config, IWebHostEnvironment env)
        {
            _config = config;
            _env = env;
        }

        public record TokenRequest(int UserId, string Role);

        [HttpPost]
        public IActionResult Create(TokenRequest request)
        {
            if (!_env.IsDevelopment())
            {
                return NotFound();
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:SigningKey"]!));

            var descriptor = new SecurityTokenDescriptor
            {
                Issuer = _config["Jwt:Issuer"],
                Audience = _config["Jwt:Audience"],
                Expires = DateTime.UtcNow.AddMinutes(30),
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("sub", request.UserId.ToString()),
                    new Claim("role", request.Role)
                }),
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            };

            var token = new JsonWebTokenHandler().CreateToken(descriptor);
            return Ok(new { token });
        }  
    }
}
