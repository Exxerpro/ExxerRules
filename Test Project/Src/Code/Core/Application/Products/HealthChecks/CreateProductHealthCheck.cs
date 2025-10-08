using IndTrace.Application.Products.Observability;
using IndTrace.Application.Repositories;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace IndTrace.Application.Products.HealthChecks;

/// <summary>
/// Health check for CreateProduct SRP services.
/// Validates repository connectivity and service availability for industrial monitoring.
/// Ensures all dependencies are operational before product creation operations.
/// </summary>
public class CreateProductHealthCheck : IHealthCheck
{
    private readonly IRepository<Product> _productRepository;
    private readonly IRepository<Customer> _customerRepository;
    private readonly IRepository<Line> _lineRepository;
    private readonly IRepository<WorkFlow> _workflowRepository;
    private readonly IRepository<Rule> _ruleRepository;
    private readonly IRepository<Recipe> _recipeRepository;
    private readonly ILogger<CreateProductHealthCheck> _logger;

    public CreateProductHealthCheck(
        IRepository<Product> productRepository,
        IRepository<Customer> customerRepository,
        IRepository<Line> lineRepository,
        IRepository<WorkFlow> workflowRepository,
        IRepository<Rule> ruleRepository,
        IRepository<Recipe> recipeRepository,
        ILogger<CreateProductHealthCheck> logger)
    {
        _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
        _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
        _lineRepository = lineRepository ?? throw new ArgumentNullException(nameof(lineRepository));
        _workflowRepository = workflowRepository ?? throw new ArgumentNullException(nameof(workflowRepository));
        _ruleRepository = ruleRepository ?? throw new ArgumentNullException(nameof(ruleRepository));
        _recipeRepository = recipeRepository ?? throw new ArgumentNullException(nameof(recipeRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        using var activity = CreateProductActivitySource.Source.StartActivity("CreateProduct.HealthCheck");
        var stopwatch = Stopwatch.StartNew();

        try
        {
            _logger.LogDebug(CreateProductLogEvents.HealthCheckStart,
                "Starting CreateProduct health check, Activity: {ActivityId}", activity?.Id);

            // Test repository connectivity with parallel execution for performance
            var healthCheckTasks = new[]
            {
                CheckRepositoryHealthAsync("Product", _productRepository, cancellationToken),
                CheckRepositoryHealthAsync("Customer", _customerRepository, cancellationToken),
                CheckRepositoryHealthAsync("Line", _lineRepository, cancellationToken),
                CheckRepositoryHealthAsync("WorkFlow", _workflowRepository, cancellationToken),
                CheckRepositoryHealthAsync("Rule", _ruleRepository, cancellationToken),
                CheckRepositoryHealthAsync("Recipe", _recipeRepository, cancellationToken)
            };

            var results = await Task.WhenAll(healthCheckTasks);
            stopwatch.Stop();

            // Analyze results
            var healthyRepositories = results.Where(r => r.IsHealthy).ToList();
            var unhealthyRepositories = results.Where(r => !r.IsHealthy).ToList();

            var data = new Dictionary<string, object>
            {
                ["TotalRepositories"] = results.Length,
                ["HealthyRepositories"] = healthyRepositories.Count,
                ["UnhealthyRepositories"] = unhealthyRepositories.Count,
                ["ResponseTimeMs"] = stopwatch.ElapsedMilliseconds,
                ["CheckTimestamp"] = DateTime.UtcNow
            };

            // Add individual repository details
            foreach (var result in results)
            {
                data[$"{result.RepositoryName}Count"] = result.Count;
                data[$"{result.RepositoryName}Healthy"] = result.IsHealthy;
                data[$"{result.RepositoryName}ResponseTimeMs"] = result.ResponseTime.TotalMilliseconds;

                if (!result.IsHealthy && !string.IsNullOrEmpty(result.Error))
                {
                    data[$"{result.RepositoryName}Error"] = result.Error;
                }
            }

            // Set activity context
            CreateProductActivitySource.SetServiceContext(activity, nameof(CreateProductHealthCheck), "CheckHealth");
            activity?.SetTag("repositories.total", results.Length.ToString());
            activity?.SetTag("repositories.healthy", healthyRepositories.Count.ToString());
            activity?.SetTag("repositories.unhealthy", unhealthyRepositories.Count.ToString());
            activity?.SetTag("healthCheck.responseTime", stopwatch.ElapsedMilliseconds.ToString());

            // Determine overall health status
            if (unhealthyRepositories.Count == 0)
            {
                // All repositories healthy
                _logger.LogInformation(CreateProductLogEvents.HealthCheckSuccess,
                    "CreateProduct health check passed - all {RepositoryCount} repositories healthy in {Duration}ms, Activity: {ActivityId}",
                    results.Length, stopwatch.ElapsedMilliseconds, activity?.Id);

                return HealthCheckResult.Healthy("CreateProduct services fully operational", data);
            }
            else if (healthyRepositories.Count > unhealthyRepositories.Count)
            {
                // Majority healthy - degraded
                var failedNames = unhealthyRepositories.Select(r => r.RepositoryName).ToList();

                _logger.LogWarning(CreateProductLogEvents.HealthCheckDegraded,
                    "CreateProduct health check degraded - {FailedCount}/{TotalCount} repositories failed: {FailedRepositories}, Duration: {Duration}ms, Activity: {ActivityId}",
                    unhealthyRepositories.Count, results.Length, string.Join(", ", failedNames),
                    stopwatch.ElapsedMilliseconds, activity?.Id);

                return HealthCheckResult.Degraded(
                    $"Repository issues detected: {string.Join(", ", failedNames)}",
                    null, data);
            }
            else
            {
                // Majority unhealthy - unhealthy
                var failedNames = unhealthyRepositories.Select(r => r.RepositoryName).ToList();

                _logger.LogError(CreateProductLogEvents.HealthCheckUnhealthy,
                    "CreateProduct health check failed - {FailedCount}/{TotalCount} repositories failed: {FailedRepositories}, Duration: {Duration}ms, Activity: {ActivityId}",
                    unhealthyRepositories.Count, results.Length, string.Join(", ", failedNames),
                    stopwatch.ElapsedMilliseconds, activity?.Id);

                return HealthCheckResult.Unhealthy(
                    $"Critical repository failures: {string.Join(", ", failedNames)}",
                    null, data);
            }
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            CreateProductActivitySource.SetErrorContext(activity, new[] { ex.Message }, ex);

            _logger.LogError(CreateProductLogEvents.HealthCheckUnhealthy, ex,
                "CreateProduct health check exception after {Duration}ms, Activity: {ActivityId}",
                stopwatch.ElapsedMilliseconds, activity?.Id);

            var errorData = new Dictionary<string, object>
            {
                ["Error"] = ex.Message,
                ["ExceptionType"] = ex.GetType().Name,
                ["ResponseTimeMs"] = stopwatch.ElapsedMilliseconds,
                ["CheckTimestamp"] = DateTime.UtcNow
            };

            return HealthCheckResult.Unhealthy("CreateProduct services unavailable due to exception", ex, errorData);
        }
    }

    /// <summary>
    /// Checks the health of an individual repository.
    /// </summary>
    private async Task<RepositoryHealthResult> CheckRepositoryHealthAsync<T>(
        string repositoryName,
        IRepository<T> repository,
        CancellationToken cancellationToken) where T : class
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            var spec = new Specification<T>(x => true); // Simple spec to count all entities
            // Simple count operation to test connectivity
            var countResult = await repository.CountAsync(spec, cancellationToken: cancellationToken);
            stopwatch.Stop();

            if (countResult.IsSuccess)
            {
                return new RepositoryHealthResult
                {
                    RepositoryName = repositoryName,
                    IsHealthy = true,
                    Count = countResult.Value,
                    ResponseTime = stopwatch.Elapsed
                };
            }
            else
            {
                var errors = string.Join("; ", countResult.Errors);

                _logger.LogWarning(new EventId(4110, "RepositoryHealthFailure"),
                    "Repository {RepositoryName} health check failed: {Errors}, Duration: {Duration}ms",
                    repositoryName, errors, stopwatch.ElapsedMilliseconds);

                return new RepositoryHealthResult
                {
                    RepositoryName = repositoryName,
                    IsHealthy = false,
                    Count = -1,
                    ResponseTime = stopwatch.Elapsed,
                    Error = errors
                };
            }
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            _logger.LogError(new EventId(4111, "RepositoryHealthException"), ex,
                "Repository {RepositoryName} health check exception: {Message}, Duration: {Duration}ms",
                repositoryName, ex.Message, stopwatch.ElapsedMilliseconds);

            return new RepositoryHealthResult
            {
                RepositoryName = repositoryName,
                IsHealthy = false,
                Count = -1,
                ResponseTime = stopwatch.Elapsed,
                Error = ex.Message
            };
        }
    }

    /// <summary>
    /// Result of checking an individual repository's health.
    /// </summary>
    private class RepositoryHealthResult
    {
        public string RepositoryName { get; set; } = string.Empty;
        public bool IsHealthy { get; set; }
        public int Count { get; set; }
        public TimeSpan ResponseTime { get; set; }
        public string? Error { get; set; }
    }
}
