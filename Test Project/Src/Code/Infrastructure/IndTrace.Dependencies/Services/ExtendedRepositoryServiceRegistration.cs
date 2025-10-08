using IndTrace.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace IndTrace.Dependencies.Services;

/// <summary>
/// Registers extended repository services that replace extension method functionality.
/// Enables proper dependency injection and testability for complex repository operations.
/// </summary>
public static class ExtendedRepositoryServiceRegistration
{
    /// <summary>
    /// Adds extended repository services to the dependency injection container.
    /// These services replace the functionality of BarCodeRepositoryExtensions,
    /// CycleRepositoryExtensions, and RegisterRepositoryExtensions.
    /// </summary>
    /// <param name="services">Service collection.</param>
    /// <returns>Service collection for chaining.</returns>
    public static IServiceCollection AddExtendedRepositoryServices(this IServiceCollection services)
    {
        //[Fix]
        //CLAUDE
        //Date: 24/08/2025
        //Reason: [ARCHITECTURAL REFACTOR] - Replace repository extension methods with proper service interfaces for testability and dependency injection

        // BarCode Services - Replacing BarCodeRepositoryExtensions
        services.AddScoped<IBarCodeService, BarCodeService>();

        // Cycle Services - Replacing CycleRepositoryExtensions
        services.AddScoped<ICycleService, CycleService>();

        // Register Services - Replacing RegisterRepositoryExtensions
        services.AddScoped<IRegisterService, RegisterService>();

        //[Fix]
        //CLAUDE
        //Date: 25/08/2025
        //Reason: [ARCHITECTURAL REFACTOR] - Add missing MasterLabelService to replace MasterLabelRepositoryExtensions
        // MasterLabel Services - Replacing MasterLabelRepositoryExtensions
        services.AddScoped<IMasterLabelService, MasterLabelService>();

        return services;
    }


    /// <summary>
    /// Validates that all extended repository services are properly registered.
    /// </summary>
    /// <param name="serviceProvider">Service provider to validate.</param>
    /// <returns>True if all services are registered, false otherwise.</returns>
    public static bool ValidateExtendedRepositoryServices(this IServiceProvider serviceProvider)
    {
        try
        {
            var barCodeService = serviceProvider.GetService<IBarCodeService>();
            var cycleService = serviceProvider.GetService<ICycleService>();
            var registerService = serviceProvider.GetService<IRegisterService>();
            var masterLabelService = serviceProvider.GetService<IMasterLabelService>();

            return barCodeService != null && cycleService != null && registerService != null && masterLabelService != null;
        }
        catch
        {
            return false;
        }
    }
}
