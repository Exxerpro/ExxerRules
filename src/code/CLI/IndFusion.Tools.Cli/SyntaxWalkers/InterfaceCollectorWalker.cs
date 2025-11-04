namespace IndFusion.Tools.Cli.SyntaxWalkers;

internal class InterfaceCollectorWalker : TypeCollectorWalker<InterfaceDeclarationSyntax>
{
    public Dictionary<string, InterfaceDeclarationSyntax> Interfaces => Types;
}
