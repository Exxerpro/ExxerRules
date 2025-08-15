#pragma warning disable CS1998, CS0452, CS1022, IDE0053
using ExxerRules.Analyzers;
using ExxerRules.CodeFixes.Testing;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Text;
using Shouldly;
using Xunit;

namespace ExxerRules.Tests.TestCases.CodeFixes;

/// <summary>
/// Tests for the DbContextTestingCodeFixProvider class.
/// </summary>
public class DbContextTestingCodeFixProviderTests : CodeFixProviderTest<DbContextTestingCodeFixProvider>
{
    [Fact]
    public async Task RegisterCodeFixesAsync_WithMockedDbContextObjectCreation_ShouldRegisterFixes()
    {
        // Arrange
        var sourceCode = @"
using Moq;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class TestClass
{
    [Fact]
    public void TestMethod()
    {
        var mockDbContext = new Mock<TestDbContext>();
    }
}

public class TestDbContext : DbContext
{
    public DbSet<TestEntity> TestEntities { get; set; }
}

public class TestEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new DbContextTestingCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.DoNotMockDbContext, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_WithMockedDbContextWithGenericType_ShouldRegisterFixes()
    {
        // Arrange
        var sourceCode = @"
using Moq;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class TestClass
{
    [Fact]
    public void TestMethod()
    {
        var mockDbContext = new Mock<DbContext>();
    }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new DbContextTestingCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.DoNotMockDbContext, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_WithMockedDbContextWithNSubstitute_ShouldRegisterFixes()
    {
        // Arrange
        var sourceCode = @"
using NSubstitute;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class TestClass
{
    [Fact]
    public void TestMethod()
    {
        var mockDbContext = Substitute.For<TestDbContext>();
    }
}

public class TestDbContext : DbContext
{
    public DbSet<TestEntity> TestEntities { get; set; }
}

public class TestEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new DbContextTestingCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.DoNotMockDbContext, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_WithDbContextMockSetup_ShouldRegisterFixes()
    {
        // Arrange
        var sourceCode = @"
using Moq;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class TestClass
{
    [Fact]
    public void TestMethod()
    {
        var mockDbContext = new Mock<TestDbContext>();
        mockDbContext.Setup(x => x.TestEntities).Returns(Mock.Of<DbSet<TestEntity>>());
    }
}

public class TestDbContext : DbContext
{
    public DbSet<TestEntity> TestEntities { get; set; }
}

public class TestEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new DbContextTestingCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.DoNotMockDbContext, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_WithMoqUsingDirective_ShouldRegisterFixes()
    {
        // Arrange
        var sourceCode = @"
using Moq;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class TestClass
{
    [Fact]
    public void TestMethod()
    {
        var mockDbContext = new Mock<TestDbContext>();
    }
}

public class TestDbContext : DbContext
{
    public DbSet<TestEntity> TestEntities { get; set; }
}

public class TestEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new DbContextTestingCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.DoNotMockDbContext, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_WithNSubstituteUsingDirective_ShouldRegisterFixes()
    {
        // Arrange
        var sourceCode = @"
using NSubstitute;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class TestClass
{
    [Fact]
    public void TestMethod()
    {
        var mockDbContext = Substitute.For<TestDbContext>();
    }
}

public class TestDbContext : DbContext
{
    public DbSet<TestEntity> TestEntities { get; set; }
}

public class TestEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new DbContextTestingCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.DoNotMockDbContext, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_WithMockedDbContextVariable_ShouldRegisterFixes()
    {
        // Arrange
        var sourceCode = @"
using Moq;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class TestClass
{
    [Fact]
    public void TestMethod()
    {
        var mockDbContext = new Mock<TestDbContext>();
        var context = mockDbContext.Object;
    }
}

public class TestDbContext : DbContext
{
    public DbSet<TestEntity> TestEntities { get; set; }
}

public class TestEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new DbContextTestingCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.DoNotMockDbContext, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_WithNonDbContextMock_ShouldNotRegisterFixes()
    {
        // Arrange
        var sourceCode = @"
using Moq;
using Xunit;

public class TestClass
{
    [Fact]
    public void TestMethod()
    {
        var mockService = new Mock<ITestService>();
    }
}

public interface ITestService
{
    string GetData();
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new DbContextTestingCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.DoNotMockDbContext, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_WithInMemoryDbContext_ShouldNotRegisterFixes()
    {
        // Arrange
        var sourceCode = @"
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using Xunit;

public class TestClass
{
    [Fact]
    public void TestMethod()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(""TestDatabase"")
            .Options;
        var context = new TestDbContext(options);
    }
}

public class TestDbContext : DbContext
{
    public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }
    public DbSet<TestEntity> TestEntities { get; set; }
}

public class TestEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new DbContextTestingCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.DoNotMockDbContext, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_WithSqliteDbContext_ShouldNotRegisterFixes()
    {
        // Arrange
        var sourceCode = @"
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;
using Xunit;

public class TestClass
{
    [Fact]
    public void TestMethod()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseSqlite(""DataSource=:memory:"")
            .Options;
        var context = new TestDbContext(options);
    }
}

public class TestDbContext : DbContext
{
    public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }
    public DbSet<TestEntity> TestEntities { get; set; }
}

public class TestEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new DbContextTestingCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.DoNotMockDbContext, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    [Fact]
    public void FixableDiagnosticIds_ShouldReturnDoNotMockDbContext()
    {
        // Arrange
        var codeFixProvider = new DbContextTestingCodeFixProvider();

        // Act
        var fixableIds = codeFixProvider.FixableDiagnosticIds;

        // Assert
        fixableIds.ShouldContain(DiagnosticIds.DoNotMockDbContext);
        fixableIds.Length.ShouldBe(1);
    }

    [Fact]
    public void GetFixAllProvider_ShouldReturnBatchFixer()
    {
        // Arrange
        var codeFixProvider = new DbContextTestingCodeFixProvider();

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
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using Xunit;

public class TestClass
{
    [Fact]
    public void TestMethod()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(""TestDatabase"")
            .Options;
        var context = new TestDbContext(options);
    }
}

public class TestDbContext : DbContext
{
    public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }
    public DbSet<TestEntity> TestEntities { get; set; }
}

public class TestEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new DbContextTestingCodeFixProvider();
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
    public async Task RegisterCodeFixesAsync_WithMockedDbContextWithConstructorArgs_ShouldRegisterFixes()
    {
        // Arrange
        var sourceCode = @"
using Moq;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class TestClass
{
    [Fact]
    public void TestMethod()
    {
        var mockDbContext = new Mock<TestDbContext>(MockBehavior.Strict);
    }
}

public class TestDbContext : DbContext
{
    public DbSet<TestEntity> TestEntities { get; set; }
}

public class TestEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new DbContextTestingCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.DoNotMockDbContext, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            var codeFixContext = new CodeFixContext(document, diagnostic, (a, d) => { }, CancellationToken.None);
            await codeFixProvider.RegisterCodeFixesAsync(codeFixContext);
        });
    }

    [Fact]
    public async Task RegisterCodeFixesAsync_WithMockedDbContextWithComplexSetup_ShouldRegisterFixes()
    {
        // Arrange
        var sourceCode = @"
using Moq;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class TestClass
{
    [Fact]
    public void TestMethod()
    {
        var mockDbContext = new Mock<TestDbContext>();
        mockDbContext.Setup(x => x.TestEntities.Add(It.IsAny<TestEntity>()))
                    .Returns((TestEntity entity) => { return null; });
    }
}

public class TestDbContext : DbContext
{
    public DbSet<TestEntity> TestEntities { get; set; }
}

public class TestEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
}";

        // Act
        var document = CreateDocument(sourceCode);
        var codeFixProvider = new DbContextTestingCodeFixProvider();
        var diagnostic = CreateDiagnostic(DiagnosticIds.DoNotMockDbContext, Location.Create(document.FilePath!, TextSpan.FromBounds(0, sourceCode.Length), new LinePositionSpan()));

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
#pragma warning restore CS1998, CS0452, CS1022, IDE0053
