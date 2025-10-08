namespace Integration.Tests.Extensions
{
    public static class HandlerTestExtensions
    {
        public static (THandler sut, IRepository<TEntity> repo, ILogger<THandler> logger)
        BuildHandler<THandler, TEntity>(this IServiceProvider services, ITestOutputHelper output, string dbKey, Func<IRepository<TEntity>, ILogger<THandler>, THandler> factory)
        where TEntity : class
        {
            using var scope = services.CreateScope();
            // Log the resolved connection string for the requested key
            DbLogging.LogConnectionString(scope.ServiceProvider, dbKey, output, nameof(BuildHandler));
            var repo = scope.ServiceProvider.GetRequiredKeyedService<IRepository<TEntity>>(dbKey);
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<THandler>>();

            var sut = factory(repo, logger);
            return (sut, repo, logger);
        }

        public static (THandler sut, TRepo repo, ILogger<THandler> logger)
        BuildHandler<THandler, TRepo>(this IServiceProvider services, ITestOutputHelper output, Func<TRepo, ILogger<THandler>, THandler> factory)
        where TRepo : notnull
        {
            using var scope = services.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<TRepo>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<THandler>>();

            var sut = factory(repo, logger);
            return (sut, repo, logger);
        }
        // Old BuildHandler methods that used CustomWebApplicationFactory have been removed.
        // Use the new overloads that accept IServiceProvider instead.
    }
}
