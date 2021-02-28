using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IGNAuthentication.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IGNLogin.Pages
{
    public class StatisticsModel : PageModel
    {
        private IUserService service;

        public int LoggedIn { get; private set; }

        public int Total { get; private set; }

        public StatisticsModel(IUserService service)
        {
            this.service = service;
        }

        public void OnGet()
        {
            var users = this.service.ListCommunity();
            var now = DateTime.UtcNow;
            LoggedIn = users.Count(x => x.LoginTime.AddMinutes(15) >= now);
            Total = users.Count();
        }
    }
}
