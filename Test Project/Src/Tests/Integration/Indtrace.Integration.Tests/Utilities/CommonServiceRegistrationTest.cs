// Src/Tests/Integration/Indtrace.Integration.Tests/Utilities/CommonServiceRegistrationTest.cs
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace Integration.Tests.Utilities;

/// <summary>
/// Test-only extensions for wiring common services in integration tests without production persistence.
/// </summary>
public static class CommonServiceRegistrationTest
{
    /// <summary>
    /// Registers common services for tests, explicitly excluding production persistence wiring.
    /// </summary>
    /// <param name="services">DI service collection.</param>
    /// <param name="configuration">Application configuration.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddCommonServicesTest(this IServiceCollection services, IConfiguration configuration)
    {
        // Intentionally minimal; production services are not wired here for tests.
        return services;
    }
}
