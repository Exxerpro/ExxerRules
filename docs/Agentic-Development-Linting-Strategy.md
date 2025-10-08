# Agentic Development Linting Strategy with IndFusion Analyzers

## 🎯 **Overview**

This document outlines a comprehensive strategy for using IndFusion analyzers as linters for agentic development, creating a "watcher" system that ensures AI-generated code adheres to your established coding standards and architectural patterns.

## 📊 **Available Analyzer Categories**

### **1. Error Handling & Functional Patterns (EXXER001-003)**

- **EXXER001**: Use Result Pattern
- **EXXER002**: Avoid Throwing Exceptions
- **EXXER003**: Do Not Throw Exceptions - Use Result`<T>` Pattern
- **EXXER200**: Validate Null Parameters at Method Entry

### **2. Async Programming (EXXER300-302)**

- **EXXER300**: Async Methods Should Accept CancellationToken
- **EXXER301**: Use ConfigureAwait(false) in Library Code
- **EXXER302**: Avoid Async Void Methods Except Event Handlers

### **3. Documentation (EXXER400)**

- **EXXER400**: Public Members Should Have XML Documentation

### **4. Code Quality (EXXER500-503, EXXER700-702, EXXER900-901)**

- **EXXER500**: Avoid Magic Numbers and Strings
- **EXXER501**: Use Expression-Bodied Members Where Appropriate
- **EXXER503**: Do Not Use Regions for Code Organization
- **EXXER700**: Use Efficient LINQ Operations
- **EXXER702**: Use Modern Pattern Matching with Declaration Patterns
- **EXXER900**: Format Project Using dotnet format Command
- **EXXER901**: Code Formatting Inconsistency Detected

### **5. Architecture (EXXER600-601)**

- **EXXER600**: Domain Layer Should Not Reference Infrastructure Layer
- **EXXER601**: Use Repository Pattern with Focused Interfaces

### **6. Logging (EXXER800-801)**

- **EXXER800**: Use Structured Logging Instead of String Concatenation
- **EXXER801**: Do Not Use Console.WriteLine in Production Code

### **7. Testing (EXXER100-104)**

- **EXXER100**: Test Naming Convention
- **EXXER101**: Use XUnit v3
- **EXXER102**: Use Shouldly
- **EXXER103**: Use NSubstitute
- **EXXER104**: Do Not Mock DbContext

## 🏗️ **Implementation Strategy**

### **Phase 1: Real-Time Linting Integration**

#### **1.1 IDE Integration**

```xml
<!-- Add to your base project files -->
<PackageReference Include="IndFusion.Analyzer" Version="1.0.6" />
<PackageReference Include="IndFusion.Fixer" Version="1.0.6" />
```

#### **1.2 EditorConfig Configuration**

```ini
# .editorconfig
[*.cs]
# IndFusion Analyzer Rules
dotnet_diagnostic.EXXER001.severity = error
dotnet_diagnostic.EXXER002.severity = error
dotnet_diagnostic.EXXER003.severity = error
dotnet_diagnostic.EXXER200.severity = warning
dotnet_diagnostic.EXXER300.severity = info
dotnet_diagnostic.EXXER301.severity = warning
dotnet_diagnostic.EXXER302.severity = warning
dotnet_diagnostic.EXXER400.severity = info
dotnet_diagnostic.EXXER500.severity = warning
dotnet_diagnostic.EXXER501.severity = info
dotnet_diagnostic.EXXER503.severity = warning
dotnet_diagnostic.EXXER600.severity = error
dotnet_diagnostic.EXXER601.severity = warning
dotnet_diagnostic.EXXER700.severity = warning
dotnet_diagnostic.EXXER800.severity = warning
dotnet_diagnostic.EXXER801.severity = warning
dotnet_diagnostic.EXXER900.severity = hidden
dotnet_diagnostic.EXXER901.severity = info
```

### **Phase 2: Agentic Development Workflow**

#### **2.1 Pre-Generation Validation**

```yaml
# .github/workflows/agentic-linting.yml
name: Agentic Development Linting
on:
  pull_request:
    paths:
      - 'src/**/*.cs'
      - '*.cs'

jobs:
  lint-agentic-code:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '10.0.x'
    
      - name: Install IndFusion Analyzers
        run: |
          dotnet add package IndFusion.Analyzer --version 1.0.6
          dotnet add package IndFusion.Fixer --version 1.0.6
    
      - name: Run IndFusion Linting
        run: |
          dotnet build --verbosity normal --no-restore
          dotnet format --verify-no-changes --verbosity diagnostic
    
      - name: Check for Agentic Code Violations
        run: |
          # Custom script to check for AI-generated code patterns
          dotnet run --project tools/AgenticCodeValidator
```

#### **2.2 Real-Time Monitoring**

```csharp
// tools/AgenticCodeValidator/Program.cs
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using IndFusion.Analyzer;

public class AgenticCodeValidator
{
    public static async Task<int> Main(string[] args)
    {
        var workspace = MSBuildWorkspace.Create();
        var project = await workspace.OpenProjectAsync(args[0]);
      
        var compilation = await project.GetCompilationAsync();
        var analyzers = GetIndFusionAnalyzers();
      
        var analysis = await compilation.WithAnalyzers(analyzers).GetAnalyzerDiagnosticsAsync();
      
        var violations = analysis.Where(d => d.Severity >= DiagnosticSeverity.Warning);
      
        if (violations.Any())
        {
            Console.WriteLine("🚨 Agentic Code Violations Detected:");
            foreach (var violation in violations)
            {
                Console.WriteLine($"  {violation.Id}: {violation.GetMessage()}");
            }
            return 1;
        }
      
        Console.WriteLine("✅ All IndFusion standards met!");
        return 0;
    }
  
    private static ImmutableArray<DiagnosticAnalyzer> GetIndFusionAnalyzers()
    {
        return ImmutableArray.Create<DiagnosticAnalyzer>(
            new UseResultPatternAnalyzer(),
            new AvoidThrowingExceptionsAnalyzer(),
            new ValidateNullParametersAnalyzer(),
            new AsyncMethodsShouldAcceptCancellationTokenAnalyzer(),
            new UseConfigureAwaitFalseAnalyzer(),
            new AvoidAsyncVoidAnalyzer(),
            new PublicMembersShouldHaveXmlDocumentationAnalyzer(),
            new AvoidMagicNumbersAndStringsAnalyzer(),
            new DoNotUseRegionsAnalyzer(),
            new DomainShouldNotReferenceInfrastructureAnalyzer(),
            new UseRepositoryPatternAnalyzer(),
            new UseEfficientLinqAnalyzer(),
            new UseStructuredLoggingAnalyzer(),
            new DoNotUseConsoleWriteLineAnalyzer()
        );
    }
}
```

### **Phase 3: AI Agent Integration**

#### **3.1 Cursor/VS Code Integration**

```json
// .vscode/settings.json
{
    "dotnet.completion.showCompletionItemsFromUnimportedNamespaces": true,
    "dotnet.inlayHints.enableInlayHintsForParameters": true,
    "dotnet.inlayHints.enableInlayHintsForLiteralParameters": true,
    "dotnet.inlayHints.enableInlayHintsForIndexerParameters": true,
    "dotnet.inlayHints.enableInlayHintsForObjectCreationParameters": true,
    "dotnet.inlayHints.enableInlayHintsForOtherParameters": true,
    "dotnet.inlayHints.suppressInlayHintsForParametersThatDifferOnlyBySuffix": true,
    "dotnet.inlayHints.suppressInlayHintsForParametersThatMatchMethodIntent": true,
    "dotnet.inlayHints.suppressInlayHintsForParametersThatMatchArgumentName": true,
  
    // IndFusion Analyzer Integration
    "omnisharp.enableRoslynAnalyzers": true,
    "omnisharp.enableEditorConfigSupport": true,
    "omnisharp.enableImportCompletion": true,
  
    // Real-time linting
    "editor.codeActionsOnSave": {
        "source.fixAll": "explicit",
        "source.organizeImports": "explicit"
    }
}
```

#### **3.2 MCP Server Integration**

```csharp
// Add to your MCP server tools
[McpServerTool("validate-agentic-code")]
public static async Task<string> ValidateAgenticCode(string filePath)
{
    var workspace = MSBuildWorkspace.Create();
    var project = await workspace.OpenProjectAsync(filePath);
    var compilation = await project.GetCompilationAsync();
  
    var analyzers = GetIndFusionAnalyzers();
    var analysis = await compilation.WithAnalyzers(analyzers).GetAnalyzerDiagnosticsAsync();
  
    var violations = analysis.Where(d => d.Severity >= DiagnosticSeverity.Warning)
                           .GroupBy(d => d.Id)
                           .Select(g => new {
                               Rule = g.Key,
                               Count = g.Count(),
                               Examples = g.Take(3).Select(d => d.GetMessage())
                           });
  
    return JsonSerializer.Serialize(violations, new JsonSerializerOptions { WriteIndented = true });
}
```

### **Phase 4: Continuous Monitoring**

#### **4.1 File System Watcher**

```csharp
// tools/AgenticWatcher/Program.cs
public class AgenticCodeWatcher
{
    private readonly FileSystemWatcher _watcher;
    private readonly IndFusionLinter _linter;
  
    public AgenticCodeWatcher(string projectPath)
    {
        _linter = new IndFusionLinter();
        _watcher = new FileSystemWatcher(projectPath, "*.cs")
        {
            IncludeSubdirectories = true,
            EnableRaisingEvents = true
        };
      
        _watcher.Changed += OnFileChanged;
        _watcher.Created += OnFileCreated;
    }
  
    private async void OnFileChanged(object sender, FileSystemEventArgs e)
    {
        if (IsAgenticCode(e.FullPath))
        {
            var violations = await _linter.AnalyzeFileAsync(e.FullPath);
            if (violations.Any())
            {
                await NotifyViolations(e.FullPath, violations);
            }
        }
    }
  
    private bool IsAgenticCode(string filePath)
    {
        // Detect AI-generated code patterns
        var content = File.ReadAllText(filePath);
        return content.Contains("// Generated by") || 
               content.Contains("// AI-generated") ||
               HasAgenticPatterns(content);
    }
}
```

#### **4.2 Real-Time Dashboard**

```csharp
// Add to your MCP Web dashboard
public class LintingDashboard : ComponentBase
{
    [Inject] private ILintingService LintingService { get; set; }
  
    private List<LintingViolation> _violations = new();
    private Timer _refreshTimer;
  
    protected override async Task OnInitializedAsync()
    {
        await RefreshViolations();
        _refreshTimer = new Timer(async _ => await RefreshViolations(), null, 
                                 TimeSpan.Zero, TimeSpan.FromSeconds(30));
    }
  
    private async Task RefreshViolations()
    {
        _violations = await LintingService.GetRecentViolationsAsync();
        StateHasChanged();
    }
}
```

## 🎯 **Agentic Development Rules**

### **Critical Rules for AI-Generated Code:**

1. **EXXER003**: Never throw exceptions - always use Result`<T>`
2. **EXXER200**: Always validate null parameters
3. **EXXER300**: Async methods must accept CancellationToken
4. **EXXER400**: All public members need XML documentation
5. **EXXER600**: Maintain clean architecture boundaries
6. **EXXER800**: Use structured logging, never Console.WriteLine

### **Quality Gates:**

- **Error Level**: Must pass (EXXER001, EXXER002, EXXER003, EXXER600, EXXER104)
- **Warning Level**: Should pass (EXXER200, EXXER301, EXXER302, EXXER500, EXXER503, EXXER601, EXXER700, EXXER800, EXXER801)
- **Info Level**: Recommended (EXXER300, EXXER400, EXXER501, EXXER702, EXXER901)

## 🚀 **Implementation Roadmap**

### **Week 1: Foundation**

- [ ] Add IndFusion.Analyzer to all projects
- [ ] Configure .editorconfig with severity levels
- [ ] Set up basic CI/CD linting

### **Week 2: Real-Time Integration**

- [ ] Implement file system watcher
- [ ] Add MCP server linting tools
- [ ] Create real-time dashboard

### **Week 3: AI Agent Integration**

- [ ] Integrate with Cursor/VS Code
- [ ] Add pre-commit hooks
- [ ] Implement agentic code detection

### **Week 4: Monitoring & Optimization**

- [ ] Set up continuous monitoring
- [ ] Create violation reporting
- [ ] Optimize performance

## 📊 **Success Metrics**

- **Code Quality**: 95%+ compliance with IndFusion standards
- **AI Code Quality**: 90%+ of AI-generated code passes linting
- **Developer Experience**: <2 second linting feedback
- **Violation Reduction**: 80% reduction in repeated violations

## 🔧 **Tools & Commands**

### **Manual Linting:**

```bash
# Check all projects
dotnet build --verbosity normal

# Format code
dotnet format --verify-no-changes

# Run specific analyzers
dotnet build /p:RunAnalyzersDuringBuild=true /p:RunAnalyzersDuringLiveAnalysis=true
```

### **Automated Fixes:**

```bash
# Apply automatic fixes
dotnet format

# Fix specific rule violations
dotnet build /p:CodeAnalysisRuleSet=IndFusion.ruleset
```

This strategy transforms your IndFusion analyzers into a comprehensive "watcher" system that ensures AI-generated code maintains your high standards for clean architecture, functional programming, and modern C# practices.
