# Solution Reorganization Instructions

## Overview

This reorganization aligns the physical folder structure with Visual Studio's logical folder structure. The solution file has been updated to reflect the new paths.

## Target Structure

After reorganization, the folder structure will match Visual Studio's logical folders:

```
src/
├── code/
│   ├── Analyzer/
│   │   ├── IndFusion.Analyzer/
│   │   └── IndFusion.Fixer/
│   ├── Cli/
│   │   ├── IndFusion.Tools.Cli/
│   │   ├── IndFusion.Tools.Cli.Core/
│   │   └── IndFusion.Tools.Cli.Console/
│   ├── Mcp/
│   │   ├── IndFusion.Mcp.Core/
│   │   ├── IndFusion.Mcp.Server/
│   │   └── IndFusion.Mcp.Web/
│   ├── SemanticRag/
│   │   ├── IndFusion.SemanticRag.Domain/
│   │   ├── IndFusion.SemanticRag.Application/
│   │   ├── IndFusion.SemanticRag.Infrastructure/
│   │   └── IndFusion.SemanticRag.WebAPI/
│   └── IndFusion.Packaging/
└── test/
    ├── AnalyzerTests/
    │   └── IndFusion.Analyzer.Tests/
    ├── CliTests/
    │   └── IndFusion.Tools.Cli.Tests/
    ├── McpTests/
    │   ├── IndFusion.Mcp.Core.Tests/
    │   ├── IndFusion.Mcp.Server.Tests/
    │   ├── IndFusion.Mcp.Tests/
    │   ├── IndFusion.Mcp.Tests.Integration/
    │   └── IndFusion.Mcp.Web.Tests/
    ├── SemanticRagTests/
    │   ├── IndFusion.SemanticRag.Tests/
    │   ├── IndFusion.SemanticRag.Tests.Unit/
    │   ├── IndFusion.SemanticRag.Tests.Integration/
    │   ├── IndFusion.SemanticRag.Tests.Architecture/
    │   └── IndFusion.SemanticRag.Tests.System/
    ├── IndFusion.SemanticRag.Tests.Standalone/
    └── IndFusion.Architecture.Tests/
```

## Steps to Complete Reorganization

### Step 1: Run the Reorganization Script

Run `reorganize-complete.ps1` to move all projects to their new locations:

```powershell
cd src
.\reorganize-complete.ps1
```

### Step 2: Update Project References

After moving projects, run `update-references.ps1` to update all ProjectReference paths in .csproj files:

```powershell
cd src
.\update-references.ps1
```

### Step 3: Manual Verification

1. Open the solution in Visual Studio
2. Verify all projects load correctly
3. Build the solution to ensure all references are correct
4. Run tests to verify everything works

### Step 4: Add Missing Projects (if needed)

If `IndFusion.Mcp.Tests.Integration` or `IndFusion.SemanticRag.Tests.System` are not in the solution, add them manually in Visual Studio or update the solution file.

## What Has Been Updated

✅ **Solution File** (`IndFusion.sln`) - All project paths updated to match new structure

## What Still Needs to Be Done

1. ✅ Move projects to new folders (run `reorganize-complete.ps1`)
2. ✅ Update ProjectReference paths in all .csproj files (run `update-references.ps1`)
3. ✅ Verify solution loads correctly
4. ⚠️ Add any missing test projects to solution (if needed)

## Notes

- The solution file has already been updated with the new paths
- After running the scripts, Visual Studio should open the solution without any issues
- All logical folder groupings in Visual Studio will now match the physical folder structure

