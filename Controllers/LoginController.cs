using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using YardManagementApplication.Models;

namespace YardManagementApplication.Controllers
{
    public class LoginController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _GPSConfiguration;

        public LoginController(IHttpClientFactory httpClientFactory, IConfiguration gPSConfiguration)
        {
            _httpClient = httpClientFactory.CreateClient();
            _GPSConfiguration = gPSConfiguration;
        }

        // Render login page
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // Process login form postback
        [HttpPost]

        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            if (!ModelState.IsValid)
                return View(loginModel);

            var json = JsonSerializer.Serialize(loginModel);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            /* gps */
            var ServerURL = _GPSConfiguration["ServerURL"]+ "/api/token/login";

            var response = await _httpClient.PostAsync(ServerURL, content);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Invalid username or password");
                return View(loginModel);
            }

            var respString = await response.Content.ReadAsStringAsync();

            var tokenResponse = JsonSerializer.Deserialize<TokenResponseModel>(
                respString,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            HttpContext.Session.SetString("AccessToken", tokenResponse.AccessToken);

            //TempData["LoginUser"] = loginModel.Username;

            HttpContext.Session.SetString("LoginUser", loginModel.Username);

            return RedirectToAction("Index", "Dashboard");
        }
    }
}
