 test (90 tests) [39:15.831] Failed: 90 tests failed
  AnalyzerTests (2 tests) [5:01.340] Failed: 2 tests failed
   IndFusion.Analyzer.Tests (2 tests) [5:01.340] Failed: 2 tests failed
    IndFusion.Analyzer.Tests.TestCases (2 tests) [5:01.340] Failed: 2 tests failed
     Async (2 tests) [0:19.063] Failed: 2 tests failed
      AvoidAsyncVoidFalsePositiveTests (1 test) [0:03.818] Failed: One or more child tests failed: 1 test failed
       Should_Not_Report_For_Partial_Methods_In_Blazor_Components [0:00.800] Failed: Shouldly.ShouldAssertException: [(10,28): warning EXXER302: Method 'OnButtonClick' should not be async void; return Task instead, (15,28): warning EXXER302: Method 'OnFormSubmit' should not be async v…
Shouldly.ShouldAssertException
[(10,28): warning EXXER302: Method 'OnButtonClick' should not be async void; return Task instead, (15,28): warning EXXER302: Method 'OnFormSubmit' should not be async void; return Task instead]
    should be empty but had
2
    items and was not empty
   at IndFusion.Analyzer.Tests.TestCases.Async.AvoidAsyncVoidFalsePositiveTests.Should_Not_Report_For_Partial_Methods_In_Blazor_Components() in /_/src/test/AnalyzerTests/IndFusion.Analyzer.Tests/TestCases/Async/AvoidAsyncVoidFalsePositiveTests.cs:line 292
   at System.Reflection.MethodBaseInvoker.InterpretedInvoke_Method(Object obj, IntPtr* args)
   at System.Reflection.MethodBaseInvoker.InvokeWithNoArgs(Object obj, BindingFlags invokeAttr)


      UseConfigureAwaitFalseFalsePositiveTests (1 test) [0:14.720] Failed: One or more child tests failed: 1 test failed
       Should_Not_Report_For_Blazor_Component_Lifecycle_Methods [0:00.791] Failed: Shouldly.ShouldAssertException: [(11,13): warning EXXER301: Await expression should use ConfigureAwait(false) to avoid deadlocks in library code, (16,13): warning EXXER301: Await expression should use…
Shouldly.ShouldAssertException
[(11,13): warning EXXER301: Await expression should use ConfigureAwait(false) to avoid deadlocks in library code, (16,13): warning EXXER301: Await expression should use ConfigureAwait(false) to avoid deadlocks in library code, (21,13): warning EXXER301: Await expression should use ConfigureAwait(false) to avoid deadlocks in library code]
    should be empty but had
3
    items and was not empty
   at IndFusion.Analyzer.Tests.TestCases.Async.UseConfigureAwaitFalseFalsePositiveTests.Should_Not_Report_For_Blazor_Component_Lifecycle_Methods() in /_/src/test/AnalyzerTests/IndFusion.Analyzer.Tests/TestCases/Async/UseConfigureAwaitFalseFalsePositiveTests.cs:line 270
   at System.Reflection.MethodBaseInvoker.InterpretedInvoke_Method(Object obj, IntPtr* args)
   at System.Reflection.MethodBaseInvoker.InvokeWithNoArgs(Object obj, BindingFlags invokeAttr)


  McpTests (8 tests) [33:20.177] Failed: 8 tests failed
   IndFusion.Mcp.Server.Tests (1 test) [0:01.193] Failed: 1 test failed
    IndFusion.Mcp.Server.Tests (1 test) [0:01.193] Failed: 1 test failed
     Integration (1 test) [0:00.943] Failed: 1 test failed
      McpServerHttpIntegrationTests (1 test) [0:00.943] Failed: One or more child tests failed: 1 test failed
       ListToolsCommand_ShouldReturnToolInventory [0:00.928] Failed: System.IO.IOException: MCP server process exited unexpectedly (exit code: 1)
System.IO.IOException
MCP server process exited unexpectedly (exit code: 1)
Server's stderr tail:
The provided file path does not exist: path/to/your/server.csproj.
   at System.Threading.Channels.AsyncOperation`2.GetResult(Int16 token)
   at System.Threading.Channels.ChannelReader`1.ReadAllAsync(CancellationToken cancellationToken)+MoveNext()
   at ModelContextProtocol.McpSessionHandler.ProcessMessagesCoreAsync(CancellationToken cancellationToken)
   at ModelContextProtocol.McpSessionHandler.ProcessMessagesCoreAsync(CancellationToken cancellationToken)
   at ModelContextProtocol.McpSessionHandler.DisposeAsync()
   at ModelContextProtocol.Client.McpClientImpl.DisposeAsync()
   at ModelContextProtocol.Client.McpClientImpl.ConnectAsync(CancellationToken cancellationToken)
   at ModelContextProtocol.Client.McpClient.CreateAsync(IClientTransport clientTransport, McpClientOptions clientOptions, ILoggerFactory loggerFactory, CancellationToken cancellationToken)
   at ModelContextProtocol.Client.McpClient.CreateAsync(IClientTransport clientTransport, McpClientOptions clientOptions, ILoggerFactory loggerFactory, CancellationToken cancellationToken)
   at IndFusion.Mcp.Server.Tests.Integration.McpServerHttpIntegrationTests.ListToolsCommand_ShouldReturnToolInventory() in /_/src/test/McpTests/IndFusion.Mcp.Server.Tests/Integration/McpServerHttpIntegrationTests.cs:line 86
   at Xunit.v3.TestRunner`2.<>c__DisplayClass5_0.<<InvokeTest>b__1>d.MoveNext() in /_/src/xunit.v3.core/Runners/TestRunner.cs:line 170
--- End of stack trace from previous location ---
   at Xunit.v3.ExceptionAggregator.RunAsync(Func`1 code) in /_/src/xunit.v3.core/Exceptions/ExceptionAggregator.cs:line 124


   IndFusion.Mcp.Tests (7 tests) [31:31.464] Failed: 7 tests failed
    IndFusion.Mcp.Tests (7 tests) [31:31.464] Failed: 7 tests failed
     Tools (2 tests) [12:26.705] Failed: 2 tests failed
      LoadSolutionTests (2 tests) [1:10.568] Failed: One or more child tests failed: 2 tests failed
       ClearSolutionCache_RemovesAllCachedSolutions [0:30.494] Failed: Test execution timed out after 30000 milliseconds
Xunit.Sdk.TestTimeoutException
Test execution timed out after 30000 milliseconds
   at Xunit.v3.XunitTestRunnerBase`2.<>c__DisplayClass10_0.<<RunTestWithTimeout>b__0>d.MoveNext() in /_/src/xunit.v3.core/Runners/XunitTestRunnerBase.cs:line 164
--- End of stack trace from previous location ---
   at Xunit.v3.ExceptionAggregator.RunAsync(Func`1 code) in /_/src/xunit.v3.core/Exceptions/ExceptionAggregator.cs:line 124


       UnloadSolution_RemovesCachedSolution [0:30.022] Failed: Test execution timed out after 30000 milliseconds
Xunit.Sdk.TestTimeoutException
Test execution timed out after 30000 milliseconds
   at Xunit.v3.XunitTestRunnerBase`2.<>c__DisplayClass10_0.<<RunTestWithTimeout>b__0>d.MoveNext() in /_/src/xunit.v3.core/Runners/XunitTestRunnerBase.cs:line 164
--- End of stack trace from previous location ---
   at Xunit.v3.ExceptionAggregator.RunAsync(Func`1 code) in /_/src/xunit.v3.core/Exceptions/ExceptionAggregator.cs:line 124


     ToolsNew (3 tests) [11:56.168] Failed: 3 tests failed
      LintRunToolTests (2 tests) [2:53.289] Failed: One or more child tests failed: 2 tests failed
       LintRun_WithSpecificFile_ReturnsFileSpecificViolations [0:30.240] Failed: Test execution timed out after 30000 milliseconds
Xunit.Sdk.TestTimeoutException
Test execution timed out after 30000 milliseconds
   at Xunit.v3.XunitTestRunnerBase`2.<>c__DisplayClass10_0.<<RunTestWithTimeout>b__0>d.MoveNext() in /_/src/xunit.v3.core/Runners/XunitTestRunnerBase.cs:line 164
--- End of stack trace from previous location ---
   at Xunit.v3.ExceptionAggregator.RunAsync(Func`1 code) in /_/src/xunit.v3.core/Exceptions/ExceptionAggregator.cs:line 124

=== Test Logging Started ===
Test: LintRunToolTests
Log File: C:\Users\Abel Briones\AppData\Local\Temp\IndFusion.Tests\Logs\LintRunToolTests_20251103_235206.log

       LintRun_WithValidSolution_ReturnsViolationsAndPolicyRecommendations [0:30.893] Failed: Test execution timed out after 30000 milliseconds
Xunit.Sdk.TestTimeoutException
Test execution timed out after 30000 milliseconds
   at Xunit.v3.XunitTestRunnerBase`2.<>c__DisplayClass10_0.<<RunTestWithTimeout>b__0>d.MoveNext() in /_/src/xunit.v3.core/Runners/XunitTestRunnerBase.cs:line 164
--- End of stack trace from previous location ---
   at Xunit.v3.ExceptionAggregator.RunAsync(Func`1 code) in /_/src/xunit.v3.core/Exceptions/ExceptionAggregator.cs:line 124

=== Test Logging Started ===
Test: LintRunToolTests
Log File: C:\Users\Abel Briones\AppData\Local\Temp\IndFusion.Tests\Logs\LintRunToolTests_20251103_235134.log

      LoadSolutionToolTests (1 test) [1:13.029] Failed: One or more child tests failed: 1 test failed
       ClearSolutionCache_RemovesAllCachedSolutions [0:30.051] Failed: Test execution timed out after 30000 milliseconds
Xunit.Sdk.TestTimeoutException
Test execution timed out after 30000 milliseconds
   at Xunit.v3.XunitTestRunnerBase`2.<>c__DisplayClass10_0.<<RunTestWithTimeout>b__0>d.MoveNext() in /_/src/xunit.v3.core/Runners/XunitTestRunnerBase.cs:line 164
--- End of stack trace from previous location ---
   at Xunit.v3.ExceptionAggregator.RunAsync(Func`1 code) in /_/src/xunit.v3.core/Exceptions/ExceptionAggregator.cs:line 124


     AnalyzeExxerFactoringOpportunitiesTests (1 test) [0:49.529] Failed: One or more child tests failed: 1 test failed
      AnalyzeExampleCode_ReturnsSuggestions [0:49.528] Failed: ModelContextProtocol.McpException: Error analyzing file: Could not find a part of the path 'F:\Dynamic\IndFusion\IndFusion.Mcp\ExxerRules\src\test\IndFusion.Mcp.Tests\ExampleCode.cs'.
ModelContextProtocol.McpException
Error analyzing file: Could not find a part of the path 'F:\Dynamic\IndFusion\IndFusion.Mcp\ExxerRules\src\test\IndFusion.Mcp.Tests\ExampleCode.cs'.
   at IndFusion.Mcp.Core.Tools.AnalyzeExxerFactoringOpportunitiesTool.AnalyzeExxerFactoringOpportunities(String solutionPath, String filePath, CancellationToken cancellationToken) in /_/src/code/Mcp/IndFusion.Mcp.Core/Tools/AnalyzeRefactoringOpportunitiesTool.cs:line 46
   at IndFusion.Mcp.Tests.AnalyzeExxerFactoringOpportunitiesTests.AnalyzeExampleCode_ReturnsSuggestions() in /_/src/test/McpTests/IndFusion.Mcp.Tests/AnalyzeRefactoringOpportunitiesTests.cs:line 40
   at Xunit.v3.TestRunner`2.<>c__DisplayClass5_0.<<InvokeTest>b__1>d.MoveNext() in /_/src/xunit.v3.core/Runners/TestRunner.cs:line 170
--- End of stack trace from previous location ---
   at Xunit.v3.ExceptionAggregator.RunAsync(Func`1 code) in /_/src/xunit.v3.core/Exceptions/ExceptionAggregator.cs:line 124

System.IO.DirectoryNotFoundException
Could not find a part of the path 'F:\Dynamic\IndFusion\IndFusion.Mcp\ExxerRules\src\test\IndFusion.Mcp.Tests\ExampleCode.cs'.
   at Microsoft.Win32.SafeHandles.SafeFileHandle.CreateFile(String fullPath, FileMode mode, FileAccess access, FileShare share, FileOptions options)
   at System.IO.File.ReadAllBytesAsync(String path, CancellationToken cancellationToken)
   at IndFusion.Mcp.Core.Tools.ExxerFactoringHelpers.ReadFileWithEncodingAsync(String filePath, CancellationToken cancellationToken) in /_/src/code/Mcp/IndFusion.Mcp.Core/Tools/RefactoringHelpers.cs:line 405
   at IndFusion.Mcp.Core.Tools.AnalyzeExxerFactoringOpportunitiesTool.LoadSyntaxTreeAndModel(String solutionPath, String filePath, CancellationToken cancellationToken) in /_/src/code/Mcp/IndFusion.Mcp.Core/Tools/AnalyzeRefactoringOpportunitiesTool.cs:line 66
   at IndFusion.Mcp.Core.Tools.AnalyzeExxerFactoringOpportunitiesTool.AnalyzeExxerFactoringOpportunities(String solutionPath, String filePath, CancellationToken cancellationToken) in /_/src/code/Mcp/IndFusion.Mcp.Core/Tools/AnalyzeRefactoringOpportunitiesTool.cs:line 31


     MetricsResourceTests (1 test) [1:04.825] Failed: One or more child tests failed: 1 test failed
      ReadMetrics_Class_ReturnsClassMetrics [0:30.237] Failed: Test execution timed out after 30000 milliseconds
Xunit.Sdk.TestTimeoutException
Test execution timed out after 30000 milliseconds
   at Xunit.v3.XunitTestRunnerBase`2.<>c__DisplayClass10_0.<<RunTestWithTimeout>b__0>d.MoveNext() in /_/src/xunit.v3.core/Runners/XunitTestRunnerBase.cs:line 164
--- End of stack trace from previous location ---
   at Xunit.v3.ExceptionAggregator.RunAsync(Func`1 code) in /_/src/xunit.v3.core/Exceptions/ExceptionAggregator.cs:line 124


  SemanticRagTests (80 tests) [0:52.229] Failed: 80 tests failed
   IndFusion.SemanticRag.Integration.Tests (5 tests) [0:00.030] Failed: 5 tests failed
    IndFusion.SemanticRag.Integration.Tests.IntegrationTests (5 tests) [0:00.030] Failed: 5 tests failed
     VectorSearchIntegrationTests (5 tests) [0:00.030] Failed: One or more child tests failed: 5 tests failed
      Should_HandleEmptyRepository_Gracefully [0:00.000] Failed: Class fixture type 'IndFusion.SemanticRag.Integration.Tests.IntegrationTests.IntegrationTestFixture' had one or more unresolved constructor arguments: IConfiguration configuration
Xunit.Sdk.TestPipelineException
Class fixture type 'IndFusion.SemanticRag.Integration.Tests.IntegrationTests.IntegrationTestFixture' had one or more unresolved constructor arguments: IConfiguration configuration
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 166
   at Xunit.v3.FixtureMappingManager.InitializeAsync(IReadOnlyCollection`1 fixtureTypes, Boolean createInstances) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 248
   at Xunit.v3.ExceptionAggregator.RunAsync(Func`1 code) in /_/src/xunit.v3.core/Exceptions/ExceptionAggregator.cs:line 124


      Should_HandleInvalidSearchQuery_Gracefully [0:00.000] Failed: Class fixture type 'IndFusion.SemanticRag.Integration.Tests.IntegrationTests.IntegrationTestFixture' had one or more unresolved constructor arguments: IConfiguration configuration
Xunit.Sdk.TestPipelineException
Class fixture type 'IndFusion.SemanticRag.Integration.Tests.IntegrationTests.IntegrationTestFixture' had one or more unresolved constructor arguments: IConfiguration configuration
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 166
   at Xunit.v3.FixtureMappingManager.InitializeAsync(IReadOnlyCollection`1 fixtureTypes, Boolean createInstances) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 248
   at Xunit.v3.ExceptionAggregator.RunAsync(Func`1 code) in /_/src/xunit.v3.core/Exceptions/ExceptionAggregator.cs:line 124


      Should_HandleInvalidVectorData_Gracefully [0:00.000] Failed: Class fixture type 'IndFusion.SemanticRag.Integration.Tests.IntegrationTests.IntegrationTestFixture' had one or more unresolved constructor arguments: IConfiguration configuration
Xunit.Sdk.TestPipelineException
Class fixture type 'IndFusion.SemanticRag.Integration.Tests.IntegrationTests.IntegrationTestFixture' had one or more unresolved constructor arguments: IConfiguration configuration
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 166
   at Xunit.v3.FixtureMappingManager.InitializeAsync(IReadOnlyCollection`1 fixtureTypes, Boolean createInstances) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 248
   at Xunit.v3.ExceptionAggregator.RunAsync(Func`1 code) in /_/src/xunit.v3.core/Exceptions/ExceptionAggregator.cs:line 124


      Should_SearchSimilarVectors_Successfully [0:00.000] Failed: Class fixture type 'IndFusion.SemanticRag.Integration.Tests.IntegrationTests.IntegrationTestFixture' had one or more unresolved constructor arguments: IConfiguration configuration
Xunit.Sdk.TestPipelineException
Class fixture type 'IndFusion.SemanticRag.Integration.Tests.IntegrationTests.IntegrationTestFixture' had one or more unresolved constructor arguments: IConfiguration configuration
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 166
   at Xunit.v3.FixtureMappingManager.InitializeAsync(IReadOnlyCollection`1 fixtureTypes, Boolean createInstances) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 248
   at Xunit.v3.ExceptionAggregator.RunAsync(Func`1 code) in /_/src/xunit.v3.core/Exceptions/ExceptionAggregator.cs:line 124


      Should_StoreAndRetrieveVectors_Successfully [0:00.004] Failed: Class fixture type 'IndFusion.SemanticRag.Integration.Tests.IntegrationTests.IntegrationTestFixture' had one or more unresolved constructor arguments: IConfiguration configuration
Xunit.Sdk.TestPipelineException
Class fixture type 'IndFusion.SemanticRag.Integration.Tests.IntegrationTests.IntegrationTestFixture' had one or more unresolved constructor arguments: IConfiguration configuration
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 166
   at Xunit.v3.FixtureMappingManager.InitializeAsync(IReadOnlyCollection`1 fixtureTypes, Boolean createInstances) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 248
   at Xunit.v3.ExceptionAggregator.RunAsync(Func`1 code) in /_/src/xunit.v3.core/Exceptions/ExceptionAggregator.cs:line 124


   IndFusion.SemanticRag.System.Tests (50 tests) [0:00.062] Failed: 50 tests failed
    IndFusion.SemanticRag.System.Tests.Infrastructure.Services (50 tests) [0:00.062] Failed: 50 tests failed
     Neo4jKnowledgeGraphServiceBehavioralTests (29 tests) [0:00.003] Failed: One or more child tests failed: 29 tests failed
      AddCodeNodeAsync_WithInvalidCodeNode_ShouldThrowArgumentException [0:00.000] Failed: Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
Xunit.Sdk.TestPipelineException
Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 195
   at Xunit.v3.FixtureMappingManager.InitializeAsync(IReadOnlyCollection`1 fixtureTypes, Boolean createInstances) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 248
   at Xunit.v3.ExceptionAggregator.RunAsync(Func`1 code) in /_/src/xunit.v3.core/Exceptions/ExceptionAggregator.cs:line 124

DotNet.Testcontainers.Builders.DockerUnavailableException
Docker is either not running or misconfigured. Please ensure that Docker is running and that the endpoint is properly configured.
You can customize your configuration using either the environment variables or the ~/.testcontainers.properties file.
For more information, visit: https://dotnet.testcontainers.org/custom_configuration/.
  Details: 
    Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
   at DotNet.Testcontainers.Guard.ThrowIf[TType](ArgumentInfo`1& argument, Func`2 condition, Func`2 ifClause) in /_/src/Testcontainers/Guard.Null.cs:line 62
   at DotNet.Testcontainers.Builders.AbstractBuilder`4.Validate() in /_/src/Testcontainers/Builders/AbstractBuilder`4.cs:line 146
   at DotNet.Testcontainers.Builders.ContainerBuilder`3.Validate() in /_/src/Testcontainers/Builders/ContainerBuilder`3.cs:line 408
   at Testcontainers.Qdrant.QdrantBuilder.Build() in /_/src/Testcontainers.Qdrant/QdrantBuilder.cs:line 69
   at IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture.InitializeAsync() in /_/src/test/SemanticRagTests/IndFusion.SemanticRag.System.Tests/Infrastructure/Fixtures/QdrantContainerFixture.cs:line 44
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 191

System.AggregateException
One or more errors occurred. (Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.)
  Exception doesn't have a stacktrace

DotNet.Testcontainers.Builders.DockerUnavailableException
Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
  Exception doesn't have a stacktrace

System.TimeoutException
The operation has timed out.
   at System.IO.Pipes.NamedPipeClientStream.ConnectInternal(Int32 timeout, CancellationToken cancellationToken, Int32 startTime)
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
--- End of stack trace from previous location ---
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
   at System.Threading.Tasks.Task.ExecuteWithThreadLocal(Task& currentTaskSlot, Thread threadPoolThread)
--- End of stack trace from previous location ---
   at Docker.DotNet.DockerClient.<>c__DisplayClass5_0.<<-ctor>b__0>d.MoveNext() in /_/src/Docker.DotNet/DockerClient.cs:line 69
--- End of stack trace from previous location ---
   at Microsoft.Net.Http.Client.ManagedHandler.ProcessRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 160
   at Microsoft.Net.Http.Client.ManagedHandler.SendAsync(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 77
   at System.Net.Http.HttpClient.<SendAsync>g__Core|83_0(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationTokenSource cts, Boolean disposeCts, CancellationTokenSource pendingRequestsCts, CancellationToken originalCancellationToken)
   at Docker.DotNet.DockerClient.PrivateMakeRequestAsync(TimeSpan timeout, HttpCompletionOption completionOption, HttpMethod method, String path, IQueryString queryString, IDictionary`2 headers, IRequestContent data, CancellationToken cancellationToken) in /_/src/Docker.DotNet/DockerClient.cs:line 433
   at Docker.DotNet.DockerClient.MakeRequestAsync[T](IEnumerable`1 errorHandlers, HttpMethod method, String path, IQueryString queryString, IRequestContent body, IDictionary`2 headers, TimeSpan timeout, CancellationToken token) in /_/src/Docker.DotNet/DockerClient.cs:line 243
   at DotNet.Testcontainers.Builders.DockerEndpointAuthenticationProvider.<>c__DisplayClass5_0.<<IsAvailable>b__0>d.MoveNext() in /_/src/Testcontainers/Builders/DockerEndpointAuthenticationProvider.cs:line 48


      AddCodeNodeAsync_WithValidCodeNode_ShouldAddCodeNode [0:00.000] Failed: Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
Xunit.Sdk.TestPipelineException
Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 195
   at Xunit.v3.FixtureMappingManager.InitializeAsync(IReadOnlyCollection`1 fixtureTypes, Boolean createInstances) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 248
   at Xunit.v3.ExceptionAggregator.RunAsync(Func`1 code) in /_/src/xunit.v3.core/Exceptions/ExceptionAggregator.cs:line 124

DotNet.Testcontainers.Builders.DockerUnavailableException
Docker is either not running or misconfigured. Please ensure that Docker is running and that the endpoint is properly configured.
You can customize your configuration using either the environment variables or the ~/.testcontainers.properties file.
For more information, visit: https://dotnet.testcontainers.org/custom_configuration/.
  Details: 
    Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
   at DotNet.Testcontainers.Guard.ThrowIf[TType](ArgumentInfo`1& argument, Func`2 condition, Func`2 ifClause) in /_/src/Testcontainers/Guard.Null.cs:line 62
   at DotNet.Testcontainers.Builders.AbstractBuilder`4.Validate() in /_/src/Testcontainers/Builders/AbstractBuilder`4.cs:line 146
   at DotNet.Testcontainers.Builders.ContainerBuilder`3.Validate() in /_/src/Testcontainers/Builders/ContainerBuilder`3.cs:line 408
   at Testcontainers.Qdrant.QdrantBuilder.Build() in /_/src/Testcontainers.Qdrant/QdrantBuilder.cs:line 69
   at IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture.InitializeAsync() in /_/src/test/SemanticRagTests/IndFusion.SemanticRag.System.Tests/Infrastructure/Fixtures/QdrantContainerFixture.cs:line 44
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 191

System.AggregateException
One or more errors occurred. (Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.)
  Exception doesn't have a stacktrace

DotNet.Testcontainers.Builders.DockerUnavailableException
Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
  Exception doesn't have a stacktrace

System.TimeoutException
The operation has timed out.
   at System.IO.Pipes.NamedPipeClientStream.ConnectInternal(Int32 timeout, CancellationToken cancellationToken, Int32 startTime)
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
--- End of stack trace from previous location ---
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
   at System.Threading.Tasks.Task.ExecuteWithThreadLocal(Task& currentTaskSlot, Thread threadPoolThread)
--- End of stack trace from previous location ---
   at Docker.DotNet.DockerClient.<>c__DisplayClass5_0.<<-ctor>b__0>d.MoveNext() in /_/src/Docker.DotNet/DockerClient.cs:line 69
--- End of stack trace from previous location ---
   at Microsoft.Net.Http.Client.ManagedHandler.ProcessRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 160
   at Microsoft.Net.Http.Client.ManagedHandler.SendAsync(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 77
   at System.Net.Http.HttpClient.<SendAsync>g__Core|83_0(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationTokenSource cts, Boolean disposeCts, CancellationTokenSource pendingRequestsCts, CancellationToken originalCancellationToken)
   at Docker.DotNet.DockerClient.PrivateMakeRequestAsync(TimeSpan timeout, HttpCompletionOption completionOption, HttpMethod method, String path, IQueryString queryString, IDictionary`2 headers, IRequestContent data, CancellationToken cancellationToken) in /_/src/Docker.DotNet/DockerClient.cs:line 433
   at Docker.DotNet.DockerClient.MakeRequestAsync[T](IEnumerable`1 errorHandlers, HttpMethod method, String path, IQueryString queryString, IRequestContent body, IDictionary`2 headers, TimeSpan timeout, CancellationToken token) in /_/src/Docker.DotNet/DockerClient.cs:line 243
   at DotNet.Testcontainers.Builders.DockerEndpointAuthenticationProvider.<>c__DisplayClass5_0.<<IsAvailable>b__0>d.MoveNext() in /_/src/Testcontainers/Builders/DockerEndpointAuthenticationProvider.cs:line 48


      AddNodeAsync_WithInvalidNode_ShouldThrowArgumentException [0:00.000] Failed: Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
Xunit.Sdk.TestPipelineException
Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 195
   at Xunit.v3.FixtureMappingManager.InitializeAsync(IReadOnlyCollection`1 fixtureTypes, Boolean createInstances) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 248
   at Xunit.v3.ExceptionAggregator.RunAsync(Func`1 code) in /_/src/xunit.v3.core/Exceptions/ExceptionAggregator.cs:line 124

DotNet.Testcontainers.Builders.DockerUnavailableException
Docker is either not running or misconfigured. Please ensure that Docker is running and that the endpoint is properly configured.
You can customize your configuration using either the environment variables or the ~/.testcontainers.properties file.
For more information, visit: https://dotnet.testcontainers.org/custom_configuration/.
  Details: 
    Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
   at DotNet.Testcontainers.Guard.ThrowIf[TType](ArgumentInfo`1& argument, Func`2 condition, Func`2 ifClause) in /_/src/Testcontainers/Guard.Null.cs:line 62
   at DotNet.Testcontainers.Builders.AbstractBuilder`4.Validate() in /_/src/Testcontainers/Builders/AbstractBuilder`4.cs:line 146
   at DotNet.Testcontainers.Builders.ContainerBuilder`3.Validate() in /_/src/Testcontainers/Builders/ContainerBuilder`3.cs:line 408
   at Testcontainers.Qdrant.QdrantBuilder.Build() in /_/src/Testcontainers.Qdrant/QdrantBuilder.cs:line 69
   at IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture.InitializeAsync() in /_/src/test/SemanticRagTests/IndFusion.SemanticRag.System.Tests/Infrastructure/Fixtures/QdrantContainerFixture.cs:line 44
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 191

System.AggregateException
One or more errors occurred. (Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.)
  Exception doesn't have a stacktrace

DotNet.Testcontainers.Builders.DockerUnavailableException
Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
  Exception doesn't have a stacktrace

System.TimeoutException
The operation has timed out.
   at System.IO.Pipes.NamedPipeClientStream.ConnectInternal(Int32 timeout, CancellationToken cancellationToken, Int32 startTime)
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
--- End of stack trace from previous location ---
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
   at System.Threading.Tasks.Task.ExecuteWithThreadLocal(Task& currentTaskSlot, Thread threadPoolThread)
--- End of stack trace from previous location ---
   at Docker.DotNet.DockerClient.<>c__DisplayClass5_0.<<-ctor>b__0>d.MoveNext() in /_/src/Docker.DotNet/DockerClient.cs:line 69
--- End of stack trace from previous location ---
   at Microsoft.Net.Http.Client.ManagedHandler.ProcessRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 160
   at Microsoft.Net.Http.Client.ManagedHandler.SendAsync(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 77
   at System.Net.Http.HttpClient.<SendAsync>g__Core|83_0(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationTokenSource cts, Boolean disposeCts, CancellationTokenSource pendingRequestsCts, CancellationToken originalCancellationToken)
   at Docker.DotNet.DockerClient.PrivateMakeRequestAsync(TimeSpan timeout, HttpCompletionOption completionOption, HttpMethod method, String path, IQueryString queryString, IDictionary`2 headers, IRequestContent data, CancellationToken cancellationToken) in /_/src/Docker.DotNet/DockerClient.cs:line 433
   at Docker.DotNet.DockerClient.MakeRequestAsync[T](IEnumerable`1 errorHandlers, HttpMethod method, String path, IQueryString queryString, IRequestContent body, IDictionary`2 headers, TimeSpan timeout, CancellationToken token) in /_/src/Docker.DotNet/DockerClient.cs:line 243
   at DotNet.Testcontainers.Builders.DockerEndpointAuthenticationProvider.<>c__DisplayClass5_0.<<IsAvailable>b__0>d.MoveNext() in /_/src/Testcontainers/Builders/DockerEndpointAuthenticationProvider.cs:line 48


      AddNodeAsync_WithNullNode_ShouldThrowArgumentException [0:00.000] Failed: Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
Xunit.Sdk.TestPipelineException
Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 195
   at Xunit.v3.FixtureMappingManager.InitializeAsync(IReadOnlyCollection`1 fixtureTypes, Boolean createInstances) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 248
   at Xunit.v3.ExceptionAggregator.RunAsync(Func`1 code) in /_/src/xunit.v3.core/Exceptions/ExceptionAggregator.cs:line 124

DotNet.Testcontainers.Builders.DockerUnavailableException
Docker is either not running or misconfigured. Please ensure that Docker is running and that the endpoint is properly configured.
You can customize your configuration using either the environment variables or the ~/.testcontainers.properties file.
For more information, visit: https://dotnet.testcontainers.org/custom_configuration/.
  Details: 
    Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
   at DotNet.Testcontainers.Guard.ThrowIf[TType](ArgumentInfo`1& argument, Func`2 condition, Func`2 ifClause) in /_/src/Testcontainers/Guard.Null.cs:line 62
   at DotNet.Testcontainers.Builders.AbstractBuilder`4.Validate() in /_/src/Testcontainers/Builders/AbstractBuilder`4.cs:line 146
   at DotNet.Testcontainers.Builders.ContainerBuilder`3.Validate() in /_/src/Testcontainers/Builders/ContainerBuilder`3.cs:line 408
   at Testcontainers.Qdrant.QdrantBuilder.Build() in /_/src/Testcontainers.Qdrant/QdrantBuilder.cs:line 69
   at IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture.InitializeAsync() in /_/src/test/SemanticRagTests/IndFusion.SemanticRag.System.Tests/Infrastructure/Fixtures/QdrantContainerFixture.cs:line 44
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 191

System.AggregateException
One or more errors occurred. (Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.)
  Exception doesn't have a stacktrace

DotNet.Testcontainers.Builders.DockerUnavailableException
Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
  Exception doesn't have a stacktrace

System.TimeoutException
The operation has timed out.
   at System.IO.Pipes.NamedPipeClientStream.ConnectInternal(Int32 timeout, CancellationToken cancellationToken, Int32 startTime)
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
--- End of stack trace from previous location ---
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
   at System.Threading.Tasks.Task.ExecuteWithThreadLocal(Task& currentTaskSlot, Thread threadPoolThread)
--- End of stack trace from previous location ---
   at Docker.DotNet.DockerClient.<>c__DisplayClass5_0.<<-ctor>b__0>d.MoveNext() in /_/src/Docker.DotNet/DockerClient.cs:line 69
--- End of stack trace from previous location ---
   at Microsoft.Net.Http.Client.ManagedHandler.ProcessRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 160
   at Microsoft.Net.Http.Client.ManagedHandler.SendAsync(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 77
   at System.Net.Http.HttpClient.<SendAsync>g__Core|83_0(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationTokenSource cts, Boolean disposeCts, CancellationTokenSource pendingRequestsCts, CancellationToken originalCancellationToken)
   at Docker.DotNet.DockerClient.PrivateMakeRequestAsync(TimeSpan timeout, HttpCompletionOption completionOption, HttpMethod method, String path, IQueryString queryString, IDictionary`2 headers, IRequestContent data, CancellationToken cancellationToken) in /_/src/Docker.DotNet/DockerClient.cs:line 433
   at Docker.DotNet.DockerClient.MakeRequestAsync[T](IEnumerable`1 errorHandlers, HttpMethod method, String path, IQueryString queryString, IRequestContent body, IDictionary`2 headers, TimeSpan timeout, CancellationToken token) in /_/src/Docker.DotNet/DockerClient.cs:line 243
   at DotNet.Testcontainers.Builders.DockerEndpointAuthenticationProvider.<>c__DisplayClass5_0.<<IsAvailable>b__0>d.MoveNext() in /_/src/Testcontainers/Builders/DockerEndpointAuthenticationProvider.cs:line 48


      AddNodeAsync_WithValidNode_ShouldAddNode [0:00.000] Failed: Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
Xunit.Sdk.TestPipelineException
Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 195
   at Xunit.v3.FixtureMappingManager.InitializeAsync(IReadOnlyCollection`1 fixtureTypes, Boolean createInstances) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 248
   at Xunit.v3.ExceptionAggregator.RunAsync(Func`1 code) in /_/src/xunit.v3.core/Exceptions/ExceptionAggregator.cs:line 124

DotNet.Testcontainers.Builders.DockerUnavailableException
Docker is either not running or misconfigured. Please ensure that Docker is running and that the endpoint is properly configured.
You can customize your configuration using either the environment variables or the ~/.testcontainers.properties file.
For more information, visit: https://dotnet.testcontainers.org/custom_configuration/.
  Details: 
    Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
   at DotNet.Testcontainers.Guard.ThrowIf[TType](ArgumentInfo`1& argument, Func`2 condition, Func`2 ifClause) in /_/src/Testcontainers/Guard.Null.cs:line 62
   at DotNet.Testcontainers.Builders.AbstractBuilder`4.Validate() in /_/src/Testcontainers/Builders/AbstractBuilder`4.cs:line 146
   at DotNet.Testcontainers.Builders.ContainerBuilder`3.Validate() in /_/src/Testcontainers/Builders/ContainerBuilder`3.cs:line 408
   at Testcontainers.Qdrant.QdrantBuilder.Build() in /_/src/Testcontainers.Qdrant/QdrantBuilder.cs:line 69
   at IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture.InitializeAsync() in /_/src/test/SemanticRagTests/IndFusion.SemanticRag.System.Tests/Infrastructure/Fixtures/QdrantContainerFixture.cs:line 44
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 191

System.AggregateException
One or more errors occurred. (Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.)
  Exception doesn't have a stacktrace

DotNet.Testcontainers.Builders.DockerUnavailableException
Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
  Exception doesn't have a stacktrace

System.TimeoutException
The operation has timed out.
   at System.IO.Pipes.NamedPipeClientStream.ConnectInternal(Int32 timeout, CancellationToken cancellationToken, Int32 startTime)
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
--- End of stack trace from previous location ---
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
   at System.Threading.Tasks.Task.ExecuteWithThreadLocal(Task& currentTaskSlot, Thread threadPoolThread)
--- End of stack trace from previous location ---
   at Docker.DotNet.DockerClient.<>c__DisplayClass5_0.<<-ctor>b__0>d.MoveNext() in /_/src/Docker.DotNet/DockerClient.cs:line 69
--- End of stack trace from previous location ---
   at Microsoft.Net.Http.Client.ManagedHandler.ProcessRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 160
   at Microsoft.Net.Http.Client.ManagedHandler.SendAsync(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 77
   at System.Net.Http.HttpClient.<SendAsync>g__Core|83_0(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationTokenSource cts, Boolean disposeCts, CancellationTokenSource pendingRequestsCts, CancellationToken originalCancellationToken)
   at Docker.DotNet.DockerClient.PrivateMakeRequestAsync(TimeSpan timeout, HttpCompletionOption completionOption, HttpMethod method, String path, IQueryString queryString, IDictionary`2 headers, IRequestContent data, CancellationToken cancellationToken) in /_/src/Docker.DotNet/DockerClient.cs:line 433
   at Docker.DotNet.DockerClient.MakeRequestAsync[T](IEnumerable`1 errorHandlers, HttpMethod method, String path, IQueryString queryString, IRequestContent body, IDictionary`2 headers, TimeSpan timeout, CancellationToken token) in /_/src/Docker.DotNet/DockerClient.cs:line 243
   at DotNet.Testcontainers.Builders.DockerEndpointAuthenticationProvider.<>c__DisplayClass5_0.<<IsAvailable>b__0>d.MoveNext() in /_/src/Testcontainers/Builders/DockerEndpointAuthenticationProvider.cs:line 48


      CreateRelationshipAsync_WithInvalidRelationship_ShouldThrowArgumentException [0:00.000] Failed: Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
Xunit.Sdk.TestPipelineException
Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 195
   at Xunit.v3.FixtureMappingManager.InitializeAsync(IReadOnlyCollection`1 fixtureTypes, Boolean createInstances) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 248
   at Xunit.v3.ExceptionAggregator.RunAsync(Func`1 code) in /_/src/xunit.v3.core/Exceptions/ExceptionAggregator.cs:line 124

DotNet.Testcontainers.Builders.DockerUnavailableException
Docker is either not running or misconfigured. Please ensure that Docker is running and that the endpoint is properly configured.
You can customize your configuration using either the environment variables or the ~/.testcontainers.properties file.
For more information, visit: https://dotnet.testcontainers.org/custom_configuration/.
  Details: 
    Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
   at DotNet.Testcontainers.Guard.ThrowIf[TType](ArgumentInfo`1& argument, Func`2 condition, Func`2 ifClause) in /_/src/Testcontainers/Guard.Null.cs:line 62
   at DotNet.Testcontainers.Builders.AbstractBuilder`4.Validate() in /_/src/Testcontainers/Builders/AbstractBuilder`4.cs:line 146
   at DotNet.Testcontainers.Builders.ContainerBuilder`3.Validate() in /_/src/Testcontainers/Builders/ContainerBuilder`3.cs:line 408
   at Testcontainers.Qdrant.QdrantBuilder.Build() in /_/src/Testcontainers.Qdrant/QdrantBuilder.cs:line 69
   at IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture.InitializeAsync() in /_/src/test/SemanticRagTests/IndFusion.SemanticRag.System.Tests/Infrastructure/Fixtures/QdrantContainerFixture.cs:line 44
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 191

System.AggregateException
One or more errors occurred. (Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.)
  Exception doesn't have a stacktrace

DotNet.Testcontainers.Builders.DockerUnavailableException
Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
  Exception doesn't have a stacktrace

System.TimeoutException
The operation has timed out.
   at System.IO.Pipes.NamedPipeClientStream.ConnectInternal(Int32 timeout, CancellationToken cancellationToken, Int32 startTime)
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
--- End of stack trace from previous location ---
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
   at System.Threading.Tasks.Task.ExecuteWithThreadLocal(Task& currentTaskSlot, Thread threadPoolThread)
--- End of stack trace from previous location ---
   at Docker.DotNet.DockerClient.<>c__DisplayClass5_0.<<-ctor>b__0>d.MoveNext() in /_/src/Docker.DotNet/DockerClient.cs:line 69
--- End of stack trace from previous location ---
   at Microsoft.Net.Http.Client.ManagedHandler.ProcessRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 160
   at Microsoft.Net.Http.Client.ManagedHandler.SendAsync(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 77
   at System.Net.Http.HttpClient.<SendAsync>g__Core|83_0(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationTokenSource cts, Boolean disposeCts, CancellationTokenSource pendingRequestsCts, CancellationToken originalCancellationToken)
   at Docker.DotNet.DockerClient.PrivateMakeRequestAsync(TimeSpan timeout, HttpCompletionOption completionOption, HttpMethod method, String path, IQueryString queryString, IDictionary`2 headers, IRequestContent data, CancellationToken cancellationToken) in /_/src/Docker.DotNet/DockerClient.cs:line 433
   at Docker.DotNet.DockerClient.MakeRequestAsync[T](IEnumerable`1 errorHandlers, HttpMethod method, String path, IQueryString queryString, IRequestContent body, IDictionary`2 headers, TimeSpan timeout, CancellationToken token) in /_/src/Docker.DotNet/DockerClient.cs:line 243
   at DotNet.Testcontainers.Builders.DockerEndpointAuthenticationProvider.<>c__DisplayClass5_0.<<IsAvailable>b__0>d.MoveNext() in /_/src/Testcontainers/Builders/DockerEndpointAuthenticationProvider.cs:line 48


      CreateRelationshipAsync_WithSelfReferencingRelationship_ShouldThrowArgumentException [0:00.000] Failed: Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
Xunit.Sdk.TestPipelineException
Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 195
   at Xunit.v3.FixtureMappingManager.InitializeAsync(IReadOnlyCollection`1 fixtureTypes, Boolean createInstances) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 248
   at Xunit.v3.ExceptionAggregator.RunAsync(Func`1 code) in /_/src/xunit.v3.core/Exceptions/ExceptionAggregator.cs:line 124

DotNet.Testcontainers.Builders.DockerUnavailableException
Docker is either not running or misconfigured. Please ensure that Docker is running and that the endpoint is properly configured.
You can customize your configuration using either the environment variables or the ~/.testcontainers.properties file.
For more information, visit: https://dotnet.testcontainers.org/custom_configuration/.
  Details: 
    Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
   at DotNet.Testcontainers.Guard.ThrowIf[TType](ArgumentInfo`1& argument, Func`2 condition, Func`2 ifClause) in /_/src/Testcontainers/Guard.Null.cs:line 62
   at DotNet.Testcontainers.Builders.AbstractBuilder`4.Validate() in /_/src/Testcontainers/Builders/AbstractBuilder`4.cs:line 146
   at DotNet.Testcontainers.Builders.ContainerBuilder`3.Validate() in /_/src/Testcontainers/Builders/ContainerBuilder`3.cs:line 408
   at Testcontainers.Qdrant.QdrantBuilder.Build() in /_/src/Testcontainers.Qdrant/QdrantBuilder.cs:line 69
   at IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture.InitializeAsync() in /_/src/test/SemanticRagTests/IndFusion.SemanticRag.System.Tests/Infrastructure/Fixtures/QdrantContainerFixture.cs:line 44
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 191

System.AggregateException
One or more errors occurred. (Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.)
  Exception doesn't have a stacktrace

DotNet.Testcontainers.Builders.DockerUnavailableException
Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
  Exception doesn't have a stacktrace

System.TimeoutException
The operation has timed out.
   at System.IO.Pipes.NamedPipeClientStream.ConnectInternal(Int32 timeout, CancellationToken cancellationToken, Int32 startTime)
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
--- End of stack trace from previous location ---
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
   at System.Threading.Tasks.Task.ExecuteWithThreadLocal(Task& currentTaskSlot, Thread threadPoolThread)
--- End of stack trace from previous location ---
   at Docker.DotNet.DockerClient.<>c__DisplayClass5_0.<<-ctor>b__0>d.MoveNext() in /_/src/Docker.DotNet/DockerClient.cs:line 69
--- End of stack trace from previous location ---
   at Microsoft.Net.Http.Client.ManagedHandler.ProcessRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 160
   at Microsoft.Net.Http.Client.ManagedHandler.SendAsync(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 77
   at System.Net.Http.HttpClient.<SendAsync>g__Core|83_0(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationTokenSource cts, Boolean disposeCts, CancellationTokenSource pendingRequestsCts, CancellationToken originalCancellationToken)
   at Docker.DotNet.DockerClient.PrivateMakeRequestAsync(TimeSpan timeout, HttpCompletionOption completionOption, HttpMethod method, String path, IQueryString queryString, IDictionary`2 headers, IRequestContent data, CancellationToken cancellationToken) in /_/src/Docker.DotNet/DockerClient.cs:line 433
   at Docker.DotNet.DockerClient.MakeRequestAsync[T](IEnumerable`1 errorHandlers, HttpMethod method, String path, IQueryString queryString, IRequestContent body, IDictionary`2 headers, TimeSpan timeout, CancellationToken token) in /_/src/Docker.DotNet/DockerClient.cs:line 243
   at DotNet.Testcontainers.Builders.DockerEndpointAuthenticationProvider.<>c__DisplayClass5_0.<<IsAvailable>b__0>d.MoveNext() in /_/src/Testcontainers/Builders/DockerEndpointAuthenticationProvider.cs:line 48


      CreateRelationshipAsync_WithValidRelationship_ShouldCreateRelationship [0:00.000] Failed: Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
Xunit.Sdk.TestPipelineException
Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 195
   at Xunit.v3.FixtureMappingManager.InitializeAsync(IReadOnlyCollection`1 fixtureTypes, Boolean createInstances) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 248
   at Xunit.v3.ExceptionAggregator.RunAsync(Func`1 code) in /_/src/xunit.v3.core/Exceptions/ExceptionAggregator.cs:line 124

DotNet.Testcontainers.Builders.DockerUnavailableException
Docker is either not running or misconfigured. Please ensure that Docker is running and that the endpoint is properly configured.
You can customize your configuration using either the environment variables or the ~/.testcontainers.properties file.
For more information, visit: https://dotnet.testcontainers.org/custom_configuration/.
  Details: 
    Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
   at DotNet.Testcontainers.Guard.ThrowIf[TType](ArgumentInfo`1& argument, Func`2 condition, Func`2 ifClause) in /_/src/Testcontainers/Guard.Null.cs:line 62
   at DotNet.Testcontainers.Builders.AbstractBuilder`4.Validate() in /_/src/Testcontainers/Builders/AbstractBuilder`4.cs:line 146
   at DotNet.Testcontainers.Builders.ContainerBuilder`3.Validate() in /_/src/Testcontainers/Builders/ContainerBuilder`3.cs:line 408
   at Testcontainers.Qdrant.QdrantBuilder.Build() in /_/src/Testcontainers.Qdrant/QdrantBuilder.cs:line 69
   at IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture.InitializeAsync() in /_/src/test/SemanticRagTests/IndFusion.SemanticRag.System.Tests/Infrastructure/Fixtures/QdrantContainerFixture.cs:line 44
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 191

System.AggregateException
One or more errors occurred. (Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.)
  Exception doesn't have a stacktrace

DotNet.Testcontainers.Builders.DockerUnavailableException
Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
  Exception doesn't have a stacktrace

System.TimeoutException
The operation has timed out.
   at System.IO.Pipes.NamedPipeClientStream.ConnectInternal(Int32 timeout, CancellationToken cancellationToken, Int32 startTime)
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
--- End of stack trace from previous location ---
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
   at System.Threading.Tasks.Task.ExecuteWithThreadLocal(Task& currentTaskSlot, Thread threadPoolThread)
--- End of stack trace from previous location ---
   at Docker.DotNet.DockerClient.<>c__DisplayClass5_0.<<-ctor>b__0>d.MoveNext() in /_/src/Docker.DotNet/DockerClient.cs:line 69
--- End of stack trace from previous location ---
   at Microsoft.Net.Http.Client.ManagedHandler.ProcessRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 160
   at Microsoft.Net.Http.Client.ManagedHandler.SendAsync(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 77
   at System.Net.Http.HttpClient.<SendAsync>g__Core|83_0(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationTokenSource cts, Boolean disposeCts, CancellationTokenSource pendingRequestsCts, CancellationToken originalCancellationToken)
   at Docker.DotNet.DockerClient.PrivateMakeRequestAsync(TimeSpan timeout, HttpCompletionOption completionOption, HttpMethod method, String path, IQueryString queryString, IDictionary`2 headers, IRequestContent data, CancellationToken cancellationToken) in /_/src/Docker.DotNet/DockerClient.cs:line 433
   at Docker.DotNet.DockerClient.MakeRequestAsync[T](IEnumerable`1 errorHandlers, HttpMethod method, String path, IQueryString queryString, IRequestContent body, IDictionary`2 headers, TimeSpan timeout, CancellationToken token) in /_/src/Docker.DotNet/DockerClient.cs:line 243
   at DotNet.Testcontainers.Builders.DockerEndpointAuthenticationProvider.<>c__DisplayClass5_0.<<IsAvailable>b__0>d.MoveNext() in /_/src/Testcontainers/Builders/DockerEndpointAuthenticationProvider.cs:line 48


      DeleteNodeAsync_WithEmptyNodeId_ShouldThrowArgumentException [0:00.000] Failed: Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
Xunit.Sdk.TestPipelineException
Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 195
   at Xunit.v3.FixtureMappingManager.InitializeAsync(IReadOnlyCollection`1 fixtureTypes, Boolean createInstances) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 248
   at Xunit.v3.ExceptionAggregator.RunAsync(Func`1 code) in /_/src/xunit.v3.core/Exceptions/ExceptionAggregator.cs:line 124

DotNet.Testcontainers.Builders.DockerUnavailableException
Docker is either not running or misconfigured. Please ensure that Docker is running and that the endpoint is properly configured.
You can customize your configuration using either the environment variables or the ~/.testcontainers.properties file.
For more information, visit: https://dotnet.testcontainers.org/custom_configuration/.
  Details: 
    Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
   at DotNet.Testcontainers.Guard.ThrowIf[TType](ArgumentInfo`1& argument, Func`2 condition, Func`2 ifClause) in /_/src/Testcontainers/Guard.Null.cs:line 62
   at DotNet.Testcontainers.Builders.AbstractBuilder`4.Validate() in /_/src/Testcontainers/Builders/AbstractBuilder`4.cs:line 146
   at DotNet.Testcontainers.Builders.ContainerBuilder`3.Validate() in /_/src/Testcontainers/Builders/ContainerBuilder`3.cs:line 408
   at Testcontainers.Qdrant.QdrantBuilder.Build() in /_/src/Testcontainers.Qdrant/QdrantBuilder.cs:line 69
   at IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture.InitializeAsync() in /_/src/test/SemanticRagTests/IndFusion.SemanticRag.System.Tests/Infrastructure/Fixtures/QdrantContainerFixture.cs:line 44
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 191

System.AggregateException
One or more errors occurred. (Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.)
  Exception doesn't have a stacktrace

DotNet.Testcontainers.Builders.DockerUnavailableException
Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
  Exception doesn't have a stacktrace

System.TimeoutException
The operation has timed out.
   at System.IO.Pipes.NamedPipeClientStream.ConnectInternal(Int32 timeout, CancellationToken cancellationToken, Int32 startTime)
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
--- End of stack trace from previous location ---
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
   at System.Threading.Tasks.Task.ExecuteWithThreadLocal(Task& currentTaskSlot, Thread threadPoolThread)
--- End of stack trace from previous location ---
   at Docker.DotNet.DockerClient.<>c__DisplayClass5_0.<<-ctor>b__0>d.MoveNext() in /_/src/Docker.DotNet/DockerClient.cs:line 69
--- End of stack trace from previous location ---
   at Microsoft.Net.Http.Client.ManagedHandler.ProcessRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 160
   at Microsoft.Net.Http.Client.ManagedHandler.SendAsync(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 77
   at System.Net.Http.HttpClient.<SendAsync>g__Core|83_0(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationTokenSource cts, Boolean disposeCts, CancellationTokenSource pendingRequestsCts, CancellationToken originalCancellationToken)
   at Docker.DotNet.DockerClient.PrivateMakeRequestAsync(TimeSpan timeout, HttpCompletionOption completionOption, HttpMethod method, String path, IQueryString queryString, IDictionary`2 headers, IRequestContent data, CancellationToken cancellationToken) in /_/src/Docker.DotNet/DockerClient.cs:line 433
   at Docker.DotNet.DockerClient.MakeRequestAsync[T](IEnumerable`1 errorHandlers, HttpMethod method, String path, IQueryString queryString, IRequestContent body, IDictionary`2 headers, TimeSpan timeout, CancellationToken token) in /_/src/Docker.DotNet/DockerClient.cs:line 243
   at DotNet.Testcontainers.Builders.DockerEndpointAuthenticationProvider.<>c__DisplayClass5_0.<<IsAvailable>b__0>d.MoveNext() in /_/src/Testcontainers/Builders/DockerEndpointAuthenticationProvider.cs:line 48


      DeleteNodeAsync_WithNullNodeId_ShouldThrowArgumentException [0:00.000] Failed: Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
Xunit.Sdk.TestPipelineException
Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 195
   at Xunit.v3.FixtureMappingManager.InitializeAsync(IReadOnlyCollection`1 fixtureTypes, Boolean createInstances) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 248
   at Xunit.v3.ExceptionAggregator.RunAsync(Func`1 code) in /_/src/xunit.v3.core/Exceptions/ExceptionAggregator.cs:line 124

DotNet.Testcontainers.Builders.DockerUnavailableException
Docker is either not running or misconfigured. Please ensure that Docker is running and that the endpoint is properly configured.
You can customize your configuration using either the environment variables or the ~/.testcontainers.properties file.
For more information, visit: https://dotnet.testcontainers.org/custom_configuration/.
  Details: 
    Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
   at DotNet.Testcontainers.Guard.ThrowIf[TType](ArgumentInfo`1& argument, Func`2 condition, Func`2 ifClause) in /_/src/Testcontainers/Guard.Null.cs:line 62
   at DotNet.Testcontainers.Builders.AbstractBuilder`4.Validate() in /_/src/Testcontainers/Builders/AbstractBuilder`4.cs:line 146
   at DotNet.Testcontainers.Builders.ContainerBuilder`3.Validate() in /_/src/Testcontainers/Builders/ContainerBuilder`3.cs:line 408
   at Testcontainers.Qdrant.QdrantBuilder.Build() in /_/src/Testcontainers.Qdrant/QdrantBuilder.cs:line 69
   at IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture.InitializeAsync() in /_/src/test/SemanticRagTests/IndFusion.SemanticRag.System.Tests/Infrastructure/Fixtures/QdrantContainerFixture.cs:line 44
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 191

System.AggregateException
One or more errors occurred. (Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.)
  Exception doesn't have a stacktrace

DotNet.Testcontainers.Builders.DockerUnavailableException
Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
  Exception doesn't have a stacktrace

System.TimeoutException
The operation has timed out.
   at System.IO.Pipes.NamedPipeClientStream.ConnectInternal(Int32 timeout, CancellationToken cancellationToken, Int32 startTime)
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
--- End of stack trace from previous location ---
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
   at System.Threading.Tasks.Task.ExecuteWithThreadLocal(Task& currentTaskSlot, Thread threadPoolThread)
--- End of stack trace from previous location ---
   at Docker.DotNet.DockerClient.<>c__DisplayClass5_0.<<-ctor>b__0>d.MoveNext() in /_/src/Docker.DotNet/DockerClient.cs:line 69
--- End of stack trace from previous location ---
   at Microsoft.Net.Http.Client.ManagedHandler.ProcessRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 160
   at Microsoft.Net.Http.Client.ManagedHandler.SendAsync(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 77
   at System.Net.Http.HttpClient.<SendAsync>g__Core|83_0(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationTokenSource cts, Boolean disposeCts, CancellationTokenSource pendingRequestsCts, CancellationToken originalCancellationToken)
   at Docker.DotNet.DockerClient.PrivateMakeRequestAsync(TimeSpan timeout, HttpCompletionOption completionOption, HttpMethod method, String path, IQueryString queryString, IDictionary`2 headers, IRequestContent data, CancellationToken cancellationToken) in /_/src/Docker.DotNet/DockerClient.cs:line 433
   at Docker.DotNet.DockerClient.MakeRequestAsync[T](IEnumerable`1 errorHandlers, HttpMethod method, String path, IQueryString queryString, IRequestContent body, IDictionary`2 headers, TimeSpan timeout, CancellationToken token) in /_/src/Docker.DotNet/DockerClient.cs:line 243
   at DotNet.Testcontainers.Builders.DockerEndpointAuthenticationProvider.<>c__DisplayClass5_0.<<IsAvailable>b__0>d.MoveNext() in /_/src/Testcontainers/Builders/DockerEndpointAuthenticationProvider.cs:line 48


      DeleteNodeAsync_WithValidNodeId_ShouldDeleteNode [0:00.000] Failed: Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
Xunit.Sdk.TestPipelineException
Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 195
   at Xunit.v3.FixtureMappingManager.InitializeAsync(IReadOnlyCollection`1 fixtureTypes, Boolean createInstances) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 248
   at Xunit.v3.ExceptionAggregator.RunAsync(Func`1 code) in /_/src/xunit.v3.core/Exceptions/ExceptionAggregator.cs:line 124

DotNet.Testcontainers.Builders.DockerUnavailableException
Docker is either not running or misconfigured. Please ensure that Docker is running and that the endpoint is properly configured.
You can customize your configuration using either the environment variables or the ~/.testcontainers.properties file.
For more information, visit: https://dotnet.testcontainers.org/custom_configuration/.
  Details: 
    Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
   at DotNet.Testcontainers.Guard.ThrowIf[TType](ArgumentInfo`1& argument, Func`2 condition, Func`2 ifClause) in /_/src/Testcontainers/Guard.Null.cs:line 62
   at DotNet.Testcontainers.Builders.AbstractBuilder`4.Validate() in /_/src/Testcontainers/Builders/AbstractBuilder`4.cs:line 146
   at DotNet.Testcontainers.Builders.ContainerBuilder`3.Validate() in /_/src/Testcontainers/Builders/ContainerBuilder`3.cs:line 408
   at Testcontainers.Qdrant.QdrantBuilder.Build() in /_/src/Testcontainers.Qdrant/QdrantBuilder.cs:line 69
   at IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture.InitializeAsync() in /_/src/test/SemanticRagTests/IndFusion.SemanticRag.System.Tests/Infrastructure/Fixtures/QdrantContainerFixture.cs:line 44
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 191

System.AggregateException
One or more errors occurred. (Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.)
  Exception doesn't have a stacktrace

DotNet.Testcontainers.Builders.DockerUnavailableException
Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
  Exception doesn't have a stacktrace

System.TimeoutException
The operation has timed out.
   at System.IO.Pipes.NamedPipeClientStream.ConnectInternal(Int32 timeout, CancellationToken cancellationToken, Int32 startTime)
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
--- End of stack trace from previous location ---
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
   at System.Threading.Tasks.Task.ExecuteWithThreadLocal(Task& currentTaskSlot, Thread threadPoolThread)
--- End of stack trace from previous location ---
   at Docker.DotNet.DockerClient.<>c__DisplayClass5_0.<<-ctor>b__0>d.MoveNext() in /_/src/Docker.DotNet/DockerClient.cs:line 69
--- End of stack trace from previous location ---
   at Microsoft.Net.Http.Client.ManagedHandler.ProcessRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 160
   at Microsoft.Net.Http.Client.ManagedHandler.SendAsync(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 77
   at System.Net.Http.HttpClient.<SendAsync>g__Core|83_0(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationTokenSource cts, Boolean disposeCts, CancellationTokenSource pendingRequestsCts, CancellationToken originalCancellationToken)
   at Docker.DotNet.DockerClient.PrivateMakeRequestAsync(TimeSpan timeout, HttpCompletionOption completionOption, HttpMethod method, String path, IQueryString queryString, IDictionary`2 headers, IRequestContent data, CancellationToken cancellationToken) in /_/src/Docker.DotNet/DockerClient.cs:line 433
   at Docker.DotNet.DockerClient.MakeRequestAsync[T](IEnumerable`1 errorHandlers, HttpMethod method, String path, IQueryString queryString, IRequestContent body, IDictionary`2 headers, TimeSpan timeout, CancellationToken token) in /_/src/Docker.DotNet/DockerClient.cs:line 243
   at DotNet.Testcontainers.Builders.DockerEndpointAuthenticationProvider.<>c__DisplayClass5_0.<<IsAvailable>b__0>d.MoveNext() in /_/src/Testcontainers/Builders/DockerEndpointAuthenticationProvider.cs:line 48


      DeleteRelationshipAsync_WithEmptyRelationshipId_ShouldReturnFailure [0:00.000] Failed: Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
Xunit.Sdk.TestPipelineException
Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 195
   at Xunit.v3.FixtureMappingManager.InitializeAsync(IReadOnlyCollection`1 fixtureTypes, Boolean createInstances) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 248
   at Xunit.v3.ExceptionAggregator.RunAsync(Func`1 code) in /_/src/xunit.v3.core/Exceptions/ExceptionAggregator.cs:line 124

DotNet.Testcontainers.Builders.DockerUnavailableException
Docker is either not running or misconfigured. Please ensure that Docker is running and that the endpoint is properly configured.
You can customize your configuration using either the environment variables or the ~/.testcontainers.properties file.
For more information, visit: https://dotnet.testcontainers.org/custom_configuration/.
  Details: 
    Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
   at DotNet.Testcontainers.Guard.ThrowIf[TType](ArgumentInfo`1& argument, Func`2 condition, Func`2 ifClause) in /_/src/Testcontainers/Guard.Null.cs:line 62
   at DotNet.Testcontainers.Builders.AbstractBuilder`4.Validate() in /_/src/Testcontainers/Builders/AbstractBuilder`4.cs:line 146
   at DotNet.Testcontainers.Builders.ContainerBuilder`3.Validate() in /_/src/Testcontainers/Builders/ContainerBuilder`3.cs:line 408
   at Testcontainers.Qdrant.QdrantBuilder.Build() in /_/src/Testcontainers.Qdrant/QdrantBuilder.cs:line 69
   at IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture.InitializeAsync() in /_/src/test/SemanticRagTests/IndFusion.SemanticRag.System.Tests/Infrastructure/Fixtures/QdrantContainerFixture.cs:line 44
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 191

System.AggregateException
One or more errors occurred. (Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.)
  Exception doesn't have a stacktrace

DotNet.Testcontainers.Builders.DockerUnavailableException
Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
  Exception doesn't have a stacktrace

System.TimeoutException
The operation has timed out.
   at System.IO.Pipes.NamedPipeClientStream.ConnectInternal(Int32 timeout, CancellationToken cancellationToken, Int32 startTime)
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
--- End of stack trace from previous location ---
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
   at System.Threading.Tasks.Task.ExecuteWithThreadLocal(Task& currentTaskSlot, Thread threadPoolThread)
--- End of stack trace from previous location ---
   at Docker.DotNet.DockerClient.<>c__DisplayClass5_0.<<-ctor>b__0>d.MoveNext() in /_/src/Docker.DotNet/DockerClient.cs:line 69
--- End of stack trace from previous location ---
   at Microsoft.Net.Http.Client.ManagedHandler.ProcessRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 160
   at Microsoft.Net.Http.Client.ManagedHandler.SendAsync(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 77
   at System.Net.Http.HttpClient.<SendAsync>g__Core|83_0(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationTokenSource cts, Boolean disposeCts, CancellationTokenSource pendingRequestsCts, CancellationToken originalCancellationToken)
   at Docker.DotNet.DockerClient.PrivateMakeRequestAsync(TimeSpan timeout, HttpCompletionOption completionOption, HttpMethod method, String path, IQueryString queryString, IDictionary`2 headers, IRequestContent data, CancellationToken cancellationToken) in /_/src/Docker.DotNet/DockerClient.cs:line 433
   at Docker.DotNet.DockerClient.MakeRequestAsync[T](IEnumerable`1 errorHandlers, HttpMethod method, String path, IQueryString queryString, IRequestContent body, IDictionary`2 headers, TimeSpan timeout, CancellationToken token) in /_/src/Docker.DotNet/DockerClient.cs:line 243
   at DotNet.Testcontainers.Builders.DockerEndpointAuthenticationProvider.<>c__DisplayClass5_0.<<IsAvailable>b__0>d.MoveNext() in /_/src/Testcontainers/Builders/DockerEndpointAuthenticationProvider.cs:line 48


      DeleteRelationshipAsync_WithNullRelationshipId_ShouldReturnFailure [0:00.000] Failed: Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
Xunit.Sdk.TestPipelineException
Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 195
   at Xunit.v3.FixtureMappingManager.InitializeAsync(IReadOnlyCollection`1 fixtureTypes, Boolean createInstances) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 248
   at Xunit.v3.ExceptionAggregator.RunAsync(Func`1 code) in /_/src/xunit.v3.core/Exceptions/ExceptionAggregator.cs:line 124

DotNet.Testcontainers.Builders.DockerUnavailableException
Docker is either not running or misconfigured. Please ensure that Docker is running and that the endpoint is properly configured.
You can customize your configuration using either the environment variables or the ~/.testcontainers.properties file.
For more information, visit: https://dotnet.testcontainers.org/custom_configuration/.
  Details: 
    Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
   at DotNet.Testcontainers.Guard.ThrowIf[TType](ArgumentInfo`1& argument, Func`2 condition, Func`2 ifClause) in /_/src/Testcontainers/Guard.Null.cs:line 62
   at DotNet.Testcontainers.Builders.AbstractBuilder`4.Validate() in /_/src/Testcontainers/Builders/AbstractBuilder`4.cs:line 146
   at DotNet.Testcontainers.Builders.ContainerBuilder`3.Validate() in /_/src/Testcontainers/Builders/ContainerBuilder`3.cs:line 408
   at Testcontainers.Qdrant.QdrantBuilder.Build() in /_/src/Testcontainers.Qdrant/QdrantBuilder.cs:line 69
   at IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture.InitializeAsync() in /_/src/test/SemanticRagTests/IndFusion.SemanticRag.System.Tests/Infrastructure/Fixtures/QdrantContainerFixture.cs:line 44
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 191

System.AggregateException
One or more errors occurred. (Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.)
  Exception doesn't have a stacktrace

DotNet.Testcontainers.Builders.DockerUnavailableException
Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
  Exception doesn't have a stacktrace

System.TimeoutException
The operation has timed out.
   at System.IO.Pipes.NamedPipeClientStream.ConnectInternal(Int32 timeout, CancellationToken cancellationToken, Int32 startTime)
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
--- End of stack trace from previous location ---
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
   at System.Threading.Tasks.Task.ExecuteWithThreadLocal(Task& currentTaskSlot, Thread threadPoolThread)
--- End of stack trace from previous location ---
   at Docker.DotNet.DockerClient.<>c__DisplayClass5_0.<<-ctor>b__0>d.MoveNext() in /_/src/Docker.DotNet/DockerClient.cs:line 69
--- End of stack trace from previous location ---
   at Microsoft.Net.Http.Client.ManagedHandler.ProcessRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 160
   at Microsoft.Net.Http.Client.ManagedHandler.SendAsync(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 77
   at System.Net.Http.HttpClient.<SendAsync>g__Core|83_0(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationTokenSource cts, Boolean disposeCts, CancellationTokenSource pendingRequestsCts, CancellationToken originalCancellationToken)
   at Docker.DotNet.DockerClient.PrivateMakeRequestAsync(TimeSpan timeout, HttpCompletionOption completionOption, HttpMethod method, String path, IQueryString queryString, IDictionary`2 headers, IRequestContent data, CancellationToken cancellationToken) in /_/src/Docker.DotNet/DockerClient.cs:line 433
   at Docker.DotNet.DockerClient.MakeRequestAsync[T](IEnumerable`1 errorHandlers, HttpMethod method, String path, IQueryString queryString, IRequestContent body, IDictionary`2 headers, TimeSpan timeout, CancellationToken token) in /_/src/Docker.DotNet/DockerClient.cs:line 243
   at DotNet.Testcontainers.Builders.DockerEndpointAuthenticationProvider.<>c__DisplayClass5_0.<<IsAvailable>b__0>d.MoveNext() in /_/src/Testcontainers/Builders/DockerEndpointAuthenticationProvider.cs:line 48


      DeleteRelationshipAsync_WithValidRelationshipId_ShouldDeleteRelationship [0:00.000] Failed: Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
Xunit.Sdk.TestPipelineException
Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 195
   at Xunit.v3.FixtureMappingManager.InitializeAsync(IReadOnlyCollection`1 fixtureTypes, Boolean createInstances) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 248
   at Xunit.v3.ExceptionAggregator.RunAsync(Func`1 code) in /_/src/xunit.v3.core/Exceptions/ExceptionAggregator.cs:line 124

DotNet.Testcontainers.Builders.DockerUnavailableException
Docker is either not running or misconfigured. Please ensure that Docker is running and that the endpoint is properly configured.
You can customize your configuration using either the environment variables or the ~/.testcontainers.properties file.
For more information, visit: https://dotnet.testcontainers.org/custom_configuration/.
  Details: 
    Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
   at DotNet.Testcontainers.Guard.ThrowIf[TType](ArgumentInfo`1& argument, Func`2 condition, Func`2 ifClause) in /_/src/Testcontainers/Guard.Null.cs:line 62
   at DotNet.Testcontainers.Builders.AbstractBuilder`4.Validate() in /_/src/Testcontainers/Builders/AbstractBuilder`4.cs:line 146
   at DotNet.Testcontainers.Builders.ContainerBuilder`3.Validate() in /_/src/Testcontainers/Builders/ContainerBuilder`3.cs:line 408
   at Testcontainers.Qdrant.QdrantBuilder.Build() in /_/src/Testcontainers.Qdrant/QdrantBuilder.cs:line 69
   at IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture.InitializeAsync() in /_/src/test/SemanticRagTests/IndFusion.SemanticRag.System.Tests/Infrastructure/Fixtures/QdrantContainerFixture.cs:line 44
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 191

System.AggregateException
One or more errors occurred. (Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.)
  Exception doesn't have a stacktrace

DotNet.Testcontainers.Builders.DockerUnavailableException
Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
  Exception doesn't have a stacktrace

System.TimeoutException
The operation has timed out.
   at System.IO.Pipes.NamedPipeClientStream.ConnectInternal(Int32 timeout, CancellationToken cancellationToken, Int32 startTime)
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
--- End of stack trace from previous location ---
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
   at System.Threading.Tasks.Task.ExecuteWithThreadLocal(Task& currentTaskSlot, Thread threadPoolThread)
--- End of stack trace from previous location ---
   at Docker.DotNet.DockerClient.<>c__DisplayClass5_0.<<-ctor>b__0>d.MoveNext() in /_/src/Docker.DotNet/DockerClient.cs:line 69
--- End of stack trace from previous location ---
   at Microsoft.Net.Http.Client.ManagedHandler.ProcessRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 160
   at Microsoft.Net.Http.Client.ManagedHandler.SendAsync(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 77
   at System.Net.Http.HttpClient.<SendAsync>g__Core|83_0(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationTokenSource cts, Boolean disposeCts, CancellationTokenSource pendingRequestsCts, CancellationToken originalCancellationToken)
   at Docker.DotNet.DockerClient.PrivateMakeRequestAsync(TimeSpan timeout, HttpCompletionOption completionOption, HttpMethod method, String path, IQueryString queryString, IDictionary`2 headers, IRequestContent data, CancellationToken cancellationToken) in /_/src/Docker.DotNet/DockerClient.cs:line 433
   at Docker.DotNet.DockerClient.MakeRequestAsync[T](IEnumerable`1 errorHandlers, HttpMethod method, String path, IQueryString queryString, IRequestContent body, IDictionary`2 headers, TimeSpan timeout, CancellationToken token) in /_/src/Docker.DotNet/DockerClient.cs:line 243
   at DotNet.Testcontainers.Builders.DockerEndpointAuthenticationProvider.<>c__DisplayClass5_0.<<IsAvailable>b__0>d.MoveNext() in /_/src/Testcontainers/Builders/DockerEndpointAuthenticationProvider.cs:line 48


      GetContextAsync_WithEmptyQuery_ShouldThrowArgumentException [0:00.000] Failed: Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
Xunit.Sdk.TestPipelineException
Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 195
   at Xunit.v3.FixtureMappingManager.InitializeAsync(IReadOnlyCollection`1 fixtureTypes, Boolean createInstances) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 248
   at Xunit.v3.ExceptionAggregator.RunAsync(Func`1 code) in /_/src/xunit.v3.core/Exceptions/ExceptionAggregator.cs:line 124

DotNet.Testcontainers.Builders.DockerUnavailableException
Docker is either not running or misconfigured. Please ensure that Docker is running and that the endpoint is properly configured.
You can customize your configuration using either the environment variables or the ~/.testcontainers.properties file.
For more information, visit: https://dotnet.testcontainers.org/custom_configuration/.
  Details: 
    Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
   at DotNet.Testcontainers.Guard.ThrowIf[TType](ArgumentInfo`1& argument, Func`2 condition, Func`2 ifClause) in /_/src/Testcontainers/Guard.Null.cs:line 62
   at DotNet.Testcontainers.Builders.AbstractBuilder`4.Validate() in /_/src/Testcontainers/Builders/AbstractBuilder`4.cs:line 146
   at DotNet.Testcontainers.Builders.ContainerBuilder`3.Validate() in /_/src/Testcontainers/Builders/ContainerBuilder`3.cs:line 408
   at Testcontainers.Qdrant.QdrantBuilder.Build() in /_/src/Testcontainers.Qdrant/QdrantBuilder.cs:line 69
   at IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture.InitializeAsync() in /_/src/test/SemanticRagTests/IndFusion.SemanticRag.System.Tests/Infrastructure/Fixtures/QdrantContainerFixture.cs:line 44
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 191

System.AggregateException
One or more errors occurred. (Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.)
  Exception doesn't have a stacktrace

DotNet.Testcontainers.Builders.DockerUnavailableException
Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
  Exception doesn't have a stacktrace

System.TimeoutException
The operation has timed out.
   at System.IO.Pipes.NamedPipeClientStream.ConnectInternal(Int32 timeout, CancellationToken cancellationToken, Int32 startTime)
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
--- End of stack trace from previous location ---
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
   at System.Threading.Tasks.Task.ExecuteWithThreadLocal(Task& currentTaskSlot, Thread threadPoolThread)
--- End of stack trace from previous location ---
   at Docker.DotNet.DockerClient.<>c__DisplayClass5_0.<<-ctor>b__0>d.MoveNext() in /_/src/Docker.DotNet/DockerClient.cs:line 69
--- End of stack trace from previous location ---
   at Microsoft.Net.Http.Client.ManagedHandler.ProcessRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 160
   at Microsoft.Net.Http.Client.ManagedHandler.SendAsync(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 77
   at System.Net.Http.HttpClient.<SendAsync>g__Core|83_0(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationTokenSource cts, Boolean disposeCts, CancellationTokenSource pendingRequestsCts, CancellationToken originalCancellationToken)
   at Docker.DotNet.DockerClient.PrivateMakeRequestAsync(TimeSpan timeout, HttpCompletionOption completionOption, HttpMethod method, String path, IQueryString queryString, IDictionary`2 headers, IRequestContent data, CancellationToken cancellationToken) in /_/src/Docker.DotNet/DockerClient.cs:line 433
   at Docker.DotNet.DockerClient.MakeRequestAsync[T](IEnumerable`1 errorHandlers, HttpMethod method, String path, IQueryString queryString, IRequestContent body, IDictionary`2 headers, TimeSpan timeout, CancellationToken token) in /_/src/Docker.DotNet/DockerClient.cs:line 243
   at DotNet.Testcontainers.Builders.DockerEndpointAuthenticationProvider.<>c__DisplayClass5_0.<<IsAvailable>b__0>d.MoveNext() in /_/src/Testcontainers/Builders/DockerEndpointAuthenticationProvider.cs:line 48


      GetContextAsync_WithNullQuery_ShouldThrowArgumentException [0:00.000] Failed: Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
Xunit.Sdk.TestPipelineException
Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 195
   at Xunit.v3.FixtureMappingManager.InitializeAsync(IReadOnlyCollection`1 fixtureTypes, Boolean createInstances) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 248
   at Xunit.v3.ExceptionAggregator.RunAsync(Func`1 code) in /_/src/xunit.v3.core/Exceptions/ExceptionAggregator.cs:line 124

DotNet.Testcontainers.Builders.DockerUnavailableException
Docker is either not running or misconfigured. Please ensure that Docker is running and that the endpoint is properly configured.
You can customize your configuration using either the environment variables or the ~/.testcontainers.properties file.
For more information, visit: https://dotnet.testcontainers.org/custom_configuration/.
  Details: 
    Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
   at DotNet.Testcontainers.Guard.ThrowIf[TType](ArgumentInfo`1& argument, Func`2 condition, Func`2 ifClause) in /_/src/Testcontainers/Guard.Null.cs:line 62
   at DotNet.Testcontainers.Builders.AbstractBuilder`4.Validate() in /_/src/Testcontainers/Builders/AbstractBuilder`4.cs:line 146
   at DotNet.Testcontainers.Builders.ContainerBuilder`3.Validate() in /_/src/Testcontainers/Builders/ContainerBuilder`3.cs:line 408
   at Testcontainers.Qdrant.QdrantBuilder.Build() in /_/src/Testcontainers.Qdrant/QdrantBuilder.cs:line 69
   at IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture.InitializeAsync() in /_/src/test/SemanticRagTests/IndFusion.SemanticRag.System.Tests/Infrastructure/Fixtures/QdrantContainerFixture.cs:line 44
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 191

System.AggregateException
One or more errors occurred. (Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.)
  Exception doesn't have a stacktrace

DotNet.Testcontainers.Builders.DockerUnavailableException
Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
  Exception doesn't have a stacktrace

System.TimeoutException
The operation has timed out.
   at System.IO.Pipes.NamedPipeClientStream.ConnectInternal(Int32 timeout, CancellationToken cancellationToken, Int32 startTime)
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
--- End of stack trace from previous location ---
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
   at System.Threading.Tasks.Task.ExecuteWithThreadLocal(Task& currentTaskSlot, Thread threadPoolThread)
--- End of stack trace from previous location ---
   at Docker.DotNet.DockerClient.<>c__DisplayClass5_0.<<-ctor>b__0>d.MoveNext() in /_/src/Docker.DotNet/DockerClient.cs:line 69
--- End of stack trace from previous location ---
   at Microsoft.Net.Http.Client.ManagedHandler.ProcessRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 160
   at Microsoft.Net.Http.Client.ManagedHandler.SendAsync(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 77
   at System.Net.Http.HttpClient.<SendAsync>g__Core|83_0(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationTokenSource cts, Boolean disposeCts, CancellationTokenSource pendingRequestsCts, CancellationToken originalCancellationToken)
   at Docker.DotNet.DockerClient.PrivateMakeRequestAsync(TimeSpan timeout, HttpCompletionOption completionOption, HttpMethod method, String path, IQueryString queryString, IDictionary`2 headers, IRequestContent data, CancellationToken cancellationToken) in /_/src/Docker.DotNet/DockerClient.cs:line 433
   at Docker.DotNet.DockerClient.MakeRequestAsync[T](IEnumerable`1 errorHandlers, HttpMethod method, String path, IQueryString queryString, IRequestContent body, IDictionary`2 headers, TimeSpan timeout, CancellationToken token) in /_/src/Docker.DotNet/DockerClient.cs:line 243
   at DotNet.Testcontainers.Builders.DockerEndpointAuthenticationProvider.<>c__DisplayClass5_0.<<IsAvailable>b__0>d.MoveNext() in /_/src/Testcontainers/Builders/DockerEndpointAuthenticationProvider.cs:line 48


      GetContextAsync_WithValidQuery_ShouldReturnContext [0:00.000] Failed: Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
Xunit.Sdk.TestPipelineException
Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 195
   at Xunit.v3.FixtureMappingManager.InitializeAsync(IReadOnlyCollection`1 fixtureTypes, Boolean createInstances) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 248
   at Xunit.v3.ExceptionAggregator.RunAsync(Func`1 code) in /_/src/xunit.v3.core/Exceptions/ExceptionAggregator.cs:line 124

DotNet.Testcontainers.Builders.DockerUnavailableException
Docker is either not running or misconfigured. Please ensure that Docker is running and that the endpoint is properly configured.
You can customize your configuration using either the environment variables or the ~/.testcontainers.properties file.
For more information, visit: https://dotnet.testcontainers.org/custom_configuration/.
  Details: 
    Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
   at DotNet.Testcontainers.Guard.ThrowIf[TType](ArgumentInfo`1& argument, Func`2 condition, Func`2 ifClause) in /_/src/Testcontainers/Guard.Null.cs:line 62
   at DotNet.Testcontainers.Builders.AbstractBuilder`4.Validate() in /_/src/Testcontainers/Builders/AbstractBuilder`4.cs:line 146
   at DotNet.Testcontainers.Builders.ContainerBuilder`3.Validate() in /_/src/Testcontainers/Builders/ContainerBuilder`3.cs:line 408
   at Testcontainers.Qdrant.QdrantBuilder.Build() in /_/src/Testcontainers.Qdrant/QdrantBuilder.cs:line 69
   at IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture.InitializeAsync() in /_/src/test/SemanticRagTests/IndFusion.SemanticRag.System.Tests/Infrastructure/Fixtures/QdrantContainerFixture.cs:line 44
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 191

System.AggregateException
One or more errors occurred. (Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.)
  Exception doesn't have a stacktrace

DotNet.Testcontainers.Builders.DockerUnavailableException
Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
  Exception doesn't have a stacktrace

System.TimeoutException
The operation has timed out.
   at System.IO.Pipes.NamedPipeClientStream.ConnectInternal(Int32 timeout, CancellationToken cancellationToken, Int32 startTime)
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
--- End of stack trace from previous location ---
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
   at System.Threading.Tasks.Task.ExecuteWithThreadLocal(Task& currentTaskSlot, Thread threadPoolThread)
--- End of stack trace from previous location ---
   at Docker.DotNet.DockerClient.<>c__DisplayClass5_0.<<-ctor>b__0>d.MoveNext() in /_/src/Docker.DotNet/DockerClient.cs:line 69
--- End of stack trace from previous location ---
   at Microsoft.Net.Http.Client.ManagedHandler.ProcessRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 160
   at Microsoft.Net.Http.Client.ManagedHandler.SendAsync(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 77
   at System.Net.Http.HttpClient.<SendAsync>g__Core|83_0(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationTokenSource cts, Boolean disposeCts, CancellationTokenSource pendingRequestsCts, CancellationToken originalCancellationToken)
   at Docker.DotNet.DockerClient.PrivateMakeRequestAsync(TimeSpan timeout, HttpCompletionOption completionOption, HttpMethod method, String path, IQueryString queryString, IDictionary`2 headers, IRequestContent data, CancellationToken cancellationToken) in /_/src/Docker.DotNet/DockerClient.cs:line 433
   at Docker.DotNet.DockerClient.MakeRequestAsync[T](IEnumerable`1 errorHandlers, HttpMethod method, String path, IQueryString queryString, IRequestContent body, IDictionary`2 headers, TimeSpan timeout, CancellationToken token) in /_/src/Docker.DotNet/DockerClient.cs:line 243
   at DotNet.Testcontainers.Builders.DockerEndpointAuthenticationProvider.<>c__DisplayClass5_0.<<IsAvailable>b__0>d.MoveNext() in /_/src/Testcontainers/Builders/DockerEndpointAuthenticationProvider.cs:line 48


      QueryAsync_WithAggregationQuery_ShouldReturnAggregatedResults [0:00.000] Failed: Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
Xunit.Sdk.TestPipelineException
Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 195
   at Xunit.v3.FixtureMappingManager.InitializeAsync(IReadOnlyCollection`1 fixtureTypes, Boolean createInstances) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 248
   at Xunit.v3.ExceptionAggregator.RunAsync(Func`1 code) in /_/src/xunit.v3.core/Exceptions/ExceptionAggregator.cs:line 124

DotNet.Testcontainers.Builders.DockerUnavailableException
Docker is either not running or misconfigured. Please ensure that Docker is running and that the endpoint is properly configured.
You can customize your configuration using either the environment variables or the ~/.testcontainers.properties file.
For more information, visit: https://dotnet.testcontainers.org/custom_configuration/.
  Details: 
    Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
   at DotNet.Testcontainers.Guard.ThrowIf[TType](ArgumentInfo`1& argument, Func`2 condition, Func`2 ifClause) in /_/src/Testcontainers/Guard.Null.cs:line 62
   at DotNet.Testcontainers.Builders.AbstractBuilder`4.Validate() in /_/src/Testcontainers/Builders/AbstractBuilder`4.cs:line 146
   at DotNet.Testcontainers.Builders.ContainerBuilder`3.Validate() in /_/src/Testcontainers/Builders/ContainerBuilder`3.cs:line 408
   at Testcontainers.Qdrant.QdrantBuilder.Build() in /_/src/Testcontainers.Qdrant/QdrantBuilder.cs:line 69
   at IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture.InitializeAsync() in /_/src/test/SemanticRagTests/IndFusion.SemanticRag.System.Tests/Infrastructure/Fixtures/QdrantContainerFixture.cs:line 44
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 191

System.AggregateException
One or more errors occurred. (Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.)
  Exception doesn't have a stacktrace

DotNet.Testcontainers.Builders.DockerUnavailableException
Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
  Exception doesn't have a stacktrace

System.TimeoutException
The operation has timed out.
   at System.IO.Pipes.NamedPipeClientStream.ConnectInternal(Int32 timeout, CancellationToken cancellationToken, Int32 startTime)
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
--- End of stack trace from previous location ---
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
   at System.Threading.Tasks.Task.ExecuteWithThreadLocal(Task& currentTaskSlot, Thread threadPoolThread)
--- End of stack trace from previous location ---
   at Docker.DotNet.DockerClient.<>c__DisplayClass5_0.<<-ctor>b__0>d.MoveNext() in /_/src/Docker.DotNet/DockerClient.cs:line 69
--- End of stack trace from previous location ---
   at Microsoft.Net.Http.Client.ManagedHandler.ProcessRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 160
   at Microsoft.Net.Http.Client.ManagedHandler.SendAsync(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 77
   at System.Net.Http.HttpClient.<SendAsync>g__Core|83_0(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationTokenSource cts, Boolean disposeCts, CancellationTokenSource pendingRequestsCts, CancellationToken originalCancellationToken)
   at Docker.DotNet.DockerClient.PrivateMakeRequestAsync(TimeSpan timeout, HttpCompletionOption completionOption, HttpMethod method, String path, IQueryString queryString, IDictionary`2 headers, IRequestContent data, CancellationToken cancellationToken) in /_/src/Docker.DotNet/DockerClient.cs:line 433
   at Docker.DotNet.DockerClient.MakeRequestAsync[T](IEnumerable`1 errorHandlers, HttpMethod method, String path, IQueryString queryString, IRequestContent body, IDictionary`2 headers, TimeSpan timeout, CancellationToken token) in /_/src/Docker.DotNet/DockerClient.cs:line 243
   at DotNet.Testcontainers.Builders.DockerEndpointAuthenticationProvider.<>c__DisplayClass5_0.<<IsAvailable>b__0>d.MoveNext() in /_/src/Testcontainers/Builders/DockerEndpointAuthenticationProvider.cs:line 48


      QueryAsync_WithCancellation_ShouldRespectCancellationToken [0:00.000] Failed: Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
Xunit.Sdk.TestPipelineException
Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 195
   at Xunit.v3.FixtureMappingManager.InitializeAsync(IReadOnlyCollection`1 fixtureTypes, Boolean createInstances) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 248
   at Xunit.v3.ExceptionAggregator.RunAsync(Func`1 code) in /_/src/xunit.v3.core/Exceptions/ExceptionAggregator.cs:line 124

DotNet.Testcontainers.Builders.DockerUnavailableException
Docker is either not running or misconfigured. Please ensure that Docker is running and that the endpoint is properly configured.
You can customize your configuration using either the environment variables or the ~/.testcontainers.properties file.
For more information, visit: https://dotnet.testcontainers.org/custom_configuration/.
  Details: 
    Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
   at DotNet.Testcontainers.Guard.ThrowIf[TType](ArgumentInfo`1& argument, Func`2 condition, Func`2 ifClause) in /_/src/Testcontainers/Guard.Null.cs:line 62
   at DotNet.Testcontainers.Builders.AbstractBuilder`4.Validate() in /_/src/Testcontainers/Builders/AbstractBuilder`4.cs:line 146
   at DotNet.Testcontainers.Builders.ContainerBuilder`3.Validate() in /_/src/Testcontainers/Builders/ContainerBuilder`3.cs:line 408
   at Testcontainers.Qdrant.QdrantBuilder.Build() in /_/src/Testcontainers.Qdrant/QdrantBuilder.cs:line 69
   at IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture.InitializeAsync() in /_/src/test/SemanticRagTests/IndFusion.SemanticRag.System.Tests/Infrastructure/Fixtures/QdrantContainerFixture.cs:line 44
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 191

System.AggregateException
One or more errors occurred. (Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.)
  Exception doesn't have a stacktrace

DotNet.Testcontainers.Builders.DockerUnavailableException
Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
  Exception doesn't have a stacktrace

System.TimeoutException
The operation has timed out.
   at System.IO.Pipes.NamedPipeClientStream.ConnectInternal(Int32 timeout, CancellationToken cancellationToken, Int32 startTime)
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
--- End of stack trace from previous location ---
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
   at System.Threading.Tasks.Task.ExecuteWithThreadLocal(Task& currentTaskSlot, Thread threadPoolThread)
--- End of stack trace from previous location ---
   at Docker.DotNet.DockerClient.<>c__DisplayClass5_0.<<-ctor>b__0>d.MoveNext() in /_/src/Docker.DotNet/DockerClient.cs:line 69
--- End of stack trace from previous location ---
   at Microsoft.Net.Http.Client.ManagedHandler.ProcessRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 160
   at Microsoft.Net.Http.Client.ManagedHandler.SendAsync(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 77
   at System.Net.Http.HttpClient.<SendAsync>g__Core|83_0(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationTokenSource cts, Boolean disposeCts, CancellationTokenSource pendingRequestsCts, CancellationToken originalCancellationToken)
   at Docker.DotNet.DockerClient.PrivateMakeRequestAsync(TimeSpan timeout, HttpCompletionOption completionOption, HttpMethod method, String path, IQueryString queryString, IDictionary`2 headers, IRequestContent data, CancellationToken cancellationToken) in /_/src/Docker.DotNet/DockerClient.cs:line 433
   at Docker.DotNet.DockerClient.MakeRequestAsync[T](IEnumerable`1 errorHandlers, HttpMethod method, String path, IQueryString queryString, IRequestContent body, IDictionary`2 headers, TimeSpan timeout, CancellationToken token) in /_/src/Docker.DotNet/DockerClient.cs:line 243
   at DotNet.Testcontainers.Builders.DockerEndpointAuthenticationProvider.<>c__DisplayClass5_0.<<IsAvailable>b__0>d.MoveNext() in /_/src/Testcontainers/Builders/DockerEndpointAuthenticationProvider.cs:line 48


      QueryAsync_WithComplexQuery_ShouldExecuteComplexQuery [0:00.000] Failed: Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
Xunit.Sdk.TestPipelineException
Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 195
   at Xunit.v3.FixtureMappingManager.InitializeAsync(IReadOnlyCollection`1 fixtureTypes, Boolean createInstances) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 248
   at Xunit.v3.ExceptionAggregator.RunAsync(Func`1 code) in /_/src/xunit.v3.core/Exceptions/ExceptionAggregator.cs:line 124

DotNet.Testcontainers.Builders.DockerUnavailableException
Docker is either not running or misconfigured. Please ensure that Docker is running and that the endpoint is properly configured.
You can customize your configuration using either the environment variables or the ~/.testcontainers.properties file.
For more information, visit: https://dotnet.testcontainers.org/custom_configuration/.
  Details: 
    Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
   at DotNet.Testcontainers.Guard.ThrowIf[TType](ArgumentInfo`1& argument, Func`2 condition, Func`2 ifClause) in /_/src/Testcontainers/Guard.Null.cs:line 62
   at DotNet.Testcontainers.Builders.AbstractBuilder`4.Validate() in /_/src/Testcontainers/Builders/AbstractBuilder`4.cs:line 146
   at DotNet.Testcontainers.Builders.ContainerBuilder`3.Validate() in /_/src/Testcontainers/Builders/ContainerBuilder`3.cs:line 408
   at Testcontainers.Qdrant.QdrantBuilder.Build() in /_/src/Testcontainers.Qdrant/QdrantBuilder.cs:line 69
   at IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture.InitializeAsync() in /_/src/test/SemanticRagTests/IndFusion.SemanticRag.System.Tests/Infrastructure/Fixtures/QdrantContainerFixture.cs:line 44
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 191

System.AggregateException
One or more errors occurred. (Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.)
  Exception doesn't have a stacktrace

DotNet.Testcontainers.Builders.DockerUnavailableException
Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
  Exception doesn't have a stacktrace

System.TimeoutException
The operation has timed out.
   at System.IO.Pipes.NamedPipeClientStream.ConnectInternal(Int32 timeout, CancellationToken cancellationToken, Int32 startTime)
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
--- End of stack trace from previous location ---
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
   at System.Threading.Tasks.Task.ExecuteWithThreadLocal(Task& currentTaskSlot, Thread threadPoolThread)
--- End of stack trace from previous location ---
   at Docker.DotNet.DockerClient.<>c__DisplayClass5_0.<<-ctor>b__0>d.MoveNext() in /_/src/Docker.DotNet/DockerClient.cs:line 69
--- End of stack trace from previous location ---
   at Microsoft.Net.Http.Client.ManagedHandler.ProcessRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 160
   at Microsoft.Net.Http.Client.ManagedHandler.SendAsync(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 77
   at System.Net.Http.HttpClient.<SendAsync>g__Core|83_0(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationTokenSource cts, Boolean disposeCts, CancellationTokenSource pendingRequestsCts, CancellationToken originalCancellationToken)
   at Docker.DotNet.DockerClient.PrivateMakeRequestAsync(TimeSpan timeout, HttpCompletionOption completionOption, HttpMethod method, String path, IQueryString queryString, IDictionary`2 headers, IRequestContent data, CancellationToken cancellationToken) in /_/src/Docker.DotNet/DockerClient.cs:line 433
   at Docker.DotNet.DockerClient.MakeRequestAsync[T](IEnumerable`1 errorHandlers, HttpMethod method, String path, IQueryString queryString, IRequestContent body, IDictionary`2 headers, TimeSpan timeout, CancellationToken token) in /_/src/Docker.DotNet/DockerClient.cs:line 243
   at DotNet.Testcontainers.Builders.DockerEndpointAuthenticationProvider.<>c__DisplayClass5_0.<<IsAvailable>b__0>d.MoveNext() in /_/src/Testcontainers/Builders/DockerEndpointAuthenticationProvider.cs:line 48


      QueryAsync_WithInvalidQuery_ShouldReturnFailure [0:00.000] Failed: Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
Xunit.Sdk.TestPipelineException
Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 195
   at Xunit.v3.FixtureMappingManager.InitializeAsync(IReadOnlyCollection`1 fixtureTypes, Boolean createInstances) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 248
   at Xunit.v3.ExceptionAggregator.RunAsync(Func`1 code) in /_/src/xunit.v3.core/Exceptions/ExceptionAggregator.cs:line 124

DotNet.Testcontainers.Builders.DockerUnavailableException
Docker is either not running or misconfigured. Please ensure that Docker is running and that the endpoint is properly configured.
You can customize your configuration using either the environment variables or the ~/.testcontainers.properties file.
For more information, visit: https://dotnet.testcontainers.org/custom_configuration/.
  Details: 
    Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
   at DotNet.Testcontainers.Guard.ThrowIf[TType](ArgumentInfo`1& argument, Func`2 condition, Func`2 ifClause) in /_/src/Testcontainers/Guard.Null.cs:line 62
   at DotNet.Testcontainers.Builders.AbstractBuilder`4.Validate() in /_/src/Testcontainers/Builders/AbstractBuilder`4.cs:line 146
   at DotNet.Testcontainers.Builders.ContainerBuilder`3.Validate() in /_/src/Testcontainers/Builders/ContainerBuilder`3.cs:line 408
   at Testcontainers.Qdrant.QdrantBuilder.Build() in /_/src/Testcontainers.Qdrant/QdrantBuilder.cs:line 69
   at IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture.InitializeAsync() in /_/src/test/SemanticRagTests/IndFusion.SemanticRag.System.Tests/Infrastructure/Fixtures/QdrantContainerFixture.cs:line 44
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 191

System.AggregateException
One or more errors occurred. (Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.)
  Exception doesn't have a stacktrace

DotNet.Testcontainers.Builders.DockerUnavailableException
Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
  Exception doesn't have a stacktrace

System.TimeoutException
The operation has timed out.
   at System.IO.Pipes.NamedPipeClientStream.ConnectInternal(Int32 timeout, CancellationToken cancellationToken, Int32 startTime)
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
--- End of stack trace from previous location ---
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
   at System.Threading.Tasks.Task.ExecuteWithThreadLocal(Task& currentTaskSlot, Thread threadPoolThread)
--- End of stack trace from previous location ---
   at Docker.DotNet.DockerClient.<>c__DisplayClass5_0.<<-ctor>b__0>d.MoveNext() in /_/src/Docker.DotNet/DockerClient.cs:line 69
--- End of stack trace from previous location ---
   at Microsoft.Net.Http.Client.ManagedHandler.ProcessRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 160
   at Microsoft.Net.Http.Client.ManagedHandler.SendAsync(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 77
   at System.Net.Http.HttpClient.<SendAsync>g__Core|83_0(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationTokenSource cts, Boolean disposeCts, CancellationTokenSource pendingRequestsCts, CancellationToken originalCancellationToken)
   at Docker.DotNet.DockerClient.PrivateMakeRequestAsync(TimeSpan timeout, HttpCompletionOption completionOption, HttpMethod method, String path, IQueryString queryString, IDictionary`2 headers, IRequestContent data, CancellationToken cancellationToken) in /_/src/Docker.DotNet/DockerClient.cs:line 433
   at Docker.DotNet.DockerClient.MakeRequestAsync[T](IEnumerable`1 errorHandlers, HttpMethod method, String path, IQueryString queryString, IRequestContent body, IDictionary`2 headers, TimeSpan timeout, CancellationToken token) in /_/src/Docker.DotNet/DockerClient.cs:line 243
   at DotNet.Testcontainers.Builders.DockerEndpointAuthenticationProvider.<>c__DisplayClass5_0.<<IsAvailable>b__0>d.MoveNext() in /_/src/Testcontainers/Builders/DockerEndpointAuthenticationProvider.cs:line 48


      QueryAsync_WithMultipleQueries_ShouldExecuteSequentially [0:00.000] Failed: Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
Xunit.Sdk.TestPipelineException
Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 195
   at Xunit.v3.FixtureMappingManager.InitializeAsync(IReadOnlyCollection`1 fixtureTypes, Boolean createInstances) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 248
   at Xunit.v3.ExceptionAggregator.RunAsync(Func`1 code) in /_/src/xunit.v3.core/Exceptions/ExceptionAggregator.cs:line 124

DotNet.Testcontainers.Builders.DockerUnavailableException
Docker is either not running or misconfigured. Please ensure that Docker is running and that the endpoint is properly configured.
You can customize your configuration using either the environment variables or the ~/.testcontainers.properties file.
For more information, visit: https://dotnet.testcontainers.org/custom_configuration/.
  Details: 
    Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
   at DotNet.Testcontainers.Guard.ThrowIf[TType](ArgumentInfo`1& argument, Func`2 condition, Func`2 ifClause) in /_/src/Testcontainers/Guard.Null.cs:line 62
   at DotNet.Testcontainers.Builders.AbstractBuilder`4.Validate() in /_/src/Testcontainers/Builders/AbstractBuilder`4.cs:line 146
   at DotNet.Testcontainers.Builders.ContainerBuilder`3.Validate() in /_/src/Testcontainers/Builders/ContainerBuilder`3.cs:line 408
   at Testcontainers.Qdrant.QdrantBuilder.Build() in /_/src/Testcontainers.Qdrant/QdrantBuilder.cs:line 69
   at IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture.InitializeAsync() in /_/src/test/SemanticRagTests/IndFusion.SemanticRag.System.Tests/Infrastructure/Fixtures/QdrantContainerFixture.cs:line 44
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 191

System.AggregateException
One or more errors occurred. (Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.)
  Exception doesn't have a stacktrace

DotNet.Testcontainers.Builders.DockerUnavailableException
Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
  Exception doesn't have a stacktrace

System.TimeoutException
The operation has timed out.
   at System.IO.Pipes.NamedPipeClientStream.ConnectInternal(Int32 timeout, CancellationToken cancellationToken, Int32 startTime)
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
--- End of stack trace from previous location ---
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
   at System.Threading.Tasks.Task.ExecuteWithThreadLocal(Task& currentTaskSlot, Thread threadPoolThread)
--- End of stack trace from previous location ---
   at Docker.DotNet.DockerClient.<>c__DisplayClass5_0.<<-ctor>b__0>d.MoveNext() in /_/src/Docker.DotNet/DockerClient.cs:line 69
--- End of stack trace from previous location ---
   at Microsoft.Net.Http.Client.ManagedHandler.ProcessRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 160
   at Microsoft.Net.Http.Client.ManagedHandler.SendAsync(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 77
   at System.Net.Http.HttpClient.<SendAsync>g__Core|83_0(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationTokenSource cts, Boolean disposeCts, CancellationTokenSource pendingRequestsCts, CancellationToken originalCancellationToken)
   at Docker.DotNet.DockerClient.PrivateMakeRequestAsync(TimeSpan timeout, HttpCompletionOption completionOption, HttpMethod method, String path, IQueryString queryString, IDictionary`2 headers, IRequestContent data, CancellationToken cancellationToken) in /_/src/Docker.DotNet/DockerClient.cs:line 433
   at Docker.DotNet.DockerClient.MakeRequestAsync[T](IEnumerable`1 errorHandlers, HttpMethod method, String path, IQueryString queryString, IRequestContent body, IDictionary`2 headers, TimeSpan timeout, CancellationToken token) in /_/src/Docker.DotNet/DockerClient.cs:line 243
   at DotNet.Testcontainers.Builders.DockerEndpointAuthenticationProvider.<>c__DisplayClass5_0.<<IsAvailable>b__0>d.MoveNext() in /_/src/Testcontainers/Builders/DockerEndpointAuthenticationProvider.cs:line 48


      QueryAsync_WithParameters_ShouldUseParametersInQuery [0:00.000] Failed: Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
Xunit.Sdk.TestPipelineException
Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 195
   at Xunit.v3.FixtureMappingManager.InitializeAsync(IReadOnlyCollection`1 fixtureTypes, Boolean createInstances) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 248
   at Xunit.v3.ExceptionAggregator.RunAsync(Func`1 code) in /_/src/xunit.v3.core/Exceptions/ExceptionAggregator.cs:line 124

DotNet.Testcontainers.Builders.DockerUnavailableException
Docker is either not running or misconfigured. Please ensure that Docker is running and that the endpoint is properly configured.
You can customize your configuration using either the environment variables or the ~/.testcontainers.properties file.
For more information, visit: https://dotnet.testcontainers.org/custom_configuration/.
  Details: 
    Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
   at DotNet.Testcontainers.Guard.ThrowIf[TType](ArgumentInfo`1& argument, Func`2 condition, Func`2 ifClause) in /_/src/Testcontainers/Guard.Null.cs:line 62
   at DotNet.Testcontainers.Builders.AbstractBuilder`4.Validate() in /_/src/Testcontainers/Builders/AbstractBuilder`4.cs:line 146
   at DotNet.Testcontainers.Builders.ContainerBuilder`3.Validate() in /_/src/Testcontainers/Builders/ContainerBuilder`3.cs:line 408
   at Testcontainers.Qdrant.QdrantBuilder.Build() in /_/src/Testcontainers.Qdrant/QdrantBuilder.cs:line 69
   at IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture.InitializeAsync() in /_/src/test/SemanticRagTests/IndFusion.SemanticRag.System.Tests/Infrastructure/Fixtures/QdrantContainerFixture.cs:line 44
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 191

System.AggregateException
One or more errors occurred. (Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.)
  Exception doesn't have a stacktrace

DotNet.Testcontainers.Builders.DockerUnavailableException
Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
  Exception doesn't have a stacktrace

System.TimeoutException
The operation has timed out.
   at System.IO.Pipes.NamedPipeClientStream.ConnectInternal(Int32 timeout, CancellationToken cancellationToken, Int32 startTime)
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
--- End of stack trace from previous location ---
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
   at System.Threading.Tasks.Task.ExecuteWithThreadLocal(Task& currentTaskSlot, Thread threadPoolThread)
--- End of stack trace from previous location ---
   at Docker.DotNet.DockerClient.<>c__DisplayClass5_0.<<-ctor>b__0>d.MoveNext() in /_/src/Docker.DotNet/DockerClient.cs:line 69
--- End of stack trace from previous location ---
   at Microsoft.Net.Http.Client.ManagedHandler.ProcessRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 160
   at Microsoft.Net.Http.Client.ManagedHandler.SendAsync(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 77
   at System.Net.Http.HttpClient.<SendAsync>g__Core|83_0(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationTokenSource cts, Boolean disposeCts, CancellationTokenSource pendingRequestsCts, CancellationToken originalCancellationToken)
   at Docker.DotNet.DockerClient.PrivateMakeRequestAsync(TimeSpan timeout, HttpCompletionOption completionOption, HttpMethod method, String path, IQueryString queryString, IDictionary`2 headers, IRequestContent data, CancellationToken cancellationToken) in /_/src/Docker.DotNet/DockerClient.cs:line 433
   at Docker.DotNet.DockerClient.MakeRequestAsync[T](IEnumerable`1 errorHandlers, HttpMethod method, String path, IQueryString queryString, IRequestContent body, IDictionary`2 headers, TimeSpan timeout, CancellationToken token) in /_/src/Docker.DotNet/DockerClient.cs:line 243
   at DotNet.Testcontainers.Builders.DockerEndpointAuthenticationProvider.<>c__DisplayClass5_0.<<IsAvailable>b__0>d.MoveNext() in /_/src/Testcontainers/Builders/DockerEndpointAuthenticationProvider.cs:line 48


      QueryAsync_WithPathQuery_ShouldReturnPathResults [0:00.000] Failed: Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
Xunit.Sdk.TestPipelineException
Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 195
   at Xunit.v3.FixtureMappingManager.InitializeAsync(IReadOnlyCollection`1 fixtureTypes, Boolean createInstances) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 248
   at Xunit.v3.ExceptionAggregator.RunAsync(Func`1 code) in /_/src/xunit.v3.core/Exceptions/ExceptionAggregator.cs:line 124

DotNet.Testcontainers.Builders.DockerUnavailableException
Docker is either not running or misconfigured. Please ensure that Docker is running and that the endpoint is properly configured.
You can customize your configuration using either the environment variables or the ~/.testcontainers.properties file.
For more information, visit: https://dotnet.testcontainers.org/custom_configuration/.
  Details: 
    Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
   at DotNet.Testcontainers.Guard.ThrowIf[TType](ArgumentInfo`1& argument, Func`2 condition, Func`2 ifClause) in /_/src/Testcontainers/Guard.Null.cs:line 62
   at DotNet.Testcontainers.Builders.AbstractBuilder`4.Validate() in /_/src/Testcontainers/Builders/AbstractBuilder`4.cs:line 146
   at DotNet.Testcontainers.Builders.ContainerBuilder`3.Validate() in /_/src/Testcontainers/Builders/ContainerBuilder`3.cs:line 408
   at Testcontainers.Qdrant.QdrantBuilder.Build() in /_/src/Testcontainers.Qdrant/QdrantBuilder.cs:line 69
   at IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture.InitializeAsync() in /_/src/test/SemanticRagTests/IndFusion.SemanticRag.System.Tests/Infrastructure/Fixtures/QdrantContainerFixture.cs:line 44
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 191

System.AggregateException
One or more errors occurred. (Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.)
  Exception doesn't have a stacktrace

DotNet.Testcontainers.Builders.DockerUnavailableException
Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
  Exception doesn't have a stacktrace

System.TimeoutException
The operation has timed out.
   at System.IO.Pipes.NamedPipeClientStream.ConnectInternal(Int32 timeout, CancellationToken cancellationToken, Int32 startTime)
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
--- End of stack trace from previous location ---
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
   at System.Threading.Tasks.Task.ExecuteWithThreadLocal(Task& currentTaskSlot, Thread threadPoolThread)
--- End of stack trace from previous location ---
   at Docker.DotNet.DockerClient.<>c__DisplayClass5_0.<<-ctor>b__0>d.MoveNext() in /_/src/Docker.DotNet/DockerClient.cs:line 69
--- End of stack trace from previous location ---
   at Microsoft.Net.Http.Client.ManagedHandler.ProcessRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 160
   at Microsoft.Net.Http.Client.ManagedHandler.SendAsync(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 77
   at System.Net.Http.HttpClient.<SendAsync>g__Core|83_0(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationTokenSource cts, Boolean disposeCts, CancellationTokenSource pendingRequestsCts, CancellationToken originalCancellationToken)
   at Docker.DotNet.DockerClient.PrivateMakeRequestAsync(TimeSpan timeout, HttpCompletionOption completionOption, HttpMethod method, String path, IQueryString queryString, IDictionary`2 headers, IRequestContent data, CancellationToken cancellationToken) in /_/src/Docker.DotNet/DockerClient.cs:line 433
   at Docker.DotNet.DockerClient.MakeRequestAsync[T](IEnumerable`1 errorHandlers, HttpMethod method, String path, IQueryString queryString, IRequestContent body, IDictionary`2 headers, TimeSpan timeout, CancellationToken token) in /_/src/Docker.DotNet/DockerClient.cs:line 243
   at DotNet.Testcontainers.Builders.DockerEndpointAuthenticationProvider.<>c__DisplayClass5_0.<<IsAvailable>b__0>d.MoveNext() in /_/src/Testcontainers/Builders/DockerEndpointAuthenticationProvider.cs:line 48


      QueryAsync_WithTimeout_ShouldRespectTimeout [0:00.000] Failed: Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
Xunit.Sdk.TestPipelineException
Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 195
   at Xunit.v3.FixtureMappingManager.InitializeAsync(IReadOnlyCollection`1 fixtureTypes, Boolean createInstances) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 248
   at Xunit.v3.ExceptionAggregator.RunAsync(Func`1 code) in /_/src/xunit.v3.core/Exceptions/ExceptionAggregator.cs:line 124

DotNet.Testcontainers.Builders.DockerUnavailableException
Docker is either not running or misconfigured. Please ensure that Docker is running and that the endpoint is properly configured.
You can customize your configuration using either the environment variables or the ~/.testcontainers.properties file.
For more information, visit: https://dotnet.testcontainers.org/custom_configuration/.
  Details: 
    Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
   at DotNet.Testcontainers.Guard.ThrowIf[TType](ArgumentInfo`1& argument, Func`2 condition, Func`2 ifClause) in /_/src/Testcontainers/Guard.Null.cs:line 62
   at DotNet.Testcontainers.Builders.AbstractBuilder`4.Validate() in /_/src/Testcontainers/Builders/AbstractBuilder`4.cs:line 146
   at DotNet.Testcontainers.Builders.ContainerBuilder`3.Validate() in /_/src/Testcontainers/Builders/ContainerBuilder`3.cs:line 408
   at Testcontainers.Qdrant.QdrantBuilder.Build() in /_/src/Testcontainers.Qdrant/QdrantBuilder.cs:line 69
   at IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture.InitializeAsync() in /_/src/test/SemanticRagTests/IndFusion.SemanticRag.System.Tests/Infrastructure/Fixtures/QdrantContainerFixture.cs:line 44
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 191

System.AggregateException
One or more errors occurred. (Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.)
  Exception doesn't have a stacktrace

DotNet.Testcontainers.Builders.DockerUnavailableException
Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
  Exception doesn't have a stacktrace

System.TimeoutException
The operation has timed out.
   at System.IO.Pipes.NamedPipeClientStream.ConnectInternal(Int32 timeout, CancellationToken cancellationToken, Int32 startTime)
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
--- End of stack trace from previous location ---
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
   at System.Threading.Tasks.Task.ExecuteWithThreadLocal(Task& currentTaskSlot, Thread threadPoolThread)
--- End of stack trace from previous location ---
   at Docker.DotNet.DockerClient.<>c__DisplayClass5_0.<<-ctor>b__0>d.MoveNext() in /_/src/Docker.DotNet/DockerClient.cs:line 69
--- End of stack trace from previous location ---
   at Microsoft.Net.Http.Client.ManagedHandler.ProcessRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 160
   at Microsoft.Net.Http.Client.ManagedHandler.SendAsync(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 77
   at System.Net.Http.HttpClient.<SendAsync>g__Core|83_0(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationTokenSource cts, Boolean disposeCts, CancellationTokenSource pendingRequestsCts, CancellationToken originalCancellationToken)
   at Docker.DotNet.DockerClient.PrivateMakeRequestAsync(TimeSpan timeout, HttpCompletionOption completionOption, HttpMethod method, String path, IQueryString queryString, IDictionary`2 headers, IRequestContent data, CancellationToken cancellationToken) in /_/src/Docker.DotNet/DockerClient.cs:line 433
   at Docker.DotNet.DockerClient.MakeRequestAsync[T](IEnumerable`1 errorHandlers, HttpMethod method, String path, IQueryString queryString, IRequestContent body, IDictionary`2 headers, TimeSpan timeout, CancellationToken token) in /_/src/Docker.DotNet/DockerClient.cs:line 243
   at DotNet.Testcontainers.Builders.DockerEndpointAuthenticationProvider.<>c__DisplayClass5_0.<<IsAvailable>b__0>d.MoveNext() in /_/src/Testcontainers/Builders/DockerEndpointAuthenticationProvider.cs:line 48


      QueryAsync_WithValidQuery_ShouldReturnActualResults [0:00.001] Failed: Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
Xunit.Sdk.TestPipelineException
Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 195
   at Xunit.v3.FixtureMappingManager.InitializeAsync(IReadOnlyCollection`1 fixtureTypes, Boolean createInstances) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 248
   at Xunit.v3.ExceptionAggregator.RunAsync(Func`1 code) in /_/src/xunit.v3.core/Exceptions/ExceptionAggregator.cs:line 124

DotNet.Testcontainers.Builders.DockerUnavailableException
Docker is either not running or misconfigured. Please ensure that Docker is running and that the endpoint is properly configured.
You can customize your configuration using either the environment variables or the ~/.testcontainers.properties file.
For more information, visit: https://dotnet.testcontainers.org/custom_configuration/.
  Details: 
    Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
   at DotNet.Testcontainers.Guard.ThrowIf[TType](ArgumentInfo`1& argument, Func`2 condition, Func`2 ifClause) in /_/src/Testcontainers/Guard.Null.cs:line 62
   at DotNet.Testcontainers.Builders.AbstractBuilder`4.Validate() in /_/src/Testcontainers/Builders/AbstractBuilder`4.cs:line 146
   at DotNet.Testcontainers.Builders.ContainerBuilder`3.Validate() in /_/src/Testcontainers/Builders/ContainerBuilder`3.cs:line 408
   at Testcontainers.Qdrant.QdrantBuilder.Build() in /_/src/Testcontainers.Qdrant/QdrantBuilder.cs:line 69
   at IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture.InitializeAsync() in /_/src/test/SemanticRagTests/IndFusion.SemanticRag.System.Tests/Infrastructure/Fixtures/QdrantContainerFixture.cs:line 44
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 191

System.AggregateException
One or more errors occurred. (Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.)
  Exception doesn't have a stacktrace

DotNet.Testcontainers.Builders.DockerUnavailableException
Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
  Exception doesn't have a stacktrace

System.TimeoutException
The operation has timed out.
   at System.IO.Pipes.NamedPipeClientStream.ConnectInternal(Int32 timeout, CancellationToken cancellationToken, Int32 startTime)
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
--- End of stack trace from previous location ---
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
   at System.Threading.Tasks.Task.ExecuteWithThreadLocal(Task& currentTaskSlot, Thread threadPoolThread)
--- End of stack trace from previous location ---
   at Docker.DotNet.DockerClient.<>c__DisplayClass5_0.<<-ctor>b__0>d.MoveNext() in /_/src/Docker.DotNet/DockerClient.cs:line 69
--- End of stack trace from previous location ---
   at Microsoft.Net.Http.Client.ManagedHandler.ProcessRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 160
   at Microsoft.Net.Http.Client.ManagedHandler.SendAsync(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 77
   at System.Net.Http.HttpClient.<SendAsync>g__Core|83_0(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationTokenSource cts, Boolean disposeCts, CancellationTokenSource pendingRequestsCts, CancellationToken originalCancellationToken)
   at Docker.DotNet.DockerClient.PrivateMakeRequestAsync(TimeSpan timeout, HttpCompletionOption completionOption, HttpMethod method, String path, IQueryString queryString, IDictionary`2 headers, IRequestContent data, CancellationToken cancellationToken) in /_/src/Docker.DotNet/DockerClient.cs:line 433
   at Docker.DotNet.DockerClient.MakeRequestAsync[T](IEnumerable`1 errorHandlers, HttpMethod method, String path, IQueryString queryString, IRequestContent body, IDictionary`2 headers, TimeSpan timeout, CancellationToken token) in /_/src/Docker.DotNet/DockerClient.cs:line 243
   at DotNet.Testcontainers.Builders.DockerEndpointAuthenticationProvider.<>c__DisplayClass5_0.<<IsAvailable>b__0>d.MoveNext() in /_/src/Testcontainers/Builders/DockerEndpointAuthenticationProvider.cs:line 48


      UpdateNodeAsync_WithEmptyNodeId_ShouldThrowArgumentException [0:00.000] Failed: Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
Xunit.Sdk.TestPipelineException
Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 195
   at Xunit.v3.FixtureMappingManager.InitializeAsync(IReadOnlyCollection`1 fixtureTypes, Boolean createInstances) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 248
   at Xunit.v3.ExceptionAggregator.RunAsync(Func`1 code) in /_/src/xunit.v3.core/Exceptions/ExceptionAggregator.cs:line 124

DotNet.Testcontainers.Builders.DockerUnavailableException
Docker is either not running or misconfigured. Please ensure that Docker is running and that the endpoint is properly configured.
You can customize your configuration using either the environment variables or the ~/.testcontainers.properties file.
For more information, visit: https://dotnet.testcontainers.org/custom_configuration/.
  Details: 
    Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
   at DotNet.Testcontainers.Guard.ThrowIf[TType](ArgumentInfo`1& argument, Func`2 condition, Func`2 ifClause) in /_/src/Testcontainers/Guard.Null.cs:line 62
   at DotNet.Testcontainers.Builders.AbstractBuilder`4.Validate() in /_/src/Testcontainers/Builders/AbstractBuilder`4.cs:line 146
   at DotNet.Testcontainers.Builders.ContainerBuilder`3.Validate() in /_/src/Testcontainers/Builders/ContainerBuilder`3.cs:line 408
   at Testcontainers.Qdrant.QdrantBuilder.Build() in /_/src/Testcontainers.Qdrant/QdrantBuilder.cs:line 69
   at IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture.InitializeAsync() in /_/src/test/SemanticRagTests/IndFusion.SemanticRag.System.Tests/Infrastructure/Fixtures/QdrantContainerFixture.cs:line 44
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 191

System.AggregateException
One or more errors occurred. (Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.)
  Exception doesn't have a stacktrace

DotNet.Testcontainers.Builders.DockerUnavailableException
Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
  Exception doesn't have a stacktrace

System.TimeoutException
The operation has timed out.
   at System.IO.Pipes.NamedPipeClientStream.ConnectInternal(Int32 timeout, CancellationToken cancellationToken, Int32 startTime)
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
--- End of stack trace from previous location ---
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
   at System.Threading.Tasks.Task.ExecuteWithThreadLocal(Task& currentTaskSlot, Thread threadPoolThread)
--- End of stack trace from previous location ---
   at Docker.DotNet.DockerClient.<>c__DisplayClass5_0.<<-ctor>b__0>d.MoveNext() in /_/src/Docker.DotNet/DockerClient.cs:line 69
--- End of stack trace from previous location ---
   at Microsoft.Net.Http.Client.ManagedHandler.ProcessRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 160
   at Microsoft.Net.Http.Client.ManagedHandler.SendAsync(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 77
   at System.Net.Http.HttpClient.<SendAsync>g__Core|83_0(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationTokenSource cts, Boolean disposeCts, CancellationTokenSource pendingRequestsCts, CancellationToken originalCancellationToken)
   at Docker.DotNet.DockerClient.PrivateMakeRequestAsync(TimeSpan timeout, HttpCompletionOption completionOption, HttpMethod method, String path, IQueryString queryString, IDictionary`2 headers, IRequestContent data, CancellationToken cancellationToken) in /_/src/Docker.DotNet/DockerClient.cs:line 433
   at Docker.DotNet.DockerClient.MakeRequestAsync[T](IEnumerable`1 errorHandlers, HttpMethod method, String path, IQueryString queryString, IRequestContent body, IDictionary`2 headers, TimeSpan timeout, CancellationToken token) in /_/src/Docker.DotNet/DockerClient.cs:line 243
   at DotNet.Testcontainers.Builders.DockerEndpointAuthenticationProvider.<>c__DisplayClass5_0.<<IsAvailable>b__0>d.MoveNext() in /_/src/Testcontainers/Builders/DockerEndpointAuthenticationProvider.cs:line 48


      UpdateNodeAsync_WithNullNodeId_ShouldThrowArgumentException [0:00.000] Failed: Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
Xunit.Sdk.TestPipelineException
Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 195
   at Xunit.v3.FixtureMappingManager.InitializeAsync(IReadOnlyCollection`1 fixtureTypes, Boolean createInstances) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 248
   at Xunit.v3.ExceptionAggregator.RunAsync(Func`1 code) in /_/src/xunit.v3.core/Exceptions/ExceptionAggregator.cs:line 124

DotNet.Testcontainers.Builders.DockerUnavailableException
Docker is either not running or misconfigured. Please ensure that Docker is running and that the endpoint is properly configured.
You can customize your configuration using either the environment variables or the ~/.testcontainers.properties file.
For more information, visit: https://dotnet.testcontainers.org/custom_configuration/.
  Details: 
    Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
   at DotNet.Testcontainers.Guard.ThrowIf[TType](ArgumentInfo`1& argument, Func`2 condition, Func`2 ifClause) in /_/src/Testcontainers/Guard.Null.cs:line 62
   at DotNet.Testcontainers.Builders.AbstractBuilder`4.Validate() in /_/src/Testcontainers/Builders/AbstractBuilder`4.cs:line 146
   at DotNet.Testcontainers.Builders.ContainerBuilder`3.Validate() in /_/src/Testcontainers/Builders/ContainerBuilder`3.cs:line 408
   at Testcontainers.Qdrant.QdrantBuilder.Build() in /_/src/Testcontainers.Qdrant/QdrantBuilder.cs:line 69
   at IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture.InitializeAsync() in /_/src/test/SemanticRagTests/IndFusion.SemanticRag.System.Tests/Infrastructure/Fixtures/QdrantContainerFixture.cs:line 44
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 191

System.AggregateException
One or more errors occurred. (Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.)
  Exception doesn't have a stacktrace

DotNet.Testcontainers.Builders.DockerUnavailableException
Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
  Exception doesn't have a stacktrace

System.TimeoutException
The operation has timed out.
   at System.IO.Pipes.NamedPipeClientStream.ConnectInternal(Int32 timeout, CancellationToken cancellationToken, Int32 startTime)
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
--- End of stack trace from previous location ---
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
   at System.Threading.Tasks.Task.ExecuteWithThreadLocal(Task& currentTaskSlot, Thread threadPoolThread)
--- End of stack trace from previous location ---
   at Docker.DotNet.DockerClient.<>c__DisplayClass5_0.<<-ctor>b__0>d.MoveNext() in /_/src/Docker.DotNet/DockerClient.cs:line 69
--- End of stack trace from previous location ---
   at Microsoft.Net.Http.Client.ManagedHandler.ProcessRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 160
   at Microsoft.Net.Http.Client.ManagedHandler.SendAsync(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 77
   at System.Net.Http.HttpClient.<SendAsync>g__Core|83_0(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationTokenSource cts, Boolean disposeCts, CancellationTokenSource pendingRequestsCts, CancellationToken originalCancellationToken)
   at Docker.DotNet.DockerClient.PrivateMakeRequestAsync(TimeSpan timeout, HttpCompletionOption completionOption, HttpMethod method, String path, IQueryString queryString, IDictionary`2 headers, IRequestContent data, CancellationToken cancellationToken) in /_/src/Docker.DotNet/DockerClient.cs:line 433
   at Docker.DotNet.DockerClient.MakeRequestAsync[T](IEnumerable`1 errorHandlers, HttpMethod method, String path, IQueryString queryString, IRequestContent body, IDictionary`2 headers, TimeSpan timeout, CancellationToken token) in /_/src/Docker.DotNet/DockerClient.cs:line 243
   at DotNet.Testcontainers.Builders.DockerEndpointAuthenticationProvider.<>c__DisplayClass5_0.<<IsAvailable>b__0>d.MoveNext() in /_/src/Testcontainers/Builders/DockerEndpointAuthenticationProvider.cs:line 48


      UpdateNodeAsync_WithValidNode_ShouldUpdateNode [0:00.000] Failed: Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
Xunit.Sdk.TestPipelineException
Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 195
   at Xunit.v3.FixtureMappingManager.InitializeAsync(IReadOnlyCollection`1 fixtureTypes, Boolean createInstances) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 248
   at Xunit.v3.ExceptionAggregator.RunAsync(Func`1 code) in /_/src/xunit.v3.core/Exceptions/ExceptionAggregator.cs:line 124

DotNet.Testcontainers.Builders.DockerUnavailableException
Docker is either not running or misconfigured. Please ensure that Docker is running and that the endpoint is properly configured.
You can customize your configuration using either the environment variables or the ~/.testcontainers.properties file.
For more information, visit: https://dotnet.testcontainers.org/custom_configuration/.
  Details: 
    Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
   at DotNet.Testcontainers.Guard.ThrowIf[TType](ArgumentInfo`1& argument, Func`2 condition, Func`2 ifClause) in /_/src/Testcontainers/Guard.Null.cs:line 62
   at DotNet.Testcontainers.Builders.AbstractBuilder`4.Validate() in /_/src/Testcontainers/Builders/AbstractBuilder`4.cs:line 146
   at DotNet.Testcontainers.Builders.ContainerBuilder`3.Validate() in /_/src/Testcontainers/Builders/ContainerBuilder`3.cs:line 408
   at Testcontainers.Qdrant.QdrantBuilder.Build() in /_/src/Testcontainers.Qdrant/QdrantBuilder.cs:line 69
   at IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture.InitializeAsync() in /_/src/test/SemanticRagTests/IndFusion.SemanticRag.System.Tests/Infrastructure/Fixtures/QdrantContainerFixture.cs:line 44
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 191

System.AggregateException
One or more errors occurred. (Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.)
  Exception doesn't have a stacktrace

DotNet.Testcontainers.Builders.DockerUnavailableException
Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
  Exception doesn't have a stacktrace

System.TimeoutException
The operation has timed out.
   at System.IO.Pipes.NamedPipeClientStream.ConnectInternal(Int32 timeout, CancellationToken cancellationToken, Int32 startTime)
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
--- End of stack trace from previous location ---
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
   at System.Threading.Tasks.Task.ExecuteWithThreadLocal(Task& currentTaskSlot, Thread threadPoolThread)
--- End of stack trace from previous location ---
   at Docker.DotNet.DockerClient.<>c__DisplayClass5_0.<<-ctor>b__0>d.MoveNext() in /_/src/Docker.DotNet/DockerClient.cs:line 69
--- End of stack trace from previous location ---
   at Microsoft.Net.Http.Client.ManagedHandler.ProcessRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 160
   at Microsoft.Net.Http.Client.ManagedHandler.SendAsync(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 77
   at System.Net.Http.HttpClient.<SendAsync>g__Core|83_0(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationTokenSource cts, Boolean disposeCts, CancellationTokenSource pendingRequestsCts, CancellationToken originalCancellationToken)
   at Docker.DotNet.DockerClient.PrivateMakeRequestAsync(TimeSpan timeout, HttpCompletionOption completionOption, HttpMethod method, String path, IQueryString queryString, IDictionary`2 headers, IRequestContent data, CancellationToken cancellationToken) in /_/src/Docker.DotNet/DockerClient.cs:line 433
   at Docker.DotNet.DockerClient.MakeRequestAsync[T](IEnumerable`1 errorHandlers, HttpMethod method, String path, IQueryString queryString, IRequestContent body, IDictionary`2 headers, TimeSpan timeout, CancellationToken token) in /_/src/Docker.DotNet/DockerClient.cs:line 243
   at DotNet.Testcontainers.Builders.DockerEndpointAuthenticationProvider.<>c__DisplayClass5_0.<<IsAvailable>b__0>d.MoveNext() in /_/src/Testcontainers/Builders/DockerEndpointAuthenticationProvider.cs:line 48


     QdrantVectorSearchServiceBehavioralTests (21 tests) [0:00.001] Failed: One or more child tests failed: 21 tests failed
      DeleteDocumentAsync_WithNullId_ShouldThrowArgumentException [0:00.000] Failed: Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
Xunit.Sdk.TestPipelineException
Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 195
   at Xunit.v3.FixtureMappingManager.InitializeAsync(IReadOnlyCollection`1 fixtureTypes, Boolean createInstances) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 248
   at Xunit.v3.ExceptionAggregator.RunAsync(Func`1 code) in /_/src/xunit.v3.core/Exceptions/ExceptionAggregator.cs:line 124

DotNet.Testcontainers.Builders.DockerUnavailableException
Docker is either not running or misconfigured. Please ensure that Docker is running and that the endpoint is properly configured.
You can customize your configuration using either the environment variables or the ~/.testcontainers.properties file.
For more information, visit: https://dotnet.testcontainers.org/custom_configuration/.
  Details: 
    Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
   at DotNet.Testcontainers.Guard.ThrowIf[TType](ArgumentInfo`1& argument, Func`2 condition, Func`2 ifClause) in /_/src/Testcontainers/Guard.Null.cs:line 62
   at DotNet.Testcontainers.Builders.AbstractBuilder`4.Validate() in /_/src/Testcontainers/Builders/AbstractBuilder`4.cs:line 146
   at DotNet.Testcontainers.Builders.ContainerBuilder`3.Validate() in /_/src/Testcontainers/Builders/ContainerBuilder`3.cs:line 408
   at Testcontainers.Qdrant.QdrantBuilder.Build() in /_/src/Testcontainers.Qdrant/QdrantBuilder.cs:line 69
   at IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture.InitializeAsync() in /_/src/test/SemanticRagTests/IndFusion.SemanticRag.System.Tests/Infrastructure/Fixtures/QdrantContainerFixture.cs:line 44
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 191

System.AggregateException
One or more errors occurred. (Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.)
  Exception doesn't have a stacktrace

DotNet.Testcontainers.Builders.DockerUnavailableException
Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
  Exception doesn't have a stacktrace

System.TimeoutException
The operation has timed out.
   at System.IO.Pipes.NamedPipeClientStream.ConnectInternal(Int32 timeout, CancellationToken cancellationToken, Int32 startTime)
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
--- End of stack trace from previous location ---
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
   at System.Threading.Tasks.Task.ExecuteWithThreadLocal(Task& currentTaskSlot, Thread threadPoolThread)
--- End of stack trace from previous location ---
   at Docker.DotNet.DockerClient.<>c__DisplayClass5_0.<<-ctor>b__0>d.MoveNext() in /_/src/Docker.DotNet/DockerClient.cs:line 69
--- End of stack trace from previous location ---
   at Microsoft.Net.Http.Client.ManagedHandler.ProcessRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 160
   at Microsoft.Net.Http.Client.ManagedHandler.SendAsync(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 77
   at System.Net.Http.HttpClient.<SendAsync>g__Core|83_0(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationTokenSource cts, Boolean disposeCts, CancellationTokenSource pendingRequestsCts, CancellationToken originalCancellationToken)
   at Docker.DotNet.DockerClient.PrivateMakeRequestAsync(TimeSpan timeout, HttpCompletionOption completionOption, HttpMethod method, String path, IQueryString queryString, IDictionary`2 headers, IRequestContent data, CancellationToken cancellationToken) in /_/src/Docker.DotNet/DockerClient.cs:line 433
   at Docker.DotNet.DockerClient.MakeRequestAsync[T](IEnumerable`1 errorHandlers, HttpMethod method, String path, IQueryString queryString, IRequestContent body, IDictionary`2 headers, TimeSpan timeout, CancellationToken token) in /_/src/Docker.DotNet/DockerClient.cs:line 243
   at DotNet.Testcontainers.Builders.DockerEndpointAuthenticationProvider.<>c__DisplayClass5_0.<<IsAvailable>b__0>d.MoveNext() in /_/src/Testcontainers/Builders/DockerEndpointAuthenticationProvider.cs:line 48


      DeleteDocumentAsync_WithValidId_ShouldDeleteDocument [0:00.000] Failed: Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
Xunit.Sdk.TestPipelineException
Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 195
   at Xunit.v3.FixtureMappingManager.InitializeAsync(IReadOnlyCollection`1 fixtureTypes, Boolean createInstances) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 248
   at Xunit.v3.ExceptionAggregator.RunAsync(Func`1 code) in /_/src/xunit.v3.core/Exceptions/ExceptionAggregator.cs:line 124

DotNet.Testcontainers.Builders.DockerUnavailableException
Docker is either not running or misconfigured. Please ensure that Docker is running and that the endpoint is properly configured.
You can customize your configuration using either the environment variables or the ~/.testcontainers.properties file.
For more information, visit: https://dotnet.testcontainers.org/custom_configuration/.
  Details: 
    Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
   at DotNet.Testcontainers.Guard.ThrowIf[TType](ArgumentInfo`1& argument, Func`2 condition, Func`2 ifClause) in /_/src/Testcontainers/Guard.Null.cs:line 62
   at DotNet.Testcontainers.Builders.AbstractBuilder`4.Validate() in /_/src/Testcontainers/Builders/AbstractBuilder`4.cs:line 146
   at DotNet.Testcontainers.Builders.ContainerBuilder`3.Validate() in /_/src/Testcontainers/Builders/ContainerBuilder`3.cs:line 408
   at Testcontainers.Qdrant.QdrantBuilder.Build() in /_/src/Testcontainers.Qdrant/QdrantBuilder.cs:line 69
   at IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture.InitializeAsync() in /_/src/test/SemanticRagTests/IndFusion.SemanticRag.System.Tests/Infrastructure/Fixtures/QdrantContainerFixture.cs:line 44
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 191

System.AggregateException
One or more errors occurred. (Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.)
  Exception doesn't have a stacktrace

DotNet.Testcontainers.Builders.DockerUnavailableException
Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
  Exception doesn't have a stacktrace

System.TimeoutException
The operation has timed out.
   at System.IO.Pipes.NamedPipeClientStream.ConnectInternal(Int32 timeout, CancellationToken cancellationToken, Int32 startTime)
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
--- End of stack trace from previous location ---
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
   at System.Threading.Tasks.Task.ExecuteWithThreadLocal(Task& currentTaskSlot, Thread threadPoolThread)
--- End of stack trace from previous location ---
   at Docker.DotNet.DockerClient.<>c__DisplayClass5_0.<<-ctor>b__0>d.MoveNext() in /_/src/Docker.DotNet/DockerClient.cs:line 69
--- End of stack trace from previous location ---
   at Microsoft.Net.Http.Client.ManagedHandler.ProcessRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 160
   at Microsoft.Net.Http.Client.ManagedHandler.SendAsync(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 77
   at System.Net.Http.HttpClient.<SendAsync>g__Core|83_0(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationTokenSource cts, Boolean disposeCts, CancellationTokenSource pendingRequestsCts, CancellationToken originalCancellationToken)
   at Docker.DotNet.DockerClient.PrivateMakeRequestAsync(TimeSpan timeout, HttpCompletionOption completionOption, HttpMethod method, String path, IQueryString queryString, IDictionary`2 headers, IRequestContent data, CancellationToken cancellationToken) in /_/src/Docker.DotNet/DockerClient.cs:line 433
   at Docker.DotNet.DockerClient.MakeRequestAsync[T](IEnumerable`1 errorHandlers, HttpMethod method, String path, IQueryString queryString, IRequestContent body, IDictionary`2 headers, TimeSpan timeout, CancellationToken token) in /_/src/Docker.DotNet/DockerClient.cs:line 243
   at DotNet.Testcontainers.Builders.DockerEndpointAuthenticationProvider.<>c__DisplayClass5_0.<<IsAvailable>b__0>d.MoveNext() in /_/src/Testcontainers/Builders/DockerEndpointAuthenticationProvider.cs:line 48


      GenerateEmbeddingAsync_WithEmptyText_ShouldThrowArgumentException [0:00.000] Failed: Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
Xunit.Sdk.TestPipelineException
Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 195
   at Xunit.v3.FixtureMappingManager.InitializeAsync(IReadOnlyCollection`1 fixtureTypes, Boolean createInstances) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 248
   at Xunit.v3.ExceptionAggregator.RunAsync(Func`1 code) in /_/src/xunit.v3.core/Exceptions/ExceptionAggregator.cs:line 124

DotNet.Testcontainers.Builders.DockerUnavailableException
Docker is either not running or misconfigured. Please ensure that Docker is running and that the endpoint is properly configured.
You can customize your configuration using either the environment variables or the ~/.testcontainers.properties file.
For more information, visit: https://dotnet.testcontainers.org/custom_configuration/.
  Details: 
    Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
   at DotNet.Testcontainers.Guard.ThrowIf[TType](ArgumentInfo`1& argument, Func`2 condition, Func`2 ifClause) in /_/src/Testcontainers/Guard.Null.cs:line 62
   at DotNet.Testcontainers.Builders.AbstractBuilder`4.Validate() in /_/src/Testcontainers/Builders/AbstractBuilder`4.cs:line 146
   at DotNet.Testcontainers.Builders.ContainerBuilder`3.Validate() in /_/src/Testcontainers/Builders/ContainerBuilder`3.cs:line 408
   at Testcontainers.Qdrant.QdrantBuilder.Build() in /_/src/Testcontainers.Qdrant/QdrantBuilder.cs:line 69
   at IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture.InitializeAsync() in /_/src/test/SemanticRagTests/IndFusion.SemanticRag.System.Tests/Infrastructure/Fixtures/QdrantContainerFixture.cs:line 44
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 191

System.AggregateException
One or more errors occurred. (Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.)
  Exception doesn't have a stacktrace

DotNet.Testcontainers.Builders.DockerUnavailableException
Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
  Exception doesn't have a stacktrace

System.TimeoutException
The operation has timed out.
   at System.IO.Pipes.NamedPipeClientStream.ConnectInternal(Int32 timeout, CancellationToken cancellationToken, Int32 startTime)
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
--- End of stack trace from previous location ---
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
   at System.Threading.Tasks.Task.ExecuteWithThreadLocal(Task& currentTaskSlot, Thread threadPoolThread)
--- End of stack trace from previous location ---
   at Docker.DotNet.DockerClient.<>c__DisplayClass5_0.<<-ctor>b__0>d.MoveNext() in /_/src/Docker.DotNet/DockerClient.cs:line 69
--- End of stack trace from previous location ---
   at Microsoft.Net.Http.Client.ManagedHandler.ProcessRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 160
   at Microsoft.Net.Http.Client.ManagedHandler.SendAsync(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 77
   at System.Net.Http.HttpClient.<SendAsync>g__Core|83_0(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationTokenSource cts, Boolean disposeCts, CancellationTokenSource pendingRequestsCts, CancellationToken originalCancellationToken)
   at Docker.DotNet.DockerClient.PrivateMakeRequestAsync(TimeSpan timeout, HttpCompletionOption completionOption, HttpMethod method, String path, IQueryString queryString, IDictionary`2 headers, IRequestContent data, CancellationToken cancellationToken) in /_/src/Docker.DotNet/DockerClient.cs:line 433
   at Docker.DotNet.DockerClient.MakeRequestAsync[T](IEnumerable`1 errorHandlers, HttpMethod method, String path, IQueryString queryString, IRequestContent body, IDictionary`2 headers, TimeSpan timeout, CancellationToken token) in /_/src/Docker.DotNet/DockerClient.cs:line 243
   at DotNet.Testcontainers.Builders.DockerEndpointAuthenticationProvider.<>c__DisplayClass5_0.<<IsAvailable>b__0>d.MoveNext() in /_/src/Testcontainers/Builders/DockerEndpointAuthenticationProvider.cs:line 48


      GenerateEmbeddingAsync_WithValidText_ShouldReturnActualEmbedding [0:00.000] Failed: Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
Xunit.Sdk.TestPipelineException
Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 195
   at Xunit.v3.FixtureMappingManager.InitializeAsync(IReadOnlyCollection`1 fixtureTypes, Boolean createInstances) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 248
   at Xunit.v3.ExceptionAggregator.RunAsync(Func`1 code) in /_/src/xunit.v3.core/Exceptions/ExceptionAggregator.cs:line 124

DotNet.Testcontainers.Builders.DockerUnavailableException
Docker is either not running or misconfigured. Please ensure that Docker is running and that the endpoint is properly configured.
You can customize your configuration using either the environment variables or the ~/.testcontainers.properties file.
For more information, visit: https://dotnet.testcontainers.org/custom_configuration/.
  Details: 
    Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
   at DotNet.Testcontainers.Guard.ThrowIf[TType](ArgumentInfo`1& argument, Func`2 condition, Func`2 ifClause) in /_/src/Testcontainers/Guard.Null.cs:line 62
   at DotNet.Testcontainers.Builders.AbstractBuilder`4.Validate() in /_/src/Testcontainers/Builders/AbstractBuilder`4.cs:line 146
   at DotNet.Testcontainers.Builders.ContainerBuilder`3.Validate() in /_/src/Testcontainers/Builders/ContainerBuilder`3.cs:line 408
   at Testcontainers.Qdrant.QdrantBuilder.Build() in /_/src/Testcontainers.Qdrant/QdrantBuilder.cs:line 69
   at IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture.InitializeAsync() in /_/src/test/SemanticRagTests/IndFusion.SemanticRag.System.Tests/Infrastructure/Fixtures/QdrantContainerFixture.cs:line 44
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 191

System.AggregateException
One or more errors occurred. (Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.)
  Exception doesn't have a stacktrace

DotNet.Testcontainers.Builders.DockerUnavailableException
Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
  Exception doesn't have a stacktrace

System.TimeoutException
The operation has timed out.
   at System.IO.Pipes.NamedPipeClientStream.ConnectInternal(Int32 timeout, CancellationToken cancellationToken, Int32 startTime)
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
--- End of stack trace from previous location ---
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
   at System.Threading.Tasks.Task.ExecuteWithThreadLocal(Task& currentTaskSlot, Thread threadPoolThread)
--- End of stack trace from previous location ---
   at Docker.DotNet.DockerClient.<>c__DisplayClass5_0.<<-ctor>b__0>d.MoveNext() in /_/src/Docker.DotNet/DockerClient.cs:line 69
--- End of stack trace from previous location ---
   at Microsoft.Net.Http.Client.ManagedHandler.ProcessRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 160
   at Microsoft.Net.Http.Client.ManagedHandler.SendAsync(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 77
   at System.Net.Http.HttpClient.<SendAsync>g__Core|83_0(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationTokenSource cts, Boolean disposeCts, CancellationTokenSource pendingRequestsCts, CancellationToken originalCancellationToken)
   at Docker.DotNet.DockerClient.PrivateMakeRequestAsync(TimeSpan timeout, HttpCompletionOption completionOption, HttpMethod method, String path, IQueryString queryString, IDictionary`2 headers, IRequestContent data, CancellationToken cancellationToken) in /_/src/Docker.DotNet/DockerClient.cs:line 433
   at Docker.DotNet.DockerClient.MakeRequestAsync[T](IEnumerable`1 errorHandlers, HttpMethod method, String path, IQueryString queryString, IRequestContent body, IDictionary`2 headers, TimeSpan timeout, CancellationToken token) in /_/src/Docker.DotNet/DockerClient.cs:line 243
   at DotNet.Testcontainers.Builders.DockerEndpointAuthenticationProvider.<>c__DisplayClass5_0.<<IsAvailable>b__0>d.MoveNext() in /_/src/Testcontainers/Builders/DockerEndpointAuthenticationProvider.cs:line 48


      SearchSimilarAsync_WithActualResults_ShouldReturnNonEmptyResults [0:00.000] Failed: Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
Xunit.Sdk.TestPipelineException
Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 195
   at Xunit.v3.FixtureMappingManager.InitializeAsync(IReadOnlyCollection`1 fixtureTypes, Boolean createInstances) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 248
   at Xunit.v3.ExceptionAggregator.RunAsync(Func`1 code) in /_/src/xunit.v3.core/Exceptions/ExceptionAggregator.cs:line 124

DotNet.Testcontainers.Builders.DockerUnavailableException
Docker is either not running or misconfigured. Please ensure that Docker is running and that the endpoint is properly configured.
You can customize your configuration using either the environment variables or the ~/.testcontainers.properties file.
For more information, visit: https://dotnet.testcontainers.org/custom_configuration/.
  Details: 
    Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
   at DotNet.Testcontainers.Guard.ThrowIf[TType](ArgumentInfo`1& argument, Func`2 condition, Func`2 ifClause) in /_/src/Testcontainers/Guard.Null.cs:line 62
   at DotNet.Testcontainers.Builders.AbstractBuilder`4.Validate() in /_/src/Testcontainers/Builders/AbstractBuilder`4.cs:line 146
   at DotNet.Testcontainers.Builders.ContainerBuilder`3.Validate() in /_/src/Testcontainers/Builders/ContainerBuilder`3.cs:line 408
   at Testcontainers.Qdrant.QdrantBuilder.Build() in /_/src/Testcontainers.Qdrant/QdrantBuilder.cs:line 69
   at IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture.InitializeAsync() in /_/src/test/SemanticRagTests/IndFusion.SemanticRag.System.Tests/Infrastructure/Fixtures/QdrantContainerFixture.cs:line 44
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 191

System.AggregateException
One or more errors occurred. (Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.)
  Exception doesn't have a stacktrace

DotNet.Testcontainers.Builders.DockerUnavailableException
Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
  Exception doesn't have a stacktrace

System.TimeoutException
The operation has timed out.
   at System.IO.Pipes.NamedPipeClientStream.ConnectInternal(Int32 timeout, CancellationToken cancellationToken, Int32 startTime)
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
--- End of stack trace from previous location ---
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
   at System.Threading.Tasks.Task.ExecuteWithThreadLocal(Task& currentTaskSlot, Thread threadPoolThread)
--- End of stack trace from previous location ---
   at Docker.DotNet.DockerClient.<>c__DisplayClass5_0.<<-ctor>b__0>d.MoveNext() in /_/src/Docker.DotNet/DockerClient.cs:line 69
--- End of stack trace from previous location ---
   at Microsoft.Net.Http.Client.ManagedHandler.ProcessRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 160
   at Microsoft.Net.Http.Client.ManagedHandler.SendAsync(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 77
   at System.Net.Http.HttpClient.<SendAsync>g__Core|83_0(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationTokenSource cts, Boolean disposeCts, CancellationTokenSource pendingRequestsCts, CancellationToken originalCancellationToken)
   at Docker.DotNet.DockerClient.PrivateMakeRequestAsync(TimeSpan timeout, HttpCompletionOption completionOption, HttpMethod method, String path, IQueryString queryString, IDictionary`2 headers, IRequestContent data, CancellationToken cancellationToken) in /_/src/Docker.DotNet/DockerClient.cs:line 433
   at Docker.DotNet.DockerClient.MakeRequestAsync[T](IEnumerable`1 errorHandlers, HttpMethod method, String path, IQueryString queryString, IRequestContent body, IDictionary`2 headers, TimeSpan timeout, CancellationToken token) in /_/src/Docker.DotNet/DockerClient.cs:line 243
   at DotNet.Testcontainers.Builders.DockerEndpointAuthenticationProvider.<>c__DisplayClass5_0.<<IsAvailable>b__0>d.MoveNext() in /_/src/Testcontainers/Builders/DockerEndpointAuthenticationProvider.cs:line 48


      SearchSimilarAsync_WithBroadOptions_ShouldUseCorrectLimit [0:00.000] Failed: Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
Xunit.Sdk.TestPipelineException
Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 195
   at Xunit.v3.FixtureMappingManager.InitializeAsync(IReadOnlyCollection`1 fixtureTypes, Boolean createInstances) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 248
   at Xunit.v3.ExceptionAggregator.RunAsync(Func`1 code) in /_/src/xunit.v3.core/Exceptions/ExceptionAggregator.cs:line 124

DotNet.Testcontainers.Builders.DockerUnavailableException
Docker is either not running or misconfigured. Please ensure that Docker is running and that the endpoint is properly configured.
You can customize your configuration using either the environment variables or the ~/.testcontainers.properties file.
For more information, visit: https://dotnet.testcontainers.org/custom_configuration/.
  Details: 
    Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
   at DotNet.Testcontainers.Guard.ThrowIf[TType](ArgumentInfo`1& argument, Func`2 condition, Func`2 ifClause) in /_/src/Testcontainers/Guard.Null.cs:line 62
   at DotNet.Testcontainers.Builders.AbstractBuilder`4.Validate() in /_/src/Testcontainers/Builders/AbstractBuilder`4.cs:line 146
   at DotNet.Testcontainers.Builders.ContainerBuilder`3.Validate() in /_/src/Testcontainers/Builders/ContainerBuilder`3.cs:line 408
   at Testcontainers.Qdrant.QdrantBuilder.Build() in /_/src/Testcontainers.Qdrant/QdrantBuilder.cs:line 69
   at IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture.InitializeAsync() in /_/src/test/SemanticRagTests/IndFusion.SemanticRag.System.Tests/Infrastructure/Fixtures/QdrantContainerFixture.cs:line 44
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 191

System.AggregateException
One or more errors occurred. (Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.)
  Exception doesn't have a stacktrace

DotNet.Testcontainers.Builders.DockerUnavailableException
Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
  Exception doesn't have a stacktrace

System.TimeoutException
The operation has timed out.
   at System.IO.Pipes.NamedPipeClientStream.ConnectInternal(Int32 timeout, CancellationToken cancellationToken, Int32 startTime)
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
--- End of stack trace from previous location ---
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
   at System.Threading.Tasks.Task.ExecuteWithThreadLocal(Task& currentTaskSlot, Thread threadPoolThread)
--- End of stack trace from previous location ---
   at Docker.DotNet.DockerClient.<>c__DisplayClass5_0.<<-ctor>b__0>d.MoveNext() in /_/src/Docker.DotNet/DockerClient.cs:line 69
--- End of stack trace from previous location ---
   at Microsoft.Net.Http.Client.ManagedHandler.ProcessRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 160
   at Microsoft.Net.Http.Client.ManagedHandler.SendAsync(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 77
   at System.Net.Http.HttpClient.<SendAsync>g__Core|83_0(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationTokenSource cts, Boolean disposeCts, CancellationTokenSource pendingRequestsCts, CancellationToken originalCancellationToken)
   at Docker.DotNet.DockerClient.PrivateMakeRequestAsync(TimeSpan timeout, HttpCompletionOption completionOption, HttpMethod method, String path, IQueryString queryString, IDictionary`2 headers, IRequestContent data, CancellationToken cancellationToken) in /_/src/Docker.DotNet/DockerClient.cs:line 433
   at Docker.DotNet.DockerClient.MakeRequestAsync[T](IEnumerable`1 errorHandlers, HttpMethod method, String path, IQueryString queryString, IRequestContent body, IDictionary`2 headers, TimeSpan timeout, CancellationToken token) in /_/src/Docker.DotNet/DockerClient.cs:line 243
   at DotNet.Testcontainers.Builders.DockerEndpointAuthenticationProvider.<>c__DisplayClass5_0.<<IsAvailable>b__0>d.MoveNext() in /_/src/Testcontainers/Builders/DockerEndpointAuthenticationProvider.cs:line 48


      SearchSimilarAsync_WithCancellation_ShouldRespectCancellationToken [0:00.000] Failed: Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
Xunit.Sdk.TestPipelineException
Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 195
   at Xunit.v3.FixtureMappingManager.InitializeAsync(IReadOnlyCollection`1 fixtureTypes, Boolean createInstances) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 248
   at Xunit.v3.ExceptionAggregator.RunAsync(Func`1 code) in /_/src/xunit.v3.core/Exceptions/ExceptionAggregator.cs:line 124

DotNet.Testcontainers.Builders.DockerUnavailableException
Docker is either not running or misconfigured. Please ensure that Docker is running and that the endpoint is properly configured.
You can customize your configuration using either the environment variables or the ~/.testcontainers.properties file.
For more information, visit: https://dotnet.testcontainers.org/custom_configuration/.
  Details: 
    Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
   at DotNet.Testcontainers.Guard.ThrowIf[TType](ArgumentInfo`1& argument, Func`2 condition, Func`2 ifClause) in /_/src/Testcontainers/Guard.Null.cs:line 62
   at DotNet.Testcontainers.Builders.AbstractBuilder`4.Validate() in /_/src/Testcontainers/Builders/AbstractBuilder`4.cs:line 146
   at DotNet.Testcontainers.Builders.ContainerBuilder`3.Validate() in /_/src/Testcontainers/Builders/ContainerBuilder`3.cs:line 408
   at Testcontainers.Qdrant.QdrantBuilder.Build() in /_/src/Testcontainers.Qdrant/QdrantBuilder.cs:line 69
   at IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture.InitializeAsync() in /_/src/test/SemanticRagTests/IndFusion.SemanticRag.System.Tests/Infrastructure/Fixtures/QdrantContainerFixture.cs:line 44
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 191

System.AggregateException
One or more errors occurred. (Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.)
  Exception doesn't have a stacktrace

DotNet.Testcontainers.Builders.DockerUnavailableException
Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
  Exception doesn't have a stacktrace

System.TimeoutException
The operation has timed out.
   at System.IO.Pipes.NamedPipeClientStream.ConnectInternal(Int32 timeout, CancellationToken cancellationToken, Int32 startTime)
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
--- End of stack trace from previous location ---
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
   at System.Threading.Tasks.Task.ExecuteWithThreadLocal(Task& currentTaskSlot, Thread threadPoolThread)
--- End of stack trace from previous location ---
   at Docker.DotNet.DockerClient.<>c__DisplayClass5_0.<<-ctor>b__0>d.MoveNext() in /_/src/Docker.DotNet/DockerClient.cs:line 69
--- End of stack trace from previous location ---
   at Microsoft.Net.Http.Client.ManagedHandler.ProcessRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 160
   at Microsoft.Net.Http.Client.ManagedHandler.SendAsync(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 77
   at System.Net.Http.HttpClient.<SendAsync>g__Core|83_0(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationTokenSource cts, Boolean disposeCts, CancellationTokenSource pendingRequestsCts, CancellationToken originalCancellationToken)
   at Docker.DotNet.DockerClient.PrivateMakeRequestAsync(TimeSpan timeout, HttpCompletionOption completionOption, HttpMethod method, String path, IQueryString queryString, IDictionary`2 headers, IRequestContent data, CancellationToken cancellationToken) in /_/src/Docker.DotNet/DockerClient.cs:line 433
   at Docker.DotNet.DockerClient.MakeRequestAsync[T](IEnumerable`1 errorHandlers, HttpMethod method, String path, IQueryString queryString, IRequestContent body, IDictionary`2 headers, TimeSpan timeout, CancellationToken token) in /_/src/Docker.DotNet/DockerClient.cs:line 243
   at DotNet.Testcontainers.Builders.DockerEndpointAuthenticationProvider.<>c__DisplayClass5_0.<<IsAvailable>b__0>d.MoveNext() in /_/src/Testcontainers/Builders/DockerEndpointAuthenticationProvider.cs:line 48


      SearchSimilarAsync_WithEmbeddingServiceFailure_ShouldPropagateFailure [0:00.000] Failed: Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
Xunit.Sdk.TestPipelineException
Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 195
   at Xunit.v3.FixtureMappingManager.InitializeAsync(IReadOnlyCollection`1 fixtureTypes, Boolean createInstances) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 248
   at Xunit.v3.ExceptionAggregator.RunAsync(Func`1 code) in /_/src/xunit.v3.core/Exceptions/ExceptionAggregator.cs:line 124

DotNet.Testcontainers.Builders.DockerUnavailableException
Docker is either not running or misconfigured. Please ensure that Docker is running and that the endpoint is properly configured.
You can customize your configuration using either the environment variables or the ~/.testcontainers.properties file.
For more information, visit: https://dotnet.testcontainers.org/custom_configuration/.
  Details: 
    Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
   at DotNet.Testcontainers.Guard.ThrowIf[TType](ArgumentInfo`1& argument, Func`2 condition, Func`2 ifClause) in /_/src/Testcontainers/Guard.Null.cs:line 62
   at DotNet.Testcontainers.Builders.AbstractBuilder`4.Validate() in /_/src/Testcontainers/Builders/AbstractBuilder`4.cs:line 146
   at DotNet.Testcontainers.Builders.ContainerBuilder`3.Validate() in /_/src/Testcontainers/Builders/ContainerBuilder`3.cs:line 408
   at Testcontainers.Qdrant.QdrantBuilder.Build() in /_/src/Testcontainers.Qdrant/QdrantBuilder.cs:line 69
   at IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture.InitializeAsync() in /_/src/test/SemanticRagTests/IndFusion.SemanticRag.System.Tests/Infrastructure/Fixtures/QdrantContainerFixture.cs:line 44
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 191

System.AggregateException
One or more errors occurred. (Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.)
  Exception doesn't have a stacktrace

DotNet.Testcontainers.Builders.DockerUnavailableException
Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
  Exception doesn't have a stacktrace

System.TimeoutException
The operation has timed out.
   at System.IO.Pipes.NamedPipeClientStream.ConnectInternal(Int32 timeout, CancellationToken cancellationToken, Int32 startTime)
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
--- End of stack trace from previous location ---
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
   at System.Threading.Tasks.Task.ExecuteWithThreadLocal(Task& currentTaskSlot, Thread threadPoolThread)
--- End of stack trace from previous location ---
   at Docker.DotNet.DockerClient.<>c__DisplayClass5_0.<<-ctor>b__0>d.MoveNext() in /_/src/Docker.DotNet/DockerClient.cs:line 69
--- End of stack trace from previous location ---
   at Microsoft.Net.Http.Client.ManagedHandler.ProcessRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 160
   at Microsoft.Net.Http.Client.ManagedHandler.SendAsync(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 77
   at System.Net.Http.HttpClient.<SendAsync>g__Core|83_0(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationTokenSource cts, Boolean disposeCts, CancellationTokenSource pendingRequestsCts, CancellationToken originalCancellationToken)
   at Docker.DotNet.DockerClient.PrivateMakeRequestAsync(TimeSpan timeout, HttpCompletionOption completionOption, HttpMethod method, String path, IQueryString queryString, IDictionary`2 headers, IRequestContent data, CancellationToken cancellationToken) in /_/src/Docker.DotNet/DockerClient.cs:line 433
   at Docker.DotNet.DockerClient.MakeRequestAsync[T](IEnumerable`1 errorHandlers, HttpMethod method, String path, IQueryString queryString, IRequestContent body, IDictionary`2 headers, TimeSpan timeout, CancellationToken token) in /_/src/Docker.DotNet/DockerClient.cs:line 243
   at DotNet.Testcontainers.Builders.DockerEndpointAuthenticationProvider.<>c__DisplayClass5_0.<<IsAvailable>b__0>d.MoveNext() in /_/src/Testcontainers/Builders/DockerEndpointAuthenticationProvider.cs:line 48


      SearchSimilarAsync_WithHighPrecisionOptions_ShouldUseCorrectThreshold [0:00.000] Failed: Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
Xunit.Sdk.TestPipelineException
Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 195
   at Xunit.v3.FixtureMappingManager.InitializeAsync(IReadOnlyCollection`1 fixtureTypes, Boolean createInstances) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 248
   at Xunit.v3.ExceptionAggregator.RunAsync(Func`1 code) in /_/src/xunit.v3.core/Exceptions/ExceptionAggregator.cs:line 124

DotNet.Testcontainers.Builders.DockerUnavailableException
Docker is either not running or misconfigured. Please ensure that Docker is running and that the endpoint is properly configured.
You can customize your configuration using either the environment variables or the ~/.testcontainers.properties file.
For more information, visit: https://dotnet.testcontainers.org/custom_configuration/.
  Details: 
    Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
   at DotNet.Testcontainers.Guard.ThrowIf[TType](ArgumentInfo`1& argument, Func`2 condition, Func`2 ifClause) in /_/src/Testcontainers/Guard.Null.cs:line 62
   at DotNet.Testcontainers.Builders.AbstractBuilder`4.Validate() in /_/src/Testcontainers/Builders/AbstractBuilder`4.cs:line 146
   at DotNet.Testcontainers.Builders.ContainerBuilder`3.Validate() in /_/src/Testcontainers/Builders/ContainerBuilder`3.cs:line 408
   at Testcontainers.Qdrant.QdrantBuilder.Build() in /_/src/Testcontainers.Qdrant/QdrantBuilder.cs:line 69
   at IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture.InitializeAsync() in /_/src/test/SemanticRagTests/IndFusion.SemanticRag.System.Tests/Infrastructure/Fixtures/QdrantContainerFixture.cs:line 44
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 191

System.AggregateException
One or more errors occurred. (Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.)
  Exception doesn't have a stacktrace

DotNet.Testcontainers.Builders.DockerUnavailableException
Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
  Exception doesn't have a stacktrace

System.TimeoutException
The operation has timed out.
   at System.IO.Pipes.NamedPipeClientStream.ConnectInternal(Int32 timeout, CancellationToken cancellationToken, Int32 startTime)
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
--- End of stack trace from previous location ---
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
   at System.Threading.Tasks.Task.ExecuteWithThreadLocal(Task& currentTaskSlot, Thread threadPoolThread)
--- End of stack trace from previous location ---
   at Docker.DotNet.DockerClient.<>c__DisplayClass5_0.<<-ctor>b__0>d.MoveNext() in /_/src/Docker.DotNet/DockerClient.cs:line 69
--- End of stack trace from previous location ---
   at Microsoft.Net.Http.Client.ManagedHandler.ProcessRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 160
   at Microsoft.Net.Http.Client.ManagedHandler.SendAsync(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 77
   at System.Net.Http.HttpClient.<SendAsync>g__Core|83_0(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationTokenSource cts, Boolean disposeCts, CancellationTokenSource pendingRequestsCts, CancellationToken originalCancellationToken)
   at Docker.DotNet.DockerClient.PrivateMakeRequestAsync(TimeSpan timeout, HttpCompletionOption completionOption, HttpMethod method, String path, IQueryString queryString, IDictionary`2 headers, IRequestContent data, CancellationToken cancellationToken) in /_/src/Docker.DotNet/DockerClient.cs:line 433
   at Docker.DotNet.DockerClient.MakeRequestAsync[T](IEnumerable`1 errorHandlers, HttpMethod method, String path, IQueryString queryString, IRequestContent body, IDictionary`2 headers, TimeSpan timeout, CancellationToken token) in /_/src/Docker.DotNet/DockerClient.cs:line 243
   at DotNet.Testcontainers.Builders.DockerEndpointAuthenticationProvider.<>c__DisplayClass5_0.<<IsAvailable>b__0>d.MoveNext() in /_/src/Testcontainers/Builders/DockerEndpointAuthenticationProvider.cs:line 48


      SearchSimilarAsync_WithIncludeEmbedding_ShouldIncludeEmbeddingInResults [0:00.000] Failed: Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
Xunit.Sdk.TestPipelineException
Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 195
   at Xunit.v3.FixtureMappingManager.InitializeAsync(IReadOnlyCollection`1 fixtureTypes, Boolean createInstances) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 248
   at Xunit.v3.ExceptionAggregator.RunAsync(Func`1 code) in /_/src/xunit.v3.core/Exceptions/ExceptionAggregator.cs:line 124

DotNet.Testcontainers.Builders.DockerUnavailableException
Docker is either not running or misconfigured. Please ensure that Docker is running and that the endpoint is properly configured.
You can customize your configuration using either the environment variables or the ~/.testcontainers.properties file.
For more information, visit: https://dotnet.testcontainers.org/custom_configuration/.
  Details: 
    Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
   at DotNet.Testcontainers.Guard.ThrowIf[TType](ArgumentInfo`1& argument, Func`2 condition, Func`2 ifClause) in /_/src/Testcontainers/Guard.Null.cs:line 62
   at DotNet.Testcontainers.Builders.AbstractBuilder`4.Validate() in /_/src/Testcontainers/Builders/AbstractBuilder`4.cs:line 146
   at DotNet.Testcontainers.Builders.ContainerBuilder`3.Validate() in /_/src/Testcontainers/Builders/ContainerBuilder`3.cs:line 408
   at Testcontainers.Qdrant.QdrantBuilder.Build() in /_/src/Testcontainers.Qdrant/QdrantBuilder.cs:line 69
   at IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture.InitializeAsync() in /_/src/test/SemanticRagTests/IndFusion.SemanticRag.System.Tests/Infrastructure/Fixtures/QdrantContainerFixture.cs:line 44
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 191

System.AggregateException
One or more errors occurred. (Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.)
  Exception doesn't have a stacktrace

DotNet.Testcontainers.Builders.DockerUnavailableException
Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
  Exception doesn't have a stacktrace

System.TimeoutException
The operation has timed out.
   at System.IO.Pipes.NamedPipeClientStream.ConnectInternal(Int32 timeout, CancellationToken cancellationToken, Int32 startTime)
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
--- End of stack trace from previous location ---
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
   at System.Threading.Tasks.Task.ExecuteWithThreadLocal(Task& currentTaskSlot, Thread threadPoolThread)
--- End of stack trace from previous location ---
   at Docker.DotNet.DockerClient.<>c__DisplayClass5_0.<<-ctor>b__0>d.MoveNext() in /_/src/Docker.DotNet/DockerClient.cs:line 69
--- End of stack trace from previous location ---
   at Microsoft.Net.Http.Client.ManagedHandler.ProcessRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 160
   at Microsoft.Net.Http.Client.ManagedHandler.SendAsync(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 77
   at System.Net.Http.HttpClient.<SendAsync>g__Core|83_0(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationTokenSource cts, Boolean disposeCts, CancellationTokenSource pendingRequestsCts, CancellationToken originalCancellationToken)
   at Docker.DotNet.DockerClient.PrivateMakeRequestAsync(TimeSpan timeout, HttpCompletionOption completionOption, HttpMethod method, String path, IQueryString queryString, IDictionary`2 headers, IRequestContent data, CancellationToken cancellationToken) in /_/src/Docker.DotNet/DockerClient.cs:line 433
   at Docker.DotNet.DockerClient.MakeRequestAsync[T](IEnumerable`1 errorHandlers, HttpMethod method, String path, IQueryString queryString, IRequestContent body, IDictionary`2 headers, TimeSpan timeout, CancellationToken token) in /_/src/Docker.DotNet/DockerClient.cs:line 243
   at DotNet.Testcontainers.Builders.DockerEndpointAuthenticationProvider.<>c__DisplayClass5_0.<<IsAvailable>b__0>d.MoveNext() in /_/src/Testcontainers/Builders/DockerEndpointAuthenticationProvider.cs:line 48


      SearchSimilarAsync_WithIncludeMetadata_ShouldIncludeMetadataInResults [0:00.000] Failed: Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
Xunit.Sdk.TestPipelineException
Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 195
   at Xunit.v3.FixtureMappingManager.InitializeAsync(IReadOnlyCollection`1 fixtureTypes, Boolean createInstances) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 248
   at Xunit.v3.ExceptionAggregator.RunAsync(Func`1 code) in /_/src/xunit.v3.core/Exceptions/ExceptionAggregator.cs:line 124

DotNet.Testcontainers.Builders.DockerUnavailableException
Docker is either not running or misconfigured. Please ensure that Docker is running and that the endpoint is properly configured.
You can customize your configuration using either the environment variables or the ~/.testcontainers.properties file.
For more information, visit: https://dotnet.testcontainers.org/custom_configuration/.
  Details: 
    Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
   at DotNet.Testcontainers.Guard.ThrowIf[TType](ArgumentInfo`1& argument, Func`2 condition, Func`2 ifClause) in /_/src/Testcontainers/Guard.Null.cs:line 62
   at DotNet.Testcontainers.Builders.AbstractBuilder`4.Validate() in /_/src/Testcontainers/Builders/AbstractBuilder`4.cs:line 146
   at DotNet.Testcontainers.Builders.ContainerBuilder`3.Validate() in /_/src/Testcontainers/Builders/ContainerBuilder`3.cs:line 408
   at Testcontainers.Qdrant.QdrantBuilder.Build() in /_/src/Testcontainers.Qdrant/QdrantBuilder.cs:line 69
   at IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture.InitializeAsync() in /_/src/test/SemanticRagTests/IndFusion.SemanticRag.System.Tests/Infrastructure/Fixtures/QdrantContainerFixture.cs:line 44
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 191

System.AggregateException
One or more errors occurred. (Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.)
  Exception doesn't have a stacktrace

DotNet.Testcontainers.Builders.DockerUnavailableException
Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
  Exception doesn't have a stacktrace

System.TimeoutException
The operation has timed out.
   at System.IO.Pipes.NamedPipeClientStream.ConnectInternal(Int32 timeout, CancellationToken cancellationToken, Int32 startTime)
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
--- End of stack trace from previous location ---
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
   at System.Threading.Tasks.Task.ExecuteWithThreadLocal(Task& currentTaskSlot, Thread threadPoolThread)
--- End of stack trace from previous location ---
   at Docker.DotNet.DockerClient.<>c__DisplayClass5_0.<<-ctor>b__0>d.MoveNext() in /_/src/Docker.DotNet/DockerClient.cs:line 69
--- End of stack trace from previous location ---
   at Microsoft.Net.Http.Client.ManagedHandler.ProcessRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 160
   at Microsoft.Net.Http.Client.ManagedHandler.SendAsync(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 77
   at System.Net.Http.HttpClient.<SendAsync>g__Core|83_0(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationTokenSource cts, Boolean disposeCts, CancellationTokenSource pendingRequestsCts, CancellationToken originalCancellationToken)
   at Docker.DotNet.DockerClient.PrivateMakeRequestAsync(TimeSpan timeout, HttpCompletionOption completionOption, HttpMethod method, String path, IQueryString queryString, IDictionary`2 headers, IRequestContent data, CancellationToken cancellationToken) in /_/src/Docker.DotNet/DockerClient.cs:line 433
   at Docker.DotNet.DockerClient.MakeRequestAsync[T](IEnumerable`1 errorHandlers, HttpMethod method, String path, IQueryString queryString, IRequestContent body, IDictionary`2 headers, TimeSpan timeout, CancellationToken token) in /_/src/Docker.DotNet/DockerClient.cs:line 243
   at DotNet.Testcontainers.Builders.DockerEndpointAuthenticationProvider.<>c__DisplayClass5_0.<<IsAvailable>b__0>d.MoveNext() in /_/src/Testcontainers/Builders/DockerEndpointAuthenticationProvider.cs:line 48


      SearchSimilarAsync_WithMetadataFilters_ShouldApplyFilters [0:00.000] Failed: Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
Xunit.Sdk.TestPipelineException
Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 195
   at Xunit.v3.FixtureMappingManager.InitializeAsync(IReadOnlyCollection`1 fixtureTypes, Boolean createInstances) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 248
   at Xunit.v3.ExceptionAggregator.RunAsync(Func`1 code) in /_/src/xunit.v3.core/Exceptions/ExceptionAggregator.cs:line 124

DotNet.Testcontainers.Builders.DockerUnavailableException
Docker is either not running or misconfigured. Please ensure that Docker is running and that the endpoint is properly configured.
You can customize your configuration using either the environment variables or the ~/.testcontainers.properties file.
For more information, visit: https://dotnet.testcontainers.org/custom_configuration/.
  Details: 
    Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
   at DotNet.Testcontainers.Guard.ThrowIf[TType](ArgumentInfo`1& argument, Func`2 condition, Func`2 ifClause) in /_/src/Testcontainers/Guard.Null.cs:line 62
   at DotNet.Testcontainers.Builders.AbstractBuilder`4.Validate() in /_/src/Testcontainers/Builders/AbstractBuilder`4.cs:line 146
   at DotNet.Testcontainers.Builders.ContainerBuilder`3.Validate() in /_/src/Testcontainers/Builders/ContainerBuilder`3.cs:line 408
   at Testcontainers.Qdrant.QdrantBuilder.Build() in /_/src/Testcontainers.Qdrant/QdrantBuilder.cs:line 69
   at IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture.InitializeAsync() in /_/src/test/SemanticRagTests/IndFusion.SemanticRag.System.Tests/Infrastructure/Fixtures/QdrantContainerFixture.cs:line 44
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 191

System.AggregateException
One or more errors occurred. (Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.)
  Exception doesn't have a stacktrace

DotNet.Testcontainers.Builders.DockerUnavailableException
Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
  Exception doesn't have a stacktrace

System.TimeoutException
The operation has timed out.
   at System.IO.Pipes.NamedPipeClientStream.ConnectInternal(Int32 timeout, CancellationToken cancellationToken, Int32 startTime)
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
--- End of stack trace from previous location ---
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
   at System.Threading.Tasks.Task.ExecuteWithThreadLocal(Task& currentTaskSlot, Thread threadPoolThread)
--- End of stack trace from previous location ---
   at Docker.DotNet.DockerClient.<>c__DisplayClass5_0.<<-ctor>b__0>d.MoveNext() in /_/src/Docker.DotNet/DockerClient.cs:line 69
--- End of stack trace from previous location ---
   at Microsoft.Net.Http.Client.ManagedHandler.ProcessRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 160
   at Microsoft.Net.Http.Client.ManagedHandler.SendAsync(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 77
   at System.Net.Http.HttpClient.<SendAsync>g__Core|83_0(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationTokenSource cts, Boolean disposeCts, CancellationTokenSource pendingRequestsCts, CancellationToken originalCancellationToken)
   at Docker.DotNet.DockerClient.PrivateMakeRequestAsync(TimeSpan timeout, HttpCompletionOption completionOption, HttpMethod method, String path, IQueryString queryString, IDictionary`2 headers, IRequestContent data, CancellationToken cancellationToken) in /_/src/Docker.DotNet/DockerClient.cs:line 433
   at Docker.DotNet.DockerClient.MakeRequestAsync[T](IEnumerable`1 errorHandlers, HttpMethod method, String path, IQueryString queryString, IRequestContent body, IDictionary`2 headers, TimeSpan timeout, CancellationToken token) in /_/src/Docker.DotNet/DockerClient.cs:line 243
   at DotNet.Testcontainers.Builders.DockerEndpointAuthenticationProvider.<>c__DisplayClass5_0.<<IsAvailable>b__0>d.MoveNext() in /_/src/Testcontainers/Builders/DockerEndpointAuthenticationProvider.cs:line 48


      SearchSimilarAsync_WithProcessingTime_ShouldMeasureActualProcessingTime [0:00.000] Failed: Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
Xunit.Sdk.TestPipelineException
Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 195
   at Xunit.v3.FixtureMappingManager.InitializeAsync(IReadOnlyCollection`1 fixtureTypes, Boolean createInstances) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 248
   at Xunit.v3.ExceptionAggregator.RunAsync(Func`1 code) in /_/src/xunit.v3.core/Exceptions/ExceptionAggregator.cs:line 124

DotNet.Testcontainers.Builders.DockerUnavailableException
Docker is either not running or misconfigured. Please ensure that Docker is running and that the endpoint is properly configured.
You can customize your configuration using either the environment variables or the ~/.testcontainers.properties file.
For more information, visit: https://dotnet.testcontainers.org/custom_configuration/.
  Details: 
    Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
   at DotNet.Testcontainers.Guard.ThrowIf[TType](ArgumentInfo`1& argument, Func`2 condition, Func`2 ifClause) in /_/src/Testcontainers/Guard.Null.cs:line 62
   at DotNet.Testcontainers.Builders.AbstractBuilder`4.Validate() in /_/src/Testcontainers/Builders/AbstractBuilder`4.cs:line 146
   at DotNet.Testcontainers.Builders.ContainerBuilder`3.Validate() in /_/src/Testcontainers/Builders/ContainerBuilder`3.cs:line 408
   at Testcontainers.Qdrant.QdrantBuilder.Build() in /_/src/Testcontainers.Qdrant/QdrantBuilder.cs:line 69
   at IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture.InitializeAsync() in /_/src/test/SemanticRagTests/IndFusion.SemanticRag.System.Tests/Infrastructure/Fixtures/QdrantContainerFixture.cs:line 44
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 191

System.AggregateException
One or more errors occurred. (Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.)
  Exception doesn't have a stacktrace

DotNet.Testcontainers.Builders.DockerUnavailableException
Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
  Exception doesn't have a stacktrace

System.TimeoutException
The operation has timed out.
   at System.IO.Pipes.NamedPipeClientStream.ConnectInternal(Int32 timeout, CancellationToken cancellationToken, Int32 startTime)
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
--- End of stack trace from previous location ---
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
   at System.Threading.Tasks.Task.ExecuteWithThreadLocal(Task& currentTaskSlot, Thread threadPoolThread)
--- End of stack trace from previous location ---
   at Docker.DotNet.DockerClient.<>c__DisplayClass5_0.<<-ctor>b__0>d.MoveNext() in /_/src/Docker.DotNet/DockerClient.cs:line 69
--- End of stack trace from previous location ---
   at Microsoft.Net.Http.Client.ManagedHandler.ProcessRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 160
   at Microsoft.Net.Http.Client.ManagedHandler.SendAsync(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 77
   at System.Net.Http.HttpClient.<SendAsync>g__Core|83_0(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationTokenSource cts, Boolean disposeCts, CancellationTokenSource pendingRequestsCts, CancellationToken originalCancellationToken)
   at Docker.DotNet.DockerClient.PrivateMakeRequestAsync(TimeSpan timeout, HttpCompletionOption completionOption, HttpMethod method, String path, IQueryString queryString, IDictionary`2 headers, IRequestContent data, CancellationToken cancellationToken) in /_/src/Docker.DotNet/DockerClient.cs:line 433
   at Docker.DotNet.DockerClient.MakeRequestAsync[T](IEnumerable`1 errorHandlers, HttpMethod method, String path, IQueryString queryString, IRequestContent body, IDictionary`2 headers, TimeSpan timeout, CancellationToken token) in /_/src/Docker.DotNet/DockerClient.cs:line 243
   at DotNet.Testcontainers.Builders.DockerEndpointAuthenticationProvider.<>c__DisplayClass5_0.<<IsAvailable>b__0>d.MoveNext() in /_/src/Testcontainers/Builders/DockerEndpointAuthenticationProvider.cs:line 48


      SearchSimilarAsync_WithQdrantFailure_ShouldPropagateException [0:00.000] Failed: Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
Xunit.Sdk.TestPipelineException
Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 195
   at Xunit.v3.FixtureMappingManager.InitializeAsync(IReadOnlyCollection`1 fixtureTypes, Boolean createInstances) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 248
   at Xunit.v3.ExceptionAggregator.RunAsync(Func`1 code) in /_/src/xunit.v3.core/Exceptions/ExceptionAggregator.cs:line 124

DotNet.Testcontainers.Builders.DockerUnavailableException
Docker is either not running or misconfigured. Please ensure that Docker is running and that the endpoint is properly configured.
You can customize your configuration using either the environment variables or the ~/.testcontainers.properties file.
For more information, visit: https://dotnet.testcontainers.org/custom_configuration/.
  Details: 
    Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
   at DotNet.Testcontainers.Guard.ThrowIf[TType](ArgumentInfo`1& argument, Func`2 condition, Func`2 ifClause) in /_/src/Testcontainers/Guard.Null.cs:line 62
   at DotNet.Testcontainers.Builders.AbstractBuilder`4.Validate() in /_/src/Testcontainers/Builders/AbstractBuilder`4.cs:line 146
   at DotNet.Testcontainers.Builders.ContainerBuilder`3.Validate() in /_/src/Testcontainers/Builders/ContainerBuilder`3.cs:line 408
   at Testcontainers.Qdrant.QdrantBuilder.Build() in /_/src/Testcontainers.Qdrant/QdrantBuilder.cs:line 69
   at IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture.InitializeAsync() in /_/src/test/SemanticRagTests/IndFusion.SemanticRag.System.Tests/Infrastructure/Fixtures/QdrantContainerFixture.cs:line 44
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 191

System.AggregateException
One or more errors occurred. (Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.)
  Exception doesn't have a stacktrace

DotNet.Testcontainers.Builders.DockerUnavailableException
Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
  Exception doesn't have a stacktrace

System.TimeoutException
The operation has timed out.
   at System.IO.Pipes.NamedPipeClientStream.ConnectInternal(Int32 timeout, CancellationToken cancellationToken, Int32 startTime)
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
--- End of stack trace from previous location ---
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
   at System.Threading.Tasks.Task.ExecuteWithThreadLocal(Task& currentTaskSlot, Thread threadPoolThread)
--- End of stack trace from previous location ---
   at Docker.DotNet.DockerClient.<>c__DisplayClass5_0.<<-ctor>b__0>d.MoveNext() in /_/src/Docker.DotNet/DockerClient.cs:line 69
--- End of stack trace from previous location ---
   at Microsoft.Net.Http.Client.ManagedHandler.ProcessRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 160
   at Microsoft.Net.Http.Client.ManagedHandler.SendAsync(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 77
   at System.Net.Http.HttpClient.<SendAsync>g__Core|83_0(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationTokenSource cts, Boolean disposeCts, CancellationTokenSource pendingRequestsCts, CancellationToken originalCancellationToken)
   at Docker.DotNet.DockerClient.PrivateMakeRequestAsync(TimeSpan timeout, HttpCompletionOption completionOption, HttpMethod method, String path, IQueryString queryString, IDictionary`2 headers, IRequestContent data, CancellationToken cancellationToken) in /_/src/Docker.DotNet/DockerClient.cs:line 433
   at Docker.DotNet.DockerClient.MakeRequestAsync[T](IEnumerable`1 errorHandlers, HttpMethod method, String path, IQueryString queryString, IRequestContent body, IDictionary`2 headers, TimeSpan timeout, CancellationToken token) in /_/src/Docker.DotNet/DockerClient.cs:line 243
   at DotNet.Testcontainers.Builders.DockerEndpointAuthenticationProvider.<>c__DisplayClass5_0.<<IsAvailable>b__0>d.MoveNext() in /_/src/Testcontainers/Builders/DockerEndpointAuthenticationProvider.cs:line 48


      SearchSimilarAsync_WithTimeout_ShouldRespectTimeout [0:00.000] Failed: Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
Xunit.Sdk.TestPipelineException
Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 195
   at Xunit.v3.FixtureMappingManager.InitializeAsync(IReadOnlyCollection`1 fixtureTypes, Boolean createInstances) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 248
   at Xunit.v3.ExceptionAggregator.RunAsync(Func`1 code) in /_/src/xunit.v3.core/Exceptions/ExceptionAggregator.cs:line 124

DotNet.Testcontainers.Builders.DockerUnavailableException
Docker is either not running or misconfigured. Please ensure that Docker is running and that the endpoint is properly configured.
You can customize your configuration using either the environment variables or the ~/.testcontainers.properties file.
For more information, visit: https://dotnet.testcontainers.org/custom_configuration/.
  Details: 
    Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
   at DotNet.Testcontainers.Guard.ThrowIf[TType](ArgumentInfo`1& argument, Func`2 condition, Func`2 ifClause) in /_/src/Testcontainers/Guard.Null.cs:line 62
   at DotNet.Testcontainers.Builders.AbstractBuilder`4.Validate() in /_/src/Testcontainers/Builders/AbstractBuilder`4.cs:line 146
   at DotNet.Testcontainers.Builders.ContainerBuilder`3.Validate() in /_/src/Testcontainers/Builders/ContainerBuilder`3.cs:line 408
   at Testcontainers.Qdrant.QdrantBuilder.Build() in /_/src/Testcontainers.Qdrant/QdrantBuilder.cs:line 69
   at IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture.InitializeAsync() in /_/src/test/SemanticRagTests/IndFusion.SemanticRag.System.Tests/Infrastructure/Fixtures/QdrantContainerFixture.cs:line 44
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 191

System.AggregateException
One or more errors occurred. (Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.)
  Exception doesn't have a stacktrace

DotNet.Testcontainers.Builders.DockerUnavailableException
Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
  Exception doesn't have a stacktrace

System.TimeoutException
The operation has timed out.
   at System.IO.Pipes.NamedPipeClientStream.ConnectInternal(Int32 timeout, CancellationToken cancellationToken, Int32 startTime)
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
--- End of stack trace from previous location ---
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
   at System.Threading.Tasks.Task.ExecuteWithThreadLocal(Task& currentTaskSlot, Thread threadPoolThread)
--- End of stack trace from previous location ---
   at Docker.DotNet.DockerClient.<>c__DisplayClass5_0.<<-ctor>b__0>d.MoveNext() in /_/src/Docker.DotNet/DockerClient.cs:line 69
--- End of stack trace from previous location ---
   at Microsoft.Net.Http.Client.ManagedHandler.ProcessRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 160
   at Microsoft.Net.Http.Client.ManagedHandler.SendAsync(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 77
   at System.Net.Http.HttpClient.<SendAsync>g__Core|83_0(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationTokenSource cts, Boolean disposeCts, CancellationTokenSource pendingRequestsCts, CancellationToken originalCancellationToken)
   at Docker.DotNet.DockerClient.PrivateMakeRequestAsync(TimeSpan timeout, HttpCompletionOption completionOption, HttpMethod method, String path, IQueryString queryString, IDictionary`2 headers, IRequestContent data, CancellationToken cancellationToken) in /_/src/Docker.DotNet/DockerClient.cs:line 433
   at Docker.DotNet.DockerClient.MakeRequestAsync[T](IEnumerable`1 errorHandlers, HttpMethod method, String path, IQueryString queryString, IRequestContent body, IDictionary`2 headers, TimeSpan timeout, CancellationToken token) in /_/src/Docker.DotNet/DockerClient.cs:line 243
   at DotNet.Testcontainers.Builders.DockerEndpointAuthenticationProvider.<>c__DisplayClass5_0.<<IsAvailable>b__0>d.MoveNext() in /_/src/Testcontainers/Builders/DockerEndpointAuthenticationProvider.cs:line 48


      SearchSimilarAsync_WithValidQuery_ShouldReturnActualSearchResults [0:00.000] Failed: Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
Xunit.Sdk.TestPipelineException
Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 195
   at Xunit.v3.FixtureMappingManager.InitializeAsync(IReadOnlyCollection`1 fixtureTypes, Boolean createInstances) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 248
   at Xunit.v3.ExceptionAggregator.RunAsync(Func`1 code) in /_/src/xunit.v3.core/Exceptions/ExceptionAggregator.cs:line 124

DotNet.Testcontainers.Builders.DockerUnavailableException
Docker is either not running or misconfigured. Please ensure that Docker is running and that the endpoint is properly configured.
You can customize your configuration using either the environment variables or the ~/.testcontainers.properties file.
For more information, visit: https://dotnet.testcontainers.org/custom_configuration/.
  Details: 
    Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
   at DotNet.Testcontainers.Guard.ThrowIf[TType](ArgumentInfo`1& argument, Func`2 condition, Func`2 ifClause) in /_/src/Testcontainers/Guard.Null.cs:line 62
   at DotNet.Testcontainers.Builders.AbstractBuilder`4.Validate() in /_/src/Testcontainers/Builders/AbstractBuilder`4.cs:line 146
   at DotNet.Testcontainers.Builders.ContainerBuilder`3.Validate() in /_/src/Testcontainers/Builders/ContainerBuilder`3.cs:line 408
   at Testcontainers.Qdrant.QdrantBuilder.Build() in /_/src/Testcontainers.Qdrant/QdrantBuilder.cs:line 69
   at IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture.InitializeAsync() in /_/src/test/SemanticRagTests/IndFusion.SemanticRag.System.Tests/Infrastructure/Fixtures/QdrantContainerFixture.cs:line 44
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 191

System.AggregateException
One or more errors occurred. (Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.)
  Exception doesn't have a stacktrace

DotNet.Testcontainers.Builders.DockerUnavailableException
Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
  Exception doesn't have a stacktrace

System.TimeoutException
The operation has timed out.
   at System.IO.Pipes.NamedPipeClientStream.ConnectInternal(Int32 timeout, CancellationToken cancellationToken, Int32 startTime)
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
--- End of stack trace from previous location ---
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
   at System.Threading.Tasks.Task.ExecuteWithThreadLocal(Task& currentTaskSlot, Thread threadPoolThread)
--- End of stack trace from previous location ---
   at Docker.DotNet.DockerClient.<>c__DisplayClass5_0.<<-ctor>b__0>d.MoveNext() in /_/src/Docker.DotNet/DockerClient.cs:line 69
--- End of stack trace from previous location ---
   at Microsoft.Net.Http.Client.ManagedHandler.ProcessRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 160
   at Microsoft.Net.Http.Client.ManagedHandler.SendAsync(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 77
   at System.Net.Http.HttpClient.<SendAsync>g__Core|83_0(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationTokenSource cts, Boolean disposeCts, CancellationTokenSource pendingRequestsCts, CancellationToken originalCancellationToken)
   at Docker.DotNet.DockerClient.PrivateMakeRequestAsync(TimeSpan timeout, HttpCompletionOption completionOption, HttpMethod method, String path, IQueryString queryString, IDictionary`2 headers, IRequestContent data, CancellationToken cancellationToken) in /_/src/Docker.DotNet/DockerClient.cs:line 433
   at Docker.DotNet.DockerClient.MakeRequestAsync[T](IEnumerable`1 errorHandlers, HttpMethod method, String path, IQueryString queryString, IRequestContent body, IDictionary`2 headers, TimeSpan timeout, CancellationToken token) in /_/src/Docker.DotNet/DockerClient.cs:line 243
   at DotNet.Testcontainers.Builders.DockerEndpointAuthenticationProvider.<>c__DisplayClass5_0.<<IsAvailable>b__0>d.MoveNext() in /_/src/Testcontainers/Builders/DockerEndpointAuthenticationProvider.cs:line 48


      StoreDocumentAsync_WithNullContent_ShouldThrowArgumentException [0:00.000] Failed: Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
Xunit.Sdk.TestPipelineException
Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 195
   at Xunit.v3.FixtureMappingManager.InitializeAsync(IReadOnlyCollection`1 fixtureTypes, Boolean createInstances) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 248
   at Xunit.v3.ExceptionAggregator.RunAsync(Func`1 code) in /_/src/xunit.v3.core/Exceptions/ExceptionAggregator.cs:line 124

DotNet.Testcontainers.Builders.DockerUnavailableException
Docker is either not running or misconfigured. Please ensure that Docker is running and that the endpoint is properly configured.
You can customize your configuration using either the environment variables or the ~/.testcontainers.properties file.
For more information, visit: https://dotnet.testcontainers.org/custom_configuration/.
  Details: 
    Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
   at DotNet.Testcontainers.Guard.ThrowIf[TType](ArgumentInfo`1& argument, Func`2 condition, Func`2 ifClause) in /_/src/Testcontainers/Guard.Null.cs:line 62
   at DotNet.Testcontainers.Builders.AbstractBuilder`4.Validate() in /_/src/Testcontainers/Builders/AbstractBuilder`4.cs:line 146
   at DotNet.Testcontainers.Builders.ContainerBuilder`3.Validate() in /_/src/Testcontainers/Builders/ContainerBuilder`3.cs:line 408
   at Testcontainers.Qdrant.QdrantBuilder.Build() in /_/src/Testcontainers.Qdrant/QdrantBuilder.cs:line 69
   at IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture.InitializeAsync() in /_/src/test/SemanticRagTests/IndFusion.SemanticRag.System.Tests/Infrastructure/Fixtures/QdrantContainerFixture.cs:line 44
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 191

System.AggregateException
One or more errors occurred. (Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.)
  Exception doesn't have a stacktrace

DotNet.Testcontainers.Builders.DockerUnavailableException
Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
  Exception doesn't have a stacktrace

System.TimeoutException
The operation has timed out.
   at System.IO.Pipes.NamedPipeClientStream.ConnectInternal(Int32 timeout, CancellationToken cancellationToken, Int32 startTime)
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
--- End of stack trace from previous location ---
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
   at System.Threading.Tasks.Task.ExecuteWithThreadLocal(Task& currentTaskSlot, Thread threadPoolThread)
--- End of stack trace from previous location ---
   at Docker.DotNet.DockerClient.<>c__DisplayClass5_0.<<-ctor>b__0>d.MoveNext() in /_/src/Docker.DotNet/DockerClient.cs:line 69
--- End of stack trace from previous location ---
   at Microsoft.Net.Http.Client.ManagedHandler.ProcessRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 160
   at Microsoft.Net.Http.Client.ManagedHandler.SendAsync(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 77
   at System.Net.Http.HttpClient.<SendAsync>g__Core|83_0(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationTokenSource cts, Boolean disposeCts, CancellationTokenSource pendingRequestsCts, CancellationToken originalCancellationToken)
   at Docker.DotNet.DockerClient.PrivateMakeRequestAsync(TimeSpan timeout, HttpCompletionOption completionOption, HttpMethod method, String path, IQueryString queryString, IDictionary`2 headers, IRequestContent data, CancellationToken cancellationToken) in /_/src/Docker.DotNet/DockerClient.cs:line 433
   at Docker.DotNet.DockerClient.MakeRequestAsync[T](IEnumerable`1 errorHandlers, HttpMethod method, String path, IQueryString queryString, IRequestContent body, IDictionary`2 headers, TimeSpan timeout, CancellationToken token) in /_/src/Docker.DotNet/DockerClient.cs:line 243
   at DotNet.Testcontainers.Builders.DockerEndpointAuthenticationProvider.<>c__DisplayClass5_0.<<IsAvailable>b__0>d.MoveNext() in /_/src/Testcontainers/Builders/DockerEndpointAuthenticationProvider.cs:line 48


      StoreDocumentAsync_WithNullId_ShouldThrowArgumentException [0:00.000] Failed: Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
Xunit.Sdk.TestPipelineException
Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 195
   at Xunit.v3.FixtureMappingManager.InitializeAsync(IReadOnlyCollection`1 fixtureTypes, Boolean createInstances) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 248
   at Xunit.v3.ExceptionAggregator.RunAsync(Func`1 code) in /_/src/xunit.v3.core/Exceptions/ExceptionAggregator.cs:line 124

DotNet.Testcontainers.Builders.DockerUnavailableException
Docker is either not running or misconfigured. Please ensure that Docker is running and that the endpoint is properly configured.
You can customize your configuration using either the environment variables or the ~/.testcontainers.properties file.
For more information, visit: https://dotnet.testcontainers.org/custom_configuration/.
  Details: 
    Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
   at DotNet.Testcontainers.Guard.ThrowIf[TType](ArgumentInfo`1& argument, Func`2 condition, Func`2 ifClause) in /_/src/Testcontainers/Guard.Null.cs:line 62
   at DotNet.Testcontainers.Builders.AbstractBuilder`4.Validate() in /_/src/Testcontainers/Builders/AbstractBuilder`4.cs:line 146
   at DotNet.Testcontainers.Builders.ContainerBuilder`3.Validate() in /_/src/Testcontainers/Builders/ContainerBuilder`3.cs:line 408
   at Testcontainers.Qdrant.QdrantBuilder.Build() in /_/src/Testcontainers.Qdrant/QdrantBuilder.cs:line 69
   at IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture.InitializeAsync() in /_/src/test/SemanticRagTests/IndFusion.SemanticRag.System.Tests/Infrastructure/Fixtures/QdrantContainerFixture.cs:line 44
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 191

System.AggregateException
One or more errors occurred. (Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.)
  Exception doesn't have a stacktrace

DotNet.Testcontainers.Builders.DockerUnavailableException
Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
  Exception doesn't have a stacktrace

System.TimeoutException
The operation has timed out.
   at System.IO.Pipes.NamedPipeClientStream.ConnectInternal(Int32 timeout, CancellationToken cancellationToken, Int32 startTime)
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
--- End of stack trace from previous location ---
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
   at System.Threading.Tasks.Task.ExecuteWithThreadLocal(Task& currentTaskSlot, Thread threadPoolThread)
--- End of stack trace from previous location ---
   at Docker.DotNet.DockerClient.<>c__DisplayClass5_0.<<-ctor>b__0>d.MoveNext() in /_/src/Docker.DotNet/DockerClient.cs:line 69
--- End of stack trace from previous location ---
   at Microsoft.Net.Http.Client.ManagedHandler.ProcessRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 160
   at Microsoft.Net.Http.Client.ManagedHandler.SendAsync(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 77
   at System.Net.Http.HttpClient.<SendAsync>g__Core|83_0(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationTokenSource cts, Boolean disposeCts, CancellationTokenSource pendingRequestsCts, CancellationToken originalCancellationToken)
   at Docker.DotNet.DockerClient.PrivateMakeRequestAsync(TimeSpan timeout, HttpCompletionOption completionOption, HttpMethod method, String path, IQueryString queryString, IDictionary`2 headers, IRequestContent data, CancellationToken cancellationToken) in /_/src/Docker.DotNet/DockerClient.cs:line 433
   at Docker.DotNet.DockerClient.MakeRequestAsync[T](IEnumerable`1 errorHandlers, HttpMethod method, String path, IQueryString queryString, IRequestContent body, IDictionary`2 headers, TimeSpan timeout, CancellationToken token) in /_/src/Docker.DotNet/DockerClient.cs:line 243
   at DotNet.Testcontainers.Builders.DockerEndpointAuthenticationProvider.<>c__DisplayClass5_0.<<IsAvailable>b__0>d.MoveNext() in /_/src/Testcontainers/Builders/DockerEndpointAuthenticationProvider.cs:line 48


      StoreDocumentAsync_WithNullMetadata_ShouldThrowArgumentException [0:00.000] Failed: Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
Xunit.Sdk.TestPipelineException
Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 195
   at Xunit.v3.FixtureMappingManager.InitializeAsync(IReadOnlyCollection`1 fixtureTypes, Boolean createInstances) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 248
   at Xunit.v3.ExceptionAggregator.RunAsync(Func`1 code) in /_/src/xunit.v3.core/Exceptions/ExceptionAggregator.cs:line 124

DotNet.Testcontainers.Builders.DockerUnavailableException
Docker is either not running or misconfigured. Please ensure that Docker is running and that the endpoint is properly configured.
You can customize your configuration using either the environment variables or the ~/.testcontainers.properties file.
For more information, visit: https://dotnet.testcontainers.org/custom_configuration/.
  Details: 
    Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
   at DotNet.Testcontainers.Guard.ThrowIf[TType](ArgumentInfo`1& argument, Func`2 condition, Func`2 ifClause) in /_/src/Testcontainers/Guard.Null.cs:line 62
   at DotNet.Testcontainers.Builders.AbstractBuilder`4.Validate() in /_/src/Testcontainers/Builders/AbstractBuilder`4.cs:line 146
   at DotNet.Testcontainers.Builders.ContainerBuilder`3.Validate() in /_/src/Testcontainers/Builders/ContainerBuilder`3.cs:line 408
   at Testcontainers.Qdrant.QdrantBuilder.Build() in /_/src/Testcontainers.Qdrant/QdrantBuilder.cs:line 69
   at IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture.InitializeAsync() in /_/src/test/SemanticRagTests/IndFusion.SemanticRag.System.Tests/Infrastructure/Fixtures/QdrantContainerFixture.cs:line 44
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 191

System.AggregateException
One or more errors occurred. (Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.)
  Exception doesn't have a stacktrace

DotNet.Testcontainers.Builders.DockerUnavailableException
Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
  Exception doesn't have a stacktrace

System.TimeoutException
The operation has timed out.
   at System.IO.Pipes.NamedPipeClientStream.ConnectInternal(Int32 timeout, CancellationToken cancellationToken, Int32 startTime)
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
--- End of stack trace from previous location ---
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
   at System.Threading.Tasks.Task.ExecuteWithThreadLocal(Task& currentTaskSlot, Thread threadPoolThread)
--- End of stack trace from previous location ---
   at Docker.DotNet.DockerClient.<>c__DisplayClass5_0.<<-ctor>b__0>d.MoveNext() in /_/src/Docker.DotNet/DockerClient.cs:line 69
--- End of stack trace from previous location ---
   at Microsoft.Net.Http.Client.ManagedHandler.ProcessRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 160
   at Microsoft.Net.Http.Client.ManagedHandler.SendAsync(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 77
   at System.Net.Http.HttpClient.<SendAsync>g__Core|83_0(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationTokenSource cts, Boolean disposeCts, CancellationTokenSource pendingRequestsCts, CancellationToken originalCancellationToken)
   at Docker.DotNet.DockerClient.PrivateMakeRequestAsync(TimeSpan timeout, HttpCompletionOption completionOption, HttpMethod method, String path, IQueryString queryString, IDictionary`2 headers, IRequestContent data, CancellationToken cancellationToken) in /_/src/Docker.DotNet/DockerClient.cs:line 433
   at Docker.DotNet.DockerClient.MakeRequestAsync[T](IEnumerable`1 errorHandlers, HttpMethod method, String path, IQueryString queryString, IRequestContent body, IDictionary`2 headers, TimeSpan timeout, CancellationToken token) in /_/src/Docker.DotNet/DockerClient.cs:line 243
   at DotNet.Testcontainers.Builders.DockerEndpointAuthenticationProvider.<>c__DisplayClass5_0.<<IsAvailable>b__0>d.MoveNext() in /_/src/Testcontainers/Builders/DockerEndpointAuthenticationProvider.cs:line 48


      StoreDocumentAsync_WithValidData_ShouldStoreDocument [0:00.000] Failed: Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
Xunit.Sdk.TestPipelineException
Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 195
   at Xunit.v3.FixtureMappingManager.InitializeAsync(IReadOnlyCollection`1 fixtureTypes, Boolean createInstances) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 248
   at Xunit.v3.ExceptionAggregator.RunAsync(Func`1 code) in /_/src/xunit.v3.core/Exceptions/ExceptionAggregator.cs:line 124

DotNet.Testcontainers.Builders.DockerUnavailableException
Docker is either not running or misconfigured. Please ensure that Docker is running and that the endpoint is properly configured.
You can customize your configuration using either the environment variables or the ~/.testcontainers.properties file.
For more information, visit: https://dotnet.testcontainers.org/custom_configuration/.
  Details: 
    Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
   at DotNet.Testcontainers.Guard.ThrowIf[TType](ArgumentInfo`1& argument, Func`2 condition, Func`2 ifClause) in /_/src/Testcontainers/Guard.Null.cs:line 62
   at DotNet.Testcontainers.Builders.AbstractBuilder`4.Validate() in /_/src/Testcontainers/Builders/AbstractBuilder`4.cs:line 146
   at DotNet.Testcontainers.Builders.ContainerBuilder`3.Validate() in /_/src/Testcontainers/Builders/ContainerBuilder`3.cs:line 408
   at Testcontainers.Qdrant.QdrantBuilder.Build() in /_/src/Testcontainers.Qdrant/QdrantBuilder.cs:line 69
   at IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture.InitializeAsync() in /_/src/test/SemanticRagTests/IndFusion.SemanticRag.System.Tests/Infrastructure/Fixtures/QdrantContainerFixture.cs:line 44
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 191

System.AggregateException
One or more errors occurred. (Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.)
  Exception doesn't have a stacktrace

DotNet.Testcontainers.Builders.DockerUnavailableException
Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
  Exception doesn't have a stacktrace

System.TimeoutException
The operation has timed out.
   at System.IO.Pipes.NamedPipeClientStream.ConnectInternal(Int32 timeout, CancellationToken cancellationToken, Int32 startTime)
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
--- End of stack trace from previous location ---
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
   at System.Threading.Tasks.Task.ExecuteWithThreadLocal(Task& currentTaskSlot, Thread threadPoolThread)
--- End of stack trace from previous location ---
   at Docker.DotNet.DockerClient.<>c__DisplayClass5_0.<<-ctor>b__0>d.MoveNext() in /_/src/Docker.DotNet/DockerClient.cs:line 69
--- End of stack trace from previous location ---
   at Microsoft.Net.Http.Client.ManagedHandler.ProcessRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 160
   at Microsoft.Net.Http.Client.ManagedHandler.SendAsync(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 77
   at System.Net.Http.HttpClient.<SendAsync>g__Core|83_0(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationTokenSource cts, Boolean disposeCts, CancellationTokenSource pendingRequestsCts, CancellationToken originalCancellationToken)
   at Docker.DotNet.DockerClient.PrivateMakeRequestAsync(TimeSpan timeout, HttpCompletionOption completionOption, HttpMethod method, String path, IQueryString queryString, IDictionary`2 headers, IRequestContent data, CancellationToken cancellationToken) in /_/src/Docker.DotNet/DockerClient.cs:line 433
   at Docker.DotNet.DockerClient.MakeRequestAsync[T](IEnumerable`1 errorHandlers, HttpMethod method, String path, IQueryString queryString, IRequestContent body, IDictionary`2 headers, TimeSpan timeout, CancellationToken token) in /_/src/Docker.DotNet/DockerClient.cs:line 243
   at DotNet.Testcontainers.Builders.DockerEndpointAuthenticationProvider.<>c__DisplayClass5_0.<<IsAvailable>b__0>d.MoveNext() in /_/src/Testcontainers/Builders/DockerEndpointAuthenticationProvider.cs:line 48


      UpdateDocumentAsync_WithValidData_ShouldUpdateDocument [0:00.000] Failed: Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
Xunit.Sdk.TestPipelineException
Collection fixture type 'IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture' threw in InitializeAsync
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 195
   at Xunit.v3.FixtureMappingManager.InitializeAsync(IReadOnlyCollection`1 fixtureTypes, Boolean createInstances) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 248
   at Xunit.v3.ExceptionAggregator.RunAsync(Func`1 code) in /_/src/xunit.v3.core/Exceptions/ExceptionAggregator.cs:line 124

DotNet.Testcontainers.Builders.DockerUnavailableException
Docker is either not running or misconfigured. Please ensure that Docker is running and that the endpoint is properly configured.
You can customize your configuration using either the environment variables or the ~/.testcontainers.properties file.
For more information, visit: https://dotnet.testcontainers.org/custom_configuration/.
  Details: 
    Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
   at DotNet.Testcontainers.Guard.ThrowIf[TType](ArgumentInfo`1& argument, Func`2 condition, Func`2 ifClause) in /_/src/Testcontainers/Guard.Null.cs:line 62
   at DotNet.Testcontainers.Builders.AbstractBuilder`4.Validate() in /_/src/Testcontainers/Builders/AbstractBuilder`4.cs:line 146
   at DotNet.Testcontainers.Builders.ContainerBuilder`3.Validate() in /_/src/Testcontainers/Builders/ContainerBuilder`3.cs:line 408
   at Testcontainers.Qdrant.QdrantBuilder.Build() in /_/src/Testcontainers.Qdrant/QdrantBuilder.cs:line 69
   at IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures.QdrantContainerFixture.InitializeAsync() in /_/src/test/SemanticRagTests/IndFusion.SemanticRag.System.Tests/Infrastructure/Fixtures/QdrantContainerFixture.cs:line 44
   at Xunit.v3.FixtureMappingManager.GetFixture(Type fixtureType) in /_/src/xunit.v3.core/Utility/FixtureMappingManager.cs:line 191

System.AggregateException
One or more errors occurred. (Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.)
  Exception doesn't have a stacktrace

DotNet.Testcontainers.Builders.DockerUnavailableException
Failed to connect to Docker endpoint at 'npipe://./pipe/docker_engine'.
  Exception doesn't have a stacktrace

System.TimeoutException
The operation has timed out.
   at System.IO.Pipes.NamedPipeClientStream.ConnectInternal(Int32 timeout, CancellationToken cancellationToken, Int32 startTime)
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
--- End of stack trace from previous location ---
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
   at System.Threading.Tasks.Task.ExecuteWithThreadLocal(Task& currentTaskSlot, Thread threadPoolThread)
--- End of stack trace from previous location ---
   at Docker.DotNet.DockerClient.<>c__DisplayClass5_0.<<-ctor>b__0>d.MoveNext() in /_/src/Docker.DotNet/DockerClient.cs:line 69
--- End of stack trace from previous location ---
   at Microsoft.Net.Http.Client.ManagedHandler.ProcessRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 160
   at Microsoft.Net.Http.Client.ManagedHandler.SendAsync(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken) in /_/src/Docker.DotNet/Microsoft.Net.Http.Client/ManagedHandler.cs:line 77
   at System.Net.Http.HttpClient.<SendAsync>g__Core|83_0(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationTokenSource cts, Boolean disposeCts, CancellationTokenSource pendingRequestsCts, CancellationToken originalCancellationToken)
   at Docker.DotNet.DockerClient.PrivateMakeRequestAsync(TimeSpan timeout, HttpCompletionOption completionOption, HttpMethod method, String path, IQueryString queryString, IDictionary`2 headers, IRequestContent data, CancellationToken cancellationToken) in /_/src/Docker.DotNet/DockerClient.cs:line 433
   at Docker.DotNet.DockerClient.MakeRequestAsync[T](IEnumerable`1 errorHandlers, HttpMethod method, String path, IQueryString queryString, IRequestContent body, IDictionary`2 headers, TimeSpan timeout, CancellationToken token) in /_/src/Docker.DotNet/DockerClient.cs:line 243
   at DotNet.Testcontainers.Builders.DockerEndpointAuthenticationProvider.<>c__DisplayClass5_0.<<IsAvailable>b__0>d.MoveNext() in /_/src/Testcontainers/Builders/DockerEndpointAuthenticationProvider.cs:line 48


   IndFusion.SemanticRag.Tests (17 tests) [0:42.519] Failed: 17 tests failed
    IndFusion.SemanticRag.Tests (17 tests) [0:42.519] Failed: 17 tests failed
     Implementations (13 tests) [0:13.291] Failed: 13 tests failed
      DocumentProcessingPipelineTests (6 tests) [0:03.333] Failed: One or more child tests failed: 6 tests failed
       DetectDocumentTypeAsync_Should_Detect_By_Content_Analysis [0:03.264] Failed: Shouldly.ShouldAssertException: DocumentType.Text
Shouldly.ShouldAssertException
DocumentType.Text
    should be
DocumentType.CSharpCode
    but was not

Additional Info:
    Failed for content analysis: public class Test {}
   at IndFusion.SemanticRag.Tests.Implementations.DocumentProcessingPipelineTests.DetectDocumentTypeAsync_Should_Detect_By_Content_Analysis() in /_/src/test/SemanticRagTests/IndFusion.SemanticRag.Tests/Implementations/DocumentProcessingPipelineTests.cs:line 299
   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task, ConfigureAwaitOptions options)
   at Xunit.v3.TestRunner`2.<>c__DisplayClass5_0.<<InvokeTest>b__1>d.MoveNext() in /_/src/xunit.v3.core/Runners/TestRunner.cs:line 170
--- End of stack trace from previous location ---
   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task, ConfigureAwaitOptions options)
   at Xunit.v3.ExceptionAggregator.RunAsync(Func`1 code) in /_/src/xunit.v3.core/Exceptions/ExceptionAggregator.cs:line 124


       DetectDocumentTypeAsync_Should_Detect_By_File_Extension [0:00.013] Failed: Shouldly.ShouldAssertException: DocumentType.Text
Shouldly.ShouldAssertException
DocumentType.Text
    should be
DocumentType.Markdown
    but was not

Additional Info:
    Failed for file extension: test.md
   at IndFusion.SemanticRag.Tests.Implementations.DocumentProcessingPipelineTests.DetectDocumentTypeAsync_Should_Detect_By_File_Extension() in /_/src/test/SemanticRagTests/IndFusion.SemanticRag.Tests/Implementations/DocumentProcessingPipelineTests.cs:line 274
   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task, ConfigureAwaitOptions options)
   at Xunit.v3.TestRunner`2.<>c__DisplayClass5_0.<<InvokeTest>b__1>d.MoveNext() in /_/src/xunit.v3.core/Runners/TestRunner.cs:line 170
--- End of stack trace from previous location ---
   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task, ConfigureAwaitOptions options)
   at Xunit.v3.ExceptionAggregator.RunAsync(Func`1 code) in /_/src/xunit.v3.core/Exceptions/ExceptionAggregator.cs:line 124


       ProcessDocumentAsync_Should_Create_Chunks_With_Fixed_Size_Strategy [0:00.003] Failed: Shouldly.ShouldAssertException: 1
Shouldly.ShouldAssertException
1
    should be greater than
1
    but was not
   at IndFusion.SemanticRag.Tests.Implementations.DocumentProcessingPipelineTests.ProcessDocumentAsync_Should_Create_Chunks_With_Fixed_Size_Strategy() in /_/src/test/SemanticRagTests/IndFusion.SemanticRag.Tests/Implementations/DocumentProcessingPipelineTests.cs:line 350
   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task, ConfigureAwaitOptions options)
   at Xunit.v3.TestRunner`2.<>c__DisplayClass5_0.<<InvokeTest>b__1>d.MoveNext() in /_/src/xunit.v3.core/Runners/TestRunner.cs:line 170
--- End of stack trace from previous location ---
   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task, ConfigureAwaitOptions options)
   at Xunit.v3.ExceptionAggregator.RunAsync(Func`1 code) in /_/src/xunit.v3.core/Exceptions/ExceptionAggregator.cs:line 124


       ProcessDocumentAsync_Should_Handle_Cancellation [0:00.003] Failed: Shouldly.ShouldAssertException: Task
Shouldly.ShouldAssertException
Task
    should throw
System.OperationCanceledException
    but did not
   at System.Threading.Tasks.ContinuationResultTaskFromResultTask`2.InnerInvoke()
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
--- End of stack trace from previous location ---
   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
   at System.Threading.Tasks.Task.ExecuteWithThreadLocal(Task& currentTaskSlot, Thread threadPoolThread)
--- End of stack trace from previous location ---
   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task, ConfigureAwaitOptions options)
   at System.Runtime.CompilerServices.TaskAwaiter`1.GetResult()
   at IndFusion.SemanticRag.Tests.Implementations.DocumentProcessingPipelineTests.ProcessDocumentAsync_Should_Handle_Cancellation() in /_/src/test/SemanticRagTests/IndFusion.SemanticRag.Tests/Implementations/DocumentProcessingPipelineTests.cs:line 409
   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task, ConfigureAwaitOptions options)
   at Xunit.v3.TestRunner`2.<>c__DisplayClass5_0.<<InvokeTest>b__1>d.MoveNext() in /_/src/xunit.v3.core/Runners/TestRunner.cs:line 170
--- End of stack trace from previous location ---
   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task, ConfigureAwaitOptions options)
   at Xunit.v3.ExceptionAggregator.RunAsync(Func`1 code) in /_/src/xunit.v3.core/Exceptions/ExceptionAggregator.cs:line 124


       ProcessDocumentAsync_Should_Not_Extract_Metadata_When_Disabled [0:00.004] Failed: Shouldly.ShouldAssertException: [] (System.Collections.Generic.Dictionary`2[System.String,System.Object])
Shouldly.ShouldAssertException
[] (System.Collections.Generic.Dictionary`2[System.String,System.Object])
    should be null but was
   at IndFusion.SemanticRag.Tests.Implementations.DocumentProcessingPipelineTests.ProcessDocumentAsync_Should_Not_Extract_Metadata_When_Disabled() in /_/src/test/SemanticRagTests/IndFusion.SemanticRag.Tests/Implementations/DocumentProcessingPipelineTests.cs:line 468
   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task, ConfigureAwaitOptions options)
   at Xunit.v3.TestRunner`2.<>c__DisplayClass5_0.<<InvokeTest>b__1>d.MoveNext() in /_/src/xunit.v3.core/Runners/TestRunner.cs:line 170
--- End of stack trace from previous location ---
   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task, ConfigureAwaitOptions options)
   at Xunit.v3.ExceptionAggregator.RunAsync(Func`1 code) in /_/src/xunit.v3.core/Exceptions/ExceptionAggregator.cs:line 124


       ProcessDocumentAsync_Should_Process_Text_Document_Successfully [0:00.002] Failed: Shouldly.ShouldAssertException: 0L
Shouldly.ShouldAssertException
0L
    should be greater than
0L
    but was not
   at IndFusion.SemanticRag.Tests.Implementations.DocumentProcessingPipelineTests.ProcessDocumentAsync_Should_Process_Text_Document_Successfully() in /_/src/test/SemanticRagTests/IndFusion.SemanticRag.Tests/Implementations/DocumentProcessingPipelineTests.cs:line 53
   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task, ConfigureAwaitOptions options)
   at Xunit.v3.TestRunner`2.<>c__DisplayClass5_0.<<InvokeTest>b__1>d.MoveNext() in /_/src/xunit.v3.core/Runners/TestRunner.cs:line 170
--- End of stack trace from previous location ---
   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task, ConfigureAwaitOptions options)
   at Xunit.v3.ExceptionAggregator.RunAsync(Func`1 code) in /_/src/xunit.v3.core/Exceptions/ExceptionAggregator.cs:line 124


      GraphQueryServiceTests (1 test) [0:03.305] Failed: One or more child tests failed: 1 test failed
       FindShortestPathAsync_Should_Return_Null_For_No_Path [0:00.034] Failed: Shouldly.ShouldAssertException: False
Shouldly.ShouldAssertException
False
    should be
True
    but was not
   at IndFusion.SemanticRag.Tests.Implementations.GraphQueryServiceTests.FindShortestPathAsync_Should_Return_Null_For_No_Path() in /_/src/test/SemanticRagTests/IndFusion.SemanticRag.Tests/Implementations/GraphQueryServiceTests.cs:line 427
   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task, ConfigureAwaitOptions options)
   at Xunit.v3.TestRunner`2.<>c__DisplayClass5_0.<<InvokeTest>b__1>d.MoveNext() in /_/src/xunit.v3.core/Runners/TestRunner.cs:line 170
--- End of stack trace from previous location ---
   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task, ConfigureAwaitOptions options)
   at Xunit.v3.ExceptionAggregator.RunAsync(Func`1 code) in /_/src/xunit.v3.core/Exceptions/ExceptionAggregator.cs:line 124


      PatternGraphQueryServiceTests (4 tests) [0:03.351] Failed: One or more child tests failed: 4 tests failed
       FindPatternRelationshipsAsync_Should_Find_Pattern_Relationships [0:00.005] Failed: Shouldly.ShouldAssertException: 1f
Shouldly.ShouldAssertException
1f
    should be
0.8f
    but was not
   at IndFusion.SemanticRag.Tests.Implementations.PatternGraphQueryServiceTests.FindPatternRelationshipsAsync_Should_Find_Pattern_Relationships() in /_/src/test/SemanticRagTests/IndFusion.SemanticRag.Tests/Implementations/PatternGraphQueryServiceTests.cs:line 183
   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task, ConfigureAwaitOptions options)
   at Xunit.v3.TestRunner`2.<>c__DisplayClass5_0.<<InvokeTest>b__1>d.MoveNext() in /_/src/xunit.v3.core/Runners/TestRunner.cs:line 170
--- End of stack trace from previous location ---
   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task, ConfigureAwaitOptions options)
   at Xunit.v3.ExceptionAggregator.RunAsync(Func`1 code) in /_/src/xunit.v3.core/Exceptions/ExceptionAggregator.cs:line 124


       FindSimilarPatternsAsync_Should_Calculate_Similarity_Correctly [0:00.006] Failed: Shouldly.ShouldAssertException: 0
Shouldly.ShouldAssertException
0
    should be greater than
0
    but was not
   at IndFusion.SemanticRag.Tests.Implementations.PatternGraphQueryServiceTests.FindSimilarPatternsAsync_Should_Calculate_Similarity_Correctly() in /_/src/test/SemanticRagTests/IndFusion.SemanticRag.Tests/Implementations/PatternGraphQueryServiceTests.cs:line 644
   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task, ConfigureAwaitOptions options)
   at Xunit.v3.TestRunner`2.<>c__DisplayClass5_0.<<InvokeTest>b__1>d.MoveNext() in /_/src/xunit.v3.core/Runners/TestRunner.cs:line 170
--- End of stack trace from previous location ---
   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task, ConfigureAwaitOptions options)
   at Xunit.v3.ExceptionAggregator.RunAsync(Func`1 code) in /_/src/xunit.v3.core/Exceptions/ExceptionAggregator.cs:line 124


       FindSimilarPatternsAsync_Should_Find_Similar_Patterns [0:00.141] Failed: Shouldly.ShouldAssertException: 0
Shouldly.ShouldAssertException
0
    should be greater than
0
    but was not
   at IndFusion.SemanticRag.Tests.Implementations.PatternGraphQueryServiceTests.FindSimilarPatternsAsync_Should_Find_Similar_Patterns() in /_/src/test/SemanticRagTests/IndFusion.SemanticRag.Tests/Implementations/PatternGraphQueryServiceTests.cs:line 281
   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task, ConfigureAwaitOptions options)
   at Xunit.v3.TestRunner`2.<>c__DisplayClass5_0.<<InvokeTest>b__1>d.MoveNext() in /_/src/xunit.v3.core/Runners/TestRunner.cs:line 170
--- End of stack trace from previous location ---
   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task, ConfigureAwaitOptions options)
   at Xunit.v3.ExceptionAggregator.RunAsync(Func`1 code) in /_/src/xunit.v3.core/Exceptions/ExceptionAggregator.cs:line 124


       QueryPatternGraphAsync_Should_Execute_Pattern_Graph_Query [0:00.005] Failed: Shouldly.ShouldAssertException: 0L
Shouldly.ShouldAssertException
0L
    should be greater than
0L
    but was not
   at IndFusion.SemanticRag.Tests.Implementations.PatternGraphQueryServiceTests.QueryPatternGraphAsync_Should_Execute_Pattern_Graph_Query() in /_/src/test/SemanticRagTests/IndFusion.SemanticRag.Tests/Implementations/PatternGraphQueryServiceTests.cs:line 100
   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task, ConfigureAwaitOptions options)
   at Xunit.v3.TestRunner`2.<>c__DisplayClass5_0.<<InvokeTest>b__1>d.MoveNext() in /_/src/xunit.v3.core/Runners/TestRunner.cs:line 170
--- End of stack trace from previous location ---
   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task, ConfigureAwaitOptions options)
   at Xunit.v3.ExceptionAggregator.RunAsync(Func`1 code) in /_/src/xunit.v3.core/Exceptions/ExceptionAggregator.cs:line 124


      PatternSuggestServiceTests (2 tests) [0:03.300] Failed: One or more child tests failed: 2 tests failed
       AnalyzePatternAsync_Should_Analyze_Specific_Pattern [0:00.022] Failed: Shouldly.ShouldAssertException: 0L
Shouldly.ShouldAssertException
0L
    should be greater than
0L
    but was not
   at IndFusion.SemanticRag.Tests.Implementations.PatternSuggestServiceTests.AnalyzePatternAsync_Should_Analyze_Specific_Pattern() in /_/src/test/SemanticRagTests/IndFusion.SemanticRag.Tests/Implementations/PatternSuggestServiceTests.cs:line 220
   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task, ConfigureAwaitOptions options)
   at Xunit.v3.TestRunner`2.<>c__DisplayClass5_0.<<InvokeTest>b__1>d.MoveNext() in /_/src/xunit.v3.core/Runners/TestRunner.cs:line 170
--- End of stack trace from previous location ---
   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task, ConfigureAwaitOptions options)
   at Xunit.v3.ExceptionAggregator.RunAsync(Func`1 code) in /_/src/xunit.v3.core/Exceptions/ExceptionAggregator.cs:line 124


       SuggestPatternsAsync_Should_Use_ConfigureAwait_False [0:00.004] Failed: Shouldly.ShouldAssertException: False
Shouldly.ShouldAssertException
False
    should be
True
    but was not
   at IndFusion.SemanticRag.Tests.Implementations.PatternSuggestServiceTests.SuggestPatternsAsync_Should_Use_ConfigureAwait_False() in /_/src/test/SemanticRagTests/IndFusion.SemanticRag.Tests/Implementations/PatternSuggestServiceTests.cs:line 564
   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task, ConfigureAwaitOptions options)
   at Xunit.v3.TestRunner`2.<>c__DisplayClass5_0.<<InvokeTest>b__1>d.MoveNext() in /_/src/xunit.v3.core/Runners/TestRunner.cs:line 170
--- End of stack trace from previous location ---
   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task, ConfigureAwaitOptions options)
   at Xunit.v3.ExceptionAggregator.RunAsync(Func`1 code) in /_/src/xunit.v3.core/Exceptions/ExceptionAggregator.cs:line 124


     Integration (3 tests) [0:03.795] Failed: 3 tests failed
      GraphRagLayerIntegrationTests (3 tests) [0:03.795] Failed: One or more child tests failed: 3 tests failed
       Error_Handling_Workflow_Should_Handle_Failures_Gracefully [0:00.006] Failed: Shouldly.ShouldAssertException: False
Shouldly.ShouldAssertException
False
    should be
True
    but was not
   at IndFusion.SemanticRag.Tests.Integration.GraphRagLayerIntegrationTests.Error_Handling_Workflow_Should_Handle_Failures_Gracefully() in /_/src/test/SemanticRagTests/IndFusion.SemanticRag.Tests/Integration/GraphRagLayerIntegrationTests.cs:line 327
   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task, ConfigureAwaitOptions options)
   at Xunit.v3.TestRunner`2.<>c__DisplayClass5_0.<<InvokeTest>b__1>d.MoveNext() in /_/src/xunit.v3.core/Runners/TestRunner.cs:line 170
--- End of stack trace from previous location ---
   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task, ConfigureAwaitOptions options)
   at Xunit.v3.ExceptionAggregator.RunAsync(Func`1 code) in /_/src/xunit.v3.core/Exceptions/ExceptionAggregator.cs:line 124


       Pattern_Suggestion_Workflow_Should_Handle_Complex_Code [0:00.003] Failed: Shouldly.ShouldAssertException: 0
Shouldly.ShouldAssertException
0
    should be greater than
0
    but was not
   at IndFusion.SemanticRag.Tests.Integration.GraphRagLayerIntegrationTests.Pattern_Suggestion_Workflow_Should_Handle_Complex_Code() in /_/src/test/SemanticRagTests/IndFusion.SemanticRag.Tests/Integration/GraphRagLayerIntegrationTests.cs:line 232
   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task, ConfigureAwaitOptions options)
   at Xunit.v3.TestRunner`2.<>c__DisplayClass5_0.<<InvokeTest>b__1>d.MoveNext() in /_/src/xunit.v3.core/Runners/TestRunner.cs:line 170
--- End of stack trace from previous location ---
   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task, ConfigureAwaitOptions options)
   at Xunit.v3.ExceptionAggregator.RunAsync(Func`1 code) in /_/src/xunit.v3.core/Exceptions/ExceptionAggregator.cs:line 124


       Performance_Workflow_Should_Complete_Within_Reasonable_Time [0:00.013] Failed: Shouldly.ShouldAssertException: False
Shouldly.ShouldAssertException
False
    should be
True
    but was not
   at IndFusion.SemanticRag.Tests.Integration.GraphRagLayerIntegrationTests.Performance_Workflow_Should_Complete_Within_Reasonable_Time() in /_/src/test/SemanticRagTests/IndFusion.SemanticRag.Tests/Integration/GraphRagLayerIntegrationTests.cs:line 412
   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task, ConfigureAwaitOptions options)
   at Xunit.v3.TestRunner`2.<>c__DisplayClass5_0.<<InvokeTest>b__1>d.MoveNext() in /_/src/xunit.v3.core/Runners/TestRunner.cs:line 170
--- End of stack trace from previous location ---
   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task, ConfigureAwaitOptions options)
   at Xunit.v3.ExceptionAggregator.RunAsync(Func`1 code) in /_/src/xunit.v3.core/Exceptions/ExceptionAggregator.cs:line 124


     Interfaces (1 test) [0:22.629] Failed: 1 test failed
      IGraphQueryServiceTests (1 test) [0:03.298] Failed: One or more child tests failed: 1 test failed
       FindShortestPathAsync_Should_Return_Null_For_No_Path [0:00.093] Failed: Shouldly.ShouldAssertException: False
Shouldly.ShouldAssertException
False
    should be
True
    but was not
   at IndFusion.SemanticRag.Tests.Interfaces.IGraphQueryServiceTests.FindShortestPathAsync_Should_Return_Null_For_No_Path() in /_/src/test/SemanticRagTests/IndFusion.SemanticRag.Tests/Interfaces/IGraphQueryServiceTests.cs:line 276
   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task, ConfigureAwaitOptions options)
   at Xunit.v3.TestRunner`2.<>c__DisplayClass5_0.<<InvokeTest>b__1>d.MoveNext() in /_/src/xunit.v3.core/Runners/TestRunner.cs:line 170
--- End of stack trace from previous location ---
   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task, ConfigureAwaitOptions options)
   at Xunit.v3.ExceptionAggregator.RunAsync(Func`1 code) in /_/src/xunit.v3.core/Exceptions/ExceptionAggregator.cs:line 124


   IndFusion.SemanticRag.Tests.Infratructure.Tests (8 tests) [0:02.110] Failed: 8 tests failed
    IndFusion.SemanticRag.Tests.Infratructure.Tests (8 tests) [0:02.110] Failed: 8 tests failed
     Infrastructure.Adapters (8 tests) [0:00.723] Failed: 8 tests failed
      Neo4jKnowledgeGraphAdapterTests (8 tests) [0:00.429] Failed: One or more child tests failed: 8 tests failed
       ExecuteGraphQueryAsync_Should_ReturnFailure_When_QueryFails [0:00.004] Failed: Shouldly.ShouldAssertException: "KG008: GetAsyncEnumerator not implemented in mock"
Shouldly.ShouldAssertException
"KG008: GetAsyncEnumerator not implemented in mock"
    should be
"KG008"
    but was not
    difference
Difference     |       |    |    |    |    |    |    |    |    |    |    |    |    |    |    |    |    |    |    |    |    |        
               |      \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/       
Index          | ...  5    6    7    8    9    10   11   12   13   14   15   16   17   18   19   20   21   22   23   24   25   ...  
Expected Value | ...                                                                                                           ...  
Actual Value   | ...  :    \s   G    e    t    A    s    y    n    c    E    n    u    m    e    r    a    t    o    r    \s   ...  
Expected Code  | ...                                                                                                           ...  
Actual Code    | ...  58   32   71   101  116  65   115  121  110  99   69   110  117  109  101  114  97   116  111  114  32   ...  

Difference     |       |    |    |    |    |    |    |    |    |    |    |    |    |    |    |    |    |    |    |    |    |        
               |      \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/       
Index          | ...  26   27   28   29   30   31   32   33   34   35   36   37   38   39   40   41   42   43   44   45   46   ...  
Expected Value | ...                                                                                                           ...  
Actual Value   | ...  n    o    t    \s   i    m    p    l    e    m    e    n    t    e    d    \s   i    n    \s   m    o    ...  
Expected Code  | ...                                                                                                           ...  
Actual Code    | ...  110  111  116  32   105  109  112  108  101  109  101  110  116  101  100  32   105  110  32   109  111  ...  

Difference     |       |    |    |    |    |    |    |    |    |    |    |    |    |    |    |    |    |    |    |    |    |   
               |      \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  
Index          | ...  28   29   30   31   32   33   34   35   36   37   38   39   40   41   42   43   44   45   46   47   48   
Expected Value | ...                                                                                                           
Actual Value   | ...  t    \s   i    m    p    l    e    m    e    n    t    e    d    \s   i    n    \s   m    o    c    k    
Expected Code  | ...                                                                                                           
Actual Code    | ...  116  32   105  109  112  108  101  109  101  110  116  101  100  32   105  110  32   109  111  99   107  

Additional Info:
    Expected error code 'KG008', but got 'KG008: GetAsyncEnumerator not implemented in mock'
   at IndFusion.SemanticRag.Tests.Infratructure.Tests.Helpers.ResultAssertions.ShouldFailWith[T](Result`1 result, String expectedErrorCode) in /_/src/test/SemanticRagTests/IndFusion.SemanticRag.Infratructure.Tests/Helpers/ResultAssertions.cs:line 58
   at IndFusion.SemanticRag.Tests.Infratructure.Tests.Infrastructure.Adapters.Neo4jKnowledgeGraphAdapterTests.ExecuteGraphQueryAsync_Should_ReturnFailure_When_QueryFails() in /_/src/test/SemanticRagTests/IndFusion.SemanticRag.Infratructure.Tests/Infrastructure/Adapters/Neo4jKnowledgeGraphAdapterTests.cs:line 389
   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task, ConfigureAwaitOptions options)
   at Xunit.v3.TestRunner`2.<>c__DisplayClass5_0.<<InvokeTest>b__1>d.MoveNext() in /_/src/xunit.v3.core/Runners/TestRunner.cs:line 170
--- End of stack trace from previous location ---
   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task, ConfigureAwaitOptions options)
   at Xunit.v3.ExceptionAggregator.RunAsync(Func`1 code) in /_/src/xunit.v3.core/Exceptions/ExceptionAggregator.cs:line 124


       ExecuteGraphQueryAsync_Should_ReturnResults_When_QueryExecutesSuccessfully [0:00.005] Failed: Shouldly.ShouldAssertException: False
Shouldly.ShouldAssertException
False
    should be
True
    but was not

Additional Info:
    Expected result to be successful, but it failed with error: KG008: GetAsyncEnumerator not implemented in mock
   at IndFusion.SemanticRag.Tests.Infratructure.Tests.Helpers.ResultAssertions.ShouldSucceed[T](Result`1 result) in /_/src/test/SemanticRagTests/IndFusion.SemanticRag.Infratructure.Tests/Helpers/ResultAssertions.cs:line 17
   at IndFusion.SemanticRag.Tests.Infratructure.Tests.Infrastructure.Adapters.Neo4jKnowledgeGraphAdapterTests.ExecuteGraphQueryAsync_Should_ReturnResults_When_QueryExecutesSuccessfully() in /_/src/test/SemanticRagTests/IndFusion.SemanticRag.Infratructure.Tests/Infrastructure/Adapters/Neo4jKnowledgeGraphAdapterTests.cs:line 373
   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task, ConfigureAwaitOptions options)
   at Xunit.v3.TestRunner`2.<>c__DisplayClass5_0.<<InvokeTest>b__1>d.MoveNext() in /_/src/xunit.v3.core/Runners/TestRunner.cs:line 170
--- End of stack trace from previous location ---
   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task, ConfigureAwaitOptions options)
   at Xunit.v3.ExceptionAggregator.RunAsync(Func`1 code) in /_/src/xunit.v3.core/Exceptions/ExceptionAggregator.cs:line 124


       GetNodeByIdAsync_Should_ReturnFailure_When_Neo4jThrowsException [0:00.015] Failed: Shouldly.ShouldAssertException: "KG008: GetAsyncEnumerator not implemented in mock"
Shouldly.ShouldAssertException
"KG008: GetAsyncEnumerator not implemented in mock"
    should be
"KG007"
    but was not
    difference
Difference     |       |    |    |    |    |    |    |    |    |    |    |    |    |    |    |    |    |    |    |    |    |        
               |      \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/       
Index          | ...  4    5    6    7    8    9    10   11   12   13   14   15   16   17   18   19   20   21   22   23   24   ...  
Expected Value | ...  7                                                                                                        ...  
Actual Value   | ...  8    :    \s   G    e    t    A    s    y    n    c    E    n    u    m    e    r    a    t    o    r    ...  
Expected Code  | ...  55                                                                                                       ...  
Actual Code    | ...  56   58   32   71   101  116  65   115  121  110  99   69   110  117  109  101  114  97   116  111  114  ...  

Difference     |       |    |    |    |    |    |    |    |    |    |    |    |    |    |    |    |    |    |    |    |    |        
               |      \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/       
Index          | ...  25   26   27   28   29   30   31   32   33   34   35   36   37   38   39   40   41   42   43   44   45   ...  
Expected Value | ...                                                                                                           ...  
Actual Value   | ...  \s   n    o    t    \s   i    m    p    l    e    m    e    n    t    e    d    \s   i    n    \s   m    ...  
Expected Code  | ...                                                                                                           ...  
Actual Code    | ...  32   110  111  116  32   105  109  112  108  101  109  101  110  116  101  100  32   105  110  32   109  ...  

Difference     |       |    |    |    |    |    |    |    |    |    |    |    |    |    |    |    |    |    |    |    |    |   
               |      \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  
Index          | ...  28   29   30   31   32   33   34   35   36   37   38   39   40   41   42   43   44   45   46   47   48   
Expected Value | ...                                                                                                           
Actual Value   | ...  t    \s   i    m    p    l    e    m    e    n    t    e    d    \s   i    n    \s   m    o    c    k    
Expected Code  | ...                                                                                                           
Actual Code    | ...  116  32   105  109  112  108  101  109  101  110  116  101  100  32   105  110  32   109  111  99   107  

Additional Info:
    Expected error code 'KG007', but got 'KG008: GetAsyncEnumerator not implemented in mock'
   at IndFusion.SemanticRag.Tests.Infratructure.Tests.Helpers.ResultAssertions.ShouldFailWith[T](Result`1 result, String expectedErrorCode) in /_/src/test/SemanticRagTests/IndFusion.SemanticRag.Infratructure.Tests/Helpers/ResultAssertions.cs:line 58
   at IndFusion.SemanticRag.Tests.Infratructure.Tests.Infrastructure.Adapters.Neo4jKnowledgeGraphAdapterTests.GetNodeByIdAsync_Should_ReturnFailure_When_Neo4jThrowsException() in /_/src/test/SemanticRagTests/IndFusion.SemanticRag.Infratructure.Tests/Infrastructure/Adapters/Neo4jKnowledgeGraphAdapterTests.cs:line 285
   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task, ConfigureAwaitOptions options)
   at Xunit.v3.TestRunner`2.<>c__DisplayClass5_0.<<InvokeTest>b__1>d.MoveNext() in /_/src/xunit.v3.core/Runners/TestRunner.cs:line 170
--- End of stack trace from previous location ---
   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task, ConfigureAwaitOptions options)
   at Xunit.v3.ExceptionAggregator.RunAsync(Func`1 code) in /_/src/xunit.v3.core/Exceptions/ExceptionAggregator.cs:line 124


       GetNodeByIdAsync_Should_ReturnFailure_When_NodeNotFound [0:00.001] Failed: Shouldly.ShouldAssertException: "KG008: GetAsyncEnumerator not implemented in mock"
Shouldly.ShouldAssertException
"KG008: GetAsyncEnumerator not implemented in mock"
    should be
"KG009"
    but was not
    difference
Difference     |       |    |    |    |    |    |    |    |    |    |    |    |    |    |    |    |    |    |    |    |    |        
               |      \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/       
Index          | ...  4    5    6    7    8    9    10   11   12   13   14   15   16   17   18   19   20   21   22   23   24   ...  
Expected Value | ...  9                                                                                                        ...  
Actual Value   | ...  8    :    \s   G    e    t    A    s    y    n    c    E    n    u    m    e    r    a    t    o    r    ...  
Expected Code  | ...  57                                                                                                       ...  
Actual Code    | ...  56   58   32   71   101  116  65   115  121  110  99   69   110  117  109  101  114  97   116  111  114  ...  

Difference     |       |    |    |    |    |    |    |    |    |    |    |    |    |    |    |    |    |    |    |    |    |        
               |      \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/       
Index          | ...  25   26   27   28   29   30   31   32   33   34   35   36   37   38   39   40   41   42   43   44   45   ...  
Expected Value | ...                                                                                                           ...  
Actual Value   | ...  \s   n    o    t    \s   i    m    p    l    e    m    e    n    t    e    d    \s   i    n    \s   m    ...  
Expected Code  | ...                                                                                                           ...  
Actual Code    | ...  32   110  111  116  32   105  109  112  108  101  109  101  110  116  101  100  32   105  110  32   109  ...  

Difference     |       |    |    |    |    |    |    |    |    |    |    |    |    |    |    |    |    |    |    |    |    |   
               |      \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  \|/  
Index          | ...  28   29   30   31   32   33   34   35   36   37   38   39   40   41   42   43   44   45   46   47   48   
Expected Value | ...                                                                                                           
Actual Value   | ...  t    \s   i    m    p    l    e    m    e    n    t    e    d    \s   i    n    \s   m    o    c    k    
Expected Code  | ...                                                                                                           
Actual Code    | ...  116  32   105  109  112  108  101  109  101  110  116  101  100  32   105  110  32   109  111  99   107  

Additional Info:
    Expected error code 'KG009', but got 'KG008: GetAsyncEnumerator not implemented in mock'
   at IndFusion.SemanticRag.Tests.Infratructure.Tests.Helpers.ResultAssertions.ShouldFailWith[T](Result`1 result, String expectedErrorCode) in /_/src/test/SemanticRagTests/IndFusion.SemanticRag.Infratructure.Tests/Helpers/ResultAssertions.cs:line 58
   at IndFusion.SemanticRag.Tests.Infratructure.Tests.Infrastructure.Adapters.Neo4jKnowledgeGraphAdapterTests.GetNodeByIdAsync_Should_ReturnFailure_When_NodeNotFound() in /_/src/test/SemanticRagTests/IndFusion.SemanticRag.Infratructure.Tests/Infrastructure/Adapters/Neo4jKnowledgeGraphAdapterTests.cs:line 271
   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task, ConfigureAwaitOptions options)
   at Xunit.v3.TestRunner`2.<>c__DisplayClass5_0.<<InvokeTest>b__1>d.MoveNext() in /_/src/xunit.v3.core/Runners/TestRunner.cs:line 170
--- End of stack trace from previous location ---
   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task, ConfigureAwaitOptions options)
   at Xunit.v3.ExceptionAggregator.RunAsync(Func`1 code) in /_/src/xunit.v3.core/Exceptions/ExceptionAggregator.cs:line 124


       GetNodeByIdAsync_Should_ReturnNode_When_NodeExists [0:00.006] Failed: Shouldly.ShouldAssertException: False
Shouldly.ShouldAssertException
False
    should be
True
    but was not

Additional Info:
    Expected result to be successful, but it failed with error: KG008: GetAsyncEnumerator not implemented in mock
   at IndFusion.SemanticRag.Tests.Infratructure.Tests.Helpers.ResultAssertions.ShouldSucceed[T](Result`1 result) in /_/src/test/SemanticRagTests/IndFusion.SemanticRag.Infratructure.Tests/Helpers/ResultAssertions.cs:line 17
   at IndFusion.SemanticRag.Tests.Infratructure.Tests.Infrastructure.Adapters.Neo4jKnowledgeGraphAdapterTests.GetNodeByIdAsync_Should_ReturnNode_When_NodeExists() in /_/src/test/SemanticRagTests/IndFusion.SemanticRag.Infratructure.Tests/Infrastructure/Adapters/Neo4jKnowledgeGraphAdapterTests.cs:line 255
   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task, ConfigureAwaitOptions options)
   at Xunit.v3.TestRunner`2.<>c__DisplayClass5_0.<<InvokeTest>b__1>d.MoveNext() in /_/src/xunit.v3.core/Runners/TestRunner.cs:line 170
--- End of stack trace from previous location ---
   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task, ConfigureAwaitOptions options)
   at Xunit.v3.ExceptionAggregator.RunAsync(Func`1 code) in /_/src/xunit.v3.core/Exceptions/ExceptionAggregator.cs:line 124


       GetRelationshipsForNodeAsync_Should_ReturnEmptyList_When_NodeHasNoRelationships [0:00.001] Failed: Shouldly.ShouldAssertException: False
Shouldly.ShouldAssertException
False
    should be
True
    but was not

Additional Info:
    Expected result to be successful, but it failed with error: KG008: GetAsyncEnumerator not implemented in mock
   at IndFusion.SemanticRag.Tests.Infratructure.Tests.Helpers.ResultAssertions.ShouldSucceed[T](Result`1 result) in /_/src/test/SemanticRagTests/IndFusion.SemanticRag.Infratructure.Tests/Helpers/ResultAssertions.cs:line 17
   at IndFusion.SemanticRag.Tests.Infratructure.Tests.Infrastructure.Adapters.Neo4jKnowledgeGraphAdapterTests.GetRelationshipsForNodeAsync_Should_ReturnEmptyList_When_NodeHasNoRelationships() in /_/src/test/SemanticRagTests/IndFusion.SemanticRag.Infratructure.Tests/Infrastructure/Adapters/Neo4jKnowledgeGraphAdapterTests.cs:line 340
   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task, ConfigureAwaitOptions options)
   at Xunit.v3.TestRunner`2.<>c__DisplayClass5_0.<<InvokeTest>b__1>d.MoveNext() in /_/src/xunit.v3.core/Runners/TestRunner.cs:line 170
--- End of stack trace from previous location ---
   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task, ConfigureAwaitOptions options)
   at Xunit.v3.ExceptionAggregator.RunAsync(Func`1 code) in /_/src/xunit.v3.core/Exceptions/ExceptionAggregator.cs:line 124


       GetRelationshipsForNodeAsync_Should_ReturnRelationships_When_NodeHasRelationships [0:00.091] Failed: Shouldly.ShouldAssertException: False
Shouldly.ShouldAssertException
False
    should be
True
    but was not

Additional Info:
    Expected result to be successful, but it failed with error: KG008: GetAsyncEnumerator not implemented in mock
   at IndFusion.SemanticRag.Tests.Infratructure.Tests.Helpers.ResultAssertions.ShouldSucceed[T](Result`1 result) in /_/src/test/SemanticRagTests/IndFusion.SemanticRag.Infratructure.Tests/Helpers/ResultAssertions.cs:line 17
   at IndFusion.SemanticRag.Tests.Infratructure.Tests.Infrastructure.Adapters.Neo4jKnowledgeGraphAdapterTests.GetRelationshipsForNodeAsync_Should_ReturnRelationships_When_NodeHasRelationships() in /_/src/test/SemanticRagTests/IndFusion.SemanticRag.Infratructure.Tests/Infrastructure/Adapters/Neo4jKnowledgeGraphAdapterTests.cs:line 323
   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task, ConfigureAwaitOptions options)
   at Xunit.v3.TestRunner`2.<>c__DisplayClass5_0.<<InvokeTest>b__1>d.MoveNext() in /_/src/xunit.v3.core/Runners/TestRunner.cs:line 170
--- End of stack trace from previous location ---
   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task, ConfigureAwaitOptions options)
   at Xunit.v3.ExceptionAggregator.RunAsync(Func`1 code) in /_/src/xunit.v3.core/Exceptions/ExceptionAggregator.cs:line 124


       StoreNodesAsync_Should_ReturnFailure_When_AnyNodeValidationFails [0:00.002] Failed: Shouldly.ShouldAssertException: False
Shouldly.ShouldAssertException
False
    should be
True
    but was not

Additional Info:
    Expected result to be a failure, but it was successful
   at IndFusion.SemanticRag.Tests.Infratructure.Tests.Helpers.ResultAssertions.ShouldFail(Result result) in /_/src/test/SemanticRagTests/IndFusion.SemanticRag.Infratructure.Tests/Helpers/ResultAssertions.cs:line 76
   at IndFusion.SemanticRag.Tests.Infratructure.Tests.Helpers.ResultAssertions.ShouldFailWith(Result result, String expectedErrorCode) in /_/src/test/SemanticRagTests/IndFusion.SemanticRag.Infratructure.Tests/Helpers/ResultAssertions.cs:line 87
   at IndFusion.SemanticRag.Tests.Infratructure.Tests.Infrastructure.Adapters.Neo4jKnowledgeGraphAdapterTests.StoreNodesAsync_Should_ReturnFailure_When_AnyNodeValidationFails() in /_/src/test/SemanticRagTests/IndFusion.SemanticRag.Infratructure.Tests/Infrastructure/Adapters/Neo4jKnowledgeGraphAdapterTests.cs:line 161
   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task, ConfigureAwaitOptions options)
   at Xunit.v3.TestRunner`2.<>c__DisplayClass5_0.<<InvokeTest>b__1>d.MoveNext() in /_/src/xunit.v3.core/Runners/TestRunner.cs:line 170
--- End of stack trace from previous location ---
   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task, ConfigureAwaitOptions options)
   at Xunit.v3.ExceptionAggregator.RunAsync(Func`1 code) in /_/src/xunit.v3.core/Exceptions/ExceptionAggregator.cs:line 124

