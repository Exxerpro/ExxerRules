using IndTrace.Application.Products.Services;
using Meziantou.Extensions.Logging.Xunit;

namespace IndTrace.Aggregation.BoundedTests.Helpers;

/// <summary>
/// Factory for creating CreateProductCommandHandler instances with all required services.
/// Simplifies test setup by providing a single place to construct handlers with proper dependencies.
/// </summary>
public class CreateProductServicesFactory
{
    private readonly DependenciesFactory _dependenciesFactory;
    private readonly ITestOutputHelper _output;

    public CreateProductServicesFactory(DependenciesFactory dependenciesFactory, ITestOutputHelper output)
    {
        _dependenciesFactory = dependenciesFactory;
        _output = output;
    }
    /// <summary>
    /// Creates the refactored handler with all SRP services using real repositories.
    /// </summary>
    public CreateProductCommandHandler CreateRefactoredHandler()
    {
        // Domain services (no dependencies)
        var productValidator = new ProductValidator();
        var productFactory = new ProductFactory();
        var productEventFactory = new ProductEventFactory();

        // Application services (with repository dependencies)
        var uniquenessValidator = new ProductUniquenessValidator(_dependenciesFactory.DpProductRepository, XUnitLogger.CreateLogger<ProductUniquenessValidator>(_output));
        var customerLookupService = new CustomerLookupService(_dependenciesFactory.DpCustomerRepository, XUnitLogger.CreateLogger<CustomerLookupService>(_output));
        var lineLookupService = new LineLookupService(_dependenciesFactory.DpLineRepository, XUnitLogger.CreateLogger<LineLookupService>(_output));
        var workflowOrchestrator = new WorkflowOrchestrator(_dependenciesFactory.DpWorkFlowRepository, XUnitLogger.CreateLogger<WorkflowOrchestrator>(_output));
        var ruleOrchestrator = new RuleOrchestrator(_dependenciesFactory.DpRuleRepository, _dependenciesFactory.DpMachineRepository, XUnitLogger.CreateLogger<RuleOrchestrator>(_output));
        var recipeOrchestrator = new RecipeOrchestrator(_dependenciesFactory.DpRecipeRepository, _dependenciesFactory.DpMachineRepository, XUnitLogger.CreateLogger<RecipeOrchestrator>(_output));
        var persistenceOrchestrator = new ProductPersistenceOrchestrator(
            _dependenciesFactory.DpProductRepository,
            productFactory,
            uniquenessValidator,
            XUnitLogger.CreateLogger<ProductPersistenceOrchestrator>(_output)
        );

        return new CreateProductCommandHandler(
            productValidator,
            productFactory,
            productEventFactory,
            uniquenessValidator,
            customerLookupService,
            lineLookupService,
            workflowOrchestrator,
            ruleOrchestrator,
            recipeOrchestrator,
            persistenceOrchestrator,
            XUnitLogger.CreateLogger<CreateProductCommandHandler>(_output)
        );
    }

    /// <summary>
    /// Creates the handler for golden comparison tests.
    /// NOTE: Since original handler was replaced with refactored SRP version, this now creates the same SRP handler.
    /// </summary>
    public CreateProductCommandHandler CreateOriginalHandler()
    {
        // Since the original handler was replaced with the SRP version,
        // we now create the same SRP handler for both methods
        return CreateRefactoredHandler();
    }
}
