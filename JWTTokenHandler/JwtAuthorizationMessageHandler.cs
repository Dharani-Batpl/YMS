using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using YardManagementApplication.Models;
using YardManagementApplication.Services;

namespace YardManagementApplication.JWTTokenHandler
{
    public class JwtAuthorizationMessageHandler : DelegatingHandler
    {
        private readonly IJwtService _jwtService;
        private readonly ILogger<JwtAuthorizationMessageHandler> _logger;
        private readonly SemaphoreSlim _refreshLock = new SemaphoreSlim(1, 1);


        public JwtAuthorizationMessageHandler(IJwtService jwtService, ILogger<JwtAuthorizationMessageHandler> logger)
        {
            _jwtService = jwtService;
            _logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Remove any stale Authorization header
            if (request.Headers.Contains("Authorization"))
                request.Headers.Remove("Authorization");

            var token = _jwtService.GetAccessToken();

            if (!string.IsNullOrWhiteSpace(token))
            {
                // 🔹 Ensure token freshness before every request
                if (await _jwtService.IsTokenExpiredAsync())
                {
                    await _refreshLock.WaitAsync(cancellationToken);
                    _logger.LogWarning("Access token expired, refreshing...");
                    var refreshed = await _jwtService.RefreshTokensAsync();
                    if (refreshed)
                    {
                        token = _jwtService.GetAccessToken();
                        _logger.LogInformation("Token refreshed successfully.");
                    }
                    else
                    {
                        _logger.LogError("Token refresh failed, using existing token.");
                    }


                    var clonedRequest = await CloneRequestAsync(request);
                    clonedRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    //response.Dispose(); // Dispose the old 401 response
                   return  await base.SendAsync(clonedRequest, cancellationToken);
                
            }

                _logger.LogDebug("Attaching fresh bearer token to request.");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                _logger.LogWarning("No access token found in JwtService when sending request.");
            }

            return await base.SendAsync(request, cancellationToken);
        }

        private async Task<HttpRequestMessage> CloneRequestAsync(HttpRequestMessage request)
        {
            var clone = new HttpRequestMessage(request.Method, request.RequestUri)
            {
                Version = request.Version
            };

            foreach (var header in request.Headers)
            {
                clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            if (request.Content != null)
            {
                var originalContent = await request.Content.ReadAsByteArrayAsync();
                clone.Content = new ByteArrayContent(originalContent);

                foreach (var header in request.Content.Headers)
                {
                    clone.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
            }

            return clone;
        }
    }

    //public class TokenRetryHandler : DelegatingHandler
    //{
    //    private readonly ITokenService _tokenService;
    //    public TokenRetryHandler(ITokenService tokenService) { _tokenService = tokenService; }

    //    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    //    {
    //        // Send initial request with current access token
    //        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _tokenService.GetAccessToken());
    //        var response = await base.SendAsync(request, cancellationToken);

    //        // If 401 received, try refreshing token and retry once
    //        if (response.StatusCode == HttpStatusCode.Unauthorized)
    //        {
    //            var refreshSuccess = await _tokenService.TryRefreshTokenAsync();
    //            if (refreshSuccess)
    //            {
    //                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _tokenService.GetAccessToken());
    //                response = await base.SendAsync(request, cancellationToken);
    //            }
    //        }
    //        return response;
    //    }
    //}

}