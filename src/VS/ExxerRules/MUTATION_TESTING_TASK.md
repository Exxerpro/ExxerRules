# Mutation Testing Task - Agentic Mode

## 🎯 Primary Objective
Run Stryker mutation testing, analyze the results, and kill surviving mutants by adding behavioral tests.

## 🚨 CRITICAL ISSUE: Command Hanging Problem
**WARNING**: The `dotnet stryker` command hangs indefinitely on this system. This is a known issue that needs to be addressed before proceeding.

## 📋 Task Breakdown

### Phase 1: Fix Command Hanging Issue
1. **Problem**: `dotnet stryker` command hangs and never returns
2. **Attempted Solutions** (all failed):
   - PowerShell timeout wrappers
   - Batch file timeout approaches
   - Standalone C# timeout runners
   - Direct command execution
3. **Required**: Find a working solution to run Stryker without hanging

### Phase 2: Run Stryker Mutation Testing
1. **Command**: `dotnet stryker` (from ExxerRules.Analyzers directory)
2. **Configuration**: Uses `stryker-config.json` in parent directory
3. **Expected Output**: HTML report in StrykerOutput directory
4. **Goal**: Generate mutation testing report

### Phase 3: Analyze Results
1. **Locate Report**: Check `StrykerOutput/[timestamp]/mutation-report.json`
2. **Identify Surviving Mutants**: Look for mutants with status "Survived"
3. **Analyze Coverage Gaps**: Understand why mutants survived
4. **Document Findings**: Create list of surviving mutants and their locations

### Phase 4: Kill Surviving Mutants
1. **Add Behavioral Tests**: Create tests that would catch the surviving mutants
2. **Focus Areas**: 
   - Result class operations (from ResultComprehensiveTests.cs)
   - Analyzer edge cases (from AnalyzerEdgeCaseTests.cs)
   - Any other areas with low mutation coverage
3. **Test Strategy**: Add tests that verify the specific behavior that mutants are changing

## 📁 Key Files and Directories

### Project Structure
```
src/VS/ExxerRules/
├── ExxerRules.Analyzers/          # Main project to mutate
├── ExxerRules.Tests/              # Test project
├── stryker-config.json            # Stryker configuration
├── StrykerOutput/                 # Output directory (if exists)
└── MUTATION_TESTING_TASK.md       # This file
```

### Important Test Files
- `ExxerRules.Tests/TestCases/Operations/ResultComprehensiveTests.cs`
- `ExxerRules.Tests/TestCases/AnalyzerEdgeCaseTests.cs`

### Configuration
- `stryker-config.json`: Contains project settings, reporters, thresholds

## 🔧 Technical Details

### Stryker Configuration
```json
{
  "stryker-config": {
    "project": "ExxerRules.Analyzers",
    "test-projects": ["ExxerRules.Tests"],
    "reporters": ["html", "progress", "json"],
    "mutation-level": "Advanced",
    "concurrency": 4,
    "thresholds": {
      "high": 80,
      "low": 60,
      "break": 0
    }
  }
}
```

### Expected Commands
```bash
# From ExxerRules.Analyzers directory
dotnet stryker

# Or with explicit parameters
dotnet stryker --reporter Progress,Html,Json --test-project ../ExxerRules.Tests
```

## 🎯 Success Criteria
1. ✅ Stryker runs without hanging
2. ✅ Mutation report is generated
3. ✅ Surviving mutants are identified
4. ✅ Additional tests are added to kill surviving mutants
5. ✅ Mutation score improves

## 🚨 Known Issues
1. **Command Hanging**: Primary blocker - needs immediate attention
2. **Dashboard Reporter**: May require API key if enabled
3. **Timeout Handling**: All attempted timeout solutions failed

## 💡 Suggested Approaches for Next Agent

### Option 1: Environment Investigation
- Check if there are environment variables causing issues
- Verify .NET and Stryker versions
- Look for conflicting processes

### Option 2: Alternative Mutation Testing
- Consider using different mutation testing tools
- Explore running Stryker in a different environment
- Use Docker container for isolation

### Option 3: Manual Analysis
- If Stryker continues to hang, manually analyze code coverage
- Focus on areas with known gaps
- Add comprehensive tests based on code review

## 📊 Current State
- **Status**: BLOCKED - Command hanging issue
- **Progress**: 0% - Cannot run Stryker
- **Priority**: HIGH - Need working mutation testing
- **Next Step**: Solve command hanging problem

## 🔍 Files to Check for Existing Results
- `StrykerOutput/` directory for any existing reports
- `MUTATION_TESTING_IMPROVEMENT_REPORT.md` for previous attempts
- Any existing mutation reports in the project

## 📝 Notes for Next Agent
- This is a C# .NET project with Roslyn analyzers
- Focus on testing the Result pattern and analyzer edge cases
- Use xUnit v3, NSubstitute, and Shouldly for testing
- Follow the established testing patterns in the existing test files
- The project uses modern C# features and functional programming patterns

## 🎯 Immediate Action Required
**SOLVE THE COMMAND HANGING ISSUE FIRST** - everything else depends on this.

