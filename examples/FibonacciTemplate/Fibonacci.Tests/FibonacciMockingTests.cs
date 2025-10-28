using Fibonacci.Core;

namespace Fibonacci.Tests;

/// <summary>
/// Tests demonstrating mocking with NSubstitute for the Fibonacci calculator.
/// </summary>
public class FibonacciMockingTests
{
    [Fact]
    public void MockedCalculator_ShouldReturnConfiguredSequence()
    {
        // Arrange
        var mockCalculator = Substitute.For<IFibonacciCalculator>();
        var expectedSequence = new long[] { 0, 1, 1, 2, 3, 5, 8, 13, 21, 34 };
        
        mockCalculator.CalculateSequence(10).Returns(expectedSequence);

        // Act
        var result = mockCalculator.CalculateSequence(10).ToArray();

        // Assert
        result.ShouldBe(expectedSequence);
        mockCalculator.Received(1).CalculateSequence(10);
    }

    [Fact]
    public void MockedCalculator_ShouldThrowException_WhenConfigured()
    {
        // Arrange
        var mockCalculator = Substitute.For<IFibonacciCalculator>();
        var exception = new ArgumentOutOfRangeException("terms", "Invalid terms");
        
        mockCalculator.When(x => x.CalculateSequence(Arg.Any<int>()))
            .Do(x => throw exception);

        // Act & Assert
        Should.Throw<ArgumentOutOfRangeException>(() => mockCalculator.CalculateSequence(5))
            .ShouldBe(exception);
    }

    [Fact]
    public void MockedCalculator_ShouldReturnDifferentValues_ForDifferentInputs()
    {
        // Arrange
        var mockCalculator = Substitute.For<IFibonacciCalculator>();
        var sequence1 = new long[] { 0, 1, 1 };
        var sequence2 = new long[] { 0, 1, 1, 2, 3, 5, 8, 13, 21, 34 };
        
        mockCalculator.CalculateSequence(3).Returns(sequence1);
        mockCalculator.CalculateSequence(10).Returns(sequence2);

        // Act
        var result1 = mockCalculator.CalculateSequence(3).ToArray();
        var result2 = mockCalculator.CalculateSequence(10).ToArray();

        // Assert
        result1.ShouldBe(sequence1);
        result2.ShouldBe(sequence2);
        mockCalculator.Received(1).CalculateSequence(3);
        mockCalculator.Received(1).CalculateSequence(10);
    }

    [Fact]
    public void MockedCalculator_ShouldValidateInput_WhenConfigured()
    {
        // Arrange
        var mockCalculator = Substitute.For<IFibonacciCalculator>();
        mockCalculator.IsValidTermCount(Arg.Any<int>()).Returns(true);
        mockCalculator.IsValidTermCount(0).Returns(false);
        mockCalculator.IsValidTermCount(-1).Returns(false);

        // Act
        var validResult1 = mockCalculator.IsValidTermCount(10);
        var validResult2 = mockCalculator.IsValidTermCount(5);
        var invalidResult1 = mockCalculator.IsValidTermCount(0);
        var invalidResult2 = mockCalculator.IsValidTermCount(-1);

        // Assert
        validResult1.ShouldBeTrue();
        validResult2.ShouldBeTrue();
        invalidResult1.ShouldBeFalse();
        invalidResult2.ShouldBeFalse();
    }

    [Fact]
    public void MockedCalculator_ShouldReturnConfiguredNthValue()
    {
        // Arrange
        var mockCalculator = Substitute.For<IFibonacciCalculator>();
        mockCalculator.CalculateNth(10).Returns(55L);
        mockCalculator.CalculateNthIterative(10).Returns(55L);

        // Act
        var recursiveResult = mockCalculator.CalculateNth(10);
        var iterativeResult = mockCalculator.CalculateNthIterative(10);

        // Assert
        recursiveResult.ShouldBe(55L);
        iterativeResult.ShouldBe(55L);
        mockCalculator.Received(1).CalculateNth(10);
        mockCalculator.Received(1).CalculateNthIterative(10);
    }

    [Fact]
    public void MockedCalculator_ShouldReturnMaxSafeTermCount_WhenConfigured()
    {
        // Arrange
        var mockCalculator = Substitute.For<IFibonacciCalculator>();
        mockCalculator.GetMaxSafeTermCount().Returns(100);

        // Act
        var result = mockCalculator.GetMaxSafeTermCount();

        // Assert
        result.ShouldBe(100);
        mockCalculator.Received(1).GetMaxSafeTermCount();
    }

    [Fact]
    public void MockedCalculator_ShouldSupportComplexScenarios()
    {
        // Arrange
        var mockCalculator = Substitute.For<IFibonacciCalculator>();
        var validSequence = new long[] { 0, 1, 1, 2, 3, 5, 8, 13, 21, 34 };
        var exception = new ArgumentOutOfRangeException("terms", "Too many terms");
        
        mockCalculator.IsValidTermCount(Arg.Is<int>(x => x > 0 && x <= 10)).Returns(true);
        mockCalculator.IsValidTermCount(Arg.Is<int>(x => x <= 0 || x > 10)).Returns(false);
        mockCalculator.CalculateSequence(Arg.Is<int>(x => x > 0 && x <= 10)).Returns(validSequence);
        mockCalculator.When(x => x.CalculateSequence(Arg.Is<int>(x => x <= 0 || x > 10)))
            .Do(x => throw exception);

        // Act & Assert - Valid cases
        var validResult = mockCalculator.CalculateSequence(10).ToArray();
        validResult.ShouldBe(validSequence);
        mockCalculator.IsValidTermCount(10).ShouldBeTrue();

        // Act & Assert - Invalid cases
        Should.Throw<ArgumentOutOfRangeException>(() => mockCalculator.CalculateSequence(15));
        mockCalculator.IsValidTermCount(15).ShouldBeFalse();
    }
}
