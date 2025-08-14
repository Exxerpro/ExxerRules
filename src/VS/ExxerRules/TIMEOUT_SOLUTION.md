# Timeout Solution for Hanging Commands

## Problem
The `dotnet test` and `dotnet stryker` commands were hanging and not returning when errors occurred, making it difficult to run tests and mutation testing reliably.

## Solution
Created a standalone C# console application that wraps commands with proper timeout handling and error management.

## Files Created

### Standalone Test Runner
- `TestRunnerStandalone/Program.cs` - Main test runner with timeout functionality
- `TestRunnerStandalone/TestRunnerStandalone.csproj` - Project file
- `run-tests-standalone.bat` - Batch file to run tests with timeout

### Standalone Stryker Runner
- `TestRunnerStandalone/StrykerRunner.cs` - Stryker runner with timeout functionality
- `run-stryker-standalone.bat` - Batch file to run Stryker with timeout

## Usage

### Running Tests with Timeout
```bash
# Using the batch file (recommended)
run-tests-standalone.bat

# Or directly with dotnet run
cd TestRunnerStandalone
dotnet run -- "dotnet test --filter \"ResultComprehensiveTests\" --verbosity minimal" 600
```

### Running Stryker with Timeout
```bash
# Using the batch file (recommended)
run-stryker-standalone.bat

# Or directly with dotnet run
cd TestRunnerStandalone
dotnet run --project TestRunnerStandalone.csproj -- "dotnet stryker --reporters Progress,Html,Json --project ExxerRules.Analyzers --test-project ExxerRules.Tests" 900
```

## Features

1. **Timeout Protection**: Commands are automatically killed after the specified timeout
2. **Error Handling**: Proper error capture and display
3. **Output Capture**: Both stdout and stderr are captured and displayed
4. **Exit Code Handling**: Proper exit code propagation
5. **Configurable Timeouts**: Default 10 minutes for tests, 15 minutes for Stryker

## How It Works

The standalone runner uses:
- `Process.Start()` to launch the command
- `Task.WhenAny()` to wait for either completion or timeout
- `process.Kill()` to terminate hanging processes
- Async/await for proper timeout handling

## Alternative Solutions Attempted

1. **PowerShell Scripts**: Failed due to PowerShell not being in PATH
2. **Batch Files with timeout**: Failed due to command hanging issues
3. **MCP Serena Server**: Had compatibility issues with shell execution

## Benefits

- **Reliable**: No more hanging commands
- **Fast**: Quick timeout detection and process termination
- **Flexible**: Configurable timeouts and commands
- **Standalone**: No dependencies on the main solution
- **Cross-platform**: Works on Windows (can be adapted for other platforms)

## Testing

The solution has been tested with:
- Simple echo commands (verification)
- dotnet test commands (actual use case)
- Various timeout values (30s, 120s, 600s)

All tests completed successfully without hanging.
