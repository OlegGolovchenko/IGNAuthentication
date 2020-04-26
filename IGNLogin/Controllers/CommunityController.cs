using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using IGNLogin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace IGNLogin.Controllers
{
    [Route("api/community")]
    [Authorize]
    [ApiController]
    public class CommunityController : ControllerBase
    {
        private IGNAuthentication.Domain.ServiceProvider _services;
        public CommunityController(IGNAuthentication.Domain.ServiceProvider services)
        {
            _services = services;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public IActionResult Register([FromBody]RegisterUserLoginModel newUser)
        {
            var user = _services.GetUserService().RegisterCommunity(newUser.Login, newUser.Password, newUser.Email);
            if (user.Id == -1)
            {
                return BadRequest(user.LastError);
            }
            var adminAccessCode = Environment.GetEnvironmentVariable("ADMIN_CODE");
            var secret = Environment.GetEnvironmentVariable("SECRET");

            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(secret);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Login)
            };
            if (newUser.AdminCode == adminAccessCode)
            {
                claims.Add(new Claim(ClaimTypes.Role, "admin"));
            }
            else
            {
                claims.Add(new Claim(ClaimTypes.Role, "user"));
            }
            var now = DateTime.UtcNow;
            var signature = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature);
            var token = new JwtSecurityToken(Environment.GetEnvironmentVariable("ISSUER"), null, claims, now, now.AddMinutes(15), signature);
            var tokenResult = tokenHandler.WriteToken(token);
            var responseUser = new UserModel
            {
                Email = user.Email,
                Login = user.Login
            };
            responseUser.Token = tokenResult;
            return Ok(responseUser);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public IActionResult Login([FromBody]UserLoginModel newUser)
        {
            var user = _services.GetUserService().LoginCommunity(newUser.Email, newUser.Password);
            if (user.Id == -1)
            {
                return BadRequest(user.LastError);
            }
            var adminAccessCode = Environment.GetEnvironmentVariable("ADMIN_CODE");
            var secret = Environment.GetEnvironmentVariable("SECRET");

            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(secret);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Login)
            };
            if (newUser.AdminCode == adminAccessCode)
            {
                claims.Add(new Claim(ClaimTypes.Role, "admin"));
            }
            else
            {
                claims.Add(new Claim(ClaimTypes.Role, "user"));
            }
            var now = DateTime.UtcNow;
            var signature = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature);
            var token = new JwtSecurityToken(Environment.GetEnvironmentVariable("ISSUER"), null, claims, now, now.AddMinutes(15), signature);
            var tokenResult = tokenHandler.WriteToken(token);
            var responseUser = new UserModel
            {
                Email = user.Email,
                Login = user.Login
            };
            responseUser.Token = tokenResult;
            return Ok(responseUser);
        }

        [HttpGet("test")]
        public IActionResult TestAuthorisation()
        {
            if (this.User != null)
            {
                return Ok($"user logged in: {this.User.Claims.FirstOrDefault(usr => usr.Type == ClaimTypes.Name)?.Value}");
            }
            return Unauthorized();
        }

        [HttpGet("isadmin")]
        public IActionResult IsAdmin()
        {
            if (this.User != null)
            {
                return Ok(this.User.Claims.FirstOrDefault(x=>x.Type == ClaimTypes.Role).Value == "admin");
            }
            return Unauthorized();
        }
    }
}