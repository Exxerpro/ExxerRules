using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using IndFusion.Analyzers.Common;
using IndFusion.Analyzers.Operations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace IndFusion.Analyzers.CodeQuality;

/// <summary>
/// Analyzer that enforces use of named constants instead of magic numbers and strings.
/// Supports the "avoid globals and hardcoding" principle by promoting named constants.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class AvoidMagicNumbersAndStringsAnalyzer : DiagnosticAnalyzer
{
    /// <summary>
    /// Gets the localized title displayed when a magic literal is detected.
    /// </summary>
    private static readonly LocalizableString Title = "Avoid magic numbers and strings";

    /// <summary>
    /// Gets the localized message format describing the magic literal that triggered the diagnostic.
    /// </summary>
    private static readonly LocalizableString MessageFormat = "Magic {0} '{1}' should be replaced with a named constant";

    /// <summary>
    /// Gets the diagnostic description that explains why magic literals should be replaced.
    /// </summary>
    private static readonly LocalizableString Description = "Magic numbers and strings should be replaced with named constants to improve code readability, maintainability, and reduce the risk of errors. Follow the principle of avoiding globals and hardcoding.";

    /// <summary>
    /// The diagnostic descriptor emitted when a magic number or string is encountered.
    /// </summary>
    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticIds.AvoidMagicNumbersAndStrings,
        Title,
        MessageFormat,
        DiagnosticCategories.CodeQuality,
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: Description);

    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    /// <inheritdoc/>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

		context.RegisterSyntaxNodeAction(AnalyzeLiteralExpression, SyntaxKind.NumericLiteralExpression);
		context.RegisterSyntaxNodeAction(AnalyzeLiteralExpression, SyntaxKind.StringLiteralExpression);
    }

    /// <summary>
    /// Evaluates literal expressions and reports diagnostics for magic numbers or strings that are not exempt.
    /// </summary>
    /// <param name="context">The syntax analysis context supplying the literal expression.</param>
    private static void AnalyzeLiteralExpression(SyntaxNodeAnalysisContext context)
    {
        var literalExpression = (LiteralExpressionSyntax)context.Node;

		// Skip if this is a constant-like declaration
		if (IsInConstantDeclaration(literalExpression))
        {
            return;
        }

		// Skip if assigning to static readonly field inside a static constructor
		if (IsAssignmentToStaticReadonlyInStaticCtor(literalExpression, context))
		{
			return;
		}

        // Skip if this is an attribute argument
        if (IsAttributeArgument(literalExpression))
        {
            return;
        }

        // Skip if this is in a switch expression or case label
        if (IsInSwitchOrCase(literalExpression))
        {
            return;
        }

        // Check for various exemption scenarios
        if (IsExemptFromMagicNumberCheck(literalExpression, context))
        {
            return;
        }

        // Analyze based on literal type
        if (literalExpression.Token.IsKind(SyntaxKind.NumericLiteralToken))
        {
            AnalyzeNumericLiteral(context, literalExpression);
        }
        else if (literalExpression.Token.IsKind(SyntaxKind.StringLiteralToken))
        {
            AnalyzeStringLiteral(context, literalExpression);
        }
    }

    /// <summary>
    /// Inspects numeric literals and reports diagnostics when they qualify as magic numbers.
    /// </summary>
    /// <param name="context">The syntax analysis context used to report diagnostics.</param>
    /// <param name="literal">The numeric literal expression being analyzed.</param>
    private static void AnalyzeNumericLiteral(SyntaxNodeAnalysisContext context, LiteralExpressionSyntax literal)
    {
        var value = literal.Token.ValueText;

        // Skip common numbers that are typically not considered magic
        if (IsCommonNumber(value))
        {
            return;
        }

        // Report diagnostic for magic number
        var diagnostic = Diagnostic.Create(
            Rule,
            literal.GetLocation(),
            "number",
            value);
        context.ReportDiagnostic(diagnostic);
    }

    /// <summary>
    /// Inspects string literals and reports diagnostics when they qualify as magic strings.
    /// </summary>
    /// <param name="context">The syntax analysis context used to report diagnostics.</param>
    /// <param name="literal">The string literal expression being analyzed.</param>
    private static void AnalyzeStringLiteral(SyntaxNodeAnalysisContext context, LiteralExpressionSyntax literal)
    {
        var value = literal.Token.ValueText;

        // Skip empty strings and very short strings (often used for formatting)
        if (string.IsNullOrEmpty(value) || value.Length <= 1)
        {
            return;
        }

        // Skip strings that look like format strings or templates
        if (IsFormatString(value))
        {
            return;
        }

        // Skip very common strings
        if (IsCommonString(value))
        {
            return;
        }

        // Report diagnostic for magic string
        var diagnostic = Diagnostic.Create(
            Rule,
            literal.GetLocation(),
            "string",
            value);
        context.ReportDiagnostic(diagnostic);
    }

	/// <summary>
	/// Determines whether the literal resides within a constant declaration and should therefore be ignored.
	/// </summary>
	/// <param name="node">The syntax node representing the literal expression.</param>
	/// <returns><c>true</c> when the literal is part of a constant declaration; otherwise, <c>false</c>.</returns>
	private static bool IsInConstantDeclaration(SyntaxNode node)
    {
        // Check if we're in a const field declaration
        var fieldDeclaration = node.FirstAncestorOrSelf<FieldDeclarationSyntax>();
        if (fieldDeclaration != null && fieldDeclaration.Modifiers.Any(SyntaxKind.ConstKeyword))
        {
            return true;
        }

        // Check if we're in a static readonly field declaration
        if (fieldDeclaration != null &&
            fieldDeclaration.Modifiers.Any(SyntaxKind.StaticKeyword) &&
            fieldDeclaration.Modifiers.Any(SyntaxKind.ReadOnlyKeyword))
        {
            return true;
        }

        // Check if we're in a local const declaration
        var localDeclaration = node.FirstAncestorOrSelf<LocalDeclarationStatementSyntax>();
        if (localDeclaration != null && localDeclaration.Modifiers.Any(SyntaxKind.ConstKeyword))
        {
            return true;
        }

        // Check if we're in an array initializer (often used for initialization)
        if (node.FirstAncestorOrSelf<InitializerExpressionSyntax>() != null)
        {
            return true;
        }

		return false;
    }

	/// <summary>
	/// Determines whether the literal initializes a static readonly field within a static constructor.
	/// </summary>
	/// <param name="literal">The literal expression under inspection.</param>
	/// <param name="context">The syntax analysis context providing semantic information.</param>
	/// <returns><c>true</c> when the literal is part of a static readonly initialization in a static constructor; otherwise, <c>false</c>.</returns>
	private static bool IsAssignmentToStaticReadonlyInStaticCtor(LiteralExpressionSyntax literal, SyntaxNodeAnalysisContext context)
	{
		// Look for an assignment expression ancestor
		var assignment = literal.FirstAncestorOrSelf<AssignmentExpressionSyntax>();
		if (assignment == null)
		{
			return false;
		}

		// Ensure we are inside a static constructor
		var ctor = assignment.FirstAncestorOrSelf<ConstructorDeclarationSyntax>();
		if (ctor == null || !ctor.Modifiers.Any(SyntaxKind.StaticKeyword))
		{
			return false;
		}

		// Resolve the left-hand side symbol
		var leftSymbol = context.SemanticModel.GetSymbolInfo(assignment.Left).Symbol as IFieldSymbol;
		if (leftSymbol == null)
		{
			return false;
		}

		// Only skip when assigning to static readonly fields
		return leftSymbol.IsStatic && leftSymbol.IsReadOnly;
	}

	/// <summary>
	/// Determines whether the literal is used as an attribute argument.
	/// </summary>
	/// <param name="node">The syntax node to inspect.</param>
	/// <returns><c>true</c> when the literal resides within an attribute argument; otherwise, <c>false</c>.</returns>
	private static bool IsAttributeArgument(SyntaxNode node) => node.FirstAncestorOrSelf<AttributeArgumentSyntax>() != null ||
               node.FirstAncestorOrSelf<AttributeSyntax>() != null;

	/// <summary>
	/// Determines whether the literal appears in a switch expression arm or case label.
	/// </summary>
	/// <param name="node">The syntax node to inspect.</param>
	/// <returns><c>true</c> when the literal is part of a switch expression or case label; otherwise, <c>false</c>.</returns>
	private static bool IsInSwitchOrCase(SyntaxNode node) => node.FirstAncestorOrSelf<SwitchExpressionArmSyntax>() != null ||
               node.FirstAncestorOrSelf<CaseSwitchLabelSyntax>() != null ||
               node.FirstAncestorOrSelf<SwitchExpressionSyntax>() != null;

	/// <summary>
	/// Determines whether the numeric literal represents a commonly accepted value that should not trigger diagnostics.
	/// </summary>
	/// <param name="value">The numeric literal value as text.</param>
	/// <returns><c>true</c> when the number is considered common and safe; otherwise, <c>false</c>.</returns>
	private static bool IsCommonNumber(string value)
    {
		// Commonly acceptable literals (configurable in future):
		// - Small integers often used as counters or toggles
		// - Powers of two typically used for sizes/flags
		// - Common network ports
		var commonNumbers = new[]
		{
			"0", "1", "-1", "4",
			"8", "16", "32", "64", "128", "256",
			"512", "1024", "2048", "4096",
			"80", "443"
		};

		return commonNumbers.Contains(value);
    }

    /// <summary>
    /// Determines whether the supplied string literal resembles a format template and should be exempted.
    /// </summary>
    /// <param name="value">The string literal value.</param>
    /// <returns><c>true</c> when the string appears to be a format template; otherwise, <c>false</c>.</returns>
    private static bool IsFormatString(string value) =>
        // Check for format strings like "{0}", "{name}", etc.
        value.Contains('{') && value.Contains('}');

    /// <summary>
    /// Determines whether the string literal is a commonly accepted value that should be exempt from diagnostics.
    /// </summary>
    /// <param name="value">The string literal value.</param>
    /// <returns><c>true</c> when the string is deemed common; otherwise, <c>false</c>.</returns>
    private static bool IsCommonString(string value)
    {
        // Very common strings that might be acceptable
        var commonStrings = new[]
        {
            " ", "\n", "\r\n", "\t", ",", ".", ":", ";",
            "true", "false", "null", "undefined"
        };

        return commonStrings.Contains(value, System.StringComparer.OrdinalIgnoreCase);
    }

    //  False-Positive Mitigation Methods

	/// <summary>
	/// Determines whether the literal expression matches any exemption scenario and should not trigger diagnostics.
	/// </summary>
	/// <param name="literal">The literal expression under evaluation.</param>
	/// <param name="context">The syntax analysis context providing semantic information.</param>
	/// <returns><c>true</c> when an exemption rule applies; otherwise, <c>false</c>.</returns>
	private static bool IsExemptFromMagicNumberCheck(LiteralExpressionSyntax literal, SyntaxNodeAnalysisContext context)
    {
        // Story 1.1: Exempt Enum Member Values
        if (IsEnumMemberValue(literal))
        {
            return true;
        }

        // Story 1.2: Exempt Bit-Flag Enum Values
        if (IsBitFlagEnumValue(literal))
        {
            return true;
        }

        // Story 1.3: Exempt Domain Range Guards
        if (IsDomainRangeGuard(literal))
        {
            return true;
        }

        // Story 1.4: Exempt Business Rule Thresholds
        if (IsBusinessRuleThreshold(literal))
        {
            return true;
        }

        // Story 1.5: Exempt Exception Messages
        if (IsExceptionMessage(literal))
        {
            return true;
        }

        // Story 1.6: Exempt Result/Validation Messages
        if (IsResultValidationMessage(literal))
        {
            return true;
        }

        // Story 1.7: Exempt Regex and Pattern Literals
        if (IsRegexOrPatternLiteral(literal))
        {
            return true;
        }

        // Story 1.8: Exempt Culture and Locale Codes
        if (IsCultureOrLocaleCode(literal))
        {
            return true;
        }

        // Story 1.9: Exempt TimeSpan and DateTime Construction
        if (IsTimeSpanOrDateTimeConstruction(literal))
        {
            return true;
        }

        // Story 1.10: Exempt Logging Message Templates
        if (IsLoggingMessageTemplate(literal))
        {
            return true;
        }

        // Story 1.11: Exempt Logging Method Parameters
        if (IsLoggingMethodParameter(literal))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Story 1.1: Exempt Enum Member Values
    /// </summary>
    /// <summary>
    /// Story 1.1: Exempts enum member values from magic-literal diagnostics.
    /// </summary>
    /// <param name="literal">The literal expression to evaluate.</param>
    /// <returns><c>true</c> when the literal belongs to an enum member; otherwise, <c>false</c>.</returns>
    private static bool IsEnumMemberValue(LiteralExpressionSyntax literal)
    {
        // Check if we're in an enum member declaration
        var enumMemberDeclaration = literal.FirstAncestorOrSelf<EnumMemberDeclarationSyntax>();
        return enumMemberDeclaration != null;
    }

    /// <summary>
    /// Story 1.2: Exempt Bit-Flag Enum Values
    /// </summary>
    /// <summary>
    /// Story 1.2: Exempts bit-flag enum values (e.g., shift expressions) from diagnostics.
    /// </summary>
    /// <param name="literal">The literal expression to evaluate.</param>
    /// <returns><c>true</c> when the literal participates in a bit-flag enum definition; otherwise, <c>false</c>.</returns>
    private static bool IsBitFlagEnumValue(LiteralExpressionSyntax literal)
    {
        // Check if we're in a bit-shift expression within an enum member
        var enumMemberDeclaration = literal.FirstAncestorOrSelf<EnumMemberDeclarationSyntax>();
        if (enumMemberDeclaration == null)
        {
            return false;
        }

        // Check if the enum has [Flags] attribute
        var enumDeclaration = enumMemberDeclaration.FirstAncestorOrSelf<EnumDeclarationSyntax>();
        if (enumDeclaration == null)
        {
            return false;
        }

        // Check for [Flags] attribute
        foreach (var attributeList in enumDeclaration.AttributeLists)
        {
            foreach (var attribute in attributeList.Attributes)
            {
                if (attribute.Name.ToString() == "Flags" || attribute.Name.ToString().EndsWith(".Flags"))
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Story 1.3: Exempt Domain Range Guards
    /// </summary>
    /// <summary>
    /// Story 1.3: Exempts literals used in domain guard clauses (e.g., range checks).
    /// </summary>
    /// <param name="literal">The literal expression under evaluation.</param>
    /// <returns><c>true</c> when the literal participates in domain range validation; otherwise, <c>false</c>.</returns>
    private static bool IsDomainRangeGuard(LiteralExpressionSyntax literal)
    {
        // Check if we're in a comparison within a guard clause that throws ArgumentOutOfRangeException
        var binaryExpression = literal.FirstAncestorOrSelf<BinaryExpressionSyntax>();
        if (binaryExpression == null)
        {
            return false;
        }

        // Look for throw statement with ArgumentOutOfRangeException
        var throwStatement = binaryExpression.FirstAncestorOrSelf<ThrowStatementSyntax>();
        if (throwStatement == null)
        {
            // Check if we're in an if statement that has a throw statement in the same block
            var ifStatement = binaryExpression.FirstAncestorOrSelf<IfStatementSyntax>();
            if (ifStatement != null)
            {
                // Check if the if statement's body contains a throw statement
                if (ifStatement.Statement is BlockSyntax block)
                {
                    foreach (var statement in block.Statements)
                    {
                        if (statement is ThrowStatementSyntax blockThrow)
                        {
                            var blockObjectCreation = blockThrow.Expression as ObjectCreationExpressionSyntax;
                            if (blockObjectCreation != null)
                            {
                                var blockTypeName = blockObjectCreation.Type.ToString();
                                if (blockTypeName.Contains("ArgumentOutOfRangeException"))
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
                else if (ifStatement.Statement is ThrowStatementSyntax directThrow)
                {
                    var directObjectCreation = directThrow.Expression as ObjectCreationExpressionSyntax;
                    if (directObjectCreation != null)
                    {
                        var directTypeName = directObjectCreation.Type.ToString();
                        if (directTypeName.Contains("ArgumentOutOfRangeException"))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        // Check if the thrown exception is ArgumentOutOfRangeException
        var objectCreation = throwStatement.Expression as ObjectCreationExpressionSyntax;
        if (objectCreation == null)
        {
            return false;
        }

        var typeName = objectCreation.Type.ToString();
        return typeName.Contains("ArgumentOutOfRangeException");
    }

    /// <summary>
    /// Story 1.4: Exempt Business Rule Thresholds
    /// </summary>
    /// <summary>
    /// Story 1.4: Exempts literals representing business rule thresholds (e.g., length checks).
    /// </summary>
    /// <param name="literal">The literal expression to inspect.</param>
    /// <returns><c>true</c> when the literal appears in a business-rule guard; otherwise, <c>false</c>.</returns>
    private static bool IsBusinessRuleThreshold(LiteralExpressionSyntax literal)
    {
        // Check if we're in a comparison against Length or Count property
        var binaryExpression = literal.FirstAncestorOrSelf<BinaryExpressionSyntax>();
        if (binaryExpression == null)
        {
            return false;
        }

        // Check if the other side of the comparison is a property access
        var otherSide = binaryExpression.Left == literal ? binaryExpression.Right : binaryExpression.Left;
        if (otherSide is MemberAccessExpressionSyntax memberAccess)
        {
            var propertyName = memberAccess.Name.Identifier.Text;
            if (propertyName == "Length" || propertyName == "Count")
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Story 1.5: Exempt Exception Messages
    /// </summary>
    /// <summary>
    /// Story 1.5: Exempts string literals used as exception messages.
    /// </summary>
    /// <param name="literal">The string literal to evaluate.</param>
    /// <returns><c>true</c> when the literal feeds an exception constructor; otherwise, <c>false</c>.</returns>
    private static bool IsExceptionMessage(LiteralExpressionSyntax literal)
    {
        // Check if we're in an exception constructor
        var objectCreation = literal.FirstAncestorOrSelf<ObjectCreationExpressionSyntax>();
        if (objectCreation == null)
        {
            return false;
        }

        var typeName = objectCreation.Type.ToString();
        return typeName.Contains("Exception");
    }

    /// <summary>
    /// Story 1.6: Exempt Result/Validation Messages
    /// </summary>
    /// <summary>
    /// Story 1.6: Exempts literals used in result or validation messaging scenarios.
    /// </summary>
    /// <param name="literal">The literal expression under consideration.</param>
    /// <returns><c>true</c> when the literal participates in validation messaging; otherwise, <c>false</c>.</returns>
    private static bool IsResultValidationMessage(LiteralExpressionSyntax literal)
    {
        // Check if we're adding to a collection named 'errors' or 'warnings'
        var invocationExpression = literal.FirstAncestorOrSelf<InvocationExpressionSyntax>();
        if (invocationExpression != null)
        {
            var memberAccess = invocationExpression.Expression as MemberAccessExpressionSyntax;
            if (memberAccess != null)
            {
                var methodName = memberAccess.Name.Identifier.Text;
                if (methodName == "Add")
                {
                    var variableName = memberAccess.Expression.ToString();
                    if (variableName.Contains("errors") || variableName.Contains("warnings"))
                    {
                        return true;
                    }
                }
            }
        }

        // Check if we're calling Result.WithFailure
        if (invocationExpression != null)
        {
            var memberAccess = invocationExpression.Expression as MemberAccessExpressionSyntax;
            if (memberAccess != null)
            {
                var methodName = memberAccess.Name.Identifier.Text;
                if (methodName == "WithFailure")
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Story 1.7: Exempt Regex and Pattern Literals
    /// </summary>
    /// <summary>
    /// Story 1.7: Exempts literals representing regular expressions or pattern tokens.
    /// </summary>
    /// <param name="literal">The literal expression to inspect.</param>
    /// <returns><c>true</c> when the literal appears to be a regex or pattern literal; otherwise, <c>false</c>.</returns>
    private static bool IsRegexOrPatternLiteral(LiteralExpressionSyntax literal)
    {
        if (!literal.Token.IsKind(SyntaxKind.StringLiteralToken))
        {
            return false;
        }

        var value = literal.Token.ValueText;

        // Check if string contains regex metacharacters
        if (value.Contains('^') || value.Contains('$') || value.Contains('[') || 
            value.Contains(']') || value.Contains('(') || value.Contains(')') ||
            value.Contains('*') || value.Contains('+') || value.Contains('?') ||
            value.Contains('\\') || value.Contains('|'))
        {
            return true;
        }

        // Check if we're in a Regex constructor or method
        var invocationExpression = literal.FirstAncestorOrSelf<InvocationExpressionSyntax>();
        if (invocationExpression != null)
        {
            var memberAccess = invocationExpression.Expression as MemberAccessExpressionSyntax;
            if (memberAccess != null)
            {
                var typeName = memberAccess.Expression.ToString();
                if (typeName.Contains("Regex"))
                {
                    return true;
                }
            }
        }

        var objectCreation = literal.FirstAncestorOrSelf<ObjectCreationExpressionSyntax>();
        if (objectCreation != null)
        {
            var typeName = objectCreation.Type.ToString();
            if (typeName.Contains("Regex"))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Story 1.8: Exempt Culture and Locale Codes
    /// </summary>
    /// <summary>
    /// Story 1.8: Exempts literals that represent culture or locale codes.
    /// </summary>
    /// <param name="literal">The literal expression under consideration.</param>
    /// <returns><c>true</c> when the literal denotes a culture or locale code; otherwise, <c>false</c>.</returns>
    private static bool IsCultureOrLocaleCode(LiteralExpressionSyntax literal)
    {
        if (!literal.Token.IsKind(SyntaxKind.StringLiteralToken))
        {
            return false;
        }

        var value = literal.Token.ValueText;

        // Check if string matches culture code pattern (e.g., "en-US", "fr-FR")
        if (System.Text.RegularExpressions.Regex.IsMatch(value, @"^[a-z]{2}-[A-Z]{2}$"))
        {
            return true;
        }

        // Check if we're in a CultureInfo or RegionInfo constructor
        var objectCreation = literal.FirstAncestorOrSelf<ObjectCreationExpressionSyntax>();
        if (objectCreation != null)
        {
            var typeName = objectCreation.Type.ToString();
            if (typeName.Contains("CultureInfo") || typeName.Contains("RegionInfo"))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Story 1.9: Exempt TimeSpan and DateTime Construction
    /// </summary>
    /// <summary>
    /// Story 1.9: Exempts numeric literals used in <see cref="System.TimeSpan"/> or <see cref="System.DateTime"/> construction.
    /// </summary>
    /// <param name="literal">The numeric literal expression to inspect.</param>
    /// <returns><c>true</c> when the literal participates in TimeSpan or DateTime construction; otherwise, <c>false</c>.</returns>
    private static bool IsTimeSpanOrDateTimeConstruction(LiteralExpressionSyntax literal)
    {
        if (!literal.Token.IsKind(SyntaxKind.NumericLiteralToken))
        {
            return false;
        }

        // Check if we're in a TimeSpan or DateTime constructor or factory method
        var objectCreation = literal.FirstAncestorOrSelf<ObjectCreationExpressionSyntax>();
        if (objectCreation != null)
        {
            var typeName = objectCreation.Type.ToString();
            if (typeName.Contains("TimeSpan") || typeName.Contains("DateTime"))
            {
                return true;
            }
        }

        var invocationExpression = literal.FirstAncestorOrSelf<InvocationExpressionSyntax>();
        if (invocationExpression != null)
        {
            var memberAccess = invocationExpression.Expression as MemberAccessExpressionSyntax;
            if (memberAccess != null)
            {
                var typeName = memberAccess.Expression.ToString();
                if (typeName.Contains("TimeSpan") || typeName.Contains("DateTime"))
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Story 1.10: Exempt Logging Message Templates
    /// </summary>
    /// <summary>
    /// Story 1.10: Exempts string literals used as logging message templates.
    /// </summary>
    /// <param name="literal">The string literal under evaluation.</param>
    /// <returns><c>true</c> when the literal is the first argument to a logging call; otherwise, <c>false</c>.</returns>
    private static bool IsLoggingMessageTemplate(LiteralExpressionSyntax literal)
    {
        if (!literal.Token.IsKind(SyntaxKind.StringLiteralToken))
        {
            return false;
        }

        // Check if we're in an ILogger.Log method call
        var invocationExpression = literal.FirstAncestorOrSelf<InvocationExpressionSyntax>();
        if (invocationExpression != null)
        {
            var memberAccess = invocationExpression.Expression as MemberAccessExpressionSyntax;
            if (memberAccess != null)
            {
                var methodName = memberAccess.Name.Identifier.Text;
                if (methodName.StartsWith("Log"))
                {
                    // Check if this is the first argument (message template)
                    var argumentList = invocationExpression.ArgumentList;
                    if (argumentList != null && argumentList.Arguments.Count > 0)
                    {
                        var firstArgument = argumentList.Arguments[0];
                        if (firstArgument.Expression == literal)
                        {
                            return true;
                        }
                    }
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Story 1.11: Exempt Logging Method Parameters
    /// </summary>
    /// <summary>
    /// Story 1.11: Exempts literals used as structured logging parameters beyond the message template.
    /// </summary>
    /// <param name="literal">The literal expression to inspect.</param>
    /// <returns><c>true</c> when the literal appears among logging method parameters; otherwise, <c>false</c>.</returns>
    private static bool IsLoggingMethodParameter(LiteralExpressionSyntax literal)
    {
        // Check if we're in an ILogger.Log method call
        var invocationExpression = literal.FirstAncestorOrSelf<InvocationExpressionSyntax>();
        if (invocationExpression != null)
        {
            var memberAccess = invocationExpression.Expression as MemberAccessExpressionSyntax;
            if (memberAccess != null)
            {
                var methodName = memberAccess.Name.Identifier.Text;
                if (methodName.StartsWith("Log"))
                {
                    // Check if this literal is a parameter in the logging method call
                    var argumentList = invocationExpression.ArgumentList;
                    if (argumentList != null && argumentList.Arguments.Count > 1)
                    {
                        // Check if this literal is in any of the arguments (not the first one which is the message template)
                        foreach (var argument in argumentList.Arguments)
                        {
                            if (argument.Expression == literal)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
        }

        return false;
    }

     // 
}
