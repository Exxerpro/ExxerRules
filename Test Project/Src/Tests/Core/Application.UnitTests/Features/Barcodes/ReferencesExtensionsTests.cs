namespace Application.UnitTests.Features.Barcodes;

/// <summary>
/// Represents the ReferencesExtensionsTests.
/// </summary>

public class ReferencesExtensionsTests
{
    /// <summary>
    /// Executes AssignReferences_ValidValues_AssignsCorrectly operation.
    /// </summary>
    [Fact]
    public void AssignReferences_ValidValues_AssignsCorrectly()
    {
        // Arrange
        // Arrange
        var barCodeInformation = BarCodeResult.Create
        (100, 300,
            CycleStatus.Started,
            FlowStatus.Created,
            PartStatus.Ok,
            MachineType.InitialPrinter,
            WorkFlowType.Initial,
            185, 370, "L1AL687508232372685",
            "L687508", 2, 15, ResultValidation.Valid);

        var references = new Dictionary<string, Register>
        {
            { "LastMachineId", new Register() {Value="0"} },
            { "NextMachineId", new Register() {Value="185"} },
            { "CycleStatus", new Register() {Value="2"} },
            { "FlowStatus", new Register() {Value="1"} },
            { "PartStatus", new Register() {Value="1"} },
            { "MachineType", new Register() {Value="4"} },
            { "WorkFlowType", new Register() {Value="1"} },
            { "BarCodeId", new Register() {Value="185"} },
            { "CycleId", new Register() {Value="370"} },
            { "Label", new Register() {Value="L1AL687508232372685"} },

            { "CyclesOk", new Register() {Value="2"} },
            { "ShiftId", new Register() {Value="15"} },
            { "ResultValidation", new Register() {Value="1"} }
        };

        barCodeInformation.InitReferences(references);

        var result = new TaskGatewayResponse();

        result.MapFrom(barCodeInformation);

        var logger = XUnitLogger.CreateLogger<ReferencesExtensionsTests>();
        logger.LogInformation("Testing AssignReferences_ValidValues_AssignsCorrectly");

        foreach (var reference in barCodeInformation.References)
        {
            logger.LogInformation($"Reference '{reference.Key}' = {reference.Value}");
        }

        logger.LogInformation($"barCodeInformation value = {barCodeInformation.ToString()}");

        logger.LogInformation($"LastMachineId: {barCodeInformation.LastMachineId}");
        logger.LogInformation($"NextMachineId: {barCodeInformation.NextMachineId}");
        logger.LogInformation($"CycleStatus: {barCodeInformation.CycleStatus.Value}");
        logger.LogInformation($"FlowStatus: {barCodeInformation.FlowStatus.Value}");
        logger.LogInformation($"PartStatus: {barCodeInformation.PartStatus.Value}");
        logger.LogInformation($"MachineType: {barCodeInformation.MachineType.Value}");
        logger.LogInformation($"WorkFlowType: {barCodeInformation.WorkFlowType.Value}");
        logger.LogInformation($"BarCodeId: {barCodeInformation.BarCodeId}");
        logger.LogInformation($"CycleId: {barCodeInformation.CycleId}");
        logger.LogInformation($"Label: {barCodeInformation.Label}");
        logger.LogInformation($"CyclesOk: {barCodeInformation.CyclesOk}");
        logger.LogInformation($"ShiftId: {barCodeInformation.ShiftId}");

        //// Act
        logger.LogInformation($"AFTER MapFrom - result.LastMachineId: {result.LastMachineId}");
        logger.LogInformation($"AFTER MapFrom - result.NextMachineId: {result.NextMachineId}");

        var applied = result.ApplyReferencesValuesResult();
        applied.IsSuccess.ShouldBeTrue();
        // Assert

        {
            references["LastMachineId"].Value.ShouldBe(barCodeInformation.LastMachineId.ToString());
            references["NextMachineId"].Value.ShouldBe(barCodeInformation.NextMachineId.ToString());
            references["CycleStatus"].Value.ShouldBe(barCodeInformation.CycleStatus.Value.ToString());
            references["FlowStatus"].Value.ShouldBe(barCodeInformation.FlowStatus.Value.ToString());
            references["PartStatus"].Value.ShouldBe(barCodeInformation.PartStatus.Value.ToString());
            references["MachineType"].Value.ShouldBe(barCodeInformation.MachineType.Value.ToString());
            references["WorkFlowType"].Value.ShouldBe(barCodeInformation.WorkFlowType.Value.ToString());
            references["BarCodeId"].Value.ShouldBe(barCodeInformation.BarCodeId.ToString());
            references["CycleId"].Value.ShouldBe(barCodeInformation.CycleId.ToString());
            references["Label"].Value.ShouldBe(barCodeInformation.Label);
            references["CyclesOk"].Value.ShouldBe(barCodeInformation.CyclesOk.ToString());
            references["ShiftId"].Value.ShouldBe(barCodeInformation.ShiftId.ToString());
        }
    }

    /// <summary>
    /// Executes AssignReferences_EmptyReferences_ThrowsArgumentOutOfRangeException operation.
    /// </summary>

    [Fact]
    public void AssignReferences_EmptyReferences_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var barCodeInformation = BarCodeResult.Create(
            100, 300,
            CycleStatus.Started,
            FlowStatus.Created,
            PartStatus.Ok,
            MachineType.InitialPrinter,
            WorkFlowType.Initial,
            185, 370, "L1AL687508232372685",
            "L687508", 2, 15, ResultValidation.Valid
        );

        var result = new TaskGatewayResponse();
        result.MapFrom(barCodeInformation);

        // Act
        var res = result.ApplyReferencesValuesResult();

        //// Assert
        res.IsFailure.ShouldBeTrue();
        res.Errors.ShouldContain(e => e.StartsWith("references can't be empty"));
    }

    /// <summary>
    /// Executes AssignReferences_NullReferences_ThrowsArgumentNullException operation.
    /// </summary>

    [Fact]
    public void AssignReferences_NullReferences_ThrowsArgumentNullException()
    {
        // Arrange
        var barCodeInformation = BarCodeResult.Create(
            100, 300,
            CycleStatus.Started,
            FlowStatus.Created,
            PartStatus.Ok,
            MachineType.InitialPrinter,
            WorkFlowType.Initial,
            185, 370, "L1AL687508232372685",
            "L687508", 2, 15, ResultValidation.Valid
        );

        var result = new TaskGatewayResponse();
        //[Fix]
        //CLAUDE
        //Date: 29/08/2025
        //Reason: [CS8625] Use proper null with cast instead of default!
        barCodeInformation.InitReferences(null!);

        result.MapFrom(barCodeInformation);
        result.References = null!;

        //// Act
        var res2 = result.ApplyReferencesValuesResult();

        // Assert
        res2.IsFailure.ShouldBeTrue();
        res2.Errors.ShouldContain(e => e.StartsWith("references can't be null"));
    }
}
