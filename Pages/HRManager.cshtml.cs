using ASPCoreWebSecurity.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

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
            var httpClient = _httpClientFactory.CreateClient("OurWebApi");
            ForecastsItems = await httpClient.GetFromJsonAsync<List<WeatherForecastDTO>>("WeatherForecast") ?? new List<WeatherForecastDTO>();
        }
    }
}