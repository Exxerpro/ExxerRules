using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IndFusion.Mcp.Mcp.Core.SyntaxWalkers;

/// <summary>
/// Analyzes a method to detect instance member usage, calls to other methods, and recursion.
/// </summary>
public class MethodAnalysisWalker : CSharpSyntaxWalker
{
    private readonly HashSet<string> _instanceMembers;
    private readonly HashSet<string> _methodNames;
    private readonly string _methodName;

    /// <summary>
    /// Gets a value indicating whether the analyzed method uses instance members.
    /// </summary>
    public bool UsesInstanceMembers { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the analyzed method invokes other methods.
    /// </summary>
    public bool CallsOtherMethods { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the analyzed method is recursive.
    /// </summary>
    public bool IsRecursive { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MethodAnalysisWalker"/> class.
    /// </summary>
    /// <param name="instanceMembers">Set of instance member names for detection.</param>
    /// <param name="methodNames">Set of method names available within the type.</param>
    /// <param name="methodName">The name of the method being analyzed.</param>
    public MethodAnalysisWalker(HashSet<string> instanceMembers, HashSet<string> methodNames, string methodName)
    {
        _instanceMembers = instanceMembers;
        _methodNames = methodNames;
        _methodName = methodName;
    }

    /// <inheritdoc />
    public override void VisitIdentifierName(IdentifierNameSyntax node)
    {
        if (_instanceMembers.Contains(node.Identifier.ValueText))
        {
            var parent = node.Parent;
            if (parent is not MemberAccessExpressionSyntax ||
                (parent is MemberAccessExpressionSyntax ma && ma.Expression == node))
            {
                UsesInstanceMembers = true;
            }
        }

        if (_methodNames.Contains(node.Identifier.ValueText))
        {
            var parent = node.Parent;
            if (parent is not InvocationExpressionSyntax &&
                (parent is not MemberAccessExpressionSyntax ||
                 (parent is MemberAccessExpressionSyntax ma && ma.Expression is ThisExpressionSyntax)))
            {
                if (node.Identifier.ValueText == _methodName)
                    IsRecursive = true;
                else
                    CallsOtherMethods = true;
            }
        }

        base.VisitIdentifierName(node);
    }

    /// <inheritdoc />
    public override void VisitInvocationExpression(InvocationExpressionSyntax node)
    {
        if (node.Expression is IdentifierNameSyntax id && _methodNames.Contains(id.Identifier.ValueText))
        {
            if (id.Identifier.ValueText == _methodName)
            {
                IsRecursive = true;
            }
            else
            {
                CallsOtherMethods = true;
            }
        }
        base.VisitInvocationExpression(node);
    }
}
