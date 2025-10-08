namespace IndTrace.Dependencies.Http
{
    /// <summary>
    /// Provides HTTP resilience policies for handling transient errors and retries.
    /// </summary>
    public static class ResiliencePolicies
    {
        /// <summary>
        /// Gets a retry policy for HTTP requests that handles transient errors and HTTP 429 responses.
        /// </summary>
        /// <returns>An asynchronous retry policy for <see cref="HttpResponseMessage"/>.</returns>
        public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => (int)msg.StatusCode == 429)
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                    onRetry: (outcome, timespan, retryAttempt, context) =>
                    {
                        // Optionally log retry
                    });
        }
    }
}

//TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate resilience policies logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
