// Src/Tests/Integration/Indtrace.Integration.Tests/Infrastructure/TestHostFixture.cs
using IndTrace.Application.BarCodes.Queries.GetBarCodeDetailMonitor;
using IndTrace.Application.BarCodes.Queries.GetReportsList.FiltersInfo;
using IndTrace.Application.BarCodes.Queries.GetReportsList.GetList;
using IndTrace.Application.BarCodes.Queries.GetReportsReport;
using IndTrace.Application.ConfigStations.Queries.GetConfigStationList;
using IndTrace.Application.Configuration.Services;
using IndTrace.Application.Models.RequestHandler;
using IndTrace.Application.Repository;
using IndTrace.Domain.Entities;
using IndTrace.Domain.Interfaces;
using IndTrace.Persistence.Interfaces;
using IndTrace.Persistence.Repositories;
using IndTrace.Persistence.Caching;
using ZiggyCreatures.Caching.Fusion;
using ZiggyCreatures.Caching.Fusion.Serialization.SystemTextJson;
using System.Text.Json;
using Integration.Tests.Utilities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Xunit.Sdk;
using System;
using System.Threading.Tasks;
using Xunit;
using IndTrace.Persistence.Converters;

namespace Integration.Tests.Infrastructure;

/// <summary>
/// Minimal TestServer host for DI-only integration tests (no production Program).
/// Registers test-only services and keyed persistence from configuration.
/// </summary>
public sealed class TestHostFixture : IAsyncLifetime, IAsyncDisposable
{
    private IHost _host = default!;

    /// <summary>
    /// Gets the service provider for resolving test services.
    /// </summary>
    public IServiceProvider Services => _host.Services;

    public async ValueTask InitializeAsync()
    {
        _host = new HostBuilder()
            .ConfigureAppConfiguration((ctx, cfg) =>
            {
                var env = ctx.HostingEnvironment;
                cfg.AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                   .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: false);

                var baseDir = AppContext.BaseDirectory;
                var testJson = Path.Combine(baseDir, "appsettings.json");
                var testEnvJson = Path.Combine(baseDir, $"appsettings.{env.EnvironmentName}.json");
                if (File.Exists(testJson)) cfg.AddJsonFile(testJson, optional: true, reloadOnChange: false);
                if (File.Exists(testEnvJson)) cfg.AddJsonFile(testEnvJson, optional: true, reloadOnChange: false);
            })
            .ConfigureWebHost(web =>
            {
                web.UseTestServer();
                web.ConfigureServices((ctx, services) =>
                {
                    // Minimal infra needed for repositories + route logs to xUnit output
                    services.AddLogging(lb =>
                    {
                        lb.ClearProviders();
                        lb.AddProvider(new TestContextLoggerProvider());
                        lb.SetMinimumLevel(LogLevel.Debug);
                    });

                    // Add ICacheService for repository caching
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

                    services.AddSingleton<ICacheService, FusionCacheService>();

                    services.AddScoped<IDateTimeMachine, DateTimeMachine>();
                    services.AddScoped<IBarCodeValidationService, BarCodeValidationService>();

                    // Fail fast / skip if no connection strings are available
                    {
                        var cfg = ctx.Configuration;
                        var hasAnyConn = false;
                        foreach (var k in Integration.Tests.Utilities.DbProfiles.Allowed)
                        {
                            if (!string.IsNullOrWhiteSpace(cfg[$"ConnectionStrings:{k}"]))
                            {
                                hasAnyConn = true; break;
                            }
                        }
                        if (Integration.Tests.Utilities.DbSkipConditions.IsCiSkip || !hasAnyConn)
                        {
                            throw new InvalidOperationException("Integration tests disabled: missing required ConnectionStrings or SKIP_DB_TESTS set.");
                        }
                    }

                    // Test-only DI (no production Program)
                    services.AddCommonServicesTest(ctx.Configuration);
                    services.AddKeyedPersistenceFromConfigTest(ctx.Configuration, logger: null!);

                    // Register IMonitorRequestDispatcher manually (no assembly scanning)
                    // Register Commands and handlers manually as well
                    services.AddScoped<IMonitorRequestDispatcher, MonitorRequestDispatcher>();

                    services.AddTransient<AppDetailsFactory>();

                    services.AddTransient<IMonitorQueryHandler<GetBarCodeDetailMonitorQuery, BarCodeDetailMonitorVm>, GetBarCodeDetailMonitorMonitorQueryHandler>();
                    services.AddTransient<IMonitorQueryHandler<GetReportsListQuery, BarCodesListVm>, GetReportsListMonitorQueryHandler>();

                    services.AddTransient<IMonitorQueryHandler<GetBarCodeReportQuery, List<BarCodeReportVm>>, GetBarCodeReportQueryHandler>();
                    services.AddTransient<IMonitorQueryHandler<GetReportsFilterInfoQuery, ReportsFilterInfoVm>, GetReportesFilterInfoMonitorQueryHandler>();

                    services.AddTransient<IMonitorRequestHandler<GetAppDetailsMonitorRequest, ApplicationConfiguration>, GetAppDetailsMonitorRequestHandler>();

                    // Register complete keyed repositories from CommonServiceRegistration pattern
                    foreach (var key in DbProfiles.Allowed)
                    {
                        // Core read/write repositories - matching CommonServiceRegistration.AddRepositories
                        services.AddKeyedScoped<IRepository<BarCode>>(key, (sp, _) =>
                        {
                            var factory = sp.GetRequiredKeyedService<IIndTraceDbContextFactory>(key);
                            var logger = sp.GetRequiredService<ILogger<Repository<BarCode>>>();
                            return new Repository<BarCode>(factory, logger);
                        });

                        services.AddKeyedScoped<IRepository<ConfigApp>>(key, (sp, _) =>
                        {
                            var factory = sp.GetRequiredKeyedService<IIndTraceDbContextFactory>(key);
                            var logger = sp.GetRequiredService<ILogger<Repository<ConfigApp>>>();
                            return new Repository<ConfigApp>(factory, logger);
                        });

                        services.AddKeyedScoped<IRepository<Customer>>(key, (sp, _) =>
                        {
                            var factory = sp.GetRequiredKeyedService<IIndTraceDbContextFactory>(key);
                            var logger = sp.GetRequiredService<ILogger<Repository<Customer>>>();
                            return new Repository<Customer>(factory, logger);
                        });

                        services.AddKeyedScoped<IRepository<Cycle>>(key, (sp, _) =>
                        {
                            var factory = sp.GetRequiredKeyedService<IIndTraceDbContextFactory>(key);
                            var logger = sp.GetRequiredService<ILogger<Repository<Cycle>>>();
                            return new Repository<Cycle>(factory, logger);
                        });

                        services.AddKeyedScoped<IRepository<DistinctRegister>>(key, (sp, _) =>
                        {
                            var factory = sp.GetRequiredKeyedService<IIndTraceDbContextFactory>(key);
                            var logger = sp.GetRequiredService<ILogger<Repository<DistinctRegister>>>();
                            return new Repository<DistinctRegister>(factory, logger);
                        });

                        services.AddKeyedScoped<IRepository<Line>>(key, (sp, _) =>
                        {
                            var factory = sp.GetRequiredKeyedService<IIndTraceDbContextFactory>(key);
                            var logger = sp.GetRequiredService<ILogger<Repository<Line>>>();
                            return new Repository<Line>(factory, logger);
                        });

                        services.AddKeyedScoped<IRepository<Machine>>(key, (sp, _) =>
                        {
                            var factory = sp.GetRequiredKeyedService<IIndTraceDbContextFactory>(key);
                            var logger = sp.GetRequiredService<ILogger<Repository<Machine>>>();
                            return new Repository<Machine>(factory, logger);
                        });

                        services.AddKeyedScoped<IReadOnlyRepository<Machine>>(key, (sp, _) =>
                        {
                            var factory = sp.GetRequiredKeyedService<IIndTraceDbContextFactory>(key);
                            var cache = sp.GetRequiredService<ICacheService>();
                            var logger = sp.GetRequiredService<ILogger<ReadOnlyRepository<Machine>>>();
                            return new ReadOnlyRepository<Machine>(factory, cache, logger);
                        });
                        services.AddKeyedScoped<IRepository<MachinePlc>>(key, (sp, _) =>
                        {
                            var factory = sp.GetRequiredKeyedService<IIndTraceDbContextFactory>(key);
                            var logger = sp.GetRequiredService<ILogger<Repository<MachinePlc>>>();
                            return new Repository<MachinePlc>(factory, logger);
                        });

                        services.AddKeyedScoped<IRepository<MasterLabel>>(key, (sp, _) =>
                        {
                            var factory = sp.GetRequiredKeyedService<IIndTraceDbContextFactory>(key);
                            var logger = sp.GetRequiredService<ILogger<Repository<MasterLabel>>>();
                            return new Repository<MasterLabel>(factory, logger);
                        });
                        services.AddKeyedScoped<IReadOnlyRepository<MasterLabel>>(key, (sp, _) =>
                        {
                            var factory = sp.GetRequiredKeyedService<IIndTraceDbContextFactory>(key);
                            var cache = sp.GetRequiredService<ICacheService>();
                            var logger = sp.GetRequiredService<ILogger<ReadOnlyRepository<MasterLabel>>>();
                            return new ReadOnlyRepository<MasterLabel>(factory, cache, logger);
                        });
                        services.AddKeyedScoped<IRepository<Plc>>(key, (sp, _) =>
                        {
                            var factory = sp.GetRequiredKeyedService<IIndTraceDbContextFactory>(key);
                            var logger = sp.GetRequiredService<ILogger<Repository<Plc>>>();
                            return new Repository<Plc>(factory, logger);
                        });

                        services.AddKeyedScoped<IRepository<Product>>(key, (sp, _) =>
                        {
                            var factory = sp.GetRequiredKeyedService<IIndTraceDbContextFactory>(key);
                            var logger = sp.GetRequiredService<ILogger<Repository<Product>>>();
                            return new Repository<Product>(factory, logger);
                        });

                        services.AddKeyedScoped<IReadOnlyRepository<Product>>(key, (sp, _) =>
                        {
                            var factory = sp.GetRequiredKeyedService<IIndTraceDbContextFactory>(key);
                            var cache = sp.GetRequiredService<ICacheService>();
                            var logger = sp.GetRequiredService<ILogger<ReadOnlyRepository<Product>>>();
                            return new ReadOnlyRepository<Product>(factory, cache, logger);
                        });

                        services.AddKeyedScoped<IRepository<Recipe>>(key, (sp, _) =>
                        {
                            var factory = sp.GetRequiredKeyedService<IIndTraceDbContextFactory>(key);
                            var logger = sp.GetRequiredService<ILogger<Repository<Recipe>>>();
                            return new Repository<Recipe>(factory, logger);
                        });
                        services.AddKeyedScoped<IReadOnlyRepository<Recipe>>(key, (sp, _) =>
                        {
                            var factory = sp.GetRequiredKeyedService<IIndTraceDbContextFactory>(key);
                            var cache = sp.GetRequiredService<ICacheService>();
                            var logger = sp.GetRequiredService<ILogger<ReadOnlyRepository<Recipe>>>();
                            return new ReadOnlyRepository<Recipe>(factory, cache, logger);
                        });
                        services.AddKeyedScoped<IRepository<Register>>(key, (sp, _) =>
                        {
                            var factory = sp.GetRequiredKeyedService<IIndTraceDbContextFactory>(key);
                            var logger = sp.GetRequiredService<ILogger<Repository<Register>>>();
                            return new Repository<Register>(factory, logger);
                        });

                        services.AddKeyedScoped<IRepository<Rule>>(key, (sp, _) =>
                        {
                            var factory = sp.GetRequiredKeyedService<IIndTraceDbContextFactory>(key);
                            var logger = sp.GetRequiredService<ILogger<Repository<Rule>>>();
                            return new Repository<Rule>(factory, logger);
                        });

                        services.AddKeyedScoped<IRepository<Shift>>(key, (sp, _) =>
                        {
                            var factory = sp.GetRequiredKeyedService<IIndTraceDbContextFactory>(key);
                            var logger = sp.GetRequiredService<ILogger<Repository<Shift>>>();
                            return new Repository<Shift>(factory, logger);
                        });

                        services.AddKeyedScoped<IRepository<Variable>>(key, (sp, _) =>
                        {
                            var factory = sp.GetRequiredKeyedService<IIndTraceDbContextFactory>(key);
                            var logger = sp.GetRequiredService<ILogger<Repository<Variable>>>();
                            return new Repository<Variable>(factory, logger);
                        });
                        services.AddKeyedScoped<IReadOnlyRepository<Variable>>(key, (sp, _) =>
                        {
                            var factory = sp.GetRequiredKeyedService<IIndTraceDbContextFactory>(key);
                            var cache = sp.GetRequiredService<ICacheService>();
                            var logger = sp.GetRequiredService<ILogger<ReadOnlyRepository<Variable>>>();
                            return new ReadOnlyRepository<Variable>(factory, cache, logger);
                        });
                        services.AddKeyedScoped<IRepository<TaskGatewayRequest>>(key, (sp, _) =>
                        {
                            var factory = sp.GetRequiredKeyedService<IIndTraceDbContextFactory>(key);
                            var logger = sp.GetRequiredService<ILogger<Repository<TaskGatewayRequest>>>();
                            return new Repository<TaskGatewayRequest>(factory, logger);
                        });

                        services.AddKeyedScoped<IRepository<TaskGatewayResponse>>(key, (sp, _) =>
                        {
                            var factory = sp.GetRequiredKeyedService<IIndTraceDbContextFactory>(key);
                            var logger = sp.GetRequiredService<ILogger<Repository<TaskGatewayResponse>>>();
                            return new Repository<TaskGatewayResponse>(factory, logger);
                        });

                        services.AddKeyedScoped<IRepository<VariablesGroup>>(key, (sp, _) =>
                        {
                            var factory = sp.GetRequiredKeyedService<IIndTraceDbContextFactory>(key);
                            var logger = sp.GetRequiredService<ILogger<Repository<VariablesGroup>>>();
                            return new Repository<VariablesGroup>(factory, logger);
                        });

                        services.AddKeyedScoped<IRepository<WorkFlow>>(key, (sp, _) =>
                        {
                            var factory = sp.GetRequiredKeyedService<IIndTraceDbContextFactory>(key);
                            var logger = sp.GetRequiredService<ILogger<Repository<WorkFlow>>>();
                            return new Repository<WorkFlow>(factory, logger);
                        });
                        services.AddKeyedScoped<IReadOnlyRepository<WorkFlow>>(key, (sp, _) =>
                        {
                            var factory = sp.GetRequiredKeyedService<IIndTraceDbContextFactory>(key);
                            var cache = sp.GetRequiredService<ICacheService>();
                            var logger = sp.GetRequiredService<ILogger<ReadOnlyRepository<WorkFlow>>>();
                            return new ReadOnlyRepository<WorkFlow>(factory, cache, logger);
                        });
                        // Previously missing entities now registered as keyed services
                        services.AddKeyedScoped<IRepository<ConfigStation>>(key, (sp, _) =>
                        {
                            var factory = sp.GetRequiredKeyedService<IIndTraceDbContextFactory>(key);
                            var logger = sp.GetRequiredService<ILogger<Repository<ConfigStation>>>();
                            return new Repository<ConfigStation>(factory, logger);
                        });

                        services.AddKeyedScoped<IBarCodeResult, BarCodeResult>(key, (sp, _) =>
                        {
                            var logger = sp.GetRequiredService<ILogger<BarCodeResult>>();
                            var barCodeRepository = sp.GetRequiredKeyedService<IRepository<BarCode>>(key);
                            var cycleRepository = sp.GetRequiredKeyedService<IRepository<Cycle>>(key);
                            var machineRepository = sp.GetRequiredKeyedService<IReadOnlyRepository<Machine>>(key);
                            var recipeRepository = sp.GetRequiredKeyedService<IReadOnlyRepository<Recipe>>(key);
                            var masterLabelRepository =
                                sp.GetRequiredKeyedService<IReadOnlyRepository<MasterLabel>>(key);
                            var shiftRepository = sp.GetRequiredKeyedService<IRepository<Shift>>(key);
                            var workFlowRepository = sp.GetRequiredKeyedService<IReadOnlyRepository<WorkFlow>>(key);
                            var variablesRepository = sp.GetRequiredKeyedService<IReadOnlyRepository<Variable>>(key);
                            var productRepository = sp.GetRequiredKeyedService<IReadOnlyRepository<Product>>(key);
                            var dateTimeMachine = sp.GetRequiredService<IDateTimeMachine>();
                            var validationService = sp.GetRequiredService<IBarCodeValidationService>();

                            return new BarCodeResult(logger, barCodeRepository, cycleRepository,
                                machineRepository, recipeRepository, masterLabelRepository,
                                shiftRepository, workFlowRepository, variablesRepository,
                                productRepository, dateTimeMachine, validationService);
                        });
                    }
                });

                web.Configure(app => { /* No endpoints required for DI-only tests */ });
            })

            .Build();

        await _host.StartAsync().ConfigureAwait(false);
    }

    public async ValueTask DisposeAsync()
    {
        await _host.StopAsync().ConfigureAwait(false);
        _host.Dispose();
    }
}

internal sealed class TestContextLoggerProvider : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName) => new TestContextLogger(categoryName);

    public void Dispose()
    { }
}

internal sealed class TestContextLogger : ILogger
{
    private readonly string _category;

    public TestContextLogger(string category) => _category = category;

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => NullScope.Instance;

    public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        var msg = formatter?.Invoke(state, exception) ?? state?.ToString() ?? string.Empty;
        var line = $"[{logLevel}] {_category}: {msg}" + (exception is null ? string.Empty : $"{Environment.NewLine}{exception}");
        if (TestContext.Current is not null)
        {
            TestContext.Current.SendDiagnosticMessage(line);
        }
        else
        {
            Console.WriteLine(line);
        }
    }

    private sealed class NullScope : IDisposable
    {
        public static readonly NullScope Instance = new();

        public void Dispose()
        { }
    }
}
