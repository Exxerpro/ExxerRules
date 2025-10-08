using Serilog;
using ILogger = Serilog.ILogger;

namespace IndTrace.Dependencies.Middleware;

/// <summary>
/// Provides extension methods for configuring a common middleware pipeline for web applications.
/// </summary>
public static class CommonPipelineConfiguration
{
    /// <summary>
    /// Configures the application's middleware pipeline with common defaults such as HTTPS redirection, static files, authentication, and CORS.
    /// </summary>
    /// <param name="app">The web application instance.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <param name="logger">The logger for logging configuration steps.</param>
    /// <returns>The configured <see cref="WebApplication"/> instance.</returns>
    public static WebApplication UseCommonPipeline(this WebApplication app, IConfiguration configuration, ILogger logger)
    {
        logger.Information("Starting to configure common the pipeline");

        app.UseHttpsRedirection();

        app.UseDeveloperExceptionPage();

        app.UseStaticFiles();  // Ensure this is called only once

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseAntiforgery();

        app.UseCors("AllowAllOrigins");

        app.UseMigrationsEndPoint();

        //TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate common pipeline configuration logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.

        logger.Information("common the pipeline configured");
        return app;
    }
}
