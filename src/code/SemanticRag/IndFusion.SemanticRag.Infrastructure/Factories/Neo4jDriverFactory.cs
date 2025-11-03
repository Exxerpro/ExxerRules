using System;
using IndFusion.SemanticRag.Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using Neo4j.Driver;

namespace IndFusion.SemanticRag.Infrastructure.Factories;

/// <summary>
/// Factory for creating Neo4j driver instances.
/// </summary>
public class Neo4jDriverFactory
{
    private readonly Neo4jOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="Neo4jDriverFactory"/> class.
    /// </summary>
    /// <param name="options">The Neo4j configuration options.</param>
    public Neo4jDriverFactory(IOptions<Neo4jOptions> options)
    {
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
    }

    /// <summary>
    /// Creates a new Neo4j driver instance.
    /// </summary>
    /// <returns>A configured Neo4j driver.</returns>
    public IDriver CreateDriver()
    {
        return GraphDatabase.Driver(_options.Uri, AuthTokens.Basic(_options.Username, _options.Password));
    }
}