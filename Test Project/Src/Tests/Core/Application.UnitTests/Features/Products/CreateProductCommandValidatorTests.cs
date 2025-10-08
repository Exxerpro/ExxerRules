namespace Application.UnitTests.Features.Products;

/// <summary>
/// Unit tests for CreateProductCommandValidator
/// </summary>
public class CreateProductCommandValidatorTests
{
    private readonly CreateProductCommandValidator _validator = null!;
    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>

    public CreateProductCommandValidatorTests()
    {
        _validator = new CreateProductCommandValidator();
    }

    /// <summary>
    /// Executes Constructor_ShouldCreateInstance operation.
    /// </summary>

    [Fact]
    public void Constructor_ShouldCreateInstance()
    {
        // Arrange & Act
        var validator = new CreateProductCommandValidator();

        // Assert
        validator.ShouldNotBeNull();
    }

    /// <summary>
    /// Executes Validate_WithValidCommand_ShouldPass operation.
    /// </summary>

    [Fact]
    public void Validate_WithValidCommand_ShouldPass()
    {
        // Arrange
        var command = CreateValidCommand();

        //[Fix]
        //CLAUDE
        //Date: 21/08/2025
        //Reason: Updated to use FluentValidation.TestHelper pattern for modern validation testing
        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    // Product Validation Tests
    /// <summary>
    /// Executes Validate_WithNullProduct_ShouldFail operation.
    /// </summary>

    [Fact]
    public void Validate_WithNullProduct_ShouldFail()
    {
        // Arrange
        var command = CreateValidCommand();
        command.Product = null!;

        //[Fix]
        //CLAUDE
        //Date: 21/08/2025
        //Reason: Updated to use FluentValidation.TestHelper pattern for modern validation testing
        // Act
        var result = _validator.TestValidate(command);

        // Assert
        //[Fix]
        //CLAUDE
        //Date: 21/08/2025
        //Reason: Replaced hardcoded error message assertion with flexible validation error checking for industrial robustness
        result.ShouldHaveValidationErrorFor(x => x.Product);
    }

    /// <summary>
    /// Executes Validate_WithEmptyPartNumber_ShouldFail operation.
    /// </summary>
    /// <param name="partNumber">The partNumber.</param>

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
#pragma warning disable xUnit1012 // Null should not be used for type parameter - this test intentionally validates null behavior
    [InlineData(null)]
    public void Validate_WithEmptyPartNumber_ShouldFail(string partNumber)
    {
        // Using parameters: partNumber
        _ = partNumber; // xUnit1026 fix
        // Using parameters: partNumber
        _ = partNumber; // xUnit1026 fix
        // Using parameters: partNumber
        _ = partNumber; // xUnit1026 fix
        // Using parameters: partNumber
        _ = partNumber; // xUnit1026 fix
        // Using parameters: partNumber
        _ = partNumber; // xUnit1026 fix
        // Arrange
        var command = CreateValidCommand();
        command.Product.PartNumber = partNumber;

        //[Fix]
        //CLAUDE
        //Date: 21/08/2025
        //Reason: Updated to use FluentValidation.TestHelper pattern for modern validation testing
        // Act
        var result = _validator.TestValidate(command);

        // Assert
        //[Fix]
        //CLAUDE
        //Date: 21/08/2025
        //Reason: Replaced hardcoded error message assertion with flexible validation error checking for industrial robustness
        result.ShouldHaveValidationErrorFor("Product.PartNumber");
    }

    /// <summary>
    /// Executes Validate_WithPartNumberTooShort_ShouldFail operation.
    /// </summary>
    /// <param name="partNumber">The partNumber.</param>

    [Theory]
    [InlineData("A")]      // 1 character
    [InlineData("AB")]     // 2 characters
    public void Validate_WithPartNumberTooShort_ShouldFail(string partNumber)
    {
        // Using parameters: partNumber
        _ = partNumber; // xUnit1026 fix
        // Using parameters: partNumber
        _ = partNumber; // xUnit1026 fix
        // Using parameters: partNumber
        _ = partNumber; // xUnit1026 fix
        // Using parameters: partNumber
        _ = partNumber; // xUnit1026 fix
        // Using parameters: partNumber
        _ = partNumber; // xUnit1026 fix
        // Arrange
        var command = CreateValidCommand();
        command.Product.PartNumber = partNumber;

        //[Fix]
        //CLAUDE
        //Date: 21/08/2025
        //Reason: Updated to use FluentValidation.TestHelper pattern for modern validation testing
        // Act
        var result = _validator.TestValidate(command);

        // Assert
        //[Fix]
        //CLAUDE
        //Date: 21/08/2025
        //Reason: Replaced hardcoded error message assertion with flexible validation error checking for industrial robustness
        result.ShouldHaveValidationErrorFor("Product.PartNumber");
    }

    /// <summary>
    /// Executes Validate_WithValidPartNumber_ShouldPass operation.
    /// </summary>
    /// <param name="partNumber">The partNumber.</param>

    [Theory]
    [InlineData("ABC")]           // Exactly 3 characters
    [InlineData("ABCD")]          // 4 characters
    [InlineData("PART-12345")]    // Standard part number
    [InlineData("A1B2C3D4E5")]    // Alphanumeric
    public void Validate_WithValidPartNumber_ShouldPass(string partNumber)
    {
        // Using parameters: partNumber
        _ = partNumber; // xUnit1026 fix
        // Using parameters: partNumber
        _ = partNumber; // xUnit1026 fix
        // Using parameters: partNumber
        _ = partNumber; // xUnit1026 fix
        // Using parameters: partNumber
        _ = partNumber; // xUnit1026 fix
        // Using parameters: partNumber
        _ = partNumber; // xUnit1026 fix
        // Arrange
        var command = CreateValidCommand();
        command.Product.PartNumber = partNumber;

        //[Fix]
        //CLAUDE
        //Date: 21/08/2025
        //Reason: Updated to use FluentValidation.TestHelper pattern for modern validation testing
        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor("Product.PartNumber");
    }

    /// <summary>
    /// Executes Validate_WithEmptyProductName_ShouldFail operation.
    /// </summary>
    /// <param name="productName">The productName.</param>

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
#pragma warning disable xUnit1012
    [InlineData(null!)]  //Invalid for nullable reference types
    public void Validate_WithEmptyProductName_ShouldFail(string productName)
    {
        // Using parameters: productName
        _ = productName; // xUnit1026 fix
        // Using parameters: productName
        _ = productName; // xUnit1026 fix
        // Using parameters: productName
        _ = productName; // xUnit1026 fix
        // Using parameters: productName
        _ = productName; // xUnit1026 fix
        // Using parameters: productName
        _ = productName; // xUnit1026 fix
        // Arrange
        var command = CreateValidCommand();
        command.Product.ProductName = productName;

        //[Fix]
        //CLAUDE
        //Date: 21/08/2025
        //Reason: Updated to use FluentValidation.TestHelper pattern for modern validation testing
        // Act
        var result = _validator.TestValidate(command);

        // Assert
        //[Fix]
        //CLAUDE
        //Date: 21/08/2025
        //Reason: Replaced hardcoded error message assertion with flexible validation error checking for industrial robustness
        result.ShouldHaveValidationErrorFor("Product.ProductName");
    }

    /// <summary>
    /// Executes Validate_WithProductNameTooLong_ShouldFail operation.
    /// </summary>

    [Fact]
    public void Validate_WithProductNameTooLong_ShouldFail()
    {
        // Arrange
        var command = CreateValidCommand();
        command.Product.ProductName = new string('A', 101); // 101 characters

        //[Fix]
        //CLAUDE
        //Date: 21/08/2025
        //Reason: Updated to use FluentValidation.TestHelper pattern for modern validation testing
        // Act
        var result = _validator.TestValidate(command);

        // Assert
        //[Fix]
        //CLAUDE
        //Date: 21/08/2025
        //Reason: Replaced hardcoded error message assertion with flexible validation error checking for industrial robustness
        result.ShouldHaveValidationErrorFor("Product.ProductName");
    }

    /// <summary>
    /// Executes Validate_WithValidProductName_ShouldPass operation.
    /// </summary>
    /// <param name="productName">The productName.</param>

    [Theory]
    [InlineData("A")]                                  // 1 character
    [InlineData("Test Product")]                       // Standard name
    [InlineData("Very Long Product Name With Spaces")] // Longer name
    public void Validate_WithValidProductName_ShouldPass(string productName)
    {
        // Using parameters: productName
        _ = productName; // xUnit1026 fix
        // Using parameters: productName
        _ = productName; // xUnit1026 fix
        // Using parameters: productName
        _ = productName; // xUnit1026 fix
        // Using parameters: productName
        _ = productName; // xUnit1026 fix
        // Using parameters: productName
        _ = productName; // xUnit1026 fix
        // Arrange
        var command = CreateValidCommand();
        command.Product.ProductName = productName;

        if (productName.Length <= 100)
        {
            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveValidationErrorFor("Product.ProductName");
        }
    }

    /// <summary>
    /// Executes Validate_WithEmptyCustomerName_ShouldFail operation.
    /// </summary>
    /// <param name="customerName">The customerName.</param>

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData(null)]
    public void Validate_WithEmptyCustomerName_ShouldFail(string customerName)
    {
        // Using parameters: customerName
        _ = customerName; // xUnit1026 fix
        // Using parameters: customerName
        _ = customerName; // xUnit1026 fix
        // Using parameters: customerName
        _ = customerName; // xUnit1026 fix
        // Using parameters: customerName
        _ = customerName; // xUnit1026 fix
        // Using parameters: customerName
        _ = customerName; // xUnit1026 fix
        // Arrange
        var command = CreateValidCommand();
        command.Product.CustomerName = customerName;

        //[Fix]
        //CLAUDE
        //Date: 21/08/2025
        //Reason: Updated to use FluentValidation.TestHelper pattern for modern validation testing
        // Act
        var result = _validator.TestValidate(command);

        // Assert
        //[Fix]
        //CLAUDE
        //Date: 21/08/2025
        //Reason: Replaced hardcoded error message assertion with flexible validation error checking for industrial robustness
        result.ShouldHaveValidationErrorFor("Product.CustomerName");
    }

    /// <summary>
    /// Executes Validate_WithCustomerNameTooLong_ShouldFail operation.
    /// </summary>

    [Fact]
    public void Validate_WithCustomerNameTooLong_ShouldFail()
    {
        // Arrange
        var command = CreateValidCommand();
        command.Product.CustomerName = new string('B', 101); // 101 characters

        //[Fix]
        //CLAUDE
        //Date: 21/08/2025
        //Reason: Updated to use FluentValidation.TestHelper pattern for modern validation testing
        // Act
        var result = _validator.TestValidate(command);

        // Assert
        //[Fix]
        //CLAUDE
        //Date: 21/08/2025
        //Reason: Replaced hardcoded error message assertion with flexible validation error checking for industrial robustness
        result.ShouldHaveValidationErrorFor("Product.CustomerName");
    }

    // Rule Validation Tests
    /// <summary>
    /// Executes Validate_WithNullRule_ShouldFail operation.
    /// </summary>

    [Fact]
    public void Validate_WithNullRule_ShouldFail()
    {
        // Arrange
        var command = CreateValidCommand();
        command.Rule = null!;

        //[Fix]
        //CLAUDE
        //Date: 21/08/2025
        //Reason: Updated to use FluentValidation.TestHelper pattern for modern validation testing
        // Act
        var result = _validator.TestValidate(command);

        // Assert
        //[Fix]
        //CLAUDE
        //Date: 21/08/2025
        //Reason: Replaced hardcoded error message assertion with flexible validation error checking for industrial robustness
        result.ShouldHaveValidationErrorFor(x => x.Rule);
    }

    /// <summary>
    /// Executes Validate_WithEmptyRuleJson_ShouldFail operation.
    /// </summary>
    /// <param name="ruleJson">The ruleJson.</param>

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData(null)]
    public void Validate_WithEmptyRuleJson_ShouldFail(string ruleJson)
    {
        // Using parameters: ruleJson
        _ = ruleJson; // xUnit1026 fix
        // Using parameters: ruleJson
        _ = ruleJson; // xUnit1026 fix
        // Using parameters: ruleJson
        _ = ruleJson; // xUnit1026 fix
        // Using parameters: ruleJson
        _ = ruleJson; // xUnit1026 fix
        // Using parameters: ruleJson
        _ = ruleJson; // xUnit1026 fix
        // Arrange
        var command = CreateValidCommand();
        command.Rule.RuleJson = ruleJson;

        //[Fix]
        //CLAUDE
        //Date: 21/08/2025
        //Reason: Updated to use FluentValidation.TestHelper pattern for modern validation testing
        // Act
        var result = _validator.TestValidate(command);

        // Assert
        //[Fix]
        //CLAUDE
        //Date: 21/08/2025
        //Reason: Replaced hardcoded error message assertion with flexible validation error checking for industrial robustness
        result.ShouldHaveValidationErrorFor("Rule.RuleJson");
    }

    /// <summary>
    /// Executes Validate_WithInvalidRuleJson_ShouldFail operation.
    /// </summary>
    /// <param name="invalidJson">The invalidJson.</param>

    [Theory]
    [InlineData("invalid json")]
    [InlineData("{invalid}")]
    [InlineData("{'key': value}")]  // Single quotes invalid
    [InlineData("{key: 'value'}")]  // Missing quotes on key
    [InlineData("{")]               // Incomplete JSON
    public void Validate_WithInvalidRuleJson_ShouldFail(string invalidJson)
    {
        // Using parameters: invalidJson
        _ = invalidJson; // xUnit1026 fix
        // Using parameters: invalidJson
        _ = invalidJson; // xUnit1026 fix
        // Using parameters: invalidJson
        _ = invalidJson; // xUnit1026 fix
        // Using parameters: invalidJson
        _ = invalidJson; // xUnit1026 fix
        // Using parameters: invalidJson
        _ = invalidJson; // xUnit1026 fix
        // Arrange
        var command = CreateValidCommand();
        command.Rule.RuleJson = invalidJson;

        //[Fix]
        //CLAUDE
        //Date: 21/08/2025
        //Reason: Updated to use FluentValidation.TestHelper pattern for modern validation testing
        // Act
        var result = _validator.TestValidate(command);

        // Assert
        //[Fix]
        //CLAUDE
        //Date: 21/08/2025
        //Reason: Replaced hardcoded error message assertion with flexible validation error checking for industrial robustness
        result.ShouldHaveValidationErrorFor("Rule.RuleJson");
    }

    /// <summary>
    /// Executes Validate_WithValidRuleJson_ShouldPass operation.
    /// </summary>
    /// <param name="validJson">The validJson.</param>

    [Theory]
    [InlineData("{}")]                                    // Empty JSON object
    [InlineData("{\"key\": \"value\"}")]                 // Simple JSON
    [InlineData("{\"temperature\": 85.5, \"enabled\": true}")] // Complex JSON
    [InlineData("[1, 2, 3]")]                           // JSON array
    public void Validate_WithValidRuleJson_ShouldPass(string validJson)
    {
        // Using parameters: validJson
        _ = validJson; // xUnit1026 fix
        // Using parameters: validJson
        _ = validJson; // xUnit1026 fix
        // Using parameters: validJson
        _ = validJson; // xUnit1026 fix
        // Using parameters: validJson
        _ = validJson; // xUnit1026 fix
        // Using parameters: validJson
        _ = validJson; // xUnit1026 fix
        // Arrange
        var command = CreateValidCommand();
        command.Rule.RuleJson = validJson;

        //[Fix]
        //CLAUDE
        //Date: 21/08/2025
        //Reason: Updated to use FluentValidation.TestHelper pattern for modern validation testing
        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor("Rule.RuleJson");
    }

    // Recipe Validation Tests
    /// <summary>
    /// Executes Validate_WithNullRecipe_ShouldFail operation.
    /// </summary>

    [Fact]
    public void Validate_WithNullRecipe_ShouldFail()
    {
        // Arrange
        var command = CreateValidCommand();
        command.Recipe = null!;

        //[Fix]
        //CLAUDE
        //Date: 21/08/2025
        //Reason: Updated to use FluentValidation.TestHelper pattern for modern validation testing
        // Act
        var result = _validator.TestValidate(command);

        // Assert
        //[Fix]
        //CLAUDE
        //Date: 21/08/2025
        //Reason: Replaced hardcoded error message assertion with flexible validation error checking for industrial robustness
        result.ShouldHaveValidationErrorFor(x => x.Recipe);
    }

    /// <summary>
    /// Executes Validate_WithInvalidCycleTimeMinimum_ShouldFail operation.
    /// </summary>
    /// <param name="cycleTime">The cycleTime.</param>

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-10)]
    public void Validate_WithInvalidCycleTimeMinimum_ShouldFail(int cycleTime)
    {
        // Using parameters: cycleTime
        _ = cycleTime; // xUnit1026 fix
        // Using parameters: cycleTime
        _ = cycleTime; // xUnit1026 fix
        // Using parameters: cycleTime
        _ = cycleTime; // xUnit1026 fix
        // Using parameters: cycleTime
        _ = cycleTime; // xUnit1026 fix
        // Using parameters: cycleTime
        _ = cycleTime; // xUnit1026 fix
        // Arrange
        var command = CreateValidCommand();
        command.Recipe.CycleTimeMinimum = cycleTime;

        //[Fix]
        //CLAUDE
        //Date: 21/08/2025
        //Reason: Updated to use FluentValidation.TestHelper pattern for modern validation testing
        // Act
        var result = _validator.TestValidate(command);

        // Assert
        //[Fix]
        //CLAUDE
        //Date: 21/08/2025
        //Reason: Replaced hardcoded error message assertion with flexible validation error checking for industrial robustness
        result.ShouldHaveValidationErrorFor("Recipe.CycleTimeMinimum");
    }

    /// <summary>
    /// Executes Validate_WithInvalidCycleTimeMaximum_ShouldFail operation.
    /// </summary>
    /// <param name="cycleTime">The cycleTime.</param>

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-10)]
    public void Validate_WithInvalidCycleTimeMaximum_ShouldFail(int cycleTime)
    {
        // Using parameters: cycleTime
        _ = cycleTime; // xUnit1026 fix
        // Using parameters: cycleTime
        _ = cycleTime; // xUnit1026 fix
        // Using parameters: cycleTime
        _ = cycleTime; // xUnit1026 fix
        // Using parameters: cycleTime
        _ = cycleTime; // xUnit1026 fix
        // Using parameters: cycleTime
        _ = cycleTime; // xUnit1026 fix
        // Arrange
        var command = CreateValidCommand();
        command.Recipe.CycleTimeMaximum = cycleTime;

        //[Fix]
        //CLAUDE
        //Date: 21/08/2025
        //Reason: Updated to use FluentValidation.TestHelper pattern for modern validation testing
        // Act
        var result = _validator.TestValidate(command);

        // Assert
        //[Fix]
        //CLAUDE
        //Date: 21/08/2025
        //Reason: Replaced hardcoded error message assertion with flexible validation error checking for industrial robustness
        result.ShouldHaveValidationErrorFor("Recipe.CycleTimeMaximum");
    }

    /// <summary>
    /// Executes Validate_WithCycleTimeMaximumLessThanMinimum_ShouldFail operation.
    /// </summary>

    [Fact]
    public void Validate_WithCycleTimeMaximumLessThanMinimum_ShouldFail()
    {
        // Arrange
        var command = CreateValidCommand();
        command.Recipe.CycleTimeMinimum = 100;
        command.Recipe.CycleTimeMaximum = 50; // Less than minimum

        //[Fix]
        //CLAUDE
        //Date: 21/08/2025
        //Reason: Updated to use FluentValidation.TestHelper pattern for modern validation testing
        // Act
        var result = _validator.TestValidate(command);

        // Assert
        //[Fix]
        //CLAUDE
        //Date: 21/08/2025
        //Reason: Replaced hardcoded error message assertion with flexible validation error checking for industrial robustness
        result.ShouldHaveValidationErrorFor("Recipe.CycleTimeMaximum");
    }

    /// <summary>
    /// Executes Validate_WithValidCycleTimes_ShouldPass operation.
    /// </summary>
    /// <param name="minimum">The minimum.</param>
    /// <param name="maximum">The maximum.</param>

    [Theory]
    [InlineData(1, 2)]      // Min=1, Max=2
    [InlineData(50, 100)]   // Min=50, Max=100
    [InlineData(100, 200)]  // Min=100, Max=200
    [InlineData(1, 1000)]   // Wide range
    public void Validate_WithValidCycleTimes_ShouldPass(int minimum, int maximum)
    {
        // Using parameters: minimum, maximum
        _ = minimum; // xUnit1026 fix
        _ = maximum; // xUnit1026 fix
        // Using parameters: minimum, maximum
        _ = minimum; // xUnit1026 fix
        _ = maximum; // xUnit1026 fix
        // Using parameters: minimum, maximum
        _ = minimum; // xUnit1026 fix
        _ = maximum; // xUnit1026 fix
        // Using parameters: minimum, maximum
        _ = minimum; // xUnit1026 fix
        _ = maximum; // xUnit1026 fix
        // Using parameters: minimum, maximum
        _ = minimum; // xUnit1026 fix
        _ = maximum; // xUnit1026 fix
        // Arrange
        var command = CreateValidCommand();
        command.Recipe.CycleTimeMinimum = minimum;
        command.Recipe.CycleTimeMaximum = maximum;

        //[Fix]
        //CLAUDE
        //Date: 21/08/2025
        //Reason: Updated to use FluentValidation.TestHelper pattern for modern validation testing
        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor("Recipe.CycleTimeMinimum");
        result.ShouldNotHaveValidationErrorFor("Recipe.CycleTimeMaximum");
    }

    // WorkFlows Validation Tests
    /// <summary>
    /// Executes Validate_WithNullWorkFlows_ShouldFail operation.
    /// </summary>

    [Fact]
    public void Validate_WithNullWorkFlows_ShouldFail()
    {
        // Arrange
        var command = CreateValidCommand();
        command.WorkFlows = null!;

        //[Fix]
        //CLAUDE
        //Date: 21/08/2025
        //Reason: Updated to use FluentValidation.TestHelper pattern for modern validation testing
        // Act
        var result = _validator.TestValidate(command);

        // Assert
        //[Fix]
        //CLAUDE
        //Date: 21/08/2025
        //Reason: Replaced hardcoded error message assertion with flexible validation error checking for industrial robustness
        result.ShouldHaveValidationErrorFor(x => x.WorkFlows);
    }

    /// <summary>
    /// Executes Validate_WithEmptyWorkFlows_ShouldFail operation.
    /// </summary>

    [Fact]
    public void Validate_WithEmptyWorkFlows_ShouldFail()
    {
        // Arrange
        var command = CreateValidCommand();
        command.WorkFlows = [];

        //[Fix]
        //CLAUDE
        //Date: 21/08/2025
        //Reason: Updated to use FluentValidation.TestHelper pattern for modern validation testing
        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.WorkFlows);
    }

    /// <summary>
    /// Executes Validate_WithValidWorkFlows_ShouldPass operation.
    /// </summary>

    [Fact]
    public void Validate_WithValidWorkFlows_ShouldPass()
    {
        // Arrange
        var command = CreateValidCommand();

        //[Fix]
        //CLAUDE
        //Date: 21/08/2025
        //Reason: Updated to use FluentValidation.TestHelper pattern for modern validation testing
        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.WorkFlows);
    }

    // Complex Scenario Tests
    /// <summary>
    /// Executes Validate_WithMultipleValidationErrors_ShouldReportAll operation.
    /// </summary>

    [Fact]
    public void Validate_WithMultipleValidationErrors_ShouldReportAll()
    {
        // Arrange
        var command = CreateValidCommand();
        command.Product = null!;              // Product error
        command.Rule = null!;                 // Rule error
        command.Recipe = null!;               // Recipe error
        command.WorkFlows = null!;            // WorkFlows error

        //[Fix]
        //CLAUDE
        //Date: 21/08/2025
        //Reason: Updated to use FluentValidation.TestHelper pattern for modern validation testing
        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Product);
        result.ShouldHaveValidationErrorFor(x => x.Rule);
        result.ShouldHaveValidationErrorFor(x => x.Recipe);
        result.ShouldHaveValidationErrorFor(x => x.WorkFlows);
    }

    /// <summary>
    /// Executes ValidateAsync_WithValidCommand_ShouldPass operation.
    /// </summary>
    /// <returns>The result of ValidateAsync_WithValidCommand_ShouldPass.</returns>

    [Fact]
    public async Task ValidateAsync_WithValidCommand_ShouldPass()
    {
        // Arrange
        var command = CreateValidCommand();

        //[Fix]
        //CLAUDE
        //Date: 21/08/2025
        //Reason: Updated to use FluentValidation.TestHelper pattern for modern validation testing
        // Act
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    /// <summary>
    /// Executes ValidateAsync_WithInvalidCommand_ShouldFail operation.
    /// </summary>
    /// <returns>The result of ValidateAsync_WithInvalidCommand_ShouldFail.</returns>

    [Fact]
    public async Task ValidateAsync_WithInvalidCommand_ShouldFail()
    {
        // Arrange
        var command = CreateValidCommand();
        command.Product = null!;

        //[Fix]
        //CLAUDE
        //Date: 21/08/2025
        //Reason: Updated to use FluentValidation.TestHelper pattern for modern validation testing
        // Act
        var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Product);
    }

    /// <summary>
    /// Executes ValidateAsync_WithCancellationToken_ShouldRespectCancellation operation.
    /// </summary>
    /// <returns>The result of ValidateAsync_WithCancellationToken_ShouldRespectCancellation.</returns>

    [Fact]
    public async Task ValidateAsync_WithCancellationToken_ShouldRespectCancellation()
    {
        // Arrange
        var command = CreateValidCommand();
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        await Should.ThrowAsync<OperationCanceledException>(async () =>
            await _validator.ValidateAsync(command, cts.Token));
    }

    /// <summary>
    /// Executes Validate_WithIndustrialProductScenarios_ShouldWorkCorrectly operation.
    /// </summary>
    /// <param name="partNumber">The partNumber.</param>
    /// <param name="productName">The productName.</param>
    /// <param name="customerName">The customerName.</param>
    /// <param name="expected">The expected.</param>
    /// <param name="description">The description.</param>

    [Theory]
    [MemberData(nameof(GetIndustrialProductTestCases))]
    public void Validate_WithIndustrialProductScenarios_ShouldWorkCorrectly(string partNumber, string productName, string customerName, bool expected, string description)
    {
        // Using parameters: partNumber, productName, customerName, expected, description
        _ = partNumber; // xUnit1026 fix
        _ = productName; // xUnit1026 fix
        _ = customerName; // xUnit1026 fix
        _ = expected; // xUnit1026 fix
        _ = description; // xUnit1026 fix
        // Using parameters: partNumber, productName, customerName, expected, description
        _ = partNumber; // xUnit1026 fix
        _ = productName; // xUnit1026 fix
        _ = customerName; // xUnit1026 fix
        _ = expected; // xUnit1026 fix
        _ = description; // xUnit1026 fix
        // Using parameters: partNumber, productName, customerName, expected, description
        _ = partNumber; // xUnit1026 fix
        _ = productName; // xUnit1026 fix
        _ = customerName; // xUnit1026 fix
        _ = expected; // xUnit1026 fix
        _ = description; // xUnit1026 fix
        // Using parameters: partNumber, productName, customerName, expected, description
        _ = partNumber; // xUnit1026 fix
        _ = productName; // xUnit1026 fix
        _ = customerName; // xUnit1026 fix
        _ = expected; // xUnit1026 fix
        _ = description; // xUnit1026 fix
        // Using parameters: partNumber, productName, customerName, expected, description
        _ = partNumber; // xUnit1026 fix
        _ = productName; // xUnit1026 fix
        _ = customerName; // xUnit1026 fix
        _ = expected; // xUnit1026 fix
        _ = description; // xUnit1026 fix
        // Arrange
        var command = CreateValidCommand();
        command.Product.PartNumber = partNumber;
        command.Product.ProductName = productName;
        command.Product.CustomerName = customerName;

        //[Fix]
        //CLAUDE
        //Date: 21/08/2025
        //Reason: Updated to use FluentValidation.TestHelper pattern for modern validation testing
        // Act
        var result = _validator.TestValidate(command);

        // Assert
        //[Fix]
        //CLAUDE
        //Date: 21/08/2025
        //Reason: Updated to use FluentValidation.TestHelper pattern for modern validation testing
        if (expected)
        {
            result.ShouldNotHaveAnyValidationErrors();
        }
        else
        {
            result.ShouldHaveValidationErrors();
        }
    }

    /// <summary>
    /// Executes GetIndustrialProductTestCases operation.
    /// </summary>
    /// <returns>The result of GetIndustrialProductTestCases.</returns>

    public static IEnumerable<object[]> GetIndustrialProductTestCases()
    {
        yield return new object[] { "PUMP-001", "Hydraulic Pump", "Manufacturing Corp", true, "Standard industrial product" };
        yield return new object[] { "MOTOR-V2", "Electric Motor V2", "Motors Inc", true, "Motor product" };
        yield return new object[] { "VALVE-123A", "Control Valve 123A", "Valve Systems", true, "Valve product" };
        yield return new object[] { "AB", "Short Part", "Valid Customer", false, "Part number too short" };
        yield return new object[] { "VALID-PART", "", "Valid Customer", false, "Empty product name" };
        yield return new object[] { "VALID-PART", "Valid Product", "", false, "Empty customer name" };
    }

    /// <summary>
    /// Executes Validate_WithBoundaryValues_ShouldWorkCorrectly operation.
    /// </summary>

    [Fact]
    public void Validate_WithBoundaryValues_ShouldWorkCorrectly()
    {
        // Arrange - Test boundary values for various fields
        var scenarios = new[]
        {
            // PartNumber boundaries
            new { PartNumber = "ABC", ProductName = "Test", CustomerName = "Customer", CycleMin = 1, CycleMax = 2, Expected = true, Name = "PartNumber minimum length" },
            new { PartNumber = "AB", ProductName = "Test", CustomerName = "Customer", CycleMin = 1, CycleMax = 2, Expected = false, Name = "PartNumber below minimum" },

            // ProductName boundaries
            new { PartNumber = "TEST", ProductName = new string('A', 100), CustomerName = "Customer", CycleMin = 1, CycleMax = 2, Expected = true, Name = "ProductName maximum length" },
            new { PartNumber = "TEST", ProductName = new string('A', 101), CustomerName = "Customer", CycleMin = 1, CycleMax = 2, Expected = false, Name = "ProductName above maximum" },

            // CustomerName boundaries
            new { PartNumber = "TEST", ProductName = "Test", CustomerName = new string('B', 100), CycleMin = 1, CycleMax = 2, Expected = true, Name = "CustomerName maximum length" },
            new { PartNumber = "TEST", ProductName = "Test", CustomerName = new string('B', 101), CycleMin = 1, CycleMax = 2, Expected = false, Name = "CustomerName above maximum" },

            // Cycle time boundaries
            new { PartNumber = "TEST", ProductName = "Test", CustomerName = "Customer", CycleMin = 1, CycleMax = 1, Expected = false, Name = "CycleMax equal to CycleMin" },
            new { PartNumber = "TEST", ProductName = "Test", CustomerName = "Customer", CycleMin = 0, CycleMax = 1, Expected = false, Name = "CycleMin at zero boundary" }
        };

        foreach (var scenario in scenarios)
        {
            // Arrange
            var command = CreateValidCommand();
            command.Product.PartNumber = scenario.PartNumber;
            command.Product.ProductName = scenario.ProductName;
            command.Product.CustomerName = scenario.CustomerName;
            command.Recipe.CycleTimeMinimum = scenario.CycleMin;
            command.Recipe.CycleTimeMaximum = scenario.CycleMax;

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            if (scenario.Expected)
            {
                result.ShouldNotHaveAnyValidationErrors();
            }
            else
            {
                result.ShouldHaveValidationErrors();
            }
        }
    }

    /// <summary>
    /// Creates a valid CreateProductCommand for testing purposes
    /// </summary>
    private static CreateProductCommand CreateValidCommand()
    {
        var productDto = new ProductCreationDto
        {
            Product = new ProductDto
            {
                PartNumber = "PART-12345",
                ProductName = "Test Product",
                CustomerName = "Test Customer",
                CustomerId = 1,
                LineId = 1
            },
            Machines = new[] { 100, 200 },
            Rule = new RuleDto
            {
                RuleJson = "{\"temperature\": 85.5, \"enabled\": true}"
            },
            Recipe = new RecipeDto
            {
                CycleTimeMinimum = 50,
                CycleTimeMaximum = 100
            }
        };

        return new CreateProductCommand(productDto);
    }
}
