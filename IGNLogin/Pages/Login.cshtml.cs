using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using IGNLogin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace IGNLogin.Pages
{
    public class LoginModel : PageModel
    {
        public string Email { get; set; }

        public string Password { get; set; }

        private IGNAuthentication.Domain.ServiceProvider _services;
        private HttpClient _apiRequester;
        private string _redirectDestination="";

        public LoginModel(IGNAuthentication.Domain.ServiceProvider services)
        {
            _apiRequester = new HttpClient();
            _services = services;
        }

        public void OnGet([FromQuery]string redir)
        {
            _redirectDestination = redir;
            Email = "";
            Password = "";
        }

        public async Task<IActionResult> OnPostAsync([FromForm]string email, [FromForm]string password)
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
                if (string.IsNullOrWhiteSpace(_redirectDestination))
                {
                    return RedirectToPage("loggedin", resultUser);
                }
                else
                {
                    return RedirectToPage(_redirectDestination, resultUser);
                }
            }
            return Unauthorized();
        }
    }
}
