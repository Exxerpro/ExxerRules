using IndTrace.Application.Common.DateTime;
using IndTrace.Application.Common.Interfaces;
using IndTrace.Persistence.DBContext;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace IndTrace.DataSeeder
{
    /// <summary>
    /// Represents the Program.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Executes Main operation.
        /// </summary>
        /// <param name="args">The args.</param>
        public static void Main(string[] args)
        {
            //TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate data seeder program logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console() // If you also want to log to console
                .WriteTo.File("logs/myapp.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            Log.Information("This is an informational message.");
            Log.Error("This is an error message.");


            IHost host = Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    services.AddHostedService<Worker>();
                    services.AddScoped<IIndTraceDbContext, IndTraceDbContext>();
                    services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog());

                    //Main database of the application

                    services.AddSingleton<IIndTraceUserService, SeederUserService>();
                    services.AddSingleton<IDateTimeMachine, DateTimeMachine>();

                    //SeederUserService : IIndTraceUserService
                    //SeederDateTimeService : IDateTimeMachine
                    const string connectionStringApp = "Server=ADMINISTRADOR;Database=IndTraceData;Trusted_Connection=True;Encrypt=False;TrustServerCertificate=True;";
                    if (string.IsNullOrWhiteSpace(connectionStringApp)) throw new NullReferenceException($"connection string IndTraceDbContext is not valid {nameof(connectionStringApp)}");



                    services.AddDbContext<IndTraceDbContext>(options =>
                        options
                            .EnableDetailedErrors()
                            .UseSqlServer(connectionStringApp,
                                b =>
                                {
                                    b.MigrationsAssembly(typeof(IndTraceDbContext).Assembly.FullName);
                                    //   b.EnableRetryOnFailure(); // Add this line to enable retry logic

                                })

                        , ServiceLifetime.Singleton);



                }).Build();

            host.Run();


            Log.CloseAndFlush();

        }
    }
}
