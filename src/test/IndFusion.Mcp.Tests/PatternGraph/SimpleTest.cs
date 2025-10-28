using IndFusion.Mcp.Core.Abstractions;
using IndFusion.Mcp.Core.Models.PatternGraph;
using IndFusion.Mcp.Core.Services;
using IndQuestResults;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace IndFusion.Mcp.Tests.PatternGraph;

/// <summary>
/// Simple test to verify basic functionality.
/// </summary>
public class SimpleTest
{
    /// <summary>
    /// Test that PatternGraphService can be instantiated.
    /// </summary>
    [Fact]
    public void PatternGraphService_CanBeInstantiated()
    {
        // Arrange
        var logger = Substitute.For<ILogger<PatternGraphService>>();
        var symbolGraphBuilder = Substitute.For<ISymbolGraphBuilder>();
        var cacheManager = Substitute.For<IGraphCacheManager>();
        
        // Act
        var service = new PatternGraphService(logger, symbolGraphBuilder, cacheManager);
        
        // Assert
        Assert.NotNull(service);
    }
    
    /// <summary>
    /// Test that GraphCacheManager can be instantiated.
    /// </summary>
    [Fact]
    public void GraphCacheManager_CanBeInstantiated()
    {
        // Arrange
        var logger = Substitute.For<ILogger<GraphCacheManager>>();
        
        // Act
        var service = new GraphCacheManager(logger);
        
        // Assert
        Assert.NotNull(service);
    }
    
    /// <summary>
    /// Test that PatternSuggestionService can be instantiated.
    /// </summary>
    [Fact]
    public void PatternSuggestionService_CanBeInstantiated()
    {
        // Arrange
        var logger = Substitute.For<ILogger<PatternSuggestionService>>();
        
        // Act
        var service = new PatternSuggestionService(logger);
        
        // Assert
        Assert.NotNull(service);
    }
    
    /// <summary>
    /// Test that SymbolGraphBuilder can be instantiated.
    /// </summary>
    [Fact]
    public void SymbolGraphBuilder_CanBeInstantiated()
    {
        // Arrange
        var logger = Substitute.For<ILogger<SymbolGraphBuilder>>();
        
        // Act
        var service = new SymbolGraphBuilder(logger);
        
        // Assert
        Assert.NotNull(service);
    }
}
