using System.Collections.Immutable;
using System.Linq;
using IndFusion.Analyzers.Common;
using IndFusion.Analyzers.Operations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace IndFusion.Analyzers.Documentation;

/// <summary>
/// Analyzer that enforces XML documentation on public members.
/// Supports the "documentation is the bridge from intent to understanding" principle.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class PublicMembersShouldHaveXmlDocumentationAnalyzer : DiagnosticAnalyzer
{
    private static readonly LocalizableString Title = "Public members should have XML documentation";
    private static readonly LocalizableString MessageFormat = "Public {0} '{1}' should have XML documentation";
    private static readonly LocalizableString Description = "Public members should have XML documentation to support IntelliSense, tooling integration, and developer understanding. Documentation is the bridge from intent to understanding.";

    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticIds.PublicMembersShouldHaveXmlDocumentation,
        Title,
        MessageFormat,
        DiagnosticCategories.Documentation,
        DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: Description);

    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    /// <inheritdoc/>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterSyntaxNodeAction(AnalyzeClass, SyntaxKind.ClassDeclaration);
        context.RegisterSyntaxNodeAction(AnalyzeInterface, SyntaxKind.InterfaceDeclaration);
        context.RegisterSyntaxNodeAction(AnalyzeStruct, SyntaxKind.StructDeclaration);
        context.RegisterSyntaxNodeAction(AnalyzeEnum, SyntaxKind.EnumDeclaration);
        context.RegisterSyntaxNodeAction(AnalyzeMethod, SyntaxKind.MethodDeclaration);
        context.RegisterSyntaxNodeAction(AnalyzeProperty, SyntaxKind.PropertyDeclaration);
        context.RegisterSyntaxNodeAction(AnalyzeField, SyntaxKind.FieldDeclaration);
        context.RegisterSyntaxNodeAction(AnalyzeEvent, SyntaxKind.EventDeclaration);
    }

    private static void AnalyzeClass(SyntaxNodeAnalysisContext context)
    {
        var classDeclaration = (ClassDeclarationSyntax)context.Node;
        AnalyzeMember(context, classDeclaration, classDeclaration.Modifiers, classDeclaration.Identifier, "class");
    }

    private static void AnalyzeInterface(SyntaxNodeAnalysisContext context)
    {
        var interfaceDeclaration = (InterfaceDeclarationSyntax)context.Node;
        AnalyzeMember(context, interfaceDeclaration, interfaceDeclaration.Modifiers, interfaceDeclaration.Identifier, "interface");
    }

    private static void AnalyzeStruct(SyntaxNodeAnalysisContext context)
    {
        var structDeclaration = (StructDeclarationSyntax)context.Node;
        AnalyzeMember(context, structDeclaration, structDeclaration.Modifiers, structDeclaration.Identifier, "struct");
    }

    private static void AnalyzeEnum(SyntaxNodeAnalysisContext context)
    {
        var enumDeclaration = (EnumDeclarationSyntax)context.Node;
        AnalyzeMember(context, enumDeclaration, enumDeclaration.Modifiers, enumDeclaration.Identifier, "enum");
    }

    private static void AnalyzeMethod(SyntaxNodeAnalysisContext context)
    {
        var methodDeclaration = (MethodDeclarationSyntax)context.Node;

        // Skip special methods
        if (IsSkippableMethod(methodDeclaration))
        {
            return;
        }

        AnalyzeMember(context, methodDeclaration, methodDeclaration.Modifiers, methodDeclaration.Identifier, "method");
    }

    private static void AnalyzeProperty(SyntaxNodeAnalysisContext context)
    {
        var propertyDeclaration = (PropertyDeclarationSyntax)context.Node;
        AnalyzeMember(context, propertyDeclaration, propertyDeclaration.Modifiers, propertyDeclaration.Identifier, "property");
    }

    private static void AnalyzeField(SyntaxNodeAnalysisContext context)
    {
        var fieldDeclaration = (FieldDeclarationSyntax)context.Node;

        // Skip const fields (they're often obvious)
        if (fieldDeclaration.Modifiers.Any(SyntaxKind.ConstKeyword))
        {
            return;
        }

        foreach (var variable in fieldDeclaration.Declaration.Variables)
        {
            AnalyzeMember(context, fieldDeclaration, fieldDeclaration.Modifiers, variable.Identifier, "field");
        }
    }

    private static void AnalyzeEvent(SyntaxNodeAnalysisContext context)
    {
        var eventDeclaration = (EventDeclarationSyntax)context.Node;
        AnalyzeMember(context, eventDeclaration, eventDeclaration.Modifiers, eventDeclaration.Identifier, "event");
    }

    private static void AnalyzeMember(
        SyntaxNodeAnalysisContext context,
        SyntaxNode node,
        SyntaxTokenList modifiers,
        SyntaxToken identifier,
        string memberType)
    {
        // Only analyze public members or interface members
        if (!IsPublicMember(modifiers) && !IsInterfaceMember(node))
        {
            return;
        }

        // Check for various exemption scenarios
        if (IsExemptFromDocumentation(node, context.SemanticModel))
        {
            return;
        }

        // Check if member has XML documentation
        if (!HasXmlDocumentation(node))
        {
            var diagnostic = Diagnostic.Create(
                Rule,
                identifier.GetLocation(),
                memberType,
                identifier.Text);
            context.ReportDiagnostic(diagnostic);
        }
    }

    private static bool IsPublicMember(SyntaxTokenList modifiers) =>
        // Member is public if it explicitly has public modifier
        // OR if it's in an interface (interface members are implicitly public)
        modifiers.Any(SyntaxKind.PublicKeyword);

    private static bool IsInterfaceMember(SyntaxNode node)
    {
        // Check if the member is inside an interface
        var parent = node.Parent;
        while (parent != null)
        {
            if (parent is InterfaceDeclarationSyntax)
            {
                return true;
            }

            parent = parent.Parent;
        }
        return false;
    }

    private static bool HasXmlDocumentation(SyntaxNode node)
    {
        // Check for XML documentation comments (///)
        var leadingTrivia = node.GetLeadingTrivia();

        return leadingTrivia.Any(trivia =>
            trivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia) ||
            trivia.IsKind(SyntaxKind.MultiLineDocumentationCommentTrivia));
    }

    private static bool IsSkippableMethod(MethodDeclarationSyntax method)
    {
        var methodName = method.Identifier.Text;

        // Skip Main method
        if (methodName == "Main")
        {
            return true;
        }

        // Skip override methods (they inherit documentation)
        if (method.Modifiers.Any(SyntaxKind.OverrideKeyword))
        {
            return true;
        }

        // Skip interface implementations (they should be documented on the interface)
        if (method.ExplicitInterfaceSpecifier != null)
        {
            return true;
        }

        // Skip constructors and destructors (often self-explanatory)
        if (method.Modifiers.Any(SyntaxKind.StaticKeyword) && methodName.EndsWith("Constructor"))
        {
            return true;
        }

        return false;
    }

    //  False-Positive Mitigation Methods

    /// <summary>
    /// Determines if a member is exempt from documentation requirements.
    /// </summary>
    /// <summary>
    /// Determines if a member is exempt from documentation requirements.
    /// </summary>
    private static bool IsExemptFromDocumentation(SyntaxNode node, SemanticModel semanticModel)
    {
        // Story 1.1: Exempt Blazor Partial Components
        if (IsBlazorPartialComponent(node))
        {
            return true;
        }

        // Story 1.2: Exempt Auto-Generated Files
        if (IsInAutoGeneratedFile(node))
        {
            return true;
        }

        // Story 1.3: Inherit Documentation for Record Members
        if (IsRecordMemberWithRecordDocumentation(node))
        {
            return true;
        }

        // Story 1.4: Exempt DTO/ViewModel Properties and Classes
        if (IsDtoOrViewModelProperty(node) || IsDtoOrViewModelClass(node))
        {
            return true;
        }

        // Story 1.5: Inherit Documentation from Interface Members
        if (IsInterfaceImplementationWithInterfaceDocumentation(node, semanticModel) || 
            IsClassImplementingDocumentedInterface(node, semanticModel))
        {
            return true;
        }

        // Story 1.6: Exempt Unit Test Classes
        if (IsUnitTestClass(node))
        {
            return true;
        }

        // Story 1.7: Aggregate Documentation from Partial Types
        if (IsPartialTypeWithDocumentationOnOtherPart(node, semanticModel))
        {
            return true;
        }

        // Story 1.8: Exempt Minimal API and Top-Level Statements
        if (IsInTopLevelStatementsFile(node))
        {
            return true;
        }

        // Story 1.9: Exempt Serialized Fields and Properties
        if (IsSerializedFieldOrProperty(node))
        {
            return true;
        }

        // Story 1.10: Provide an Opt-Out Attribute
        if (HasAllowUndocumentedMembersAttribute(node))
        {
            return true;
        }

        // Story 1.11: Exempt Attribute Classes
        if (IsAttributeClass(node))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Story 1.1: Exempt Blazor Partial Components
    /// </summary>
    /// <summary>
    /// Story 1.1: Exempt Blazor Partial Components
    /// </summary>
    /// <summary>
    /// Story 1.1: Exempt Blazor Partial Components
    /// </summary>
    /// <summary>
    /// Story 1.1: Exempt Blazor Partial Components
    /// </summary>
    /// <summary>
    /// Story 1.1: Exempt Blazor Partial Components
    /// </summary>
    /// <summary>
    /// Story 1.1: Exempt Blazor Partial Components
    /// </summary>
    private static bool IsBlazorPartialComponent(SyntaxNode node)
    {
        // Check if the node is a class declaration or inside a partial class that inherits from ComponentBase
        var classDeclaration = node as ClassDeclarationSyntax ?? node.Ancestors().OfType<ClassDeclarationSyntax>().FirstOrDefault();
        if (classDeclaration == null)
        {
            return false;
        }

        // Check if it's a partial class
        if (!classDeclaration.Modifiers.Any(SyntaxKind.PartialKeyword))
        {
            return false;
        }

        // Check if it inherits from ComponentBase - check both direct and fully qualified names
        var baseType = classDeclaration.BaseList?.Types.FirstOrDefault();
        if (baseType != null)
        {
            var baseTypeName = baseType.Type.ToString();
            if (baseTypeName == "ComponentBase" || 
                baseTypeName == "Microsoft.AspNetCore.Components.ComponentBase" ||
                baseTypeName.EndsWith(".ComponentBase"))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Story 1.2: Exempt Auto-Generated Files
    /// </summary>
    /// <summary>
    /// Story 1.2: Exempt Auto-Generated Files
    /// </summary>
    /// <summary>
    /// Story 1.2: Exempt Auto-Generated Files
    /// </summary>
    /// <summary>
    /// Story 1.2: Exempt Auto-Generated Files
    /// </summary>
    private static bool IsInAutoGeneratedFile(SyntaxNode node)
    {
        var syntaxTree = node.SyntaxTree;
        var root = syntaxTree.GetRoot();
        
        // Check the first few lines for auto-generated markers
        var firstTrivia = root.GetLeadingTrivia();
        
        foreach (var trivia in firstTrivia)
        {
            if (trivia.IsKind(SyntaxKind.SingleLineCommentTrivia) ||
                trivia.IsKind(SyntaxKind.MultiLineCommentTrivia))
            {
                var commentText = trivia.ToString().ToLowerInvariant();
                if (commentText.Contains("auto-generated") || 
                    commentText.Contains("this code was generated") ||
                    commentText.Contains("generated by a tool"))
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Story 1.3: Inherit Documentation for Record Members
    /// </summary>
    private static bool IsRecordMemberWithRecordDocumentation(SyntaxNode node)
    {
        var recordDeclaration = node.Ancestors().OfType<RecordDeclarationSyntax>().FirstOrDefault();
        if (recordDeclaration == null)
        {
            return false;
        }

        // Check if the record itself has documentation
        return HasXmlDocumentation(recordDeclaration);
    }

    /// <summary>
    /// Story 1.4: Exempt DTO/ViewModel Properties
    /// </summary>
    private static bool IsDtoOrViewModelProperty(SyntaxNode node)
    {
        var classDeclaration = node.Ancestors().OfType<ClassDeclarationSyntax>().FirstOrDefault();
        if (classDeclaration == null)
        {
            return false;
        }

        var className = classDeclaration.Identifier.Text;
        var namespaceName = GetNamespaceName(classDeclaration);

        // Check if it's a DTO or ViewModel
        return className.EndsWith("Dto") || 
               className.EndsWith("ViewModel") ||
               className.EndsWith("Model") ||
               namespaceName?.Contains(".Dto") == true ||
               namespaceName?.Contains(".ViewModel") == true ||
               namespaceName?.Contains(".Models") == true;
    }

    /// <summary>
    /// Story 1.4: Exempt DTO/ViewModel Classes
    /// </summary>
    private static bool IsDtoOrViewModelClass(SyntaxNode node)
    {
        if (node is ClassDeclarationSyntax classDeclaration)
        {
            var className = classDeclaration.Identifier.Text;
            var namespaceName = GetNamespaceName(classDeclaration);

            // Check if it's a DTO or ViewModel class
            return className.EndsWith("Dto") || 
                   className.EndsWith("ViewModel") ||
                   className.EndsWith("Model") ||
                   namespaceName?.Contains(".Dto") == true ||
                   namespaceName?.Contains(".ViewModel") == true ||
                   namespaceName?.Contains(".Models") == true;
        }

        return false;
    }

    /// <summary>
    /// Story 1.5: Inherit Documentation from Interface Members
    /// </summary>
    private static bool IsInterfaceImplementationWithInterfaceDocumentation(SyntaxNode node, SemanticModel semanticModel)
    {
        if (node is not MethodDeclarationSyntax methodDeclaration)
        {
            return false;
        }

        // Check if this method implements an interface member
        var symbol = semanticModel.GetDeclaredSymbol(methodDeclaration);
        if (symbol == null)
        {
            return false;
        }

        // Check explicit interface implementations
        var interfaceMembers = symbol.ExplicitInterfaceImplementations;
        foreach (var interfaceMember in interfaceMembers)
        {
            if (HasSymbolXmlDocumentation(interfaceMember))
            {
                return true;
            }
        }

        // Check implicit interface implementations
        var containingType = symbol.ContainingType;
        if (containingType != null)
        {
            foreach (var interfaceType in containingType.AllInterfaces)
            {
                var interfaceMember = interfaceType.GetMembers(symbol.Name).FirstOrDefault();
                if (interfaceMember != null && HasSymbolXmlDocumentation(interfaceMember))
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Story 1.5: Exempt Classes Implementing Documented Interfaces
    /// </summary>
    private static bool IsClassImplementingDocumentedInterface(SyntaxNode node, SemanticModel semanticModel)
    {
        if (node is ClassDeclarationSyntax classDeclaration)
        {
            var classSymbol = semanticModel.GetDeclaredSymbol(classDeclaration);
            if (classSymbol != null)
            {
                foreach (var interfaceType in classSymbol.AllInterfaces)
                {
                    if (HasSymbolXmlDocumentation(interfaceType))
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Story 1.6: Exempt Unit Test Classes
    /// </summary>
    private static bool IsUnitTestClass(SyntaxNode node)
    {
        ClassDeclarationSyntax classDeclaration;
        
        // If the node itself is a class declaration, use it directly
        if (node is ClassDeclarationSyntax directClass)
        {
            classDeclaration = directClass;
        }
        else
        {
            // Otherwise, look for the class declaration in ancestors
            classDeclaration = node.Ancestors().OfType<ClassDeclarationSyntax>().FirstOrDefault();
        }
        
        if (classDeclaration == null)
        {
            return false;
        }

        var className = classDeclaration.Identifier.Text;
        var namespaceName = GetNamespaceName(classDeclaration);

        // Check if it's a test class
        return className.EndsWith("Tests") || 
               className.EndsWith("Test") ||
               namespaceName?.Contains(".Tests") == true ||
               namespaceName?.Contains(".Test") == true;
    }

    /// <summary>
    /// Story 1.7: Aggregate Documentation from Partial Types
    /// </summary>
    private static bool IsPartialTypeWithDocumentationOnOtherPart(SyntaxNode node, SemanticModel semanticModel)
    {
        // Check if the current node is a partial class with documentation
        if (node is ClassDeclarationSyntax classDeclaration && 
            classDeclaration.Modifiers.Any(SyntaxKind.PartialKeyword) &&
            HasXmlDocumentation(classDeclaration))
        {
            return true;
        }

        // Check if we're inside a partial class or if the node itself is a partial class
        var containingClass = node.Ancestors().OfType<ClassDeclarationSyntax>().FirstOrDefault();
        if (containingClass == null && node is ClassDeclarationSyntax nodeClass)
        {
            containingClass = nodeClass;
        }

        if (containingClass == null || !containingClass.Modifiers.Any(SyntaxKind.PartialKeyword))
        {
            return false;
        }

        // Check if any part of the partial class has documentation
        var symbol = semanticModel.GetDeclaredSymbol(containingClass);
        if (symbol == null)
        {
            return false;
        }

        // Check all syntax references for documentation
        foreach (var reference in symbol.DeclaringSyntaxReferences)
        {
            var otherPart = reference.GetSyntax();
            if (HasXmlDocumentation(otherPart))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Story 1.8: Exempt Minimal API and Top-Level Statements
    /// </summary>
    private static bool IsInTopLevelStatementsFile(SyntaxNode node)
    {
        var syntaxTree = node.SyntaxTree;
        var root = syntaxTree.GetRoot();

        // Check if the file has top-level statements (not in a namespace)
        var hasTopLevelStatements = root.ChildNodes().Any(n => 
            n is not NamespaceDeclarationSyntax && 
            n is not UsingDirectiveSyntax &&
            n is not GlobalStatementSyntax == false);

        return hasTopLevelStatements;
    }

    /// <summary>
    /// Story 1.9: Exempt Serialized Fields and Properties
    /// </summary>
    private static bool IsSerializedFieldOrProperty(SyntaxNode node)
    {
        // Check for serialization attributes on the current node
        var attributeLists = node switch
        {
            PropertyDeclarationSyntax prop => prop.AttributeLists,
            FieldDeclarationSyntax field => field.AttributeLists,
            _ => default
        };

        if (attributeLists != default)
        {
            foreach (var attributeList in attributeLists)
            {
                foreach (var attribute in attributeList.Attributes)
                {
                    var attributeName = attribute.Name.ToString();
                    if (attributeName.Contains("JsonPropertyName") ||
                        attributeName.Contains("DataMember") ||
                        attributeName.Contains("Required") ||
                        attributeName.Contains("Serializable"))
                    {
                        return true;
                    }
                }
            }
        }

        // If the current node is a class, check if it contains serialized properties
        if (node is ClassDeclarationSyntax classDeclaration)
        {
            foreach (var member in classDeclaration.Members)
            {
                if (member is PropertyDeclarationSyntax prop)
                {
                    foreach (var attributeList in prop.AttributeLists)
                    {
                        foreach (var attribute in attributeList.Attributes)
                        {
                            var attributeName = attribute.Name.ToString();
                            if (attributeName.Contains("JsonPropertyName") ||
                                attributeName.Contains("DataMember") ||
                                attributeName.Contains("Required") ||
                                attributeName.Contains("Serializable"))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Story 1.10: Provide an Opt-Out Attribute
    /// </summary>
    private static bool HasAllowUndocumentedMembersAttribute(SyntaxNode node)
    {
        // Check attributes on the current node
        var attributeLists = node switch
        {
            ClassDeclarationSyntax cls => cls.AttributeLists,
            MethodDeclarationSyntax method => method.AttributeLists,
            PropertyDeclarationSyntax prop => prop.AttributeLists,
            FieldDeclarationSyntax field => field.AttributeLists,
            _ => default
        };

        if (attributeLists != default)
        {
            foreach (var attributeList in attributeLists)
            {
                foreach (var attribute in attributeList.Attributes)
                {
                    var attributeName = attribute.Name.ToString();
                    if (attributeName == "AllowUndocumentedMembers" ||
                        attributeName.EndsWith(".AllowUndocumentedMembers"))
                    {
                        return true;
                    }
                }
            }
        }

        // If the current node doesn't have the attribute, check the containing class
        var containingClass = node.Ancestors().OfType<ClassDeclarationSyntax>().FirstOrDefault();
        if (containingClass != null)
        {
            foreach (var attributeList in containingClass.AttributeLists)
            {
                foreach (var attribute in attributeList.Attributes)
                {
                    var attributeName = attribute.Name.ToString();
                    if (attributeName == "AllowUndocumentedMembers" ||
                        attributeName.EndsWith(".AllowUndocumentedMembers"))
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Story 1.11: Exempt Attribute Classes
    /// </summary>
    private static bool IsAttributeClass(SyntaxNode node)
    {
        if (node is ClassDeclarationSyntax classDeclaration)
        {
            var className = classDeclaration.Identifier.Text;
            return className.EndsWith("Attribute");
        }
        return false;
    }

     // 

    //  Helper Methods

    private static string? GetNamespaceName(SyntaxNode node)
    {
        var namespaceDeclaration = node.Ancestors().OfType<NamespaceDeclarationSyntax>().FirstOrDefault();
        return namespaceDeclaration?.Name.ToString();
    }

    private static bool HasSymbolXmlDocumentation(ISymbol symbol)
    {
        return !string.IsNullOrEmpty(symbol.GetDocumentationCommentXml());
    }

     // 
}