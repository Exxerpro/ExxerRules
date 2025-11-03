using System.Threading.Tasks;
using IndFusion.Analyzers;
using IndFusion.Analyzers.Async;
using IndFusion.Analyzer.Tests.Testing;
using Shouldly;
using Xunit;

namespace IndFusion.Analyzer.Tests.TestCases.Async;

/// <summary>
/// Tests for UseConfigureAwaitFalseAnalyzer false-positive mitigation scenarios.
/// </summary>
public class UseConfigureAwaitFalseFalsePositiveTests
{
    //  Story 1.1: Exempt Test Methods

    /// <summary>
    /// Tests that await expressions in test methods are not flagged.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Test_Methods()
    {
        const string testCode = @"
using System.Threading.Tasks;
using Xunit;

namespace TestProject
{
    public class TestClass
    {
        [Fact]
        public async Task Should_Do_Something()
        {
            await Task.Delay(100);
        }

        [Theory]
        public async Task Should_Do_Something_With_Params(int value)
        {
            await Task.Delay(100);
        }

        [Test]
        public async Task Should_Test_Something()
        {
            await Task.Delay(100);
        }

        [TestMethod]
        public async Task Should_TestMethod_Something()
        {
            await Task.Delay(100);
        }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseConfigureAwaitFalseAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

     // 

    //  Story 1.2: Exempt Test Helper Methods

    /// <summary>
    /// Tests that await expressions in test helper methods are not flagged.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Test_Helper_Methods()
    {
        const string testCode = @"
using System.Threading.Tasks;

namespace TestProject
{
    public class UserServiceTests
    {
        private async Task SetupUserAsync()
        {
            await Task.Delay(100);
        }

        private async Task CleanupUserAsync()
        {
            await Task.Delay(100);
        }
    }

    public class DataServiceSpecs
    {
        private async Task InitializeDataAsync()
        {
            await Task.Delay(100);
        }
    }

    public class ApiControllerTest
    {
        private async Task ConfigureApiAsync()
        {
            await Task.Delay(100);
        }
    }

    public class UserServiceSpec
    {
        private async Task SetupTestDataAsync()
        {
            await Task.Delay(100);
        }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseConfigureAwaitFalseAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

     // 

    //  Story 1.3: Exempt Test-Related Namespaces

    /// <summary>
    /// Tests that await expressions in test-related namespaces are not flagged.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Test_Related_Namespaces()
    {
        const string testCode = @"
using System.Threading.Tasks;

namespace TestProject.Tests
{
    public class Service
    {
        public async Task ProcessAsync()
        {
            await Task.Delay(100);
        }
    }
}

namespace TestProject.TestUtilities
{
    public class Helper
    {
        public async Task DoSomethingAsync()
        {
            await Task.Delay(100);
        }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseConfigureAwaitFalseAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

     // 

    //  Story 1.4: Exempt IAsyncLifetime Implementations

    /// <summary>
    /// Tests that await expressions in IAsyncLifetime methods are not flagged.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_IAsyncLifetime_Methods()
    {
        const string testCode = @"
using System.Threading.Tasks;
using Xunit;

namespace TestProject
{
    public class TestFixture : IAsyncLifetime
    {
        public async Task InitializeAsync()
        {
            await Task.Delay(100);
        }

        public async Task DisposeAsync()
        {
            await Task.Delay(100);
        }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseConfigureAwaitFalseAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

     // 

    //  Story 1.5: Exempt Collection and Assembly Fixtures

    /// <summary>
    /// Tests that await expressions in collection and assembly fixtures are not flagged.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Collection_And_Assembly_Fixtures()
    {
        const string testCode = @"
using System.Threading.Tasks;
using Xunit;

namespace TestProject
{
    [CollectionDefinition(""Database"")]
    public class DatabaseFixture : IAsyncLifetime
    {
        public async Task InitializeAsync()
        {
            await Task.Delay(100);
        }

        public async Task DisposeAsync()
        {
            await Task.Delay(100);
        }
    }

    [Collection(""Database"")]
    public class DatabaseTestFixture
    {
        private async Task SetupDatabaseAsync()
        {
            await Task.Delay(100);
        }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseConfigureAwaitFalseAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

     // 

    //  Story 1.6: Exempt Blazor Component Lifecycle Methods

    /// <summary>
    /// Tests that await expressions in Blazor component lifecycle methods are not flagged.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Blazor_Component_Lifecycle_Methods()
    {
        const string testCode = @"
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace TestProject
{
    public class MyComponent : ComponentBase
    {
        protected override async Task OnInitializedAsync()
        {
            await Task.Delay(100);
        }

        protected override async Task OnParametersSetAsync()
        {
            await Task.Delay(100);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await Task.Delay(100);
        }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseConfigureAwaitFalseAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

     // 

    //  Story 1.7: Exempt Blazor EventCallback Handlers

    /// <summary>
    /// Tests that await expressions in Blazor event handlers are not flagged.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Blazor_Event_Handlers()
    {
        const string testCode = @"
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace TestProject
{
    public class MyComponent : ComponentBase
    {
        private async Task OnValidSubmitAsync()
        {
            await Task.Delay(100);
        }

        private async Task OnButtonClickAsync()
        {
            await Task.Delay(100);
        }

        private async Task OnFormSubmitAsync()
        {
            await Task.Delay(100);
        }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseConfigureAwaitFalseAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

     // 

    //  Story 1.8: Exempt Awaits on Expressions Without ConfigureAwait Overloads

    /// <summary>
    /// Tests that await expressions on tasks without ConfigureAwait overloads are not flagged.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Expressions_Without_ConfigureAwait_Overloads()
    {
        const string testCode = @"
using System.Threading.Tasks;

namespace TestProject
{
    public class Service
    {
        public async Task ProcessAsync()
        {
            // Task.WhenAll doesn't have ConfigureAwait overload
            await Task.WhenAll(Task.Delay(100), Task.Delay(200));
            
            // Task.FromResult doesn't have ConfigureAwait overload
            await Task.FromResult(42);
            
            // Task.Yield doesn't have ConfigureAwait overload
            await Task.Yield();
        }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseConfigureAwaitFalseAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

     // 

    //  Positive Control Tests

    /// <summary>
    /// Tests that regular await expressions without ConfigureAwait are still flagged (positive control).
    /// </summary>
    [Fact]
    public void Should_Report_For_Regular_Await_Expressions_Without_ConfigureAwait()
    {
        const string testCode = @"
using System.Threading.Tasks;

namespace TestProject
{
    public class RegularService
    {
        public async Task ProcessDataAsync()
        {
            await Task.Delay(100);
        }

        public async Task<string> GetDataAsync()
        {
            await Task.Delay(100);
            return ""data"";
        }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseConfigureAwaitFalseAnalyzer());
        diagnostics.ShouldNotBeEmpty();
        diagnostics.ShouldAllBe(d => d.Id == DiagnosticIds.UseConfigureAwaitFalse);
    }

     // 
}
