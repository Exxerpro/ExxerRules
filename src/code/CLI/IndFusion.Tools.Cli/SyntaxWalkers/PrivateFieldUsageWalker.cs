namespace IndFusion.Tools.Cli.SyntaxWalkers;

internal class PrivateFieldUsageWalker : TrackedNameWalker
{
    public HashSet<string> UsedFields => Matches;

    public PrivateFieldUsageWalker(HashSet<string> privateFieldNames)
        : base(privateFieldNames)
    {
    }
}
