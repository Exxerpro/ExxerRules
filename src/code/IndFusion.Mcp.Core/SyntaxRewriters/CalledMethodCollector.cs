using Microsoft.CodeAnalysis.CSharp.Syntax;
using IndFusion.Mcp.Mcp.Core.SyntaxWalkers;

namespace IndFusion.Mcp.Mcp.Core.SyntaxRewriters;

/// <summary>
/// Collects method names that are invoked within a visited syntax tree, filtered by a target set.
/// </summary>
public class CalledMethodCollector : TrackedNameWalker
{
    /// <summary>
    /// Gets the set of method names discovered during traversal.
    /// </summary>
    public HashSet<string> CalledMethods => Matches;

    /// <summary>
    /// Initializes a new instance of the <see cref="CalledMethodCollector"/> class.
    /// </summary>
    /// <param name="methodNames">The set of method names to track.</param>
    public CalledMethodCollector(HashSet<string> methodNames)
        : base(methodNames)
    {
    }

    /// <summary>
    /// Attempts to record an invocation when it targets a tracked method name.
    /// </summary>
    /// <param name="node">The invocation expression node.</param>
    /// <returns><c>true</c> when the invocation was recorded; otherwise <c>false</c>.</returns>
    protected override bool TryRecordInvocation(InvocationExpressionSyntax node)
    {
        var name = GetInvocationName(node);
        if (name != null && IsTarget(name))
        {
            RecordMatch(name);
            return true;
        }
        return false;
    }
}
