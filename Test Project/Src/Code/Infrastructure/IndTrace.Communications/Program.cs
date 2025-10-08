// <copyright file="Program.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Communications;

using IndTrace.Application.Performance.Request.Command.Create;
using IndTrace.Application.Repository;
using IndTrace.Gateway.Gateway;
using IndTrace.Persistence.Caching;
using IndTrace.Persistence.Interfaces;
using ZiggyCreatures.Caching.Fusion;
using ZiggyCreatures.Caching.Fusion.Serialization.SystemTextJson;
using System.Text.Json;
using Serilog;
using IndTrace.Persistence.Converters;
using IndTrace.HubConnection.Validators;
using IndTrace.Application.Cycles;
using IndTrace.CognexComm;

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
        // Load externalized, centralized config
        var configuration = ConfigLoader.Load();

        var logger = Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();

        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddSerilog(); // Use the already configured Serilog instance
        });

        Microsoft.Extensions.Logging.ILogger msLogger = loggerFactory.CreateLogger("Runners");

        var windowTitle = Runners.EnsureProgramIsSingleton(msLogger);

        Runners.EnsureProgramIsHighPriority(msLogger);

        var builder = Host.CreateApplicationBuilder(args);

        builder.Configuration.AddConfiguration(configuration);

        // Set console title here
        Console.Title = windowTitle;

        var logFilePath = configuration["Serilog:WriteTo:2:Args:path"]; // Adjust index if needed
        Log.Information("Serilog is logging to file: {LogFilePath}", logFilePath);

        Log.Information("Logging configured");

        try
        {
            Log.Information("Starting web host");

            // Add services to the container
            // Add Serilog to the DI container
            // Ensure the logger is available throughout the app
            builder.Logging.ClearProviders();
            builder.Logging.AddSerilog(logger);

            // Also add Console and Debug providers to see output in development
            builder.Logging.AddConsole();
            builder.Logging.AddDebug();

            // Add other required services
            builder.Services.AddSingleton<IMonitorRequestDispatcher, MonitorRequestDispatcher>();

            builder.Services.AddTransient<IMonitorRequestHandler<GetAppDetailsMonitorRequest, ApplicationConfiguration>, GetAppDetailsMonitorRequestHandler>();

            builder.Services.AddSingleton<IGatewayCommandDispatcher, GatewayCommandDispatcher>();

            builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(GatewayPersistenceBehavior<,>)); // Second, to persist the Request
            builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>)); // Third, to validate the Request
            builder.Services.AddSingleton(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>)); // Only in release, to handle unhandled exceptions

            builder.Services.AddTransient<INotificationService, IndTraceNotificationService>();

            builder.Services.AddSingleton<IIndTraceUserService, IndTraceUserService>();

            builder.Services.AddSingleton<CacheManager<ApplicationConfiguration>>(sp =>
                new CacheManager<ApplicationConfiguration>(
                    TimeSpan.FromMinutes(60)));

            builder.Services.AddTransient<AppDetailsFactory>();
            builder.Services.AddSingleton<IMonitorRequestHandler<GetAppDetailsMonitorRequest, ApplicationConfiguration>, GetAppDetailsMonitorRequestHandler>();

            builder.Services.AddSingleton<IIsOeeEnabledChecker, IsOeeEnabledChecker>();
            builder.Services.AddSingleton<IndTraceConfigurationService>();
            builder.Services.AddSingleton<IndTraceConfiguration>();

            builder.Services.AddSingleton<IIndTraceDbContext, IndTraceDbContext>();
            builder.Services.AddSingleton<Rule>();

            //[Fix]
            //CLAUDE
            //Date: 27/09/2025
            //Reason: [DI Registration] - Removed duplicate DateTimeMachine registration (was registered on line 104 and 118-119)
            builder.Services.AddSingleton<IDateTimeMachine, DateTimeMachine>();

            builder.Services.AddSingleton<IIndTraceDbContextFactory, IndTraceDbContextFactory>();

            builder.Services.AddSingleton<IBarCodeValidationService, BarCodeValidationService>();
            builder.Services.AddTransient<IBarCodeResult, BarCodeResult>();
            builder.Services.AddSingleton<IShiftService, ShiftService>();

            // Cognex worker dependencies (Singleton-only host):
            builder.Services.Configure<DataManSettings>(builder.Configuration.GetSection("DataManSettings"));
            builder.Services.AddSingleton<DataMan>();
            builder.Services.AddSingleton<IBarCodeScanned, BarCodeScanned>();

            builder.Services.AddRepositories(ServiceLifetime.Singleton);
            builder.Services.AddReadOnlyRepositories(ServiceLifetime.Singleton);

            // Add SRP cycle services (refactored handlers by default; pass 'false' to fallback)
            builder.Services.AddCycleServices();

            // Register Hub connection abstractions (IHubConnection, factory, metrics dashboard)
            builder.Services.AddHubConnectionAbstractions(builder.Configuration);

            builder.Services.AddSingleton<IGatewayRequestHandler<PerformanceDataCommand, TaskGatewayResponse>, CreatePerformanceDataCommandHandler>();

            builder.Services.AddSingleton<IGatewayRequestHandler<CreateBarCodeCommand, TaskGatewayResponse>, CreateBarCodeCommandHandler>();
            builder.Services.AddSingleton<IGatewayRequestHandler<ReadBarCodeQuery, TaskGatewayResponse>, GetBarCodeDetailGatewayQueryHandler>();
            builder.Services.AddSingleton<IGatewayRequestHandler<CreateCyclesCommand, TaskGatewayResponse>, CreateCyclesCommandHandler>();

            // Legacy cycle handlers remain available for manual fallback only by calling
            // AddCycleServices(useRefactoredHandlers: false) in the composition root setup.

            builder.Services.AddSingleton<IGatewayRequestHandler<UpdateBarCodeCommand, TaskGatewayResponse>, UpdateBarCodeCommandHandler>();

            // HubMonitorOptions and its validator are registered via AddHubConnectionAbstractions

            builder.Services.AddHostedService<GatewayWorkerManager>();
            builder.Services.AddHostedService<CognexBackGroundWorker>();

            // Add ICacheService for repository caching (replaces HybridCache)
            builder.Services.AddFusionCache()
                .WithSerializer(
                    new FusionCacheSystemTextJsonSerializer(
                        new JsonSerializerOptions
                        {
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                            WriteIndented = false,
                            Converters =
                            {
                                new EnumModelJsonConverter() // Critical: Support for SmartEnum serialization
                            }
                        }));
            builder.Services.AddSingleton<ICacheService, FusionCacheService>();

            // Configure DbContext
            var connectionStringApp = builder.Configuration.GetConnectionString("IndTraceDbContext");
            if (string.IsNullOrEmpty(connectionStringApp))
            {
                Log.Information("Could not find the 'IndTraceDbContext' connection string.");
                throw new InvalidOperationException("Could not find the 'IndTraceDbContext' connection string.");
            }

            // TODO [VERIFY]
            // ABR CHECK THIS STILL WORK, BECAUSE I DON'T HAVE REGISTERED A CONTEXT FACTORY IN THE PERSISTENCE PROJECT
            // I HAVE A CONTEXT FACTORY ON THE CLIENTS CLASS
            // DbContext registration updated to use AddPooledDbContextFactory for improved performance and thread safety.
            // This allows IDbContextFactory<IndTraceDbContext> to provide pooled DbContext instances.
            // Change applied: 2025-06-12
            builder.Services.AddPooledDbContextFactory<IndTraceDbContext>(options =>
                options.EnableDetailedErrors()
                    .UseSqlServer(connectionStringApp, sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly(typeof(IndTraceDbContext).Assembly.FullName);
                        sqlOptions.EnableRetryOnFailure();
                    })
                    .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));

            var host = builder.Build();
            await host.RunAsync();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Host terminated unexpectedly");
            Console.ReadLine();
        }
        finally
        {
            await Log.CloseAndFlushAsync();
            Console.ReadLine();
        }
    }
}
