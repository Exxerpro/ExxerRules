xUnit.net v3 Microsoft.Testing.Platform Runner v3.0.1+9214bef154 (64-bit .NET 10.0.0-rc.2.25502.107)

failed IndFusion.Mcp.Tests.Architecture.NamingConventionTests.ServiceAsyncMethods_ShouldHaveAsyncSuffix (123ms)
  Shouldly.ShouldAssertException : ["IndFusion.Mcp.Core.Services.ToolCallLogger.Playback"]
      should be empty but had
  1
      item and was not empty

  Additional Info:
      Service async methods should end with 'Async': IndFusion.Mcp.Core.Services.ToolCallLogger.Playback
    at IndFusion.Mcp.Tests.Architecture.NamingConventionTests.ServiceAsyncMethods_ShouldHaveAsyncSuffix() in /_/src/test/IndFusion.Mcp.Tests/Architecture/NamingConventionTests.cs:59
    at System.Reflection.MethodBaseInvoker.InterpretedInvoke_Method(Object obj, IntPtr* args)
    at System.Reflection.MethodBaseInvoker.InvokeWithNoArgs(Object obj, BindingFlags invokeAttr)
failed (canceled) IndFusion.Mcp.Tests.MetricsResourceTests.ReadMetrics_Class_ReturnsClassMetrics (30s 112ms)
  Xunit.Runner.InProc.SystemConsole.TestingPlatform.XunitException: Test execution timed out after 30000 milliseconds
    --- End of stack trace from previous location ---
failed (canceled) IndFusion.Mcp.Tests.ToolsNew.LoadSolutionToolTests.UnloadSolution_RemovesCachedSolution (30s 112ms)
  Xunit.Runner.InProc.SystemConsole.TestingPlatform.XunitException: Test execution timed out after 30000 milliseconds
    --- End of stack trace from previous location ---

[✓171/x3/↓0] IndFusion.Mcp.Tests.dll (net10.0|x64)                                                                                                              (49s)
  IndFusion.Mcp.Tests.ClassLengthMetricsTests.ListClassLengths_ReturnsMetrics                                                                                   (46s)
failed IndFusion.Mcp.Tests.MetricsResourceTests.ReadMetrics_Method_ReturnsMethodMetrics (17s 136ms)
  Xunit.Runner.InProc.SystemConsole.TestingPlatform.XunitException: System.Collections.Generic.KeyNotFoundException : The given key was not present in the dictionary.
    at System.Text.Json.JsonElement.GetProperty(String propertyName)
    at IndFusion.Mcp.Tests.MetricsResourceTests.ReadMetrics_Method_ReturnsMethodMetrics() in /_/src/test/IndFusion.Mcp.Tests/MetricsResourceTests.cs:81
    --- End of stack trace from previous location ---

[✓193/x4/↓0] IndFusion.Mcp.Tests.dll (net10.0|x64)                                                                                                              (54s)
  IndFusion.Mcp.Tests.ClassLengthMetricsTests.ListClassLengths_ReturnsMetrics                                                                                   (51s)
[✓193/x4/↓0] IndFusion.Mcp.Tests.dll (net10.0|x64)                                                                                                              (55s)
  IndFusion.Mcp.Tests.ClassLengthMetricsTests.ListClassLengths_ReturnsMetrics                                                                                   (51s)
[✓193/x4/↓0] IndFusion.Mcp.Tests.dll (net10.0|x64)                                                                                                              (56s)
  IndFusion.Mcp.Tests.ClassLengthMetricsTests.ListClassLengths_ReturnsMetrics                                                                                   (52s)
  IndFusion.Mcp.Tests.AnalyzeExxerFactoringOpportunitiesTests.AnalyzeExampleCode_ReturnsSuggestions                                                             (52s)
[✓193/x4/↓0] IndFusion.Mcp.Tests.dll (net10.0|x64)                                                                                                              (56s)
[✓193/x4/↓0] IndFusion.Mcp.Tests.dll (net10.0|x64)                                                                                                              (57s)
  IndFusion.Mcp.Tests.ClassLengthMetricsTests.ListClassLengths_ReturnsMetrics                                                                                   (54s)
  IndFusion.Mcp.Tests.AnalyzeExxerFactoringOpportunitiesTests.AnalyzeExampleCode_ReturnsSuggestions                                                             (53s)
  IndFusion.Mcp.Tests.Services.LintingServiceTests.RunLintingAsync_WithScope_ReturnsFilteredResults                                                             (42s)
  IndFusion.Mcp.Tests.Tools.IntroduceParameterTests.IntroduceParameter_InvalidMethod_ReturnsError                                                               (29s)
[✓195/x4/↓0] IndFusion.Mcp.Tests.dll (net10.0|x64)                                                                                                              (59s)
  IndFusion.Mcp.Tests.ClassLengthMetricsTests.ListClassLengths_ReturnsMetrics                                                                                   (55s)
  IndFusion.Mcp.Tests.AnalyzeExxerFactoringOpportunitiesTests.AnalyzeExampleCode_ReturnsSuggestions                                                             (54s)
  IndFusion.Mcp.Tests.Services.LintingServiceTests.RunLintingAsync_WithScope_ReturnsFilteredResults                                                             (43s)
  IndFusion.Mcp.Tests.ToolsNew.LintRunToolTests.LintRun_WithProgressReporter_CallsProgressCallback                                                              (29s)
failed (canceled) IndFusion.Mcp.Tests.ToolsNew.LintRunToolTests.LintRun_WithProgressReporter_CallsProgressCallback (30s 606ms)
  Xunit.Runner.InProc.SystemConsole.TestingPlatform.XunitException: Test execution timed out after 30000 milliseconds
    --- End of stack trace from previous location ---
failed (canceled) IndFusion.Mcp.Tests.MetricsProviderTests.GetFileMetrics_CachesToDiskAndMemory (30s 009ms)
  Xunit.Runner.InProc.SystemConsole.TestingPlatform.XunitException: Test execution timed out after 30000 milliseconds
    --- End of stack trace from previous location ---
failed (canceled) IndFusion.Mcp.Tests.ToolsNew.LoadSolutionToolTests.LoadSolution_ValidPath_ReturnsSuccess (30s 004ms)
  Xunit.Runner.InProc.SystemConsole.TestingPlatform.XunitException: Test execution timed out after 30000 milliseconds
    --- End of stack trace from previous location ---
failed IndFusion.Mcp.Tests.Tools.MoveMultipleMethodsConstructorInjectionTests.MoveMultipleMethods_ConstructorInjection_UsesThis (25s 820ms)
  Assert.DoesNotContain() Failure: Sub-string found
                                        ↓ (pos 43)
  String: ···"\n    private readonly cA _a;\r\n\r\n    public static "···
  Found:  "_a"
    at IndFusion.Mcp.Tests.Tools.MoveMultipleMethodsConstructorInjectionTests.MoveMultipleMethods_ConstructorInjection_UsesThis() in /_/src/test/IndFusion.Mcp.Tests/Tools/MoveMultipleMethodsConstructorInjectionTests.cs:39
    --- End of stack trace from previous location ---

[✓216/x8/↓0] IndFusion.Mcp.Tests.dll (net10.0|x64)                                                                                                           (1m 11s)
  IndFusion.Mcp.Tests.ClassLengthMetricsTests.ListClassLengths_ReturnsMetrics                                                                                (1m 08s)
[✓218/x8/↓0] IndFusion.Mcp.Tests.dll (net10.0|x64)                                                                                                           (1m 14s)
failed IndFusion.Mcp.Tests.MetricsResourceTests.ReadMetrics_File_ReturnsJson (10s 129ms)
  Assert.True() Failure
  Expected: True
  Actual:   False
    at IndFusion.Mcp.Tests.MetricsResourceTests.ReadMetrics_File_ReturnsJson() in /_/src/test/IndFusion.Mcp.Tests/MetricsResourceTests.cs:31
    --- End of stack trace from previous location ---

[✓229/x9/↓0] IndFusion.Mcp.Tests.dll (net10.0|x64)                                                                                                           (1m 24s)
  IndFusion.Mcp.Tests.ClassLengthMetricsTests.ListClassLengths_ReturnsMetrics                                                                                (1m 21s)
  IndFusion.Mcp.Tests.AnalyzeExxerFactoringOpportunitiesTests.AnalyzeExampleCode_ReturnsSuggestions                                                          (1m 20s)
failed IndFusion.Mcp.Tests.MetricsResourceTests.ReadMetrics_Directory_ReturnsAggregatedJson (9s 241ms)
  Assert.Contains() Failure: Filter not matched in collection
  Collection: [{
      "name": "B",
      "linesOfCode": 43,
      "methods": [
        {
          "name": "Get",
          "linesOfCode": 2,
          "parameterCount": 0,
          "cyclomaticComplexity": 1,
          "maxNestingDepth": 0
        },
        {
          "name": "Add",
          "linesOfCode": 2,
          "parameterCount": 1,
          "cyclomaticComplexity": 1,
          "maxNestingDepth": 0
        },
        {
          "name": "BaseAdd",
          "linesOfCode": 3,
          "parameterCount": 2,
          "cyclomaticComplexity": 1,
          "maxNestingDepth": 0
        },
        {
          "name": "Get",
          "linesOfCode": 2,
          "parameterCount": 0,
          "cyclomaticComplexity": 1,
          "maxNestingDepth": 0
        },
        {
          "name": "BaseGet",
          "linesOfCode": 3,
          "parameterCount": 1,
          "cyclomaticComplexity": 1,
          "maxNestingDepth": 0
        },
        {
          "name": "Add",
          "linesOfCode": 2,
          "parameterCount": 1,
          "cyclomaticComplexity": 1,
          "maxNestingDepth": 0
        },
        {
          "name": "Get",
          "linesOfCode": 2,
          "parameterCount": 1,
          "cyclomaticComplexity": 1,
          "maxNestingDepth": 0
        },
        {
          "name": "Add",
          "linesOfCode": 2,
          "parameterCount": 2,
          "cyclomaticComplexity": 1,
          "maxNestingDepth": 0
        },
        {
          "name": "Get",
          "linesOfCode": 2,
          "parameterCount": 1,
          "cyclomaticComplexity": 1,
          "maxNestingDepth": 0
        },
        {
          "name": "Add",
          "linesOfCode": 2,
          "parameterCount": 2,
          "cyclomaticComplexity": 1,
          "maxNestingDepth": 0
        },
        {
          "name": "Get",
          "linesOfCode": 2,
          "parameterCount": 1,
          "cyclomaticComplexity": 1,
          "maxNestingDepth": 0
        },
        {
          "name": "Add",
          "linesOfCode": 2,
          "parameterCount": 2,
          "cyclomaticComplexity": 1,
          "maxNestingDepth": 0
        },
        {
          "name": "Get",
          "linesOfCode": 2,
          "parameterCount": 1,
          "cyclomaticComplexity": 1,
          "maxNestingDepth": 0
        },
        {
          "name": "Add",
          "linesOfCode": 2,
          "parameterCount": 2,
          "cyclomaticComplexity": 1,
          "maxNestingDepth": 0
        }
      ]
    }, {
      "name": "MathUtilities",
      "linesOfCode": 67,
      "methods": [
        {
          "name": "FormatCurrency",
          "linesOfCode": 4,
          "parameterCount": 1,
          "cyclomaticComplexity": 1,
          "maxNestingDepth": 0
        },
        {
          "name": "LogOperation",
          "linesOfCode": 4,
          "parameterCount": 1,
          "cyclomaticComplexity": 1,
          "maxNestingDepth": 0
        },
        {
          "name": "BaseLogOperation",
          "linesOfCode": 3,
          "parameterCount": 1,
          "cyclomaticComplexity": 1,
          "maxNestingDepth": 0
        },
        {
          "name": "FormatCurrency",
          "linesOfCode": 4,
          "parameterCount": 1,
          "cyclomaticComplexity": 1,
          "maxNestingDepth": 0
        },
        {
          "name": "BaseFormatCurrency",
          "linesOfCode": 3,
          "parameterCount": 1,
          "cyclomaticComplexity": 1,
          "maxNestingDepth": 0
        },
        {
          "name": "LogOperation",
          "linesOfCode": 4,
          "parameterCount": 1,
          "cyclomaticComplexity": 1,
          "maxNestingDepth": 0
        },
        {
          "name": "FormatCurrency",
          "linesOfCode": 4,
          "parameterCount": 1,
          "cyclomaticComplexity": 1,
          "maxNestingDepth": 0
        },
        {
          "name": "LogOperation",
          "linesOfCode": 4,
          "parameterCount": 1,
          "cyclomaticComplexity": 1,
          "maxNestingDepth": 0
        },
        {
          "name": "FormatCurrency",
          "linesOfCode": 4,
          "parameterCount": 1,
          "cyclomaticComplexity": 1,
          "maxNestingDepth": 0
        },
        {
          "name": "LogOperation",
          "linesOfCode": 4,
          "parameterCount": 1,
          "cyclomaticComplexity": 1,
          "maxNestingDepth": 0
        },
        {
          "name": "FormatCurrency",
          "linesOfCode": 4,
          "parameterCount": 1,
          "cyclomaticComplexity": 1,
          "maxNestingDepth": 0
        },
        {
          "name": "LogOperation",
          "linesOfCode": 4,
          "parameterCount": 1,
          "cyclomaticComplexity": 1,
          "maxNestingDepth": 0
        },
        {
          "name": "FormatCurrency",
          "linesOfCode": 4,
          "parameterCount": 1,
          "cyclomaticComplexity": 1,
          "maxNestingDepth": 0
        },
        {
          "name": "LogOperation",
          "linesOfCode": 4,
          "parameterCount": 1,
          "cyclomaticComplexity": 1,
          "maxNestingDepth": 0
        },
        {
          "name": "FormatCurrency",
          "linesOfCode": 4,
          "parameterCount": 1,
          "cyclomaticComplexity": 1,
          "maxNestingDepth": 0
        },
        {
          "name": "LogOperation",
          "linesOfCode": 4,
          "parameterCount": 1,
          "cyclomaticComplexity": 1,
          "maxNestingDepth": 0
        }
      ]
    }, {
      "name": "Calculator",
      "linesOfCode": 129,
      "methods": [
        {
          "name": "Calculate",
          "linesOfCode": 13,
          "parameterCount": 2,
          "cyclomaticComplexity": 3,
          "maxNestingDepth": 1
        },
        {
          "name": "GetAverage",
          "linesOfCode": 4,
          "parameterCount": 0,
          "cyclomaticComplexity": 1,
          "maxNestingDepth": 0
        },
        {
          "name": "FormatResult",
          "linesOfCode": 4,
          "parameterCount": 1,
          "cyclomaticComplexity": 1,
          "maxNestingDepth": 0
        },
        {
          "name": "GetFormattedNumber",
          "linesOfCode": 4,
          "parameterCount": 1,
          "cyclomaticComplexity": 1,
          "maxNestingDepth": 0
        },
        {
          "name": "SetFormat",
          "linesOfCode": 4,
          "parameterCount": 1,
          "cyclomaticComplexity": 1,
          "maxNestingDepth": 0
        },
        {
          "name": "FormatCurrency",
          "linesOfCode": 4,
          "parameterCount": 1,
          "cyclomaticComplexity": 1,
          "maxNestingDepth": 0
        },
        {
          "name": "LogOperation",
          "linesOfCode": 4,
          "parameterCount": 1,
          "cyclomaticComplexity": 1,
          "maxNestingDepth": 0
        },
        {
          "name": "Multiply",
          "linesOfCode": 5,
          "parameterCount": 3,
          "cyclomaticComplexity": 1,
          "maxNestingDepth": 0
        },
        {
          "name": "UnusedHelper",
          "linesOfCode": 6,
          "parameterCount": 0,
          "cyclomaticComplexity": 1,
          "maxNestingDepth": 0
        }
      ]
    }, {
      "name": "cA",
      "linesOfCode": 8,
      "methods": [
        {
          "name": "Get",
          "linesOfCode": 1,
          "parameterCount": 0,
          "cyclomaticComplexity": 1,
          "maxNestingDepth": 0
        },
        {
          "name": "Add",
          "linesOfCode": 4,
          "parameterCount": 1,
          "cyclomaticComplexity": 1,
          "maxNestingDepth": 0
        }
      ]
    }, {
      "name": "B",
      "linesOfCode": 1,
      "methods": []
    }, ···]
    at IndFusion.Mcp.Tests.MetricsResourceTests.ReadMetrics_Directory_ReturnsAggregatedJson() in /_/src/test/IndFusion.Mcp.Tests/MetricsResourceTests.cs:46
    --- End of stack trace from previous location ---

Test run summary: Failed! - bin\Debug\net10.0\IndFusion.Mcp.Tests.dll (net10.0|x64)
  total: 252
  failed: 10
  succeeded: 242
  skipped: 0
  duration: 1m 50s 195ms