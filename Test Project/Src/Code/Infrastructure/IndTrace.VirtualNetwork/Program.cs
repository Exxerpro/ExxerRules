using IndTrace.Dependencies.Interceptors;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Polly;
using IndTrace.Dependencies.Simulations;
using Serilog;
using System.Data;

namespace IndTrace.VirtualNetwork;

/// <summary>
/// The main entry point for the IndTrace.VirtualNetwork application.
/// This application acts as a standalone background simulator with minimal dependencies.
/// </summary>
public class Program
{
    /// <summary>
    /// The main method that runs the application.
    /// </summary>
    /// <param name="args">Command-line arguments passed to the application.</param>
    public static void Main(string[] args)
    {
        // Check if the application is running as Administrator
        // If not, relaunch it with administrative privileges
        // This is necessary for certain operations that require elevated permissions
        // Check if the application is running as Administrator

        /*
         * IndTrace.VirtualNetwork
         * -----------------------
         * A standalone background simulator with minimal dependencies.
         *
         * ⚠️ Requires Administrator rights to add virtual NICs.
         * ✅ Run via Visual Studio *as Administrator* when debugging.
         * 🧪 Debug mode will assert if not elevated to prevent accidental restarts.
         * 📦 Production auto-restarts with elevated privileges if needed.
         *
         * Author: Exxerpro Solutions
         */

        // ⚡ Bootstrap logger (early log capture before host is built)

        // ⚡ Bootstrap logger (early log capture before host is built)
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .WriteTo.File("logs/bootstrap.log", rollingInterval: RollingInterval.Day)
            .CreateBootstrapLogger();

        try
        {
            Log.Information("🔧 Starting application setup...");

            var builder = Host.CreateApplicationBuilder(args);
            var environment = builder.Environment;
            // Configure Serilog
            var logger = Log.Logger = new LoggerConfiguration()
                 .ReadFrom.Configuration(builder.Configuration)
                 .Enrich.FromLogContext()
                 .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                 .WriteTo.Async(a => a.File("logs/log.txt", rollingInterval: RollingInterval.Day))
                 .Enrich.FromLogContext()
                 .WriteTo.Seq("http://localhost:5341")
                 .CreateLogger();

            // Clear default logging providers and add Serilog
            builder.Logging.ClearProviders();
            builder.Logging.AddSerilog();

            // Load native library for Snap7
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            });
            Microsoft.Extensions.Logging.ILogger loggerSnap7 = loggerFactory.CreateLogger("NativeLoader");

            NativeLibLoader.LoadNativeLibrary("snap7", loggerSnap7);

            // Admin check
            if (!Runners.IsRunningAsAdministrator())
            {
                if (environment.IsDevelopment())
                {
                    logger.Information("⚠️ You must run this project as Administrator in Development mode.");
                    logger.Information("💡 Tip: Right-click on Visual Studio and choose 'Run as Administrator'.");
                    Debug.Fail("Not running as Administrator.");
                    Console.ReadLine();
                    return;
                }
                else
                {
                    Runners.RelaunchAsAdministrator();
                    return;
                }
            }

            Microsoft.Extensions.Logging.ILogger msLogger = loggerFactory.CreateLogger("Runners");

            var windowTitle = Runners.EnsureProgramIsSingleton(msLogger);

            // Global exception logging
            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                Log.Fatal((Exception)args.ExceptionObject, "Unhandled exception occurred");
                Log.CloseAndFlush();
                logger.Information("Unhandled exception. Press Enter to exit.");
                Console.ReadLine();
            };

            builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            // Load externalized, centralized config
            var configuration = ConfigLoader.Load();

            builder.Services.AddOptions<SimulationSettings>()
                .Bind(builder.Configuration.GetSection("SimulationSettings"))
                .ValidateDataAnnotations();

            //[Fix]
            //CLAUDE
            //Date: 27/09/2025
            //Reason: [Missing Registration] - Added missing PlcDbOptions required by PlcDbTagsRepository factory
            builder.Services.Configure<PlcDbOptions>(
                builder.Configuration.GetSection("PlcDb"));

            builder.Services.AddSingleton<LoggerFactory>();
            builder.Services.AddSingleton<CommandSimulator>();
            builder.Services.AddHostedService<VirtualNetworkWorker>();
            var connectionString = configuration.GetConnectionString("IndTraceDbContext");

            builder.Services.AddSingleton<IDbConnection>(sp =>
                new SqlConnection(configuration.GetConnectionString("IndTraceDbContext")));

            //Debug.Assert(string.IsNullOrWhiteSpace(connectionString));

            try
            {
                var retryPolicy = Polly.Policy
                    .Handle<SqlException>()
                    .WaitAndRetry(3, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                        (exception, timeSpan, retryCount, context) =>
                        {
                            Log.Warning($"⏳ Retry {retryCount} due to SQL error: {exception.Message}");
                        });

                retryPolicy.Execute(() =>
                {
                    using var conn = new SqlConnection(connectionString);
                    conn.Open();
                    Log.Information("✅ Database connection successful.");
                });
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "❌ Database connection failed after retries.");
                Environment.ExitCode = 1;
                return;
            }

            builder.Services.TryAddSingleton<SimulationEngine>();

            builder.Services.AddSingleton<PlcDbTagsRepository>(sp =>
            {
                var db = sp.GetRequiredService<IDbConnection>();
                var loggerIndTrace = sp.GetRequiredService<ILogger<PlcDbTagsRepository>>();

                var options = sp.GetRequiredService<IOptions<PlcDbOptions>>();
                return new PlcDbTagsRepository(options, loggerIndTrace, db);
            });

            var loggerConfiguration = Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .Enrich.FromLogContext()
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                .WriteTo.Async(a => a.File("logs/log.txt", rollingInterval: RollingInterval.Day))
                .Enrich.FromLogContext()
                .WriteTo.Async(s => s.Seq("http://localhost:5341"))
                .CreateLogger();

            // Clear default logging providers and add Serilog
            builder.Logging.ClearProviders();
            builder.Logging.AddSerilog();

            var host = builder.Build();
            host.Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "💥 Application terminated unexpectedly");
        }
        finally
        {
            Log.Information("🔚 Application shutting down.");
            Log.CloseAndFlush();
            Console.ReadLine();
            Console.WriteLine("Press Enter to exit.");
            Console.ReadLine();
        }
    }
}
