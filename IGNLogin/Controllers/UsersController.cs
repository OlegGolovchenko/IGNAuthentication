﻿using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IGNLogin.Controllers
{
    [Route("api/user")]
    [Authorize]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private IGNAuthentication.Domain.ServiceProvider _services;
        public UsersController(IGNAuthentication.Domain.ServiceProvider services)
        {
            _services = services;
        }
        [HttpGet]
        [AllowAnonymous]
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

        [HttpPost("activate")]
        [Authorize(Roles = "admin")]
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

        [HttpPost("deactivate")]
        [Authorize(Roles = "admin")]
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

        [HttpPost("updatem")]
        [Authorize(Roles = "admin")]
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

        [HttpPost("delete")]
        [Authorize(Roles = "admin")]
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

        [HttpGet("active")]
        [AllowAnonymous]
        public IActionResult IsActive([FromQuery]string email)
        {
            try
            {
                var isActive = _services.GetUserService().IsUserActive(email);
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
                var offlineCode = _services.GetUserService().GetOfflineActivationDataForUser(email);
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