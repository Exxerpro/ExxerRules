using Calculator.Core.Models;

namespace Calculator.Tests.Models;

/// <summary>
/// Tests for OperationType enum using xUnit v3 features.
/// </summary>
public class OperationTypeTests
{
    [Theory]
    [InlineData(OperationType.Add, "Add")]
    [InlineData(OperationType.Subtract, "Subtract")]
    [InlineData(OperationType.Multiply, "Multiply")]
    [InlineData(OperationType.Divide, "Divide")]
    [InlineData(OperationType.Power, "Power")]
    [InlineData(OperationType.SquareRoot, "SquareRoot")]
    [InlineData(OperationType.Percentage, "Percentage")]
    public void ToString_ShouldReturnCorrectName(OperationType operation, string expectedName)
    {
        // Act
        var result = operation.ToString();

        // Assert
        result.ShouldBe(expectedName);
    }

    [Fact]
    public void Enum_ShouldHaveAllExpectedValues()
    {
        // Arrange
        var expectedValues = new[]
        {
            OperationType.Add,
            OperationType.Subtract,
            OperationType.Multiply,
            OperationType.Divide,
            OperationType.Power,
            OperationType.SquareRoot,
            OperationType.Percentage
        };

        // Act
        var actualValues = Enum.GetValues<OperationType>();

        // Assert
        actualValues.ShouldBe(expectedValues);
    }

    [Theory]
    [InlineData(0, OperationType.Add)]
    [InlineData(1, OperationType.Subtract)]
    [InlineData(2, OperationType.Multiply)]
    [InlineData(3, OperationType.Divide)]
    [InlineData(4, OperationType.Power)]
    [InlineData(5, OperationType.SquareRoot)]
    [InlineData(6, OperationType.Percentage)]
    public void Enum_ShouldHaveCorrectNumericValues(int expectedValue, OperationType operation)
    {
        // Act
        var actualValue = (int)operation;

        // Assert
        actualValue.ShouldBe(expectedValue);
    }

    [Fact]
    public void Enum_ShouldBeParseableFromString()
    {
        // Arrange
        const string operationName = "Multiply";

        // Act
        var success = Enum.TryParse<OperationType>(operationName, out var result);

        // Assert
        success.ShouldBeTrue();
        result.ShouldBe(OperationType.Multiply);
    }

    [Fact]
    public void Enum_ShouldBeParseableFromStringIgnoreCase()
    {
        // Arrange
        const string operationName = "divide";

        // Act
        var success = Enum.TryParse<OperationType>(operationName, true, out var result);

        // Assert
        success.ShouldBeTrue();
        result.ShouldBe(OperationType.Divide);
    }

    [Fact]
    public void Enum_ShouldHandleInvalidString()
    {
        // Arrange
        const string invalidName = "InvalidOperation";

        // Act
        var success = Enum.TryParse<OperationType>(invalidName, out var result);

        // Assert
        success.ShouldBeFalse();
        result.ShouldBe(default(OperationType));
    }

    [Theory]
    [InlineData(OperationType.Add, OperationType.Add, true)]
    [InlineData(OperationType.Multiply, OperationType.Divide, false)]
    [InlineData(OperationType.SquareRoot, OperationType.SquareRoot, true)]
    public void Enum_Equality_ShouldWorkCorrectly(OperationType left, OperationType right, bool expectedEqual)
    {
        // Act
        var areEqual = left == right;

        // Assert
        areEqual.ShouldBe(expectedEqual);
    }
}
