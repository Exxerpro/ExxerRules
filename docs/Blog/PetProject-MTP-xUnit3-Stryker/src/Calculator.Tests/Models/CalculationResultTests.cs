using Calculator.Core.Models;

namespace Calculator.Tests.Models;

/// <summary>
/// Tests for CalculationResult record using xUnit v3 features.
/// </summary>
public class CalculationResultTests
{
    [Fact]
    public void Success_ShouldCreateSuccessfulResult()
    {
        // Arrange
        const decimal value = 42.5m;

        // Act
        var result = CalculationResult.Success(value);

        // Assert
        result.Value.ShouldBe(value);
        result.IsSuccess.ShouldBeTrue();
        result.IsFailure.ShouldBeFalse();
        result.ErrorMessage.ShouldBeNull();
    }

    [Fact]
    public void Failure_ShouldCreateFailedResult()
    {
        // Arrange
        const string errorMessage = "Test error message";

        // Act
        var result = CalculationResult.Failure(errorMessage);

        // Assert
        result.Value.ShouldBe(0);
        result.IsSuccess.ShouldBeFalse();
        result.IsFailure.ShouldBeTrue();
        result.ErrorMessage.ShouldBe(errorMessage);
    }

    [Fact]
    public void Constructor_WithAllParameters_ShouldSetProperties()
    {
        // Arrange
        const decimal value = 10.0m;
        const bool isSuccess = true;
        const string errorMessage = "Test error";

        // Act
        var result = new CalculationResult(value, isSuccess, errorMessage);

        // Assert
        result.Value.ShouldBe(value);
        result.IsSuccess.ShouldBe(isSuccess);
        result.ErrorMessage.ShouldBe(errorMessage);
    }

    [Fact]
    public void Constructor_WithMinimalParameters_ShouldSetDefaults()
    {
        // Arrange
        const decimal value = 5.0m;
        const bool isSuccess = false;

        // Act
        var result = new CalculationResult(value, isSuccess);

        // Assert
        result.Value.ShouldBe(value);
        result.IsSuccess.ShouldBe(isSuccess);
        result.ErrorMessage.ShouldBeNull();
    }

    [Theory]
    [InlineData(0.0, true, null)]
    [InlineData(-5.5, false, "Error")]
    [InlineData(999.999, true, "")]
    [InlineData(0.0001, false, "Division by zero")]
    public void Constructor_WithVariousParameters_ShouldSetCorrectValues(decimal value, bool isSuccess, string? errorMessage)
    {
        // Act
        var result = new CalculationResult(value, isSuccess, errorMessage);

        // Assert
        result.Value.ShouldBe(value);
        result.IsSuccess.ShouldBe(isSuccess);
        result.ErrorMessage.ShouldBe(errorMessage);
    }

    [Fact]
    public void IsFailure_WhenSuccess_ShouldReturnFalse()
    {
        // Arrange
        var result = CalculationResult.Success(10.0m);

        // Act & Assert
        result.IsFailure.ShouldBeFalse();
    }

    [Fact]
    public void IsFailure_WhenFailure_ShouldReturnTrue()
    {
        // Arrange
        var result = CalculationResult.Failure("Error");

        // Act & Assert
        result.IsFailure.ShouldBeTrue();
    }

    [Fact]
    public void Record_Equality_ShouldWorkCorrectly()
    {
        // Arrange
        var result1 = CalculationResult.Success(5.0m);
        var result2 = CalculationResult.Success(5.0m);
        var result3 = CalculationResult.Success(10.0m);

        // Act & Assert
        result1.ShouldBe(result2);
        result1.ShouldNotBe(result3);
    }

    [Fact]
    public void Record_ToString_ShouldIncludeAllProperties()
    {
        // Arrange
        var result = new CalculationResult(42.5m, true, "Test error");

        // Act
        var toString = result.ToString();

        // Assert
        toString.ShouldContain("42.5");
        toString.ShouldContain("True");
        toString.ShouldContain("Test error");
    }
}
