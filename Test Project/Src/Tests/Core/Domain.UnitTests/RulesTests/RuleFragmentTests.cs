namespace IndTrace.Domain.UnitTests.RulesTests;

/// <summary>
/// Unit tests for the <see cref="RuleFragment"/> entity.
/// </summary>
public class RuleFragmentTests
{
    /// <summary>
    /// Executes RuleFragment_RuleFragment_Properties_ShouldSetAndGetCorrectly operation.
    /// </summary>
    [Fact]
    public void RuleFragment_RuleFragment_Properties_ShouldSetAndGetCorrectly()
    {
        // Arrange
        var fragment = new RuleFragment();

        // Act
        fragment.Name = "TestFragment";
        fragment.Action = "Validate";
        fragment.Origin = "PLC";
        fragment.Value = "12345";
        fragment.LengthMin = 5;
        fragment.LengthMax = 10;
        fragment.Length = 5;
        fragment.Incremental = true;

        // Assert
        fragment.Name.ShouldBe("TestFragment");
        fragment.Action.ShouldBe("Validate");
        fragment.Origin.ShouldBe("PLC");
        fragment.Value.ShouldBe("12345");
        fragment.LengthMin.ShouldBe(5);
        fragment.LengthMax.ShouldBe(10);
        fragment.Length.ShouldBe(5);
        fragment.Incremental.ShouldBeTrue();
    }
}
