using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using YardManagementApplication.Models;

namespace YardManagementApplication.Services
{
    public class JwtService : IJwtService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<JwtService> _logger;

        // 🔹 Cache tokens in memory for immediate reuse
        private static string? _cachedAccessToken;
        private static string? _cachedRefreshToken;
        private static DateTimeOffset _cachedExpiry;

        private readonly string _baseApiUrl;

        public JwtService(IHttpContextAccessor httpContextAccessor, IHttpClientFactory httpClientFactory,
            IConfiguration configuration, ILogger<JwtService> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = logger;
            _baseApiUrl = _configuration.GetValue("ApiSettings:ServerURL", "http://localhost:8099");
        }

        public string GetAccessToken()
        {
            // Prefer in-memory cache
            if (!string.IsNullOrEmpty(_cachedAccessToken))
                return _cachedAccessToken;

            return _httpContextAccessor.HttpContext?.Request.Cookies["X-AccessToken"] ?? string.Empty;
        }

        public string GetRefreshToken()
        {
            if (!string.IsNullOrEmpty(_cachedRefreshToken))
                return _cachedRefreshToken;

            return _httpContextAccessor.HttpContext?.Request.Cookies["X-RefreshToken"] ?? string.Empty;
        }

        public async Task<bool> RefreshTokensAsync()
        {
            var refreshToken = GetRefreshToken();
            if (string.IsNullOrEmpty(refreshToken))
                return false;

            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_baseApiUrl);

            var payload = new { RefreshToken = refreshToken };
            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("/api/token/refresh", content);
            if (!response.IsSuccessStatusCode)
                return false;

            var respString = await response.Content.ReadAsStringAsync();
            var newTokens = JsonSerializer.Deserialize<TokenResponseModel>(respString,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (newTokens == null)
                return false;

            // 🔹 Update both memory and cookies
            UpdateInMemoryTokens(newTokens);
            StoreTokensinCookies(newTokens);

            return true;
        }

        private void UpdateInMemoryTokens(TokenResponseModel tokens)
        {
            _cachedAccessToken = tokens.AccessToken;
            _cachedRefreshToken = tokens.RefreshToken;
            _cachedExpiry = DateTimeOffset.UtcNow.AddSeconds(tokens.ExpiresIn);
        }

        public async Task<bool> IsTokenExpiredAsync()
        {
            // Check in-memory first
            //if (_cachedExpiry != default)
            //    return DateTimeOffset.UtcNow > _cachedExpiry;

            using (HttpClient client = _httpClientFactory.CreateClient())
            {
                client.BaseAddress = new Uri(_baseApiUrl);
                HttpRequestMessage msg= new HttpRequestMessage(HttpMethod.Get, "/api/token/validate");
                msg.Headers.Add("Authorization", "Bearer " + GetAccessToken());
                
                var response = await client.SendAsync(msg);
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                    return true;
                else
                    return false;
            }


            //// fallback for first-time use
            //var expiresAtStr = _httpContextAccessor.HttpContext.Session.GetString("ExpiresAt");
            //if (string.IsNullOrEmpty(expiresAtStr))
            //    return true;

            //var expiresAt = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expiresAtStr));
            //return DateTimeOffset.Now > expiresAt;
        }

      

        private void ClearCookies()
        {
            _httpContextAccessor.HttpContext?.Response.Cookies.Delete("X-AccessToken");
            _httpContextAccessor.HttpContext?.Response.Cookies.Delete("X-RefreshToken");
        }

        public void Logout()
        {
            ClearCookies();
            _cachedAccessToken = null;
            _cachedRefreshToken = null;
            _cachedExpiry = default;

        }

        public void StoreTokensinCookies(TokenResponseModel tokens)
        {
            _logger.LogWarning("Access Token: {AccessToken}", tokens.AccessToken);
            _logger.LogWarning("Refresh Token: {RefreshToken}", tokens.RefreshToken);
            _logger.LogWarning("****************Storing tokens in cookies************");

            ClearCookies();
            _httpContextAccessor.HttpContext?.Response.Cookies.Append("X-AccessToken", tokens.AccessToken,
                new CookieOptions { HttpOnly = true, Secure = true, SameSite = SameSiteMode.Strict });
            _httpContextAccessor.HttpContext?.Response.Cookies.Append("X-RefreshToken", tokens.RefreshToken,
                new CookieOptions { HttpOnly = true, Secure = true, SameSite = SameSiteMode.Strict });
        }
    }

}