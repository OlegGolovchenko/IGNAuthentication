using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Text.Unicode;
using System.Threading.Tasks;
using IGNAuthentication.Domain.Interfaces.Services;
using IGNLogin.Exceptions;
using IGNLogin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace IGNLogin.Controllers
{
    [Route("api/community")]
    [Authorize]
    [ApiController]
    public class CommunityController : ControllerBase
    {
        private IUserService _service;
        public CommunityController(IUserService service)
        {
            _service = service;
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
            var user = _service.GetUser(email);
            if(user.Id == -1)
            {
                return NotFound($"User with email {email} does not exist");
            }
            try
            {
                _service.AssignRoles(user.Id, "User;Admin");
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
                return Ok(_service.ListCommunity().
                                    Where(usr=> usr.Login != User.Claims.
                                          FirstOrDefault(uc => uc.Type == ClaimTypes.Name)?.Value));
            }
            catch(Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpGet("loggedInCount")]
        [AllowAnonymous]
        public IActionResult CountLoggedInUsers()
        {
            try
            {
                var now = DateTime.UtcNow;
                return Ok(_service.ListCommunity().
                    Where(usr=>usr.LoginTime.AddMinutes(15) >= now).
                    Count());
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public IActionResult Register([FromBody]RegisterUserLoginModel newUser)
        {
            var user = _service.RegisterCommunity(newUser.Login, newUser.Password, newUser.Email);
            user.LoginTime = _service.UpdateLoginTime(user.Id, out var lastError);
            user.ContactEmail = _service.UpdateContactEmail(user.Id, newUser.Email, out lastError);
            if (user.Id == -1 || lastError != null)
            {
                return BadRequest(new IgrokNetException(user.LastError.Message));
            }
            var adminAccessCode = Environment.GetEnvironmentVariable("ADMIN_CODE");
            var secret = Environment.GetEnvironmentVariable("SECRET");

            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(secret);
            var offacc = _service.GetOfflineActivationDataForUser(user.Email);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Login),
                new Claim("IgRok-Net:OfflineCode", offacc),
                new Claim("IgRok-Net:IsActive", $"{user.IsActive||user.Roles.Contains("Admin")}")
            };
            if(!user.Roles.Any())
            {
                claims.Add(new Claim(ClaimTypes.Role, "User"));
            }
            foreach (var role in user.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            var signature = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature);
            var token = new JwtSecurityToken(Environment.GetEnvironmentVariable("ISSUER"), null, claims, user.LoginTime.ToUniversalTime(), user.LoginTime.ToUniversalTime().AddMinutes(15), signature);
            var tokenResult = tokenHandler.WriteToken(token);
            var responseUser = new UserModel
            {
                Email = user.Email,
                Login = user.Login,
                OfflineActivationCode = offacc
            };
            responseUser.Token = tokenResult;
            return Ok(responseUser);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public IActionResult Login([FromBody]UserLoginModel newUser)
        {
            var user = _service.LoginCommunity(newUser.Email, newUser.Password);
            user.LoginTime = _service.UpdateLoginTime(user.Id, out var lastError);
            user.ContactEmail = _service.UpdateContactEmail(user.Id, newUser.Email, out lastError);
            if (user.Id == -1 || lastError != null)
            {
                return BadRequest(new IgrokNetException(user.LastError.Message));
            }
            var adminAccessCode = Environment.GetEnvironmentVariable("ADMIN_CODE");
            var secret = Environment.GetEnvironmentVariable("SECRET");

            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(secret);
            var offacc = _service.GetOfflineActivationDataForUser(user.Email);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Login),
                new Claim("IgRok-Net:OfflineCode", offacc),
                new Claim("IgRok-Net:IsActive", $"{user.IsActive||user.Roles.Contains("Admin")}")
            };
            if (!user.Roles.Any())
            {
                claims.Add(new Claim(ClaimTypes.Role, "User"));
            }
            foreach (var role in user.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            var signature = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature);
            var token = new JwtSecurityToken(Environment.GetEnvironmentVariable("ISSUER"), null, claims, user.LoginTime.ToUniversalTime(), user.LoginTime.ToUniversalTime().AddMinutes(15), signature);
            var tokenResult = tokenHandler.WriteToken(token);
            var responseUser = new UserModel
            {
                Email = user.Email,
                Login = user.Login,
                OfflineActivationCode = offacc
            };
            responseUser.Token = tokenResult;
            return Ok(responseUser);
        }

        [HttpGet("my")]
        public IActionResult MyUser([FromQuery]string token)
        {
            if(this.User != null)
            {
                bool.TryParse(this.User.Claims.SingleOrDefault(x => x.Type == "IgRok-Net:IsActive")?.Value, out var active);
                var usr = new UserModel
                {
                    Login = this.User.Identity.Name,
                    Email = this.User.Claims.SingleOrDefault(x => x.Type == ClaimTypes.Email)?.Value,
                    OfflineActivationCode = this.User.Claims.SingleOrDefault(x=>x.Type == "IgRok-Net:OfflineCode")?.Value,
                    IsActive = active,
                    Token = token
                };
                return Ok(usr);
            }
            return Unauthorized();
        }

        [HttpGet("offline")]
        [Produces("application/octet-stream")]
        [AllowAnonymous()]
        public async Task<IActionResult> MyOfflineCodeAsync([FromQuery] string token)
        {
            if (!string.IsNullOrWhiteSpace(token))
            {

                UserModel user = null;
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    var userResponse = await httpClient.GetAsync($"http://localhost:5000/api/community/my?token={token}");
                    if (userResponse.IsSuccessStatusCode)
                    {
                        user = JsonConvert.DeserializeObject<UserModel>(await userResponse.Content.ReadAsStringAsync());
                    }
                }
                return File(Encoding.UTF8.GetBytes(user.OfflineActivationCode), "text/plain", "license.txt");
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