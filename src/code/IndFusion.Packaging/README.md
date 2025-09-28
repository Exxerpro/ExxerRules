# ExxerRules - Modern Development Conventions

[![NuGet](https://img.shields.io/nuget/v/ExxerRules.svg)](https://www.nuget.org/packages/ExxerRules/)
[![Downloads](https://img.shields.io/nuget/dt/ExxerRules.svg)](https://www.nuget.org/packages/ExxerRules/)
[![MIT License](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)

**ExxerRules** is a comprehensive suite of 20 production-ready Roslyn analyzers that enforce Modern Development Conventions (MDC) for C# projects. No IDE extensions required - works everywhere .NET works.

## 🚀 Quick Start

### Installation

```bash
# Package Manager Console
Install-Package ExxerRules

# .NET CLI
dotnet add package ExxerRules

# PackageReference
<PackageReference Include="ExxerRules" Version="1.0.0" PrivateAssets="all" />
```

### Zero Configuration

Works out-of-the-box! The analyzers activate automatically and start improving your code quality immediately.

## 📦 What's Included

### 🏗️ **Architecture** (2 analyzers)
- **ER1001**: Domain Should Not Reference Infrastructure
- **ER1002**: Use Repository Pattern

### ⚡ **Async Best Practices** (2 analyzers)  
- **ER2001**: Async Methods Should Accept CancellationToken
- **ER2002**: Use ConfigureAwait(false)

### 🛡️ **Error Handling** (2 analyzers)
- **ER6001**: Avoid Throwing Exceptions
- **ER6002**: Use Result<T> Pattern

### 🧪 **Modern Testing** (5 analyzers)
- **ER12001**: Do Not Mock DbContext
- **ER12002**: Do Not Use FluentAssertions (use Shouldly)
- **ER12003**: Do Not Use Moq (use NSubstitute)
- **ER12004**: Test Naming Convention
- **ER12005**: Use xUnit v3

### ⚡ **Performance** (1 analyzer)
- **ER11001**: Use Efficient LINQ

### ✨ **Modern C#** (2 analyzers)
- **ER9001**: Use Expression-Bodied Members
- **ER9002**: Use Modern Pattern Matching

### 💎 **Code Quality** (2 analyzers)
- **ER4001**: Avoid Magic Numbers and Strings
- **ER4002**: Do Not Use Regions

### 📚 **Documentation** (1 analyzer)
- **ER5001**: Public Members Should Have XML Documentation

### 📊 **Logging** (2 analyzers)
- **ER8001**: Do Not Use Console.WriteLine
- **ER8002**: Use Structured Logging

### 🛡️ **Null Safety** (1 analyzer)
- **ER10001**: Validate Null Parameters

### 🔄 **Functional Patterns** (1 analyzer)
- **ER7001**: Do Not Throw Exceptions in Functional Code

### 🎨 **Code Formatting** (2 analyzers)
- **ER3001**: Code Formatting Standards
- **ER3002**: Project Formatting Standards

## ⚙️ Configuration

### Basic Configuration

Add to your `.csproj` file:

```xml
<PropertyGroup>
  <!-- Enable/disable all analyzers -->
  <ExxerRulesEnabled>true</ExxerRulesEnabled>

  <!-- Set severity: error, warning, info, hint -->
  <ExxerRulesSeverity>warning</ExxerRulesSeverity>
</PropertyGroup>
```

### Advanced Configuration

Enable/disable specific categories:

```xml
<PropertyGroup>
  <ExxerRulesArchitecture>true</ExxerRulesArchitecture>
  <ExxerRulesAsync>true</ExxerRulesAsync>
  <ExxerRulesErrorHandling>true</ExxerRulesErrorHandling>
  <ExxerRulesTesting>true</ExxerRulesTesting>
  <ExxerRulesPerformance>true</ExxerRulesPerformance>
  <ExxerRulesModernCSharp>true</ExxerRulesModernCSharp>
  <ExxerRulesCodeQuality>true</ExxerRulesCodeQuality>
  <ExxerRulesDocumentation>false</ExxerRulesDocumentation>
  <ExxerRulesLogging>true</ExxerRulesLogging>
  <ExxerRulesNullSafety>true</ExxerRulesNullSafety>
  <ExxerRulesFunctionalPatterns>true</ExxerRulesFunctionalPatterns>
  <ExxerRulesCodeFormatting>true</ExxerRulesCodeFormatting>
</PropertyGroup>
```

### Directory.Build.props Integration

Apply to all projects in your solution:

```xml
<!-- Directory.Build.props -->
<Project>
  <PropertyGroup>
    <ExxerRulesEnabled>true</ExxerRulesEnabled>
    <ExxerRulesSeverity>warning</ExxerRulesSeverity>
    <!-- Disable documentation rules for test projects -->
    <ExxerRulesDocumentation Condition="$(MSBuildProjectName.Contains('Test'))">false</ExxerRulesDocumentation>
  </PropertyGroup>
</Project>
```

## 🎯 Modern Development Conventions

### ✅ **Recommended Patterns**
- **Result<T>** for error handling instead of exceptions
- **Repository Pattern** for data access abstraction
- **xUnit v3** with **NSubstitute** and **Shouldly** for testing
- **CancellationToken** for async operations
- **ConfigureAwait(false)** for library code
- **Structured logging** with **ILogger<T>**

### ❌ **Anti-Patterns Detected**
- Exception throwing for control flow
- Direct infrastructure references from domain
- Magic numbers and strings
- Console.WriteLine for logging
- Legacy test frameworks (MSTest, NUnit)
- FluentAssertions and Moq (replaced by better alternatives)

## 🛠️ CI/CD Integration

Works seamlessly in build pipelines:

```yaml
# Azure DevOps
- task: DotNetCoreCLI@2
  displayName: 'Build with ExxerRules'
  inputs:
    command: 'build'
    projects: '**/*.csproj'
    arguments: '--configuration Release --verbosity normal'

# GitHub Actions
- name: Build with ExxerRules
  run: dotnet build --configuration Release --verbosity normal
```

## 📋 Requirements

- **.NET Standard 2.0** or higher
- **C# 7.0** or higher
- Works with:
  - Visual Studio 2019/2022
  - Visual Studio Code (with C# extension)
  - JetBrains Rider
  - Command line builds
  - CI/CD systems

## 🌟 Benefits

- **Zero Setup** - Install and go
- **Consistent Quality** - Same rules across team and CI/CD
- **Gradual Adoption** - Enable categories incrementally
- **Performance** - Fast analysis with minimal impact
- **Modern Standards** - Based on current C# best practices
- **Open Source** - MIT licensed, community driven

## 📄 License

Licensed under the [MIT License](https://github.com/exxerai/exxer-rules/blob/main/LICENSE).

## 🔗 Links

- **GitHub**: https://github.com/exxerai/exxer-rules
- **Issues**: https://github.com/exxerai/exxer-rules/issues
- **NuGet**: https://www.nuget.org/packages/ExxerRules/

---

**Built with ❤️ by the ExxerAI Team**

*Making C# development more reliable, maintainable, and performant - everywhere.*
