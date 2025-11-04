namespace IndFusion.Tools.Cli.SyntaxWalkers;

internal class TypeCollectorWalker<T> : CSharpSyntaxWalker where T : TypeDeclarationSyntax
{
    public Dictionary<string, T> Types { get; } = [];

    public override void Visit(SyntaxNode? node)
    {
        if (node is T typed)
        {
            var name = typed.Identifier.ValueText;
            if (!Types.ContainsKey(name))
                Types[name] = typed;
        }
        base.Visit(node);
    }
}
