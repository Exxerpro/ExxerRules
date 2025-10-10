using System.Threading.Tasks;
using IndFusion.Analyzers;
using IndFusion.Analyzers.Async;
using IndFusion.Analyzer.Tests.Testing;
using Shouldly;
using Xunit;

namespace IndFusion.Analyzer.Tests.TestCases.Async;

/// <summary>
/// Tests for AsyncMethodsShouldAcceptCancellationTokenAnalyzer false-positive mitigation scenarios.
/// </summary>
public class AsyncMethodsShouldAcceptCancellationTokenFalsePositiveTests
{
    #region Story 1.1: Exempt Overridden and Explicitly Implemented Methods

    /// <summary>
    /// Tests that overridden async methods are not flagged.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Overridden_Methods()
    {
        const string testCode = @"
using System.Threading.Tasks;

namespace TestProject
{
    public abstract class BaseService
    {
        public abstract Task ProcessAsync();
    }

    public class ConcreteService : BaseService
    {
        public override async Task ProcessAsync()
        {
            await Task.Delay(100);
        }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new AsyncMethodsShouldAcceptCancellationTokenAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

    /// <summary>
    /// Tests that explicitly implemented interface methods are not flagged.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Explicit_Interface_Implementation()
    {
        const string testCode = @"
using System.Threading.Tasks;

namespace TestProject
{
    public interface IDataService
    {
        Task<string> GetDataAsync();
    }

    public class DataService : IDataService
    {
        async Task<string> IDataService.GetDataAsync()
        {
            await Task.Delay(100);
            return ""data"";
        }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new AsyncMethodsShouldAcceptCancellationTokenAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

    #endregion

    #region Story 1.2: Exempt Blazor Lifecycle Methods

    /// <summary>
    /// Tests that Blazor lifecycle methods are not flagged.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Blazor_Lifecycle_Methods()
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

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new AsyncMethodsShouldAcceptCancellationTokenAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

    #endregion

    #region Story 1.3: Exempt SignalR Hub Lifecycle Methods

    /// <summary>
    /// Tests that SignalR hub lifecycle methods are not flagged.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_SignalR_Hub_Lifecycle_Methods()
    {
        const string testCode = @"
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace TestProject
{
    public class ChatHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            await Task.Delay(100);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await Task.Delay(100);
        }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new AsyncMethodsShouldAcceptCancellationTokenAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

    #endregion

    #region Story 1.4: Exempt Test Methods

    /// <summary>
    /// Tests that test methods are not flagged.
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
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new AsyncMethodsShouldAcceptCancellationTokenAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

    #endregion

    #region Story 1.5: Exempt Test Class Helper Methods

    /// <summary>
    /// Tests that helper methods in test classes are not flagged.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Test_Class_Helper_Methods()
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

    public class ApiControllerTestFixture
    {
        private async Task ConfigureApiAsync()
        {
            await Task.Delay(100);
        }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new AsyncMethodsShouldAcceptCancellationTokenAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

    #endregion

    #region Story 1.6: Exempt IAsyncLifetime Contract Methods

    /// <summary>
    /// Tests that IAsyncLifetime methods are not flagged.
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

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new AsyncMethodsShouldAcceptCancellationTokenAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

    #endregion

    #region Story 1.7: Exempt Test Fixture Methods

    /// <summary>
    /// Tests that test fixture methods are not flagged.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Test_Fixture_Methods()
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

    public class DatabaseTestFixture
    {
        private async Task SetupDatabaseAsync()
        {
            await Task.Delay(100);
        }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new AsyncMethodsShouldAcceptCancellationTokenAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

    #endregion

    #region Story 1.8: Exempt Blazor EventCallback Handlers

    /// <summary>
    /// Tests that Blazor event handlers are not flagged.
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

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new AsyncMethodsShouldAcceptCancellationTokenAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

    #endregion

    #region Story 1.9: Analyze Cancellation Availability

    /// <summary>
    /// Tests that methods without cancellation overloads are not flagged.
    /// </summary>
    [Fact]
    public void Should_Not_Report_When_Cancellation_Not_Available()
    {
        const string testCode = @"
using System.Threading.Tasks;

namespace TestProject
{
    public class Service
    {
        public async Task ProcessAsync()
        {
            // This method doesn't have a CancellationToken overload
            await SomeMethodWithoutCancellation();
        }

        private async Task SomeMethodWithoutCancellation()
        {
            // Use a method that truly doesn't have CancellationToken overload
            await Task.Yield();
        }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new AsyncMethodsShouldAcceptCancellationTokenAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

    #endregion

    #region Story 1.10: Be Aware of Captured Tokens

    /// <summary>
    /// Tests that methods with captured tokens are not flagged.
    /// </summary>
    [Fact]
    public void Should_Not_Report_When_Token_Is_Captured()
    {
        const string testCode = @"
using System.Threading;
using System.Threading.Tasks;

namespace TestProject
{
    public class Service
    {
        private readonly CancellationToken _cancellationToken;

        public Service(CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;
        }

        public async Task ProcessAsync()
        {
            await Task.Delay(100, _cancellationToken);
        }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new AsyncMethodsShouldAcceptCancellationTokenAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

    #endregion

    #region Positive Control Tests

    /// <summary>
    /// Tests that regular async methods without CancellationToken are still flagged (positive control).
    /// </summary>
    [Fact]
    public void Should_Report_For_Regular_Async_Methods_Without_Token()
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

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new AsyncMethodsShouldAcceptCancellationTokenAnalyzer());
        diagnostics.ShouldNotBeEmpty();
        diagnostics.ShouldAllBe(d => d.Id == DiagnosticIds.AsyncMethodsShouldAcceptCancellationToken);
    }

    #endregion
}
