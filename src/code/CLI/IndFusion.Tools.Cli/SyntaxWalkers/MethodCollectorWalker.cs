namespace IndFusion.Tools.Cli.SyntaxWalkers;

internal class MethodCollectorWalker : CSharpSyntaxWalker
{
    private readonly HashSet<string> _targets;
    public Dictionary<string, MethodDeclarationSyntax> Methods { get; } = [];

    public MethodCollectorWalker(HashSet<string> targets)
    {
        _targets = targets;
    }

    public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
    {
        if (node.Parent is ClassDeclarationSyntax cls)
        {
            var key = $"{cls.Identifier.ValueText}.{node.Identifier.ValueText}";
            if (_targets.Contains(key) && !Methods.ContainsKey(key))
            {
                Methods[key] = node;
            }
        }
        base.VisitMethodDeclaration(node);
    }
}
