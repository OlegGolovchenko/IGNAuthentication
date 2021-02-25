using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using IGNLogin.Exceptions;
using IGNLogin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace IGNLogin.Pages
{
    public class LoginModel : PageModel
    {
        public string Email { get; set; }

        public string Password { get; set; }

        public string Error { get; private set; }

        private HttpClient _apiRequester;
        private string _redirectDestination="";

        public LoginModel()
        {
            _apiRequester = new HttpClient();
        }

        public void OnGet([FromQuery]string redir)
        {
            _redirectDestination = redir;
            Email = "";
            Password = "";
        }

        public async Task<IActionResult> OnPostAsync([FromForm]string email, [FromForm]string password,[FromQuery]string redir)
        {
            var result = await _apiRequester.PostAsync($"{Request.Scheme}://{Request.Host}/api/community/login", 
                new StringContent(JsonConvert.SerializeObject(new UserLoginModel
                {
                    Email = email,
                    Password = password
                }),Encoding.UTF8,"application/json"));
            if (result.IsSuccessStatusCode)
            {
                var resultUser = JsonConvert.DeserializeObject<UserModel>(await result.Content.ReadAsStringAsync());
                if (string.IsNullOrWhiteSpace(redir))
                {
                    return RedirectToPage("loggedin", new { resultUser.Token });
                }
                else
                {
                    return Redirect($"{redir}?token={resultUser.Token}");
                }
            }
            else
            {
                var resultError = JsonConvert.DeserializeObject<IgrokNetException>(await result.Content.ReadAsStringAsync());
                Error = resultError?.Message;
                return Page();
            }
        }
    }
}
