using IndFusion.SemanticRag.Application.Interfaces;
using IndFusion.SemanticRag.Domain.Ports;
using IndFusion.SemanticRag.Infrastructure.Adapters;
using IndFusion.SemanticRag.Infrastructure.Configuration;
using IndFusion.SemanticRag.Infrastructure.Factories;
using IndFusion.SemanticRag.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Neo4j.Driver;
using Qdrant.Client;

namespace IndFusion.SemanticRag.Infrastructure;

/// <summary>
/// Extension methods for configuring Semantic RAG services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Semantic RAG services to the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddSemanticRagServices(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        // Register configuration options
        services.Configure<QdrantOptions>(configuration.GetSection("Qdrant"));
        services.Configure<OllamaOptions>(configuration.GetSection("Ollama"));
        services.Configure<Neo4jOptions>(configuration.GetSection("Neo4j"));
        services.Configure<RedisOptions>(configuration.GetSection("Redis"));

        // Register Qdrant client
        services.AddSingleton<QdrantClient>(provider =>
        {
            var options = provider.GetRequiredService<IOptions<QdrantOptions>>().Value;
            return new QdrantClient(options.Host, options.Port);
        });

        // Register Neo4j driver
        services.AddSingleton<IDriver>(provider =>
        {
            var factory = new Neo4jDriverFactory(provider.GetRequiredService<IOptions<Neo4jOptions>>());
            return factory.CreateDriver();
        });

        // Register core services
        services.AddScoped<IOcrService>(provider =>
        {
            var logger = provider.GetRequiredService<ILogger<TesseractOcrService>>();
            return new TesseractOcrService(logger);
        });
        services.AddScoped<IVectorSearchService, QdrantVectorSearchService>();
        services.AddScoped<IPatternKnowledgeBase, PatternKnowledgeBaseService>();
        services.AddScoped<ICodeAnalysisService, RoslynCodeAnalysisService>();
        services.AddScoped<ISemanticPatternEngine, SemanticPatternEngineService>();
        services.AddScoped<IRagService, RagService>();
        services.AddScoped<IDocumentProcessingPipeline, DocumentProcessingPipeline>();
        services.AddScoped<IDocumentIngestionService, DocumentIngestionService>();

        // Register domain ports (hexagonal architecture)
        services.AddScoped<IVectorSearchPort, QdrantVectorSearchService>();
        services.AddScoped<IKnowledgeGraphServicePort, Neo4jKnowledgeGraphService>();
        services.AddScoped<IKnowledgeGraphPort, Neo4jKnowledgeGraphAdapter>();
        services.AddScoped<IEmbeddingServicePort, OllamaEmbeddingServiceAdapter>();
        services.AddScoped<IDocumentIngestionPort, DocumentIngestionService>();

        // Register port adapters (technology-agnostic ports)
        services.AddScoped<IVectorDatabasePort>(provider =>
        {
            var qdrantClient = provider.GetRequiredService<QdrantClient>();
            var logger = provider.GetRequiredService<ILogger<QdrantVectorDatabaseAdapter>>();
            return new QdrantVectorDatabaseAdapter(qdrantClient, logger);
        });

        services.AddScoped<IGraphDatabasePort>(provider =>
        {
            var driver = provider.GetRequiredService<IDriver>();
            var options = provider.GetRequiredService<IOptions<Neo4jOptions>>();
            var logger = provider.GetRequiredService<ILogger<Neo4jGraphDatabaseAdapter>>();
            return new Neo4jGraphDatabaseAdapter(driver, options, logger);
        });

        // Register HTTP clients
        services.AddHttpClient<OllamaClient>();
        services.AddHttpClient<OllamaEmbeddingServiceAdapter>(client =>
        {
            var options = configuration.GetSection("Ollama").Get<OllamaOptions>() ?? new OllamaOptions();
            client.BaseAddress = new Uri(options.BaseUrl);
            client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
        });

        return services;
    }
}
