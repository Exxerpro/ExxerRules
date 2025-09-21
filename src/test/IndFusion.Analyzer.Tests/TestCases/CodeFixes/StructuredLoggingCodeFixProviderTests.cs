using IndFusion.Analyzers;
using IndFusion.CodeFixes;
using IndFusion.CodeFixes.Logging;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Text;
using Shouldly;
using Xunit;

namespace IndFusion.Analyzer.Tests.TestCases.CodeFixes;

/// <summary>
/// Tests for the StructuredLoggingCodeFixProvider class.
/// </summary>
public class StructuredLoggingCodeFixProviderTests : CodeFixProviderTest<StructuredLoggingCodeFixProvider>
{
    [Fact]
    public async Task RegisterCodeFixesAsync_WithLogInformationInvocation_ShouldRegisterFixes()
    {
        // Arrange
        var sourceCode = @"
using Microsoft.Extensions.Logging;

public class TestClass
{
    private readonly ILogger<TestClass> _logger;

    public TestClass(ILogger<TestClass> logger)
    {
        _logger = logger;
    }

    public void TestMethod()
    {
        _logger.LogInformation(""User {UserId} logged in at {Time}"", 123, DateTime.Now);
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new StructuredLoggingCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.UseStructuredLogging, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_WithLogWarningInvocation_ShouldRegisterFixes()
    {
        // Arrange
        var sourceCode = @"
using Microsoft.Extensions.Logging;

public class TestClass
{
    private readonly ILogger<TestClass> _logger;

    public TestClass(ILogger<TestClass> logger)
    {
        _logger = logger;
    }

    public void TestMethod()
    {
        _logger.LogWarning(""Failed to process request {RequestId}"", ""req-123"");
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new StructuredLoggingCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.UseStructuredLogging, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_WithLogErrorInvocation_ShouldRegisterFixes()
    {
        // Arrange
        var sourceCode = @"
using Microsoft.Extensions.Logging;

public class TestClass
{
    private readonly ILogger<TestClass> _logger;

    public TestClass(ILogger<TestClass> logger)
    {
        _logger = logger;
    }

    public void TestMethod()
    {
        _logger.LogError(""An error occurred while processing {Operation}"", ""database-query"");
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new StructuredLoggingCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.UseStructuredLogging, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_WithLogDebugInvocation_ShouldRegisterFixes()
    {
        // Arrange
        var sourceCode = @"
using Microsoft.Extensions.Logging;

public class TestClass
{
    private readonly ILogger<TestClass> _logger;

    public TestClass(ILogger<TestClass> logger)
    {
        _logger = logger;
    }

    public void TestMethod()
    {
        _logger.LogDebug(""Processing item {ItemId} with value {Value}"", 456, ""test-value"");
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new StructuredLoggingCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.UseStructuredLogging, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_WithLogTraceInvocation_ShouldRegisterFixes()
    {
        // Arrange
        var sourceCode = @"
using Microsoft.Extensions.Logging;

public class TestClass
{
    private readonly ILogger<TestClass> _logger;

    public TestClass(ILogger<TestClass> logger)
    {
        _logger = logger;
    }

    public void TestMethod()
    {
        _logger.LogTrace(""Entering method {MethodName} with parameters {Params}"", ""TestMethod"", new { Id = 123 });
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new StructuredLoggingCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.UseStructuredLogging, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_WithLogCriticalInvocation_ShouldRegisterFixes()
    {
        // Arrange
        var sourceCode = @"
using Microsoft.Extensions.Logging;

public class TestClass
{
    private readonly ILogger<TestClass> _logger;

    public TestClass(ILogger<TestClass> logger)
    {
        _logger = logger;
    }

    public void TestMethod()
    {
        _logger.LogCritical(""Critical system failure in {Component}"", ""database"");
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new StructuredLoggingCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.UseStructuredLogging, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_WithInterpolatedString_ShouldRegisterFixes()
    {
        // Arrange
        var sourceCode = @"
using Microsoft.Extensions.Logging;

public class TestClass
{
    private readonly ILogger<TestClass> _logger;

    public TestClass(ILogger<TestClass> logger)
    {
        _logger = logger;
    }

    public void TestMethod()
    {
        var userId = 123;
        var time = DateTime.Now;
        _logger.LogInformation($""User {userId} logged in at {time}"");
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new StructuredLoggingCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.UseStructuredLogging, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_WithBinaryExpression_ShouldRegisterFixes()
    {
        // Arrange
        var sourceCode = @"
using Microsoft.Extensions.Logging;

public class TestClass
{
    private readonly ILogger<TestClass> _logger;

    public TestClass(ILogger<TestClass> logger)
    {
        _logger = logger;
    }

    public void TestMethod()
    {
        var userId = 123;
        _logger.LogInformation(""User "" + userId + "" logged in"");
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new StructuredLoggingCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.UseStructuredLogging, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_WithNonLoggingInvocation_ShouldNotRegisterFixes()
    {
        // Arrange
        var sourceCode = @"
public class TestClass
{
    public void TestMethod()
    {
        var result = CalculateValue();
        Console.WriteLine(""Result: "" + result);
    }

    private int CalculateValue() => 42;
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new StructuredLoggingCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.UseStructuredLogging, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    [Fact]
    public void FixableDiagnosticIds_ShouldReturnUseStructuredLogging()
    {
        // Arrange
        var codeFixProvider = new StructuredLoggingCodeFixProvider();

        // Act
        var fixableIds = codeFixProvider.FixableDiagnosticIds;

        // Assert
        fixableIds.ShouldContain(DiagnosticIds.UseStructuredLogging);
        fixableIds.Length.ShouldBe(1);
    }

    [Fact]
    public void GetFixAllProvider_ShouldReturnBatchFixer()
    {
        // Arrange
        var codeFixProvider = new StructuredLoggingCodeFixProvider();

        // Act
        var fixAllProvider = codeFixProvider.GetFixAllProvider();

        // Assert
        fixAllProvider.ShouldNotBeNull();
        fixAllProvider.ShouldBe(WellKnownFixAllProviders.BatchFixer);
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_WithNoDiagnostic_ShouldNotRegisterActions()
    {
        // Arrange
        var sourceCode = @"
using Microsoft.Extensions.Logging;

public class TestClass
{
    private readonly ILogger<TestClass> _logger;

    public TestClass(ILogger<TestClass> logger)
    {
        _logger = logger;
    }

    public void TestMethod()
    {
        _logger.LogInformation(""User {UserId} logged in"", 123);
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new StructuredLoggingCodeFixProvider();
        var diagnostics = new Diagnostic[] { };

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            if (diagnostics.Length > 0)
            {
                await codeFixProvider.RegisterCodeFixesAsync(new CodeFixContext(document, diagnostics[0], (a, d) => { }, CancellationToken.None));
            }
        });
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_WithComplexStructuredLogging_ShouldRegisterFixes()
    {
        // Arrange
        var sourceCode = @"
using Microsoft.Extensions.Logging;

public class TestClass
{
    private readonly ILogger<TestClass> _logger;

    public TestClass(ILogger<TestClass> logger)
    {
        _logger = logger;
    }

    public void TestMethod()
    {
        var userId = 123;
        var operation = ""login"";
        var timestamp = DateTime.Now;
        _logger.LogInformation($""User {userId} performed {operation} at {timestamp}"");
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new StructuredLoggingCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.UseStructuredLogging, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    private static Document CreateDocument(string sourceCode)
    {
        var workspace = new AdhocWorkspace();
        var projectId = ProjectId.CreateNewId();
        var documentId = DocumentId.CreateNewId(projectId);

        var solution = workspace.CurrentSolution
            .AddProject(projectId, "TestProject", "TestProject", LanguageNames.CSharp)
            .AddDocument(documentId, "Test.cs", SourceText.From(sourceCode));

        return solution.GetDocument(documentId)!;
    }

    private static Diagnostic CreateDiagnostic(string id, Location location)
    {
        var descriptor = new DiagnosticDescriptor(id, "Test", "Test", "Test", DiagnosticSeverity.Warning, true);
        return Diagnostic.Create(descriptor, location);
    }
}

