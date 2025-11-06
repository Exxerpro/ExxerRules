using IndFusion.SemanticRag.Domain.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;

namespace IndFusion.SemanticRag.Infrastructure.Services;

/// <summary>
/// Analyzer that walks the syntax tree and detects pattern violations.
/// </summary>
internal class PatternViolationAnalyzer : CSharpSyntaxWalker
{
    private readonly SemanticModel _semanticModel;
    private readonly string _context;
    private readonly ILogger _logger;
    private int _violationCounter = 0;

    public List<PatternViolation> Violations { get; } = new();

    public PatternViolationAnalyzer(SemanticModel semanticModel, string context, ILogger logger)
    {
        _semanticModel = semanticModel;
        _context = context;
        _logger = logger;
    }

    public override void VisitLocalDeclarationStatement(LocalDeclarationStatementSyntax node)
    {
        base.VisitLocalDeclarationStatement(node);
        
        // Check for unused variables
        foreach (var variable in node.Declaration.Variables)
        {
            var symbol = _semanticModel.GetDeclaredSymbol(variable);
            if (symbol != null)
            {
                // Get the root to search for all references
                var root = node.SyntaxTree.GetRoot();
                
                // Find all references to this symbol in the code
                var references = root.DescendantNodes()
                    .OfType<IdentifierNameSyntax>()
                    .Where(id =>
                    {
                        var referenceSymbol = _semanticModel.GetSymbolInfo(id).Symbol;
                        return SymbolEqualityComparer.Default.Equals(referenceSymbol, symbol);
                    })
                    .Where(id => id.Identifier.ValueText != variable.Identifier.ValueText || id.Parent != variable)
                    .ToList();
                
                // If no references found, the variable is unused
                if (references.Count == 0)
                {
                    var location = node.GetLocation();
                    var lineSpan = location.GetLineSpan();
                    
                    Violations.Add(new PatternViolation(
                        Id: $"UNUSED_VAR_{++_violationCounter}",
                        PatternId: "UNUSED_VARIABLE",
                        PatternName: "Unused Variable",
                        Severity: PatternSeverity.Warning,
                        Message: $"Variable '{variable.Identifier.ValueText}' is declared but never used",
                        FilePath: null,
                        LineNumber: lineSpan.StartLinePosition.Line + 1,
                        ColumnNumber: lineSpan.StartLinePosition.Character + 1,
                        CodeSnippet: variable.ToString(),
                        Context: new Dictionary<string, object> { ["Context"] = _context } as IReadOnlyDictionary<string, object>,
                        CreatedAt: DateTimeOffset.UtcNow));
                }
            }
        }
    }

    public override void VisitThrowStatement(ThrowStatementSyntax node)
    {
        base.VisitThrowStatement(node);
        
        // Check for throw statements (anti-pattern in functional programming)
        var location = node.GetLocation();
        var lineSpan = location.GetLineSpan();
        
        Violations.Add(new PatternViolation(
            Id: $"THROW_STMT_{++_violationCounter}",
            PatternId: "THROW_STATEMENT",
            PatternName: "Throw Statement",
            Severity: PatternSeverity.Error,
            Message: "Throw statements should be avoided. Prefer returning Result<T> pattern.",
            FilePath: null,
            LineNumber: lineSpan.StartLinePosition.Line + 1,
            ColumnNumber: lineSpan.StartLinePosition.Character + 1,
            CodeSnippet: node.ToString(),
            Context: new Dictionary<string, object> { ["Context"] = _context } as IReadOnlyDictionary<string, object>,
            CreatedAt: DateTimeOffset.UtcNow));
    }
}