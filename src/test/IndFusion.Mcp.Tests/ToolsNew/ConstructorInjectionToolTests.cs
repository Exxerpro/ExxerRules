using IndFusion.Mcp.Tests.Tools;
using IndFusion.Mcp.Core.Tools;

namespace IndFusion.Mcp.Tests.ToolsNew;

/// <summary>
/// Tests for converting parameter usage into constructor-injected fields/properties.
/// </summary>
public class ConstructorInjectionToolTests : TestBase
    {
        /// <summary>
        /// Converts a method parameter to a constructor-injected field and updates call sites.
        /// </summary>
        [Fact]
        public async Task ConstructorInjection_AddsField()
        {
            const string initialCode = "class C{ int M(int x){ return x+1; } void Call(){ M(1); } }";

            var testFile = Path.Combine(TestOutputPath, "ConstructorInjection.cs");
            await TestUtilities.CreateTestFile(testFile, initialCode);

            // Use null solution path to force single-file mode (like CleanupUsingsTool)
            var result = await ConstructorInjectionTool.ConvertToConstructorInjection(
                null!,
                testFile,
                new[] { new ConstructorInjectionTool.MethodParameterPair("M", "x") },
                false,
                cancellationToken: Xunit.TestContext.Current.CancellationToken);

        Assert.Contains("Successfully injected", result);
        var fileContent = await File.ReadAllTextAsync(testFile, cancellationToken: Xunit.TestContext.Current.CancellationToken);
        Assert.Contains("_x", fileContent);
    }
}
