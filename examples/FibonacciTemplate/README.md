# Fibonacci Template Project

A comprehensive .NET 10 template project demonstrating the **XUnit v3 Universal Configuration Pattern** with a Fibonacci sequence calculator.

## 🚀 Features

- **Clean Architecture**: Separation of concerns with Core, Console, and Test projects
- **XUnit v3 Universal Configuration**: Proven testing setup with Microsoft Testing Platform
- **Comprehensive Testing**: Unit tests, performance tests, mocking tests, and data-driven tests
- **Input Validation**: Robust validation with meaningful error messages
- **Performance Optimization**: Both recursive and iterative Fibonacci implementations
- **Console Application**: Interactive and command-line interface support

## 📁 Project Structure

```
FibonacciTemplate/
├── Fibonacci.Core/           # Core business logic
│   ├── IFibonacciCalculator.cs
│   └── FibonacciCalculator.cs
├── Fibonacci.Console/        # Console application
│   └── Program.cs
├── Fibonacci.Tests/          # Test project (XUnit v3 Universal Configuration)
│   ├── FibonacciCalculatorTests.cs
│   ├── FibonacciPerformanceTests.cs
│   ├── FibonacciMockingTests.cs
│   └── FibonacciDataDrivenTests.cs
├── FibonacciTemplate.sln     # Solution file
└── README.md
```

## 🛠️ Technologies Used

- **.NET 10** - Latest .NET framework
- **XUnit v3** - Modern testing framework
- **Microsoft Testing Platform** - Unified testing platform
- **NSubstitute** - Mocking framework
- **Shouldly** - Fluent assertion library
- **C# 12** - Latest C# language features

## 🚀 Quick Start

### Prerequisites

- .NET 10 SDK
- Visual Studio 2022 or VS Code

### Build and Run

```bash
# Restore packages
dotnet restore

# Build solution
dotnet build

# Run tests
dotnet test

# Run console application
dotnet run --project Fibonacci.Console
```

### Command Line Usage

```bash
# Interactive mode
dotnet run --project Fibonacci.Console

# With arguments
dotnet run --project Fibonacci.Console -- 10
```

## 🧪 Testing

This project demonstrates the **XUnit v3 Universal Configuration Pattern** with:

### Test Types

1. **Unit Tests** (`FibonacciCalculatorTests.cs`)
   - Basic functionality testing
   - Edge case validation
   - Exception handling

2. **Performance Tests** (`FibonacciPerformanceTests.cs`)
   - Performance comparisons
   - Scalability testing
   - Concurrent execution testing

3. **Mocking Tests** (`FibonacciMockingTests.cs`)
   - NSubstitute integration
   - Mock configuration
   - Complex scenario testing

4. **Data-Driven Tests** (`FibonacciDataDrivenTests.cs`)
   - Theory-based testing
   - Member data sources
   - Comprehensive test coverage

### Running Tests

```bash
# Run all tests
dotnet test

# Run specific test class
dotnet test --filter "ClassName=FibonacciCalculatorTests"

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run with detailed output
dotnet test --logger "console;verbosity=detailed"
```

## 📊 Fibonacci Calculator Features

### Core Functionality

- **Sequence Generation**: Calculate Fibonacci sequences up to 92 terms (long overflow limit)
- **Nth Term Calculation**: Both recursive and iterative approaches
- **Input Validation**: Comprehensive validation with meaningful error messages
- **Performance Optimization**: Iterative approach for better performance

### API Methods

```csharp
// Calculate sequence
IEnumerable<long> CalculateSequence(int terms)

// Calculate nth term (recursive)
long CalculateNth(int n)

// Calculate nth term (iterative)
long CalculateNthIterative(int n)

// Validation
bool IsValidTermCount(int terms)
int GetMaxSafeTermCount()
```

### Example Usage

```csharp
var calculator = new FibonacciCalculator();

// Calculate first 10 terms
var sequence = calculator.CalculateSequence(10);
// Result: [0, 1, 1, 2, 3, 5, 8, 13, 21, 34]

// Calculate 10th term
var tenthTerm = calculator.CalculateNth(10);
// Result: 55

// Validate input
var isValid = calculator.IsValidTermCount(50);
// Result: true
```

## 🔧 XUnit v3 Universal Configuration

This project uses the proven XUnit v3 Universal Configuration Pattern:

### Key Features

- **Microsoft Testing Platform**: Unified testing experience
- **Global Usings**: Clean test code with implicit usings
- **Comprehensive Package Set**: All necessary testing packages
- **Performance Testing**: Built-in performance testing capabilities
- **Code Coverage**: Integrated code coverage collection

### Configuration Benefits

- ✅ **Consistent Setup**: Same configuration across all test projects
- ✅ **Modern Tooling**: Latest testing frameworks and tools
- ✅ **Performance Focused**: Built-in performance testing capabilities
- ✅ **Comprehensive Coverage**: All testing scenarios covered
- ✅ **Future Proof**: Ready for .NET 10 and beyond

## 🎯 Use Cases

This template is perfect for:

- **Learning XUnit v3**: Understanding modern testing patterns
- **Template Projects**: Starting new projects with proven configuration
- **Testing Demonstrations**: Showcasing comprehensive testing strategies
- **Performance Testing**: Learning performance testing techniques
- **Clean Architecture**: Understanding separation of concerns

## 📈 Performance Characteristics

- **Iterative Approach**: O(n) time complexity, O(1) space complexity
- **Recursive Approach**: O(2^n) time complexity, O(n) space complexity
- **Max Safe Terms**: 92 terms (long overflow limit)
- **Concurrent Safe**: Thread-safe implementation

## 🔍 Code Quality

- **XML Documentation**: Comprehensive documentation for all public members
- **Input Validation**: Robust validation with meaningful error messages
- **Exception Handling**: Proper exception handling with specific exception types
- **Performance Considerations**: Optimized for both correctness and performance
- **Test Coverage**: Comprehensive test coverage for all scenarios

## 📚 Learning Resources

- [XUnit v3 Universal Configuration Pattern](../../docs/XUnit-v3-Universal-Configuration-Pattern.md)
- [Microsoft Testing Platform Documentation](https://learn.microsoft.com/en-us/dotnet/core/testing/)
- [NSubstitute Documentation](https://nsubstitute.github.io/)
- [Shouldly Documentation](https://shouldly.readthedocs.io/)

## 🤝 Contributing

This is a template project designed for learning and demonstration purposes. Feel free to:

- Use as a starting point for your own projects
- Modify and extend the functionality
- Add additional test scenarios
- Improve performance optimizations

## 📄 License

This project is part of the ExxerRules examples and follows the same licensing terms.

---

**Happy Testing with XUnit v3! 🎉**
