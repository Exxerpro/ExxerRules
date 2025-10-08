using IndTrace.Application.BarCodes.Queries.Builders;
using IndTrace.Application.BarCodes.Queries.Composers;
using IndTrace.Application.BarCodes.Queries.DataLoaders;
using IndTrace.Application.BarCodes.Queries.Filters;
using IndTrace.Application.BarCodes.Queries.Mappers;
using IndTrace.Application.Common.Services;
using IndTrace.Application.Machines.Queries.GetMachinesConfig.DataLoaders;
using IndTrace.Application.Machines.Queries.GetMachinesConfig.Assemblers;
using IndTrace.Application.Plcs.Queries.GetDetail.DataLoaders;
using IndTrace.Application.Plcs.Queries.GetDetail.Assemblers;
using IndTrace.Domain.Services.BarCodes;

namespace IndTrace.Application.Common.Extensions;

/// <summary>
/// Provides extension methods for registering application services in the dependency injection container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds CreateBarCode SRP services to the dependency injection container.
    /// These services implement the Single Responsibility Principle for CreateBarCode handler refactoring.
    /// </summary>
    /// <param name="services">Service collection.</param>
    /// <returns>Service collection for chaining.</returns>
    public static IServiceCollection AddCreateBarCodeSrpServices(this IServiceCollection services)
    {
        //[Fix]
        //CLAUDE
        //Date: 21/09/2025
        //Reason: [SRP REFACTOR] - Add CreateBarCode SRP services for handler decomposition following Single Responsibility Principle

        // Domain Services (Pure business logic, zero dependencies)
        services.AddScoped<IBarCodeFactory, BarCodeFactory>();
        services.AddScoped<ICycleFactory, CycleFactory>();

        // Application Services (Orchestration and infrastructure concerns)
        services.AddScoped<IMachineValidator, MachineValidator>();
        services.AddScoped<IProductLookupService, ProductLookupService>();
        services.AddScoped<IRuleExecutionService, RuleExecutionService>();
        services.AddScoped<IBarCodePersistenceOrchestrator, BarCodePersistenceOrchestrator>();
        services.AddScoped<IReferenceVariableService, ReferenceVariableService>();
        services.AddScoped<IBarCodeResponseBuilder, BarCodeResponseBuilder>();

        // Cross-cutting concerns
        services.AddScoped<IAuditLogger, AuditLogger>();

        return services;
    }

    /// <summary>
    /// Registers all SRP-refactored services with industrial safety compliance.
    /// All services use Result&lt;T&gt; pattern, defensive validation, and structured logging.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <returns>The service collection for method chaining.</returns>
    /// <remarks>
    /// This method registers all services identified in the SRP refactoring analysis.
    ///
    /// NOTE: This is the registration template for future SRP services implementation.
    /// Services are commented out as they will be implemented in subsequent refactoring phases.
    ///
    /// All services follow CLAUDE.md industrial safety standards with:
    /// - Zero-exception patterns using Result&lt;T&gt;
    /// - Defensive validation with 3-character minimums
    /// - Cancellation token support throughout
    /// - Structured logging for manufacturing traceability
    /// </remarks>
    public static IServiceCollection AddSrpRefactoredServices(
        this IServiceCollection services)
    {
        //[Fix]
        //CLAUDE
        //Date: 26/09/2025
        //Reason: [SRP REFACTOR] - Registration of SRP services for GetReportsListMonitorQueryHandler and GetReportesFilterInfoMonitorQueryHandler refactoring

        // === STATIC HELPERS ===
        // These are already created and available as static methods:
        // - RegisterMachineIdUpdater (static class - no registration needed)
        // - BarCodeQueries (static class - no registration needed)
        // - BarCodeDtoMapper (static class - no registration needed)
        // - BarCodeLookup (static class - no registration needed)
        // - QueryHelpers (static class - no registration needed)

        // === IMPLEMENTED SRP SERVICES ===
        // The following services are implemented and ready for use:
        // ✅ IReportsListQueryComposer, IBarCodeListMapper, IRegisterDataFilter (GetReportsListMonitorQueryHandler)
        // ✅ IReportsFilterInfoBuilder (GetReportesFilterInfoMonitorQueryHandler)

        // === FUTURE SRP SERVICES (To be implemented in subsequent phases) ===
        // The following services are designed and documented but not yet implemented:

        // Query Composers (Singleton for performance as they are stateless)
        services.AddSingleton<IReportsListQueryComposer, ReportsListQueryComposer>();
        // services.AddSingleton<IPlcDetailQueryComposer, PlcDetailQueryComposer>();
        // services.AddSingleton<IBarCodeDetailQueryComposer, BarCodeDetailQueryComposer>();

        // Assemblers (will be Singleton for performance)
        services.AddSingleton<IMachineConfigAssembler, MachineConfigAssembler>();
        services.AddSingleton<IPlcDetailAssembler, PlcDetailAssembler>();

        // Data Loaders (Scoped for repository dependencies)
        services.AddScoped<IBarCodeDetailDataLoader, BarCodeDetailDataLoader>();
        services.AddScoped<IMachineConfigDataLoader, MachineConfigDataLoader>();
        services.AddScoped<IPlcDetailDataLoader, PlcDetailDataLoader>();

        // Mappers (Scoped for logging dependencies)
        services.AddScoped<IBarCodeDetailMapper, BarCodeDetailMapper>();
        services.AddScoped<IBarCodeListMapper, BarCodeListMapper>();

        // Builders (Scoped for repository dependencies and logging)
        services.AddScoped<IReportsFilterInfoBuilder, ReportsFilterInfoBuilder>();

        // Validators and Policies (Scoped for business logic)
        services.AddScoped<IRegisterDataFilter, RegisterDataFilter>();
        // services.AddScoped<IStationValidator, StationValidator>();
        // services.AddScoped<ICycleLimitPolicy, CycleLimitPolicy>();

        // Domain Services (will be Scoped for complex operations)
        // services.AddScoped<ICycleCreator, CycleCreator>();
        // services.AddScoped<IBarCodeUpdater, BarCodeUpdater>();
        // services.AddScoped<IGatewayAuditFactory, GatewayAuditFactory>();

        return services;
    }

    /// <summary>
    /// Registers handlers with their new SRP-compliant dependencies.
    /// Call this after AddSrpRefactoredServices() to ensure proper dependency resolution.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <returns>The service collection for method chaining.</returns>
    /// <remarks>
    /// This method will register refactored handlers that use the SRP services.
    ///
    /// NOTE: This is the registration template for future handler refactoring.
    /// Handler registrations are commented out as the SRP service implementations
    /// will be created in subsequent refactoring phases.
    ///
    /// Benefits of the planned refactoring:
    /// - Reduced complexity (6-8 dependencies reduced to 2-5)
    /// - Single responsibility per service
    /// - Industrial safety compliance with Result&lt;T&gt; patterns
    /// - Comprehensive error handling and logging
    /// </remarks>
    public static IServiceCollection AddRefactoredHandlers(
        this IServiceCollection services)
    {
        //[Fix]
        //CLAUDE
        //Date: 26/09/2025
        //Reason: [SRP REFACTOR] - Template for refactored handler registration (implementations to follow)

        // === FUTURE HANDLER REGISTRATIONS ===
        // The following handler registrations will be activated as SRP services are implemented:

        // Query Handlers (Monitor Context)
        // services.AddScoped<IMonitorQueryHandler<GetReportsListQuery, BarCodesListVm>>(sp => ...);
        // services.AddScoped<IMonitorRequestHandler<GetMachineConfigQuery, MachineConfigVm>>(sp => ...);
        // services.AddScoped<IMonitorQueryHandler<GetPlcDetailQuery, PlcDetailVm>>(sp => ...);

        // Command Handlers (Gateway Context)
        // services.AddScoped<IGatewayRequestHandler<CreateCyclesCommand, TaskGatewayResponse>>(sp => ...);

        return services;
    }
}
