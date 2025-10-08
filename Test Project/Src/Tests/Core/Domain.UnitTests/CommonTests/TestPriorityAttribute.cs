namespace IndTrace.Domain.UnitTests.CommonTests;
/// <summary>
/// Represents the TestPriorityAttribute.
/// </summary>

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class TestPriorityAttribute(int priority) : Attribute
{
    /// <summary>
    /// Gets or sets the Priority.
    /// </summary>
    public int Priority { get; } = priority;
}
