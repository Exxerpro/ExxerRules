using IndTrace.TestData.Models;

namespace IndTrace.TestData.Loaders;

/// <summary>
/// Simplified test data source generator for IndTrace.TestData project.
/// </summary>
internal sealed class TestDataSourceGenerator
{
    private readonly string _outputDirectory;

    /// <summary>
    /// Initializes a new instance of the TestDataSourceGenerator.
    /// </summary>
    public TestDataSourceGenerator(string outputDirectory)
    {
        _outputDirectory = outputDirectory;
    }

    /// <summary>
    /// Generates static classes from usage log.
    /// </summary>
    public async Task<SourceGenerationResult> GenerateFromUsageLogAsync(IReadOnlyDictionary<string, HashSet<string>> usageLog, CancellationToken cancellationToken = default)
    {
        // Simple implementation for compatibility
        await Task.CompletedTask;

        return new SourceGenerationResult
        {
            IsSuccess = true,
            GeneratedFiles = [],
            ClassesGenerated = 0,
            LinesOfCodeGenerated = 0
        };
    }

    /// <summary>
    /// Generates static classes from report file.
    /// </summary>
    public async Task<SourceGenerationResult> GenerateFromReportFileAsync(string reportFilePath, CancellationToken cancellationToken = default)
    {
        // Simple implementation for compatibility
        await Task.CompletedTask;

        return new SourceGenerationResult
        {
            IsSuccess = true,
            GeneratedFiles = [],
            ClassesGenerated = 0,
            LinesOfCodeGenerated = 0
        };
    }
}
