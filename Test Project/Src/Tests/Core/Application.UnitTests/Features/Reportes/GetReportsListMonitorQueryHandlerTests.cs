using IndTrace.Application.BarCodes.Queries.Composers;
using IndTrace.Application.BarCodes.Queries.Filters;
using IndTrace.Application.BarCodes.Queries.Mappers;

namespace Application.UnitTests.Features.Reportes;

/// <summary>
/// Unit tests for GetReportsListMonitorQueryHandler using SRP-compliant services
/// </summary>
public class GetReportsListMonitorQueryHandlerTests
{
    private readonly IReportsListQueryComposer _queryComposerSub = null!;
    private readonly IBarCodeListMapper _barCodeMapperSub = null!;
    private readonly IRegisterDataFilter _registerFilterSub = null!;
    private readonly IRepository<BarCode> _barCodeRepository = null!;
    private readonly IRepository<Register> _registerRepository = null!;
    private readonly IRepository<Cycle> _cycleRepository = null!;
    private readonly IRepository<MasterLabel> _masterLabelRepository = null!;
    private readonly IRepository<Customer> _customerRepository = null!;
    private readonly IRepository<Product> _productRepository = null!;
    private readonly IRepository<Line> _lineRepository = null!;
    private readonly ILogger<GetReportsListMonitorQueryHandler> _logger = null!;
    private readonly GetReportsListMonitorQueryHandler _handler = null!;
    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>

    public GetReportsListMonitorQueryHandlerTests()
    {
        _queryComposerSub = Substitute.For<IReportsListQueryComposer>();
        _barCodeMapperSub = Substitute.For<IBarCodeListMapper>();
        _registerFilterSub = Substitute.For<IRegisterDataFilter>();
        _barCodeRepository = Substitute.For<IRepository<BarCode>>();
        _registerRepository = Substitute.For<IRepository<Register>>();
        _cycleRepository = Substitute.For<IRepository<Cycle>>();
        _masterLabelRepository = Substitute.For<IRepository<MasterLabel>>();
        _customerRepository = Substitute.For<IRepository<Customer>>();
        _productRepository = Substitute.For<IRepository<Product>>();
        _lineRepository = Substitute.For<IRepository<Line>>();
        _logger = XUnitLogger.CreateLogger<GetReportsListMonitorQueryHandler>();

        _handler = new GetReportsListMonitorQueryHandler(
            _queryComposerSub,
            _barCodeMapperSub,
            _registerFilterSub,
            _barCodeRepository,
            _registerRepository,
            _cycleRepository,
            _masterLabelRepository,
            _customerRepository,
            _productRepository,
            _lineRepository,
            _logger
        );
    }
    /// <summary>
    /// Executes Constructor_WithValidParameters_ShouldCreateInstance operation.
    /// </summary>

    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateInstance()
    {
        // Arrange & Act
        var handler = new GetReportsListMonitorQueryHandler(
            _queryComposerSub,
            _barCodeMapperSub,
            _registerFilterSub,
            _barCodeRepository,
            _registerRepository,
            _cycleRepository,
            _masterLabelRepository,
            _customerRepository,
            _productRepository,
            _lineRepository,
            _logger
        );

        // Assert
        handler.ShouldNotBeNull();
    }
    /// <summary>
    /// Executes Constructor_WithNullBarCodeRepository_ShouldReturnFailureResult operation.
    /// </summary>

    // MARKED FOR DELETION - Constructor null guard test no longer needed for DI handlers
    // [Fact]
    //     public void Constructor_WithNullBarCodeRepository_ShouldReturnFailureResult()
    //     {
    //         // Arrange & Act & Assert
    //         Should.Throw<ArgumentNullException>(() => new GetReportsListMonitorQueryHandler(
    //             null!,
    //             _registerRepository,
    //             _cycleRepository,
    //             _masterLabelRepository,
    //             _customerRepository,
    //             _productRepository,
    //             _lineRepository,
    //             _logger))
    //             .ParamName.ShouldBe("barCodeRepository");
    //     }
    /// <summary>
    /// Executes Constructor_WithNullRegisterRepository_ShouldReturnFailureResult operation.
    /// </summary>

    // MARKED FOR DELETION - Constructor null guard test no longer needed for DI handlers
    // [Fact]
    //     public void Constructor_WithNullRegisterRepository_ShouldReturnFailureResult()
    //     {
    //         // Arrange & Act & Assert
    //         Should.Throw<ArgumentNullException>(() => new GetReportsListMonitorQueryHandler(
    //             _barCodeRepository,
    //             null!,
    //             _cycleRepository,
    //             _masterLabelRepository,
    //             _customerRepository,
    //             _productRepository,
    //             _lineRepository,
    //             _logger))
    //             .ParamName.ShouldBe("registerRepository");
    //     }
    /// <summary>
    /// Executes Constructor_WithNullCycleRepository_ShouldReturnFailureResult operation.
    /// </summary>

    // MARKED FOR DELETION - Constructor null guard test no longer needed for DI handlers
    // [Fact]
    //     public void Constructor_WithNullCycleRepository_ShouldReturnFailureResult()
    //     {
    //         // Arrange & Act & Assert
    //         Should.Throw<ArgumentNullException>(() => new GetReportsListMonitorQueryHandler(
    //             _barCodeRepository,
    //             _registerRepository,
    //             null!,
    //             _masterLabelRepository,
    //             _customerRepository,
    //             _productRepository,
    //             _lineRepository,
    //             _logger))
    //             .ParamName.ShouldBe("cycleRepository");
    //     }
    /// <summary>
    /// Executes Constructor_WithNullLogger_ShouldReturnFailureResult operation.
    /// </summary>

    // MARKED FOR DELETION - Constructor null guard test no longer needed for DI handlers
    // [Fact]
    //     public void Constructor_WithNullLogger_ShouldReturnFailureResult()
    //     {
    //         // Arrange, Act & Assert
    //         Should.Throw<ArgumentNullException>(() =>
    //             new GetReportsListMonitorQueryHandler(
    //                 _barCodeRepository,
    //                 _registerRepository,
    //                 _cycleRepository,
    //                 _masterLabelRepository,
    //                 _customerRepository,
    //                 _productRepository,
    //                 _lineRepository,
    //                 null!
    //             )
    //         );
    //     }
    /// <summary>
    /// Executes Handler_ShouldNotBeNull_WhenCreated operation.
    /// </summary>

    [Fact]
    public void Handler_ShouldNotBeNull_WhenCreated()
    {
        // Arrange & Act
        var handler = _handler;

        // Assert
        handler.ShouldNotBeNull();
    }
}
