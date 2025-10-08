using IndTrace.Domain.Entities.BarCodes;
using IndTrace.Application.BarCodes.Queries.GetBarCodeList;
using IndTrace.Application.Models.Interfaces;

namespace Application.UnitTests.Features.Barcodes;

/// <summary>
/// Unit tests for GetBarCodesListQueryHandler
/// </summary>
public class GetBarCodesListQueryHandlerTests
{
    private readonly IReadOnlyRepository<BarCode> _barCodeRepository;
    private readonly IReadOnlyRepository<MasterLabel> _masterLabelRepository;
    private readonly IReadOnlyRepository<Cycle> _cycleRepository;
    private readonly GetBarCodesListQueryHandler _handler;

    public GetBarCodesListQueryHandlerTests()
    {
        _barCodeRepository = Substitute.For<IReadOnlyRepository<BarCode>>();
        _masterLabelRepository = Substitute.For<IReadOnlyRepository<MasterLabel>>();
        _cycleRepository = Substitute.For<IReadOnlyRepository<Cycle>>();
        _handler = new GetBarCodesListQueryHandler(_barCodeRepository, _masterLabelRepository, _cycleRepository);
    }

    [Fact]
    public void Constructor_ShouldCreateInstance()
    {
        // Arrange & Act
        var handler = new GetBarCodesListQueryHandler(_barCodeRepository, _masterLabelRepository, _cycleRepository);

        // Assert
        handler.ShouldNotBeNull();
    }

    [Fact]
    public void Handler_ShouldImplementCorrectInterface()
    {
        // Assert
        _handler.ShouldNotBeNull();
        IMonitorRequestHandler<GetBarCodesListQuery, IndTrace.Application.BarCodes.Queries.GetBarCodeList.BarCodesListVm> monitorRequestHandler = _handler.ShouldBeAssignableTo<IMonitorRequestHandler<GetBarCodesListQuery, IndTrace.Application.BarCodes.Queries.GetBarCodeList.BarCodesListVm>>();
    }
}
