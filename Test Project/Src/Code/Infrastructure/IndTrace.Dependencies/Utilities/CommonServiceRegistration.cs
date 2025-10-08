using IndTrace.Domain.Models;
using IndTrace.Application.Models.Notifications;
using IndTrace.Application.Models.RequestHandler;
using IndTrace.Application.Registers.Services;
using IndTrace.Application.Repository;
using IndTrace.HubConnection.Extensions;
using IndTrace.Dependencies.Repositories;
using IndTrace.Dependencies.Services;
using IndTrace.Domain.Entities.BarCodes;
using IndTrace.Domain.Entities;
using IndTrace.Domain.Interfaces;
using IndTrace.Persistence.Caching;
using IndTrace.Persistence.DBContext;
using IndTrace.Persistence.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using MudBlazor.Services;
using MudExtensions.Services;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using Serilog;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using ILogger = Serilog.ILogger;
using IndTrace.Persistence.Converters;
using IndTrace.Persistence.Repositories;
using IndTrace.Persistence.Behaviors;
using ZiggyCreatures.Caching.Fusion;
using ZiggyCreatures.Caching.Fusion.Serialization.SystemTextJson;
using Microsoft.Extensions.Options;

/// <summary>
/// Provides extension methods for registering common services, repositories, logging, and application infrastructure.
/// </summary>
/// <remarks>
/// This class centralizes the registration of core, repository, and infrastructure services for the IndTrace application.
/// It ensures consistent dependency injection patterns and simplifies service setup for both read/write and read-only repositories.
/// </remarks>
namespace IndTrace.Dependencies.Utilities;

public static class CommonServiceRegistration
{
    /// <summary>
    /// Registers common services, middleware, and configuration for the application.
    /// </summary>
    /// <param name="services">The service collection to add the registrations to.</param>
    /// <param name="configuration">The application configuration instance.</param>
    /// <param name="logger">The logger instance for logging information.</param>
    /// <returns>The updated <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddCommonServices(this IServiceCollection services, IConfiguration configuration, ILogger logger)
    {
        logger.Information("Adding common services to the container");

        services.AddMediatorAndMapper();

        services.AddAntiforgery(options =>
        {
            options.HeaderName = "X-CSRF-TOKEN";
        });

        services.AddScoped<IDateTimeMachine, DateTimeMachine>();

        // Register Hub connection abstractions (IHubConnection, factory, metrics dashboard)
        services.AddHubConnectionAbstractions(configuration);

        // Add response compression
        services.AddResponseCompression(opts =>
        {
            opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                new[] { "application/octet-stream" });
        });

        services.AddSingleton<ILoggerFactory, LoggerFactory>();
        services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));

        services.AddSingleton<IndTraceConfiguration>();

        // Add FusionCache and serializer (used by ICacheService implementations)
        services.AddFusionCache()
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

        // Register concrete inner cache service (FusionCache-based)
        services.AddSingleton<FusionCacheService>();
        // Bind toggle options and expose ICacheService as a toggle decorator
        services.Configure<CacheToggleOptions>(configuration.GetSection("Caching:Toggle"));
        services.AddSingleton<ICacheService>(sp =>
        {
            var toggleOptions = sp.GetRequiredService<IOptions<CacheToggleOptions>>().Value;
            var logger = sp.GetRequiredService<ILogger<CacheToggleCacheService>>();
            ICacheService inner = sp.GetRequiredService<FusionCacheService>();
            return new CacheToggleCacheService(inner, toggleOptions, logger);
        });

        // Register a production cache partition provider (empty prefix) and initialize the builder
        services.AddSingleton<ICachePartitionProvider, ProductionCachePartitionProvider>();
        // Bind cache key options from config: section Caching:Keys
        services.Configure<CacheKeyOptions>(configuration.GetSection("Caching:Keys"));
        services.AddSingleton<CachePartitionInitializer>();

        services.AddSingleton(sp =>
            new CacheManager<ApplicationConfiguration>(
                TimeSpan.FromMinutes(60)));
        services.AddTransient<AppDetailsFactory>();
        services.AddTransient<IMonitorRequestHandler<GetAppDetailsMonitorRequest, ApplicationConfiguration>, GetAppDetailsMonitorRequestHandler>();
        services.AddScoped<IndTraceConfigurationService>();

        services.AddIndTracePersistence(configuration, logger);

        services.AddCors(options =>
        {
            options.AddPolicy(
                "AllowAllOrigins",
                builder =>
                {
                    builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
        });

        services.AddMudServices(options =>
        {
            options.PopoverOptions.ThrowOnDuplicateProvider = false;
        });

        services.AddMudExtensions();
        logger.Information("Common services added to the container");
        // Configure OpenTelemetry

        services.AddScoped<IRegisterInformationService, RegisterInformationService>();
        services.AddScoped<IDistinctRegisterService, DistinctRegisterService>();

        services.AddTransient<GetBarCodeDetailQuery>();
        services.AddTransient<GetBarCodesListQuery>();

        // TODO: CreateBarCode SRP services will be registered in test layer during Phase 3

        //TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate common service registration logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
        //TODO [DRY][CURSOR][20/JUNE/2025] - Check for repeated registration or configuration logic. Refactor for maintainability if necessary.
        //TODO [PERFORMANCE][CURSOR][20/JUNE/2025] - For high-frequency registration operations, consider optimizing service registration and memory usage.

        return services;
    }

    /// <summary>
    /// Registers event-related services and hybrid cache configuration.
    /// </summary>
    /// <param name="services">The service collection to add the registrations to.</param>
    /// <param name="configuration">The application configuration instance.</param>
    /// <param name="logger">The logger instance for logging information.</param>
    /// <returns>The updated <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddEventsServices(this IServiceCollection services, IConfiguration configuration, ILogger logger)
    {
        // Add FusionCache and our cache service abstraction
        services.AddFusionCache()
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

        // Register ICacheService implementation (FusionCache-based)
        services.AddSingleton<ICacheService, FusionCacheService>();

        services.AddScoped<EventsService>();

        return services;
    }

    /// <summary>
    /// Registers all read/write repositories for the application, grouped for clarity and maintainability.
    /// </summary>
    /// <param name="services">The service collection to add the registrations to.</param>
    /// <param name="lifeTime">The desired service lifetime (Singleton, Scoped, or Transient).</param>
    /// <returns>The updated <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddRepositories(this IServiceCollection services, ServiceLifetime lifeTime)
    {
        services.AddRepository<IRepository<BarCode>, Repository<BarCode>, BarCode>(lifeTime);
        services.AddRepository<IRepository<ConfigApp>, Repository<ConfigApp>, ConfigApp>(lifeTime);
        services.AddRepository<IRepository<Customer>, Repository<Customer>, Customer>(lifeTime);
        services.AddRepository<IRepository<Cycle>, Repository<Cycle>, Cycle>(lifeTime);

        services.AddRepository<IRepository<DistinctRegister>, Repository<DistinctRegister>, DistinctRegister>(lifeTime);
        services.AddRepository<IRepository<Line>, Repository<Line>, Line>(lifeTime);
        services.AddRepository<IRepository<Machine>, Repository<Machine>, Machine>(lifeTime);
        services.AddRepository<IRepository<MachinePlc>, Repository<MachinePlc>, MachinePlc>(lifeTime);

        services.AddRepository<IRepository<MasterLabel>, Repository<MasterLabel>, MasterLabel>(lifeTime);
        services.AddRepository<IRepository<Plc>, Repository<Plc>, Plc>(lifeTime);
        services.AddRepository<IRepository<Product>, Repository<Product>, Product>(lifeTime);
        services.AddRepository<IRepository<Recipe>, Repository<Recipe>, Recipe>(lifeTime);

        services.AddRepository<IRepository<Register>, Repository<Register>, Register>(lifeTime);
        services.AddRepository<IRepository<Rule>, Repository<Rule>, Rule>(lifeTime);
        services.AddRepository<IRepository<Shift>, Repository<Shift>, Shift>(lifeTime);
        services.AddRepository<IRepository<Variable>, Repository<Variable>, Variable>(lifeTime);

        services.AddRepository<IRepository<TaskGatewayRequest>, Repository<TaskGatewayRequest>, TaskGatewayRequest>(lifeTime);
        services.AddRepository<IRepository<TaskGatewayResponse>, Repository<TaskGatewayResponse>, TaskGatewayResponse>(lifeTime);
        services.AddRepository<IRepository<VariablesGroup>, Repository<VariablesGroup>, VariablesGroup>(lifeTime);
        services.AddRepository<IRepository<WorkFlow>, Repository<WorkFlow>, WorkFlow>(lifeTime);

        return services;
    }

    /// <summary>
    /// Registers all read-only repositories for the application, grouped for clarity and maintainability.
    /// </summary>
    /// <param name="services">The service collection to add the registrations to.</param>
    /// <param name="lifeTime">The desired service lifetime (Singleton, Scoped, or Transient).</param>
    /// <returns>The updated <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddReadOnlyRepositories(this IServiceCollection services, ServiceLifetime lifeTime)
    {
        services.AddReadOnlyRepository<IReadOnlyRepository<BarCode>, ReadOnlyRepository<BarCode>, BarCode>(lifeTime);
        services.AddReadOnlyRepository<IReadOnlyRepository<ConfigApp>, ReadOnlyRepository<ConfigApp>, ConfigApp>(lifeTime);
        services.AddReadOnlyRepository<IReadOnlyRepository<Customer>, ReadOnlyRepository<Customer>, Customer>(lifeTime);
        services.AddReadOnlyRepository<IReadOnlyRepository<Cycle>, ReadOnlyRepository<Cycle>, Cycle>(lifeTime);

        services.AddReadOnlyRepository<IReadOnlyRepository<DistinctRegister>, ReadOnlyRepository<DistinctRegister>, DistinctRegister>(lifeTime);
        services.AddReadOnlyRepository<IReadOnlyRepository<Line>, ReadOnlyRepository<Line>, Line>(lifeTime);
        services.AddReadOnlyRepository<IReadOnlyRepository<Machine>, ReadOnlyRepository<Machine>, Machine>(lifeTime);
        services.AddReadOnlyRepository<IReadOnlyRepository<MachinePlc>, ReadOnlyRepository<MachinePlc>, MachinePlc>(lifeTime);

        services.AddReadOnlyRepository<IReadOnlyRepository<MasterLabel>, ReadOnlyRepository<MasterLabel>, MasterLabel>(lifeTime);
        services.AddReadOnlyRepository<IReadOnlyRepository<Plc>, ReadOnlyRepository<Plc>, Plc>(lifeTime);
        services.AddReadOnlyRepository<IReadOnlyRepository<Product>, ReadOnlyRepository<Product>, Product>(lifeTime);
        services.AddReadOnlyRepository<IReadOnlyRepository<Recipe>, ReadOnlyRepository<Recipe>, Recipe>(lifeTime);

        services.AddReadOnlyRepository<IReadOnlyRepository<Register>, ReadOnlyRepository<Register>, Register>(lifeTime);
        services.AddReadOnlyRepository<IReadOnlyRepository<Rule>, ReadOnlyRepository<Rule>, Rule>(lifeTime);
        services.AddReadOnlyRepository<IReadOnlyRepository<Shift>, ReadOnlyRepository<Shift>, Shift>(lifeTime);
        services.AddReadOnlyRepository<IReadOnlyRepository<Variable>, ReadOnlyRepository<Variable>, Variable>(lifeTime);

        services.AddReadOnlyRepository<IReadOnlyRepository<TaskGatewayRequest>, ReadOnlyRepository<TaskGatewayRequest>, TaskGatewayRequest>(lifeTime);
        services.AddReadOnlyRepository<IReadOnlyRepository<TaskGatewayResponse>, ReadOnlyRepository<TaskGatewayResponse>, TaskGatewayResponse>(lifeTime);
        services.AddReadOnlyRepository<IReadOnlyRepository<VariablesGroup>, ReadOnlyRepository<VariablesGroup>, VariablesGroup>(lifeTime);
        services.AddReadOnlyRepository<IReadOnlyRepository<WorkFlow>, ReadOnlyRepository<WorkFlow>, WorkFlow>(lifeTime);

        return services;
    }

    /// <summary>
    /// Registers logging providers and configures OpenTelemetry logging.
    /// </summary>
    /// <param name="services">The service collection to add the registrations to.</param>
    /// <param name="configuration">The application configuration instance.</param>
    /// <param name="logger">The logger instance for logging information.</param>
    /// <returns>The updated <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddLoggingCollection(this IServiceCollection services, IConfiguration configuration, ILogger logger)
    {
        logger.Information("Common services added to the container");

        // Configure OpenTelemetry Logging

        services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.ClearProviders();
            loggingBuilder.AddSerilog();
        });

        services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.AddSerilog();
                loggingBuilder.AddOpenTelemetry(openTelemetryLoggerOptions =>
               {
                   openTelemetryLoggerOptions.SetResourceBuilder(
                       ResourceBuilder.CreateEmpty()
                           .AddService("IndTrace")
                           .AddAttributes(new Dictionary<string, object>
                           {
                               // Add any desired resource attributes here
                               ["deployment.environment"] = "development",
                           }));

                   // Some important options to improve data quality
                   openTelemetryLoggerOptions.IncludeScopes = true;
                   openTelemetryLoggerOptions.IncludeFormattedMessage = true;

                   openTelemetryLoggerOptions.AddOtlpExporter(exporter =>
                   {
                       // The full endpoint path is required here, when using
                       // the `HttpProtobuf` protocol option.
                       exporter.Endpoint = new Uri("http://localhost:5341/ingest/otlp/v1/logs");
                       exporter.Protocol = OtlpExportProtocol.HttpProtobuf;
                       // Optional `X-Seq-ApiKey` header for authentication, if required
                       exporter.Headers = "X-Seq-ApiKey=abcde12345";
                   });
               });
            });

        return services;
    }

    /// <summary>
    /// Registers CoreAdmin with custom options for the admin panel.
    /// </summary>
    /// <param name="services">The service collection to add the registrations to.</param>
    /// <returns>The updated <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddCoreAdminWithOptions(this IServiceCollection services)
    {
        var coreOptions = new CoreAdminOptions()
        {
            IgnoreEntityTypes = new List<Type>()
            {
                typeof(Recipe),
                typeof(ConfigAppFromJson),
                typeof(ConfigDatabaseLog),
                typeof(ProductSpec),
                typeof(Setting),
                typeof(KpiOee),
                typeof(PerformanceSpec),
                typeof(ConnectionStatus),
                typeof(UserLoginInfo),
                typeof(ConfigDb),
                typeof(Edge),
                typeof(Order),
                typeof(Setting),
                typeof(StoppageRegister),
                typeof(Tooling),
                typeof(IndTraceUser),
                typeof(MachineStatus),
            },
            RestrictToRoles = new string[] { "Admin", "Administrator", "Exxerpro", "Valeo" },
            Title = "IndTrace Admin Panel",
        };

        services.AddCoreAdmin(coreOptions);

        return services;
    }

    /// <summary>
    /// Registers SignalR hub connection factory.
    /// </summary>
    /// <param name="services">The service collection to add the registrations to.</param>
    /// <param name="configuration">The application configuration instance.</param>
    /// <param name="logger">The logger instance for logging information.</param>
    /// <returns>The updated <see cref="IServiceCollection"/>.</returns>
    private static IServiceCollection AddSignalRHub(this IServiceCollection services, IConfiguration configuration, ILogger logger)
    {
        // Kept for backward compatibility; registration now handled by AddHubConnectionAbstractions above
        return services;
    }

    /// <summary>
    /// Registers Blazor, SignalR, and MVC services for the application.
    /// </summary>
    /// <param name="services">The service collection to add the registrations to.</param>
    /// <param name="configuration">The application configuration instance.</param>
    /// <param name="logger">The logger instance for logging information.</param>
    /// <returns>The updated <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddBlazorServices(this IServiceCollection services, IConfiguration configuration, ILogger logger)
    {
        services.AddSignalR();
        services.AddServerSideBlazor(options =>
        {
            options.DetailedErrors = true;
        });
        services.AddRazorPages();
        services.AddRazorComponents()
            .AddInteractiveServerComponents();
        services.AddControllersWithViews();
        return services;
    }

    /// <summary>
    /// Registers the monitor request dispatcher with the specified lifetime.
    /// Handlers and pipeline behaviors must be registered explicitly in the composition root.
    /// </summary>
    /// <param name="services">The service collection to add the registrations to.</param>
    /// <param name="lifetime">The desired service lifetime (Singleton, Scoped, or Transient).</param>
    /// <returns>The updated <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddCommandDispatchers(this IServiceCollection services, ServiceLifetime lifetime)
    {
        switch (lifetime)
        {
            case ServiceLifetime.Singleton:
                services.AddSingleton<IMonitorRequestDispatcher, MonitorRequestDispatcher>();
                break;
            case ServiceLifetime.Scoped:
                services.AddScoped<IMonitorRequestDispatcher, MonitorRequestDispatcher>();
                break;
            case ServiceLifetime.Transient:
                services.AddTransient<IMonitorRequestDispatcher, MonitorRequestDispatcher>();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(lifetime), lifetime, null);
        }

        return services;
    }

    /// <summary>
    /// Registers mediator pipeline behaviors and notification services.
    /// </summary>
    /// <param name="services">The service collection to add the registrations to.</param>
    /// <returns>The updated <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddMediatorAndMapper(this IServiceCollection services)
    {
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(EventLoggerBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        services.AddTransient<INotificationService, IndTraceNotificationService>();

        return services;
    }

    /// <summary>
    /// Registers persistence services, DbContext, and related infrastructure.
    /// </summary>
    /// <param name="services">The service collection to add the registrations to.</param>
    /// <param name="configuration">The application configuration instance.</param>
    /// <param name="logger">The logger instance for logging information.</param>
    /// <returns>The updated <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddIndTracePersistence(this IServiceCollection services, IConfiguration configuration, ILogger logger)
    {
        var connectionString = configuration.GetConnectionString(nameof(IndTraceDbContext))
                               ?? throw new InvalidOperationException($"Connection string '{nameof(IndTraceDbContext)}' not found.");

        if (string.IsNullOrEmpty(connectionString))
        {
            Log.Information("Could not find the 'IndTraceDbContext' connection string.");
            throw new InvalidOperationException($"Could not find the '{nameof(IndTraceDbContext)}'connection string.");
        }
        //TODO [VERIFY]
        //ABR CHECK THIS STILL WORK, BECAUSE I DON'T HAVE REGISTERED A CONTEXT FACTORY IN THE PERSISTENCE PROJECT
        // I HAVE A CONTEXT FACTORY ON THE CLIENTS CLASS
        // DbContext registration updated to use AddPooledDbContextFactory for improved performance and thread safety.
        // This allows IDbContextFactory<IndTraceDbContext> to provide pooled DbContext instances.
        // Change applied: 2025-06-12
        services.AddPooledDbContextFactory<IndTraceDbContext>(options =>
            options.UseSqlServer(connectionString, actions =>
                {
                    actions.MigrationsAssembly(typeof(IndTraceDbContext).Assembly.FullName)
                        .EnableRetryOnFailure(maxRetryCount: 4, maxRetryDelay: TimeSpan.FromSeconds(2),
                            errorNumbersToAdd: []);
                })
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors()
                .ConfigureWarnings(warnings =>
                {
                    warnings.Default(WarningBehavior.Log).Log(CoreEventId.SaveChangesCompleted, CoreEventId.FirstWithoutOrderByAndFilterWarning, CoreEventId.RowLimitingOperationWithoutOrderByWarning);
                }));

        //TODO ADD DATABASE FOR HANGFIRE
        var hangFireString = configuration.GetConnectionString(nameof(IndTraceDbContext))
                               ?? throw new InvalidOperationException($"Connection string '{nameof(IndTraceDbContext)}' not found.");

        if (string.IsNullOrEmpty(connectionString))
        {
            Log.Information("Could not find the 'IndTraceDbContext' connection string.");
            throw new InvalidOperationException($"Could not find the '{nameof(IndTraceDbContext)}'connection string.");
        }

        // Add Hangfire services

        //TODO REVIEW THE CONFIGURATION

        /*
        services.AddHangfire(config =>
            config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(connectionString, new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.FromSeconds(15),
                    UseRecommendedIsolationLevel = true,
                    DisableGlobalLocks = true
                }));

        */

        // Add the Hangfire server
        //TODO REVIEW THE CONFIGURATION

        //services.AddHangfireServer();

        // Register your application services
        services.AddScoped<DistinctRegisterService>();
        services.AddScoped<IIndTraceDbContext, IndTraceDbContext>();
        services.AddScoped<IIndTraceDbContextFactory, IndTraceDbContextFactory>();

        services.AddDatabaseDeveloperPageExceptionFilter();

        return services;
    }

    /// <summary>
    /// Creates and configures a Serilog logger for the host application builder.
    /// </summary>
    /// <param name="builder">The host application builder.</param>
    /// <param name="configuration">The application configuration instance.</param>
    /// <returns>The configured <see cref="ILogger"/>.</returns>
    public static ILogger CreateLogger(this HostApplicationBuilder builder, IConfiguration configuration)
    {
        var logger = Log.Logger = new LoggerConfiguration()
             .ReadFrom.Configuration(configuration) // Optional, if you wish to configure via appsettings.json
             .Enrich.FromLogContext()
             .WriteTo.Console()
             .WriteTo.File("Logs/logs.txt", rollingInterval: RollingInterval.Day)
             .WriteTo.Seq(
                 serverUrl: "http://localhost:5341",
                 apiKey: null, // API key is not used in this example
                 controlLevelSwitch: null, // No control level switch is provided
                 messageHandler: new HttpClientHandler
                 {
                     Credentials = new NetworkCredential("admin", "admin"),
                 }
             )
             .CreateLogger();

        return logger;
    }

    /// <summary>
    /// Creates and configures a Serilog logger for the web application builder.
    /// </summary>
    /// <param name="builder">The web application builder.</param>
    /// <param name="configuration">The application configuration instance.</param>
    /// <returns>The configured <see cref="ILogger"/>.</returns>
    public static ILogger CreateLogger(this WebApplicationBuilder builder, IConfiguration configuration)
    {
        var logger = Log.Logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(configuration) // Optional, if you wish to configure via appsettings.json
                    .Enrich.FromLogContext()
                    .WriteTo.Console()
                    .WriteTo.File("Logs/logs.txt", rollingInterval: RollingInterval.Day)
                    .WriteTo.Seq(
                        serverUrl: "http://localhost:5341",
                        apiKey: null, // API key is not used in this example
                        controlLevelSwitch: null, // No control level switch is provided
                        messageHandler: new HttpClientHandler
                        {
                            Credentials = new NetworkCredential("admin", "admin"),
                        }
                    )
                    .CreateLogger();

        return logger;
    }
}
