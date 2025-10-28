# Calculator Demo - XUnit v3 Universal Configuration Pattern

This project demonstrates the **XUnit v3 Universal Configuration Pattern** with a simple calculator application and comprehensive test suite.

## 🎯 What This Demonstrates

- ✅ **XUnit v3 Universal Configuration** - Works across all environments
- ✅ **Microsoft Testing Platform Integration** - Full MTP support
- ✅ **Modern .NET 10 Testing** - Latest C# features and patterns
- ✅ **Comprehensive Test Coverage** - Unit, integration, and data-driven tests
- ✅ **Mocking with NSubstitute** - Advanced testing scenarios
- ✅ **Shouldly Assertions** - Readable and maintainable assertions

## 🚀 Quick Start

### Prerequisites
- .NET 10 SDK
- Visual Studio 2022 or VS Code

### Running the Tests

```bash
# Navigate to the test project
cd Calculator.Tests

# Run all tests
dotnet test

# Run with detailed output
dotnet test --logger:"console;verbosity=detailed"

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test class
dotnet test --filter "BasicCalculatorTests"

# Run specific test method
dotnet test --filter "Add_ShouldReturnCorrectSum_WhenGivenTwoNumbers"
```

### Building the Solution

```bash
# Build the entire solution
dotnet build

# Build in Release mode
dotnet build -c Release
```

## 📁 Project Structure

```
CalculatorDemo/
├── Calculator/                    # Main calculator library
│   ├── Calculator.csproj         # Library project file
│   ├── ICalculator.cs            # Calculator interface
│   └── BasicCalculator.cs        # Calculator implementation
├── Calculator.Tests/             # Test project
│   ├── Calculator.Tests.csproj   # Test project with XUnit v3 config
│   ├── BasicCalculatorTests.cs   # Unit tests
│   ├── CalculatorIntegrationTests.cs # Integration tests
│   ├── CalculatorMockingTests.cs # Mocking examples
│   └── CalculatorDataDrivenTests.cs # Data-driven tests
└── README.md                     # This file
```

## 🧪 Test Categories

### 1. Unit Tests (`BasicCalculatorTests.cs`)
- Tests individual methods in isolation
- Covers happy path and error scenarios
- Uses both `[Fact]` and `[Theory]` attributes

### 2. Integration Tests (`CalculatorIntegrationTests.cs`)
- Tests calculator with real dependencies
- Covers concurrent operations
- Tests edge cases and precision

### 3. Mocking Tests (`CalculatorMockingTests.cs`)
- Demonstrates NSubstitute mocking capabilities
- Shows verification patterns
- Covers async scenarios

### 4. Data-Driven Tests (`CalculatorDataDrivenTests.cs`)
- Uses `[Theory]` with `[InlineData]`
- Tests multiple input combinations
- Covers edge cases systematically

## 🔧 XUnit v3 Configuration Features

This project uses the **XUnit v3 Universal Configuration Pattern** which provides:

### ✅ Universal Compatibility
- **Console**: `dotnet test` execution
- **Visual Studio**: Full Test Explorer integration
- **VS Code**: Complete debugging support
- **CI/CD**: Pipeline-friendly configuration
- **PowerShell**: Windows PowerShell integration

### ✅ Modern Testing Stack
- **XUnit v3**: Latest testing framework
- **Microsoft Testing Platform**: Full MTP integration
- **NSubstitute**: Advanced mocking
- **Shouldly**: Readable assertions
- **Coverlet**: Code coverage collection

### ✅ Enterprise Features
- **TRX Reporting**: Structured test results
- **Code Coverage**: Built-in coverage collection
- **Parallel Execution**: Optimized performance
- **Global Usings**: Reduced boilerplate

## 📊 Test Execution Examples

### Running Specific Tests

```bash
# Run all tests in a class
dotnet test --filter "ClassName=BasicCalculatorTests"

# Run tests with specific trait
dotnet test --filter "Category=Unit"

# Run tests matching a pattern
dotnet test --filter "Name~Add"

# Run tests with specific namespace
dotnet test --filter "FullyQualifiedName~Calculator.Tests.BasicCalculatorTests"
```

### Coverage Analysis

```bash
# Generate coverage report
dotnet test --collect:"XPlat Code Coverage"

# Coverage report will be generated in:
# TestResults/[guid]/coverage.cobertura.xml
```

## 🎯 Key Testing Patterns Demonstrated

### 1. Basic Unit Testing
```csharp
[Fact]
public void Add_ShouldReturnCorrectSum_WhenGivenTwoNumbers()
{
    // Arrange
    var calculator = new BasicCalculator();
    var a = 5.0;
    var b = 3.0;
    var expected = 8.0;

    // Act
    var result = calculator.Add(a, b);

    // Assert
    result.ShouldBe(expected);
}
```

### 2. Data-Driven Testing
```csharp
[Theory]
[InlineData(0, 0, 0)]
[InlineData(1, 1, 2)]
[InlineData(-1, 1, 0)]
public void Add_ShouldReturnCorrectSum_ForVariousInputs(double a, double b, double expected)
{
    // Act
    var result = _calculator.Add(a, b);

    // Assert
    result.ShouldBe(expected);
}
```

### 3. Exception Testing
```csharp
[Fact]
public void Divide_ShouldThrowDivideByZeroException_WhenDivisorIsZero()
{
    // Arrange
    var a = 10.0;
    var b = 0.0;

    // Act & Assert
    Should.Throw<DivideByZeroException>(() => _calculator.Divide(a, b))
        .Message.ShouldBe("Cannot divide by zero.");
}
```

### 4. Mocking and Verification
```csharp
[Fact]
public void MockedCalculator_ShouldVerifyMethodCalls_WhenMethodsAreInvoked()
{
    // Arrange
    var mockCalculator = Substitute.For<ICalculator>();
    var a = 10.0;
    var b = 5.0;

    // Act
    mockCalculator.Add(a, b);
    mockCalculator.Subtract(a, b);

    // Assert
    mockCalculator.Received(1).Add(a, b);
    mockCalculator.Received(1).Subtract(a, b);
    mockCalculator.DidNotReceive().Multiply(Arg.Any<double>(), Arg.Any<double>());
}
```

## 🚀 Benefits of This Configuration

1. **Zero Configuration Drift** - Works consistently across all environments
2. **Future-Proof** - Compatible with .NET 11+ and future XUnit versions
3. **Developer Productivity** - Global usings and modern C# features
4. **Enterprise Ready** - Full CI/CD integration and reporting
5. **Community Standard** - Follows .NET testing best practices

## 📚 Learn More

- [XUnit v3 Universal Configuration Pattern](../docs/XUnit-v3-Universal-Configuration-Pattern.md)
- [XUnit v3 Official Documentation](https://xunit.net/docs/v3)
- [Microsoft Testing Platform Guide](https://docs.microsoft.com/en-us/dotnet/core/testing/)
- [NSubstitute Documentation](https://nsubstitute.github.io/)
- [Shouldly Documentation](https://shouldly.readthedocs.io/)

---

**This project demonstrates the power and flexibility of the XUnit v3 Universal Configuration Pattern. Use it as a template for your own .NET 10 test projects!** 🎉
