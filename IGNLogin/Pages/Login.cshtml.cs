using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
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
                    return RedirectToPage("loggedin", resultUser);
                }
                else
                {
                    return Redirect($"{redir}?Login={resultUser.Login}&Email={resultUser.Email}&Token={resultUser.Token}");
                }
            }
            else
            {
                var resultError = JsonConvert.DeserializeObject<Exception>(await result.Content.ReadAsStringAsync());
                return BadRequest(resultError);
            }
        }
    }
}
