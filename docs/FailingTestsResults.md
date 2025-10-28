dotnet run
xUnit.net v3 Microsoft.Testing.Platform Runner v3.0.1+9214bef154 (64-bit .NET 10.0.0-rc.2.25502.107)

failed IndFusion.Mcp.Tests.Services.LintingServiceTests.RunLintingAsync_WithScope_ReturnsFilteredResults (1s 446ms)
  Xunit.Runner.InProc.SystemConsole.TestingPlatform.XunitException: System.InvalidOperationException : Test solution not found. Searched: F:\Dynamic\IndFusion\IndFusion.Mcp\ExxerRules\src\test\IndFusion.Mcp.Tests\TestOutput\..\..\..\..\IndFusion.sln
    at IndFusion.Mcp.Tests.Services.LintingServiceTests.GetTestSolutionPath() in /_/src/test/IndFusion.Mcp.Tests/Services/LintingServiceTests.cs:193
    at IndFusion.Mcp.Tests.Services.LintingServiceTests.RunLintingAsync_WithScope_ReturnsFilteredResults() in /_/src/test/IndFusion.Mcp.Tests/Services/LintingServiceTests.cs:80
    --- End of stack trace from previous location ---
failed IndFusion.Mcp.Tests.Services.LintingServiceTests.GetPolicyAsync_ValidSolutionPath_ReturnsPolicy (11ms)
  Xunit.Runner.InProc.SystemConsole.TestingPlatform.XunitException: System.InvalidOperationException : Test solution not found. Searched: F:\Dynamic\IndFusion\IndFusion.Mcp\ExxerRules\src\test\IndFusion.Mcp.Tests\TestOutput\..\..\..\..\IndFusion.sln
    at IndFusion.Mcp.Tests.Services.LintingServiceTests.GetTestSolutionPath() in /_/src/test/IndFusion.Mcp.Tests/Services/LintingServiceTests.cs:193
    at IndFusion.Mcp.Tests.Services.LintingServiceTests.GetPolicyAsync_ValidSolutionPath_ReturnsPolicy() in /_/src/test/IndFusion.Mcp.Tests/Services/LintingServiceTests.cs:141
    --- End of stack trace from previous location ---
failed IndFusion.Mcp.Tests.Services.LintingServiceTests.StopWatcherAsync_ValidSolutionPath_ReturnsSuccess (5ms)
  Xunit.Runner.InProc.SystemConsole.TestingPlatform.XunitException: System.InvalidOperationException : Test solution not found. Searched: F:\Dynamic\IndFusion\IndFusion.Mcp\ExxerRules\src\test\IndFusion.Mcp.Tests\TestOutput\..\..\..\..\IndFusion.sln
    at IndFusion.Mcp.Tests.Services.LintingServiceTests.GetTestSolutionPath() in /_/src/test/IndFusion.Mcp.Tests/Services/LintingServiceTests.cs:193
    at IndFusion.Mcp.Tests.Services.LintingServiceTests.StopWatcherAsync_ValidSolutionPath_ReturnsSuccess() in /_/src/test/IndFusion.Mcp.Tests/Services/LintingServiceTests.cs:123
    --- End of stack trace from previous location ---
failed IndFusion.Mcp.Tests.Services.LintingServiceTests.StartWatcherAsync_ValidRequest_ReturnsSuccess (1ms)
  Xunit.Runner.InProc.SystemConsole.TestingPlatform.XunitException: System.InvalidOperationException : Test solution not found. Searched: F:\Dynamic\IndFusion\IndFusion.Mcp\ExxerRules\src\test\IndFusion.Mcp.Tests\TestOutput\..\..\..\..\IndFusion.sln
    at IndFusion.Mcp.Tests.Services.LintingServiceTests.GetTestSolutionPath() in /_/src/test/IndFusion.Mcp.Tests/Services/LintingServiceTests.cs:193
    at IndFusion.Mcp.Tests.Services.LintingServiceTests.StartWatcherAsync_ValidRequest_ReturnsSuccess() in /_/src/test/IndFusion.Mcp.Tests/Services/LintingServiceTests.cs:100
    --- End of stack trace from previous location ---
failed IndFusion.Mcp.Tests.Services.LintingServiceTests.UpdatePolicyAsync_ValidPolicy_ReturnsSuccess (2ms)
  Xunit.Runner.InProc.SystemConsole.TestingPlatform.XunitException: System.InvalidOperationException : Test solution not found. Searched: F:\Dynamic\IndFusion\IndFusion.Mcp\ExxerRules\src\test\IndFusion.Mcp.Tests\TestOutput\..\..\..\..\IndFusion.sln
    at IndFusion.Mcp.Tests.Services.LintingServiceTests.GetTestSolutionPath() in /_/src/test/IndFusion.Mcp.Tests/Services/LintingServiceTests.cs:193
    at IndFusion.Mcp.Tests.Services.LintingServiceTests.UpdatePolicyAsync_ValidPolicy_ReturnsSuccess() in /_/src/test/IndFusion.Mcp.Tests/Services/LintingServiceTests.cs:161
    --- End of stack trace from previous location ---
failed IndFusion.Mcp.Tests.Services.LintingServiceTests.RunLintingAsync_ValidSolutionPath_ReturnsSuccessWithViolations (1ms)
  Xunit.Runner.InProc.SystemConsole.TestingPlatform.XunitException: System.InvalidOperationException : Test solution not found. Searched: F:\Dynamic\IndFusion\IndFusion.Mcp\ExxerRules\src\test\IndFusion.Mcp.Tests\TestOutput\..\..\..\..\IndFusion.sln
    at IndFusion.Mcp.Tests.Services.LintingServiceTests.GetTestSolutionPath() in /_/src/test/IndFusion.Mcp.Tests/Services/LintingServiceTests.cs:193
    at IndFusion.Mcp.Tests.Services.LintingServiceTests.RunLintingAsync_ValidSolutionPath_ReturnsSuccessWithViolations() in /_/src/test/IndFusion.Mcp.Tests/Services/LintingServiceTests.cs:35
    --- End of stack trace from previous location ---
failed (canceled) IndFusion.Mcp.Tests.ToolsNew.LoadSolutionToolTests.UnloadSolution_RemovesCachedSolution (5s 120ms)
  Xunit.Runner.InProc.SystemConsole.TestingPlatform.XunitException: Test execution timed out after 5000 milliseconds
    --- End of stack trace from previous location ---
failed (canceled) IndFusion.Mcp.Tests.MetricsProviderTests.GetFileMetrics_CachesToDiskAndMemory (5s 120ms)
  Xunit.Runner.InProc.SystemConsole.TestingPlatform.XunitException: Test execution timed out after 5000 milliseconds
    --- End of stack trace from previous location ---
failed (canceled) IndFusion.Mcp.Tests.Tools.LoadSolutionTests.LoadSolution_ValidPath_ReturnsSuccess (5s 863ms)
  Xunit.Runner.InProc.SystemConsole.TestingPlatform.XunitException: Test execution timed out after 5000 milliseconds
    --- End of stack trace from previous location ---
failed (canceled) IndFusion.Mcp.Tests.ToolsNew.LoadSolutionToolTests.LoadSolution_ValidPath_ReturnsSuccess (6s 611ms)
  Xunit.Runner.InProc.SystemConsole.TestingPlatform.XunitException: Test execution timed out after 5000 milliseconds
    --- End of stack trace from previous location ---
failed (canceled) IndFusion.Mcp.Tests.Tools.LoadSolutionTests.ClearSolutionCache_RemovesAllCachedSolutions (5s 998ms)
  Xunit.Runner.InProc.SystemConsole.TestingPlatform.XunitException: Test execution timed out after 5000 milliseconds
    --- End of stack trace from previous location ---
failed (canceled) IndFusion.Mcp.Tests.ToolsNew.LoadSolutionToolTests.ClearSolutionCache_RemovesAllCachedSolutions (5s 424ms)
  Xunit.Runner.InProc.SystemConsole.TestingPlatform.XunitException: Test execution timed out after 5000 milliseconds
    --- End of stack trace from previous location ---
failed (canceled) IndFusion.Mcp.Tests.Tools.LoadSolutionTests.UnloadSolution_RemovesCachedSolution (5s 212ms)
  Xunit.Runner.InProc.SystemConsole.TestingPlatform.XunitException: Test execution timed out after 5000 milliseconds
    --- End of stack trace from previous location ---
failed (canceled) IndFusion.Mcp.Tests.ToolsNew.LintRunToolTests.LintRun_WithProgressReporter_CallsProgressCallback (10s 023ms)
  Xunit.Runner.InProc.SystemConsole.TestingPlatform.XunitException: Test execution timed out after 10000 milliseconds
    --- End of stack trace from previous location ---
failed (canceled) IndFusion.Mcp.Tests.MetricsResourceTests.ReadMetrics_Class_ReturnsClassMetrics (5s 023ms)
  Xunit.Runner.InProc.SystemConsole.TestingPlatform.XunitException: Test execution timed out after 5000 milliseconds
    --- End of stack trace from previous location ---
failed (canceled) IndFusion.Mcp.Tests.ToolsNew.LintRunToolTests.LintRun_WithCancellation_RespectsCancellationToken (5s 234ms)
  Xunit.Runner.InProc.SystemConsole.TestingPlatform.XunitException: Test execution timed out after 5000 milliseconds
    --- End of stack trace from previous location ---
failed (canceled) IndFusion.Mcp.Tests.MetricsResourceTests.ReadMetrics_Method_ReturnsMethodMetrics (5s 009ms)
  Xunit.Runner.InProc.SystemConsole.TestingPlatform.XunitException: Test execution timed out after 5000 milliseconds
    --- End of stack trace from previous location ---
failed (canceled) IndFusion.Mcp.Tests.MetricsResourceTests.ReadMetrics_InvalidPath_ReturnsError (5s 022ms)
  Xunit.Runner.InProc.SystemConsole.TestingPlatform.XunitException: Test execution timed out after 5000 milliseconds
    --- End of stack trace from previous location ---
failed (canceled) IndFusion.Mcp.Tests.MetricsResourceTests.ReadMetrics_File_ReturnsJson (5s 026ms)
  Xunit.Runner.InProc.SystemConsole.TestingPlatform.XunitException: Test execution timed out after 5000 milliseconds
    --- End of stack trace from previous location ---
failed (canceled) IndFusion.Mcp.Tests.MetricsResourceTests.ReadMetrics_Directory_ReturnsAggregatedJson (5s 005ms)
  Xunit.Runner.InProc.SystemConsole.TestingPlatform.XunitException: Test execution timed out after 5000 milliseconds
    --- End of stack trace from previous location ---
failed IndFusion.Mcp.Tests.ToolsNew.LintRunToolTests.LintRun_WithValidSolution_ReturnsViolationsAndPolicyRecommendations (8s 970ms)
  Assert.Contains() Failure: Sub-string not found
  String:    "=== exxer linting analysis results ===\r\nanalysis c"···
  Not found: "policy"
    at IndFusion.Mcp.Tests.ToolsNew.LintRunToolTests.LintRun_WithValidSolution_ReturnsViolationsAndPolicyRecommendations() in /_/src/test/IndFusion.Mcp.Tests/ToolsNew/LintRunToolTests.cs:31
    --- End of stack trace from previous location ---

Test run summary: Failed! - bin\Debug\net10.0\IndFusion.Mcp.Tests.dll (net10.0|x64)
  total: 194
  failed: 21
  succeeded: 173
  skipped: 0
  duration: 4m 04s 569ms