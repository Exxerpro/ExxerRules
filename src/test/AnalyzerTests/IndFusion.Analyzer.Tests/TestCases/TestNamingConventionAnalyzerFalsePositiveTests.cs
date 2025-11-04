using IndFusion.Analyzer.Testing;
using IndFusion.Analyzer.Tests.Testing;
using Shouldly;
using Xunit;

namespace IndFusion.Analyzer.Tests.TestCases;

/// <summary>
/// Regression tests for EXXER100 false-positive mitigations on test naming conventions.
/// </summary>
public class TestNamingConventionAnalyzerFalsePositiveTests
{
    /// <summary>
    /// Allows method-under-test prefixes like LoadPlcsAsync_Should_Return_...
    /// </summary>
    [Fact]
    public void Analyzer_Allows_MethodPrefix()
    {
        const string testCode = @"\
using Xunit;\
\
public class SampleTests\
{\
\t[Fact]\
\tpublic void LoadPlcsAsync_Should_Return_Success_When_PLCs_Found() { }\
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new TestNamingConventionAnalyzer());
        diagnostics.Length.ShouldBe(0);
    }

    /// <summary>
    /// Allows feature-oriented prefixes like Services_Should_Not_Depend_On_UI.
    /// </summary>
    [Fact]
    public void Analyzer_Allows_FeaturePrefix()
    {
        const string testCode = @"\
using Xunit;\
\
public class ArchitectureTests\
{\
\t[Fact]\
\tpublic void Services_Should_Not_Depend_On_UI() { }\
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new TestNamingConventionAnalyzer());
        diagnostics.Length.ShouldBe(0);
    }

    /// <summary>
    /// Accepts Async suffixes adjacent to segments.
    /// </summary>
    [Fact]
    public void Analyzer_Allows_Async_Suffix()
    {
        const string testCode = @"\
using Xunit;\
\
public class AsyncTests\
{\
\t[Fact]\
\tpublic void ProcessAsync_Should_Return_Result_When_Success() { }\
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new TestNamingConventionAnalyzer());
        diagnostics.Length.ShouldBe(0);
    }

    /// <summary>
    /// Permits alternate causal connectors like For/With/If/On.
    /// </summary>
    [Fact]
    public void Analyzer_Allows_Alternate_Causal_Connectors()
    {
        const string testCode = @"\
using Xunit;\
\
public class BarcodeTests\
{\
\t[Theory]\
\tpublic void Should_Return_BarCodeDetail_For_Known_Label() { }\
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new TestNamingConventionAnalyzer());
        diagnostics.Length.ShouldBe(0);
    }

    /// <summary>
    /// Allows compound conditions with additional And/With segments.
    /// </summary>
    [Fact]
    public void Analyzer_Allows_Compound_Conditions()
    {
        const string testCode = @"\
using Xunit;\
\
public class HubConnectionExtensionsTests\
{\
\t[Fact]\
\tpublic void Extensions_Should_Handle_Multiple_Concurrent_Real_Connections() { }\
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new TestNamingConventionAnalyzer());
        diagnostics.Length.ShouldBe(0);
    }

    /// <summary>
    /// Allows behavior-only names without explicit _When_ condition.
    /// </summary>
    [Fact]
    public void Analyzer_Allows_Behavior_Only()
    {
        const string testCode = @"\
using Xunit;\
\
public class MetricsTests\
{\
\t[Fact]\
\tpublic void Metrics_Should_Track_Reconnection_Count() { }\
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new TestNamingConventionAnalyzer());
        diagnostics.Length.ShouldBe(0);
    }

    /// <summary>
    /// Permits lowercase condition fragments after connectors.
    /// </summary>
    [Fact]
    public void Analyzer_Allows_Lowercase_Conditions()
    {
        const string testCode = @"\
using Xunit;\
\
public class CommandHandlerTests\
{\
\t[Fact]\
\tpublic void Process_Should_Return_Failure_When_cancelled() { }\
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new TestNamingConventionAnalyzer());
        diagnostics.Length.ShouldBe(0);
    }

    /// <summary>
    /// Skips diagnostics when DisplayName or Description is provided.
    /// </summary>
    [Fact]
    public void Analyzer_Allows_DisplayName_Override()
    {
        const string testCode = @"using Xunit;

public class DisplayNameTests
{
    [Fact(DisplayName = ""Scenario: user requests data"")]
    public void Test1() { }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new TestNamingConventionAnalyzer());
        diagnostics.Length.ShouldBe(0);
    }

    /// <summary>
    /// Relaxes naming when enclosed within nested context classes (Given_/When_).
    /// </summary>
    [Fact]
    public void Analyzer_Allows_Nested_Context_Classes()
    {
        const string testCode = @"using Xunit;

public class Given_Data
{
    public class When_No_Data
    {
        [Fact]
        public void ReturnsEmptyResult() { }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new TestNamingConventionAnalyzer());
        diagnostics.Length.ShouldBe(0);
    }

    /// <summary>
    /// Honors the AllowTestNamingVariations opt-out attribute.
    /// </summary>
    [Fact]
    public void Analyzer_Allows_OptOut_Attribute()
    {
        const string testCode = @"\
using System;\
using Xunit;\
\
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]\
public sealed class AllowTestNamingVariationsAttribute : Attribute { }\
\
public class SampleSpecs\
{\
\t[Fact, AllowTestNamingVariations]\
\tpublic void GivenUser_WhenAuthenticated_ThenShowsDashboard() { }\
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new TestNamingConventionAnalyzer());
        diagnostics.Length.ShouldBe(0);
    }
}