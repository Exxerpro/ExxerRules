using Microsoft.CodeAnalysis.CSharp;

namespace IndFusion.Mcp.Core.SyntaxWalkers;

/// <summary>
/// Base syntax walker that collects discovered names into a set.
/// </summary>
public abstract class NameCollectorWalker : CSharpSyntaxWalker
{
    /// <summary>
    /// Gets the set of collected names.
    /// </summary>
    public HashSet<string> Names { get; } = [];

    /// <summary>
    /// Adds the specified name to the collection.
    /// </summary>
    /// <param name="name">The name to add.</param>
    protected void Add(string name) => Names.Add(name);
}
