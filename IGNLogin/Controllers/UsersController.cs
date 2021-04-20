using System;
using IGNAuthentication.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IGNLogin.Controllers
{
    [Route("api/user")]
    [Authorize]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private IUserService _service;
        public UsersController(IUserService service)
        {
            _service = service;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetOrCreateUser([FromQuery]string email)
        {
            try
            {
                var user = _service.CreateUser(email);
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

        [HttpPost("activate")]
        [Authorize(Roles = "User")]
        public IActionResult ActivateUser([FromQuery]string email)
        {
            try
            {
                _service.ActivateUser(email);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpPost("activateById")]
        [Authorize(Roles = "Admin")]
        public IActionResult ActivateUser([FromQuery]long id)
        {
            try
            {
                _service.ActivateUser(id);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpPost("deactivate")]
        [Authorize(Roles = "User")]
        public IActionResult DeactivateUser([FromQuery]string email)
        {
            try
            {
                _service.DeactivateUser(email);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpPost("deactivateById")]
        [Authorize(Roles = "Admin")]
        public IActionResult DeactivateUser([FromQuery]long id)
        {
            try
            {
                _service.DeactivateUser(id);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpPost("updatem")]
        [Authorize(Roles = "Admin")]
        public IActionResult ChangeEmail([FromQuery]string email, [FromQuery]string newMail)
        {
            try
            {
                _service.ChangeUserEmail(email, newMail);
                var user = _service.GetUser(email);
                return Ok(user);
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpPost("delete")]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteUser([FromQuery]string email)
        {
            try
            {
                _service.DeleteUser(email);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpPost("delete")]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteUser([FromQuery]long id)
        {
            try
            {
                _service.DeleteUser(id);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpGet("active")]
        [AllowAnonymous]
        public IActionResult IsActive([FromQuery]string email)
        {
            try
            {
                var isActive = _service.IsUserActive(email);
                return Ok(isActive);
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpGet("offline")]
        [AllowAnonymous]
        public IActionResult GetOfflineCode([FromQuery] string email)
        {
            try
            {
                var offlineCode = _service.GetOfflineActivationDataForUser(email);
                return Ok(offlineCode);
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpGet("offkeygen.zip")]
        [Produces("application/octet-stream")]
        [AllowAnonymous]
        public IActionResult GetOfflineKeyGen()
        {
            return new FileContentResult(System.IO.File.ReadAllBytes(".\\Programs\\IGNOfflineActivator.zip"), "application/octet-stream");
        }
    }
}