using System;

namespace IndTrace.TestData.Attributes;

/// <summary>
/// Marks classes or methods as release/staging data for infrastructure testing.
/// Used to distinguish between production test data and developmental examples.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public sealed class ReleaseAttribute : Attribute
{
    public ReleaseAttribute() { }

    public ReleaseAttribute(string description)
    {
        Description = description;
    }

    public string? Description { get; }
}
