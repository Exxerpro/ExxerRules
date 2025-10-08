using System.Runtime.CompilerServices;
using IndTrace.Application.BarCodes.Queries.DataLoaders;
using IndTrace.Application.BarCodes.Queries.Mappers;
using IndTrace.Application.BarCodes.Queries.Builders;
using IndTrace.Application.Cycles;
using IndTrace.Application.Cycles.Policies;
using IndTrace.Application.Cycles.Services;
using IndTrace.Application.Cycles.Validation;
using IndTrace.Application.Gateway.Auditing;
using IndTrace.Application.Plcs.Queries.GetDetail.DataLoaders;
using IndTrace.Application.Plcs.Queries.GetDetail.Assemblers;
using IndTrace.Application.Machines.Queries.GetMachinesConfig.DataLoaders;
using IndTrace.Application.Machines.Queries.GetMachinesConfig.Assemblers;
using IndTrace.Persistence.Caching;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Serilog;
using ZiggyCreatures.Caching.Fusion;
using IndTrace.Application.ConfigApplication.Commands.Create;
using IndTrace.Application.Settings.Commands.Create;
using IndTrace.Application.Settings.Queries.GetSettingDetail;
using IndTrace.Application.Settings.Queries.GetSettingsList;
using IndTrace.Application.Variables.Commands.Create;
using IndTrace.Application.Variables.Queries.GetVariableDetail;
using IndTrace.Application.Variables.Queries.GetVariableList;
using IndTrace.Application.WorkFlows.Commands.Create;
using IndTrace.Application.WorkFlows.Queries.GetDetail;
using IndTrace.Application.Products.Queries.GetProductDetail;
using IndTrace.Application.Products.Services;
using IndTrace.Application.Products.Services.Interfaces;
using IndTrace.Application.Plcs.Commands.Create;
using IndTrace.Application.Plcs.Queries.GetDetail;
using IndTrace.Application.Machines.Commands.Create;
using IndTrace.Application.Machines.Commands.Update;
using IndTrace.Application.Machines.Commands.Enable;
using IndTrace.Application.Machines.Queries.GetDetail;
using IndTrace.Application.Machines.Queries.GetMachinesConfig;
using IndTrace.Application.MachinesPlcs.Commands.Create;
using IndTrace.Application.MachinesPlcs.Commands.Update;
using IndTrace.Application.MachinesPlcs.Queries.GetDetail;
using IndTrace.Application.Shifts.Commands.Create;
using IndTrace.Application.Shifts.Commands.Update;
using IndTrace.Application.Shifts.Queries.GetShftDetail;
using IndTrace.Application.Shifts.Queries.GetShiftList;
using IndTrace.Application.Registers.Queries.GetRegisterList;
using IndTrace.Application.Cycles.Queries.GetCyclesList;
using IndTrace.Application.Cycles.Queries.GetCiyclesDetail;
using IndTrace.Application.Notifications.Events.GetEventList;
using IndTrace.Application.ConfigApplication.Commands.Update;
using IndTrace.Application.ConfigApplication.Queries.GetConfigAppsList;
using IndTrace.Application.BarCodes.Queries.GetBarCodeDetailMonitor;
using IndTrace.Application.BarCodes.Queries.GetReportsList.FiltersInfo;
using IndTrace.Application.BarCodes.Queries.GetReportsList.GetList;
using ZiggyCreatures.Caching.Fusion.Serialization.SystemTextJson;
using IndTrace.Persistence.Converters;

/// <summary>
/// Working subset of service registrations that compile successfully
/// Contains only handlers with correct interfaces and constructor parameters
/// </summary>
/// <remarks>
/// This is a simplified version of ServiceRegistration.cs that excludes:
/// - BarCode handlers (complex constructors)
/// - ConfigStation handlers (missing entity)
/// - Cycles command handlers (constructor mismatches)
/// - OEE handlers (interface mismatches)
/// - Performance handlers (interface mismatches)
/// </remarks>
namespace IndTrace.Aggregation.BoundedTests.Services;

public static class ServiceRegistration
{
    /// <summary>
    /// Registers common services, middleware, and configuration for the application.
    /// </summary>
    /// <param name="services">The service collection to add the registrations to.</param>
    /// <returns>The updated <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddCommonServices(this IServiceCollection services)
    {
        services.AddInterceptors();

        services.AddScoped<IDateTimeMachine, DateTimeMachine>();

        services.AddSingleton<ILoggerFactory, LoggerFactory>();
        services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));

        services.AddSingleton<IndTraceConfiguration>();

        // Add ICacheService for repository caching (FusionCache-based)
        services.AddFusionCache()
            .WithSerializer(
                new FusionCacheSystemTextJsonSerializer(
                    new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        WriteIndented = false,
                        Converters =
                        {
                            new EnumModelJsonConverter()
                        }
                    }));
        services.AddSingleton<ICacheService, FusionCacheService>();

        services.AddSingleton(sp =>
            new CacheManager<ApplicationConfiguration>(
                TimeSpan.FromMinutes(60)));
        services.AddTransient<AppDetailsFactory>();
        services
            .AddTransient<IMonitorRequestHandler<GetAppDetailsMonitorRequest, ApplicationConfiguration>,
                GetAppDetailsMonitorRequestHandler>();
        services.AddScoped<IndTraceConfigurationService>();

        services.AddScoped<IRegisterInformationService, RegisterInformationService>();
        services.AddScoped<IDistinctRegisterService, DistinctRegisterService>();

        // Product services - Critical for data integrity in manufacturing traceability
        services.AddScoped<IProductUniquenessValidator, ProductUniquenessValidator>();

        services.AddTransient<GetBarCodeDetailQuery>();
        services.AddTransient<GetBarCodesListQuery>();

        return services;
    }

    public static IServiceCollection AddRepositoriesCollection(this IServiceCollection services)
    {
        services
        .AddSingleton<ICacheService>(sp => new FusionCacheService(
                sp.GetRequiredService<IFusionCache>(),
                sp.GetRequiredService<ILogger<FusionCacheService>>()))

            // Cache partitioning for test isolation
            .AddSingleton<ICachePartitionProvider>(new TestCachePartitionProvider())

            // Repository registrations - MANUALLY BUILT for tests!
            // Core entities
            .AddScoped<IRepository<BarCode>>(sp =>
            {
                var factory = sp.GetRequiredService<IIndTraceDbContextFactory>();
                var logger = sp.GetRequiredService<ILogger<Repository<BarCode>>>();
                return new Repository<BarCode>(factory, logger);
            })
            .AddScoped<IReadOnlyRepository<BarCode>>(sp =>
            {
                var factory = sp.GetRequiredService<IIndTraceDbContextFactory>();
                var cache = sp.GetRequiredService<ICacheService>();
                var logger = sp.GetRequiredService<ILogger<ReadOnlyRepository<BarCode>>>();
                return new ReadOnlyRepository<BarCode>(factory, cache, logger);
            })
            .AddScoped<IRepository<Customer>>(sp =>
            {
                var factory = sp.GetRequiredService<IIndTraceDbContextFactory>();
                var logger = sp.GetRequiredService<ILogger<Repository<Customer>>>();
                return new Repository<Customer>(factory, logger);
            })
            .AddScoped<IReadOnlyRepository<Customer>>(sp =>
            {
                var factory = sp.GetRequiredService<IIndTraceDbContextFactory>();
                var cache = sp.GetRequiredService<ICacheService>();
                var logger = sp.GetRequiredService<ILogger<ReadOnlyRepository<Customer>>>();
                return new ReadOnlyRepository<Customer>(factory, cache, logger);
            })
            .AddScoped<IRepository<Cycle>>(sp =>
            {
                var factory = sp.GetRequiredService<IIndTraceDbContextFactory>();
                var logger = sp.GetRequiredService<ILogger<Repository<Cycle>>>();
                return new Repository<Cycle>(factory, logger);
            })
            .AddScoped<IReadOnlyRepository<Cycle>>(sp =>
            {
                var factory = sp.GetRequiredService<IIndTraceDbContextFactory>();
                var cache = sp.GetRequiredService<ICacheService>();
                var logger = sp.GetRequiredService<ILogger<ReadOnlyRepository<Cycle>>>();
                return new ReadOnlyRepository<Cycle>(factory, cache, logger);
            })
            .AddScoped<IRepository<Machine>>(sp =>
            {
                var factory = sp.GetRequiredService<IIndTraceDbContextFactory>();
                var logger = sp.GetRequiredService<ILogger<Repository<Machine>>>();
                return new Repository<Machine>(factory, logger);
            })
            .AddScoped<IReadOnlyRepository<Machine>>(sp =>
            {
                var factory = sp.GetRequiredService<IIndTraceDbContextFactory>();
                var cache = sp.GetRequiredService<ICacheService>();
                var logger = sp.GetRequiredService<ILogger<ReadOnlyRepository<Machine>>>();
                return new ReadOnlyRepository<Machine>(factory, cache, logger);
            })
            .AddScoped<IRepository<Product>>(sp =>
            {
                var factory = sp.GetRequiredService<IIndTraceDbContextFactory>();
                var logger = sp.GetRequiredService<ILogger<Repository<Product>>>();
                return new Repository<Product>(factory, logger);
            })
            .AddScoped<IReadOnlyRepository<Product>>(sp =>
            {
                var factory = sp.GetRequiredService<IIndTraceDbContextFactory>();
                var cache = sp.GetRequiredService<ICacheService>();
                var logger = sp.GetRequiredService<ILogger<ReadOnlyRepository<Product>>>();
                return new ReadOnlyRepository<Product>(factory, cache, logger);
            })
            .AddScoped<IRepository<Recipe>>(sp =>
            {
                var factory = sp.GetRequiredService<IIndTraceDbContextFactory>();
                var logger = sp.GetRequiredService<ILogger<Repository<Recipe>>>();
                return new Repository<Recipe>(factory, logger);
            })
            .AddScoped<IReadOnlyRepository<Recipe>>(sp =>
            {
                var factory = sp.GetRequiredService<IIndTraceDbContextFactory>();
                var cache = sp.GetRequiredService<ICacheService>();
                var logger = sp.GetRequiredService<ILogger<ReadOnlyRepository<Recipe>>>();
                return new ReadOnlyRepository<Recipe>(factory, cache, logger);
            })
            .AddScoped<IRepository<MasterLabel>>(sp =>
            {
                var factory = sp.GetRequiredService<IIndTraceDbContextFactory>();
                var logger = sp.GetRequiredService<ILogger<Repository<MasterLabel>>>();
                return new Repository<MasterLabel>(factory, logger);
            })
            .AddScoped<IReadOnlyRepository<MasterLabel>>(sp =>
            {
                var factory = sp.GetRequiredService<IIndTraceDbContextFactory>();
                var cache = sp.GetRequiredService<ICacheService>();
                var logger = sp.GetRequiredService<ILogger<ReadOnlyRepository<MasterLabel>>>();
                return new ReadOnlyRepository<MasterLabel>(factory, cache, logger);
            })
            .AddScoped<IRepository<Shift>>(sp =>
            {
                var factory = sp.GetRequiredService<IIndTraceDbContextFactory>();
                var logger = sp.GetRequiredService<ILogger<Repository<Shift>>>();
                return new Repository<Shift>(factory, logger);
            })
            .AddScoped<IReadOnlyRepository<Shift>>(sp =>
            {
                var factory = sp.GetRequiredService<IIndTraceDbContextFactory>();
                var cache = sp.GetRequiredService<ICacheService>();
                var logger = sp.GetRequiredService<ILogger<ReadOnlyRepository<Shift>>>();
                return new ReadOnlyRepository<Shift>(factory, cache, logger);
            })
            .AddScoped<IRepository<WorkFlow>>(sp =>
            {
                var factory = sp.GetRequiredService<IIndTraceDbContextFactory>();
                var logger = sp.GetRequiredService<ILogger<Repository<WorkFlow>>>();
                return new Repository<WorkFlow>(factory, logger);
            })
            .AddScoped<IReadOnlyRepository<WorkFlow>>(sp =>
            {
                var factory = sp.GetRequiredService<IIndTraceDbContextFactory>();
                var cache = sp.GetRequiredService<ICacheService>();
                var logger = sp.GetRequiredService<ILogger<ReadOnlyRepository<WorkFlow>>>();
                return new ReadOnlyRepository<WorkFlow>(factory, cache, logger);
            })
            .AddScoped<IRepository<Variable>>(sp =>
            {
                var factory = sp.GetRequiredService<IIndTraceDbContextFactory>();
                var logger = sp.GetRequiredService<ILogger<Repository<Variable>>>();
                return new Repository<Variable>(factory, logger);
            })
            .AddScoped<IReadOnlyRepository<Variable>>(sp =>
            {
                var factory = sp.GetRequiredService<IIndTraceDbContextFactory>();
                var cache = sp.GetRequiredService<ICacheService>();
                var logger = sp.GetRequiredService<ILogger<ReadOnlyRepository<Variable>>>();
                return new ReadOnlyRepository<Variable>(factory, cache, logger);
            })
            .AddScoped<IRepository<Register>>(sp =>
            {
                var factory = sp.GetRequiredService<IIndTraceDbContextFactory>();
                var logger = sp.GetRequiredService<ILogger<Repository<Register>>>();
                return new Repository<Register>(factory, logger);
            })
            .AddScoped<IReadOnlyRepository<Register>>(sp =>
            {
                var factory = sp.GetRequiredService<IIndTraceDbContextFactory>();
                var cache = sp.GetRequiredService<ICacheService>();
                var logger = sp.GetRequiredService<ILogger<ReadOnlyRepository<Register>>>();
                return new ReadOnlyRepository<Register>(factory, cache, logger);
            })

            // Missing repositories
            .AddScoped<IRepository<Line>>(sp =>
            {
                var factory = sp.GetRequiredService<IIndTraceDbContextFactory>();
                var logger = sp.GetRequiredService<ILogger<Repository<Line>>>();
                return new Repository<Line>(factory, logger);
            })
            .AddScoped<IReadOnlyRepository<Line>>(sp =>
            {
                var factory = sp.GetRequiredService<IIndTraceDbContextFactory>();
                var cache = sp.GetRequiredService<ICacheService>();
                var logger = sp.GetRequiredService<ILogger<ReadOnlyRepository<Line>>>();
                return new ReadOnlyRepository<Line>(factory, cache, logger);
            })
            .AddScoped<IRepository<MachinePlc>>(sp =>
            {
                var factory = sp.GetRequiredService<IIndTraceDbContextFactory>();
                var logger = sp.GetRequiredService<ILogger<Repository<MachinePlc>>>();
                return new Repository<MachinePlc>(factory, logger);
            })
            .AddScoped<IReadOnlyRepository<MachinePlc>>(sp =>
            {
                var factory = sp.GetRequiredService<IIndTraceDbContextFactory>();
                var cache = sp.GetRequiredService<ICacheService>();
                var logger = sp.GetRequiredService<ILogger<ReadOnlyRepository<MachinePlc>>>();
                return new ReadOnlyRepository<MachinePlc>(factory, cache, logger);
            })
            .AddScoped<IRepository<Plc>>(sp =>
            {
                var factory = sp.GetRequiredService<IIndTraceDbContextFactory>();
                var logger = sp.GetRequiredService<ILogger<Repository<Plc>>>();
                return new Repository<Plc>(factory, logger);
            })
            .AddScoped<IReadOnlyRepository<Plc>>(sp =>
            {
                var factory = sp.GetRequiredService<IIndTraceDbContextFactory>();
                var cache = sp.GetRequiredService<ICacheService>();
                var logger = sp.GetRequiredService<ILogger<ReadOnlyRepository<Plc>>>();
                return new ReadOnlyRepository<Plc>(factory, cache, logger);
            })
            .AddScoped<IRepository<Rule>>(sp =>
            {
                var factory = sp.GetRequiredService<IIndTraceDbContextFactory>();
                var logger = sp.GetRequiredService<ILogger<Repository<Rule>>>();
                return new Repository<Rule>(factory, logger);
            })
            .AddScoped<IReadOnlyRepository<Rule>>(sp =>
            {
                var factory = sp.GetRequiredService<IIndTraceDbContextFactory>();
                var cache = sp.GetRequiredService<ICacheService>();
                var logger = sp.GetRequiredService<ILogger<ReadOnlyRepository<Rule>>>();
                return new ReadOnlyRepository<Rule>(factory, cache, logger);
            })
            .AddScoped<IRepository<DistinctRegister>>(sp =>
            {
                var factory = sp.GetRequiredService<IIndTraceDbContextFactory>();
                var logger = sp.GetRequiredService<ILogger<Repository<DistinctRegister>>>();
                return new Repository<DistinctRegister>(factory, logger);
            })
            .AddScoped<IReadOnlyRepository<DistinctRegister>>(sp =>
            {
                var factory = sp.GetRequiredService<IIndTraceDbContextFactory>();
                var cache = sp.GetRequiredService<ICacheService>();
                var logger = sp.GetRequiredService<ILogger<ReadOnlyRepository<DistinctRegister>>>();
                return new ReadOnlyRepository<DistinctRegister>(factory, cache, logger);
            })
            .AddScoped<IRepository<TaskGatewayRequest>>(sp =>
            {
                var factory = sp.GetRequiredService<IIndTraceDbContextFactory>();
                var logger = sp.GetRequiredService<ILogger<Repository<TaskGatewayRequest>>>();
                return new Repository<TaskGatewayRequest>(factory, logger);
            })
            .AddScoped<IReadOnlyRepository<TaskGatewayRequest>>(sp =>
            {
                var factory = sp.GetRequiredService<IIndTraceDbContextFactory>();
                var cache = sp.GetRequiredService<ICacheService>();
                var logger = sp.GetRequiredService<ILogger<ReadOnlyRepository<TaskGatewayRequest>>>();
                return new ReadOnlyRepository<TaskGatewayRequest>(factory, cache, logger);
            })
            .AddScoped<IRepository<TaskGatewayResponse>>(sp =>
            {
                var factory = sp.GetRequiredService<IIndTraceDbContextFactory>();
                var logger = sp.GetRequiredService<ILogger<Repository<TaskGatewayResponse>>>();
                return new Repository<TaskGatewayResponse>(factory, logger);
            })
            .AddScoped<IReadOnlyRepository<TaskGatewayResponse>>(sp =>
            {
                var factory = sp.GetRequiredService<IIndTraceDbContextFactory>();
                var cache = sp.GetRequiredService<ICacheService>();
                var logger = sp.GetRequiredService<ILogger<ReadOnlyRepository<TaskGatewayResponse>>>();
                return new ReadOnlyRepository<TaskGatewayResponse>(factory, cache, logger);
            })
            .AddScoped<IRepository<VariablesGroup>>(sp =>
            {
                var factory = sp.GetRequiredService<IIndTraceDbContextFactory>();
                var logger = sp.GetRequiredService<ILogger<Repository<VariablesGroup>>>();
                return new Repository<VariablesGroup>(factory, logger);
            })
            .AddScoped<IReadOnlyRepository<VariablesGroup>>(sp =>
            {
                var factory = sp.GetRequiredService<IIndTraceDbContextFactory>();
                var cache = sp.GetRequiredService<ICacheService>();
                var logger = sp.GetRequiredService<ILogger<ReadOnlyRepository<VariablesGroup>>>();
                return new ReadOnlyRepository<VariablesGroup>(factory, cache, logger);
            })
            .AddScoped<IRepository<Setting>>(sp =>
            {
                var factory = sp.GetRequiredService<IIndTraceDbContextFactory>();
                var logger = sp.GetRequiredService<ILogger<Repository<Setting>>>();
                return new Repository<Setting>(factory, logger);
            })
            .AddScoped<IReadOnlyRepository<Setting>>(sp =>
            {
                var factory = sp.GetRequiredService<IIndTraceDbContextFactory>();
                var cache = sp.GetRequiredService<ICacheService>();
                var logger = sp.GetRequiredService<ILogger<ReadOnlyRepository<Setting>>>();
                return new ReadOnlyRepository<Setting>(factory, cache, logger);
            })
            .AddScoped<IRepository<Domain.Entities.ConfigApp>>(sp =>
            {
                var factory = sp.GetRequiredService<IIndTraceDbContextFactory>();
                var logger = sp.GetRequiredService<ILogger<Repository<Domain.Entities.ConfigApp>>>();
                return new Repository<Domain.Entities.ConfigApp>(factory, logger);
            })
            .AddScoped<IReadOnlyRepository<Domain.Entities.ConfigApp>>(sp =>
            {
                var factory = sp.GetRequiredService<IIndTraceDbContextFactory>();
                var cache = sp.GetRequiredService<ICacheService>();
                var logger = sp.GetRequiredService<ILogger<ReadOnlyRepository<Domain.Entities.ConfigApp>>>();
                return new ReadOnlyRepository<Domain.Entities.ConfigApp>(factory, cache, logger);
            })
            .AddScoped<IRepository<OeeRegister>>(sp =>
            {
                var factory = sp.GetRequiredService<IIndTraceDbContextFactory>();
                var logger = sp.GetRequiredService<ILogger<Repository<OeeRegister>>>();
                return new Repository<OeeRegister>(factory, logger);
            })
            .AddScoped<IReadOnlyRepository<OeeRegister>>(sp =>
            {
                var factory = sp.GetRequiredService<IIndTraceDbContextFactory>();
                var cache = sp.GetRequiredService<ICacheService>();
                var logger = sp.GetRequiredService<ILogger<ReadOnlyRepository<OeeRegister>>>();
                return new ReadOnlyRepository<OeeRegister>(factory, cache, logger);
            })
            .AddScoped<IRepository<KpiOee>>(sp =>
            {
                var factory = sp.GetRequiredService<IIndTraceDbContextFactory>();
                var logger = sp.GetRequiredService<ILogger<Repository<KpiOee>>>();
                return new Repository<KpiOee>(factory, logger);
            })
            .AddScoped<IReadOnlyRepository<KpiOee>>(sp =>
            {
                var factory = sp.GetRequiredService<IIndTraceDbContextFactory>();
                var cache = sp.GetRequiredService<ICacheService>();
                var logger = sp.GetRequiredService<ILogger<ReadOnlyRepository<KpiOee>>>();
                return new ReadOnlyRepository<KpiOee>(factory, cache, logger);
            })
            .AddScoped<IRepository<PerformanceData>>(sp =>
            {
                var factory = sp.GetRequiredService<IIndTraceDbContextFactory>();
                var logger = sp.GetRequiredService<ILogger<Repository<PerformanceData>>>();
                return new Repository<PerformanceData>(factory, logger);
            })
            .AddScoped<IReadOnlyRepository<PerformanceData>>(sp =>
            {
                var factory = sp.GetRequiredService<IIndTraceDbContextFactory>();
                var cache = sp.GetRequiredService<ICacheService>();
                var logger = sp.GetRequiredService<ILogger<ReadOnlyRepository<PerformanceData>>>();
                return new ReadOnlyRepository<PerformanceData>(factory, cache, logger);
            })
            .AddScoped<IRepository<FlowStatus>>(sp =>
            {
                var factory = sp.GetRequiredService<IIndTraceDbContextFactory>();
                var logger = sp.GetRequiredService<ILogger<Repository<FlowStatus>>>();
                return new Repository<FlowStatus>(factory, logger);
            })
            .AddScoped<IReadOnlyRepository<FlowStatus>>(sp =>
            {
                var factory = sp.GetRequiredService<IIndTraceDbContextFactory>();
                var cache = sp.GetRequiredService<ICacheService>();
                var logger = sp.GetRequiredService<ILogger<ReadOnlyRepository<FlowStatus>>>();
                return new ReadOnlyRepository<FlowStatus>(factory, cache, logger);
            })
            .AddScoped<IRepository<ShiftsCatalog>>(sp =>
            {
                var factory = sp.GetRequiredService<IIndTraceDbContextFactory>();
                var logger = sp.GetRequiredService<ILogger<Repository<ShiftsCatalog>>>();
                return new Repository<ShiftsCatalog>(factory, logger);
            })
            .AddScoped<IReadOnlyRepository<ShiftsCatalog>>(sp =>
            {
                var factory = sp.GetRequiredService<IIndTraceDbContextFactory>();
                var cache = sp.GetRequiredService<ICacheService>();
                var logger = sp.GetRequiredService<ILogger<ReadOnlyRepository<ShiftsCatalog>>>();
                return new ReadOnlyRepository<ShiftsCatalog>(factory, cache, logger);
            });

        return services;
    }

    /// <summary>
    /// Adds the BarCodeResult service to the service collection with scoped lifetime.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddBarCodeResult(this IServiceCollection services)
    {
        services.AddScoped<IBarCodeResult, BarCodeResult>(sp =>
        {
            var logger = sp.GetRequiredService<ILogger<BarCodeResult>>();
            var barCodeRepository = sp.GetRequiredService<IRepository<BarCode>>();
            var cycleRepository = sp.GetRequiredService<IRepository<Cycle>>();
            var machineRepository = sp.GetRequiredService<IReadOnlyRepository<Machine>>();
            var recipeRepository = sp.GetRequiredService<IReadOnlyRepository<Recipe>>();
            var masterLabelRepository = sp.GetRequiredService<IReadOnlyRepository<MasterLabel>>();
            var shiftRepository = sp.GetRequiredService<IRepository<Shift>>();
            var workFlowRepository = sp.GetRequiredService<IReadOnlyRepository<WorkFlow>>();
            var variablesRepository = sp.GetRequiredService<IReadOnlyRepository<Variable>>();
            var productRepository = sp.GetRequiredService<IReadOnlyRepository<Product>>();
            var dateTimeMachine = sp.GetRequiredService<IDateTimeMachine>();
            var validationService = sp.GetRequiredService<IBarCodeValidationService>();

            return new BarCodeResult(logger, barCodeRepository, cycleRepository,
                machineRepository, recipeRepository, masterLabelRepository,
                shiftRepository, workFlowRepository, variablesRepository,
                productRepository, dateTimeMachine, validationService);
        });
        return services;
    }

    /// <summary>
    /// Registers known working command handlers only (to avoid compilation errors)
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddCommandHandlers(this IServiceCollection services)
    {
        services
            // Only register handlers that are known to exist and compile
            .AddScoped<IMonitorRequestHandler<CreateWorkFlowCommand, WorkFlowCreatedEvent>, CreateWorkFlowCommandHandler>()
            .AddScoped<IMonitorRequestHandler<CreateVariableCommand, VariableCreatedEvent>, CreateVariableCommandHandler>()
            .AddScoped<IMonitorRequestHandler<CreateSettingCommand, SettingCreatedEvent>, CreateSettingCommandHandler>();
            // CreateProductCommandHandler registered in AddProductsHandlers with SRP services

        // TODO: Add more handlers gradually as they are verified to exist
        return services;
    }

    /// <summary>
    /// Registers known working query handlers only (to avoid compilation errors)
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddQueryHandlers(this IServiceCollection services)
    {
        services
            // Only register handlers that are known to exist and compile
            .AddScoped<IMonitorRequestHandler<GetAppDetailsMonitorRequest, ApplicationConfiguration>, GetAppDetailsMonitorRequestHandler>();

        // TODO: Add more query handlers gradually as they are verified to exist
        return services;
    }

    /// <summary>
    /// Registers all application services
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services
            // Essential services that we know exist
            .AddScoped<DateTimeMachine>(sp => new DateTimeMachine())
            .AddScoped<IDateTimeMachine>(sp => sp.GetRequiredService<DateTimeMachine>())

            // Validation Services (that we know exist)
            .AddScoped<IBarCodeValidationService>(sp => new BarCodeValidationService())
            .AddScoped<BarCodeValidationService>(sp => new BarCodeValidationService())

            // Core Business Services (that we know exist from current working registrations)
            .AddScoped<MasterLabelService>(sp =>
            {
                var masterLabelRepository = sp.GetRequiredService<IReadOnlyRepository<MasterLabel>>();
                return new MasterLabelService(masterLabelRepository);
            })
            .AddScoped<IShiftService>(sp =>
            {
                var shiftRepository = sp.GetRequiredService<IRepository<Shift>>();
                var cycleRepository = sp.GetRequiredService<IRepository<Cycle>>();
                var shiftDetectionRuleExecutor = sp.GetRequiredService<IShiftDetectionRuleExecutor>();
                var logger = sp.GetRequiredService<ILogger<ShiftService>>();
                var dateTimeMachine = sp.GetRequiredService<IDateTimeMachine>();
                return new ShiftService(shiftRepository, cycleRepository, shiftDetectionRuleExecutor, logger, dateTimeMachine);
            })
            .AddScoped<ShiftService>(sp =>
            {
                var shiftRepository = sp.GetRequiredService<IRepository<Shift>>();
                var cycleRepository = sp.GetRequiredService<IRepository<Cycle>>();
                var shiftDetectionRuleExecutor = sp.GetRequiredService<IShiftDetectionRuleExecutor>();
                var logger = sp.GetRequiredService<ILogger<ShiftService>>();
                var dateTimeMachine = sp.GetRequiredService<IDateTimeMachine>();
                return new ShiftService(shiftRepository, cycleRepository, shiftDetectionRuleExecutor, logger, dateTimeMachine);
            })
            .AddScoped<IShiftDetectionRuleExecutor>(sp => new ShiftDetectionRuleExecutor())
            .AddScoped<ShiftDetectionRuleExecutor>(sp => new ShiftDetectionRuleExecutor())

            // Services with verified constructors
            .AddScoped<IRegisterService>(sp =>
            {
                var registerRepository = sp.GetRequiredService<IRepository<Register>>();
                var variableRepository = sp.GetRequiredService<IRepository<Variable>>();
                return new RegisterService(registerRepository, variableRepository);
            })
            .AddScoped<RegisterService>(sp =>
            {
                var registerRepository = sp.GetRequiredService<IRepository<Register>>();
                var variableRepository = sp.GetRequiredService<IRepository<Variable>>();
                return new RegisterService(registerRepository, variableRepository);
            })
            .AddScoped<ICycleService>(sp =>
            {
                var cycleRepository = sp.GetRequiredService<IRepository<Cycle>>();
                return new CycleService(cycleRepository);
            })
            .AddScoped<CycleService>(sp =>
            {
                var cycleRepository = sp.GetRequiredService<IRepository<Cycle>>();
                return new CycleService(cycleRepository);
            })
            .AddScoped<IProductService>(sp =>
            {
                var monitorRequestDispatcher = sp.GetRequiredService<IMonitorRequestDispatcher>();
                var logger = sp.GetRequiredService<ILogger<ProductService>>();
                return new ProductService(monitorRequestDispatcher, logger);
            })
            .AddScoped<ProductService>(sp =>
            {
                var monitorRequestDispatcher = sp.GetRequiredService<IMonitorRequestDispatcher>();
                var logger = sp.GetRequiredService<ILogger<ProductService>>();
                return new ProductService(monitorRequestDispatcher, logger);
            });

        // TODO: Add more services gradually as they are verified to exist and have correct constructors
        return services;
    }

    /// <summary>
    /// Adds the BarCode services to the service collection with scoped lifetime.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddBarCodeService(this IServiceCollection services)
    {
        services
        .AddScoped<IBarCodeService>(sp =>
            {
                var barCodeRepository = sp.GetRequiredService<IRepository<BarCode>>();
                var registerRepository = sp.GetRequiredService<IRepository<Register>>();
                var cycleRepository = sp.GetRequiredService<IRepository<Cycle>>();
                return new BarCodeService(barCodeRepository, registerRepository, cycleRepository);
            })
            .AddScoped<BarCodeService>(sp =>
            {
                var barCodeRepository = sp.GetRequiredService<IRepository<BarCode>>();
                var registerRepository = sp.GetRequiredService<IRepository<Register>>();
                var cycleRepository = sp.GetRequiredService<IRepository<Cycle>>();
                return new BarCodeService(barCodeRepository, registerRepository, cycleRepository);
            })
            .AddScoped<IMasterLabelService>(sp =>
            {
                var masterLabelRepository = sp.GetRequiredService<IReadOnlyRepository<MasterLabel>>();
                return new MasterLabelService(masterLabelRepository);
            });
        return services;
    }

    /// <summary>
    /// Registers command dispatchers and pipeline behaviors for Monitor command handling.
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
                services.AddSingleton<IGatewayCommandDispatcher, GatewayCommandDispatcher>();
                break;

            case ServiceLifetime.Scoped:
                services.AddScoped<IMonitorRequestDispatcher, MonitorRequestDispatcher>();
                services.AddScoped<IGatewayCommandDispatcher, GatewayCommandDispatcher>();
                break;

            case ServiceLifetime.Transient:
                services.AddTransient<IMonitorRequestDispatcher, MonitorRequestDispatcher>();
                services.AddTransient<IGatewayCommandDispatcher, GatewayCommandDispatcher>();
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(lifetime), lifetime, null!);
        }

        return services;
    }

    /// <summary>
    /// Registers test logging with xUnit
    /// </summary>
    /// <param name="services">The service collection to add the registrations to.</param>
    /// <param name="_testOutputHelper">The xUnit test output helper</param>
    /// <returns>The updated <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddTestLogging(this IServiceCollection services, ITestOutputHelper _testOutputHelper)
    {
        services.AddSingleton<ILoggerProvider>(new XUnitLoggerProvider(_testOutputHelper, appendScope: false));
        return services;
    }

    /// <summary>
    /// Registers mediator pipeline behaviors and notification services.
    /// </summary>
    /// <param name="services">The service collection to add the registrations to.</param>
    /// <returns>The updated <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddInterceptors(this IServiceCollection services)
    {
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(EventLoggerBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        services.AddTransient<INotificationService, IndTraceNotificationService>();

        return services;
    }

    // Add the working handler extension methods that compile successfully

    /// <summary>
    /// Adds ConfigApp command and query handlers to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddConfigAppHandlers(this IServiceCollection services)
    {
        // ConfigApp Command Handlers
        services.AddScoped<IMonitorRequestHandler<CreateConfigAppCommand, ConfigAppCreated>>(sp =>
        {
            var repository = sp.GetRequiredService<IRepository<Domain.Entities.ConfigApp>>();
            var logger = sp.GetRequiredService<ILogger<CreateConfigAppCommandHandler>>();
            return new CreateConfigAppCommandHandler(repository, logger);
        });

        services.AddScoped<IMonitorRequestHandler<UpdateConfigAppCommand, ConfigAppDto>>(sp =>
        {
            var repository = sp.GetRequiredService<IRepository<Domain.Entities.ConfigApp>>();
            var logger = sp.GetRequiredService<ILogger<UpdateConfigAppCommandHandler>>();
            return new UpdateConfigAppCommandHandler(repository, logger);
        });

        // ConfigApp Query Handlers
        services.AddScoped<IMonitorRequestHandler<GetConfigAppsDetailQuery, ConfigAppDto>>(sp =>
        {
            var repository = sp.GetRequiredService<IRepository<Domain.Entities.ConfigApp>>();
            var logger = sp.GetRequiredService<ILogger<GetConfigAppsDetailQueryHandler>>();
            return new GetConfigAppsDetailQueryHandler(repository, logger);
        });

        services.AddScoped<IMonitorRequestHandler<GetConfigAppsListQuery, ConfigAppsListVm>>(sp =>
        {
            var repository = sp.GetRequiredService<IRepository<Domain.Entities.ConfigApp>>();
            var logger = sp.GetRequiredService<ILogger<GetConfigAppsListQueryHandler>>();
            return new GetConfigAppsListQueryHandler(repository, logger);
        });

        return services;
    }

    /// <summary>
    /// Adds Settings command and query handlers to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddSettingsHandlers(this IServiceCollection services)
    {
        // Settings Command Handlers
        services.AddScoped<IMonitorRequestHandler<CreateSettingCommand, SettingCreatedEvent>>(sp =>
        {
            var repository = sp.GetRequiredService<IRepository<Domain.Entities.Setting>>();
            var logger = sp.GetRequiredService<ILogger<CreateSettingCommandHandler>>();
            return new CreateSettingCommandHandler(repository, logger);
        });

        services.AddScoped<IMonitorRequestHandler<UpdateSettingCommand, SettingDetailVm>>(sp =>
        {
            var repository = sp.GetRequiredService<IRepository<Domain.Entities.Setting>>();
            var logger = sp.GetRequiredService<ILogger<UpdateSettingCommandHandler>>();
            return new UpdateSettingCommandHandler(repository, logger);
        });

        // Settings Query Handlers
        services.AddScoped<IMonitorRequestHandler<GetSettingDetailQuery, SettingDetailVm>>(sp =>
        {
            var repository = sp.GetRequiredService<IRepository<Domain.Entities.Setting>>();
            var logger = sp.GetRequiredService<ILogger<GetSettingDetailQueryHandler>>();
            return new GetSettingDetailQueryHandler(repository, logger);
        });

        services.AddScoped<IMonitorRequestHandler<GetSettingsListQuery, SettingsListVm>>(sp =>
        {
            var repository = sp.GetRequiredService<IRepository<Domain.Entities.Setting>>();
            var logger = sp.GetRequiredService<ILogger<GetSettingsListQueryHandler>>();
            return new GetSettingsListQueryHandler(repository, logger);
        });

        return services;
    }

    /// <summary>
    /// Adds Variables command and query handlers to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddVariablesHandlers(this IServiceCollection services)
    {
        // Variables Command Handlers
        services.AddScoped<IMonitorRequestHandler<CreateVariableCommand, VariableCreatedEvent>>(sp =>
        {
            var repository = sp.GetRequiredService<IRepository<Variable>>();
            var logger = sp.GetRequiredService<ILogger<CreateVariableCommandHandler>>();
            return new CreateVariableCommandHandler(repository, logger);
        });

        services.AddScoped<IMonitorRequestHandler<UpdateVariableCommand, VariableDetailVm>>(sp =>
        {
            var repository = sp.GetRequiredService<IRepository<Variable>>();
            var logger = sp.GetRequiredService<ILogger<UpdateVariableCommandHandler>>();
            return new UpdateVariableCommandHandler(repository, logger);
        });

        // Variables Query Handlers
        services.AddScoped<IMonitorRequestHandler<GetVariableDetailQuery, VariableDetailVm>>(sp =>
        {
            var repository = sp.GetRequiredService<IRepository<Variable>>();
            var logger = sp.GetRequiredService<ILogger<GetVariableDetailQueryHandler>>();
            return new GetVariableDetailQueryHandler(repository, logger);
        });

        services.AddScoped<IMonitorRequestHandler<GetVariableListQuery, VariableListVm>>(sp =>
        {
            var repository = sp.GetRequiredService<IRepository<Variable>>();
            var logger = sp.GetRequiredService<ILogger<GetVariableListQueryHandler>>();
            return new GetVariableListQueryHandler(repository, logger);
        });

        return services;
    }

    /// <summary>
    /// Adds WorkFlows command and query handlers to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddWorkFlowsHandlers(this IServiceCollection services)
    {
        // WorkFlows Command Handlers
        services.AddScoped<IMonitorRequestHandler<CreateWorkFlowCommand, WorkFlowCreatedEvent>>(sp =>
        {
            var repository = sp.GetRequiredService<IRepository<WorkFlow>>();
            var logger = sp.GetRequiredService<ILogger<CreateWorkFlowCommandHandler>>();
            return new CreateWorkFlowCommandHandler(repository, logger);
        });

        services.AddScoped<IMonitorRequestHandler<UpdateWorkFlowCommand, WorkFlowDetailVm>>(sp =>
        {
            var repository = sp.GetRequiredService<IRepository<WorkFlow>>();
            var logger = sp.GetRequiredService<ILogger<UpdateWorkFlowCommandHandler>>();
            return new UpdateWorkFlowCommandHandler(repository, logger);
        });

        // WorkFlows Query Handlers
        services.AddScoped<IMonitorRequestHandler<GetWorkFlowDetailQuery, List<WorkFlowDetailVm>>>(sp =>
        {
            var productRepository = sp.GetRequiredService<IRepository<Product>>();
            var workFlowRepository = sp.GetRequiredService<IRepository<WorkFlow>>();
            var logger = sp.GetRequiredService<ILogger<GetWorkFlowDetailQueryHandler>>();
            return new GetWorkFlowDetailQueryHandler(productRepository, workFlowRepository, logger);
        });

        return services;
    }

    /// <summary>
    /// Adds Products command and query handlers to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddProductsHandlers(this IServiceCollection services)
    {
        // Register SRP services (dependency graph order: leaf services first)

        // 1. DOMAIN SERVICES (no external dependencies)
        services.AddScoped<IProductValidator, ProductValidator>();
        services.AddScoped<IProductFactory, ProductFactory>();
        services.AddScoped<IProductEventFactory, ProductEventFactory>();

        // 2. APPLICATION SERVICES (depend on repositories)
        services.AddScoped<IProductUniquenessValidator, ProductUniquenessValidator>();
        services.AddScoped<ICustomerLookupService, CustomerLookupService>();
        services.AddScoped<ILineLookupService, LineLookupService>();
        services.AddScoped<IWorkflowOrchestrator, WorkflowOrchestrator>();

        // 3. ORCHESTRATION SERVICES (depend on repositories and machine repositories)
        services.AddScoped<IRuleOrchestrator, RuleOrchestrator>();

        // 4. RECIPE AND PERSISTENCE ORCHESTRATION (depend on repositories and other services)
        services.AddScoped<IRecipeOrchestrator, RecipeOrchestrator>();
        services.AddScoped<IProductPersistenceOrchestrator, ProductPersistenceOrchestrator>();

        // Products Command Handlers - Updated for SRP services
        services.AddScoped<IMonitorRequestHandler<CreateProductCommand, ProductCreatedEvent>>(sp =>
        {
            // Domain services
            var productValidator = sp.GetRequiredService<IProductValidator>();
            var productFactory = sp.GetRequiredService<IProductFactory>();
            var productEventFactory = sp.GetRequiredService<IProductEventFactory>();

            // Application services
            var uniquenessValidator = sp.GetRequiredService<IProductUniquenessValidator>();
            var customerLookupService = sp.GetRequiredService<ICustomerLookupService>();
            var lineLookupService = sp.GetRequiredService<ILineLookupService>();
            var workflowOrchestrator = sp.GetRequiredService<IWorkflowOrchestrator>();
            var ruleOrchestrator = sp.GetRequiredService<IRuleOrchestrator>();
            var recipeOrchestrator = sp.GetRequiredService<IRecipeOrchestrator>();
            var persistenceOrchestrator = sp.GetRequiredService<IProductPersistenceOrchestrator>();

            // Infrastructure
            var logger = sp.GetRequiredService<ILogger<CreateProductCommandHandler>>();

            return new CreateProductCommandHandler(
                productValidator, productFactory, productEventFactory,
                uniquenessValidator, customerLookupService, lineLookupService,
                workflowOrchestrator, ruleOrchestrator, recipeOrchestrator,
                persistenceOrchestrator, logger);
        });

        services.AddScoped<IMonitorRequestHandler<UpdateProductCommand, ProductDto>>(sp =>
        {
            var repository = sp.GetRequiredService<IRepository<Product>>();
            var monitorRequestDispatcher = sp.GetRequiredService<IMonitorRequestDispatcher>();
            var logger = sp.GetRequiredService<ILogger<UpdateProductCommandHandler>>();
            return new UpdateProductCommandHandler(repository, monitorRequestDispatcher, logger);
        });

        // Products Query Handlers
        services.AddScoped<IMonitorRequestHandler<GetProductDetailQuery, ProductDto>>(sp =>
        {
            var productRepository = sp.GetRequiredService<IRepository<Product>>();
            var customerRepository = sp.GetRequiredService<IRepository<Customer>>();
            var logger = sp.GetRequiredService<ILogger<GetProductDetailQueryHandler>>();
            return new GetProductDetailQueryHandler(productRepository, customerRepository, logger);
        });

        return services;
    }

    /// <summary>
    /// Adds PLCs command and query handlers to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddPlcsHandlers(this IServiceCollection services)
    {
        // PLC SRP Service Registrations
        services.AddScoped<IPlcDetailDataLoader, PlcDetailDataLoader>();
        services.AddScoped<IPlcDetailAssembler, PlcDetailAssembler>();

        // PLCs Command Handlers
        services.AddScoped<IMonitorRequestHandler<CreatePlcCommand, PlcCreated>>(sp =>
        {
            var repository = sp.GetRequiredService<IRepository<Plc>>();
            var logger = sp.GetRequiredService<ILogger<CreatePlcCommandHandler>>();
            return new CreatePlcCommandHandler(repository, logger);
        });

        services.AddScoped<IMonitorRequestHandler<UpdatePlcCommand, PlcDto>>(sp =>
        {
            var repository = sp.GetRequiredService<IRepository<Plc>>();
            var logger = sp.GetRequiredService<ILogger<UpdatePlcCommandHandler>>();
            return new UpdatePlcCommandHandler(repository, logger);
        });

        // PLCs Query Handlers
        services.AddScoped<IMonitorRequestHandler<GetPlcDetailQuery, PlcDto>>(sp =>
        {
            var dataLoader = sp.GetRequiredService<IPlcDetailDataLoader>();
            var assembler = sp.GetRequiredService<IPlcDetailAssembler>();
            var logger = sp.GetRequiredService<ILogger<GetPlcDetailQueryHandler>>();
            return new GetPlcDetailQueryHandler(dataLoader, assembler, logger);
        });

        return services;
    }

    /// <summary>
    /// Adds Machines command and query handlers to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddMachinesHandlers(this IServiceCollection services)
    {
        // Machine SRP Service Registrations
        services.AddScoped<IMachineConfigDataLoader, MachineConfigDataLoader>();
        services.AddScoped<IMachineConfigAssembler, MachineConfigAssembler>();

        // Machines Command Handlers
        services.AddScoped<IMonitorRequestHandler<CreateMachineMonitorRequest, MachineCreated>>(sp =>
        {
            var repository = sp.GetRequiredService<IRepository<Machine>>();
            var logger = sp.GetRequiredService<ILogger<CreateMachineMonitorRequestHandler>>();
            return new CreateMachineMonitorRequestHandler(repository, logger);
        });

        services.AddScoped<IMonitorRequestHandler<MachineUpdateCommand, MachineDto>>(sp =>
        {
            var repository = sp.GetRequiredService<IRepository<Machine>>();
            var logger = sp.GetRequiredService<ILogger<MachineUpdateCommandHandler>>();
            var monitorRequestDispatcher = sp.GetRequiredService<IMonitorRequestDispatcher>();
            return new MachineUpdateCommandHandler(repository, logger, monitorRequestDispatcher);
        });

        services.AddScoped<IMonitorRequestHandler<ToggleEnableMachineCommand, MachineDto>>(sp =>
        {
            var repository = sp.GetRequiredService<IRepository<Machine>>();
            return new TooGleMachineEnableCommandHandler(repository);
        });

        // Machines Query Handlers
        services.AddScoped<IMonitorRequestHandler<GetMachineDetailQuery, MachineDto>>(sp =>
        {
            var repository = sp.GetRequiredService<IRepository<Machine>>();
            var logger = sp.GetRequiredService<ILogger<GetMachineDetailQueryHandler>>();
            return new GetMachineDetailQueryHandler(repository, logger);
        });

        services.AddScoped<IMonitorRequestHandler<GetMachineConfigQuery, MachineConfigVm>>(sp =>
        {
            var dataLoader = sp.GetRequiredService<IMachineConfigDataLoader>();
            var assembler = sp.GetRequiredService<IMachineConfigAssembler>();
            var logger = sp.GetRequiredService<ILogger<GetMachineConfigQueryHandler>>();
            return new GetMachineConfigQueryHandler(dataLoader, assembler, logger);
        });

        return services;
    }

    /// <summary>
    /// Adds MachinesPlcs command and query handlers to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddMachinesPlcsHandlers(this IServiceCollection services)
    {
        // MachinesPlcs Command Handlers
        services.AddScoped<IMonitorRequestHandler<CreateMachinePlcCommand, MachinePlcCreated>>(sp =>
        {
            var repository = sp.GetRequiredService<IRepository<MachinePlc>>();
            var logger = sp.GetRequiredService<ILogger<CreateMachinePlcCommandHandler>>();
            return new CreateMachinePlcCommandHandler(repository, logger);
        });

        services.AddScoped<IMonitorRequestHandler<UpdateMachinePlcCommand, MachinePlcDetailVm>>(sp =>
        {
            var repository = sp.GetRequiredService<IRepository<MachinePlc>>();
            var logger = sp.GetRequiredService<ILogger<UpdateMachinePlcCommandHandler>>();
            return new UpdateMachinePlcCommandHandler(repository, logger);
        });

        // MachinesPlcs Query Handlers
        services.AddScoped<IMonitorRequestHandler<GetMachinePlcDetailQuery, MachinePlcDetailVm>>(sp =>
        {
            var repository = sp.GetRequiredService<IRepository<MachinePlc>>();
            var logger = sp.GetRequiredService<ILogger<GetMachinePlcDetailQueryHandler>>();
            return new GetMachinePlcDetailQueryHandler(repository, logger);
        });

        return services;
    }

    /// <summary>
    /// Adds Shifts command and query handlers to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddShiftsHandlers(this IServiceCollection services)
    {
        // Shifts Command Handlers
        services.AddScoped<IMonitorRequestHandler<CreateShiftCommand, ShiftCreatedEvent>>(sp =>
        {
            var shiftService = sp.GetRequiredService<IShiftService>();
            return new CreateShiftCommandHandler(shiftService);
        });

        services.AddScoped<IMonitorRequestHandler<UpdateShiftCommand, ShiftDetailVm>>(sp =>
        {
            var repository = sp.GetRequiredService<IRepository<Shift>>();
            var logger = sp.GetRequiredService<ILogger<UpdateShiftCommandHandler>>();
            return new UpdateShiftCommandHandler(repository, logger);
        });

        // Shifts Query Handlers
        services.AddScoped<IMonitorRequestHandler<GetShiftDetailQuery, ShiftDetailVm>>(sp =>
        {
            var repository = sp.GetRequiredService<IRepository<Shift>>();
            var logger = sp.GetRequiredService<ILogger<GetShiftDetailQueryHandler>>();
            return new GetShiftDetailQueryHandler(repository, logger);
        });

        services.AddScoped<IMonitorRequestHandler<GetShiftsListQuery, ShiftsListVm>>(sp =>
        {
            var repository = sp.GetRequiredService<IRepository<Shift>>();
            var logger = sp.GetRequiredService<ILogger<GetShiftsListQueryHandler>>();
            return new GetShiftsListQueryHandler(repository, logger);
        });

        return services;
    }

    /// <summary>
    /// Adds Registers query handlers to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddRegistersHandlers(this IServiceCollection services)
    {
        // Registers Query Handlers
        services.AddScoped<IMonitorRequestHandler<GetRegistersListQuery, IEnumerable<RegisterDto>>>(sp =>
        {
            var variableRepository = sp.GetRequiredService<IRepository<Variable>>();
            var registerRepository = sp.GetRequiredService<IRepository<Register>>();
            return new GetRegistersListQueryHandler(variableRepository, registerRepository);
        });

        return services;
    }

    /// <summary>
    /// Adds Cycles query handlers to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddCyclesHandlers(this IServiceCollection services)
    {
        // Cycles Query Handlers (simple constructors only)
        services.AddScoped<IMonitorRequestHandler<GetCyclesListQuery, CyclesListVm>>(sp =>
        {
            var repository = sp.GetRequiredService<IRepository<Cycle>>();
            var logger = sp.GetRequiredService<ILogger<GetCyclesListQueryHandler>>();
            return new GetCyclesListQueryHandler(repository, logger);
        });

        services.AddScoped<IMonitorRequestHandler<GetCyclesDetailQuery, CyclesDetailVm>>(sp =>
        {
            var repository = sp.GetRequiredService<IRepository<Cycle>>();
            var logger = sp.GetRequiredService<ILogger<GetCyclesDetailQueryHandler>>();
            return new GetCyclesDetailQueryHandler(repository, logger);
        });

        return services;
    }

    /// <summary>
    /// Adds Notifications query handlers to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddNotificationsHandlers(this IServiceCollection services)
    {
        // Notifications Query Handlers
        services.AddScoped<IMonitorRequestHandler<GetEventsListQuery, EventsListVm>>(sp =>
        {
            var repositoryRequests = sp.GetRequiredService<IRepository<TaskGatewayRequest>>();
            var repositoryResponses = sp.GetRequiredService<IRepository<TaskGatewayResponse>>();
            return new GetEventsListQueryHandler(repositoryRequests, repositoryResponses);
        });

        return services;
    }

    /// <summary>
    /// Adds BarCode command and query handlers to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddBarCodeHandlers(this IServiceCollection services)
    {
        // BarCode SRP Service Registrations
        services.AddScoped<IReportsFilterInfoBuilder, ReportsFilterInfoBuilder>();

        // BarCode Command Handlers
        services.AddScoped<IGatewayRequestHandler<CreateBarCodeCommand, TaskGatewayResponse>>(sp =>
        {
            var ruleRepository = sp.GetRequiredService<IReadOnlyRepository<Rule>>();
            var cycleRepository = sp.GetRequiredService<IRepository<Cycle>>();
            var machineRepository = sp.GetRequiredService<IReadOnlyRepository<Machine>>();
            var barCodeRepository = sp.GetRequiredService<IRepository<BarCode>>();
            var productRepository = sp.GetRequiredService<IReadOnlyRepository<Product>>();
            var variableRepository = sp.GetRequiredService<IReadOnlyRepository<Variable>>();
            var requestRepository = sp.GetRequiredService<IRepository<TaskGatewayRequest>>();
            var shiftService = sp.GetRequiredService<IShiftService>();
            var dateTimeMachine = sp.GetRequiredService<IDateTimeMachine>();
            var masterLabelService = sp.GetRequiredService<IMasterLabelService>();
            var barCodeService = sp.GetRequiredService<IBarCodeService>();
            var logger = sp.GetRequiredService<ILogger<CreateBarCodeCommandHandler>>();
            return new CreateBarCodeCommandHandler(ruleRepository, cycleRepository, machineRepository,
                barCodeRepository, productRepository, variableRepository, requestRepository, shiftService,
                dateTimeMachine, masterLabelService, barCodeService, logger);
        });

        services.AddScoped<IMonitorRequestHandler<RejectBarCodeCommand, BarCodeRejectedView>>(sp =>
        {
            var barCodeRepository = sp.GetRequiredService<IRepository<BarCode>>();
            var repositoryCommand = sp.GetRequiredService<IRepository<TaskGatewayRequest>>();
            var repositoryCycles = sp.GetRequiredService<IRepository<Cycle>>();
            var dateTimeMachine = sp.GetRequiredService<IDateTimeMachine>();
            return new RejectBarCodeCommandHandler(barCodeRepository, repositoryCommand, repositoryCycles, dateTimeMachine);
        });

        services.AddScoped<IMonitorRequestHandler<RestoreBarCodeCommand, BarCodeRestoredView>>(sp =>
        {
            var barCodeRepository = sp.GetRequiredService<IRepository<BarCode>>();
            var repositoryCommand = sp.GetRequiredService<IRepository<TaskGatewayRequest>>();
            var repositoryCycles = sp.GetRequiredService<IRepository<Cycle>>();
            var dateTimeMachine = sp.GetRequiredService<IDateTimeMachine>();
            return new RestoreBarCodeCommandHandler(barCodeRepository, repositoryCommand, repositoryCycles, dateTimeMachine);
        });

        services.AddScoped<IGatewayRequestHandler<UpdateBarCodeCommand, TaskGatewayResponse>>(sp =>
        {
            var dateTimeMachine = sp.GetRequiredService<IDateTimeMachine>();
            var repositoryCycle = sp.GetRequiredService<IRepository<Cycle>>();
            var barCodeResult = sp.GetRequiredService<IBarCodeResult>();
            return new UpdateBarCodeCommandHandler(dateTimeMachine, repositoryCycle, barCodeResult);
        });

        // BarCode Supporting Services
        services.AddScoped<IBarCodeDetailDataLoader, BarCodeDetailDataLoader>();
        services.AddScoped<IBarCodeDetailMapper, BarCodeDetailMapper>();

        // BarCode Query Handlers
        services.AddScoped<IMonitorRequestHandler<GetBarCodeDetailQuery, BarCodeDetailVm>>(sp =>
        {
            var dataLoader = sp.GetRequiredService<IBarCodeDetailDataLoader>();
            var mapper = sp.GetRequiredService<IBarCodeDetailMapper>();
            var barCodeResult = sp.GetRequiredService<IBarCodeResult>();
            var logger = sp.GetRequiredService<ILogger<GetBarCodeReportQueryHandler>>();
            return new GetBarCodeReportQueryHandler(dataLoader, mapper, barCodeResult, logger);
        });

        services.AddScoped<IMonitorRequestHandler<GetBarCodeDetailQrCodeQuery, BarCodeDetailMonitorVm>>(sp =>
        {
            var barCodeRepository = sp.GetRequiredService<IRepository<BarCode>>();
            var dataLoader = sp.GetRequiredService<IBarCodeDetailDataLoader>();
            var mapper = sp.GetRequiredService<IBarCodeDetailMapper>();
            var logger = sp.GetRequiredService<ILogger<GetBarCodeDetailQueryQrCodeHandler>>();
            return new GetBarCodeDetailQueryQrCodeHandler(barCodeRepository, dataLoader, mapper, logger);
        });

        services.AddScoped<IMonitorRequestHandler<GetBarCodesLabelQuery, IndTrace.Application.BarCodes.Queries.GetBarCodeList.BarCodesListVm>>(sp =>
        {
            var barCodeRepository = sp.GetRequiredService<IRepository<BarCode>>();
            var dispatcher = sp.GetRequiredService<IMonitorRequestDispatcher>();
            var dateTimeMachine = sp.GetRequiredService<IDateTimeMachine>();
            var logger = sp.GetRequiredService<ILogger<GetBarCodesLabelHandler>>();
            return new GetBarCodesLabelHandler(barCodeRepository, dispatcher, dateTimeMachine, logger);
        });

        services.AddScoped<IMonitorRequestHandler<GetBarCodesListQuery, IndTrace.Application.BarCodes.Queries.GetBarCodeList.BarCodesListVm>>(sp =>
        {
            var barCodeRepository = sp.GetRequiredService<IReadOnlyRepository<BarCode>>();
            var masterLabelRepository = sp.GetRequiredService<IReadOnlyRepository<MasterLabel>>();
            var cycleRepository = sp.GetRequiredService<IReadOnlyRepository<Cycle>>();
            return new GetBarCodesListQueryHandler(barCodeRepository, masterLabelRepository, cycleRepository);
        });

        // ✅ NEWLY ADDED: Missing BarCode Query Handlers (Due Diligence Audit Fix)
        services.AddScoped<IMonitorQueryHandler<GetBarCodeDetailMonitorQuery, BarCodeDetailMonitorVm>>(sp =>
        {
            var barCodeRepository = sp.GetRequiredService<IRepository<BarCode>>();
            var dataLoader = sp.GetRequiredService<IBarCodeDetailDataLoader>();
            var mapper = sp.GetRequiredService<IBarCodeDetailMapper>();
            var logger = sp.GetRequiredService<ILogger<GetBarCodeDetailMonitorMonitorQueryHandler>>();
            return new GetBarCodeDetailMonitorMonitorQueryHandler(barCodeRepository, dataLoader, mapper, logger);
        });

        services.AddScoped<IMonitorQueryHandler<GetReportsFilterInfoQuery, ReportsFilterInfoVm>>(sp =>
        {
            var filterInfoBuilder = sp.GetRequiredService<IReportsFilterInfoBuilder>();
            var logger = sp.GetRequiredService<ILogger<GetReportesFilterInfoMonitorQueryHandler>>();
            return new GetReportesFilterInfoMonitorQueryHandler(filterInfoBuilder, logger);
        });

        // TODO: GetReportsListMonitorQueryHandler has interface compatibility issues
        // Handler exists but interface doesn't match IMonitorQueryHandler pattern
        // Requires investigation of proper interface implementation

        return services;
    }

    /// <summary>
    /// Adds ConfigStation command and query handlers to the service collection.
    /// TODO: ConfigStation entity doesn't exist in domain - handlers disabled until entity is created
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddConfigStationHandlers(this IServiceCollection services)
    {
        // TODO: ConfigStation entity doesn't exist - commenting out until domain entity is created
        /*
        // ConfigStation Command Handlers
        services.AddScoped<IMonitorRequestHandler<CreateConfigStationCommand, ConfigStationCreated>>(sp =>
        {
            var repository = sp.GetRequiredService<IRepository<ConfigStation>>();
            var logger = sp.GetRequiredService<ILogger<CreateConfigStationCommandHandler>>();
            return new CreateConfigStationCommandHandler(repository, logger);
        });

        services.AddScoped<IMonitorRequestHandler<UpdateConfigStationCommand, ConfigStationUpdated>>(sp =>
        {
            var repository = sp.GetRequiredService<IRepository<ConfigStation>>();
            var logger = sp.GetRequiredService<ILogger<UpdateConfigStationCommandHandler>>();
            return new UpdateConfigStationCommandHandler(repository, logger);
        });

        // ConfigStation Query Handlers
        services.AddScoped<IMonitorRequestHandler<GetConfigStationDetailQuery, ConfigStationDetailVm>>(sp =>
        {
            var repository = sp.GetRequiredService<IRepository<ConfigStation>>();
            var logger = sp.GetRequiredService<ILogger<GetConfigStationDetailQueryHandler>>();
            return new GetConfigStationDetailQueryHandler(repository, logger);
        });

        services.AddScoped<IMonitorRequestHandler<GetConfigStationListQuery, ApplicationConfiguration>>(sp =>
        {
            var repository = sp.GetRequiredService<IRepository<ConfigStation>>();
            var logger = sp.GetRequiredService<ILogger<GetConfigStationListQueryHandler>>();
            return new GetConfigStationListQueryHandler(repository, logger);
        });
        */

        return services;
    }

    /// <summary>
    /// Adds Cycles command handlers to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddCyclesCommandHandlers(this IServiceCollection services)
    {
        // Cycle SRP Service Registrations
        services.AddScoped<IndTrace.Application.Cycles.Validation.IStationValidator, IndTrace.Application.Cycles.Validation.StationValidator>();
        services.AddScoped<ICycleLimitPolicy, CycleLimitPolicy>();
        services.AddScoped<ICycleCreator, CycleCreator>();
        services.AddScoped<IBarCodeUpdater, BarCodeUpdater>();
        services.AddScoped<IGatewayAuditFactory, GatewayAuditFactory>();

        // Cycles Command Handlers
        services.AddScoped<IGatewayRequestHandler<CreateCyclesCommand, TaskGatewayResponse>>(sp =>
        {
            var logger = sp.GetRequiredService<ILogger<CreateCyclesCommandHandler>>();
            var dateTimeMachine = sp.GetRequiredService<IDateTimeMachine>();
            var barCodeResult = sp.GetRequiredService<IBarCodeResult>();
            var stationValidator = sp.GetRequiredService<IStationValidator>();
            var cycleLimitPolicy = sp.GetRequiredService<ICycleLimitPolicy>();
            var cycleCreator = sp.GetRequiredService<ICycleCreator>();
            var barCodeUpdater = sp.GetRequiredService<IBarCodeUpdater>();
            var gatewayAuditFactory = sp.GetRequiredService<IGatewayAuditFactory>();
            return new CreateCyclesCommandHandler(logger, dateTimeMachine, barCodeResult, stationValidator, cycleLimitPolicy, cycleCreator, barCodeUpdater, gatewayAuditFactory);
        });

        // Add cycle services using the composition root
        services.AddCycleServices(useRefactoredHandlers: true);

        // Cycles Query Handlers
        services.AddScoped<IMonitorRequestHandler<GetCyclesDetailQuery, CyclesDetailVm>>(sp =>
        {
            var repository = sp.GetRequiredService<IRepository<Cycle>>();
            var logger = sp.GetRequiredService<ILogger<GetCyclesDetailQueryHandler>>();
            return new GetCyclesDetailQueryHandler(repository, logger);
        });

        services.AddScoped<IMonitorRequestHandler<GetCyclesListQuery, CyclesListVm>>(sp =>
        {
            var repository = sp.GetRequiredService<IRepository<Cycle>>();
            var logger = sp.GetRequiredService<ILogger<GetCyclesListQueryHandler>>();
            return new GetCyclesListQueryHandler(repository, logger);
        });

        return services;
    }

    /// <summary>
    /// Adds Performance command handlers to the service collection.
    /// TODO: PerformanceDataCommand type not accessible from test project - handlers disabled
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddPerformanceHandlers(this IServiceCollection services)
    {
        // TODO: PerformanceDataCommand type not accessible - commenting out
        /*
        // Performance Command Handlers
        services.AddScoped<IGatewayRequestHandler<PerformanceDataCommand, TaskGatewayResponse>>(sp =>
        {
            var logger = sp.GetRequiredService<ILogger<CreatePerformanceDataCommandHandler>>();
            var oeeRegisterRepo = sp.GetRequiredService<IRepository<OeeRegister>>();
            var kpiOeeRepo = sp.GetRequiredService<IRepository<KpiOee>>();
            return new CreatePerformanceDataCommandHandler(logger, oeeRegisterRepo, kpiOeeRepo);
        });
        */

        return services;
    }

    /// <summary>
    /// Adds OEE command handlers to the service collection.
    /// TODO: CalculateOeeCommand and OeeMetrics types not accessible from test project - handlers disabled
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddOeeHandlers(this IServiceCollection services)
    {
        // TODO: CalculateOeeCommand and OeeMetrics types not accessible - commenting out
        /*
        // OEE Command Handlers
        services.AddScoped<ICommandHandler<CalculateOeeCommand, OeeMetrics>>(sp =>
        {
            return new CalculateOeeCommandHandler();
        });
        */

        return services;
    }

    /// <summary>
    /// Adds missing Product handlers to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddProductsHandlersComplete(this IServiceCollection services)
    {
        // Product Command Handlers (UpdateProductCommandHandler is missing)
        services.AddScoped<IMonitorRequestHandler<UpdateProductCommand, ProductDto>>(sp =>
        {
            var repository = sp.GetRequiredService<IRepository<Product>>();
            var monitorRequestDispatcher = sp.GetRequiredService<IMonitorRequestDispatcher>();
            var logger = sp.GetRequiredService<ILogger<UpdateProductCommandHandler>>();
            return new UpdateProductCommandHandler(repository, monitorRequestDispatcher, logger);
        });

        // Product Query Handlers
        services.AddScoped<IMonitorRequestHandler<GetProductDetailQuery, ProductDto>>(sp =>
        {
            var productRepository = sp.GetRequiredService<IRepository<Product>>();
            var customerRepository = sp.GetRequiredService<IRepository<Customer>>();
            var logger = sp.GetRequiredService<ILogger<GetProductDetailQueryHandler>>();
            return new GetProductDetailQueryHandler(productRepository, customerRepository, logger);
        });

        return services;
    }

    /// <summary>
    /// Adds missing Shift handlers to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddShiftsHandlersComplete(this IServiceCollection services)
    {
        // Shift Command Handlers (UpdateShiftCommandHandler is missing)
        services.AddScoped<IMonitorRequestHandler<UpdateShiftCommand, ShiftDetailVm>>(sp =>
        {
            var repository = sp.GetRequiredService<IRepository<Shift>>();
            var logger = sp.GetRequiredService<ILogger<UpdateShiftCommandHandler>>();
            return new UpdateShiftCommandHandler(repository, logger);
        });

        // Shift Query Handlers
        services.AddScoped<IMonitorRequestHandler<GetShiftDetailQuery, ShiftDetailVm>>(sp =>
        {
            var repository = sp.GetRequiredService<IRepository<Shift>>();
            var logger = sp.GetRequiredService<ILogger<GetShiftDetailQueryHandler>>();
            return new GetShiftDetailQueryHandler(repository, logger);
        });

        services.AddScoped<IMonitorRequestHandler<GetShiftsListQuery, ShiftsListVm>>(sp =>
        {
            var repository = sp.GetRequiredService<IRepository<Shift>>();
            var logger = sp.GetRequiredService<ILogger<GetShiftsListQueryHandler>>();
            return new GetShiftsListQueryHandler(repository, logger);
        });

        return services;
    }

    /// <summary>
    /// Adds Register query handlers to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddRegistersHandlersComplete(this IServiceCollection services)
    {
        // Register Query Handlers
        services.AddScoped<IMonitorRequestHandler<GetRegistersListQuery, IEnumerable<RegisterDto>>>(sp =>
        {
            var variableRepository = sp.GetRequiredService<IRepository<Variable>>();
            var registerRepository = sp.GetRequiredService<IRepository<Register>>();
            return new GetRegistersListQueryHandler(variableRepository, registerRepository);
        });

        return services;
    }
}

/*

  Method 'HandleAsync' not found on behavior type
IndTrace.Application.Models.Interfaces.IPipelineBehavior`2
[[IndTrace.Application.Settings.Commands.Create.CreateSettingCommand,
IndTrace.Application, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null],

[IndQuestResults.Result`1[[IndTrace.Application.Settings.Commands.Create.SettingCreatedEvent, IndTrace.Application, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]],
IndTrace.Domain, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]
 */
