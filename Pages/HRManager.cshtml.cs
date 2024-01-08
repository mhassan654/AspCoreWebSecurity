using ASPCoreWebSecurity.Authorization;
using ASPCoreWebSecurity.DTO;
using ASPCoreWebSecurity.Pages.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace ASPCoreWebSecurity.Pages
{

    [Authorize]
    public class HRManager(IHttpClientFactory httpClientFactory) : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

        [BindProperty]
        public List<WeatherForecastDTO> ForecastsItems { get; set; } = new List<WeatherForecastDTO>();

        public async Task OnGetAsync()
        {
            JwtToken token = new();

            var strTokenObj = HttpContext.Session.GetString("access_token");

            if (string.IsNullOrEmpty(strTokenObj))
            {
               token = await Authenticate();
            }
            else
            {
                token = JsonConvert.DeserializeObject<JwtToken>(strTokenObj) ?? new JwtToken();
            }

            if(token == null || string.IsNullOrWhiteSpace(token.AccessToken) || token.ExpiresAt <= DateTime.UtcNow)
            {
                await Authenticate();
            }

            var httpClient = _httpClientFactory.CreateClient("OurWebApi");

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token?.AccessToken??string.Empty);
            ForecastsItems = await httpClient.GetFromJsonAsync<List<WeatherForecastDTO>>("WeatherForecast") ?? new List<WeatherForecastDTO>();
        }

        private async Task<JwtToken> Authenticate()
        {
            var httpClient = _httpClientFactory.CreateClient("OurWebApi");
            var res = await httpClient.PostAsJsonAsync("auth", new Credential { Password = "password", UserName = "admin" });
            res.EnsureSuccessStatusCode();
            string strJwt = await res.Content.ReadAsStringAsync();
            HttpContext.Session.SetString("access_token", strJwt);
            return JsonConvert.DeserializeObject<JwtToken>(strJwt) ?? new JwtToken();
        }
    }
}