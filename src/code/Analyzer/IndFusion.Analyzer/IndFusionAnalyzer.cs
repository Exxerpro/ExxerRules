using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using IndFusion.Analyzers.Architecture;
using IndFusion.Analyzers.Async;
using IndFusion.Analyzers.CodeFormatting;
using IndFusion.Analyzers.CodeQuality;
using IndFusion.Analyzers.Documentation;
using IndFusion.Analyzers.ErrorHandling;
using IndFusion.Analyzers.FunctionalPatterns;
using IndFusion.Analyzers.Logging;
using IndFusion.Analyzers.ModernCSharp;
using IndFusion.Analyzers.NullSafety;
using IndFusion.Analyzers.Performance;
using IndFusion.Analyzers.Testing;

namespace IndFusion.Analyzers;

/// <summary>
/// Coordinates registration of every diagnostics analyzer shipped in the IndFusion suite so the compiler loads a single entry point.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class IndFusionAnalyzer : DiagnosticAnalyzer
{
    /// <summary>
    /// Gets the complete catalog of diagnostics exposed by the composed analyzers.
    /// </summary>
    /// <value>
    /// An immutable array that aggregates the <see cref="DiagnosticDescriptor"/> instances from each analyzer registered by this package.
    /// </value>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
    {
        get
        {
            var builder = ImmutableArray.CreateBuilder<DiagnosticDescriptor>();
            
            // Architecture analyzers
            builder.AddRange(new DomainShouldNotReferenceInfrastructureAnalyzer().SupportedDiagnostics);
            builder.AddRange(new UseRepositoryPatternAnalyzer().SupportedDiagnostics);
            
            // Async analyzers
            builder.AddRange(new AsyncMethodsShouldAcceptCancellationTokenAnalyzer().SupportedDiagnostics);
            builder.AddRange(new AvoidAsyncVoidAnalyzer().SupportedDiagnostics);
            builder.AddRange(new UseConfigureAwaitFalseAnalyzer().SupportedDiagnostics);
            
            // Code formatting analyzers
            builder.AddRange(new CodeFormattingAnalyzer().SupportedDiagnostics);
            builder.AddRange(new ProjectFormattingAnalyzer().SupportedDiagnostics);
            
            // Code quality analyzers
            builder.AddRange(new AvoidMagicNumbersAndStringsAnalyzer().SupportedDiagnostics);
            builder.AddRange(new DoNotUseRegionsAnalyzer().SupportedDiagnostics);
            
            // Documentation analyzers
            builder.AddRange(new PublicMembersShouldHaveXmlDocumentationAnalyzer().SupportedDiagnostics);
            
            // Error handling analyzers
            builder.AddRange(new AvoidThrowingExceptionsAnalyzer().SupportedDiagnostics);
            builder.AddRange(new UseResultPatternAnalyzer().SupportedDiagnostics);
            
            // Functional patterns analyzers
            builder.AddRange(new DoNotThrowExceptionsAnalyzer().SupportedDiagnostics);
            
            // Logging analyzers
            builder.AddRange(new DoNotUseConsoleWriteLineAnalyzer().SupportedDiagnostics);
            builder.AddRange(new UseStructuredLoggingAnalyzer().SupportedDiagnostics);
            
            // Modern C# analyzers
            builder.AddRange(new UseExpressionBodiedMembersAnalyzer().SupportedDiagnostics);
            builder.AddRange(new UseModernPatternMatchingAnalyzer().SupportedDiagnostics);
            
            // Null safety analyzers
            builder.AddRange(new ValidateNullParametersAnalyzer().SupportedDiagnostics);
            
            // Performance analyzers
            builder.AddRange(new UseEfficientLinqAnalyzer().SupportedDiagnostics);
            
            // Testing analyzers
            builder.AddRange(new DoNotMockDbContextAnalyzer().SupportedDiagnostics);
            builder.AddRange(new DoNotUseFluentAssertionsAnalyzer().SupportedDiagnostics);
            builder.AddRange(new DoNotUseMoqAnalyzer().SupportedDiagnostics);
            builder.AddRange(new TestNamingConventionAnalyzer().SupportedDiagnostics);
            builder.AddRange(new UseXUnitV3Analyzer().SupportedDiagnostics);
            
            return builder.ToImmutable();
        }
    }

    /// <summary>
    /// Initializes the analyzer by delegating <paramref name="context"/> setup to each constituent analyzer instance.
    /// </summary>
    /// <param name="context">The Roslyn analysis context responsible for registering actions and configuring execution.</param>
    /// <remarks>
    /// Generated code is excluded and concurrent execution is enabled before dispatching initialization to every analyzer in the suite,
    /// ensuring consistent configuration across all diagnostics.
    /// </remarks>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        // Register all analyzers
        var analyzers = new DiagnosticAnalyzer[]
        {
            // Architecture analyzers
            new DomainShouldNotReferenceInfrastructureAnalyzer(),
            new UseRepositoryPatternAnalyzer(),
            
            // Async analyzers
            new AsyncMethodsShouldAcceptCancellationTokenAnalyzer(),
            new AvoidAsyncVoidAnalyzer(),
            new UseConfigureAwaitFalseAnalyzer(),
            
            // Code formatting analyzers
            new CodeFormattingAnalyzer(),
            new ProjectFormattingAnalyzer(),
            
            // Code quality analyzers
            new AvoidMagicNumbersAndStringsAnalyzer(),
            new DoNotUseRegionsAnalyzer(),
            
            // Documentation analyzers
            new PublicMembersShouldHaveXmlDocumentationAnalyzer(),
            
            // Error handling analyzers
            new AvoidThrowingExceptionsAnalyzer(),
            new UseResultPatternAnalyzer(),
            
            // Functional patterns analyzers
            new DoNotThrowExceptionsAnalyzer(),
            
            // Logging analyzers
            new DoNotUseConsoleWriteLineAnalyzer(),
            new UseStructuredLoggingAnalyzer(),
            
            // Modern C# analyzers
            new UseExpressionBodiedMembersAnalyzer(),
            new UseModernPatternMatchingAnalyzer(),
            
            // Null safety analyzers
            new ValidateNullParametersAnalyzer(),
            
            // Performance analyzers
            new UseEfficientLinqAnalyzer(),
            
            // Testing analyzers
            new DoNotMockDbContextAnalyzer(),
            new DoNotUseFluentAssertionsAnalyzer(),
            new DoNotUseMoqAnalyzer(),
            new TestNamingConventionAnalyzer(),
            new UseXUnitV3Analyzer()
        };

        foreach (var analyzer in analyzers)
        {
            analyzer.Initialize(context);
        }
    }
}
