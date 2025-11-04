namespace IndFusion.Tools.Cli.Tools;

/// <summary>
/// Generates a new interface from selected class members and updates the class to implement it.
/// </summary>
[McpServerToolType]
public static class ExtractInterfaceTool
{
    /// <summary>
    /// Extracts an interface file from the specified class and updates the class to implement it.
    /// </summary>
    /// <param name="solutionPath">Absolute path to the solution file (.sln).</param>
    /// <param name="filePath">Path to the C# file containing the class.</param>
    /// <param name="className">Name of the class to extract from.</param>
    /// <param name="memberList">Comma separated list of member names to include.</param>
    /// <param name="interfaceFilePath">Path to write the generated interface file.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A status message describing the generated interface and updates.</returns>
    [McpServerTool, Description("Extract a simple interface from a class")]
    public static async Task<string> ExtractInterface(
        [Description("Absolute path to the solution file (.sln)")] string solutionPath,
        [Description("Path to the C# file containing the class")] string filePath,
        [Description("Name of the class to extract from")] string className,
        [Description("Comma separated list of member names to include")] string memberList,
        [Description("Path to write the generated interface file")] string interfaceFilePath,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var solution = await ExxerFactoringHelpers.GetOrLoadSolution(solutionPath, cancellationToken);
            var document = ExxerFactoringHelpers.GetDocumentByPath(solution, filePath);
            if (document == null)
                throw new McpException($"Error: File {filePath} not found in solution");

            var root = (CompilationUnitSyntax)(await document.GetSyntaxRootAsync(cancellationToken))!;
            var classNode = root.DescendantNodes().OfType<ClassDeclarationSyntax>()
                .FirstOrDefault(c => c.Identifier.ValueText == className);
            if (classNode == null)
                throw new McpException($"Error: Class {className} not found");

            var chosen = memberList.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(m => m.Trim()).ToHashSet(StringComparer.Ordinal);

            var members = new List<MemberDeclarationSyntax>();
            foreach (var member in classNode.Members)
            {
                var name = member switch
                {
                    MethodDeclarationSyntax m => m.Identifier.ValueText,
                    PropertyDeclarationSyntax p => p.Identifier.ValueText,
                    _ => null
                };
                if (name != null && chosen.Contains(name))
                {
                    switch (member)
                    {
                        case MethodDeclarationSyntax m:
                            members.Add(m.WithBody(null)
                                .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken))
                                .WithModifiers([]));
                            break;

                        case PropertyDeclarationSyntax p:
                            var accessors = p.AccessorList ?? SyntaxFactory.AccessorList();
                            accessors = SyntaxFactory.AccessorList(SyntaxFactory.List(
                                accessors.Accessors.Select(a => a.WithBody(null)
                                    .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)))));
                            members.Add(p.WithAccessorList(accessors).WithModifiers([]));
                            break;
                    }
                }
            }

            if (members.Count == 0)
                throw new McpException("Error: No matching members found");

            var interfaceName = Path.GetFileNameWithoutExtension(interfaceFilePath);
            var iface = SyntaxFactory.InterfaceDeclaration(interfaceName)
                .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)))
                .WithMembers(SyntaxFactory.List(members));

            MemberDeclarationSyntax interfaceNode = iface;
            string? nsName = (classNode.Parent as BaseNamespaceDeclarationSyntax)?.Name.ToString();
            if (!string.IsNullOrEmpty(nsName))
            {
                interfaceNode = SyntaxFactory.FileScopedNamespaceDeclaration(
                        SyntaxFactory.ParseName(nsName))
                    .AddMembers(interfaceNode);
            }

            var ifaceUnit = SyntaxFactory.CompilationUnit()
                .WithUsings(root.Usings)
                .WithMembers(SyntaxFactory.SingletonList(interfaceNode))
                .NormalizeWhitespace();

            var encoding = await ExxerFactoringHelpers.GetFileEncodingAsync(filePath, cancellationToken);
            await File.WriteAllTextAsync(interfaceFilePath, ifaceUnit.ToFullString(), encoding, cancellationToken);

            var baseList = SyntaxFactory.BaseList(
                    SyntaxFactory.SingletonSeparatedList<BaseTypeSyntax>(
                        SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName(interfaceName))))
                .WithColonToken(SyntaxFactory.Token(SyntaxKind.ColonToken).WithTrailingTrivia(SyntaxFactory.Space));
            var updatedClass = classNode.WithBaseList(baseList);
            var newRoot = root.ReplaceNode(classNode, updatedClass);
            var formatted = newRoot.NormalizeWhitespace().ToFullString();
            await File.WriteAllTextAsync(filePath, formatted, encoding, cancellationToken);
            ExxerFactoringHelpers.UpdateFileCaches(filePath, formatted);
            ExxerFactoringHelpers.AddDocumentToProject(document.Project, interfaceFilePath);
            return $"Successfully extracted interface '{interfaceName}' to {interfaceFilePath}";
        }
        catch (Exception ex)
        {
            throw new McpException($"Error extracting interface: {ex.Message}", ex);
        }
    }
}
