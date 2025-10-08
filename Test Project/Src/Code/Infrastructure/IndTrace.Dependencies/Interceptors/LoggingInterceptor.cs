namespace IndTrace.Dependencies.Interceptors;

/// <summary>
/// Intercepts method calls and logs before and after invocation for debugging purposes.
/// </summary>
public class LoggingInterceptor : IInterceptor
{
    /// <summary>
    /// Intercepts a method invocation, logging before and after the call.
    /// </summary>
    /// <param name="invocation">The method invocation information.</param>
    //TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate logging interceptor logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
    /// <summary>
    /// Executes Intercept operation.
    /// </summary>
    /// <param name="invocation">The invocation.</param>
    public void Intercept(IInvocation invocation)
    {
        Console.WriteLine($"[Before] Calling: {invocation.Method.Name}");

        invocation.Proceed(); // Proceed with the actual method

        Console.WriteLine($"[After] Done: {invocation.Method.Name}");
    }
}
