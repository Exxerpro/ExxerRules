namespace IndFusion.Tools.Cli.SyntaxWalkers;

internal abstract class NameCollectorWalker : CSharpSyntaxWalker
{
    public HashSet<string> Names { get; } = new();

    protected void Add(string name) => Names.Add(name);
}
