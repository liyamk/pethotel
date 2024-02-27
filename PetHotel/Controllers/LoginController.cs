using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PetHotel.IServices;
using PetHotel.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace PetHotel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class LoginController : ControllerBase
    {
        private readonly IRepository<User> _userRepo;
        private readonly IConfiguration _config;
        private readonly int expirationMinutes = 60;
        public LoginController(IRepository<User> userRepo, IConfiguration config)
        {
            _userRepo = userRepo;
            _config = config;
        }

        [HttpPost]
        public async Task<ActionResult> LoginAsync([FromBody] LoginUser loginUser)
        {
            if (loginUser == null)
            {
                return Unauthorized();
            }

            var user = await AuthenticateUserAsync(loginUser);
            if (user != null)
            {
                var token = CreateJwtToken(user);
                user.LastLoginDate = DateTime.UtcNow;
                await _userRepo.UpdateAsync(user);
                return Ok(token);
            }

            return Unauthorized();
        }

        private JwtToken CreateJwtToken(User user)
        {
            var issuer = _config["Jwt:Issuer"];
            var audience = _config["Jwt:Audience"];
            var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var expiration = DateTime.UtcNow.AddMinutes(expirationMinutes);

            var signingKey = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256
            );
  
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: authClaims,
                notBefore: DateTime.UtcNow,
                expires: expiration,
                signingCredentials: signingKey
            );

            var tokenStr = new JwtSecurityTokenHandler().WriteToken(token);

            return new JwtToken
            {
                Token = tokenStr,
                Expiration = expiration
            };
        }

        private async Task<User?> AuthenticateUserAsync(LoginUser loginUser)
        {
            var user = await _userRepo.GetAsync(x => x.UserName == loginUser.UserName);
            if (user != null)
            {
                if (user.Password == loginUser.Password)
                {
                    return user;
                }
            }            

            return null;
        }
    }
}
