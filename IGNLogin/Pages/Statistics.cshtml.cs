using System.Net.Http;
using System.Threading.Tasks;
using IGNAuthentication.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace IGNLogin.Pages
{
    public class StatisticsModel : PageModel
    {
        private IUserService service;

        public int LoggedIn { get; private set; }

        public int Total { get; private set; }

        private HttpClient _apiRequester;

        public StatisticsModel()
        {
            _apiRequester = new HttpClient();
        }

        public async Task OnGetAsync()
        {
            var usersResponse = await _apiRequester.GetAsync("http://localhost:5000/api/community/usersCount");
            if (usersResponse.IsSuccessStatusCode)
            {
                Total = JsonConvert.DeserializeObject<int>(await usersResponse.Content.ReadAsStringAsync());
            }
            var usersCountResponse = await _apiRequester.GetAsync("http://localhost:5000/api/community/loggedInCount");
            if (usersCountResponse.IsSuccessStatusCode)
            {
                LoggedIn = JsonConvert.DeserializeObject<int>(await usersCountResponse.Content.ReadAsStringAsync());
            }
        }
    }
}
