using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IGNAuthentication.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IGNLogin.Pages
{
    public class ActivateUserModel : PageModel
    {
        private IUserService _service;

        public ActivateUserModel(IUserService service)
        {
            _service = service;
        }

        public IActionResult OnGet([FromQuery]long id,[FromQuery]Models.UserModel userModel)
        {
            _service.ActivateUser(id);
            return RedirectToPage("loggedin", userModel);
        }
    }
}
