namespace IndFusion.Tools.Cli.SyntaxWalkers;

internal class ClassCollectorWalker : TypeCollectorWalker<ClassDeclarationSyntax>
{
    public Dictionary<string, ClassDeclarationSyntax> Classes => Types;
}
