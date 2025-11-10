namespace YardManagementApplication.Middlewares
{
    public class ApiUnauthorizedInterceptorMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ApiUnauthorizedInterceptorMiddleware> _logger;

        public ApiUnauthorizedInterceptorMiddleware(
            RequestDelegate next,
            ILogger<ApiUnauthorizedInterceptorMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Store the original response body stream
            var originalBodyStream = context.Response.Body;

            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            await _next(context);

            // Read the response body
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var responseText = await new StreamReader(context.Response.Body).ReadToEndAsync();
            context.Response.Body.Seek(0, SeekOrigin.Begin);

            // Check if response contains 401 or unauthorized status
            if (IsUnauthorizedResponse(responseText, context.Response.StatusCode))
            {
                _logger.LogWarning("Unauthorized API response detected, redirecting to login");

                context.Response.Clear();
                context.Response.StatusCode = 302; // Redirect
                context.Response.Headers["Location"] = "/Login/Login";
                return;
            }

            // Copy the modified response back to the original stream
            await responseBody.CopyToAsync(originalBodyStream);
        }

        private bool IsUnauthorizedResponse(string responseBody, int statusCode)
        {
            // Check if status code is 200 but body contains 401
            if (!string.IsNullOrEmpty(responseBody))
            {
                if (responseBody.Contains("Token is invalid or expired"))
                {
                    return true;
                }
                // Check for 401 in JSON response
                if (responseBody.Contains("\"Status\":401") ||
                    responseBody.Contains("\"statusCode\":401") ||
                    responseBody.Contains("\"code\":401") 
                    //||
                    //responseBody.Contains("401")
                    )
                {
                    return true;
                }
            }

            return false;
        }
    }
}