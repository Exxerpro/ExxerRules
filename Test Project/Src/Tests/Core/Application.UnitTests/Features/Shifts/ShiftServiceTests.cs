using IndTrace.Application.Shifts.Services;

namespace Application.UnitTests.Features.Shifts;

/// <summary>
/// Unit tests for ShiftService
/// </summary>
public class ShiftServiceTests
{
    //[Fix]
    //CLAUDE
    //Date: 29/08/2025
    //Reason: [CS0414] Removed unused private fields - only constructor tests present
    private readonly IShiftDetectionRuleExecutor shiftDetectionRuleExecutor = new ShiftDetectionRuleExecutor();

    [Fact]
    public void Constructor_WithValidRepositories_ShouldNotThrowException()
    {
        // Arrange
        var shiftRepository = Substitute.For<IRepository<IndTrace.Domain.Entities.Shift>>();
        var cycleRepository = Substitute.For<IRepository<IndTrace.Domain.Entities.Cycle>>();
        var logger = XUnitLogger.CreateLogger<ShiftService>();
        var dateTimeMachine = Substitute.For<IDateTimeMachine>();

        // Act & Assert
        Should.NotThrow(() =>
            new ShiftService(shiftRepository, cycleRepository, shiftDetectionRuleExecutor, logger, dateTimeMachine));
    }
}
