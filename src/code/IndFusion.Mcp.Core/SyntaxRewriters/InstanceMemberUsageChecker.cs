using IndFusion.Mcp.Mcp.Core.SyntaxWalkers;

namespace IndFusion.Mcp.Mcp.Core.SyntaxRewriters;

/// <summary>
/// Walker that detects whether any of the provided instance member names are used within a visited subtree.
/// </summary>
public class InstanceMemberUsageChecker : TrackedNameWalker
{
    /// <summary>
    /// Gets a value indicating whether at least one tracked instance member was encountered.
    /// </summary>
    public bool HasInstanceMemberUsage => Matches.Count > 0;

    /// <summary>
    /// Initializes a new instance of the <see cref="InstanceMemberUsageChecker"/> class.
    /// </summary>
    /// <param name="knownInstanceMembers">Set of instance member names to track.</param>
    public InstanceMemberUsageChecker(HashSet<string> knownInstanceMembers)
        : base(knownInstanceMembers)
    {
    }
}
