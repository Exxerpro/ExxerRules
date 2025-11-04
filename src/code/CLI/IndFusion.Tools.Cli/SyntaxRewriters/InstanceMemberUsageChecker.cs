using IndFusion.Tools.Cli.SyntaxWalkers;

namespace IndFusion.Tools.Cli.SyntaxRewriters;

internal class InstanceMemberUsageChecker : TrackedNameWalker
{
    public bool HasInstanceMemberUsage => Matches.Count > 0;

    public InstanceMemberUsageChecker(HashSet<string> knownInstanceMembers)
        : base(knownInstanceMembers)
    {
    }
}
