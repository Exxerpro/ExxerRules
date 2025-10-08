// <copyright file="Program.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Hub.Server
{
    using Serilog;

    using IndTrace.HubConnection.Extensions;
    using IndTrace.HubConnection.Dashboard;
    using Microsoft.AspNetCore.Http;

    /// <summary>
    /// Represents the Program.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Executes Main operation.
        /// </summary>
        /// <param name="args">The args.</param>
        /// <returns>The result of Main.</returns>
        public static async Task Main(string[] args)
        {
            // TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate program startup logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.

            // Load externalized, centralized config
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var logger = Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddSerilog(); // Use the already configured Serilog instance
            });
            Microsoft.Extensions.Logging.ILogger msLogger = loggerFactory.CreateLogger("Runners");

            var windowTitle = Runners.EnsureProgramIsSingleton(msLogger);

            var builder = WebApplication.CreateBuilder(args);

            // Set console title here
            Console.Title = windowTitle;

            // Create Serilog logger
            var logFilePath = configuration["Serilog:WriteTo:1:Args:path"]; // Adjust index if needed
            Log.Information("Serilog is logging to file: {LogFilePath}", logFilePath);

            // Set up logging
            builder.Logging.ClearProviders();
            builder.Logging.AddSerilog(logger, dispose: true);

            // Also add Console and Debug providers to see output in development
            builder.Logging.AddConsole();
            builder.Logging.AddDebug();

            // Use Kestrel server options from configuration
            builder.WebHost.UseKestrel((context, options) =>
            {
                // Load Kestrel configuration from appsettings.json
                options.Configure(context.Configuration.GetSection("Kestrel"));
            });

            builder.Services.Configure<WorkerHubServerOptions>(configuration.GetSection("WorkerHubServer"));
            builder.Services.AddHostedService<WorkerHubServer>();
            builder.Services.AddSignalR();
            // Register hub connection abstractions for client to upstream hub
            builder.Services.AddHubConnectionAbstractions(configuration);

            logger.Information("Starting to build services");

            builder.Logging.ClearProviders();
            builder.Logging.AddSerilog(logger, dispose: true);

            var app = builder.Build();
            app.MapHub<EventMonitorHub>("/EventMonitor");

            // Map dashboard endpoints for ADR metrics exposure
            app.MapHubMetricsEndpoints();

            // Lightweight health endpoints for hub and metrics
            app.MapGet("/health/hub", () => Results.Ok(new { Status = "Healthy", Timestamp = DateTimeOffset.UtcNow }))
               .WithName("HubHealth");
            // TODO: Implement IHubMetricsDashboard interface
            // app.MapGet("/health/metrics", async (IHubMetricsDashboard dashboard, CancellationToken ct) =>
            // {
            //     var health = await dashboard.GetHealthStatusAsync(ct).ConfigureAwait(false);
            //     return Results.Ok(health);
            // }).WithName("HubMetricsHealth");

            app.UseDeveloperExceptionPage();

            app.UseRouting();

            Log.Information("Starting web host");
            await app.RunAsync();
        }
    }
}
