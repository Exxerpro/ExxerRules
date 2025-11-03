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
    /// <summary>
    /// Gets the localized title displayed when a formatting issue is discovered.
    /// </summary>
    private static readonly LocalizableString Title = "Code formatting inconsistency detected";

    /// <summary>
    /// Gets the localized message format used to describe the detected formatting issue.
    /// </summary>
    private static readonly LocalizableString MessageFormat = "Formatting issue detected: {0}. Consider running 'dotnet format' to fix automatically.";

    /// <summary>
    /// Gets the diagnostic description explaining why the rule exists.
    /// </summary>
    private static readonly LocalizableString Description = "Detects common code formatting issues that can be automatically fixed with 'dotnet format' command.";

    /// <summary>
    /// The diagnostic descriptor emitted when formatting inconsistencies are found.
    /// </summary>
    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticIds.CodeFormattingIssue,
        Title,
        MessageFormat,
        DiagnosticCategories.CodeQuality,
        DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: Description);

    /// <summary>
    /// Gets the diagnostic descriptors supported by this analyzer.
    /// </summary>
    /// <value>An immutable array containing the formatting issue rule.</value>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    /// <summary>
    /// Registers syntax node actions that evaluate classes, methods, properties, and variables for formatting issues.
    /// </summary>
    /// <param name="context">The Roslyn analysis context used to register actions.</param>
    /// <remarks>
    /// Generated code is excluded and concurrent execution is enabled before attaching formatting checks.
    /// </remarks>
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

    /// <summary>
    /// Inspects class declarations for brace-placement consistency and reports diagnostics when deviations are found.
    /// </summary>
    /// <param name="context">The syntax analysis context supplying the class declaration.</param>
    private static void AnalyzeClassDeclaration(SyntaxNodeAnalysisContext context)
    {
        var classDeclaration = (ClassDeclarationSyntax)context.Node;

        // Check for inconsistent brace placement
        if (HasInconsistentBraces(classDeclaration))
        {
            ReportFormattingIssue(context, classDeclaration, "Inconsistent brace placement");
        }

    }

    /// <summary>
    /// Placeholder hook for method-level formatting analysis; currently does not emit diagnostics to avoid false positives.
    /// </summary>
    /// <param name="context">The syntax analysis context for method declarations.</param>
    private static void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context)
    {
        // Method-level formatting checks require a deeper understanding of style preferences.
        // To avoid false positives we intentionally skip analysing method bodies here.
    }

    /// <summary>
    /// Placeholder hook for property formatting analysis; intentionally left empty to reduce noise.
    /// </summary>
    /// <param name="context">The syntax analysis context for property declarations.</param>
    private static void AnalyzePropertyDeclaration(SyntaxNodeAnalysisContext context)
    {
        // Property accessor formatting is highly style-dependent; skip reporting to avoid noise.
    }

    /// <summary>
    /// Evaluates variable declarations for formatting irregularities such as inconsistent alignment or initialization spacing.
    /// </summary>
    /// <param name="context">The syntax analysis context supplying the variable declaration.</param>
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

    /// <summary>
    /// Emits a diagnostic describing the specified formatting issue.
    /// </summary>
    /// <param name="context">The syntax analysis context used to report diagnostics.</param>
    /// <param name="node">The syntax node that exhibited the formatting problem.</param>
    /// <param name="issueDescription">A human-readable description of the issue.</param>
    private static void ReportFormattingIssue(SyntaxNodeAnalysisContext context, SyntaxNode node, string issueDescription)
    {
        var diagnostic = Diagnostic.Create(
            Rule,
            node.GetLocation(),
            issueDescription);

        context.ReportDiagnostic(diagnostic);
    }

    // Formatting detection methods
    /// <summary>
    /// Determines whether the class declaration exhibits inconsistent brace placement relative to its identifier.
    /// </summary>
    /// <param name="classDeclaration">The class declaration to inspect.</param>
    /// <returns><c>true</c> when brace placement deviates from expected spacing; otherwise, <c>false</c>.</returns>
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

    /// <summary>
    /// Determines whether consecutive class members are missing the expected blank line separation.
    /// </summary>
    /// <param name="classDeclaration">The class declaration whose members are inspected.</param>
    /// <returns><c>true</c> when substantial members appear without spacing; otherwise, <c>false</c>.</returns>
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

    /// <summary>
    /// Determines whether a method's parameter list mixes single-line and multi-line formatting inconsistently.
    /// </summary>
    /// <param name="methodDeclaration">The method declaration to analyze.</param>
    /// <returns><c>true</c> when parameter formatting appears inconsistent; otherwise, <c>false</c>.</returns>
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

    /// <summary>
    /// Determines whether assignment expressions inside the method lack spacing around the equals sign.
    /// </summary>
    /// <param name="methodDeclaration">The method declaration to inspect.</param>
    /// <returns><c>true</c> when missing spacing is detected; otherwise, <c>false</c>.</returns>
    private static bool HasMissingOperatorSpacing(MethodDeclarationSyntax methodDeclaration)
    {
        foreach (var assignment in methodDeclaration.DescendantNodes().OfType<AssignmentExpressionSyntax>())
        {
            if (assignment.OperatorToken.IsKind(SyntaxKind.EqualsToken) &&
                !HasProperSpacingAroundEquals(assignment.OperatorToken))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Determines whether property accessors are formatted inconsistently (e.g., mixed single- and multi-line styles).
    /// </summary>
    /// <param name="propertyDeclaration">The property declaration to analyze.</param>
    /// <returns><c>true</c> when accessor formatting is inconsistent; otherwise, <c>false</c>.</returns>
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

    /// <summary>
    /// Determines whether variable declarations show inconsistent spacing or alignment.
    /// </summary>
    /// <param name="variableDeclaration">The variable declaration to inspect.</param>
    /// <returns><c>true</c> when formatting issues are detected; otherwise, <c>false</c>.</returns>
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

            if (!HasProperSpacingAroundEquals(initializer.EqualsToken))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Determines whether the equals token has the expected whitespace on both sides.
    /// </summary>
    /// <param name="equalsToken">The equals token encountered within an assignment.</param>
    /// <returns><c>true</c> when spacing is correct; otherwise, <c>false</c>.</returns>
    private static bool HasProperSpacingAroundEquals(SyntaxToken equalsToken)
    {
        var previousToken = equalsToken.GetPreviousToken();
        var nextToken = equalsToken.GetNextToken();

        if (!previousToken.IsKind(SyntaxKind.None) &&
            TokensShareLine(previousToken, equalsToken) &&
            !HasTrailingWhitespace(previousToken) &&
            !HasLeadingWhitespace(equalsToken))
        {
            return false;
        }

        if (!nextToken.IsKind(SyntaxKind.None) &&
            TokensShareLine(equalsToken, nextToken) &&
            !HasTrailingWhitespace(equalsToken) &&
            !HasLeadingWhitespace(nextToken))
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Determines whether the specified token has leading whitespace trivia.
    /// </summary>
    /// <param name="token">The token to inspect.</param>
    /// <returns><c>true</c> when leading whitespace exists; otherwise, <c>false</c>.</returns>
    private static bool HasLeadingWhitespace(SyntaxToken token) =>
        token.LeadingTrivia.Any(static t => t.IsKind(SyntaxKind.WhitespaceTrivia));

    /// <summary>
    /// Determines whether the specified token has trailing whitespace trivia.
    /// </summary>
    /// <param name="token">The token to inspect.</param>
    /// <returns><c>true</c> when trailing whitespace exists; otherwise, <c>false</c>.</returns>
    private static bool HasTrailingWhitespace(SyntaxToken token) =>
        token.TrailingTrivia.Any(static t => t.IsKind(SyntaxKind.WhitespaceTrivia));

    /// <summary>
    /// Determines whether two tokens appear on the same source line.
    /// </summary>
    /// <param name="left">The left token.</param>
    /// <param name="right">The right token.</param>
    /// <returns><c>true</c> when both tokens share a line; otherwise, <c>false</c>.</returns>
    private static bool TokensShareLine(SyntaxToken left, SyntaxToken right)
    {
        var leftSpan = left.GetLocation().GetLineSpan();
        var rightSpan = right.GetLocation().GetLineSpan();
        return leftSpan.EndLinePosition.Line == rightSpan.StartLinePosition.Line;
    }

    /// <summary>
    /// Determines whether the member represents a substantial declaration that should be separated by blank lines.
    /// </summary>
    /// <param name="member">The member to evaluate.</param>
    /// <returns><c>true</c> when the member is a method, property, class, or constructor; otherwise, <c>false</c>.</returns>
    private static bool IsSubstantialMember(MemberDeclarationSyntax member) =>
        member is MethodDeclarationSyntax or
               PropertyDeclarationSyntax or
               ClassDeclarationSyntax or
               ConstructorDeclarationSyntax;

    #region False-Positive Mitigation

    /// <summary>
    /// Central method to determine whether a variable declaration should be exempted from formatting diagnostics.
    /// </summary>
    /// <param name="variableDeclaration">The variable declaration candidate.</param>
    /// <param name="context">The analysis context providing semantic information.</param>
    /// <returns><c>true</c> when any exemption scenario matches; otherwise, <c>false</c>.</returns>
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
    /// Story 1.1: Correctly Handle LINQ Projections.
    /// </summary>
    /// <param name="variableDeclaration">The variable declaration to inspect for LINQ projection syntax.</param>
    /// <returns><c>true</c> when the initializer uses common LINQ projection operators; otherwise, <c>false</c>.</returns>
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
    /// Story 1.2: Correctly Handle Guard Clause Mock Data Assignments.
    /// </summary>
    /// <param name="variableDeclaration">The variable declaration to analyze.</param>
    /// <returns><c>true</c> when the declaration resides inside a <c>#if DEBUG</c> guard; otherwise, <c>false</c>.</returns>
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
    /// Story 1.3: Correctly Handle Dictionary Initializations.
    /// </summary>
    /// <param name="variableDeclaration">The variable declaration being inspected.</param>
    /// <returns><c>true</c> when the initializer resembles dictionary literals that should be exempt; otherwise, <c>false</c>.</returns>
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
    /// Story 1.4: Correctly Handle Awaited Repository Calls.
    /// </summary>
    /// <param name="variableDeclaration">The variable declaration to analyse.</param>
    /// <returns><c>true</c> when the initializer awaits repository calls that are exempt from formatting checks; otherwise, <c>false</c>.</returns>
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
    /// Story 1.5: Correctly Handle Projections to DTOs.
    /// </summary>
    /// <param name="variableDeclaration">The variable declaration to inspect.</param>
    /// <returns><c>true</c> when the initializer appears to project results into DTO types; otherwise, <c>false</c>.</returns>
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
    /// Story 1.6: Correctly Handle GroupBy/ToDictionary Pipelines.
    /// </summary>
    /// <param name="variableDeclaration">The variable declaration under inspection.</param>
    /// <returns><c>true</c> when the initializer chains <c>GroupBy</c> and <c>ToDictionary</c>; otherwise, <c>false</c>.</returns>
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
    /// Story 1.7: Correctly Handle Fluent Result Pipelines.
    /// </summary>
    /// <param name="variableDeclaration">The variable declaration being analyzed.</param>
    /// <returns><c>true</c> when the initializer participates in fluent result pipelines; otherwise, <c>false</c>.</returns>
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
    /// Story 1.8: Correctly Handle Specification Builder Assignments.
    /// </summary>
    /// <param name="variableDeclaration">The variable declaration to inspect.</param>
    /// <returns><c>true</c> when the initializer constructs specification builders exempt from formatting; otherwise, <c>false</c>.</returns>
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
    /// Story 1.9: Correctly Handle Dictionary Materialization from Collections.
    /// </summary>
    /// <param name="variableDeclaration">The variable declaration to analyze.</param>
    /// <returns><c>true</c> when the initializer materializes a dictionary via LINQ; otherwise, <c>false</c>.</returns>
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
