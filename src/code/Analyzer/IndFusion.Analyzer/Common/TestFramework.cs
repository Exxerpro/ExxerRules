namespace IndFusion.Analyzer.Common;

/// <summary>
/// Enumeration of supported test frameworks.
/// </summary>
public enum TestFramework
{
    /// <summary>
    /// Represents an unknown or unsupported test framework.
    /// </summary>
    Unknown,

    /// <summary>
    /// Represents the xUnit testing framework.
    /// </summary>
    XUnit,

    /// <summary>
    /// Represents the NUnit testing framework.
    /// </summary>
    NUnit,

    /// <summary>
    /// Represents the MSTest testing framework.
    /// </summary>
    MSTest
}