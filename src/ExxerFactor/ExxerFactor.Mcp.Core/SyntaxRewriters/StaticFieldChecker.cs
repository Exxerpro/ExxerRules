using Microsoft.CodeAnalysis.CSharp.Syntax;
using ExxerFactor.Mcp.Core.SyntaxWalkers;

namespace ExxerFactor.Mcp.Core.SyntaxRewriters;

/// <summary>
/// Walker that detects references to static fields among a provided set of names.
/// </summary>
public class StaticFieldChecker : TrackedNameWalker
{
    /// <summary>
    /// Gets a value indicating whether any static field references were found.
    /// </summary>
    public bool HasStaticFieldReferences => Matches.Count > 0;

    /// <summary>
    /// Initializes a new instance of the <see cref="StaticFieldChecker"/> class.
    /// </summary>
    /// <param name="staticFieldNames">Set of static field names to track.</param>
    public StaticFieldChecker(HashSet<string> staticFieldNames)
        : base(staticFieldNames)
    {
    }

    /// <inheritdoc />
    protected override bool ShouldRecordIdentifier(IdentifierNameSyntax node)
    {
        var parent = node.Parent;
        if (!IsTarget(node.Identifier.ValueText) || IsParameterOrType(parent))
            return false;

        if (parent is MemberAccessExpressionSyntax memberAccess)
        {
            return memberAccess.Name == node;
        }

        return true;
    }
}