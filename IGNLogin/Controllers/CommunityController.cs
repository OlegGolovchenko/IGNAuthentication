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

        [HttpGet("makeadmin")]
        [AllowAnonymous]
        public IActionResult AssignAdminRole([FromQuery]string email, [FromQuery]string adminCode)
        {
            var adminAccessCode = Environment.GetEnvironmentVariable("ADMIN_CODE");
            if (string.IsNullOrEmpty(adminCode) || adminCode != adminAccessCode)
            {
                return Unauthorized("wrong admin code provided");
            }
            var user = _services.GetUserService().GetUser(email);
            if(user.Id == -1)
            {
                return NotFound($"User with email {email} does not exist");
            }
            try
            {
                _services.GetUserService().AssignRoles(user.Id, "User;Admin");
            }
            catch(Exception e)
            {
                return BadRequest(e);
            }
            return Ok();
        }

        [HttpGet("list")]
        [Authorize(Roles = "Admin")]
        public IActionResult ListUsers()
        {
            try
            {
                return Ok(_services.GetUserService().
                                    ListCommunity().
                                    Where(usr=> usr.Login != User.Claims.
                                          FirstOrDefault(uc => uc.Type == ClaimTypes.Name)?.Value));
            }
            catch(Exception e)
            {
                return BadRequest(e);
            }
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
            if(!user.Roles.Any())
            {
                claims.Add(new Claim(ClaimTypes.Role, "User"));
            }
            foreach (var role in user.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
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
            if (!user.Roles.Any())
            {
                claims.Add(new Claim(ClaimTypes.Role, "User"));
            }
            foreach (var role in user.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
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
                return Ok(this.User.Claims.FirstOrDefault(x=>x.Type == ClaimTypes.Role && x.Value == "Admin") != null);
            }
            return Unauthorized();
        }
    }
}