using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using IGNAuthentication.Domain.Models;
using IGNLogin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Org.BouncyCastle.Bcpg.Sig;

namespace IGNLogin.Pages
{
    public class LoggedInModel : PageModel
    {
        public string UserName { get; set; }
        public IEnumerable<CommunityUserListModel> Users { get; set; }

        public Models.UserModel UsrModel { get; set; }

        private IGNAuthentication.Domain.ServiceProvider _services;
        private HttpClient _apiRequester;

        public LoggedInModel(IGNAuthentication.Domain.ServiceProvider services)
        {
            _apiRequester = new HttpClient();
            _services = services;
        }

        public async Task OnGetAsync([FromQuery] Models.UserModel usr)
        {
            UserName = usr.Login;
            UsrModel = usr;
            _apiRequester.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", usr.Token);
            var isadminresponse = await _apiRequester.GetAsync($"{Request.Scheme}://{Request.Host}/api/community/isadmin");
            if (isadminresponse.IsSuccessStatusCode)
            {
                var isadmin = JsonConvert.DeserializeObject<bool>(await isadminresponse.Content.ReadAsStringAsync());
                if (isadmin)
                {
                    Users = _services.GetUserService().ListCommunity().Where(u=>u.Login != usr.Login);
                }
            }
            else
            {
                Users = new List<CommunityUserListModel>();
            }
        }

        public IActionResult OnPost()
        {
            return Redirect("login");
        }
    }
}
