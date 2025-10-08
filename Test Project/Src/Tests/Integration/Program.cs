using BlazorDownloadFile;
using IndTrace.Application.BarCodes.Queries.GetBarCodeDetailMonitor;
using IndTrace.Application.BarCodes.Queries.GetReportsDetails.GetBarCodeDetail;
using IndTrace.Application.BarCodes.Queries.GetReportsList.FiltersInfo;
using IndTrace.Application.BarCodes.Queries.GetReportsList.GetList;
using IndTrace.Application.BarCodes.Services;
using IndTrace.Application.ConfigStations.Queries.GetConfigStationList;
using IndTrace.Application.Configuration.Services;
using IndTrace.Application.Models.CacheServices;
using IndTrace.Application.Models.DateTimeModels;
using IndTrace.Application.Models.Extensions;
using IndTrace.Application.Models.Helpers;
using IndTrace.Application.Models.Interfaces;
using IndTrace.Application.Models.RequestHandler;
using IndTrace.Application.Products.Services;
using IndTrace.Application.Shifts.Commands.Create;
using IndTrace.Application.Shifts.Services;
using IndTrace.Application.UI.Models;
using IndTrace.Application.UI.Services;
using IndTrace.Application.UserService;
using IndTrace.CognexComm;
using IndTrace.Dependencies;
using IndTrace.Domain.Entities.BarCodes;
using IndTrace.Domain.Interfaces;
using IndTrace.Monitor.Components;
using IndTrace.Monitor.Components.Account;
using IndTrace.Monitor.Extensions;
using IndTrace.Monitor.Services;
using IndTrace.Monitor.Worker;
using IndTrace.HubConnection.Extensions;
using IndTrace.UI.Models.Shifts;
using IndTrace.UI.Models.Users;

using Serilog;

namespace IndTrace.Monitor;
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
        ProcessExtensions.VerifyIfApplicationIsNotRunning("IndTrace.Monitor");

        var builder = WebApplication.CreateBuilder(args);

        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();
        var logger = builder.CreateLogger(configuration);

        // Set console title here
        Console.Title = "Monitor";

        logger.Information("Starting to build services");

        builder.Services.AddMediator(ServiceLifetime.Scoped);
        builder.Services.AddBlazorServices(configuration, logger);

        builder.Services.AddCommonServices(configuration, logger);

        builder.Services.AddEventsServices(configuration, logger);

        builder.Services.AddLoggingCollection(configuration, logger);

        builder.Services.AddCascadingAuthenticationState();

        builder.Services.AddIndTracePersistence(configuration, logger);

        builder.Services.AddCascadingAuthenticationState();

        builder.Services.AddIndTraceIdentity(configuration, logger);

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

        // Register Hub connection abstractions (IHubConnection, factory, metrics dashboard)
        builder.Services.AddHubConnectionAbstractions(builder.Configuration);

        builder.Services.AddSingleton<IndTraceConfigurationService>();

        builder.Services.AddSingleton<IndTraceEventsService>();

        builder.Services.AddScoped<IndTraceConfigurationService>();

        builder.Services.AddSingleton<IDateTimeMachine, DateTimeMachine>();
        builder.Services.AddScoped<IDateTimeMachine, DateTimeMachine>();

        builder.Services.AddScoped<CreateShiftCommand>();

        builder.Services.AddSingleton<CacheManager<ApplicationConfiguration>>(
            sp => new CacheManager<ApplicationConfiguration>(TimeSpan.FromMinutes(60)));

        builder.Services.AddTransient<AppDetailsFactory>();

        builder.Services.AddTransient<IGuiQueryHandler<GetBarCodeDetailMonitorQuery, BarCodeDetailMonitorVm>, GetBarCodeDetailMonitorGuiQueryHandler>();
        builder.Services.AddTransient<IGuiQueryHandler<GetReportsListQuery, BarCodesListVm>, GetReportsListGuiQueryHandler>();

        builder.Services.AddTransient<IGuiQueryHandler<GetReportDetailQuery, List<BarCodeReportVm>>, GetBarCodeDetailQueryHandler>();
		builder.Services.AddTransient<IGuiQueryHandler<GetReportsFilterInfoQuery, ReportsFilterInfoVm>, GetReportesFilterInfoGuiQueryHandler>();

        builder.Services.AddTransient<IGuiRequestHandler<GetAppDetailsGuiRequest, ApplicationConfiguration>, GetAppDetailsGuiRequestHandler>();


        // Add services to the container.
        builder.Services.AddHealthChecks();
        builder.Services.AddScoped<HttpClientInterceptor>();
        builder.Services.AddScoped(sp =>
        {
            var logger = sp.GetRequiredService<ILogger<HttpClientInterceptor>>();
            var interceptor = new HttpClientInterceptor(logger);

            return new HttpClient(interceptor)
            {
                BaseAddress = new Uri(builder.Configuration["applicationUrl"] ?? throw new InvalidOperationException("ApiBaseUrl configuration is missing"))
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
