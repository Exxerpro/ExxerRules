namespace IndFusion.SemanticRag.Infrastructure.Configuration;

/// <summary>
/// Configuration options for Qdrant vector database.
/// </summary>
public class QdrantOptions
{
    /// <summary>
    /// Qdrant server host.
    /// </summary>
    public string Host { get; set; } = "localhost";

    /// <summary>
    /// Qdrant server port.
    /// </summary>
    public int Port { get; set; } = 6333;

    /// <summary>
    /// API key for authentication (optional).
    /// </summary>
    public string? ApiKey { get; set; }

    /// <summary>
    /// Collection name for storing vectors.
    /// </summary>
    public string CollectionName { get; set; } = "semantic_patterns";

    /// <summary>
    /// Vector dimension size.
    /// </summary>
    public int VectorSize { get; set; } = 1536;
}

/// <summary>
/// Configuration options for Ollama LLM service.
/// </summary>
public class OllamaOptions
{
    /// <summary>
    /// Ollama server base URL.
    /// </summary>
    public string BaseUrl { get; set; } = "http://localhost:11434";

    /// <summary>
    /// Model name to use for embeddings.
    /// </summary>
    public string EmbeddingModel { get; set; } = "nomic-embed-text";

    /// <summary>
    /// Model name to use for text generation.
    /// </summary>
    public string TextModel { get; set; } = "llama3.2";

    /// <summary>
    /// Model name (alias for EmbeddingModel for backward compatibility).
    /// </summary>
    public string Model 
    { 
        get => EmbeddingModel; 
        set => EmbeddingModel = value; 
    }

    /// <summary>
    /// Embedding dimension size.
    /// </summary>
    public int EmbeddingDimension { get; set; } = 768;

    /// <summary>
    /// Maximum text length for processing.
    /// </summary>
    public int MaxTextLength { get; set; } = 8192;

    /// <summary>
    /// Maximum concurrency for requests.
    /// </summary>
    public int MaxConcurrency { get; set; } = 5;

    /// <summary>
    /// Timeout for requests in seconds.
    /// </summary>
    public int TimeoutSeconds { get; set; } = 30;
}

/// <summary>
/// Configuration options for Neo4j graph database.
/// </summary>
public class Neo4jOptions
{
    /// <summary>
    /// Neo4j server URI.
    /// </summary>
    public string Uri { get; set; } = "bolt://localhost:7687";

    /// <summary>
    /// Username for authentication.
    /// </summary>
    public string Username { get; set; } = "neo4j";

    /// <summary>
    /// Password for authentication.
    /// </summary>
    public string Password { get; set; } = "password";

    /// <summary>
    /// Database name.
    /// </summary>
    public string Database { get; set; } = "neo4j";

    /// <summary>
    /// Connection pool size.
    /// </summary>
    public int MaxConnectionPoolSize { get; set; } = 50;
}

/// <summary>
/// Configuration options for Redis cache.
/// </summary>
public class RedisOptions
{
    /// <summary>
    /// Redis connection string.
    /// </summary>
    public string ConnectionString { get; set; } = "localhost:6379";

    /// <summary>
    /// Database number.
    /// </summary>
    public int Database { get; set; } = 0;

    /// <summary>
    /// Default expiration time in minutes.
    /// </summary>
    public int DefaultExpirationMinutes { get; set; } = 60;
}
