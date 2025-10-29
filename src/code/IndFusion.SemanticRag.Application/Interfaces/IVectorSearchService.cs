using IndFusion.SemanticRag.Domain.Models;
using IndFusion.SemanticRag.Domain.Ports;

namespace IndFusion.SemanticRag.Application.Interfaces;

/// <summary>
/// Service for vector search operations in the Semantic RAG system.
/// This is the application layer interface that coordinates domain services.
/// </summary>
public interface IVectorSearchService : IVectorSearchPort
{
    // This interface inherits from the domain port to maintain clean architecture
    // The implementation will coordinate between domain services
}