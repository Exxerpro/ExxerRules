using IndTrace.Application.BarCodes.Queries.GetReportsReport;
using IndTrace.Application.UI.Models;
using IndTrace.Domain.Enum;

namespace Application.UnitTests.Features.Barcodes;

/// <summary>
/// Comprehensive unit tests for BarCodeReportVm - Manufacturing barcode report view model
/// Tests cover automotive, electronics, pharmaceutical, and aerospace manufacturing report scenarios
/// </summary>
public class BarCodeReportVmTests
{
    /// <summary>
    /// Executes Should_CreateInstance_When_Instantiated operation.
    /// </summary>
    [Fact]
    public void Should_CreateInstance_When_Instantiated()
    {
        // Arrange & Act
        var reportVm = new BarCodeReportVm();

        // Assert
        reportVm.ShouldNotBeNull();
        reportVm.MachineId.ShouldBe(0);
        reportVm.BarCodeId.ShouldBe(0);
        reportVm.CycleId.ShouldBe(0);
        reportVm.Label.ShouldBe(string.Empty);
        reportVm.ProductId.ShouldBe(0);
        reportVm.LastMachineId.ShouldBe(0);
        reportVm.NextMachineId.ShouldBe(0);
        reportVm.Cycles.ShouldNotBeNull().ShouldBeEmpty();
        reportVm.Registers.ShouldNotBeNull().ShouldBeEmpty();
        reportVm.CreatedOn.ShouldBe(default(DateTime));
        reportVm.ModifiedOn.ShouldBe(default(DateTime));
    }

    /// <summary>
    /// Executes Should_SetAllProperties_When_ValidManufacturingDataProvided operation.
    /// </summary>

    [Fact]
    public void Should_SetAllProperties_When_ValidManufacturingDataProvided()
    {
        // Arrange
        var reportVm = new BarCodeReportVm();
        var expectedCreatedOn = new DateTime(2024, 1, 15, 8, 30, 0);
        var expectedModifiedOn = new DateTime(2024, 1, 16, 14, 45, 30);
        var expectedCycles = new List<CycleView> { new CycleView() };
        var expectedRegisters = new List<RegisterView> { new RegisterView() };

        // Act - Ford F-150 Engine Manufacturing Report
        reportVm.MachineId = 5001;
        reportVm.BarCodeId = 10001;
        reportVm.CycleId = 20001;
        reportVm.Label = "VIN:1FTFW1ET5DFC12345";
        reportVm.ProductId = 5080;
        reportVm.LastMachineId = 5000;
        reportVm.NextMachineId = 5002;
        reportVm.CycleStatus = CycleStatus.FinishedOk;
        reportVm.FlowStatus = FlowStatus.Finished;
        reportVm.PartStatus = PartStatus.Ok;
        reportVm.MachineType = MachineType.Process;
        reportVm.WorkFlowType = WorkFlowType.Serial;
        reportVm.Cycles = expectedCycles;
        reportVm.Registers = expectedRegisters;
        reportVm.CreatedOn = expectedCreatedOn;
        reportVm.ModifiedOn = expectedModifiedOn;

        // Assert
        reportVm.MachineId.ShouldBe(5001);
        reportVm.BarCodeId.ShouldBe(10001);
        reportVm.CycleId.ShouldBe(20001);
        reportVm.Label.ShouldBe("VIN:1FTFW1ET5DFC12345");
        reportVm.ProductId.ShouldBe(5080);
        reportVm.LastMachineId.ShouldBe(5000);
        reportVm.NextMachineId.ShouldBe(5002);
        reportVm.CycleStatus.ShouldBe(CycleStatus.FinishedOk);
        reportVm.FlowStatus.ShouldBe(FlowStatus.Finished);
        reportVm.PartStatus.ShouldBe(PartStatus.Ok);
        reportVm.MachineType.ShouldBe(MachineType.Process);
        reportVm.WorkFlowType.ShouldBe(WorkFlowType.Serial);
        reportVm.Cycles.ShouldBe(expectedCycles);
        reportVm.Registers.ShouldBe(expectedRegisters);
        reportVm.CreatedOn.ShouldBe(expectedCreatedOn);
        reportVm.ModifiedOn.ShouldBe(expectedModifiedOn);
    }

    /// <summary>
    /// Executes Should_HandleManufacturingScenarios_When_IndustryDataProvided operation.
    /// </summary>

    [Theory]
    [InlineData(1001, 5001, "VIN:1FTFW1ET5DFC12345", 100, "Ford F-150 automotive manufacturing")]
    [InlineData(2002, 7002, "PCB:C02YG0VZJHD4", 200, "iPhone 15 Pro electronics manufacturing")]
    [InlineData(3003, 9003, "BATCH:LOT-PFZ-2024-001", 300, "Pfizer vaccine pharmaceutical manufacturing")]
    [InlineData(4004, 8004, "SN:BA777X-WING-001", 400, "Boeing 777X aerospace manufacturing")]
    public void Should_HandleManufacturingScenarios_When_IndustryDataProvided(
        int barCodeId, int machineId, string label, int productId, string description)
    {
        var logger = XUnitLogger.CreateLogger();
        logger.LogInformation(description);

        // Arrange
        var reportVm = new BarCodeReportVm();

        // Act
        reportVm.BarCodeId = barCodeId;
        reportVm.MachineId = machineId;
        reportVm.Label = label;
        reportVm.ProductId = productId;

        // Assert
        reportVm.BarCodeId.ShouldBe(barCodeId);
        reportVm.MachineId.ShouldBe(machineId);
        reportVm.Label.ShouldBe(label);
        reportVm.ProductId.ShouldBe(productId);
    }

    /// <summary>
    /// Executes Should_ConvertToDto_When_ValidBarCodeEntityProvided operation.
    /// </summary>

    [Fact]
    public void Should_ConvertToDto_When_ValidBarCodeEntityProvided()
    {
        // Arrange - Tesla Model Y Battery Pack Manufacturing
        var barCodeEntity = new BarCode
        {
            MachineId = 7002,
            BarCodeId = 2002,
            Label = "BATT:TSLA-MY-4680-001",
            FlowStatus = (int)FlowStatus.Finished,
            PartStatus = (int)PartStatus.Ok,
            ProductId = 200,
            CreatedOn = new DateTime(2024, 2, 10, 9, 15, 0),
            ModifiedOn = new DateTime(2024, 2, 12, 16, 20, 45)
        };

        // Act
        var resultOfT = BarCodeReportVm.ToDto(barCodeEntity);

        // Assert
        resultOfT.IsSuccess.ShouldBeTrue();
        resultOfT.Value.ShouldNotBeNull();
        var reportDto = resultOfT.Value;
        reportDto.ShouldNotBeNull();
        reportDto.ShouldNotBeNull();

        reportDto.ShouldNotBeNull();
        reportDto.MachineId.ShouldBe(barCodeEntity.MachineId);
        reportDto.BarCodeId.ShouldBe(barCodeEntity.BarCodeId);
        reportDto.Label.ShouldBe(barCodeEntity.Label);
        reportDto.FlowStatus.ShouldBe(barCodeEntity.FlowStatus);
        reportDto.PartStatus.ShouldBe(barCodeEntity.PartStatus);
        reportDto.ProductId.ShouldBe(barCodeEntity.ProductId);
        reportDto.CreatedOn.ShouldBe(barCodeEntity.CreatedOn);
        reportDto.ModifiedOn.ShouldBe(barCodeEntity.ModifiedOn);
    }

    /// <summary>
    /// Executes Should_ConvertToEntity_When_ValidReportDtoProvided operation.
    /// </summary>

    [Fact]
    public void Should_ConvertToEntity_When_ValidReportDtoProvided()
    {
        // Arrange - Pfizer COVID-19 Vaccine Manufacturing Report
        var reportDto = new BarCodeReportVm
        {
            MachineId = 9003,
            BarCodeId = 3003,
            Label = "BATCH:LOT-PFZ-2024-001",
            FlowStatus = FlowStatus.Finished,
            PartStatus = PartStatus.Ok,
            ProductId = 300,
            CreatedOn = new DateTime(2024, 3, 5, 7, 0, 0),
            ModifiedOn = new DateTime(2024, 3, 7, 11, 30, 15)
        };

        // Act
        var resultOfT = BarCodeReportVm.ToEntity(reportDto);

        // Assert
        resultOfT.IsSuccess.ShouldBeTrue();
        resultOfT.Value.ShouldNotBeNull();
        var barCodeEntity = resultOfT.Value;
        barCodeEntity.ShouldNotBeNull();
        barCodeEntity.ShouldNotBeNull();

        barCodeEntity.ShouldNotBeNull();
        barCodeEntity.MachineId.ShouldBe(reportDto.MachineId);
        barCodeEntity.BarCodeId.ShouldBe(reportDto.BarCodeId);
        barCodeEntity.Label.ShouldBe(reportDto.Label);
        barCodeEntity.FlowStatus.Value.ShouldBe(reportDto.FlowStatus.Value);
        barCodeEntity.PartStatus.Value.ShouldBe(reportDto.PartStatus.Value);
        barCodeEntity.ProductId.ShouldBe(reportDto.ProductId);
        barCodeEntity.CreatedOn.ShouldBe(reportDto.CreatedOn);
        barCodeEntity.ModifiedOn.ShouldBe(reportDto.ModifiedOn);
    }

    /// <summary>
    /// Executes Should_ConvertListToDto_When_ValidBarCodeCollectionProvided operation.
    /// </summary>

    [Fact]
    public void Should_ConvertListToDto_When_ValidBarCodeCollectionProvided()
    {
        // Arrange - Multiple manufacturing reports
        var barCodeEntities = new List<BarCode>
        {
            new BarCode { BarCodeId = 1001, Label = "VIN:1FTFW1ET5DFC12345", MachineId = 5001, ProductId = 5080 },
            new BarCode { BarCodeId = 2002, Label = "PCB:C02YG0VZJHD4", MachineId = 7002, ProductId = 200 },
            new BarCode { BarCodeId = 3003, Label = "BATCH:LOT-PFZ-2024-001", MachineId = 9003, ProductId = 300 }
        };

        // Act
        var resultOfT = BarCodeReportVm.ToDtoList(barCodeEntities);

        // Assert
        resultOfT.IsSuccess.ShouldBeTrue();
        resultOfT.Value.ShouldNotBeNull();
        var reportDtoList = resultOfT.Value.ToList();

        reportDtoList.ShouldNotBeNull();
        reportDtoList.Count.ShouldBe(3);
        reportDtoList[0].Label.ShouldBe("VIN:1FTFW1ET5DFC12345");
        reportDtoList[1].Label.ShouldBe("PCB:C02YG0VZJHD4");
        reportDtoList[2].Label.ShouldBe("BATCH:LOT-PFZ-2024-001");
    }

    /// <summary>
    /// Executes Should_ReturnFailureResult_When_NullEntityProvided operation.
    /// </summary>

    [Fact]
    public void Should_ReturnFailureResult_When_NullEntityProvided()
    {
        // Arrange
        BarCode? nullEntity = null!;

        // Act
        var result = BarCodeReportVm.ToDto(nullEntity!);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Errors.ShouldNotBeNull();
        result.Errors.ShouldContain("BarCode source cannot be null");
    }

    /// <summary>
    /// Executes Should_ReturnFailureResult_When_NullDtoProvided operation.
    /// </summary>

    [Fact]
    public void Should_ReturnFailureResult_When_NullDtoProvided()
    {
        // Arrange
        BarCodeReportVm? nullDto = null!;

        // Act
        var result = BarCodeReportVm.ToEntity(nullDto!);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Errors.ShouldNotBeNull();
        result.Errors.ShouldContain("BarCodeReportVm source cannot be null");
    }

    /// <summary>
    /// Executes Should_ReturnFailureResult_When_NullListProvided operation.
    /// </summary>

    [Fact]
    public void Should_ReturnFailureResult_When_NullListProvided()
    {
        // Arrange
        IEnumerable<BarCode>? nullList = null!;

        // Act
        var result = BarCodeReportVm.ToDtoList(nullList!);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Errors.ShouldNotBeNull();
        result.Errors.ShouldContain("BarCode collection cannot be null");
    }

    /// <summary>
    /// Executes Should_HandleNullLabel_When_EntityHasNullLabel operation.
    /// </summary>

    [Fact]
    public void Should_HandleNullLabel_When_EntityHasNullLabel()
    {
        // Arrange
        var barCodeEntity = new BarCode
        {
            MachineId = 9999,
            BarCodeId = 9999,
            Label = null!,
            ProductId = 999
        };

        // Act
        var resultOfT = BarCodeReportVm.ToDto(barCodeEntity);

        // Assert
        resultOfT.IsSuccess.ShouldBeTrue();
        resultOfT.Value.ShouldNotBeNull();
        var reportDto = resultOfT.Value;
        reportDto.ShouldNotBeNull();
        reportDto.ShouldNotBeNull();
        reportDto.ShouldNotBeNull();

        reportDto.Label.ShouldBe("");
    }

    /// <summary>
    /// Executes Should_HandleConcurrentAccess_When_MultipleThreadsModifyProperties operation.
    /// </summary>

    [Fact]
    public async Task Should_HandleConcurrentAccess_When_MultipleThreadsModifyProperties()
    {
        // Arrange
        var reportVm = new BarCodeReportVm();
        var tasks = new List<Task>();

        // Act
        for (int i = 0; i < 10; i++)
        {
            int threadIndex = i;
            tasks.Add(Task.Run(() =>
            {
                reportVm.MachineId = 5000 + threadIndex;
                reportVm.BarCodeId = 10000 + threadIndex;
                reportVm.Label = $"LABEL-{threadIndex:D3}";
                reportVm.ProductId = 5080 + threadIndex;

                return Task.FromResult(reportVm);
            }));
        }

        await Task.WhenAll(tasks.ToArray());

        // Assert
        reportVm.MachineId.ShouldBeInRange(5000, 5009);
        reportVm.BarCodeId.ShouldBeInRange(10000, 10009);
        reportVm.Label.ShouldNotBeNull();
        reportVm.ProductId.ShouldBeInRange(5080, 5089);
    }
}
