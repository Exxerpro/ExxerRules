# IndFusion.Tools.Cli Enhancement Plan

## Meta
**Title**: IndFusion.Tools.Cli Enhancement Plan
**Description**: Comprehensive plan to transform the CLI from placeholder to professional refactoring tool
**Created-at**: 2025-01-15T00:00:00Z
**Last-updated-at**: 2025-01-15T00:00:00Z
**Applies-to**: IndFusion.Tools.Cli project, CLI development, refactoring tools
**File-matcher**: src/code/IndFusion.Tools.Cli/**/*.cs, CLI commands, tool execution

## Current State Analysis

### What We Have:
- ✅ **Placeholder CLI** with basic structure (`Program.cs` - just prints messages)
- ✅ **Extensive Tool Infrastructure** (35+ refactoring tools in `/Tools` directory)
- ✅ **MCP Core Library** with service abstractions and Roslyn integration
- ✅ **NuGet Tool Packaging** configured (`PackAsTool=true`, command name: `indfusion`)
- ✅ **Roslyn Integration** with syntax rewriters, walkers, and analysis tools
- ✅ **Solution Loading** and workspace management capabilities

### What's Missing:
- ❌ **Command Line Interface** (System.CommandLine integration)
- ❌ **Tool Discovery & Execution** system
- ❌ **Configuration Management** (settings, profiles)
- ❌ **Interactive Mode** for guided refactoring
- ❌ **Batch Operations** support
- ❌ **Progress Reporting** and logging integration
- ❌ **Help System** and documentation

## Phase 1: Core CLI Infrastructure (Week 1-2)

### 1.1 Command Line Interface Setup
```csharp
// New files to create:
- Commands/BaseCommand.cs
- Commands/RefactorCommand.cs  
- Commands/AnalyzeCommand.cs
- Commands/ConfigCommand.cs
- Commands/InteractiveCommand.cs
- Configuration/AppSettings.cs
- Services/CommandExecutor.cs
- Services/ToolDiscoveryService.cs
```

**Implementation:**
- Integrate `System.CommandLine` for modern CLI experience
- Create command hierarchy: `indfusion <verb> <options>`
- Implement global options: `--verbose`, `--config`, `--log-level`
- Add command completion and help system

### 1.2 Tool Discovery & Execution Engine
```csharp
// Enhance existing:
- Tools/ListTools.cs (make it discoverable)
- Services/ToolRegistry.cs (new)
- Services/RefactoringOrchestrator.cs (new)
```

**Features:**
- Dynamic tool discovery using reflection
- Tool parameter validation and help generation
- Execution context management (solution, workspace, logging)
- Error handling and rollback capabilities

### 1.3 Configuration Management
```csharp
// New files:
- Configuration/IndFusionConfig.cs
- Configuration/ConfigManager.cs
- Configuration/DefaultSettings.json
```

**Configuration Options:**
- Default solution paths
- Logging preferences
- Tool-specific settings
- Workspace cache settings
- Output formatting options

## Phase 2: Core Commands Implementation (Week 3-4)

### 2.1 Refactor Command
```bash
# Usage examples:
indfusion refactor extract-method --solution MyApp.sln --file Services/UserService.cs --range "10:5-15:20" --name "ValidateUserInput"
indfusion refactor move-method --solution MyApp.sln --file Services/UserService.cs --method "ProcessUser" --target "Services/UserProcessor"
indfusion refactor introduce-variable --solution MyApp.sln --file Services/UserService.cs --line 25 --column 10 --name "userData"
```

**Implementation:**
- Map existing tools to CLI commands
- Parameter validation and transformation
- Progress reporting during execution
- Result formatting and output

### 2.2 Analyze Command
```bash
# Usage examples:
indfusion analyze metrics --solution MyApp.sln --path "src/"
indfusion analyze complexity --solution MyApp.sln --file Services/UserService.cs
indfusion analyze opportunities --solution MyApp.sln --path "src/" --output refactoring-opportunities.json
```

**Features:**
- Code metrics calculation
- Complexity analysis
- Refactoring opportunity detection
- Export results to JSON/CSV

### 2.3 Interactive Mode
```bash
# Usage:
indfusion interactive --solution MyApp.sln
```

**Features:**
- Guided refactoring workflow
- Step-by-step code analysis
- Interactive tool selection
- Preview changes before applying
- Undo/redo capabilities

## Phase 3: Advanced Features (Week 5-6)

### 3.1 Batch Operations
```bash
# Usage examples:
indfusion batch --solution MyApp.sln --script refactoring-script.json
indfusion batch --solution MyApp.sln --pattern "*.cs" --tool "extract-method" --auto-name
```

**Features:**
- Script-based batch processing
- Pattern-based file selection
- Automated parameter generation
- Progress tracking and reporting
- Error handling and continuation

### 3.2 Configuration Profiles
```bash
# Usage examples:
indfusion config create --name "enterprise" --template enterprise.json
indfusion refactor --profile enterprise --solution MyApp.sln --tool extract-method
```

**Features:**
- Multiple configuration profiles
- Profile templates for different scenarios
- Environment-specific settings
- Team sharing capabilities

### 3.3 Integration Features
```bash
# Usage examples:
indfusion integrate --solution MyApp.sln --analyzer ExxerRules
indfusion integrate --solution MyApp.sln --fixer ExxerRules --auto-fix
```

**Features:**
- Integration with ExxerRules analyzers
- Automatic fix application
- Custom rule integration
- CI/CD pipeline integration

## Phase 4: Developer Experience (Week 7-8)

### 4.1 Enhanced Help System
- Context-sensitive help
- Interactive tutorials
- Video demonstrations
- Example scenarios
- Best practices guide

### 4.2 Logging & Diagnostics
```bash
# Usage examples:
indfusion refactor --log-level debug --log-file refactoring.log
indfusion analyze --verbose --metrics-detail high
```

**Features:**
- Structured logging with Serilog
- Performance metrics collection
- Debug information output
- Error reporting and telemetry

### 4.3 Output Formatting
```bash
# Usage examples:
indfusion analyze --output json --format pretty
indfusion refactor --output markdown --include-diff
```

**Features:**
- Multiple output formats (JSON, Markdown, HTML)
- Diff generation and display
- Progress bars and status updates
- Color-coded output

## Phase 5: Testing & Documentation (Week 9-10)

### 5.1 Testing Strategy
- Unit tests for all commands
- Integration tests with sample solutions
- Performance benchmarks
- Error scenario testing
- User acceptance testing

### 5.2 Documentation
- Complete CLI reference
- Getting started guide
- Advanced usage scenarios
- Troubleshooting guide
- API documentation

## Technical Implementation Details

### Command Structure:
```csharp
public class RefactorCommand : BaseCommand
{
    [Option("--solution", Description = "Path to solution file")]
    public string SolutionPath { get; set; } = string.Empty;
    
    [Option("--tool", Description = "Refactoring tool to execute")]
    public string ToolName { get; set; } = string.Empty;
    
    [Option("--file", Description = "Target file path")]
    public string FilePath { get; set; } = string.Empty;
    
    // ... other options
}
```

### Tool Execution Pipeline:
```csharp
public class RefactoringOrchestrator
{
    public async Task<RefactoringResult> ExecuteAsync(
        RefactoringRequest request, 
        CancellationToken cancellationToken)
    {
        // 1. Validate inputs
        // 2. Load solution/workspace
        // 3. Discover and instantiate tool
        // 4. Execute refactoring
        // 5. Apply changes
        // 6. Generate report
        // 7. Cleanup resources
    }
}
```

### Configuration Schema:
```json
{
  "defaultSolution": "MyApp.sln",
  "logging": {
    "level": "Information",
    "output": "console",
    "file": "indfusion.log"
  },
  "tools": {
    "extractMethod": {
      "defaultVisibility": "private",
      "autoFormat": true
    }
  },
  "workspace": {
    "cacheEnabled": true,
    "cacheDirectory": ".indfusion-cache"
  }
}
```

## Success Metrics

### Functionality Metrics:
- ✅ All 35+ existing tools accessible via CLI
- ✅ <2 second tool discovery time
- ✅ <5 second solution loading time
- ✅ 100% command coverage with help

### User Experience Metrics:
- ✅ Intuitive command structure
- ✅ Comprehensive error messages
- ✅ Progress indication for long operations
- ✅ Consistent output formatting

### Performance Metrics:
- ✅ <100ms command startup time
- ✅ Memory usage <500MB for typical solutions
- ✅ Support for solutions with 1000+ files

## Implementation Priority

### High Priority (Must Have):
1. **Command Line Interface** - Core functionality
2. **Tool Discovery & Execution** - Essential for tool usage
3. **Refactor Command** - Primary use case
4. **Configuration Management** - User experience

### Medium Priority (Should Have):
1. **Analyze Command** - Code analysis capabilities
2. **Interactive Mode** - Enhanced user experience
3. **Batch Operations** - Power user features
4. **Enhanced Logging** - Debugging and monitoring

### Low Priority (Nice to Have):
1. **Integration Features** - Advanced scenarios
2. **Multiple Output Formats** - Flexibility
3. **Advanced Configuration** - Power user features
4. **Performance Optimizations** - Scalability

## Example Usage Scenarios

### Basic Refactoring:
```bash
# Extract method refactoring
indfusion refactor extract-method --solution MyApp.sln --file Services/UserService.cs --range "10:5-15:20" --name "ValidateUserInput"

# Move method to different class
indfusion refactor move-method --solution MyApp.sln --file Services/UserService.cs --method "ProcessUser" --target "Services/UserProcessor"

# Introduce variable for complex expression
indfusion refactor introduce-variable --solution MyApp.sln --file Services/UserService.cs --line 25 --column 10 --name "userData"
```

### Code Analysis:
```bash
# Generate code metrics
indfusion analyze metrics --solution MyApp.sln --path "src/" --output metrics.json

# Analyze complexity
indfusion analyze complexity --solution MyApp.sln --file Services/UserService.cs

# Find refactoring opportunities
indfusion analyze opportunities --solution MyApp.sln --path "src/" --output opportunities.json
```

### Interactive Workflow:
```bash
# Start interactive mode
indfusion interactive --solution MyApp.sln

# Guided refactoring session
# 1. Select file to analyze
# 2. Choose refactoring tool
# 3. Preview changes
# 4. Apply or skip
# 5. Continue with next opportunity
```

### Batch Operations:
```bash
# Execute script-based refactoring
indfusion batch --solution MyApp.sln --script refactoring-script.json

# Pattern-based bulk operations
indfusion batch --solution MyApp.sln --pattern "*.cs" --tool "extract-method" --auto-name

# Apply fixes from analyzer
indfusion integrate --solution MyApp.sln --fixer ExxerRules --auto-fix
```

## Conclusion

This plan transforms the CLI from a placeholder into a powerful, professional-grade refactoring tool that leverages the extensive infrastructure already built in the project. The phased approach ensures steady progress while maintaining quality and user experience throughout the development process.

The resulting CLI will provide developers with a modern, intuitive interface to access the powerful refactoring capabilities already implemented in the IndFusion.Tools.Cli project, making code refactoring more accessible and efficient.
