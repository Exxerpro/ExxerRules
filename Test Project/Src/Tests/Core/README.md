# IndTrace Unit Test Generation Tools

This directory contains automated tools for generating and managing unit tests for the IndTrace solution.

## Files

- `generate_unit_tests.bat` - Simple batch script to scan for classes needing tests
- `generate_unit_tests.ps1` - PowerShell script for automated test generation
- `run_test_generation.bat` - Menu-driven batch interface for all test operations
- `README.md` - This documentation file

## Quick Start

### Option 1: Menu-Driven Interface (Recommended)
```bash
run_test_generation.bat
```

This provides a user-friendly menu with options to:
1. Scan for classes that need tests
2. Generate all test files
3. Generate test for specific class
4. Run existing tests
5. Show test coverage report

### Option 2: Direct PowerShell Usage
```bash
# Scan for classes (no generation)
powershell -ExecutionPolicy Bypass -File "generate_unit_tests.ps1"

# Generate all test files
powershell -ExecutionPolicy Bypass -File "generate_unit_tests.ps1" -GenerateAll

# Generate test for specific class
powershell -ExecutionPolicy Bypass -File "generate_unit_tests.ps1" -SpecificClass "CreateBarCodeCommandHandler"
```

### Option 3: Simple Batch Scan
```bash
generate_unit_tests.bat
```

## Test Generation Strategy

### Application Layer Tests
Generated tests follow this pattern:
- **Command Handlers**: Test constructor, Handle method, validation
- **Query Handlers**: Test constructor, Handle method, data retrieval
- **Validators**: Test validation rules, error messages
- **Services**: Test business logic, dependencies
- **Specifications**: Test specification logic, filtering

### Domain Layer Tests
Generated tests follow this pattern:
- **Entities**: Test constructors, properties, domain logic
- **Value Objects**: Test immutability, equality, validation
- **Domain Services**: Test business rules, calculations
- **Specifications**: Test domain specifications

## Test Style Guidelines

All generated tests follow these conventions:

### Framework Stack
- **xUnit** - Testing framework
- **NSubstitute** - Mocking framework
- **Shouldly** - Assertion library

### Naming Convention
```csharp
[MethodName]_[Scenario]_[ExpectedResult]
// Examples:
Constructor_WithValidParameters_ShouldCreateInstance()
Handle_WithValidCommand_ShouldReturnSuccess()
Validate_WithInvalidData_ShouldReturnErrors()
```

### Structure (Arrange-Act-Assert)
```csharp
[Fact]
public void MethodName_Scenario_ExpectedResult()
{
    // Arrange
    // Setup test data and dependencies

    // Act
    // Execute the method under test

    // Assert
    // Verify the results
}
```

## Generated Test Templates

### Application Layer Template
```csharp
public class ClassNameTests
{
    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateInstance()
    {
        // TODO: Add constructor parameters
        var instance = new ClassName();
        instance.ShouldNotBeNull();
    }

    [Fact]
    public void Constructor_WithInvalidParameters_ShouldThrowException()
    {
        // TODO: Add invalid parameters and exception assertion
    }

    [Fact]
    public void Properties_WhenSet_ShouldReturnCorrectValues()
    {
        // TODO: Test property setters and getters
    }

    [Fact]
    public void Methods_WhenCalled_ShouldReturnExpectedResults()
    {
        // TODO: Call methods and verify results
    }
}
```

### Domain Layer Template
```csharp
public class ClassNameTests
{
    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateInstance()
    {
        // TODO: Add constructor parameters
        var instance = new ClassName();
        instance.ShouldNotBeNull();
    }

    [Fact]
    public void Constructor_WithInvalidParameters_ShouldThrowException()
    {
        // TODO: Add invalid parameters and exception assertion
    }

    [Fact]
    public void Properties_WhenSet_ShouldReturnCorrectValues()
    {
        // TODO: Test property setters and getters
    }

    [Fact]
    public void Methods_WhenCalled_ShouldReturnExpectedResults()
    {
        // TODO: Call methods and verify results
    }

    [Fact]
    public void DomainLogic_WhenExecuted_ShouldFollowBusinessRules()
    {
        // TODO: Execute domain logic and verify business rules
    }
}
```

## Test Coverage Goals

### Priority 1: Critical Path
- Command/Query handlers
- Domain entities
- Business logic services

### Priority 2: Supporting Classes
- Validators
- Specifications
- Value objects

### Priority 3: Infrastructure
- Repositories
- External service adapters

## Running Tests

### All Tests
```bash
dotnet test
```

### Specific Test Project
```bash
dotnet test --project Tests/Core/Application.UnitTests
dotnet test --project Tests/Core/Domain.UnitTests
```

### With Coverage
```bash
dotnet test --collect:"XPlat Code Coverage"
```

## Continuous Integration

These tools are designed to work in CI/CD pipelines:

```yaml
# Example GitHub Actions step
- name: Generate Unit Tests
  run: |
    cd Src/Tests/Core
    powershell -ExecutionPolicy Bypass -File "generate_unit_tests.ps1" -GenerateAll

- name: Run Tests
  run: |
    dotnet test --collect:"XPlat Code Coverage"
```

## Troubleshooting

### PowerShell Execution Policy
If you get execution policy errors:
```bash
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
```

### Missing Dependencies
Ensure test projects have these packages:
```xml
<PackageReference Include="Microsoft.NET.Test.Sdk" />
<PackageReference Include="xunit" />
<PackageReference Include="xunit.runner.visualstudio" />
<PackageReference Include="NSubstitute" />
<PackageReference Include="Shouldly" />
```

### Test Discovery Issues
- Ensure test classes are public
- Ensure test methods are public and have `[Fact]` or `[Theory]` attributes
- Check that test projects reference the correct source projects

## Contributing

When adding new test generation templates:
1. Update the PowerShell script with new template functions
2. Add appropriate test patterns for the new class types
3. Update this README with new guidelines
4. Ensure generated tests follow the established conventions
