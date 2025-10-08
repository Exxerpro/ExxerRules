using BlazorDownloadFile;
using IndTrace.Application.BarCodes.Queries.GetBarCodeDetailMonitor;
using IndTrace.Application.BarCodes.Queries.GetReportsList.FiltersInfo;
using IndTrace.Application.BarCodes.Queries.GetReportsList.GetList;
using IndTrace.Application.BarCodes.Queries.GetReportsReport;
using IndTrace.Application.BarCodes.Services;
using IndTrace.Application.ConfigStations.Queries.GetConfigStationList;
using IndTrace.Application.Configuration.Services;
using IndTrace.Application.Models.CacheServices;
using IndTrace.Domain.Models;
using IndTrace.Application.Models.Helpers;
using IndTrace.Application.Models.Interfaces;
using IndTrace.Application.Models.Services;
using IndTrace.Application.Products.Services;
using IndTrace.Application.Shifts.Commands.Create;
using IndTrace.Application.Shifts.Services;
using IndTrace.Application.UI.Models;
using IndTrace.Application.UI.Services;
using IndTrace.Application.UserService;
using IndTrace.CognexComm;
using IndTrace.Dependencies;
using IndTrace.HubConnection.Extensions;
using IndTrace.Dependencies.Injections;
using IndTrace.Dependencies.Interceptors;
using IndTrace.Dependencies.Middleware;
using IndTrace.Dependencies.Services;
using IndTrace.Dependencies.Startup;
using IndTrace.Dependencies.Utilities;
using IndTrace.HubConnection.Validators;
using IndTrace.Domain.Entities.BarCodes;
using IndTrace.Domain.Interfaces;
using IndTrace.Monitor.Components;
using IndTrace.Monitor.Components.Account;
using IndTrace.Monitor.Worker;
using IndTrace.UI.Models.Shifts;
using IndTrace.UI.Models.Users;
using Microsoft.Extensions.Options;
using Serilog;
using IndTrace.Persistence.Services;

namespace IndTrace.Monitor;

/// <summary>
/// The main program class for the IndTrace Monitor application.
/// </summary>
public class Program
{
    /// <summary>
    /// The main entry point for the IndTrace Monitor application.
    /// </summary>
    /// <param name="args">Command line arguments.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task Main(string[] args)
    {
        // Load externalized, centralized config
        var configuration = ConfigLoader.Load();

        var builder = WebApplication.CreateBuilder(args);

        var logger = builder.CreateLogger(configuration);

        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddSerilog(); // Use the already configured Serilog instance
        });
        Microsoft.Extensions.Logging.ILogger msLogger = loggerFactory.CreateLogger("Runners");

        if (!builder.Environment.IsEnvironment("Test"))
        {
            var windowTitle = Runners.EnsureProgramIsSingleton(msLogger);
        }
        // Set console title here
        Console.Title = "Monitor";

        logger.Information("Starting to build services");

        builder.Services.AddHealthChecks();

        builder.Services.AddCommandDispatchers(ServiceLifetime.Scoped);
        builder.Services.AddBlazorServices(configuration, logger);

        builder.Services.AddCommonServices(configuration, logger);

        builder.Services.AddEventsServices(configuration, logger);

        builder.Services.AddLoggingCollection(configuration, logger);

        //[Fix]
        //CLAUDE
        //Date: 27/09/2025
        //Reason: [DI Registration] - Removed duplicate AddCascadingAuthenticationState() (was called on lines 87 and 91)
        builder.Services.AddCascadingAuthenticationState();

        builder.Services.AddIndTracePersistence(configuration, logger);

        // Skip Identity wiring in integration test environment to avoid EF store user type mismatch
        if (!builder.Environment.IsEnvironment("Test"))
        {
            builder.Services.AddIndTraceIdentity(configuration, logger);
        }

        builder.Services.AddBlazorDownloadFile();

        // Bind the DataManSettings section from appsettings.json
        builder.Services.Configure<DataManSettings>(configuration.GetSection("DataManSettings"));

        // Register the DataMan class with dependency injection
        builder.Services.AddSingleton<DataMan>();
        builder.Services.AddSingleton<SystemMetricsService>();

        builder.Logging.ClearProviders();
        builder.Logging.AddSerilog(logger, dispose: true);

        // Services only for IndTrace.Monitor

        builder.Services.AddScoped<FixedSizedStack<string>>();
        builder.Services.AddScoped<Dictionary<int, ControllerMonitor>>();
        builder.Services.AddScoped<ShiftModel>();

        //builder.Services.AddSingleton<OeeState>();
        //builder.Services.AddScoped<IndFusionWorker>();

        // HubMonitorOptions and its validator are registered via AddHubConnectionAbstractions

        builder.Services.AddSingleton<IUserOfflineCreationService, UserOfflineCreationService>();

        builder.Services.AddScoped<UserInputModel>();
        builder.Services.AddScoped<IProductService, ProductService>();
        builder.Services.AddScoped<IShiftService, ShiftService>();

        builder.Services.AddScoped<IIndTraceUserService, IndTraceUserService>();

        builder.Services.AddRepositories(ServiceLifetime.Scoped);
        builder.Services.AddReadOnlyRepositories(ServiceLifetime.Scoped);

        builder.Services.AddScoped<IBarCodeValidationService, BarCodeValidationService>();
        builder.Services.AddScoped<IBarCodeResult, BarCodeResult>();

        builder.Services.AddHostedService<HubMonitorWorker>();

        // Register DB health check background service
        builder.Services.AddHostedService<DatabaseHealthCheckService>();

        builder.Services.AddHubConnectionAbstractions(configuration);

        //[Fix]
        //CLAUDE
        //Date: 27/09/2025
        //Reason: [DI Registration] - Changed IndTraceConfigurationService to Scoped only (was registered as both Singleton and Scoped)
        builder.Services.AddScoped<IndTraceConfigurationService>();

        builder.Services.AddSingleton<IndTraceEventsService>();

        //[Fix]
        //CLAUDE
        //Date: 27/09/2025
        //Reason: [DI Registration] - Changed IDateTimeMachine to Scoped only (was registered as both Singleton and Scoped)
        builder.Services.AddScoped<IDateTimeMachine, DateTimeMachine>();

        builder.Services.AddScoped<CreateShiftCommand>();

        builder.Services.AddScoped<IIsOeeEnabledChecker, IsOeeEnabledChecker>();
        builder.Services.AddSingleton<CacheManager<ApplicationConfiguration>>(
            sp => new CacheManager<ApplicationConfiguration>(TimeSpan.FromMinutes(60)));

        builder.Services.AddTransient<AppDetailsFactory>();

        builder.Services.AddTransient<IMonitorQueryHandler<GetBarCodeDetailMonitorQuery, BarCodeDetailMonitorVm>, GetBarCodeDetailMonitorMonitorQueryHandler>();
        builder.Services.AddTransient<IMonitorQueryHandler<GetReportsListQuery, BarCodesListVm>, GetReportsListMonitorQueryHandler>();

        builder.Services.AddTransient<IMonitorQueryHandler<GetBarCodeReportQuery, List<BarCodeReportVm>>, GetBarCodeReportQueryHandler>();
        builder.Services.AddTransient<IMonitorRequestHandler<GetBarCodeDetailQrCodeQuery, BarCodeDetailMonitorVm>, GetBarCodeDetailQueryQrCodeHandler>();
        builder.Services.AddTransient<IMonitorQueryHandler<GetReportsFilterInfoQuery, ReportsFilterInfoVm>, GetReportesFilterInfoMonitorQueryHandler>();

        builder.Services.AddTransient<IMonitorRequestHandler<GetAppDetailsMonitorRequest, ApplicationConfiguration>, GetAppDetailsMonitorRequestHandler>();

        //[Fix]
        //CLAUDE
        //Date: 27/09/2025
        //Reason: [DI Registration] - Removed duplicate AddHealthChecks() (was already called on line 76)
        builder.Services.AddScoped<HttpClientInterceptor>();
        builder.Services.AddScoped(sp =>
        {
            var logger = sp.GetRequiredService<ILogger<HttpClientInterceptor>>();
            var interceptor = new HttpClientInterceptor(logger);

            return new HttpClient(interceptor)
            {
                BaseAddress = new Uri(builder.Configuration["applicationUrl"] ?? throw new InvalidOperationException("ApiBaseUrl configuration is missing")),
            };
        });

        builder.Services.AddCoreAdminWithOptions();

        //suspending because is just a prototype
        //TODO FINISH AND ENABLE
        //builder.Services.AddHostedService<IndFusionWorker>();
        try
        {
            // Build application

            logger.Information("Starting to build application");

            var app = builder.Build();

            app.UseCommonPipeline(configuration, logger);

            //TODO DISABLE TO DEBUG THE SIGNALR HUB
            // Use Hangfire Dashboard (optional)
            // TODO DETECT MOMENTS WHEN THERE IS NO ACTIVITY IN THE MONITOR
            //app.UseHangfireDashboard();

            // Example: Scheduling the recurring job
            //var recurringJobManager = app.Services.GetRequiredService<IRecurringJobManager>();
            //recurringJobManager.AddOrUpdate<DistinctRegisterService>(
            //    "UpdateDistinctRegisters",
            //    service => service.UpdateDistinctRegistersAsync(CancellationToken.None),
            //    Cron.Daily);

            // Map your endpoints here
            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.MapRazorPages();
            app.MapControllers();

            // Add additional endpoints required by the Identity /Account Razor components.
            app.MapAdditionalIdentityEndpoints();

            app.UseCoreAdminCustomUrl("AdminPanel");
            app.UseCoreAdminCustomTitle("IndTrace Admin Panel");

            // Register health check middleware
            app.MapHealthChecks("/health").ShortCircuit();

            Log.Information("Starting web host");
            await app.RunAsync();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application start-up failed");
        }
        finally
        {
            await Log.CloseAndFlushAsync();
        }
    }
}
