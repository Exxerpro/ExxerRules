using System.Linq;
using IndFusion.Analyzer.Async;
using IndFusion.Analyzer.Tests.Testing;
using Shouldly;
using Xunit;

namespace IndFusion.Analyzer.Tests.TestCases.Async;

/// <summary>
/// Comprehensive tests for async-related analyzers.
/// </summary>
public class AsyncComprehensiveTests
{
	/// <summary>
	/// Tests that async void methods are flagged when not used as event handlers.
	/// </summary>
	[Fact]
	public void AvoidAsyncVoid_Should_Flag_NonEventHandler()
	{
		const string code = @"
namespace TestProject
{
	public class Svc
	{
		public async void Run() { await System.Threading.Tasks.Task.Yield(); }
	}
}";

		var diags = AnalyzerTestHelper.RunAnalyzer(code, new AvoidAsyncVoidAnalyzer());
		diags.Any(d => d.Id == DiagnosticIds.AvoidAsyncVoid).ShouldBeTrue();
	}

	/// <summary>
	/// Tests that async void methods are not flagged when used as event handlers.
	/// </summary>
	[Fact]
	public void AvoidAsyncVoid_Should_NotFlag_EventHandlerPattern()
	{
		const string code = @"
using System;

namespace TestProject
{
	public class Ui
	{
		public async void OnClick(object sender, EventArgs e) { await System.Threading.Tasks.Task.Yield(); }
	}
}";

		var diags = AnalyzerTestHelper.RunAnalyzer(code, new AvoidAsyncVoidAnalyzer());
		diags.Length.ShouldBe(0);
	}
}
