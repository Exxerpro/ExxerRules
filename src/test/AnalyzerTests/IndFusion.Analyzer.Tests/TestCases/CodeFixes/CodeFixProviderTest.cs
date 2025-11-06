using Microsoft.CodeAnalysis.CodeFixes;

namespace IndFusion.Analyzer.Tests.TestCases.CodeFixes;

/// <summary>
/// Base class for code fix provider tests.
/// </summary>
/// <typeparam name="T">The code fix provider type.</typeparam>
public abstract class CodeFixProviderTest<T> where T : CodeFixProvider, new()
{
    /// <summary>
    /// Gets the code fix provider under test.
    /// </summary>
    protected T CodeFixProvider { get; } = new T();
}