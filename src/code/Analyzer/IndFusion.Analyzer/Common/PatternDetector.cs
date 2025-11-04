using System.Text.RegularExpressions;
using IndFusion.Analyzer.Operations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IndFusion.Analyzer.Common;

/// <summary>
/// Utility class for detecting code patterns using functional approach with ExxerRules.Analyzers.Operations.
/// </summary>
public static class PatternDetector
{
    /// <summary>
    /// Error message emitted when the supplied method name argument is null or whitespace.
    /// </summary>
    private const string methodNameNullErrorMessage = "Method name cannot be null or empty";

    /// <summary>
    /// Error message emitted when the supplied regex pattern argument is null or whitespace.
    /// </summary>
    private const string patternNullErrorMessage = "Pattern cannot be null or empty";

    /// <summary>
    /// Error message template used when a regex pattern fails to compile.
    /// </summary>
    private const string invalidRegexErrorMessage = "Invalid regex pattern: {0}";

    /// <summary>
    /// Error message emitted when a method declaration argument is null.
    /// </summary>
    private const string methodNullErrorMessage = "Method cannot be null";

    /// <summary>
    /// Error message emitted when a semantic model argument is null.
    /// </summary>
    private const string semanticModelNullErrorMessage = "Semantic model cannot be null";

    /// <summary>
    /// Error message emitted when a class declaration argument is null.
    /// </summary>
    private const string classDeclarationNullErrorMessage = "Class declaration cannot be null";

    /// <summary>
    /// Suffix appended to parameter names to construct standardized null-error messages.
    /// </summary>
    private const string cannotBeNullSuffix = " cannot be null";

    /// <summary>
    /// Substring used to identify xUnit-related attributes.
    /// </summary>
    private const string xunitFramework = "Xunit";

    /// <summary>
    /// Attribute name used for xUnit fact methods.
    /// </summary>
    private const string factAttribute = "Fact";

    /// <summary>
    /// Attribute name used for xUnit theory methods.
    /// </summary>
    private const string theoryAttribute = "Theory";

    /// <summary>
    /// Substring used to identify NUnit-related attributes.
    /// </summary>
    private const string nunitFramework = "NUnit";

    /// <summary>
    /// Attribute name used for NUnit test methods.
    /// </summary>
    private const string testAttribute = "Test";

    /// <summary>
    /// Substring used to identify MSTest-related attributes.
    /// </summary>
    private const string msTestFramework = "Microsoft.VisualStudio.TestTools";

    /// <summary>
    /// Attribute name used for MSTest methods.
    /// </summary>
    private const string testMethodAttribute = "TestMethod";

    /// <summary>
    /// Fully qualified attribute name for xUnit fact methods.
    /// </summary>
    private const string xunitFactAttribute = "Xunit.Fact";

    /// <summary>
    /// Fully qualified attribute name for xUnit theory methods.
    /// </summary>
    private const string xunitTheoryAttribute = "Xunit.Theory";

    /// <summary>
    /// Fully qualified attribute name for NUnit test methods.
    /// </summary>
    private const string nunitTestAttribute = "NUnit.Framework.Test";

    /// <summary>
    /// Fully qualified attribute name for MSTest methods.
    /// </summary>
    private const string msTestMethodAttribute = "Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod";

    /// <summary>
    /// Class-name suffix that denotes a container of multiple tests.
    /// </summary>
    private const string testsSuffix = "Tests";

    /// <summary>
    /// Class-name suffix that denotes a single test case.
    /// </summary>
    private const string testSuffix = "Test";

    /// <summary>
    /// Class-name suffix that denotes a collection of specifications.
    /// </summary>
    private const string specsSuffix = "Specs";

    /// <summary>
    /// Class-name suffix that denotes a single specification.
    /// </summary>
    private const string specSuffix = "Spec";

    /// <summary>
    /// Attribute name used by MSTest to flag test classes.
    /// </summary>
    private const string testClassAttribute = "TestClass";

    /// <summary>
    /// Attribute name used by NUnit to flag test fixtures.
    /// </summary>
    private const string testFixtureAttribute = "TestFixture";

    /// <summary>
    /// Validates whether a method name conforms to the supplied regex naming pattern.
    /// </summary>
    /// <param name="methodName">The method name to validate.</param>
    /// <param name="pattern">The regex pattern to match against.</param>
    /// <returns>
    /// A successful result containing <c>true</c> when the name matches the pattern; otherwise, <c>false</c>.
    /// Returns a failed result with an explanatory message if the input arguments are invalid or the regex cannot be compiled.
    /// </returns>
    public static Result<bool> ValidateMethodNaming(string methodName, string pattern)
    {
        // EXXER200: Validate null parameters
        if (methodName is null)
        {
            return AnalysisResult.Failure<bool>(nameof(methodName) + cannotBeNullSuffix);
        }

        if (pattern is null)
        {
            return AnalysisResult.Failure<bool>(nameof(pattern) + cannotBeNullSuffix);
        }

        if (string.IsNullOrWhiteSpace(methodName))
        {
            return AnalysisResult.Failure<bool>(methodNameNullErrorMessage);
        }

        if (string.IsNullOrWhiteSpace(pattern))
        {
            return AnalysisResult.Failure<bool>(patternNullErrorMessage);
        }

        try
        {
            var regex = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var isMatch = regex.IsMatch(methodName);
            return AnalysisResult.Success(isMatch);
        }
        catch (ArgumentException ex)
        {
            return AnalysisResult.Failure<bool>(string.Format(invalidRegexErrorMessage, ex.Message));
        }
    }

    /// <summary>
    /// Analyzes a method declaration and captures any supported test attributes applied to it.
    /// </summary>
    /// <param name="method">The method declaration to analyze.</param>
    /// <param name="semanticModel">The semantic model for type resolution.</param>
    /// <returns>
    /// A successful result containing the discovered test attribute information, or a failed result with an explanatory message when the inputs are invalid.
    /// </returns>
    public static Result<TestAttributeInfo> DetectTestAttributes(MethodDeclarationSyntax method, SemanticModel semanticModel)
    {
        // EXXER200: Validate null parameters
        if (method is null)
        {
            return AnalysisResult.Failure<TestAttributeInfo>(methodNullErrorMessage);
        }

        if (semanticModel is null)
        {
            return AnalysisResult.Failure<TestAttributeInfo>(semanticModelNullErrorMessage);
        }

        var testAttributes = new[]
        {
            factAttribute, theoryAttribute, testAttribute, testMethodAttribute,
            xunitFactAttribute, xunitTheoryAttribute,
            nunitTestAttribute,
            msTestMethodAttribute
        };

        var foundAttributes = new List<string>();

        foreach (var attributeList in method.AttributeLists)
        {
            foreach (var attribute in attributeList.Attributes)
            {
                var attributeName = attribute.Name.ToString();

                var matchingAttribute = testAttributes.FirstOrDefault(ta =>
                    attributeName == ta ||
                    attributeName.EndsWith("." + ta.Split('.').Last()) ||
                    ta.EndsWith("." + attributeName));

                if (matchingAttribute != null)
                {
                    foundAttributes.Add(matchingAttribute);
                }

                // Use semantic model for more accurate detection
                var attributeSymbol = semanticModel.GetSymbolInfo(attribute).Symbol;
                if (attributeSymbol is IMethodSymbol constructor)
                {
                    var attributeType = constructor.ContainingType.ToDisplayString();
                    var semanticMatch = testAttributes.FirstOrDefault(ta => attributeType.Contains(ta.Split('.').Last()));
                    if (semanticMatch != null && !foundAttributes.Contains(semanticMatch))
                    {
                        foundAttributes.Add(semanticMatch);
                    }
                }
            }
        }

        var info = new TestAttributeInfo(
            foundAttributes,
            foundAttributes.Count > 0,
            GetTestFramework(foundAttributes));

        return AnalysisResult.Success(info);
    }

    /// <summary>
    /// Determines whether a class is likely a test class based on naming conventions and class-level attributes.
    /// </summary>
    /// <param name="classDeclaration">The class declaration to analyze.</param>
    /// <returns>A result indicating if the class is a test class.</returns>
    public static Result<bool> DetectTestClass(ClassDeclarationSyntax classDeclaration)
    {
        // EXXER200: Validate null parameters
        if (classDeclaration is null)
        {
            return AnalysisResult.Failure<bool>(classDeclarationNullErrorMessage);
        }

        var className = classDeclaration.Identifier.ValueText;

        // Check naming patterns
        var testClassSuffixes = new[] { testsSuffix, testSuffix, specsSuffix, specSuffix };
        var hasTestSuffix = testClassSuffixes.Any(suffix => className.EndsWith(suffix));

        // Check class attributes
        var testClassAttributes = new[] { testClassAttribute, testFixtureAttribute };
        var hasTestAttribute = classDeclaration.AttributeLists
            .SelectMany(list => list.Attributes)
            .Any(attr => testClassAttributes.Any(testAttr =>
                attr.Name.ToString().Contains(testAttr)));

        var isTestClass = hasTestSuffix || hasTestAttribute;
        return AnalysisResult.Success(isTestClass);
    }

    /// <summary>
    /// Infers the test framework represented by the supplied attribute names.
    /// </summary>
    /// <param name="attributes">The collection of attribute names discovered on a method.</param>
    /// <returns>
    /// A <see cref="TestFramework"/> value that best matches the supplied attributes, or <see cref="TestFramework.Unknown"/> when no framework can be inferred.
    /// </returns>
    /// <remarks>
    /// The method tolerates <c>null</c> or empty collections and defaults to <see cref="TestFramework.Unknown"/> so analyzer callers remain resilient.
    /// </remarks>
    private static TestFramework GetTestFramework(List<string> attributes)
    {
        // EXXER200: Validate null parameters - return safe default instead of throwing
        if (attributes is null)
        {
            return TestFramework.Unknown;
        }

        if (attributes.Any(a => a.Contains(xunitFramework) || a == factAttribute || a == theoryAttribute))
        {
            return TestFramework.XUnit;
        }

        if (attributes.Any(a => a.Contains(nunitFramework) || a == testAttribute))
        {
            return TestFramework.NUnit;
        }

        if (attributes.Any(a => a.Contains(msTestFramework) || a == testMethodAttribute))
        {
            return TestFramework.MSTest;
        }

        return TestFramework.Unknown;
    }
}

/// <summary>
/// Information about test attributes found on a method.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="TestAttributeInfo"/> class.
/// </remarks>
/// <param name="attributeNames">The attribute names discovered on the analyzed method.</param>
/// <param name="hasTestAttributes">
/// A value indicating whether at least one supported test attribute was identified.
/// </param>
/// <param name="framework">The test framework inferred from the supplied attributes.</param>
public class TestAttributeInfo(IReadOnlyList<string> attributeNames, bool hasTestAttributes, TestFramework framework)
{
    /// <summary>
    /// Gets the discovered test attribute names in discovery order, or an empty list when none were found.
    /// </summary>
    public IReadOnlyList<string> AttributeNames { get; } = attributeNames ?? [];

    /// <summary>
    /// Gets a value indicating whether any supported test attributes were discovered.
    /// </summary>
    public bool HasTestAttributes { get; } = hasTestAttributes;

    /// <summary>
    /// Gets the detected test framework corresponding to the attribute set.
    /// </summary>
    public TestFramework Framework { get; } = framework;

    /// <summary>
    /// Gets a value indicating whether the attribute name collection remains valid for analysis.
    /// </summary>
    public bool IsValid => AttributeNames != null;
}

/// <summary>
/// Enumeration of supported test frameworks.
/// </summary>
public enum TestFramework
{
    /// <summary>
    /// Represents an unknown or unsupported test framework.
    /// </summary>
    Unknown,

    /// <summary>
    /// Represents the xUnit testing framework.
    /// </summary>
    XUnit,

    /// <summary>
    /// Represents the NUnit testing framework.
    /// </summary>
    NUnit,

    /// <summary>
    /// Represents the MSTest testing framework.
    /// </summary>
    MSTest
}
