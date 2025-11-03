using System;
using System.Threading.Tasks;
using System.Windows.Input;
using IndFusion.Analyzers;
using IndFusion.Analyzers.Async;
using IndFusion.Analyzer.Tests.Testing;
using Shouldly;
using Xunit;

namespace IndFusion.Analyzer.Tests.TestCases.Async;

/// <summary>
/// Tests for AvoidAsyncVoidAnalyzer false-positive mitigation scenarios.
/// </summary>
public class AvoidAsyncVoidFalsePositiveTests
{
    //  Story 1.1: Allow Nullable Event Handler Parameters

    /// <summary>
    /// Tests that async void methods with nullable event handler parameters are not flagged.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Nullable_Event_Handler_Parameters()
    {
        const string testCode = @"
using System;
using System.Threading.Tasks;

namespace TestProject
{
    public class EventHandler
    {
        public async void OnButtonClick(object? sender, EventArgs? e)
        {
            await Task.Delay(100);
        }

        public async void OnDataChanged(object? sender, EventArgs? e)
        {
            await Task.Delay(100);
        }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new AvoidAsyncVoidAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

     // 

    //  Story 1.2: Allow Derived EventArgs Types

    /// <summary>
    /// Tests that async void methods with derived EventArgs types are not flagged.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Derived_EventArgs_Types()
    {
        const string testCode = @"
using System;
using System.Threading.Tasks;

namespace TestProject
{
    public class CustomEventArgs : EventArgs
    {
        public string Message { get; set; } = string.Empty;
    }

    public class EventHandler
    {
        public async void OnCustomEvent(object sender, CustomEventArgs e)
        {
            await Task.Delay(100);
        }

        public async void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            await Task.Delay(100);
        }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new AvoidAsyncVoidAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

     // 

    //  Story 1.3: Allow Routed Event Patterns

    /// <summary>
    /// Tests that async void methods with RoutedEventArgs are not flagged.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Routed_Event_Patterns()
    {
        const string testCode = @"
using System;
using System.Threading.Tasks;
using System.Windows;

namespace TestProject
{
    public class EventHandler
    {
        public async void OnRoutedEvent(object sender, RoutedEventArgs e)
        {
            await Task.Delay(100);
        }

        public async void OnMouseClick(object sender, MouseButtonEventArgs e)
        {
            await Task.Delay(100);
        }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new AvoidAsyncVoidAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

     // 

    //  Story 1.4: Allow ICommand.Execute Methods

    /// <summary>
    /// Tests that async void ICommand.Execute implementations are not flagged.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_ICommand_Execute_Methods()
    {
        const string testCode = @"
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TestProject
{
    public class AsyncCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => true;

        public async void Execute(object? parameter)
        {
            await Task.Delay(100);
        }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new AvoidAsyncVoidAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

     // 

    //  Story 1.5: Allow Overridden async void Methods

    /// <summary>
    /// Tests that overridden async void methods are not flagged.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Overridden_Async_Void_Methods()
    {
        const string testCode = @"
using System;
using System.Threading.Tasks;

namespace TestProject
{
    public abstract class BaseClass
    {
        public abstract void DoSomething();
    }

    public class DerivedClass : BaseClass
    {
        public override async void DoSomething()
        {
            await Task.Delay(100);
        }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new AvoidAsyncVoidAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

     // 

    //  Story 1.6: Allow Interface Implementations Requiring void

    /// <summary>
    /// Tests that async void methods implementing interfaces requiring void are not flagged.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Interface_Implementations_Requiring_Void()
    {
        const string testCode = @"
using System;
using System.Threading.Tasks;

namespace TestProject
{
    public interface IAsyncOperation
    {
        void Execute();
    }

    public class AsyncOperation : IAsyncOperation
    {
        public async void Execute()
        {
            await Task.Delay(100);
        }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new AvoidAsyncVoidAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

     // 

    //  Story 1.7: Allow Custom EventHandler Delegate Aliases

    /// <summary>
    /// Tests that async void methods matching custom event handler delegates are not flagged.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Custom_EventHandler_Delegate_Aliases()
    {
        const string testCode = @"
using System;
using System.Threading.Tasks;

namespace TestProject
{
    public delegate void CustomEventHandler(object sender, string message);

    public class EventHandler
    {
        public async void OnCustomEvent(object sender, string message)
        {
            await Task.Delay(100);
        }

        public async void OnDataEvent(object sender, int data)
        {
            await Task.Delay(100);
        }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new AvoidAsyncVoidAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

     // 

    //  Story 1.8: Allow Partial Methods in Blazor Components

    /// <summary>
    /// Tests that async void event handlers in Blazor components are not flagged.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Partial_Methods_In_Blazor_Components()
    {
        const string testCode = @"
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace TestProject
{
    public partial class MyComponent : ComponentBase
    {
        private async void OnButtonClick()
        {
            await Task.Delay(100);
        }

        private async void OnFormSubmit()
        {
            await Task.Delay(100);
        }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new AvoidAsyncVoidAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

     // 

    //  Story 1.9: Allow Fire-and-Forget Methods with an Attribute

    /// <summary>
    /// Tests that async void methods with FireAndForget attribute are not flagged.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Fire_And_Forget_Methods_With_Attribute()
    {
        const string testCode = @"
using System;
using System.Threading.Tasks;

namespace TestProject
{
    public class FireAndForgetAttribute : Attribute { }

    public class Service
    {
        [FireAndForget]
        public async void ProcessInBackground()
        {
            await Task.Delay(100);
        }

        [FireAndForget]
        public async void LogAsync()
        {
            await Task.Delay(100);
        }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new AvoidAsyncVoidAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

     // 

    //  Positive Control Tests

    /// <summary>
    /// Tests that regular async void methods are still flagged (positive control).
    /// </summary>
    [Fact]
    public void Should_Report_For_Regular_Async_Void_Methods()
    {
        const string testCode = @"
using System.Threading.Tasks;

namespace TestProject
{
    public class RegularService
    {
        public async void ProcessData()
        {
            await Task.Delay(100);
        }

        public async void DoSomething()
        {
            await Task.Delay(100);
        }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new AvoidAsyncVoidAnalyzer());
        diagnostics.ShouldNotBeEmpty();
        diagnostics.ShouldAllBe(d => d.Id == DiagnosticIds.AvoidAsyncVoid);
    }

     // 
}
