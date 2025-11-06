using IndFusion.SemanticRag.Application.Services;
using IndFusion.SemanticRag.Domain.Services;
using Microsoft.Extensions.DependencyInjection;

namespace IndFusion.SemanticRag.Tests.Integration;

/// <summary>
/// Integration test fixture for setting up the test environment.
/// </summary>
public class IntegrationTestFixture : IDisposable
{
    /// <summary>
    /// Gets the service provider.
    /// </summary>
    public IServiceProvider ServiceProvider { get; }

    /// <summary>
    /// Initializes a new instance of the IntegrationTestFixture class.
    /// </summary>
    public IntegrationTestFixture()
    {
        var services = new ServiceCollection();

        // Add logging
        services.AddLogging(builder => builder.AddConsole());

        // Add mock knowledge graph port
        var mockKnowledgeGraphPort = Substitute.For<IKnowledgeGraphPort>();
        SetupMockKnowledgeGraphPort(mockKnowledgeGraphPort);
        services.AddSingleton(mockKnowledgeGraphPort);

        // Add services
        services.AddScoped<IGraphQueryService, GraphQueryService>();
        services.AddScoped<IPatternSuggestService, PatternSuggestService>();
        services.AddScoped<IPatternGraphQueryService, PatternGraphQueryService>();

        ServiceProvider = services.BuildServiceProvider();
    }

    private static void SetupMockKnowledgeGraphPort(IKnowledgeGraphPort mockPort)
    {
        // Setup mock responses for common queries
        mockPort.QueryNodesAsync(Arg.Any<string>(), Arg.Any<IReadOnlyDictionary<string, object>>(), Arg.Any<CancellationToken>())
            .Returns(Result<IReadOnlyList<KnowledgeNode>>.Success(
            [
                new(
                    Id: "test-node",
                    Label: "CodeNode",
                    Properties: new Dictionary<string, object> { ["name"] = "TestNode" },
                    CreatedAt: DateTimeOffset.UtcNow,
                    UpdatedAt: DateTimeOffset.UtcNow)
            ]));

        mockPort.QueryRelationshipsAsync(Arg.Any<string>(), Arg.Any<IReadOnlyDictionary<string, object>>(), Arg.Any<CancellationToken>())
            .Returns(Result<IReadOnlyList<KnowledgeRelationship>>.Success([]));

        mockPort.GetNodeCountAsync(Arg.Any<CancellationToken>())
            .Returns(Result<int>.Success(1000));

        mockPort.GetRelationshipCountAsync(Arg.Any<CancellationToken>())
            .Returns(Result<int>.Success(2500));
    }

    /// <summary>
    /// Disposes the integration test fixture.
    /// </summary>
    public void Dispose()
    {
        if (ServiceProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
}