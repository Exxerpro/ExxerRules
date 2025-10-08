using IndTrace.Application.UserService;
using IndTrace.Domain.Interfaces;
using IndTrace.Identity.Data;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using ILogger = Serilog.ILogger;
using IndTrace.Identity.Users;

namespace IndTrace.Dependencies.Injections;

/// <summary>
/// Provides extension methods for registering common services in the IndTrace Monitor application.
/// </summary>
public static class CommonServiceRegistration
{
    /// <summary>
    /// Adds IndTrace Identity services including authentication, authorization, and database context.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <param name="logger">The logger instance.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddIndTraceIdentity(this IServiceCollection services, IConfiguration configuration, ILogger logger)
    {
        services.AddScoped<Identity.Users.IdentityUserAccessor>();
        services.AddScoped<Identity.Users.IdentityRedirectManager>();
        services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

        services.AddAuthentication(options =>
            {
                options.DefaultScheme = IdentityConstants.ApplicationScheme;
                options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            })
            .AddIdentityCookies();

        var connectionString = configuration.GetConnectionString(nameof(IndTraceDbIdentity))
                               ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        services.AddDbContext<IndTraceDbIdentity>(options =>
        {
            options.UseSqlServer(connectionString, actions =>
            {
                actions.MigrationsAssembly(typeof(IndTraceDbIdentity).Assembly.FullName)
                    .EnableRetryOnFailure(maxRetryCount: 4, maxRetryDelay: TimeSpan.FromSeconds(2),
                        errorNumbersToAdd: Array.Empty<int>());
            })
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors()
                .ConfigureWarnings(warnings =>
                {
                    warnings.Default(WarningBehavior.Log).Log(new[]
                    {
                        CoreEventId.SaveChangesCompleted,
                        CoreEventId.FirstWithoutOrderByAndFilterWarning,
                        CoreEventId.RowLimitingOperationWithoutOrderByWarning,
                    });
                });
        });

        services.AddDatabaseDeveloperPageExceptionFilter();
        services
            .AddIdentityCore<IIndTraceApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
            .AddEntityFrameworkStores<IndTraceDbIdentity>()
            .AddSignInManager()
            .AddDefaultTokenProviders();

        services.AddScoped<IIndTraceUserService, IndTraceUserService>();

        services.AddScoped<UserManager<IIndTraceApplicationUser>>(provider =>
        provider.GetRequiredService<UserManager<IIndTraceApplicationUser>>());
        services.AddScoped<UserManager<IIndTraceApplicationUser>, UserManager<IIndTraceApplicationUser>>();
        services.AddScoped<SignInManager<IIndTraceApplicationUser>>();

        services.AddAuthorization(options =>
        {
            options.AddPolicy("RequireAdministratorRole", policy => policy.RequireRole("Administrator"));
        });

        return services;
    }
}
