# Pet Project: MTP 2.0 + xUnit v3 + Stryker.NET Testing

**Purpose**: Test Microsoft Testing Platform 2.0, xUnit v3, and Stryker.NET integration before committing to main project  
**Date**: 2025-01-19  
**Status**: Experimental  

## 🎯 Project Goals

1. **Test MTP 2.0 stability** and feature completeness
2. **Validate xUnit v3** integration and new features
3. **Verify Stryker.NET** compatibility with new tooling
4. **Document findings** and recommendations for main project
5. **Create migration path** if tools prove stable

## 📁 Project Structure

```
PetProject-MTP-xUnit3-Stryker/
├── src/
│   ├── Calculator.Core/           # Business logic
│   ├── Calculator.Console/        # Console application
│   └── Calculator.Tests/          # xUnit v3 tests with MTP 2.0
├── tools/
│   ├── stryker-config.json       # Stryker.NET configuration
│   └── test-config.json          # MTP 2.0 configuration
├── results/
│   ├── stryker/                  # Mutation testing results
│   └── coverage/                 # Code coverage results
└── docs/
    ├── findings.md               # Test findings and issues
    └── recommendations.md        # Migration recommendations
```

## 🧮 Calculator Features

### Core Operations
- **Basic Arithmetic**: Add, Subtract, Multiply, Divide
- **Advanced Operations**: Power, Square Root, Percentage
- **Memory Functions**: Store, Recall, Clear
- **History**: Track calculation history
- **Error Handling**: Division by zero, invalid inputs

### Business Logic Complexity
- **Input Validation**: Number parsing and validation
- **Precision Handling**: Decimal precision management
- **Error States**: Comprehensive error handling
- **State Management**: Calculator state persistence

## 🧪 Testing Strategy

### Unit Tests (xUnit v3)
- **Operation Tests**: All mathematical operations
- **Edge Cases**: Boundary conditions and error scenarios
- **State Tests**: Memory and history functionality
- **Integration Tests**: End-to-end calculation flows

### Mutation Testing (Stryker.NET)
- **Mutation Coverage**: Test effectiveness measurement
- **Quality Gates**: Minimum mutation score thresholds
- **Performance**: Mutation testing execution time
- **Integration**: CI/CD pipeline integration

## 🚀 Getting Started

### Prerequisites
- .NET 10 Preview (latest)
- Visual Studio 2022 17.10+ (or VS Code)
- Git

### Setup Commands
```bash
# Clone and navigate
cd PetProject-MTP-xUnit3-Stryker

# Restore tools
dotnet tool restore

# Build project
dotnet build

# Run tests
dotnet test

# Run mutation testing
dotnet stryker
```

## 📊 Success Criteria

### MTP 2.0 Evaluation
- [ ] **Stability**: No crashes or instability issues
- [ ] **Performance**: Test execution time acceptable
- [ ] **Features**: All expected features working
- [ ] **Integration**: Seamless xUnit v3 integration

### xUnit v3 Evaluation
- [ ] **New Features**: Utilize new xUnit v3 capabilities
- [ ] **Compatibility**: Works with existing test patterns
- [ ] **Performance**: Improved test execution speed
- [ ] **Developer Experience**: Better debugging and reporting

### Stryker.NET Evaluation
- [ ] **Compatibility**: Works with MTP 2.0 and xUnit v3
- [ ] **Performance**: Acceptable mutation testing time
- [ ] **Accuracy**: Correctly identifies weak tests
- [ ] **Integration**: Fits into development workflow

## 🚨 Risk Mitigation

### Fallback Strategy
- **Current Setup**: Keep main project on stable tooling
- **Gradual Migration**: Only migrate if pet project proves stable
- **Rollback Plan**: Document rollback procedures
- **Timeline**: Complete evaluation before November ship date

### Testing Approach
- **Parallel Development**: Pet project alongside main development
- **Feature Parity**: Test equivalent complexity to main project
- **Real Scenarios**: Use realistic test cases and scenarios
- **Performance Baseline**: Establish performance benchmarks

## 📈 Expected Outcomes

### Positive Outcomes
- **Tool Stability**: MTP 2.0 and xUnit v3 prove stable
- **Quality Improvement**: Stryker.NET identifies test gaps
- **Performance Gains**: Faster test execution
- **Developer Experience**: Better tooling and debugging

### Risk Scenarios
- **Tool Instability**: MTP 2.0 or xUnit v3 has issues
- **Integration Problems**: Stryker.NET compatibility issues
- **Performance Regression**: Slower test execution
- **Migration Complexity**: Difficult to migrate main project

## 🎯 Decision Points

### Go/No-Go Criteria
- **Stability**: No critical issues or crashes
- **Performance**: Within acceptable limits
- **Feature Completeness**: All required features working
- **Integration**: Seamless tool integration

### Timeline
- **Week 1**: Setup and basic functionality testing
- **Week 2**: Advanced features and edge cases
- **Week 3**: Performance and stability testing
- **Week 4**: Documentation and recommendations

## 📝 Documentation

All findings, issues, and recommendations will be documented in:
- `docs/findings.md` - Detailed test results and issues
- `docs/recommendations.md` - Migration recommendations
- `results/` - Test results and reports

## 🔗 Related Resources

- [Microsoft Testing Platform 2.0](https://devblogs.microsoft.com/dotnet/mtp-adoption-frameworks/)
- [xUnit v3 Documentation](https://xunit.net/docs/getting-started/v3/)
- [Stryker.NET Documentation](https://stryker-mutator.io/docs/stryker-net/)
- [.NET 10 Preview](https://devblogs.microsoft.com/dotnet/announcing-dotnet-10-preview-1/)
