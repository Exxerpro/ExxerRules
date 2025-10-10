using Microsoft.Extensions.Logging;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using IndFusion.Tools.Cli.Commands;

namespace IndFusion.Tools.Cli.Services;

/// <summary>
/// Analyzes code for metrics, complexity, and refactoring opportunities
/// </summary>
public class CodeAnalyzer
{
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the CodeAnalyzer class
    /// </summary>
    /// <param name="logger">The logger instance</param>
    public CodeAnalyzer(ILogger logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Analyzes code based on the analysis request
    /// </summary>
    /// <param name="request">The analysis request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Analysis result</returns>
    public async Task<AnalysisResult> AnalyzeAsync(AnalysisRequest request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Starting code analysis: {AnalysisType}", request.AnalysisType);

            // Load solution
            using var workspace = MSBuildWorkspace.Create();
            var solution = await workspace.OpenSolutionAsync(request.SolutionPath, progress: null, cancellationToken);
            
            var analysisData = new AnalysisData();

            // Perform analysis based on type
            switch (request.AnalysisType.ToLowerInvariant())
            {
                case "metrics":
                    await AnalyzeMetricsAsync(solution, analysisData, request, cancellationToken);
                    break;
                case "complexity":
                    await AnalyzeComplexityAsync(solution, analysisData, request, cancellationToken);
                    break;
                case "opportunities":
                    await AnalyzeOpportunitiesAsync(solution, analysisData, request, cancellationToken);
                    break;
                case "all":
                    await AnalyzeMetricsAsync(solution, analysisData, request, cancellationToken);
                    await AnalyzeComplexityAsync(solution, analysisData, request, cancellationToken);
                    await AnalyzeOpportunitiesAsync(solution, analysisData, request, cancellationToken);
                    break;
                default:
                    return AnalysisResult.Failure($"Unknown analysis type: {request.AnalysisType}");
            }

            // Generate output based on format
            var result = GenerateOutput(analysisData, request.OutputFormat);
            
            _logger.LogInformation("Code analysis completed successfully");
            return AnalysisResult.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during code analysis");
            return AnalysisResult.Failure($"Analysis failed: {ex.Message}");
        }
    }

    /// <summary>
    /// Analyzes code metrics
    /// </summary>
    private async Task AnalyzeMetricsAsync(Solution solution, AnalysisData data, AnalysisRequest request, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Analyzing code metrics");

        foreach (var project in solution.Projects)
        {
            var compilation = await project.GetCompilationAsync(cancellationToken);
            if (compilation == null) continue;

            foreach (var syntaxTree in compilation.SyntaxTrees)
            {
                var root = await syntaxTree.GetRootAsync(cancellationToken);
                var semanticModel = compilation.GetSemanticModel(syntaxTree);

                var metrics = CalculateMetrics(root, semanticModel);
                data.Metrics.Add(new FileMetrics
                {
                    FilePath = syntaxTree.FilePath,
                    LinesOfCode = metrics.LinesOfCode,
                    CyclomaticComplexity = metrics.CyclomaticComplexity,
                    MethodCount = metrics.MethodCount,
                    ClassCount = metrics.ClassCount,
                    InterfaceCount = metrics.InterfaceCount,
                    PropertyCount = metrics.PropertyCount,
                    FieldCount = metrics.FieldCount
                });
            }
        }
    }

    /// <summary>
    /// Analyzes code complexity
    /// </summary>
    private async Task AnalyzeComplexityAsync(Solution solution, AnalysisData data, AnalysisRequest request, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Analyzing code complexity");

        foreach (var project in solution.Projects)
        {
            var compilation = await project.GetCompilationAsync(cancellationToken);
            if (compilation == null) continue;

            foreach (var syntaxTree in compilation.SyntaxTrees)
            {
                var root = await syntaxTree.GetRootAsync(cancellationToken);
                var semanticModel = compilation.GetSemanticModel(syntaxTree);

                var complexityAnalyzer = new ComplexityAnalyzer();
                complexityAnalyzer.Visit(root);

                foreach (var method in complexityAnalyzer.ComplexMethods)
                {
                    data.ComplexityIssues.Add(new ComplexityIssue
                    {
                        FilePath = syntaxTree.FilePath,
                        MethodName = method.Name,
                        Line = method.Line,
                        Complexity = method.Complexity,
                        Issue = $"Method '{method.Name}' has high cyclomatic complexity ({method.Complexity})"
                    });
                }
            }
        }
    }

    /// <summary>
    /// Analyzes refactoring opportunities
    /// </summary>
    private async Task AnalyzeOpportunitiesAsync(Solution solution, AnalysisData data, AnalysisRequest request, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Analyzing refactoring opportunities");

        foreach (var project in solution.Projects)
        {
            var compilation = await project.GetCompilationAsync(cancellationToken);
            if (compilation == null) continue;

            foreach (var syntaxTree in compilation.SyntaxTrees)
            {
                var root = await syntaxTree.GetRootAsync(cancellationToken);
                var semanticModel = compilation.GetSemanticModel(syntaxTree);

                var opportunityAnalyzer = new OpportunityAnalyzer();
                opportunityAnalyzer.Visit(root);

                foreach (var opportunity in opportunityAnalyzer.Opportunities)
                {
                    data.Opportunities.Add(new RefactoringOpportunity
                    {
                        Type = opportunity.Type,
                        File = syntaxTree.FilePath,
                        Line = opportunity.Line,
                        Description = opportunity.Description
                    });
                }
            }
        }
    }

    /// <summary>
    /// Calculates basic metrics for a syntax tree
    /// </summary>
    private static CodeMetrics CalculateMetrics(SyntaxNode root, SemanticModel semanticModel)
    {
        var metrics = new CodeMetrics();
        
        // Count lines of code (excluding comments and whitespace)
        var lines = root.SyntaxTree.GetText().Lines;
        metrics.LinesOfCode = lines.Count(line => !string.IsNullOrWhiteSpace(line.ToString()) && !line.ToString().TrimStart().StartsWith("//"));

        // Count different types of declarations
        var classDeclarations = root.DescendantNodes().OfType<ClassDeclarationSyntax>();
        var methodDeclarations = root.DescendantNodes().OfType<MethodDeclarationSyntax>();
        var propertyDeclarations = root.DescendantNodes().OfType<PropertyDeclarationSyntax>();
        var fieldDeclarations = root.DescendantNodes().OfType<FieldDeclarationSyntax>();
        var interfaceDeclarations = root.DescendantNodes().OfType<InterfaceDeclarationSyntax>();

        metrics.ClassCount = classDeclarations.Count();
        metrics.MethodCount = methodDeclarations.Count();
        metrics.PropertyCount = propertyDeclarations.Count();
        metrics.FieldCount = fieldDeclarations.Count();
        metrics.InterfaceCount = interfaceDeclarations.Count();

        // Calculate cyclomatic complexity
        metrics.CyclomaticComplexity = CalculateCyclomaticComplexity(root);

        return metrics;
    }

    /// <summary>
    /// Calculates cyclomatic complexity for a syntax tree
    /// </summary>
    private static int CalculateCyclomaticComplexity(SyntaxNode root)
    {
        var complexity = 1; // Base complexity

        // Count decision points
        complexity += root.DescendantNodes().OfType<IfStatementSyntax>().Count();
        complexity += root.DescendantNodes().OfType<WhileStatementSyntax>().Count();
        complexity += root.DescendantNodes().OfType<ForStatementSyntax>().Count();
        complexity += root.DescendantNodes().OfType<ForEachStatementSyntax>().Count();
        complexity += root.DescendantNodes().OfType<SwitchStatementSyntax>().Count();
        complexity += root.DescendantNodes().OfType<ConditionalExpressionSyntax>().Count();
        complexity += root.DescendantNodes().OfType<CatchClauseSyntax>().Count();

        return complexity;
    }

    /// <summary>
    /// Generates output based on the requested format
    /// </summary>
    private static AnalysisOutput GenerateOutput(AnalysisData data, string format)
    {
        return format.ToLowerInvariant() switch
        {
            "json" => GenerateJsonOutput(data),
            "csv" => GenerateCsvOutput(data),
            "markdown" => GenerateMarkdownOutput(data),
            "console" or _ => GenerateConsoleOutput(data)
        };
    }

    /// <summary>
    /// Generates JSON output
    /// </summary>
    private static AnalysisOutput GenerateJsonOutput(AnalysisData data)
    {
        var json = System.Text.Json.JsonSerializer.Serialize(data, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
        return new AnalysisOutput { JsonOutput = json };
    }

    /// <summary>
    /// Generates CSV output
    /// </summary>
    private static AnalysisOutput GenerateCsvOutput(AnalysisData data)
    {
        var csv = new System.Text.StringBuilder();
        
        // Metrics CSV
        csv.AppendLine("Type,FilePath,LinesOfCode,CyclomaticComplexity,MethodCount,ClassCount,InterfaceCount,PropertyCount,FieldCount");
        foreach (var metric in data.Metrics)
        {
            csv.AppendLine($"Metrics,{metric.FilePath},{metric.LinesOfCode},{metric.CyclomaticComplexity},{metric.MethodCount},{metric.ClassCount},{metric.InterfaceCount},{metric.PropertyCount},{metric.FieldCount}");
        }

        // Complexity CSV
        csv.AppendLine("Type,FilePath,MethodName,Line,Complexity,Issue");
        foreach (var issue in data.ComplexityIssues)
        {
            csv.AppendLine($"Complexity,{issue.FilePath},{issue.MethodName},{issue.Line},{issue.Complexity},\"{issue.Issue}\"");
        }

        // Opportunities CSV
        csv.AppendLine("Type,FilePath,Line,RefactoringType,Description");
        foreach (var opportunity in data.Opportunities)
        {
            csv.AppendLine($"Opportunity,{opportunity.File},{opportunity.Line},{opportunity.Type},\"{opportunity.Description}\"");
        }

        return new AnalysisOutput { CsvOutput = csv.ToString() };
    }

    /// <summary>
    /// Generates Markdown output
    /// </summary>
    private static AnalysisOutput GenerateMarkdownOutput(AnalysisData data)
    {
        var markdown = new System.Text.StringBuilder();
        
        markdown.AppendLine("# Code Analysis Report");
        markdown.AppendLine();

        // Metrics section
        markdown.AppendLine("## Code Metrics");
        markdown.AppendLine();
        markdown.AppendLine("| File | Lines of Code | Complexity | Methods | Classes | Interfaces | Properties | Fields |");
        markdown.AppendLine("|------|---------------|------------|---------|---------|------------|------------|--------|");
        
        foreach (var metric in data.Metrics)
        {
            markdown.AppendLine($"| {metric.FilePath} | {metric.LinesOfCode} | {metric.CyclomaticComplexity} | {metric.MethodCount} | {metric.ClassCount} | {metric.InterfaceCount} | {metric.PropertyCount} | {metric.FieldCount} |");
        }

        // Complexity section
        if (data.ComplexityIssues.Any())
        {
            markdown.AppendLine();
            markdown.AppendLine("## Complexity Issues");
            markdown.AppendLine();
            markdown.AppendLine("| File | Method | Line | Complexity | Issue |");
            markdown.AppendLine("|------|--------|------|------------|-------|");
            
            foreach (var issue in data.ComplexityIssues)
            {
                markdown.AppendLine($"| {issue.FilePath} | {issue.MethodName} | {issue.Line} | {issue.Complexity} | {issue.Issue} |");
            }
        }

        // Opportunities section
        if (data.Opportunities.Any())
        {
            markdown.AppendLine();
            markdown.AppendLine("## Refactoring Opportunities");
            markdown.AppendLine();
            markdown.AppendLine("| File | Line | Type | Description |");
            markdown.AppendLine("|------|------|------|-------------|");
            
            foreach (var opportunity in data.Opportunities)
            {
                markdown.AppendLine($"| {opportunity.File} | {opportunity.Line} | {opportunity.Type} | {opportunity.Description} |");
            }
        }

        return new AnalysisOutput { MarkdownOutput = markdown.ToString() };
    }

    /// <summary>
    /// Generates console output
    /// </summary>
    private static AnalysisOutput GenerateConsoleOutput(AnalysisData data)
    {
        var console = new System.Text.StringBuilder();
        
        console.AppendLine("📊 Code Analysis Report");
        console.AppendLine("======================");
        console.AppendLine();

        // Summary
        var totalFiles = data.Metrics.Count;
        var totalLines = data.Metrics.Sum(m => m.LinesOfCode);
        var totalMethods = data.Metrics.Sum(m => m.MethodCount);
        var totalClasses = data.Metrics.Sum(m => m.ClassCount);
        var avgComplexity = data.Metrics.Any() ? data.Metrics.Average(m => m.CyclomaticComplexity) : 0;

        console.AppendLine($"📈 Summary:");
        console.AppendLine($"   Files analyzed: {totalFiles}");
        console.AppendLine($"   Total lines of code: {totalLines:N0}");
        console.AppendLine($"   Total methods: {totalMethods}");
        console.AppendLine($"   Total classes: {totalClasses}");
        console.AppendLine($"   Average complexity: {avgComplexity:F1}");
        console.AppendLine();

        // Top complex files
        var topComplexFiles = data.Metrics.OrderByDescending(m => m.CyclomaticComplexity).Take(5);
        if (topComplexFiles.Any())
        {
            console.AppendLine("🔴 Top 5 Most Complex Files:");
            foreach (var file in topComplexFiles)
            {
                console.AppendLine($"   {file.FilePath} (Complexity: {file.CyclomaticComplexity})");
            }
            console.AppendLine();
        }

        // Complexity issues
        if (data.ComplexityIssues.Any())
        {
            console.AppendLine($"⚠️  Complexity Issues ({data.ComplexityIssues.Count}):");
            foreach (var issue in data.ComplexityIssues.Take(10))
            {
                console.AppendLine($"   {issue.FilePath}:{issue.Line} - {issue.Issue}");
            }
            if (data.ComplexityIssues.Count > 10)
            {
                console.AppendLine($"   ... and {data.ComplexityIssues.Count - 10} more");
            }
            console.AppendLine();
        }

        // Refactoring opportunities
        if (data.Opportunities.Any())
        {
            console.AppendLine($"🔧 Refactoring Opportunities ({data.Opportunities.Count}):");
            foreach (var opportunity in data.Opportunities.Take(10))
            {
                console.AppendLine($"   {opportunity.File}:{opportunity.Line} - {opportunity.Type}: {opportunity.Description}");
            }
            if (data.Opportunities.Count > 10)
            {
                console.AppendLine($"   ... and {data.Opportunities.Count - 10} more");
            }
        }

        return new AnalysisOutput { ConsoleOutput = console.ToString() };
    }
}

/// <summary>
/// Represents the result of code analysis
/// </summary>
public class AnalysisResult
{
    /// <summary>
    /// Gets whether the analysis was successful
    /// </summary>
    public bool Success { get; private set; }

    /// <summary>
    /// Gets the error message if the analysis failed
    /// </summary>
    public string? ErrorMessage { get; private set; }

    /// <summary>
    /// Gets the analysis output
    /// </summary>
    public AnalysisOutput? Output { get; private set; }

    /// <summary>
    /// Creates a successful analysis result
    /// </summary>
    /// <param name="output">The analysis output</param>
    /// <returns>Successful analysis result</returns>
    public static AnalysisResult Success(AnalysisOutput output)
    {
        return new AnalysisResult { Success = true, Output = output };
    }

    /// <summary>
    /// Creates a failed analysis result
    /// </summary>
    /// <param name="errorMessage">Error message</param>
    /// <returns>Failed analysis result</returns>
    public static AnalysisResult Failure(string errorMessage)
    {
        return new AnalysisResult { Success = false, ErrorMessage = errorMessage };
    }
}

/// <summary>
/// Represents the output of code analysis in different formats
/// </summary>
public class AnalysisOutput
{
    /// <summary>
    /// Gets or sets the JSON output
    /// </summary>
    public string? JsonOutput { get; set; }

    /// <summary>
    /// Gets or sets the CSV output
    /// </summary>
    public string? CsvOutput { get; set; }

    /// <summary>
    /// Gets or sets the Markdown output
    /// </summary>
    public string? MarkdownOutput { get; set; }

    /// <summary>
    /// Gets or sets the console output
    /// </summary>
    public string? ConsoleOutput { get; set; }
}

/// <summary>
/// Contains all analysis data
/// </summary>
public class AnalysisData
{
    /// <summary>
    /// Gets or sets the code metrics
    /// </summary>
    public List<FileMetrics> Metrics { get; set; } = new();

    /// <summary>
    /// Gets or sets the complexity issues
    /// </summary>
    public List<ComplexityIssue> ComplexityIssues { get; set; } = new();

    /// <summary>
    /// Gets or sets the refactoring opportunities
    /// </summary>
    public List<RefactoringOpportunity> Opportunities { get; set; } = new();
}

/// <summary>
/// Represents metrics for a file
/// </summary>
public class FileMetrics
{
    /// <summary>
    /// Gets or sets the file path
    /// </summary>
    public string FilePath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the lines of code
    /// </summary>
    public int LinesOfCode { get; set; }

    /// <summary>
    /// Gets or sets the cyclomatic complexity
    /// </summary>
    public int CyclomaticComplexity { get; set; }

    /// <summary>
    /// Gets or sets the method count
    /// </summary>
    public int MethodCount { get; set; }

    /// <summary>
    /// Gets or sets the class count
    /// </summary>
    public int ClassCount { get; set; }

    /// <summary>
    /// Gets or sets the interface count
    /// </summary>
    public int InterfaceCount { get; set; }

    /// <summary>
    /// Gets or sets the property count
    /// </summary>
    public int PropertyCount { get; set; }

    /// <summary>
    /// Gets or sets the field count
    /// </summary>
    public int FieldCount { get; set; }
}

/// <summary>
/// Represents a complexity issue
/// </summary>
public class ComplexityIssue
{
    /// <summary>
    /// Gets or sets the file path
    /// </summary>
    public string FilePath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the method name
    /// </summary>
    public string MethodName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the line number
    /// </summary>
    public int Line { get; set; }

    /// <summary>
    /// Gets or sets the complexity value
    /// </summary>
    public int Complexity { get; set; }

    /// <summary>
    /// Gets or sets the issue description
    /// </summary>
    public string Issue { get; set; } = string.Empty;
}

/// <summary>
/// Represents basic code metrics
/// </summary>
public class CodeMetrics
{
    /// <summary>
    /// Gets or sets the lines of code
    /// </summary>
    public int LinesOfCode { get; set; }

    /// <summary>
    /// Gets or sets the cyclomatic complexity
    /// </summary>
    public int CyclomaticComplexity { get; set; }

    /// <summary>
    /// Gets or sets the method count
    /// </summary>
    public int MethodCount { get; set; }

    /// <summary>
    /// Gets or sets the class count
    /// </summary>
    public int ClassCount { get; set; }

    /// <summary>
    /// Gets or sets the interface count
    /// </summary>
    public int InterfaceCount { get; set; }

    /// <summary>
    /// Gets or sets the property count
    /// </summary>
    public int PropertyCount { get; set; }

    /// <summary>
    /// Gets or sets the field count
    /// </summary>
    public int FieldCount { get; set; }
}

/// <summary>
/// Analyzer for detecting complex methods
/// </summary>
public class ComplexityAnalyzer : CSharpSyntaxWalker
{
    /// <summary>
    /// Gets the list of complex methods
    /// </summary>
    public List<ComplexMethod> ComplexMethods { get; } = new();

    /// <summary>
    /// Visits a method declaration
    /// </summary>
    public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
    {
        var complexity = CalculateComplexity(node);
        if (complexity > 10) // Threshold for complex methods
        {
            ComplexMethods.Add(new ComplexMethod
            {
                Name = node.Identifier.ValueText,
                Line = node.GetLocation().GetLineSpan().StartLinePosition.Line + 1,
                Complexity = complexity
            });
        }

        base.VisitMethodDeclaration(node);
    }

    /// <summary>
    /// Calculates complexity for a method
    /// </summary>
    private static int CalculateComplexity(SyntaxNode node)
    {
        var complexity = 1; // Base complexity

        // Count decision points
        complexity += node.DescendantNodes().OfType<IfStatementSyntax>().Count();
        complexity += node.DescendantNodes().OfType<WhileStatementSyntax>().Count();
        complexity += node.DescendantNodes().OfType<ForStatementSyntax>().Count();
        complexity += node.DescendantNodes().OfType<ForEachStatementSyntax>().Count();
        complexity += node.DescendantNodes().OfType<SwitchStatementSyntax>().Count();
        complexity += node.DescendantNodes().OfType<ConditionalExpressionSyntax>().Count();
        complexity += node.DescendantNodes().OfType<CatchClauseSyntax>().Count();

        return complexity;
    }
}

/// <summary>
/// Represents a complex method
/// </summary>
public class ComplexMethod
{
    /// <summary>
    /// Gets or sets the method name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the line number
    /// </summary>
    public int Line { get; set; }

    /// <summary>
    /// Gets or sets the complexity value
    /// </summary>
    public int Complexity { get; set; }
}

/// <summary>
/// Analyzer for detecting refactoring opportunities
/// </summary>
public class OpportunityAnalyzer : CSharpSyntaxWalker
{
    /// <summary>
    /// Gets the list of refactoring opportunities
    /// </summary>
    public List<OpportunityInfo> Opportunities { get; } = new();

    /// <summary>
    /// Visits a method declaration
    /// </summary>
    public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
    {
        // Check for long methods
        var lineCount = node.GetLocation().GetLineSpan().EndLinePosition.Line - node.GetLocation().GetLineSpan().StartLinePosition.Line + 1;
        if (lineCount > 50)
        {
            Opportunities.Add(new OpportunityInfo
            {
                Type = "ExtractMethod",
                Line = node.GetLocation().GetLineSpan().StartLinePosition.Line + 1,
                Description = $"Method '{node.Identifier.ValueText}' is too long ({lineCount} lines). Consider extracting methods."
            });
        }

        // Check for too many parameters
        if (node.ParameterList.Parameters.Count > 5)
        {
            Opportunities.Add(new OpportunityInfo
            {
                Type = "IntroduceParameterObject",
                Line = node.GetLocation().GetLineSpan().StartLinePosition.Line + 1,
                Description = $"Method '{node.Identifier.ValueText}' has too many parameters ({node.ParameterList.Parameters.Count}). Consider introducing a parameter object."
            });
        }

        base.VisitMethodDeclaration(node);
    }

    /// <summary>
    /// Visits a class declaration
    /// </summary>
    public override void VisitClassDeclaration(ClassDeclarationSyntax node)
    {
        // Check for large classes
        var memberCount = node.Members.Count;
        if (memberCount > 20)
        {
            Opportunities.Add(new OpportunityInfo
            {
                Type = "ExtractClass",
                Line = node.GetLocation().GetLineSpan().StartLinePosition.Line + 1,
                Description = $"Class '{node.Identifier.ValueText}' is too large ({memberCount} members). Consider extracting classes."
            });
        }

        base.VisitClassDeclaration(node);
    }
}

/// <summary>
/// Represents a refactoring opportunity
/// </summary>
public class OpportunityInfo
{
    /// <summary>
    /// Gets or sets the type of refactoring
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the line number
    /// </summary>
    public int Line { get; set; }

    /// <summary>
    /// Gets or sets the description
    /// </summary>
    public string Description { get; set; } = string.Empty;
}