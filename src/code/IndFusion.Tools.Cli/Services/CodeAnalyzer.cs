namespace IndFusion.Tools.Cli.Services;

using IndFusion.Mcp.Core.Abstractions;

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
    /// Analyzes code for quality metrics and refactoring opportunities
    /// </summary>
    /// <param name="solutionPath">Path to the solution file</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <param name="request">Analysis request parameters</param>
    /// <returns>Analysis result</returns>
    public async Task<AnalysisResult> AnalyzeAsync(string solutionPath, AnalysisRequest request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Starting code analysis: {AnalysisType}", request.AnalysisType);

            // Load solution
            using var workspace = MSBuildWorkspace.Create();

            if (request.SolutionPath is null) return AnalysisResult.Failure($"Solution Null path: {request.AnalysisType}");

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
        if (data.ComplexityIssues.Count != 0)
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
        if (data.Opportunities.Count != 0)
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
        var avgComplexity = data.Metrics.Count != 0 ? data.Metrics.Average(m => m.CyclomaticComplexity) : 0;

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
        if (data.ComplexityIssues.Count != 0)
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
        if (data.Opportunities.Count != 0)
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