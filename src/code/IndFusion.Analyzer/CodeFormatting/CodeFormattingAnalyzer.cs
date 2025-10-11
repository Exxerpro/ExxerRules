using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace IndFusion.Analyzers.CodeFormatting;

/// <summary>
/// Analyzer that detects common formatting issues and suggests running dotnet format.
/// SRP: Responsible for detecting formatting inconsistencies and suggesting automated fixes.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class CodeFormattingAnalyzer : DiagnosticAnalyzer
{
    private static readonly LocalizableString Title = "Code formatting inconsistency detected";
    private static readonly LocalizableString MessageFormat = "Formatting issue detected: {0}. Consider running 'dotnet format' to fix automatically.";
    private static readonly LocalizableString Description = "Detects common code formatting issues that can be automatically fixed with 'dotnet format' command.";

    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticIds.CodeFormattingIssue,
        Title,
        MessageFormat,
        DiagnosticCategories.CodeQuality,
        DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: Description);

    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    /// <inheritdoc/>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        // Register various syntax node analyzers for formatting issues
        context.RegisterSyntaxNodeAction(AnalyzeClassDeclaration, SyntaxKind.ClassDeclaration);
        context.RegisterSyntaxNodeAction(AnalyzeMethodDeclaration, SyntaxKind.MethodDeclaration);
        context.RegisterSyntaxNodeAction(AnalyzePropertyDeclaration, SyntaxKind.PropertyDeclaration);
        context.RegisterSyntaxNodeAction(AnalyzeVariableDeclaration, SyntaxKind.VariableDeclaration);
    }

    private static void AnalyzeClassDeclaration(SyntaxNodeAnalysisContext context)
    {
        var classDeclaration = (ClassDeclarationSyntax)context.Node;

        // Check for inconsistent brace placement
        if (HasInconsistentBraces(classDeclaration))
        {
            ReportFormattingIssue(context, classDeclaration, "Inconsistent brace placement");
        }

        // Check for missing blank lines between members
        if (HasMissingBlankLinesBetweenMembers(classDeclaration))
        {
            ReportFormattingIssue(context, classDeclaration, "Missing blank lines between class members");
        }
    }

    private static void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context)
    {
        var methodDeclaration = (MethodDeclarationSyntax)context.Node;

        // Check for inconsistent parameter formatting
        if (HasInconsistentParameterFormatting(methodDeclaration))
        {
            ReportFormattingIssue(context, methodDeclaration, "Inconsistent parameter formatting");
        }

        // Check for missing spaces around operators in method body
        if (HasMissingOperatorSpacing(methodDeclaration))
        {
            ReportFormattingIssue(context, methodDeclaration, "Missing spaces around operators");
        }
    }

    private static void AnalyzePropertyDeclaration(SyntaxNodeAnalysisContext context)
    {
        var propertyDeclaration = (PropertyDeclarationSyntax)context.Node;

        // Check for inconsistent accessor formatting
        if (HasInconsistentAccessorFormatting(propertyDeclaration))
        {
            ReportFormattingIssue(context, propertyDeclaration, "Inconsistent property accessor formatting");
        }
    }

    private static void AnalyzeVariableDeclaration(SyntaxNodeAnalysisContext context)
    {
        var variableDeclaration = (VariableDeclarationSyntax)context.Node;

        // Check if this should be exempted from formatting checks
        if (IsExemptFromCodeFormattingCheck(variableDeclaration, context))
        {
            return;
        }

        // Check for inconsistent variable initialization formatting
        if (HasInconsistentVariableFormatting(variableDeclaration))
        {
            ReportFormattingIssue(context, variableDeclaration, "Inconsistent variable declaration formatting");
        }
    }

    private static void ReportFormattingIssue(SyntaxNodeAnalysisContext context, SyntaxNode node, string issueDescription)
    {
        var diagnostic = Diagnostic.Create(
            Rule,
            node.GetLocation(),
            issueDescription);

        context.ReportDiagnostic(diagnostic);
    }

    // Formatting detection methods
    private static bool HasInconsistentBraces(ClassDeclarationSyntax classDeclaration)
    {
        // Simple check: if opening brace is not on the same line or next line consistently
        var openBrace = classDeclaration.OpenBraceToken;
        var identifier = classDeclaration.Identifier;

        if (!openBrace.IsKind(SyntaxKind.None))
        {
            var identifierLine = identifier.GetLocation().GetLineSpan().StartLinePosition.Line;
            var braceLine = openBrace.GetLocation().GetLineSpan().StartLinePosition.Line;

            // Check if there are inconsistent line breaks (more sophisticated logic could be added)
            return Math.Abs(braceLine - identifierLine) > 2;
        }

        return false;
    }

    private static bool HasMissingBlankLinesBetweenMembers(ClassDeclarationSyntax classDeclaration)
    {
        var members = classDeclaration.Members;

        for (var i = 1; i < members.Count; i++)
        {
            var previousMember = members[i - 1];
            var currentMember = members[i];

            var previousLine = previousMember.GetLocation().GetLineSpan().EndLinePosition.Line;
            var currentLine = currentMember.GetLocation().GetLineSpan().StartLinePosition.Line;

            // If members are on consecutive lines, might need formatting
            if (currentLine - previousLine == 1)
            {
                // Check if both members are substantial (not just fields)
                if (IsSubstantialMember(previousMember) && IsSubstantialMember(currentMember))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private static bool HasInconsistentParameterFormatting(MethodDeclarationSyntax methodDeclaration)
    {
        var parameterList = methodDeclaration.ParameterList;
        if (parameterList?.Parameters.Count > 2)
        {
            // Check if parameters are formatted consistently (all on one line vs. each on separate line)
            var firstParam = parameterList.Parameters[0];
            var lastParam = parameterList.Parameters.Last();

            var firstLine = firstParam.GetLocation().GetLineSpan().StartLinePosition.Line;
            var lastLine = lastParam.GetLocation().GetLineSpan().StartLinePosition.Line;

            // If we have many parameters spanning multiple lines inconsistently
            return lastLine - firstLine > 0 && parameterList.Parameters.Count > 3;
        }

        return false;
    }

    private static bool HasMissingOperatorSpacing(MethodDeclarationSyntax methodDeclaration)
    {
        // This is a simplified check - in practice, you'd want more sophisticated analysis
        var methodText = methodDeclaration.ToString();

        // Look for common spacing issues (simplified patterns)
        return methodText.Contains("=") && (methodText.Contains(" =") || methodText.Contains("= ")) &&
               (methodText.Contains("if(") || methodText.Contains("for(") || methodText.Contains("while("));
    }

    private static bool HasInconsistentAccessorFormatting(PropertyDeclarationSyntax propertyDeclaration)
    {
        var accessorList = propertyDeclaration.AccessorList;
        if (accessorList?.Accessors.Count >= 2)
        {
            var getAccessor = accessorList.Accessors.FirstOrDefault(a => a.Keyword.IsKind(SyntaxKind.GetKeyword));
            var setAccessor = accessorList.Accessors.FirstOrDefault(a => a.Keyword.IsKind(SyntaxKind.SetKeyword));

            if (getAccessor != null && setAccessor != null)
            {
                // Check if they're formatted differently (one has body, other doesn't, etc.)
                var getHasBody = getAccessor.Body != null;
                var setHasBody = setAccessor.Body != null;

                // If one has a body and the other doesn't, might indicate formatting inconsistency
                return getHasBody != setHasBody && (getAccessor.ExpressionBody != null || setAccessor.ExpressionBody != null);
            }
        }

        return false;
    }

    private static bool HasInconsistentVariableFormatting(VariableDeclarationSyntax variableDeclaration)
    {
        foreach (var variable in variableDeclaration.Variables)
        {
            if (variable.Initializer is not { } initializer)
            {
                continue;
            }

            // Ignore complex initialisers (multi-line, object/dictionary/await projections, fluent pipelines, etc.)
            var valueText = initializer.Value.ToString();
            if (valueText.Contains("\n") ||
                valueText.Contains("{") ||
                valueText.Contains("}") ||
                valueText.Contains("await") ||
                valueText.Contains("(") ||
                valueText.Contains(".") ||
                valueText.Contains("new "))
            {
                continue;
            }

            var equalsToken = initializer.EqualsToken;
            var previousToken = equalsToken.GetPreviousToken();
            var nextToken = equalsToken.GetNextToken();

            var hasSpaceBefore = equalsToken.LeadingTrivia.Any(static t => t.IsKind(SyntaxKind.WhitespaceTrivia)) ||
                                 previousToken.TrailingTrivia.Any(static t => t.IsKind(SyntaxKind.WhitespaceTrivia));

            var hasSpaceAfter = equalsToken.TrailingTrivia.Any(static t => t.IsKind(SyntaxKind.WhitespaceTrivia)) ||
                                nextToken.LeadingTrivia.Any(static t => t.IsKind(SyntaxKind.WhitespaceTrivia));

            if (!hasSpaceBefore || !hasSpaceAfter)
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsSubstantialMember(MemberDeclarationSyntax member) =>
        // Consider methods, properties, classes as substantial (not just fields)
        member is MethodDeclarationSyntax or
               PropertyDeclarationSyntax or
               ClassDeclarationSyntax or
               ConstructorDeclarationSyntax;

    #region False-Positive Mitigation

    /// <summary>
    /// Central method to check if a variable declaration should be exempted from code formatting checks.
    /// </summary>
    private static bool IsExemptFromCodeFormattingCheck(VariableDeclarationSyntax variableDeclaration, SyntaxNodeAnalysisContext context)
    {
        return IsLinqProjection(variableDeclaration) ||
               IsInDebugGuardClause(variableDeclaration) ||
               IsDictionaryInitialization(variableDeclaration) ||
               IsAwaitedRepositoryCall(variableDeclaration) ||
               IsProjectionToDto(variableDeclaration) ||
               IsGroupByToDictionaryPipeline(variableDeclaration) ||
               IsFluentResultPipeline(variableDeclaration) ||
               IsSpecificationBuilderAssignment(variableDeclaration) ||
               IsDictionaryMaterialization(variableDeclaration);
    }

    /// <summary>
    /// Story 1.1: Correctly Handle LINQ Projections
    /// </summary>
    private static bool IsLinqProjection(VariableDeclarationSyntax variableDeclaration)
    {
        var initializer = variableDeclaration.Variables.FirstOrDefault()?.Initializer?.Value;
        if (initializer == null)
        {
            return false;
        }

        // Check if the initializer contains LINQ methods
        var initializerText = initializer.ToString();
        return initializerText.Contains(".Select(") ||
               initializerText.Contains(".Where(") ||
               initializerText.Contains(".ToList()") ||
               initializerText.Contains(".ToArray()");
    }

    /// <summary>
    /// Story 1.2: Correctly Handle Guard Clause Mock Data Assignments
    /// </summary>
    private static bool IsInDebugGuardClause(VariableDeclarationSyntax variableDeclaration)
    {
        // Check if the variable declaration is within a #if DEBUG block
        var trivia = variableDeclaration.GetLeadingTrivia();
        foreach (var trivium in trivia)
        {
            if (trivium.IsKind(SyntaxKind.IfDirectiveTrivia))
            {
                var directive = trivium.ToString();
                if (directive.Contains("#if DEBUG"))
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Story 1.3: Correctly Handle Dictionary Initializations
    /// </summary>
    private static bool IsDictionaryInitialization(VariableDeclarationSyntax variableDeclaration)
    {
        var type = variableDeclaration.Type.ToString();
        var initializer = variableDeclaration.Variables.FirstOrDefault()?.Initializer?.Value;
        
        if (initializer == null)
        {
            return false;
        }

        // Check if it's a dictionary initialization
        return type.Contains("Dictionary") && 
               (initializer.ToString().Contains("{") && initializer.ToString().Contains("}"));
    }

    /// <summary>
    /// Story 1.4: Correctly Handle Awaited Repository Calls
    /// </summary>
    private static bool IsAwaitedRepositoryCall(VariableDeclarationSyntax variableDeclaration)
    {
        var initializer = variableDeclaration.Variables.FirstOrDefault()?.Initializer?.Value;
        if (initializer == null)
        {
            return false;
        }

        var initializerText = initializer.ToString();
        return initializerText.Contains("await") && 
               (initializerText.Contains("Repository") || 
                initializerText.Contains("GetByIdAsync") ||
                initializerText.Contains("GetAllAsync") ||
                initializerText.Contains("SaveAsync"));
    }

    /// <summary>
    /// Story 1.5: Correctly Handle Projections to DTOs
    /// </summary>
    private static bool IsProjectionToDto(VariableDeclarationSyntax variableDeclaration)
    {
        var type = variableDeclaration.Type.ToString();
        var initializer = variableDeclaration.Variables.FirstOrDefault()?.Initializer?.Value;
        
        if (initializer == null)
        {
            return false;
        }

        // Check if it's a projection to a DTO
        return (type.Contains("Dto") || type.Contains("DTO")) && 
               initializer.ToString().Contains(".Select(");
    }

    /// <summary>
    /// Story 1.6: Correctly Handle GroupBy/ToDictionary Pipelines
    /// </summary>
    private static bool IsGroupByToDictionaryPipeline(VariableDeclarationSyntax variableDeclaration)
    {
        var initializer = variableDeclaration.Variables.FirstOrDefault()?.Initializer?.Value;
        if (initializer == null)
        {
            return false;
        }

        var initializerText = initializer.ToString();
        return initializerText.Contains(".GroupBy(") && 
               initializerText.Contains(".ToDictionary(");
    }

    /// <summary>
    /// Story 1.7: Correctly Handle Fluent Result Pipelines
    /// </summary>
    private static bool IsFluentResultPipeline(VariableDeclarationSyntax variableDeclaration)
    {
        var type = variableDeclaration.Type.ToString();
        var initializer = variableDeclaration.Variables.FirstOrDefault()?.Initializer?.Value;
        
        if (initializer == null)
        {
            return false;
        }

        var initializerText = initializer.ToString();
        return type.Contains("Result") && 
               (initializerText.Contains("await") && 
                (initializerText.Contains(".Map(") || initializerText.Contains(".Bind(")));
    }

    /// <summary>
    /// Story 1.8: Correctly Handle Specification Builder Assignments
    /// </summary>
    private static bool IsSpecificationBuilderAssignment(VariableDeclarationSyntax variableDeclaration)
    {
        var type = variableDeclaration.Type.ToString();
        var initializer = variableDeclaration.Variables.FirstOrDefault()?.Initializer?.Value;
        
        if (initializer == null)
        {
            return false;
        }

        var initializerText = initializer.ToString();
        return type.Contains("Specification") && 
               initializerText.Contains("new Specification<");
    }

    /// <summary>
    /// Story 1.9: Correctly Handle Dictionary Materialization from Collections
    /// </summary>
    private static bool IsDictionaryMaterialization(VariableDeclarationSyntax variableDeclaration)
    {
        var type = variableDeclaration.Type.ToString();
        var initializer = variableDeclaration.Variables.FirstOrDefault()?.Initializer?.Value;
        
        if (initializer == null)
        {
            return false;
        }

        var initializerText = initializer.ToString();
        return type.Contains("Dictionary") && 
               initializerText.Contains(".ToDictionary(");
    }

    #endregion
}
