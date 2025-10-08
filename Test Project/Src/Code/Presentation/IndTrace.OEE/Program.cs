// <copyright file="Program.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

using IndTrace.Application.Performance.Request.Command.Create;

namespace IndTrace.OEE;

using System.Data;
using System.Threading.Channels;
using IndTrace.Application.Models.Services;
using IndTrace.DataStore.DataAccess;
using IndTrace.DataStore.Interfaces;
using IndTrace.DataStore.Services.OEE.Interfaces;
using IndTrace.Dependencies.Http;
using IndTrace.HubConnection.Extensions;
using IndTrace.Dependencies.Interceptors;
using IndTrace.Dependencies.Services;
using IndTrace.Dependencies.Startup;
using IndTrace.Dependencies.Telemetry;
using IndTrace.Domain.Entities;
using IndTrace.OEE.Components;
using IndTrace.OEE.Components.Account;
using IndTrace.OEE.Infrastructure.Channels;
using IndTrace.OEE.Infrastructure.Repository;
using IndTrace.OEE.Infrastructure.Services;
using IndTrace.OEE.Workers;
using IndTrace.Persistence.DBContext;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MudBlazor.Services;
using QuestDB;
using QuestDB.Senders;
using Serilog;

/// <summary>
/// Entry point and configuration for the IndTrace OEE (Overall Equipment Effectiveness) monitoring application.
/// This Blazor Server application provides real-time OEE calculations, visualization, and analytics for industrial manufacturing processes.
/// </summary>
/// <remarks>
/// The application implements a vertical slice architecture with its own domain models, infrastructure services,
/// and UI components. It integrates with QuestDB for time-series data storage, SignalR for real-time communication,
/// and supports multiple deployment environments with comprehensive monitoring and logging capabilities.
/// </remarks>
public class Program
{
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    /// <param name="args">Command-line arguments.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task Main(string[] args)
    {
        // This is the entry point for the application.
        // The actual application logic is in the Program class.

        // TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Extract configuration loading logic into a dedicated configuration service.
        // Load externalized, centralized config
        var configuration = ConfigLoader.Load();

        // TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Consider using Microsoft.Extensions.Logging for consistency across the application.
        var logger = Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();

        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddSerilog(); // Use the already configured Serilog instance
        });

        logger.Information("Hello, Serilog!");

        Microsoft.Extensions.Logging.ILogger msLogger = loggerFactory.CreateLogger("Runners");

        // TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Consider using a more robust singleton pattern or removing this requirement for containerized deployments.
        var windowTitle = Runners.EnsureProgramIsSingleton(msLogger);

        var builder = WebApplication.CreateBuilder(args);

        builder.Configuration.AddConfiguration(configuration);

        // Set console title here
        Console.Title = windowTitle;

        var logFilePath = configuration["Serilog:WriteTo:2:Args:path"]; // Adjust index if needed
        Log.Information("Serilog is logging to file: {LogFilePath}", logFilePath);

        Log.Information("Logging configured");
        try
        {
            // TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Extract all service registrations into extension methods (e.g., AddOeeServices, AddInfrastructureServices).
            // Add Telemetry services
            builder.ConfigureOpenTelemetry();

            // Add Default Health Checks
            builder.AddDefaultHealthChecks();

            // TODO [PERFORMANCE][CURSOR][20/JUNE/2025] - Configure MudBlazor theme and global settings for optimal performance.
            // Add MudBlazor services
            builder.Services.AddMudServices();

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            // TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Consider implementing custom authentication policies for OEE-specific access control.
            builder.Services.AddCascadingAuthenticationState();
            builder.Services.AddScoped<IdentityUserAccessor>();
            builder.Services.AddScoped<IdentityRedirectManager>();
            builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

            builder.Services.AddAuthentication(options =>
                {
                    options.DefaultScheme = IdentityConstants.ApplicationScheme;
                    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
                })
                .AddIdentityCookies();

            // TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Implement configuration validation using IValidateOptions<T> for all connection strings.
            var connectionString = builder.Configuration.GetConnectionString("IndTraceDbContext") ?? throw new InvalidOperationException("Connection string 'IndTraceDbContext' not found.");
            if (string.IsNullOrEmpty(connectionString))
            {
                Log.Information("Could not find the 'IndTraceDbContext' connection string.");
                throw new InvalidOperationException("Could not find the 'IndTraceDbContext' connection string.");
            }

            // TODO [PERFORMANCE][CURSOR][20/JUNE/2025] - Configure connection pool size and timeout settings for production workloads.
            // Use main solution's SQL Server context instead of SQLite
            builder.Services.AddPooledDbContextFactory<IndTraceDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            // Use main solution's Identity setup with ApplicationUser

            // Add the new OEE domain services
            builder.Services.AddOeeServices();

            // Add OEE repository
            builder.Services.AddScoped<IndTrace.Application.Repository.IOeeRepository, IndTrace.DataStore.Repositories.OeeRepository>();

            // HubMonitorOptions and its validator are registered via AddHubConnectionAbstractions

            // TODO [PERFORMANCE][CURSOR][20/JUNE/2025] - Implement circuit breaker pattern for HTTP clients to handle downstream service failures gracefully.
            builder.Services.AddHttpClient("KpiReaderClient", (sp, client) =>
            {
                var config = sp.GetRequiredService<IConfiguration>();
                var baseAddress = config["KpiDataReader:ReaderBaseAddress"];
                if (string.IsNullOrWhiteSpace(baseAddress))
                {
                    throw new InvalidOperationException("Configuration 'KpiDataReader:ReaderBaseAddress' is missing or empty");
                }
                client.BaseAddress = new Uri(baseAddress);
                client.Timeout = TimeSpan.FromSeconds(10);
            }).AddPolicyHandler(ResiliencePolicies.GetRetryPolicy());

            builder.Services.AddHttpClient("KpiWriterClient", (sp, client) =>
            {
                var config = sp.GetRequiredService<IConfiguration>();
                var baseAddress = config["KpiDataSink:WriterBaseAddress"];
                if (string.IsNullOrWhiteSpace(baseAddress))
                {
                    throw new InvalidOperationException("Configuration 'KpiDataSink:WriterBaseAddress' is missing or empty");
                }
                client.BaseAddress = new Uri(baseAddress);
                client.Timeout = TimeSpan.FromSeconds(10);
            }).AddPolicyHandler(ResiliencePolicies.GetRetryPolicy());

            // TODO [RESOURCE LEAK][CURSOR][20/JUNE/2025] - Implement ISender as a scoped or transient service with proper disposal pattern
            // instead of singleton.
            // TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Extract QuestDB configuration into options pattern with validation.
            builder.Services.AddSingleton<ISender>(sp =>
            {
                var config = sp.GetRequiredService<IConfiguration>();
                var connStr = config["QuestDb:Sender"];
                if (string.IsNullOrWhiteSpace(connStr))
                {
                    throw new InvalidOperationException("Configuration 'QuestDb:Sender' is missing or empty");
                }
                return Sender.New(connStr);
            });

            // TODO [SOLID][CURSOR][20/JUNE/2025] - Separate read and write concerns into different interfaces and implementations.
            builder.Services.AddSingleton<IKpiDataReader>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<KpiDataSink>>();
                var client = sp.GetRequiredService<IHttpClientFactory>().CreateClient("KpiReaderClient");
                var sender = sp.GetRequiredService<ISender>();

                return new KpiDataReader(logger, client, sender);
            });

            builder.Services.AddSingleton<IKpiDataSink>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<KpiDataSink>>();
                var client = sp.GetRequiredService<IHttpClientFactory>().CreateClient("KpiWriterClient");
                var sender = sp.GetRequiredService<ISender>();

                return new KpiDataSink(logger, client, sender);
            });

            // Register concrete for classes depending directly on KpiDataSink
            builder.Services.AddSingleton<KpiDataSink>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<KpiDataSink>>();
                var client = sp.GetRequiredService<IHttpClientFactory>().CreateClient("KpiWriterClient");
                var sender = sp.GetRequiredService<ISender>();
                return new KpiDataSink(logger, client, sender);
            });

            // Reactive OEE processing service and worker
            builder.Services.AddSingleton<IReactiveEventService, ReactiveEventService>();

            // TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Consider implementing factory pattern for channel processors to support multiple OEE calculation strategies.
            builder.Services.AddSingleton<IOeeChannelProcessor, OeeChannelProcessor>();

            //[Fix]
            //CLAUDE
            //Date: 27/09/2025
            //Reason: [DI Registration] - Removed duplicate PlcDbOptions configuration (was registered twice)
            builder.Services.Configure<PlcDbOptions>(
                builder.Configuration.GetSection("PlcDb"));

            // TODO [RESOURCE LEAK][CURSOR][20/JUNE/2025] - Use IDbConnectionFactory instead of singleton IDbConnection to avoid connection leaks and timeouts.
            //[Fix]
            //CLAUDE
            //Date: 27/09/2025
            //Reason: [Resource Management] - Changed IDbConnection from Singleton to Scoped to prevent connection leaks
            // BANNER: Consider implementing IDbConnectionFactory pattern for better connection management
            builder.Services.AddScoped<IDbConnection>(sp =>
                new SqlConnection(configuration.GetConnectionString("IndTraceDbContext")));

            // TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Implement repository factory pattern for better testability and separation of concerns.
            builder.Services.AddSingleton<ITagsRepository>(sp =>
            {
                var db = sp.GetRequiredService<IDbConnection>();
                var logger = sp.GetRequiredService<ILogger<PlcDbTagsRepository>>();
                var options = sp.GetRequiredService<IOptions<PlcDbOptions>>();
                return new PlcDbTagsRepository(options, logger, db);
            });

            builder.Services.AddSingleton<IPlcDBRepository>(sp =>
            {
                var db = sp.GetRequiredService<IDbConnection>();
                var logger = sp.GetRequiredService<ILogger<PlcDbTagsRepository>>();
                var options = sp.GetRequiredService<IOptions<PlcDbOptions>>();
                return new PlcVirtualRepository(options, logger, db);
            });

            builder.Services.AddHubConnectionAbstractions(builder.Configuration);

            // TODO [PERFORMANCE][CURSOR][20/JUNE/2025] - Configure channel capacity and writer options for optimal throughput and memory usage.
            builder.Services.AddSingleton<IChannelBroker<PerformanceData>, ChannelBroker<PerformanceData>>();

            builder.Services.AddSingleton(Channel.CreateUnbounded<PerformanceData>());
            builder.Services.AddSingleton(Channel.CreateUnbounded<OeeRegisterDto>());

            //[Fix]
            //CLAUDE
            //Date: 27/09/2025
            //Reason: [DI Registration] - Removed duplicate OeeChannelProcessor registration (already registered as IOeeChannelProcessor on line 204)
            builder.Services.AddHostedService<OeeProcessorWorker>();

            builder.Services.AddHostedService<OoeWorkerClient>();
            builder.Services.AddHostedService<ReactiveEventWorker>();

            var app = builder.Build();

            // TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Add middleware for request/response logging and performance monitoring in production.
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Error", createScopeForErrors: true);

                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseAntiforgery();

            app.MapStaticAssets();
            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            // TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Add custom health checks for QuestDB, SignalR hub, and PLC connectivity.
            app.MapDefaultHealthEndpoints();

            // Add additional endpoints required by the Identity /Account Razor components.
            app.MapAdditionalIdentityEndpoints();

            app.Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Host terminated unexpectedly");

            // TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Remove Console.ReadLine() in production - this blocks containerized deployments.
            Console.ReadLine();
        }
        finally
        {
            await Log.CloseAndFlushAsync();

            // TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Remove Console.ReadLine() in production - this blocks graceful shutdown.
            Console.ReadLine();
        }
    }
}
