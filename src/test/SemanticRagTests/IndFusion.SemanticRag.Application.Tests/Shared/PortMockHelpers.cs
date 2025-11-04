using IndFusion.SemanticRag.Domain.Ports;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace IndFusion.SemanticRag.Application.Tests.Shared;

/// <summary>
/// Helper methods for creating and configuring port interface mocks in IITDD tests.
/// </summary>
public static class PortMockHelpers
{
    /// <summary>
    /// Creates a mock instance of <see cref="IVectorDatabasePort"/>.
    /// </summary>
    /// <returns>A mock port instance.</returns>
    public static IVectorDatabasePort CreateMockVectorDatabasePort()
    {
        return Substitute.For<IVectorDatabasePort>();
    }

    /// <summary>
    /// Creates a mock instance of <see cref="IGraphDatabasePort"/>.
    /// </summary>
    /// <returns>A mock port instance.</returns>
    public static IGraphDatabasePort CreateMockGraphDatabasePort()
    {
        return Substitute.For<IGraphDatabasePort>();
    }

    /// <summary>
    /// Creates a mock instance of <see cref="IEmbeddingServicePort"/>.
    /// </summary>
    /// <returns>A mock port instance.</returns>
    public static IEmbeddingServicePort CreateMockEmbeddingServicePort()
    {
        return Substitute.For<IEmbeddingServicePort>();
    }

    /// <summary>
    /// Creates a mock instance of <see cref="IVectorSearchPort"/>.
    /// </summary>
    /// <returns>A mock port instance.</returns>
    public static IVectorSearchPort CreateMockVectorSearchPort()
    {
        return Substitute.For<IVectorSearchPort>();
    }

    /// <summary>
    /// Creates a mock instance of <see cref="IKnowledgeGraphServicePort"/>.
    /// </summary>
    /// <returns>A mock port instance.</returns>
    public static IKnowledgeGraphServicePort CreateMockKnowledgeGraphServicePort()
    {
        return Substitute.For<IKnowledgeGraphServicePort>();
    }

    /// <summary>
    /// Creates a mock instance of <see cref="IDocumentIngestionPort"/>.
    /// </summary>
    /// <returns>A mock port instance.</returns>
    public static IDocumentIngestionPort CreateMockDocumentIngestionPort()
    {
        return Substitute.For<IDocumentIngestionPort>();
    }

    /// <summary>
    /// Creates a mock logger instance for a given type.
    /// </summary>
    /// <typeparam name="T">The type to create a logger for.</typeparam>
    /// <returns>A mock logger instance.</returns>
    public static ILogger<T> CreateMockLogger<T>()
    {
        return Substitute.For<ILogger<T>>();
    }
}
