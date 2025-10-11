using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading;
using IndFusion.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace IndFusion.Analyzer.Tests.Testing;

/// <summary>
/// Provides helper methods for testing diagnostic analyzers.
/// SRP: Encapsulates analyzer execution logic and compilation details.
/// </summary>
public static class AnalyzerTestHelper
{
    /// <summary>
    /// Runs a diagnostic analyzer on the given source code and returns the diagnostics.
    /// </summary>
    /// <param name="sourceCode">The source code to analyze.</param>
    /// <param name="analyzer">The analyzer to run.</param>
    /// <returns>The diagnostics reported by the analyzer.</returns>
    public static ImmutableArray<Diagnostic> RunAnalyzer(string sourceCode, DiagnosticAnalyzer analyzer) => RunAnalyzer(sourceCode, analyzer, includeHidden: false);

    /// <summary>
    /// Runs analyzer with support for passing a file path and analyzer options.
    /// </summary>
    public static ImmutableArray<Diagnostic> RunAnalyzer(
        string sourceCode,
        DiagnosticAnalyzer analyzer,
        string? filePath = null,
        AnalyzerOptions? analyzerOptions = null,
        bool includeHidden = false)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(sourceCode, path: filePath ?? "TestFile.cs");
        var references = GetMetadataReferences();

        var compilation = CSharpCompilation.Create(
            "TestAssembly",
            [syntaxTree],
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var compilationWithAnalyzers = analyzerOptions is null
            ? compilation.WithAnalyzers([analyzer])
            : compilation.WithAnalyzers([analyzer], new CompilationWithAnalyzersOptions(
                analyzerOptions,
                onAnalyzerException: null,
                concurrentAnalysis: true,
                logAnalyzerExecutionTime: false,
                reportSuppressedDiagnostics: includeHidden));

        var token = TryGetTestCancellationToken();
        var diagnostics = compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync(token).GetAwaiter().GetResult();
        return includeHidden
            ? [.. diagnostics]
            : [.. diagnostics.Where(d => d.Severity != DiagnosticSeverity.Hidden || d.Id == DiagnosticIds.ProjectFormatting)];
    }


    /// <summary>
    /// Runs a diagnostic analyzer on given source and returns diagnostics.
    /// Honors the current xUnit test cancellation token when available.
    /// </summary>
    /// <param name="sourceCode">C# source to analyze.</param>
    /// <param name="analyzer">Analyzer instance.</param>
    /// <param name="includeHidden">Include hidden diagnostics when true.</param>
    /// <returns>Diagnostics produced by the analyzer.</returns>
    public static ImmutableArray<Diagnostic> RunAnalyzer(string sourceCode, DiagnosticAnalyzer analyzer, bool includeHidden)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(sourceCode);
        var references = GetMetadataReferences();

        var compilation = CSharpCompilation.Create(
            "TestAssembly",
            [syntaxTree],
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var compilationWithAnalyzers = compilation.WithAnalyzers([analyzer]);

        // Use xUnit v3 TestContext cancellation token when running under tests
        var token = TryGetTestCancellationToken();

        var diagnostics = compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync(token).GetAwaiter().GetResult();
        return includeHidden
            ? [.. diagnostics]
            : [.. diagnostics.Where(d => d.Severity != DiagnosticSeverity.Hidden || d.Id == DiagnosticIds.ProjectFormatting)];
    }

    /// <summary>
    /// Attempts to acquire xUnit v3 TestContext cancellation token; returns default otherwise.
    /// </summary>
    private static CancellationToken TryGetTestCancellationToken()
    {
        try
        {
            // xUnit v3 exposes TestContext.Current.CancellationToken
            var testContextType = Type.GetType("Xunit.Sdk.TestContext, xunit.v3.core")
                                  ?? Type.GetType("Xunit.Sdk.TestContext, xunit.v3");
            if (testContextType is null) return default;
            var currentProp = testContextType.GetProperty("Current", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            var current = currentProp?.GetValue(null);
            var ctProp = current?.GetType().GetProperty("CancellationToken");
            if (ctProp is null) return default;
            var ctObj = ctProp.GetValue(current);
            return ctObj is CancellationToken ct ? ct : default;
        }
        catch
        {
            return default;
        }
    }

    /// <summary>
    /// Gets the metadata references required for compilation.
    /// </summary>
    /// <returns>An array of metadata references.</returns>
    private static MetadataReference[] GetMetadataReferences()
    {
        // Get basic .NET references
        var trustedAssembliesPaths = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!)
            .Split(Path.PathSeparator);

        var neededAssemblies = new[]
{
	"System.Runtime",
	"System.Collections",
	"System.Linq",
	"System.Console",
	"System.CodeDom",
	"System.ComponentModel",
	"System.ComponentModel.Primitives",
	"System.ComponentModel.TypeConverter",
	"System.Runtime.Extensions",
	"System.Runtime.InteropServices",
	"System.Threading",
	"System.Threading.Tasks",
	"netstandard",
	"mscorlib",
	"Microsoft.Extensions.Logging",
	"Microsoft.Extensions.Logging.Abstractions",
	"Microsoft.EntityFrameworkCore",
	"Microsoft.EntityFrameworkCore.Abstractions",
	"Microsoft.AspNetCore.Components",
	"Microsoft.AspNetCore.Components.Web",
	"Microsoft.Data.SqlClient",
	"System.Data.SqlClient"
};

        var references = trustedAssembliesPaths
            .Where(p => neededAssemblies.Any(na => Path.GetFileNameWithoutExtension(p) == na))
            .Select(p => MetadataReference.CreateFromFile(p))
            .ToList();

        // Add specific references for testing framework
        try
        {
            // Try to add XUnit reference if available
            var xunitPath = Path.Combine(AppContext.BaseDirectory, "xunit.v3.core.dll");
            if (File.Exists(xunitPath))
            {
                references.Add(MetadataReference.CreateFromFile(xunitPath));
            }

            // Try to add Moq reference if available
            var moqPath = Path.Combine(AppContext.BaseDirectory, "Moq.dll");
            if (File.Exists(moqPath))
            {
                references.Add(MetadataReference.CreateFromFile(moqPath));
            }

            // Try to add NSubstitute reference if available
            var nsubstitutePath = Path.Combine(AppContext.BaseDirectory, "NSubstitute.dll");
            if (File.Exists(nsubstitutePath))
            {
                references.Add(MetadataReference.CreateFromFile(nsubstitutePath));
            }
        }
        catch
        {
            // If references aren't available, continue without them
        }

        return [.. references];
    }
}
