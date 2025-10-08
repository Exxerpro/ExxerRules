# Microsoft Testing Platform Migration Guide

## Overview

This guide provides a complete migration path from xUnit v2 to Microsoft Testing Platform with xUnit v3 for the IndTrace solution.

## Benefits

- **Performance**: Up to 3x faster test execution
- **Modern Architecture**: Latest testing platform with better extensibility
- **Better Reporting**: Enhanced test results and diagnostics
- **Parallel Execution**: Improved parallel test execution
- **Future-Proof**: Microsoft's recommended testing platform

## Prerequisites

- .NET 8.0 or later
- PowerShell 7.0 or later
- Git (for backup branches)

## Migration Steps

### 1. Automatic Migration (Recommended)

Run the automated migration script:

```powershell
# Dry run to see what would be migrated
.\Src\Tests\migration-script.ps1 -DryRun

# Perform the actual migration
.\Src\Tests\migration-script.ps1
```

### 2. Manual Migration (If Needed)

If automatic migration fails, follow these manual steps:

#### Step 2.1: Update Project Files

For each test project, update the `.csproj` file:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>

    <!-- Microsoft Testing Platform Configuration -->
    <UseMicrosoftTestingPlatformRunner>true</UseMicrosoftTestingPlatformRunner>
    <TestingPlatformDotnetTestSupport>true</TestingPlatformDotnetTestSupport>
    <TestingPlatformServer>true</TestingPlatformServer>
  </PropertyGroup>

  <ItemGroup>
    <!-- Remove old xUnit packages -->
    <!-- <PackageReference Include="xunit" /> -->
    <!-- <PackageReference Include="xunit.runner.visualstudio" /> -->

    <!-- Microsoft Testing Platform -->
    <PackageReference Include="Microsoft.Testing.Platform" />
    <PackageReference Include="Microsoft.Testing.Extensions.TrxReport" />

    <!-- xUnit v3 -->
    <PackageReference Include="xunit.v3" />
    <PackageReference Include="xunit.v3.core" />
    <PackageReference Include="xunit.v3.runner.visualstudio">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
  </ItemGroup>

  <!-- Microsoft Testing Platform Configuration -->
  <ItemGroup>
    <ProjectCapability Include="DiagnoseCapabilities" />
    <ProjectCapability Include="TestingPlatformServer" />
    <ProjectCapability Include="TestContainer" />
  </ItemGroup>
</Project>
```

#### Step 2.2: Create Testing Platform Configuration

Create a `{ProjectName}.testingplatformconfig.json` file in each test project:

```json
{
  "$schema": "https://aka.ms/testingplatform/v1/configuration.schema.json",
  "version": "1",
  "runSettings": {
    "dotnetTestSupport": {
      "enabled": true
    },
    "xunit": {
      "enabled": true,
      "parallelizeTestCollections": true,
      "maxParallelThreads": 0,
      "diagnosticMessages": false,
      "internalDiagnosticMessages": false,
      "failSkips": false,
      "stopOnFail": false,
      "preEnumerateTheories": true
    }
  },
  "logging": {
    "logLevel": {
      "default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "extensions": [
    {
      "extensionId": "Microsoft.Testing.Extensions.TrxReport",
      "configuration": {
        "outputPath": "TestResults",
        "fileName": "test-results.trx"
      }
    }
  ],
  "telemetry": {
    "enabled": false
  },
  "diagnostics": {
    "enabled": false
  }
}
```

### 3. Update Test Code (If Needed)

Most xUnit v2 tests will work without changes. However, some updates may be needed:

#### 3.1: Using Statements

Update using statements:

```csharp
// Old (xUnit v2)
using Xunit;
using Xunit.Abstractions;

// New (xUnit v3)
using Xunit;
using Xunit.Abstractions;
```

#### 3.2: Test Attributes

Most attributes remain the same:

```csharp
[Fact]
[Theory]
[InlineData(1, 2, 3)]
[MemberData(nameof(TestData))]
[Trait("Category", "Unit")]
```

#### 3.3: Test Output

Update test output if needed:

```csharp
// Old
public class MyTests : IClassFixture<TestFixture>
{
    private readonly ITestOutputHelper _output;

    public MyTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void Test()
    {
        _output.WriteLine("Test output");
    }
}

// New (same syntax, but better performance)
public class MyTests : IClassFixture<TestFixture>
{
    private readonly ITestOutputHelper _output;

    public MyTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void Test()
    {
        _output.WriteLine("Test output");
    }
}
```

### 4. Update CI/CD Pipelines

Update your CI/CD pipelines to use the new testing platform:

#### Azure DevOps

```yaml
- task: DotNetCoreCLI@2
  displayName: 'Run Tests'
  inputs:
    command: 'test'
    projects: '**/*Tests/*.csproj'
    arguments: '--logger trx --results-directory $(Build.ArtifactStagingDirectory)/TestResults'
    publishTestResults: true
    testResultsFormat: 'VSTest'
    testResultsFiles: '**/*.trx'
```

#### GitHub Actions

```yaml
- name: Run Tests
  run: |
    dotnet test --logger trx --results-directory TestResults
    dotnet tool install --global Microsoft.Testing.Platform.Cli
    testingplatform test --project **/*Tests/*.csproj
```

### 5. Update Test Data Management

If you're using the optimized test data management system:

```csharp
// Use the new test data manager
public class MyTests : IClassFixture<ModernTestDataManager>
{
    private readonly ModernTestDataManager _testData;

    public MyTests(ModernTestDataManager testData)
    {
        _testData = testData;
    }

    [Fact]
    public async Task TestWithOptimizedData()
    {
        var context = await _testData.CreateOptimizedContextAsync();
        // Your test logic here
    }
}
```

## Validation

### 1. Build Validation

```bash
dotnet build Src
```

### 2. Test Execution

```bash
# Run all tests
dotnet test Src/Tests --logger trx

# Run specific test project
dotnet test Src/Tests/Core/Application.UnitTests --logger trx

# Run with Microsoft Testing Platform CLI
dotnet tool install --global Microsoft.Testing.Platform.Cli
testingplatform test --project Src/Tests/Core/Application.UnitTests/Application.UnitTests.csproj
```

### 3. Performance Comparison

Compare test execution times:

```bash
# Before migration
time dotnet test Src/Tests --logger trx

# After migration
time dotnet test Src/Tests --logger trx
```

## Troubleshooting

### Common Issues

#### 1. Package Version Conflicts

If you encounter package version conflicts:

```bash
# Clean solution
dotnet clean Src
dotnet restore Src

# Rebuild
dotnet build Src
```

#### 2. Test Discovery Issues

If tests aren't being discovered:

1. Check that `UseMicrosoftTestingPlatformRunner` is set to `true`
2. Verify the testing platform configuration file exists
3. Ensure all required packages are installed

#### 3. Performance Issues

If performance doesn't improve:

1. Check the testing platform configuration
2. Verify parallel execution settings
3. Review test data loading optimization

### Rollback Plan

If you need to rollback:

```bash
# Restore from backup files
Get-ChildItem -Path "Src" -Recurse -Filter "*.csproj.backup" | ForEach-Object {
    Copy-Item $_.FullName ($_.FullName -replace '\.backup$', '')
}

# Or restore from git backup branch
git checkout backup/pre-microsoft-testing-platform-YYYYMMDD-HHMMSS
```

## Best Practices

### 1. Test Organization

- Group related tests in the same class
- Use meaningful test names
- Follow AAA pattern (Arrange, Act, Assert)

### 2. Performance Optimization

- Use the optimized test data management system
- Minimize database calls in tests
- Use parallel execution where appropriate

### 3. Configuration

- Keep testing platform configuration minimal
- Disable telemetry in CI/CD environments
- Use appropriate log levels

## Resources

- [Microsoft Testing Platform Documentation](https://learn.microsoft.com/en-us/dotnet/testing-platform/)
- [xUnit v3 Documentation](https://xunit.net/)
- [Migration Guide](https://learn.microsoft.com/en-us/dotnet/testing-platform/migrate-from-vstest)
- [Performance Best Practices](https://learn.microsoft.com/en-us/dotnet/testing-platform/performance)

## Support

If you encounter issues during migration:

1. Check the troubleshooting section above
2. Review the Microsoft Testing Platform documentation
3. Create an issue in the project repository
4. Contact the development team

## Migration Checklist

- [ ] Run automated migration script
- [ ] Verify all test projects are migrated
- [ ] Update CI/CD pipelines
- [ ] Test build and execution
- [ ] Validate performance improvements
- [ ] Update documentation
- [ ] Train team on new features
- [ ] Monitor test execution in production

---

**Note**: This migration provides significant performance improvements and modernizes the testing infrastructure. The automated script handles most of the complexity, making the migration process smooth and reliable.
