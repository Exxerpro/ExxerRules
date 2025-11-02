using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Caching.Memory;
using ModelContextProtocol;
using IndFusion.Mcp.Core.SyntaxWalkers;

namespace IndFusion.Mcp.Core.Tools;

/// <summary>
/// Computes and caches file/class/method metrics for C# sources, with optional persistence to disk.
/// </summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1052:Static holder types should be Static or NotInheritable", Justification = "Utility class")]
public static class MetricsProvider
{
    private static readonly MemoryCache _cache = new(new MemoryCacheOptions());
    private static bool _disposed = false;

    private static string GetMetricsFilePath(string solutionPath, string filePath)
    {
        var solutionDir = Path.GetDirectoryName(solutionPath)!;
        var relative = Path.GetRelativePath(solutionDir, filePath);
        var metricsPath = Path.Combine(solutionDir, ".ExxerFactor-Mcp", "metrics", relative);
        return Path.ChangeExtension(metricsPath, ".json");
    }

    /// <summary>
    /// Computes or retrieves cached metrics for a given file in a solution.
    /// </summary>
    /// <param name="solutionPath">Absolute path to the solution (.sln).</param>
    /// <param name="filePath">Absolute path to the C# file.</param>
    /// <param name="cancellationToken">Cancellation token to observe during the async operation.</param>
    /// <returns>JSON string containing metrics.</returns>
    public static async Task<string> GetFileMetrics(
        string solutionPath,
        string filePath,
        CancellationToken cancellationToken = default)
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(MetricsProvider), "Cannot access disposed MetricsProvider");
        }

        try
        {
            Console.WriteLine($"GetFileMetrics called with solutionPath: {solutionPath}, filePath: {filePath}");
            
            var key = $"{solutionPath}|{filePath}";

            if (_cache.TryGetValue(key, out string? cached))
            {
                Console.WriteLine($"Found in memory cache for key: {key}");
                return cached!;
            }

            var metricsFile = GetMetricsFilePath(solutionPath, filePath);
            Console.WriteLine($"Metrics file path: {metricsFile}");
            if (File.Exists(metricsFile))
            {
                var fromDisk = await File.ReadAllTextAsync(metricsFile, cancellationToken).ConfigureAwait(false);
                Console.WriteLine($"Found cached metrics file, content length: {fromDisk.Length}");
                
                // Validate JSON before returning from disk cache
                try
                {
                    JsonDocument.Parse(fromDisk);
                    Console.WriteLine($"Setting memory cache from disk for key: {key}");
                    _cache.Set(key, fromDisk);
                    return fromDisk;
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"Cached file contains invalid JSON: {ex.Message}, recomputing...");
                    // Fall through to recompute metrics
                }
            }

            Console.WriteLine($"File does not exist, computing metrics...");
            var (tree, model) = await LoadTreeAndModel(solutionPath, filePath, cancellationToken);
            var root = await tree.GetRootAsync(cancellationToken);
            var metrics = MetricsCalculator.Calculate(root, model);
            var json = JsonSerializer.Serialize(metrics, new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            
            Console.WriteLine($"Generated JSON length: {json.Length}");
            
            // Validate JSON before caching
            try
            {
                JsonDocument.Parse(json);
                Console.WriteLine($"Setting memory cache for key: {key}");
                _cache.Set(key, json);
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"JSON validation failed: {ex.Message}");
                throw new McpException($"Invalid JSON generated for metrics: {ex.Message}", ex);
            }

            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(metricsFile)!);
                await SaveMetricsWithRetryAsync(metricsFile, json, cancellationToken);
                Console.WriteLine($"Saved metrics to disk: {metricsFile}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to save metrics to disk: {ex.Message}");
                // ignore disk cache errors
            }
            return json;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetFileMetrics: {ex.Message}");
            throw new McpException($"Error analyzing metrics: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Recomputes metrics for the given file and updates caches and on-disk copy.
    /// </summary>
    /// <param name="solutionPath">Absolute path to the solution (.sln).</param>
    /// <param name="filePath">Absolute path to the C# file.</param>
    /// <param name="cancellationToken">Cancellation token to observe during the async operation.</param>
    public static Task RefreshFileMetrics(string solutionPath, string filePath, CancellationToken cancellationToken = default)
    {
        // recompute metrics and update cache/disk
        return GetFileMetrics(solutionPath, filePath, cancellationToken);
    }

    private static async Task<(SyntaxTree tree, SemanticModel? model)> LoadTreeAndModel(string solutionPath, string filePath, CancellationToken cancellationToken = default)
    {
        var solution = await ExxerFactoringHelpers.GetOrLoadSolution(solutionPath, cancellationToken);
        var doc = ExxerFactoringHelpers.GetDocumentByPath(solution, filePath);
        Console.WriteLine($"LoadTreeAndModel: Looking for {filePath}");
        Console.WriteLine($"LoadTreeAndModel: Found document: {doc != null}");
        if (doc != null)
        {
            var tree = await doc.GetSyntaxTreeAsync(cancellationToken) ?? CSharpSyntaxTree.ParseText(await File.ReadAllTextAsync(filePath, cancellationToken));
            var model = await doc.GetSemanticModelAsync(cancellationToken);
            return (tree, model);
        }
        Console.WriteLine($"LoadTreeAndModel: Document not found, falling back to file reading");
        var syntaxTree = CSharpSyntaxTree.ParseText(await File.ReadAllTextAsync(filePath, cancellationToken));
        return (syntaxTree, null);
    }

    private static class MetricsCalculator
    {
        public static FileMetrics Calculate(SyntaxNode root, SemanticModel? model)
        {
            var span = root.SyntaxTree.GetLineSpan(root.FullSpan);
            var fileLoc = span.EndLinePosition.Line - span.StartLinePosition.Line + 1;
            var fileMetrics = new FileMetrics { LinesOfCode = fileLoc };

            var classNodes = root.DescendantNodes().OfType<ClassDeclarationSyntax>().ToList();
            fileMetrics.NumberOfClasses = classNodes.Count;
            foreach (var cls in classNodes)
            {
                var clsSpan = cls.GetLocation().GetLineSpan();
                var clsLoc = clsSpan.EndLinePosition.Line - clsSpan.StartLinePosition.Line + 1;
                var clsMetrics = new ClassMetrics { Name = cls.Identifier.Text, LinesOfCode = clsLoc };
                foreach (var method in cls.Members.OfType<MethodDeclarationSyntax>())
                {
                    var mSpan = method.GetLocation().GetLineSpan();
                    var mLoc = mSpan.EndLinePosition.Line - mSpan.StartLinePosition.Line + 1;
                    var walker = new ComplexityWalker();
                    walker.Visit(method);
                    clsMetrics.Methods.Add(new MethodMetrics
                    {
                        Name = method.Identifier.Text,
                        LinesOfCode = mLoc,
                        ParameterCount = method.ParameterList.Parameters.Count,
                        CyclomaticComplexity = walker.Complexity,
                        MaxNestingDepth = walker.MaxDepth
                    });
                    if (method.Modifiers.Any(SyntaxKind.PublicKeyword))
                        fileMetrics.NumberOfPublicMethods++;
                    else if (method.Modifiers.Any(SyntaxKind.PrivateKeyword) ||
                             (!method.Modifiers.Any(SyntaxKind.ProtectedKeyword) && !method.Modifiers.Any(SyntaxKind.InternalKeyword)))
                        fileMetrics.NumberOfPrivateMethods++;
                }
                fileMetrics.Classes.Add(clsMetrics);
            }
            return fileMetrics;
        }
    }


    private class FileMetrics
    {
        public int LinesOfCode { get; set; }
        public int NumberOfClasses { get; set; }
        public int NumberOfPublicMethods { get; set; }
        public int NumberOfPrivateMethods { get; set; }
        public List<ClassMetrics> Classes { get; } = new();
    }

    private class ClassMetrics
    {
        public string Name { get; set; } = string.Empty;
        public int LinesOfCode { get; set; }
        public List<MethodMetrics> Methods { get; } = new();
    }

    private class MethodMetrics
    {
        public string Name { get; set; } = string.Empty;
        public int LinesOfCode { get; set; }
        public int ParameterCount { get; set; }
        public int CyclomaticComplexity { get; set; }
        public int MaxNestingDepth { get; set; }
    }

    /// <summary>
    /// Saves metrics to disk with retry logic to handle file locking issues.
    /// </summary>
    /// <param name="filePath">The path to save the metrics file.</param>
    /// <param name="content">The content to write to the file.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    private static async Task SaveMetricsWithRetryAsync(string filePath, string content, CancellationToken cancellationToken)
    {
        const int maxRetries = 3; // Reduced retries to fail faster
        const int baseDelayMs = 100; // Increased base delay

        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                // Ensure directory exists before writing
                var directory = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // Use atomic write pattern: write to temp file first, then move
                var tempFilePath = filePath + ".tmp";
                
                // Write to temporary file first
                using (var fileStream = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
                using (var writer = new StreamWriter(fileStream))
                {
                    await writer.WriteAsync(content);
                    await writer.FlushAsync();
                }

                // Atomic move to final location
                File.Move(tempFilePath, filePath, overwrite: true);
                return; // Success, exit retry loop
            }
            catch (IOException ex) when (attempt < maxRetries)
            {
                var delay = TimeSpan.FromMilliseconds(baseDelayMs * attempt);
                Console.WriteLine($"File access failed on attempt {attempt}/{maxRetries} for {filePath}: {ex.Message}. Retrying in {delay.TotalMilliseconds}ms...");
                await Task.Delay(delay, cancellationToken);
            }
            catch (UnauthorizedAccessException ex) when (attempt < maxRetries)
            {
                var delay = TimeSpan.FromMilliseconds(baseDelayMs * attempt);
                Console.WriteLine($"Access denied on attempt {attempt}/{maxRetries} for {filePath}: {ex.Message}. Retrying in {delay.TotalMilliseconds}ms...");
                await Task.Delay(delay, cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error saving metrics to {filePath} on attempt {attempt}: {ex.Message}");
                throw; // Re-throw unexpected exceptions
            }
        }

        throw new InvalidOperationException($"Failed to save metrics to {filePath} after {maxRetries} attempts");
    }

    /// <summary>
    /// Disposes the static MemoryCache to prevent memory leaks and disposal errors.
    /// </summary>
    public static void Dispose()
    {
        if (!_disposed)
        {
            _cache?.Dispose();
            _disposed = true;
        }
    }
}
