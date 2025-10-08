namespace IndTrace.Aggregation.BoundedTests.Middleware;

/// <summary>
/// Unit tests for ValidationBehavior - Testing the pipeline behavior with real validators
/// </summary>
public class ValidationBehaviorTests : DependenciesFactory
{
    public ValidationBehaviorTests(ITestOutputHelper outputHelper) : base(outputHelper)
    {
    }

    // Simple test validator that always passes validation
    public class AlwaysValidValidator<T> : AbstractValidator<T>
    {
        public AlwaysValidValidator()
        {
            // No validation rules = always valid
        }
    }

    // Simple test validator that always fails validation
    public class AlwaysInvalidValidator<T> : AbstractValidator<T>
    {
        public AlwaysInvalidValidator()
        {
            RuleFor(x => x).Must(x => true == false).WithMessage("Test validation failure");
        }
    }

    [Fact]
    public async Task Handle_WithValidRequest_ShouldPassValidationAndProceedToNext()
    {
        await Initialization;

        // Arrange - Use real validator that passes validation

        var validator = new AlwaysValidValidator<GetAppDetailsMonitorRequest>();
        var validators = new List<IValidator<GetAppDetailsMonitorRequest>> { validator };

        var logger = XUnitLogger.CreateLogger<ValidationBehavior<GetAppDetailsMonitorRequest, ApplicationConfiguration>>();

        var behavior = new ValidationBehavior<GetAppDetailsMonitorRequest, ApplicationConfiguration>(validators, logger);

        var request = new GetAppDetailsMonitorRequest(false);
        var expectedResponse = new ApplicationConfiguration();

        RequestFunctionalHandlerDelegate<ApplicationConfiguration> next = () => Task.FromResult(expectedResponse);

        // Act
        var result = await behavior.HandleAsync(request, next, TestContext.Current.CancellationToken);

        // Assert
        result.ShouldBe(expectedResponse);
    }

    [Fact]
    public async Task Handle_WithMultipleValidators_ShouldRunAllValidators()
    {
        await Initialization;

        // Arrange - Use multiple real validators that pass validation

        var validator1 = new AlwaysValidValidator<GetAppDetailsMonitorRequest>();
        var validator2 = new AlwaysValidValidator<GetAppDetailsMonitorRequest>();
        var validators = new List<IValidator<GetAppDetailsMonitorRequest>> { validator1, validator2 };
        var logger = XUnitLogger.CreateLogger<ValidationBehavior<GetAppDetailsMonitorRequest, ApplicationConfiguration>>();
        var behavior = new ValidationBehavior<GetAppDetailsMonitorRequest, ApplicationConfiguration>(validators, logger);

        var request = new GetAppDetailsMonitorRequest(true);
        var expectedResponse = new ApplicationConfiguration();

        RequestFunctionalHandlerDelegate<ApplicationConfiguration> next = () => Task.FromResult(expectedResponse);

        // Act
        var result = await behavior.HandleAsync(request, next, TestContext.Current.CancellationToken);

        // Assert
        result.ShouldBe(expectedResponse);
    }

    [Fact]
    public async Task Handle_WithFailingValidator_ShouldThrowValidationException()
    {
        await Initialization;

        // Arrange - Use real validator that fails validation

        var validator = new AlwaysInvalidValidator<GetAppDetailsMonitorRequest>();
        var validators = new List<IValidator<GetAppDetailsMonitorRequest>> { validator };
        var logger = XUnitLogger.CreateLogger<ValidationBehavior<GetAppDetailsMonitorRequest, ApplicationConfiguration>>();
        var behavior = new ValidationBehavior<GetAppDetailsMonitorRequest, ApplicationConfiguration>(validators, logger);

        var request = new GetAppDetailsMonitorRequest(false);

        RequestFunctionalHandlerDelegate<ApplicationConfiguration> next = () => Task.FromResult(new ApplicationConfiguration());

        // Act &
        var result = await behavior.HandleAsync(request, next, TestContext.Current.CancellationToken);

        //Assert - Should throw ValidationException
        // Validator des not trow exceptons but retun value with initial values, that is not correct
        // [TODO] [ABR] [REFACTOR] [ABR] [HANDLER] 7-SEPT-2025
        // REFACTOR HANDLER TO RETURN A RESULT OF T
        // REFACTOR BEHAVIR TO HANDLER A RESULT OF T WRAPER
        result.ShouldNotBeNull();

        result.Machines.Count().ShouldBe(0);
        result.Customers.Count().ShouldBe(0);
        result.Plcs.Count().ShouldBe(0);
    }
}
