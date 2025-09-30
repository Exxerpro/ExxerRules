# Plan for Adding .NET Standard 2.0 Support

This document outlines the plan to add .NET Standard 2.0 support to the `IndFusion.Mcp.Core` project and other relevant projects in the solution. This will enable the libraries to be used by a wider range of .NET implementations.

## 1. Analysis

- **`IndFusion.Analyzer`:** The `IndFusion.Analyzer` project already targets `netstandard2.0`, so no changes are required for this project.

- **`IndFusion.Mcp.Core` and other projects:** The target framework for most projects in the solution is defined in `src/Directory.Build.props`. The current target framework is `net10.0`, which appears to be a custom framework definition.

## 2. Proposed Changes

To add .NET Standard 2.0 support, we will modify the `src/Directory.Build.props` file to support multi-targeting. This will allow the projects to be compiled for both `net10.0` and `netstandard2.0`.

### 2.1. Modify `Directory.Build.props`

The following change should be made to `src/Directory.Build.props`:

**Current:**
```xml
<PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
</PropertyGroup>
```

**Proposed:**
```xml
<PropertyGroup>
    <TargetFrameworks>net10.0;netstandard2.0</TargetFrameworks>
</PropertyGroup>
```

This change will cause all projects that inherit this property to be built for both target frameworks.

### 2.2. Dependency Analysis

After making the change to `Directory.Build.props`, it is important to analyze the dependencies of each project to ensure they are compatible with `.NET Standard 2.0`. The .NET Portability Analyzer can be a useful tool for this purpose.

For any dependencies that are not compatible with `.NET Standard 2.0`, you will need to find a compatible version or an alternative library.

### 2.3. Conditional Compilation

In some cases, you may need to use conditional compilation to write code that is specific to a particular target framework. This can be done using the `#if` preprocessor directive.

For example, if you need to use an API that is only available in `.NET Standard 2.0`, you can use the following code:

```csharp
#if NETSTANDARD2_0
    // Code that uses .NET Standard 2.0 specific APIs
#endif
```

For more information on preprocessor directives, see the [Microsoft documentation](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/preprocessor-directives).

## 3. Verification

After making the changes, it is important to build the entire solution and run all tests to ensure that the changes have not introduced any regressions.

You can build the solution from the command line using the following command:

```
dotnet build
```

You can run the tests using the following command:

```
dotnet test
```

## 4. Microsoft Documentation

- **Multi-targeting:** [https://docs.microsoft.com/en-us/dotnet/standard/library-guidance/cross-platform-targeting](https://docs.microsoft.com/en-us/dotnet/standard/library-guidance/cross-platform-targeting)
- **.NET Standard:** [https://docs.microsoft.com/en-us/dotnet/standard/net-standard](https://docs.microsoft.com/en-us/dotnet/standard/net-standard)

This plan provides a comprehensive overview of the steps required to add .NET Standard 2.0 support to the solution. By following these steps, you can ensure that the libraries are compatible with a wider range of .NET implementations.
