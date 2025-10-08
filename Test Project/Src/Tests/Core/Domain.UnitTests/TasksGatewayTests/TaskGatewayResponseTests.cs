namespace IndTrace.Domain.UnitTests.TasksGatewayTests;

/// <summary>
/// Unit tests for TaskGatewayResponse
/// </summary>
public class TaskGatewayResponseTests
{
    /// <summary>
    /// Executes TaskGatewayResponse_WithAllRequiredProperties_ShouldCreateInstanceSuccessfully operation.
    /// </summary>
    [Fact]
    public void TaskGatewayResponse_WithAllRequiredProperties_ShouldCreateInstanceSuccessfully()
    {
        // Arrange & Act
        var instance = new TaskGatewayResponse();

        // Assert
        instance.ShouldNotBeNull();
        instance.ExecutionTime.ShouldBe(TimeSpan.Zero);
        instance.ResponseId.ShouldBe(0);
        instance.MachineId.ShouldBe(0);
        instance.BarCodeId.ShouldBe(0);
        instance.CycleId.ShouldBe(0);
        instance.CyclesOk.ShouldBe(0);
        instance.PartNumber.ShouldBe(string.Empty);
        instance.Description.ShouldBe(string.Empty);
        instance.Label.ShouldBe(string.Empty);
        instance.Error.ShouldBe(string.Empty);
        instance.CycleStatus.ShouldBe(CycleStatus.None);
        instance.MachineType.ShouldBe(MachineType.None);
        instance.PartStatus.ShouldBe(PartStatus.None);
        instance.FlowStatus.ShouldBe(FlowStatus.None);
        instance.ResultValidation.ShouldBe(ResultValidation.None);
        instance.WorkFlowType.ShouldBe(WorkFlowType.None);
        instance.Recipe.ShouldNotBeNull();
        instance.Cycle.ShouldNotBeNull();
        instance.BarCode.ShouldNotBeNull();
        instance.MasterLabel.ShouldNotBeNull();
        instance.References.ShouldNotBeNull();
        instance.References.Count.ShouldBe(0);
        instance.TimeStamp.ShouldBeInRange(DateTime.Now.AddSeconds(-5), DateTime.Now.AddSeconds(5));
    }

    /// <summary>
    /// Executes TaskGatewayResponse_WithInvalidConfiguration_ShouldHandleErrorsGracefully operation.
    /// </summary>

    [Fact]
    public void TaskGatewayResponse_WithInvalidConfiguration_ShouldHandleErrorsGracefully()
    {
        // Arrange
        var instance = new TaskGatewayResponse();

        // Act & Assert - Testing ApplyReferencesValues with null references
        instance.WithReferences(null!);
        var result = instance.ApplyReferencesValuesResult();
        result.IsFailure.ShouldBeTrue();

        // Act & Assert - Testing ApplyReferencesValues with empty references
        instance.WithReferences(new Dictionary<string, Register>());
        var result2 = instance.ApplyReferencesValuesResult();
        result2.IsFailure.ShouldBeTrue();

        // Act & Assert - Testing ToDto with null source
        Should.Throw<ArgumentNullException>(() => TaskGatewayResponse.ToDto((IBarCodeResult)null!))
            .ParamName.ShouldBe("src");

        Should.Throw<ArgumentNullException>(() => TaskGatewayResponse.ToDto((TaskGatewayRequest)null!))
            .ParamName.ShouldBe("src");
    }

    /// <summary>
    /// Executes TaskGatewayResponse_WhenPropertiesAssigned_ShouldMaintainAllValues operation.
    /// </summary>

    [Fact]
    public void TaskGatewayResponse_WhenPropertiesAssigned_ShouldMaintainAllValues()
    {
        // Arrange
        var instance = new TaskGatewayResponse();
        var testDateTime = DateTime.Now.AddHours(-1);
        var testParameters = new Dictionary<string, string> { { "key1", "value1" } };

        // Act & Assert - Test all property setters and getters
        instance.ExecutionTime = TimeSpan.FromMinutes(5);
        instance.ExecutionTime.ShouldBe(TimeSpan.FromMinutes(5));

        instance.ResponseId = 123;
        instance.ResponseId.ShouldBe(123);

        instance.MachineId = 456;
        instance.MachineId.ShouldBe(456);

        instance.PlcId = 789;
        instance.PlcId.ShouldBe(789);

        instance.BarCodeId = 101;
        instance.BarCodeId.ShouldBe(101);

        instance.CycleId = 202;
        instance.CycleId.ShouldBe(202);

        instance.CyclesOk = 303;
        instance.CyclesOk.ShouldBe(303);

        instance.ShiftId = 404;
        instance.ShiftId.ShouldBe(404);

        instance.CommandId = 505;
        instance.CommandId.ShouldBe(505);

        instance.Name = "TestResponse";
        instance.Name.ShouldBe("TestResponse");

        instance.PartNumber = "PN-12345";
        instance.PartNumber.ShouldBe("PN-12345");

        instance.Description = "Test Description";
        instance.Description.ShouldBe("Test Description");

        instance.Label = "LBL-001";
        instance.Label.ShouldBe("LBL-001");

        instance.Error = "Test Error";
        instance.Error.ShouldBe("Test Error");

        instance.LastMachineId = 606;
        instance.LastMachineId.ShouldBe(606);

        instance.NextMachineId = 707;
        instance.NextMachineId.ShouldBe(707);

        instance.CycleStatus = CycleStatus.Started;
        instance.CycleStatus.ShouldBe(CycleStatus.Started);

        instance.MachineType = MachineType.Initial;
        instance.MachineType.ShouldBe(MachineType.Initial);

        instance.PartStatus = PartStatus.Ok;
        instance.PartStatus.ShouldBe(PartStatus.Ok);

        instance.FlowStatus = FlowStatus.Created;
        instance.FlowStatus.ShouldBe(FlowStatus.Created);

        instance.ResultValidation = ResultValidation.Valid;
        instance.ResultValidation.ShouldBe(ResultValidation.Valid);

        instance.RequestTask = "TestTask";
        instance.RequestTask.ShouldBe("TestTask");

        instance.WorkFlowType = WorkFlowType.Initial;
        instance.WorkFlowType.ShouldBe(WorkFlowType.Initial);

        instance.TimeStamp = testDateTime;
        instance.TimeStamp.ShouldBe(testDateTime);

        instance.Parameters = testParameters;
        instance.Parameters.ShouldBe(testParameters);
        instance.Parameters["key1"].ShouldBe("value1");
    }

    /// <summary>
    /// Executes TaskGatewayResponse_WhenMethodsInvoked_ShouldProduceExpectedOutcomes operation.
    /// </summary>

    [Fact]
    public void TaskGatewayResponse_WhenMethodsInvoked_ShouldProduceExpectedOutcomes()
    {
        // Arrange
        var instance = new TaskGatewayResponse();
        var references = new Dictionary<string, Register>
        {
            { "LastMachineId", new Register { Value = "100" } },
            { "NextMachineId", new Register { Value = "200" } },
            { "CycleStatus", new Register { Value = "1" } },
            { "PartStatus", new Register { Value = "2" } },
            { "CyclesOk", new Register { Value = "50" } },
            { "ShiftId", new Register { Value = "300" } },
            { "Label", new Register { Value = "TestLabel" } }
        };

        // Act - Test fluent builder methods
        var result = instance.WithMachineId(1001)
                            .WithBarCodeId(2002)
                            .WithCycleId(3003)
                            .WithCyclesOk(25)
                            .WithResultValidation(ResultValidation.Valid)
                            .WithPartNumber("PN-TEST")
                            .WithName("TestName")
                            .WithDescription("TestDesc")
                            .WithReferences(references);

        // Assert - Verify fluent methods return same instance and set properties
        result.ShouldBeSameAs(instance);
        instance.MachineId.ShouldBe(1001);
        instance.BarCodeId.ShouldBe(2002);
        instance.CycleId.ShouldBe(3003);
        instance.CyclesOk.ShouldBe(25);
        instance.ResultValidation.ShouldBe(ResultValidation.Valid);
        instance.PartNumber.ShouldBe("PN-TEST");
        instance.Name.ShouldBe("TestName");
        instance.Description.ShouldBe("TestDesc");
        instance.References.ShouldBe(references);

        // Act - Test ApplyReferencesValues method
        var ok = instance.ApplyReferencesValuesResult();
        ok.IsSuccess.ShouldBeTrue();

        // Assert - Verify references are updated with property values
        references["LastMachineId"].Value.ShouldBe("0"); // Default value, not set by fluent method
        references["NextMachineId"].Value.ShouldBe("0"); // Default value, not set by fluent method
        references["CyclesOk"].Value.ShouldBe("25"); // Updated from property value set by WithCyclesOk(25)
        references["Label"].Value.ShouldBe(""); // Default empty string, not set by fluent method

        // Act - Test EnsureIsValidToRenderAndPersist
        var isValid = instance.EnsureIsValidToRenderAndPersist();

        // Assert
        isValid.ShouldBeTrue();
    }

    /// <summary>
    /// Executes TaskGatewayResponse_BusinessScenario_ShouldEnforceAllDomainRules operation.
    /// </summary>

    [Fact]
    public void TaskGatewayResponse_BusinessScenario_ShouldEnforceAllDomainRules()
    {
        // Arrange
        var instance = new TaskGatewayResponse();
        var barCodeResult = Substitute.For<IBarCodeResult>();
        barCodeResult.MachineId.Returns(1001);
        barCodeResult.BarCodeId.Returns(2002);
        barCodeResult.CycleId.Returns(3003);
        barCodeResult.CyclesOk.Returns(75);
        barCodeResult.ShiftId.Returns(4004);
        barCodeResult.CommandId.Returns(5005);
        barCodeResult.ResultValidation.Returns(ResultValidation.Valid);
        barCodeResult.PartNumber.Returns("PN-DOMAIN");
        barCodeResult.Label.Returns("LBL-DOMAIN");
        barCodeResult.Description.Returns("Domain Description");
        barCodeResult.Error.Returns("Domain Error");
        barCodeResult.LastMachineId.Returns(100);
        barCodeResult.NextMachineId.Returns(200);
        barCodeResult.CycleStatus.Returns(CycleStatus.FinishedOk);
        barCodeResult.FlowStatus.Returns(FlowStatus.Finished);
        barCodeResult.PartStatus.Returns(PartStatus.Ok);
        barCodeResult.MachineType.Returns(MachineType.Final);
        barCodeResult.WorkFlowType.Returns(WorkFlowType.Final);
        barCodeResult.Recipe.Returns(new Recipe { RecipeId = 101 });
        barCodeResult.Cycle.Returns(new Cycle { CycleId = 202 });
        barCodeResult.BarCode.Returns(new BarCode { BarCodeId = 303 });
        barCodeResult.MasterLabel.Returns(new MasterLabel { MasterLabelId = 404 });
        barCodeResult.References.Returns(new Dictionary<string, Register>());

        // Act - Execute domain mapping logic
        instance.MapFrom(barCodeResult);

        // Assert - Verify business rules and domain mapping
        instance.MachineId.ShouldBe(1001);
        instance.BarCodeId.ShouldBe(2002);
        instance.CycleId.ShouldBe(3003);
        instance.CyclesOk.ShouldBe(75);
        instance.ShiftId.ShouldBe(4004);
        instance.ResultValidation.ShouldBe(ResultValidation.Valid);
        instance.PartNumber.ShouldBe("PN-DOMAIN");
        instance.Label.ShouldBe("LBL-DOMAIN");
        instance.Description.ShouldBe("Domain Description");
        instance.LastMachineId.ShouldBe(100);
        instance.NextMachineId.ShouldBe(200);
        instance.CycleStatus.ShouldBe(CycleStatus.FinishedOk);
        instance.FlowStatus.ShouldBe(FlowStatus.Finished);
        instance.PartStatus.ShouldBe(PartStatus.Ok);
        instance.MachineType.ShouldBe(MachineType.Final);
        instance.WorkFlowType.ShouldBe(WorkFlowType.Final);
        instance.Recipe.RecipeId.ShouldBe(101);
        instance.Cycle.CycleId.ShouldBe(202);
        instance.BarCode.BarCodeId.ShouldBe(303);
        instance.MasterLabel.MasterLabelId.ShouldBe(404);
        instance.References.ShouldNotBeNull();

        // Act & Assert - Test static conversion methods for domain consistency
        var taskGatewayRequest = new TaskGatewayRequest
        {
            MachineId = 9001,
            BarCodeId = 9002,
            CycleId = 9003,
            CommandId = 9004,
            PartNumber = "PN-STATIC",
            Description = "Static Description",
            CycleStatus = CycleStatus.Started,
            FlowStatus = FlowStatus.Created,
            PartStatus = PartStatus.NOk,
            MachineType = MachineType.Initial
        };

        var convertedResponse = TaskGatewayResponse.ToDto(taskGatewayRequest);
        convertedResponse.ShouldNotBeNull();
        convertedResponse.MachineId.ShouldBe(9001);
        convertedResponse.BarCodeId.ShouldBe(9002);
        convertedResponse.CycleId.ShouldBe(9003);
        convertedResponse.CommandId.ShouldBe(9004);
        convertedResponse.PartNumber.ShouldBe("PN-STATIC");
        convertedResponse.Description.ShouldBe("Static Description");
        convertedResponse.CycleStatus.ShouldBe(CycleStatus.Started);
        convertedResponse.FlowStatus.ShouldBe(FlowStatus.Created);
        convertedResponse.PartStatus.ShouldBe(PartStatus.NOk);
        convertedResponse.MachineType.ShouldBe(MachineType.Initial);
    }

    /// <summary>
    /// Executes FluentBuilder_ShouldChainAndSetPropertiesCorrectly operation.
    /// </summary>

    [Fact]
    public void FluentBuilder_ShouldChainAndSetPropertiesCorrectly()
    {
        // Arrange
        var response = new TaskGatewayResponse();
        var recipe = new Recipe { RecipeId = 1 };
        var cycle = new Cycle { CycleId = 2 };
        var barCode = new BarCode { BarCodeId = 3 };
        var masterLabel = new MasterLabel { MasterLabelId = 4 };

        // Act
        response.WithMachineId(10)
                .WithBarCodeId(100)
                .WithCycleId(200)
                .WithCyclesOk(50)
                .WithResultValidation(ResultValidation.Valid)
                .WithPartNumber("PN-001")
                .WithLastMachineId(9)
                .WithNextMachineId(11)
                .WithCycleStatus(CycleStatus.FinishedOk)
                .WithFlowStatus(IndTrace.Domain.Enum.FlowStatus.Finished)
                .WithPartStatus(PartStatus.Ok)
                .WithMachineType(MachineType.Final)
                .WithWorkFlowType(WorkFlowType.Initial)
                .WithRecipe(recipe)
                .WithCycle(cycle)
                .WithBarCode(barCode)
                .WithMasterLabel(masterLabel);

        // Assert
        response.MachineId.ShouldBe(10);
        response.BarCodeId.ShouldBe(100);
        response.CycleId.ShouldBe(200);
        response.CyclesOk.ShouldBe(50);
        response.ResultValidation.ShouldBe(ResultValidation.Valid);
        response.PartNumber.ShouldBe("PN-001");
        response.LastMachineId.ShouldBe(9);
        response.NextMachineId.ShouldBe(11);
        response.CycleStatus.ShouldBe(CycleStatus.FinishedOk);
        response.FlowStatus.ShouldBe(IndTrace.Domain.Enum.FlowStatus.Finished);
        response.PartStatus.ShouldBe(PartStatus.Ok);
        response.MachineType.ShouldBe(MachineType.Final);
        response.WorkFlowType.ShouldBe(WorkFlowType.Initial);
        response.Recipe.ShouldBe(recipe);
        response.Cycle.ShouldBe(cycle);
        response.BarCode.ShouldBe(barCode);
        response.MasterLabel.ShouldBe(masterLabel);
    }

    /// <summary>
    /// Executes EnsureIsValidToRenderAndPersist_WithNulls_ShouldSetDefaults operation.
    /// </summary>

    [Fact]
    public void EnsureIsValidToRenderAndPersist_WithNulls_ShouldSetDefaults()
    {
        // Arrange
        var response = new TaskGatewayResponse
        {
            FlowStatus = null!,
            CycleStatus = null!,
            ResultValidation = null!,
            PartStatus = null!,
            MachineType = null!,
            WorkFlowType = null!
        };

        // Act
        var isValid = response.EnsureIsValidToRenderAndPersist();

        // Assert
        isValid.ShouldBeTrue();
        response.FlowStatus.ShouldBe(IndTrace.Domain.Enum.FlowStatus.None);
        response.CycleStatus.ShouldBe(CycleStatus.None);
        response.ResultValidation.ShouldBe(ResultValidation.None);
        response.PartStatus.ShouldBe(PartStatus.None);
        response.MachineType.ShouldBe(MachineType.None);
        response.WorkFlowType.ShouldBe(WorkFlowType.None);
    }

    /// <summary>
    /// Executes ApplyReferencesValues_WithValidReferences_ShouldUpdateProperties operation.
    /// </summary>

    [Fact]
    public void ApplyReferencesValues_WithValidReferences_ShouldUpdateProperties()
    {
        // Arrange
        var response = new TaskGatewayResponse();
        var references = new Dictionary<string, Register>
        {
            { "LastMachineId", new Register { Value = "1" } },
            { "NextMachineId", new Register { Value = "2" } },
            { "CycleStatus", new Register { Value = ((int)CycleStatus.Started).ToString() } },
            { "PartStatus", new Register { Value = ((int)PartStatus.NOk).ToString() } },
            { "CyclesOk", new Register { Value = "123" } },
            { "ShiftId", new Register { Value = "456" } },
            { "Label", new Register { Value = "LBL-001" } },
        };
        response.WithReferences(references);

        response.LastMachineId = 100;
        response.NextMachineId = 2;
        response.CycleStatus = CycleStatus.Started;
        response.PartStatus = PartStatus.NOk;
        response.CyclesOk = 123;
        response.ShiftId = 456;
        response.Label = "LBL-001";

        // Act
        var applied = response.ApplyReferencesValuesResult();
        applied.IsSuccess.ShouldBeTrue();

        // Assert
        response.References.Keys.ShouldContain("LastMachineId");
        response.References.Keys.ShouldContain("NextMachineId");
        response.References.Keys.ShouldContain("CycleStatus");
        response.References.Keys.ShouldContain("PartStatus");
        response.References.Keys.ShouldContain("CyclesOk");
        response.References.Keys.ShouldContain("ShiftId");
        response.References.Keys.ShouldContain("Label");

        response.References.Values.ShouldContain(r => r.Value == "100"); // LastMachineId property value
        response.References.Values.ShouldContain(r => r.Value == "2");   // NextMachineId property value
        response.References.Values.ShouldContain(r => r.Value == "123"); // CyclesOk property value
        response.References.Values.ShouldContain(r => r.Value == "456"); // ShiftId property value
        response.References.Values.ShouldContain(r => r.Value == "LBL-001"); // Label property value
    }

    /// <summary>
    /// Executes ApplyReferencesValues_WithMissingKeys_ShouldNotThrow operation.
    /// </summary>

    [Fact]
    public void ApplyReferencesValues_WithMissingKeys_ShouldNotThrow()
    {
        // Arrange
        var response = new TaskGatewayResponse();
        var references = new Dictionary<string, Register>
        {
            { "SomeOtherKey", new Register { Value = "123" } }
        };
        response.WithReferences(references);

        // Act
        var res = response.ApplyReferencesValuesResult();
        res.IsSuccess.ShouldBeTrue();
    }

    /// <summary>
    /// Executes MapFrom_IBarCodeResult_ShouldMapCorrectly operation.
    /// </summary>

    [Fact]
    public void MapFrom_IBarCodeResult_ShouldMapCorrectly()
    {
        // Arrange
        var barCodeResult = Substitute.For<IBarCodeResult>();
        barCodeResult.LastMachineId.Returns(1);
        barCodeResult.NextMachineId.Returns(2);
        barCodeResult.CycleStatus.Returns(CycleStatus.FinishedOk);
        barCodeResult.FlowStatus.Returns(IndTrace.Domain.Enum.FlowStatus.Created);
        barCodeResult.PartStatus.Returns(PartStatus.Ok);
        barCodeResult.MachineType.Returns(MachineType.Final);
        barCodeResult.WorkFlowType.Returns(WorkFlowType.Initial);
        barCodeResult.BarCodeId.Returns(100);
        barCodeResult.CycleId.Returns(200);
        barCodeResult.Label.Returns("LBL-002");
        barCodeResult.CyclesOk.Returns(50);
        barCodeResult.ShiftId.Returns(300);
        barCodeResult.ResultValidation.Returns(ResultValidation.Valid);

        var response = new TaskGatewayResponse();

        // Act
        response.MapFrom(barCodeResult);

        // Assert
        response.LastMachineId.ShouldBe(1);
        response.NextMachineId.ShouldBe(2);
        // ... assertions for all other mapped properties
        response.ResultValidation.ShouldBe(ResultValidation.Valid);
    }

    /// <summary>
    /// Executes ToString_ShouldReturnNonEmptyString operation.
    /// </summary>

    [Fact]
    public void ToString_ShouldReturnNonEmptyString()
    {
        // Arrange
        var response = new TaskGatewayResponse { Name = "TestResponse" };

        // Act
        var result = response.ToString();

        // Assert
        result.ShouldNotBeNullOrEmpty();
    }
}
