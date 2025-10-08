using IndTrace.DataStore.Models;

namespace IndTrace.DataStore.Interfaces;

/// <summary>
/// Defines a runner for executing test paths on products.
/// </summary>
public interface ITestPathRunner
{
    /// <summary>
    /// Executes a test path asynchronously for the specified product and parameters.
    /// </summary>
    /// <param name="product">The product to test.</param>
    /// <param name="pathType">The type of test path.</param>
    /// <param name="sequenceIndex">The sequence index for the test path.</param>
    /// <param name="flavor">The execution flavor.</param>
    /// <param name="maxNumberMachines">The maximum number of machines to use.</param>
    /// <param name="options">The dry run options.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task ExecutePathAsync(
        Product product,
        TestPathType pathType,
        int sequenceIndex,
        ExecutionFlavor flavor,
        int maxNumberMachines, DryRunOptions options, CancellationToken cancellationToken);
}

//TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate ITestPathRunner logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
