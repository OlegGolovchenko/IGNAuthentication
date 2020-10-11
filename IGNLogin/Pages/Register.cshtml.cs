using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using IGNLogin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace IGNLogin.Pages
{
    public class RegisterModel : PageModel
    {
        public string Email { get; set; }

        public string Password { get; set; }

        private IGNAuthentication.Domain.ServiceProvider _services;
        private HttpClient _apiRequester;
        private string _redirectDestination = "";

        public RegisterModel(IGNAuthentication.Domain.ServiceProvider services)
        {
            _apiRequester = new HttpClient();
            _services = services;
        }

        public void OnGet([FromQuery] string redir)
        {
            _redirectDestination = redir;
            Email = "";
            Password = "";
        }

        public async Task<IActionResult> OnPostAsync([FromForm] string email, [FromForm] string password, [FromForm] string login, [FromQuery] string redir)
        {
            var result = await _apiRequester.PostAsync($"{Request.Scheme}://{Request.Host}/api/community/register",
                new StringContent(JsonConvert.SerializeObject(new RegisterUserLoginModel
                {
                    Login = login,
                    Email = email,
                    Password = password
                }), Encoding.UTF8, "application/json"));
            if (result.IsSuccessStatusCode)
            {
                var resultUser = JsonConvert.DeserializeObject<UserModel>(await result.Content.ReadAsStringAsync());
                if (string.IsNullOrWhiteSpace(redir))
                {
                    return RedirectToPage("loggedin", resultUser);
                }
                else
                {
                    return Redirect($"{redir}?Login={resultUser.Login}&Email={resultUser.Email}&Token={resultUser.Token}");
                }
            }
            return Unauthorized();
        }
    }
}
