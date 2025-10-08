// <copyright file="Program.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Simulator
{
    using IndTrace.DataStore.DataAccess;
    using IndTrace.DataStore.IModelsComs;
    using IndTrace.DataStore.Interfaces;
    using IndTrace.DataStore.Models;
    using IndTrace.DataStore.ModelsComs;
    using IndTrace.Simulator.Comms;
    using IndTrace.Simulator.Export;
    using IndTrace.Simulator.Mocks;
    using IndTrace.Simulator.Simulation;
    using IndTrace.Simulator.Validation;
    using Microsoft.Data.SqlClient;
    using Microsoft.Extensions.Options;
    using Serilog;

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
            var builder = Host.CreateApplicationBuilder(args);

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            Console.Title = "IndTrace Fixture Generator";
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            //[Fix]
            //CLAUDE
            //Date: 27/09/2025
            //Reason: [DI Registration] - Removed duplicate Serilog configuration (already configured above)

            var logFilePath = configuration["Serilog:WriteTo:2:Args:path"]; // Adjust index if needed
            Log.Information("Serilog is logging to file: {LogFilePath}", logFilePath);

            Log.Information("Logging configured");

            builder.Services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.AddSerilog();
            });

            builder.Services.Configure<DryRunOptions>(builder.Configuration.GetSection("DryRunOptions"));

            var provider = builder.Services.BuildServiceProvider();
            var options = provider.GetRequiredService<IOptions<DryRunOptions>>().Value;

            Log.Information(options.ToString());

            builder.Services.AddSingleton<IDbConnection>(sp =>
                new SqlConnection(configuration.GetConnectionString("IndTraceDbContext")));

            builder.Services.AddSingleton<IProductRepository, ProductRepository>();

            builder.Services.AddSingleton<IPlcClient, S7PlcClient>();
            builder.Services.AddSingleton<ITestTagWriter, MockTestTagWriter>();
            builder.Services.AddSingleton<IFixtureExporter, MockFixtureExporter>();

            builder.Services.AddSingleton<IMachineResolver, MachineResolver>();

            //[Fix]
            //CLAUDE
            //Date: 27/09/2025
            //Reason: [DI Registration] - Removed duplicate IFixtureDb registration (was registered twice)
            builder.Services.AddSingleton<IFixtureDb, FixtureDb>();
            builder.Services.AddSingleton<IFixtureValidator, FixtureValidator>();

            builder.Services.AddSingleton<IMachineStateEvaluator, MachineStateEvaluator>();

            builder.Services.AddSingleton<ITagsRepository, PlcDbTagsRepository>();

            //[Fix]
            //CLAUDE
            //Date: 27/09/2025
            //Reason: [DI Registration] - Removed duplicate MachineResolver registration (already registered as IMachineResolver on line 78)
            builder.Services.AddSingleton<BarcodeVerifier>();
            builder.Services.AddSingleton<IFixtureStore>(sp => new CsvFixtureStore("fixtures.csv"));
            builder.Services.AddSingleton<ITestPathRunner, TestPathRunner>();
            builder.Services.AddHostedService<FixtureHostedService>();

            var host = builder.Build();
            host.Run();
        }
    }
}
