using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IGNLogin.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IGNLogin.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private IGNAuthentication.Domain.ServiceProvider _services;
        public UsersController(IGNAuthentication.Domain.ServiceProvider services)
        {
            _services = services;
        }
        [HttpPost]
        public IActionResult GetOrCreateUser([FromQuery]string email)
        {
            try
            {
                var user = _services.GetUserService().GetOrCreateUser(email);
                if(user != null)
                {
                    return Ok(new UserModel
                    {
                        Id = user.Id,
                        Email = user.Email,
                        IsActive = user.IsActive
                    });
                }
                return NotFound();
            }
            catch(Exception e)
            {
                return BadRequest(e);
            }
        }
    }
}