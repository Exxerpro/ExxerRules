using System;
using System.Collections.Immutable;
using IndFusion.Analyzers.NullSafety;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

class Program
{
    static void Main()
    {
        const string testCode = @"
using System;

namespace TestProject
{
    public class IncompleteValidationService
    {
        public void ProcessData(string input, object config, int? value)
        {
            // Only validate input
            ArgumentNullException.ThrowIfNull(input);
            
            // Use all parameters
            var length = input.Length;
            var type = config.GetType();
            var hasValue = value.HasValue;
        }
    }
}";

        var syntaxTree = CSharpSyntaxTree.ParseText(testCode);
        var compilation = CSharpCompilation.Create("TestAssembly", new[] { syntaxTree });
        var analyzer = new ValidateNullParametersAnalyzer();
        var compilationWithAnalyzers = compilation.WithAnalyzers(ImmutableArray.Create<DiagnosticAnalyzer>(analyzer));
        var diagnostics = compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync().GetAwaiter().GetResult();
        
        Console.WriteLine($"Found {diagnostics.Length} diagnostics:");
        foreach (var diagnostic in diagnostics)
        {
            Console.WriteLine($"- {diagnostic.Id}: {diagnostic.GetMessage()}");
        }
    }
}