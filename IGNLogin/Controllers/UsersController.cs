using System;
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
        [HttpGet]
        public IActionResult GetOrCreateUser([FromQuery]string email)
        {
            try
            {
                var user = _services.GetUserService().GetOrCreateUser(email);
                if (user != null)
                {
                    return Ok(user);
                }
                return NotFound();
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpGet("activate")]
        public IActionResult ActivateUser([FromQuery]string email)
        {
            try
            {
                _services.GetUserService().ActivateUser(email);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpGet("deactivate")]
        public IActionResult DeactivateUser([FromQuery]string email)
        {
            try
            {
                _services.GetUserService().DeactivateUser(email);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpGet("updatem")]
        public IActionResult ChangeEmail([FromQuery]string email, [FromQuery]string newMail)
        {
            try
            {
                var user = _services.GetUserService().ChangeUserEmail(email, newMail);
                return Ok(user);
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpGet("delete")]
        public IActionResult DeleteUser([FromQuery]string email)
        {
            try
            {
                _services.GetUserService().DeactivateUser(email);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
    }
}