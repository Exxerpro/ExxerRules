namespace IndFusion.Analyzer.Common;

/// <summary>
/// Information about test attributes found on a method.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="TestAttributeInfo"/> class.
/// </remarks>
/// <param name="attributeNames">The attribute names discovered on the analyzed method.</param>
/// <param name="hasTestAttributes">
/// A value indicating whether at least one supported test attribute was identified.
/// </param>
/// <param name="framework">The test framework inferred from the supplied attributes.</param>
public class TestAttributeInfo(IReadOnlyList<string> attributeNames, bool hasTestAttributes, TestFramework framework)
{
    /// <summary>
    /// Gets the discovered test attribute names in discovery order, or an empty list when none were found.
    /// </summary>
    public IReadOnlyList<string> AttributeNames { get; } = attributeNames ?? [];

    /// <summary>
    /// Gets a value indicating whether any supported test attributes were discovered.
    /// </summary>
    public bool HasTestAttributes { get; } = hasTestAttributes;

    /// <summary>
    /// Gets the detected test framework corresponding to the attribute set.
    /// </summary>
    public TestFramework Framework { get; } = framework;

    /// <summary>
    /// Gets a value indicating whether the attribute name collection remains valid for analysis.
    /// </summary>
    public bool IsValid => AttributeNames != null;
}