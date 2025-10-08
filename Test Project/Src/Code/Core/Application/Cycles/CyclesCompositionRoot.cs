// <copyright file="CyclesCompositionRoot.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Cycles;

using IndTrace.Application.Cycles.Services;
using IndTrace.Application.Cycles.Services.Strategies;

/// <summary>
/// Configures dependency injection for cycle-related services.
/// </summary>
public static class CyclesCompositionRoot
{
    /// <summary>
    /// Adds cycle services to the service collection with transparent substitution support.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="useRefactoredHandlers">Whether to use the refactored handlers.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddCycleServices(
        this IServiceCollection services,
        bool useRefactoredHandlers = false)
    {
        // Core application services
        services.AddScoped<IBarCodeInfoProvider, BarCodeInfoProvider>();
        services.AddScoped<IStationValidator, StationValidator>();
        services.AddScoped<IRegisterCleaner, RegisterCleaner>();
        services.AddScoped<IPersistenceOrchestrator, PersistenceOrchestrator>();
        services.AddScoped<ICommandLogger, CommandLogger>();

        // Domain services (singleton for stateless business rules)
        ServiceCollectionServiceExtensions.AddSingleton<ICycleTimeValidator, CycleTimeValidator>(services);
        ServiceCollectionServiceExtensions.AddSingleton<IFlowStatusCalculator, FlowStatusCalculator>(services);

        // Strategy implementations
        services.AddScoped<OkUpdateStrategy>();
        services.AddScoped<NotOkUpdateStrategy>();

        // Strategy factory
        services.AddScoped<ICycleUpdateStrategyFactory, CycleUpdateStrategyFactory>();

        // Handler registration with transparent substitution
        if (useRefactoredHandlers)
        {
            // Use new unified handler for both commands
            services.AddScoped<UpdateCyclesCommandHandler>();

            // Register as both command handlers
            services.AddScoped<IGatewayRequestHandler<UpdateCyclesOkCommand, TaskGatewayResponse>>(
                provider => provider.GetRequiredService<UpdateCyclesCommandHandler>());

            services.AddScoped<IGatewayRequestHandler<UpdateCyclesNotOkCommand, TaskGatewayResponse>>(
                provider => provider.GetRequiredService<UpdateCyclesCommandHandler>());
        }
        // else: Keep existing handler registrations (UpdateCyclesOkCommandHandler, UpdateCyclesNotOkCommandHandler)

        return services;
    }
}
