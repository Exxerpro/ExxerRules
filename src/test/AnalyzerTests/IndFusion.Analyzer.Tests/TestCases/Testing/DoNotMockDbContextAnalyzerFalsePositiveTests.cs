using IndFusion.Analyzer.Testing;
using IndFusion.Analyzer.Tests.Testing;
using Microsoft.CodeAnalysis;
using Shouldly;
using Xunit;

namespace IndFusion.Analyzer.Tests.TestCases.Testing;

/// <summary>
/// Test cases for DoNotMockDbContext analyzer false-positive mitigation scenarios.
/// </summary>
public class DoNotMockDbContextAnalyzerFalsePositiveTests
{
    /// <summary>
    /// Tests that domain context records are not flagged as DbContext violations.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Domain_Context_Record()
    {
        const string testCode = @"
using Moq;
namespace Samples;
public sealed record MachineConfigContext(string PartNumber);
public static class Sut
{
    public static void Arrange() => _ = new Mock<MachineConfigContext>();
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new DoNotMockDbContextAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

    /// <summary>
    /// Tests that private context records are not flagged as DbContext violations.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Private_Context_Record()
    {
        const string testCode = @"
using Moq;
namespace Samples;
public class Handler
{
    private record CreateBarcodeContext(string Label);
    public void Arrange() => _ = new Mock<CreateBarcodeContext>();
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new DoNotMockDbContextAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

    /// <summary>
    /// Tests that nested context classes are not flagged as DbContext violations.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Nested_Context_Class()
    {
        const string testCode = @"
using Moq;
namespace Samples;
public sealed class Handler
{
    private class BarCodeCreationContext { }
    public void Arrange() => _ = new Mock<BarCodeCreationContext>();
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new DoNotMockDbContextAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

    /// <summary>
    /// Tests that record contexts in aggregations are not flagged as DbContext violations.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Record_Context_In_Aggregations()
    {
        const string testCode = @"
using NSubstitute;
namespace Samples;
public sealed record PlcDetailContext(int PlcId);
public static class Sut =>
    Substitute.For<PlcDetailContext>();";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new DoNotMockDbContextAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

    /// <summary>
    /// Tests that interface-based contexts are not flagged as DbContext violations.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Interface_Based_Context()
    {
        const string testCode = @"
using Moq;
namespace Samples;
public interface IDataContext { }
public sealed class Repo
{
    public Repo()
    {
        _ = new Mock<IDataContext>();
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new DoNotMockDbContextAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

    /// <summary>
    /// Tests that MSTest TestContext is not flagged as DbContext violations.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_MSTest_TestContext()
    {
        const string testCode = @"
using Moq;
namespace Samples;
public class TestContext { }
public static class Harness
{
    public static TestContext Create() => new Mock<TestContext>().Object;
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new DoNotMockDbContextAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

    /// <summary>
    /// Tests that ASP.NET Core HttpContext is not flagged as DbContext violations.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_AspNet_HttpContext()
    {
        const string testCode = @"
using Moq;
using Microsoft.AspNetCore.Http;
namespace Samples;
public static class MiddlewareHarness
{
    public static HttpContext Arrange() => new Mock<HttpContext>().Object;
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new DoNotMockDbContextAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

    /// <summary>
    /// Tests that DataAnnotations ValidationContext is not flagged as DbContext violations.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_DataAnnotations_ValidationContext()
    {
        const string testCode = @"
using Moq;
using System.ComponentModel.DataAnnotations;
namespace Samples;
public static class ValidatorHarness
{
    public static ValidationContext Create(object target) =>
        new Mock<ValidationContext>(target).Object;
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new DoNotMockDbContextAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

    /// <summary>
    /// Tests that IDbContextFactory substitutes are not flagged as DbContext violations.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_DbContextFactory_Substitute()
    {
        const string testCode = @"
using NSubstitute;
using Microsoft.EntityFrameworkCore;
namespace Samples;
public class IndTraceDbContext : DbContext { }
public static class Harness =>
    Substitute.For<IDbContextFactory<IndTraceDbContext>>();";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new DoNotMockDbContextAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

    /// <summary>
    /// Tests that test context logger helpers are not flagged as DbContext violations.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_TestContextLogger_Helper()
    {
        const string testCode = @"
using Moq;
namespace Integration.Tests.Infrastructure;
public sealed class TestContextLogger { }
public static class Sut
{
    public static TestContextLogger Arrange() =>
        new Mock<TestContextLogger>().Object;
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new DoNotMockDbContextAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

    /// <summary>
    /// Tests that Mock.Of calls with domain contexts are not flagged.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Mock_Of_Domain_Context()
    {
        const string testCode = @"
using Moq;
namespace Samples;
public sealed record DomainContext(string Value);
public static class Sut
{
    public static DomainContext Create() => Mock.Of<DomainContext>();
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new DoNotMockDbContextAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

    /// <summary>
    /// Tests that Substitute.For calls with domain contexts are not flagged.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Substitute_For_Domain_Context()
    {
        const string testCode = @"
using NSubstitute;
namespace Samples;
public sealed record WorkflowContext(string ProcessId);
public static class Sut
{
    public static WorkflowContext Create() => Substitute.For<WorkflowContext>();
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new DoNotMockDbContextAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

    /// <summary>
    /// Tests that actual EF Core DbContext mocking is still flagged (positive control).
    /// </summary>
    [Fact]
    public void Should_Report_For_Actual_DbContext_Mocking()
    {
        const string testCode = @"
using Moq;
using Microsoft.EntityFrameworkCore;
namespace Samples;
public class IndTraceDbContext : DbContext { }
public static class Sut
{
    public static void Arrange() => _ = new Mock<IndTraceDbContext>();
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new DoNotMockDbContextAnalyzer());
        diagnostics.ShouldNotBeEmpty();
        diagnostics.ShouldAllBe(d => d.Id == DiagnosticIds.DoNotMockDbContext);
    }

    /// <summary>
    /// Tests that Mock.Of with actual DbContext is still flagged (positive control).
    /// </summary>
    [Fact]
    public void Should_Report_For_Mock_Of_Actual_DbContext()
    {
        const string testCode = @"
using Moq;
using Microsoft.EntityFrameworkCore;
namespace Samples;
public class TestDbContext : DbContext { }
public static class Sut
{
    public static TestDbContext Create() => Mock.Of<TestDbContext>();
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new DoNotMockDbContextAnalyzer());
        diagnostics.ShouldNotBeEmpty();
        diagnostics.ShouldAllBe(d => d.Id == DiagnosticIds.DoNotMockDbContext);
    }

    /// <summary>
    /// Tests that Substitute.For with actual DbContext is still flagged (positive control).
    /// </summary>
    [Fact]
    public void Should_Report_For_Substitute_For_Actual_DbContext()
    {
        const string testCode = @"
using NSubstitute;
using Microsoft.EntityFrameworkCore;
namespace Samples;
public class TestDbContext : DbContext { }
public static class Sut
{
    public static TestDbContext Create() => Substitute.For<TestDbContext>();
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new DoNotMockDbContextAnalyzer());
        diagnostics.ShouldNotBeEmpty();
        diagnostics.ShouldAllBe(d => d.Id == DiagnosticIds.DoNotMockDbContext);
    }
}
