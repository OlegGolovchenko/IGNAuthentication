using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IGNLogin.Pages
{
    public class ActivateUserModel : PageModel
    {
        private IGNAuthentication.Domain.ServiceProvider _services;

        public ActivateUserModel(IGNAuthentication.Domain.ServiceProvider services)
        {
            _services = services;
        }

        public IActionResult OnGet([FromQuery]long id,[FromQuery]Models.UserModel userModel)
        {
            _services.GetUserService().ActivateUser(id);
            return RedirectToPage("loggedin", userModel);
        }
    }
}
