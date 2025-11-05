using IndFusion.SemanticRag.Application.Interfaces;
using IndFusion.SemanticRag.Domain.Models;
using Microsoft.Extensions.Logging;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.IO;

namespace IndFusion.SemanticRag.Infrastructure.Services;

/// <summary>
/// Implementation of code analysis service using Roslyn.
/// </summary>
public class RoslynCodeAnalysisService : ICodeAnalysisService
{
    private readonly ILogger<RoslynCodeAnalysisService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="RoslynCodeAnalysisService"/> class.
    /// </summary>
    /// <param name="logger">Logger instance.</param>
    public RoslynCodeAnalysisService(ILogger<RoslynCodeAnalysisService> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<CodeAnalysisResult> AnalyzeProjectAsync(
        string projectPath, 
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(projectPath))
            throw new ArgumentException("Project path cannot be null or empty", nameof(projectPath));

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        _logger.LogInformation("Analyzing project: {ProjectPath}", projectPath);
        
        cancellationToken.ThrowIfCancellationRequested();
        
        var violations = new List<PatternViolation>();
        var suggestions = new List<PatternSuggestion>();
        var filesAnalyzed = 0;
        var linesOfCode = 0;
        
        try
        {
            // Check if project path exists
            if (!File.Exists(projectPath) && !Directory.Exists(projectPath))
            {
                _logger.LogWarning("Project path does not exist: {ProjectPath}", projectPath);
                stopwatch.Stop();
                return new CodeAnalysisResult
                {
                    Violations = [],
                    Suggestions = [],
                    ComplianceScore = 0.0f,
                    ElapsedMilliseconds = stopwatch.ElapsedMilliseconds,
                    FilesAnalyzed = 0,
                    LinesOfCode = 0
                };
            }
            
            // Load project using MSBuild workspace
            using var workspace = MSBuildWorkspace.Create();
            Project? project;
            
            if (File.Exists(projectPath) && projectPath.EndsWith(".csproj", StringComparison.OrdinalIgnoreCase))
            {
                project = await workspace.OpenProjectAsync(projectPath, progress: null, cancellationToken);
            }
            else if (Directory.Exists(projectPath))
            {
                var csprojFiles = Directory.GetFiles(projectPath, "*.csproj", SearchOption.TopDirectoryOnly);
                if (csprojFiles.Length == 0)
                {
                    _logger.LogWarning("No .csproj file found in directory: {ProjectPath}", projectPath);
                    stopwatch.Stop();
                    return new CodeAnalysisResult
                    {
                        Violations = [],
                        Suggestions = [],
                        ComplianceScore = 0.0f,
                        ElapsedMilliseconds = stopwatch.ElapsedMilliseconds,
                        FilesAnalyzed = 0,
                        LinesOfCode = 0
                    };
                }
                project = await workspace.OpenProjectAsync(csprojFiles[0], progress: null, cancellationToken);
            }
            else
            {
                _logger.LogWarning("Invalid project path: {ProjectPath}", projectPath);
                stopwatch.Stop();
                return new CodeAnalysisResult
                {
                    Violations = [],
                    Suggestions = [],
                    ComplianceScore = 0.0f,
                    ElapsedMilliseconds = stopwatch.ElapsedMilliseconds,
                    FilesAnalyzed = 0,
                    LinesOfCode = 0
                };
            }
            
            if (project == null)
            {
                _logger.LogWarning("Failed to load project: {ProjectPath}", projectPath);
                stopwatch.Stop();
                return new CodeAnalysisResult
                {
                    Violations = [],
                    Suggestions = [],
                    ComplianceScore = 0.0f,
                    ElapsedMilliseconds = stopwatch.ElapsedMilliseconds,
                    FilesAnalyzed = 0,
                    LinesOfCode = 0
                };
            }
            
            // Get compilation to enable semantic analysis
            var compilation = await project.GetCompilationAsync(cancellationToken);
            if (compilation == null)
            {
                _logger.LogWarning("Failed to get compilation for project: {ProjectPath}", projectPath);
                stopwatch.Stop();
                return new CodeAnalysisResult
                {
                    Violations = [],
                    Suggestions = [],
                    ComplianceScore = 0.0f,
                    ElapsedMilliseconds = stopwatch.ElapsedMilliseconds,
                    FilesAnalyzed = 0,
                    LinesOfCode = 0
                };
            }
            
            // Analyze all C# documents in the project
            foreach (var document in project.Documents)
            {
                cancellationToken.ThrowIfCancellationRequested();
                
                if (document.SourceCodeKind != SourceCodeKind.Regular)
                    continue;
                
                filesAnalyzed++;
                
                var syntaxTree = await document.GetSyntaxTreeAsync(cancellationToken);
                if (syntaxTree == null)
                    continue;
                
                // Count lines of code
                var root = await syntaxTree.GetRootAsync(cancellationToken);
                linesOfCode += root.GetText().Lines.Count;
                
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                
                // Get diagnostics from compilation
                var diagnostics = compilation.GetDiagnostics();
                var fileDiagnostics = diagnostics.Where(d => d.Location.SourceTree == syntaxTree).ToList();
                
                // Convert diagnostics to violations
                foreach (var diagnostic in fileDiagnostics)
                {
                    var location = diagnostic.Location;
                    var lineSpan = location.GetLineSpan();
                    
                    var severity = diagnostic.Severity switch
                    {
                        DiagnosticSeverity.Error => PatternSeverity.Error,
                        DiagnosticSeverity.Warning => PatternSeverity.Warning,
                        DiagnosticSeverity.Info => PatternSeverity.Info,
                        _ => PatternSeverity.Info
                    };
                    
                    violations.Add(new PatternViolation(
                        Id: $"DIAG_{diagnostic.Id}_{violations.Count + 1}",
                        PatternId: diagnostic.Id,
                        PatternName: diagnostic.Descriptor?.Title?.ToString() ?? diagnostic.Id,
                        Severity: severity,
                        Message: diagnostic.GetMessage(),
                        FilePath: document.FilePath,
                        LineNumber: lineSpan.StartLinePosition.Line + 1,
                        ColumnNumber: lineSpan.StartLinePosition.Character + 1,
                        CodeSnippet: diagnostic.Location.GetLineSpan().ToString(),
                        Context: new Dictionary<string, object> { ["DiagnosticId"] = diagnostic.Id } as IReadOnlyDictionary<string, object>,
                        CreatedAt: DateTimeOffset.UtcNow));
                }
            }
            
            // Calculate compliance score (1.0 = perfect, 0.0 = all errors)
            var errorCount = violations.Count(v => v.Severity == PatternSeverity.Error || v.Severity == PatternSeverity.Critical);
            var warningCount = violations.Count(v => v.Severity == PatternSeverity.Warning);
            var totalViolations = violations.Count;
            var complianceScore = totalViolations == 0 
                ? 1.0f 
                : Math.Max(0.0f, 1.0f - (errorCount * 0.1f + warningCount * 0.05f)); // Penalize errors more than warnings
            
            stopwatch.Stop();
            
            _logger.LogInformation(
                "Project analysis completed: {ViolationsCount} violations, {FilesCount} files, {LinesCount} lines, Score={Score}",
                totalViolations,
                filesAnalyzed,
                linesOfCode,
                complianceScore);
            
            return new CodeAnalysisResult
            {
                Violations = violations,
                Suggestions = suggestions,
                ComplianceScore = complianceScore,
                ElapsedMilliseconds = stopwatch.ElapsedMilliseconds,
                FilesAnalyzed = filesAnalyzed,
                LinesOfCode = linesOfCode
            };
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Error analyzing project: {ProjectPath}", projectPath);
            
            return new CodeAnalysisResult
            {
                Violations = [],
                Suggestions = [],
                ComplianceScore = 0.0f,
                ElapsedMilliseconds = stopwatch.ElapsedMilliseconds,
                FilesAnalyzed = filesAnalyzed,
                LinesOfCode = linesOfCode
            };
        }
    }

    /// <inheritdoc />
    public async Task<CodeAnalysisResult> AnalyzeFileAsync(
        string filePath, 
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path cannot be null or empty", nameof(filePath));

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        _logger.LogInformation("Analyzing file: {FilePath}", filePath);
        
        cancellationToken.ThrowIfCancellationRequested();
        
        var violations = new List<PatternViolation>();
        var suggestions = new List<PatternSuggestion>();
        var linesOfCode = 0;
        
        try
        {
            // Check if file exists
            if (!File.Exists(filePath))
            {
                _logger.LogWarning("File does not exist: {FilePath}", filePath);
                stopwatch.Stop();
                return new CodeAnalysisResult
                {
                    Violations = [],
                    Suggestions = [],
                    ComplianceScore = 0.0f,
                    ElapsedMilliseconds = stopwatch.ElapsedMilliseconds,
                    FilesAnalyzed = 0,
                    LinesOfCode = 0
                };
            }
            
            // Read file content
            var fileContent = await File.ReadAllTextAsync(filePath, cancellationToken);
            
            // Parse code using Roslyn
            var syntaxTree = CSharpSyntaxTree.ParseText(fileContent, path: filePath);
            var root = await syntaxTree.GetRootAsync(cancellationToken);
            
            // Count lines of code
            linesOfCode = root.GetText().Lines.Count;
            
            // Get compilation for semantic analysis
            var references = AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES") as string;
            ImmutableArray<MetadataReference> metadataReferences;
            if (references != null)
            {
                var refs = references.Split(Path.PathSeparator)
                    .Where(p => !string.IsNullOrWhiteSpace(p) && File.Exists(p))
                    .Select(p => (MetadataReference)MetadataReference.CreateFromFile(p))
                    .ToArray();
                metadataReferences = ImmutableArray<MetadataReference>.Empty.AddRange(refs);
            }
            else
            {
                metadataReferences = ImmutableArray<MetadataReference>.Empty;
            }
            
            var compilation = CSharpCompilation.Create(
                "FileAnalysis",
                new[] { syntaxTree },
                metadataReferences,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
            
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            
            // Get diagnostics from compilation
            var diagnostics = compilation.GetDiagnostics();
            var fileDiagnostics = diagnostics.Where(d => d.Location.SourceTree == syntaxTree).ToList();
            
            // Convert diagnostics to violations
            foreach (var diagnostic in fileDiagnostics)
            {
                var location = diagnostic.Location;
                var lineSpan = location.GetLineSpan();
                
                var severity = diagnostic.Severity switch
                {
                    DiagnosticSeverity.Error => PatternSeverity.Error,
                    DiagnosticSeverity.Warning => PatternSeverity.Warning,
                    DiagnosticSeverity.Info => PatternSeverity.Info,
                    _ => PatternSeverity.Info
                };
                
                violations.Add(new PatternViolation(
                    Id: $"DIAG_{diagnostic.Id}_{violations.Count + 1}",
                    PatternId: diagnostic.Id,
                    PatternName: diagnostic.Descriptor?.Title?.ToString() ?? diagnostic.Id,
                    Severity: severity,
                    Message: diagnostic.GetMessage(),
                    FilePath: filePath,
                    LineNumber: lineSpan.StartLinePosition.Line + 1,
                    ColumnNumber: lineSpan.StartLinePosition.Character + 1,
                    CodeSnippet: diagnostic.Location.GetLineSpan().ToString(),
                    Context: new Dictionary<string, object> { ["DiagnosticId"] = diagnostic.Id } as IReadOnlyDictionary<string, object>,
                    CreatedAt: DateTimeOffset.UtcNow));
            }
            
            // Calculate compliance score
            var errorCount = violations.Count(v => v.Severity == PatternSeverity.Error || v.Severity == PatternSeverity.Critical);
            var warningCount = violations.Count(v => v.Severity == PatternSeverity.Warning);
            var complianceScore = violations.Count == 0 
                ? 1.0f 
                : Math.Max(0.0f, 1.0f - (errorCount * 0.1f + warningCount * 0.05f));
            
            stopwatch.Stop();
            
            _logger.LogInformation(
                "File analysis completed: {ViolationsCount} violations, {LinesCount} lines, Score={Score}",
                violations.Count,
                linesOfCode,
                complianceScore);
            
            return new CodeAnalysisResult
            {
                Violations = violations,
                Suggestions = suggestions,
                ComplianceScore = complianceScore,
                ElapsedMilliseconds = stopwatch.ElapsedMilliseconds,
                FilesAnalyzed = 1,
                LinesOfCode = linesOfCode
            };
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Error analyzing file: {FilePath}", filePath);
            
            return new CodeAnalysisResult
            {
                Violations = [],
                Suggestions = [],
                ComplianceScore = 0.0f,
                ElapsedMilliseconds = stopwatch.ElapsedMilliseconds,
                FilesAnalyzed = 0,
                LinesOfCode = 0
            };
        }
    }

    /// <inheritdoc />
    public async Task<CodeAnalysisResult> AnalyzeCodeAsync(
        string code, 
        string language, 
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Code cannot be null or empty", nameof(code));
        
        if (string.IsNullOrWhiteSpace(language))
            throw new ArgumentException("Language cannot be null or empty", nameof(language));

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        _logger.LogInformation("Analyzing code snippet in language: {Language}", language);
        
        cancellationToken.ThrowIfCancellationRequested();
        
        var violations = new List<PatternViolation>();
        var suggestions = new List<PatternSuggestion>();
        var linesOfCode = code.Split('\n', StringSplitOptions.None).Length;
        
        try
        {
            // Only support C# for now
            if (!language.Equals("C#", StringComparison.OrdinalIgnoreCase) && 
                !language.Equals("csharp", StringComparison.OrdinalIgnoreCase) &&
                !language.Equals("cs", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning("Unsupported language: {Language}, returning empty results", language);
                stopwatch.Stop();
                return new CodeAnalysisResult
                {
                    Violations = [],
                    Suggestions = [],
                    ComplianceScore = 1.0f,
                    ElapsedMilliseconds = stopwatch.ElapsedMilliseconds,
                    FilesAnalyzed = 0,
                    LinesOfCode = linesOfCode
                };
            }
            
            // Parse code using Roslyn
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var root = await syntaxTree.GetRootAsync(cancellationToken);
            
            // Get compilation for semantic analysis
            var references = AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES") as string;
            ImmutableArray<MetadataReference> metadataReferences;
            if (references != null)
            {
                var refs = references.Split(Path.PathSeparator)
                    .Where(p => !string.IsNullOrWhiteSpace(p) && File.Exists(p))
                    .Select(p => (MetadataReference)MetadataReference.CreateFromFile(p))
                    .ToArray();
                metadataReferences = ImmutableArray<MetadataReference>.Empty.AddRange(refs);
            }
            else
            {
                metadataReferences = ImmutableArray<MetadataReference>.Empty;
            }
            
            var compilation = CSharpCompilation.Create(
                "CodeAnalysis",
                new[] { syntaxTree },
                metadataReferences,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
            
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            
            // Get diagnostics from compilation
            var diagnostics = compilation.GetDiagnostics();
            var fileDiagnostics = diagnostics.Where(d => d.Location.SourceTree == syntaxTree).ToList();
            
            // Convert diagnostics to violations
            foreach (var diagnostic in fileDiagnostics)
            {
                var location = diagnostic.Location;
                var lineSpan = location.GetLineSpan();
                
                var severity = diagnostic.Severity switch
                {
                    DiagnosticSeverity.Error => PatternSeverity.Error,
                    DiagnosticSeverity.Warning => PatternSeverity.Warning,
                    DiagnosticSeverity.Info => PatternSeverity.Info,
                    _ => PatternSeverity.Info
                };
                
                violations.Add(new PatternViolation(
                    Id: $"DIAG_{diagnostic.Id}_{violations.Count + 1}",
                    PatternId: diagnostic.Id,
                    PatternName: diagnostic.Descriptor?.Title?.ToString() ?? diagnostic.Id,
                    Severity: severity,
                    Message: diagnostic.GetMessage(),
                    FilePath: null,
                    LineNumber: lineSpan.StartLinePosition.Line + 1,
                    ColumnNumber: lineSpan.StartLinePosition.Character + 1,
                    CodeSnippet: diagnostic.Location.GetLineSpan().ToString(),
                    Context: new Dictionary<string, object> { ["DiagnosticId"] = diagnostic.Id } as IReadOnlyDictionary<string, object>,
                    CreatedAt: DateTimeOffset.UtcNow));
            }
            
            // Calculate compliance score
            var errorCount = violations.Count(v => v.Severity == PatternSeverity.Error || v.Severity == PatternSeverity.Critical);
            var warningCount = violations.Count(v => v.Severity == PatternSeverity.Warning);
            var complianceScore = violations.Count == 0 
                ? 1.0f 
                : Math.Max(0.0f, 1.0f - (errorCount * 0.1f + warningCount * 0.05f));
            
            stopwatch.Stop();
            
            _logger.LogInformation(
                "Code analysis completed: {ViolationsCount} violations, {LinesCount} lines, Score={Score}",
                violations.Count,
                linesOfCode,
                complianceScore);
            
            return new CodeAnalysisResult
            {
                Violations = violations,
                Suggestions = suggestions,
                ComplianceScore = complianceScore,
                ElapsedMilliseconds = stopwatch.ElapsedMilliseconds,
                FilesAnalyzed = 1,
                LinesOfCode = linesOfCode
            };
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Error analyzing code snippet");
            
            return new CodeAnalysisResult
            {
                Violations = [],
                Suggestions = [],
                ComplianceScore = 0.0f,
                ElapsedMilliseconds = stopwatch.ElapsedMilliseconds,
                FilesAnalyzed = 0,
                LinesOfCode = linesOfCode
            };
        }
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<AnalyzerInfo>> GetAvailableAnalyzersAsync(
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting available analyzers");
        
        cancellationToken.ThrowIfCancellationRequested();
        
        var analyzers = new List<AnalyzerInfo>();
        
        try
        {
            // Get built-in Roslyn analyzers
            // Note: This is a simplified implementation - in production, you'd discover installed analyzers
            analyzers.Add(new AnalyzerInfo
            {
                Id = "CS0001",
                Name = "C# Compiler",
                Description = "C# compiler diagnostics and warnings",
                Category = "Compiler",
                IsEnabled = true
            });
            
            analyzers.Add(new AnalyzerInfo
            {
                Id = "CS0168",
                Name = "Unused Variable",
                Description = "Warns about unused variables",
                Category = "Code Quality",
                IsEnabled = true
            });
            
            analyzers.Add(new AnalyzerInfo
            {
                Id = "CS0219",
                Name = "Unused Assignment",
                Description = "Warns about unused assignments",
                Category = "Code Quality",
                IsEnabled = true
            });
            
            analyzers.Add(new AnalyzerInfo
            {
                Id = "CS4014",
                Name = "Async Warning",
                Description = "Warns about async methods without await",
                Category = "Async",
                IsEnabled = true
            });
            
            _logger.LogInformation("Found {Count} available analyzers", analyzers.Count);
            return analyzers;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting available analyzers");
            return [];
        }
    }
}
