namespace Application.UnitTests.Features.Barcodes;

/// <summary>
/// Unit tests for BarCodesListVm
/// </summary>
public class BarCodesListVmTests
{
    /// <summary>
    /// Executes Constructor_WithValidParameters_ShouldCreateInstance operation.
    /// </summary>
    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateInstance()
    {
        // Arrange & Act
        var instance = new IndTrace.Application.BarCodes.Queries.GetBarCodeList.BarCodesListVm();
        // Assert
        instance.ShouldNotBeNull();
    }
}
