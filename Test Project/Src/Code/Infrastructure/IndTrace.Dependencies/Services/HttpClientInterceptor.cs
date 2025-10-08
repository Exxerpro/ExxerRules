namespace IndTrace.Dependencies.Services;

/// <summary>
/// HTTP client interceptor that handles authentication and logging for outgoing HTTP requests.
/// </summary>
/// <param name="logger">Logger instance for the interceptor.</param>
public class HttpClientInterceptor(ILogger<HttpClientInterceptor> logger) : DelegatingHandler
{
    /// <summary>
    /// Intercepts HTTP requests to add authentication headers and log request/response information.
    /// </summary>
    /// <param name="request">The HTTP request message.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A task representing the asynchronous operation that returns the HTTP response.</returns>
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // Log the monitorRequest
        logger.LogInformation("Sending monitorRequest to {Url}", request.RequestUri);

        // Add authorization header if needed
        if (request.Headers.Authorization == null)
        {
            var token = await this.GetTokenAsync();
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
        }

        var response = await base.SendAsync(request, cancellationToken);

        // Log the response
        if (!response.IsSuccessStatusCode)
        {
            logger.LogError("Response error: {StatusCode} - {Reason}", response.StatusCode, response.ReasonPhrase);
        }

        return response;
    }

    /// <summary>
    /// Gets the authentication token for the HTTP request.
    /// </summary>
    /// <returns>A task representing the asynchronous operation that returns the token string.</returns>
    private Task<string> GetTokenAsync()
    {
        // Implement your logic to get the token, e.g., from local storage or a token service
        return Task.FromResult(string.Empty);
    }
}

/// <summary>
/// Extension methods for HttpClient to enable request interception.
/// </summary>
public static class HttpClientExtensions
{
    /// <summary>
    /// Enables request interception for the HTTP client with authentication and logging.
    /// </summary>
    /// <param name="httpClient">The HTTP client to enable interception for.</param>
    /// <param name="services">The service provider for dependency injection.</param>
    /// <returns>The HTTP client with interception enabled.</returns>
    public static HttpClient EnableIntercept(this HttpClient httpClient, IServiceProvider services)
    {
        var interceptor = services.GetService<HttpClientInterceptor>();
        if (interceptor != null)
        {
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "your-token-here");
            var client = new HttpClient(interceptor)
            {
                BaseAddress = httpClient.BaseAddress,
            };
            return client;
        }
        return httpClient;
    }
}
