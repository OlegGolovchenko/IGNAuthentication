using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using IGNAuthentication.Domain.Interfaces.Services;
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

        private IUserService _service;
        private HttpClient _apiRequester;

        public LoggedInModel(IUserService service)
        {
            _apiRequester = new HttpClient();
            _service = service;
            Users = new List<CommunityUserListModel>();
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
                    Users = _service.ListCommunity().Where(u=>u.Login != usr.Login);
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
