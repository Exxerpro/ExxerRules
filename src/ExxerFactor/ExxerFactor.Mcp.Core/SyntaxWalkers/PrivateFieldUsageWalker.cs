namespace ExxerFactor.Mcp.Core.SyntaxWalkers;

/// <summary>
/// Tracks usage occurrences of private fields by name.
/// </summary>
public class PrivateFieldUsageWalker : TrackedNameWalker
{
    /// <summary>
    /// Gets the set of private field names that were used.
    /// </summary>
    public HashSet<string> UsedFields => Matches;

    /// <summary>
    /// Initializes a new instance of the <see cref="PrivateFieldUsageWalker"/> class.
    /// </summary>
    /// <param name="privateFieldNames">The set of private field names to track.</param>
    public PrivateFieldUsageWalker(HashSet<string> privateFieldNames)
        : base(privateFieldNames)
    {
    }
}