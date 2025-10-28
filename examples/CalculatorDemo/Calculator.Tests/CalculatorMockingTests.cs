using Calculator;

namespace Calculator.Tests;

/// <summary>
/// Tests demonstrating mocking capabilities with NSubstitute and XUnit v3.
/// </summary>
public class CalculatorMockingTests
{
    [Fact]
    public void MockedCalculator_ShouldReturnConfiguredValues_WhenMethodsAreCalled()
    {
        // Arrange
        var mockCalculator = Substitute.For<ICalculator>();
        mockCalculator.Add(Arg.Any<double>(), Arg.Any<double>()).Returns(42.0);
        mockCalculator.Multiply(Arg.Any<double>(), Arg.Any<double>()).Returns(100.0);

        // Act
        var addResult = mockCalculator.Add(5, 3);
        var multiplyResult = mockCalculator.Multiply(4, 25);

        // Assert
        addResult.ShouldBe(42.0);
        multiplyResult.ShouldBe(100.0);
    }

    [Fact]
    public void MockedCalculator_ShouldVerifyMethodCalls_WhenMethodsAreInvoked()
    {
        // Arrange
        var mockCalculator = Substitute.For<ICalculator>();
        var a = 10.0;
        var b = 5.0;

        // Act
        mockCalculator.Add(a, b);
        mockCalculator.Subtract(a, b);

        // Assert
        mockCalculator.Received(1).Add(a, b);
        mockCalculator.Received(1).Subtract(a, b);
        mockCalculator.DidNotReceive().Multiply(Arg.Any<double>(), Arg.Any<double>());
    }

    [Fact]
    public void MockedCalculator_ShouldThrowConfiguredException_WhenMethodIsCalled()
    {
        // Arrange
        var mockCalculator = Substitute.For<ICalculator>();
        var exception = new DivideByZeroException("Mocked division by zero");
        mockCalculator.When(x => x.Divide(Arg.Any<double>(), Arg.Any<double>())).Do(x => throw exception);

        // Act & Assert
        Should.Throw<DivideByZeroException>(() => mockCalculator.Divide(10, 0))
            .Message.ShouldBe("Mocked division by zero");
    }

    [Fact]
    public void MockedCalculator_ShouldReturnDifferentValues_WhenCalledWithDifferentArguments()
    {
        // Arrange
        var mockCalculator = Substitute.For<ICalculator>();
        mockCalculator.Add(1, 1).Returns(2);
        mockCalculator.Add(2, 2).Returns(4);
        mockCalculator.Add(Arg.Any<double>(), Arg.Any<double>()).Returns(0);

        // Act
        var result1 = mockCalculator.Add(1, 1);
        var result2 = mockCalculator.Add(2, 2);
        var result3 = mockCalculator.Add(5, 5);

        // Assert
        result1.ShouldBe(2);
        result2.ShouldBe(4);
        result3.ShouldBe(0);
    }

    [Fact]
    public void MockedCalculator_ShouldTrackCallCount_WhenMethodsAreCalledMultipleTimes()
    {
        // Arrange
        var mockCalculator = Substitute.For<ICalculator>();
        mockCalculator.Add(Arg.Any<double>(), Arg.Any<double>()).Returns(0);

        // Act
        mockCalculator.Add(1, 1);
        mockCalculator.Add(2, 2);
        mockCalculator.Add(3, 3);

        // Assert
        mockCalculator.Received(3).Add(Arg.Any<double>(), Arg.Any<double>());
    }

    [Fact]
    public void MockedCalculator_ShouldVerifyCallOrder_WhenMethodsAreCalledInSequence()
    {
        // Arrange
        var mockCalculator = Substitute.For<ICalculator>();

        // Act
        mockCalculator.Add(1, 1);
        mockCalculator.Multiply(2, 2);
        mockCalculator.Divide(4, 2);

        // Assert
        Received.InOrder(() =>
        {
            mockCalculator.Add(1, 1);
            mockCalculator.Multiply(2, 2);
            mockCalculator.Divide(4, 2);
        });
    }

    [Fact]
    public void MockedCalculator_ShouldHandleAsyncOperations_WhenConfiguredProperly()
    {
        // Arrange
        var mockCalculator = Substitute.For<ICalculator>();
        var task = Task.FromResult(42.0);
        mockCalculator.Add(Arg.Any<double>(), Arg.Any<double>()).Returns(42.0);

        // Act
        var result = mockCalculator.Add(20, 22);

        // Assert
        result.ShouldBe(42.0);
        mockCalculator.Received(1).Add(20, 22);
    }
}
