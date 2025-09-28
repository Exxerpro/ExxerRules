using Microsoft.CodeAnalysis.CSharp.Syntax;
using IndFusion.Mcp.Core.SyntaxWalkers;

namespace IndFusion.Mcp.Core.SyntaxRewriters;

/// <summary>
/// Walker that checks whether any of the specified method names are invoked within the visited subtree.
/// </summary>
public class MethodCallChecker : TrackedNameWalker
{
    /// <summary>
    /// Gets a value indicating whether at least one tracked method call was found.
    /// </summary>
    public bool HasMethodCalls => Matches.Count > 0;

    /// <summary>
    /// Initializes a new instance of the <see cref="MethodCallChecker"/> class.
    /// </summary>
    /// <param name="classMethodNames">Set of method names belonging to the class under analysis.</param>
    public MethodCallChecker(HashSet<string> classMethodNames)
        : base(classMethodNames)
    {
    }

    /// <summary>
    /// Attempts to record an invocation when it targets a tracked method name.
    /// </summary>
    protected override bool TryRecordInvocation(InvocationExpressionSyntax node)
    {
        if (node.Expression is IdentifierNameSyntax identifier && IsTarget(identifier.Identifier.ValueText))
        {
            RecordMatch(identifier.Identifier.ValueText);
            return true;
        }
        return false;
    }
}
