using FluentAssertions;

public class TestClass
{
    public void TestMethod()
    {
        var result = GetResult();
        result.Should().Be(42);
    }
    
    private int GetResult() => 42;
}