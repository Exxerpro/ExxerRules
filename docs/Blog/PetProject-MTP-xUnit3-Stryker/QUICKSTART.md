# Quick Start Guide: MTP 2.0 + xUnit v3 + Stryker.NET

## 🚀 Get Started in 5 Minutes

### Prerequisites
- .NET 10 Preview (latest)
- PowerShell (Windows) or Bash (Linux/macOS)
- Git

### 1. Clone and Setup
```bash
# Navigate to the pet project directory
cd docs/Audit/PetProject-MTP-xUnit3-Stryker

# Run setup script
./scripts/setup.ps1  # Windows
# or
./scripts/setup.sh   # Linux/macOS
```

### 2. Run Tests
```bash
# Run all tests
dotnet test

# Run tests with coverage
./scripts/run-tests.ps1
```

### 3. Run Mutation Testing
```bash
# Run Stryker.NET mutation testing
./scripts/run-stryker.ps1
```

### 4. Try the Calculator
```bash
# Run the console application
dotnet run --project src/Calculator.Console
```

## 🧪 Test the Calculator

### Basic Operations
```
calc> add 5 3
Result: 8

calc> mul 4 2.5
Result: 10

calc> div 15 3
Result: 5

calc> sqrt 16
Result: 4
```

### Advanced Features
```
calc> store 42
Stored 42 in memory

calc> recall
Memory: 42

calc> history
Calculation History:
  14:30:15 - 5 + 3 = 8
  14:30:20 - 4 * 2.5 = 10

calc> help
# Shows all available commands
```

## 📊 View Results

### Test Results
- **Coverage Report**: `results/coverage/index.html`
- **Test Results**: `results/test-results/test-results.trx`

### Mutation Testing Results
- **HTML Report**: `results/stryker/mutation-report.html`
- **JSON Report**: `results/stryker/mutation-report.json`

## 🔧 Configuration

### Stryker.NET Configuration
Edit `tools/stryker-config.json` to customize:
- Mutation thresholds
- Test projects
- Report formats
- Ignore patterns

### Test Configuration
Edit `src/Calculator.Tests/Calculator.Tests.csproj` to:
- Add new test dependencies
- Configure test settings
- Customize test execution

## 🐛 Troubleshooting

### Common Issues

#### Build Errors
```bash
# Clean and rebuild
dotnet clean
dotnet build
```

#### Test Failures
```bash
# Run tests with verbose output
dotnet test --verbosity detailed
```

#### Stryker.NET Issues
```bash
# Check Stryker.NET version
dotnet stryker --version

# Run with debug output
dotnet stryker --log-level debug
```

### Getting Help
- Check the logs in `results/` directory
- Review the documentation in `docs/`
- Check the GitHub issues for known problems

## 📈 Performance Tips

### Faster Test Execution
- Use `--parallel` flag for parallel test execution
- Configure test project for optimal performance
- Use test filtering for focused testing

### Faster Mutation Testing
- Adjust `max-concurrent-test-runners` in Stryker config
- Use `--timeout-ms` to set appropriate timeouts
- Filter mutations with `ignore-mutations` patterns

## 🎯 Next Steps

1. **Explore the Code**: Review the calculator implementation
2. **Add Tests**: Add new test cases to improve coverage
3. **Experiment**: Try different Stryker.NET configurations
4. **Document**: Record your findings in `docs/findings.md`

## 📚 Learn More

- [MTP 2.0 Documentation](https://devblogs.microsoft.com/dotnet/mtp-adoption-frameworks/)
- [xUnit v3 Documentation](https://xunit.net/docs/getting-started/v3/)
- [Stryker.NET Documentation](https://stryker-mutator.io/docs/stryker-net/)

---

**Happy Testing!** 🧪✨
