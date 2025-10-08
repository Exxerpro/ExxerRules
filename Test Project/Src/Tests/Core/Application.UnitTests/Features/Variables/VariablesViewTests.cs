namespace Application.UnitTests.Features.Variables;

/// <summary>
/// Unit tests for VariablesView
/// </summary>
public class VariablesViewTests
{
    /// <summary>
    /// Executes Constructor_WithDefaultParameters_ShouldCreateInstance operation.
    /// </summary>
    [Fact]
    public void Constructor_WithDefaultParameters_ShouldCreateInstance()
    {
        // Arrange & Act
        var instance = new VariablesView();

        // Assert
        instance.ShouldNotBeNull();
        instance.VariableId.ShouldBe(0);
        instance.MachineId.ShouldBe(0);
        instance.PlcId.ShouldBe(0);
        instance.Name.ShouldBe(string.Empty);
        instance.Description.ShouldBe(string.Empty);
        instance.Alias.ShouldBe(string.Empty);
        instance.Address.ShouldBe(string.Empty);
        instance.NetType.ShouldBe(string.Empty);
        instance.Length.ShouldBe(0);
        instance.IsActive.ShouldBe(0);
        instance.Direction.ShouldBe(0);
        instance.VariableGroupId.ShouldBe(0);
        instance.VariableSpecId.ShouldBeNull();
        instance.TagStatus.ShouldBe(0);
        instance.NativeType.ShouldBe(string.Empty);
        //[Fix]
        //CLAUDE
        //Date: 22/08/2025
        //Reason: Pattern 17 Fix - VariablesView constructor initializes Value to string.Empty, not null
        instance.Value.ShouldBe(string.Empty);
        instance.NativeAddress.ShouldBe(string.Empty);
    }

    /// <summary>
    /// Executes Properties_WhenSet_ShouldReturnCorrectValues operation.
    /// </summary>

    [Fact]
    public void Properties_WhenSet_ShouldReturnCorrectValues()
    {
        // Arrange - Siemens S7-1500 PLC variable for robotic welding cell
        var instance = new VariablesView();
        const int expectedVariableId = 1501;
        const int expectedMachineId = 10001;
        const int expectedPlcId = 15;
        const string expectedName = "WeldingCellCycleStart";
        const string expectedDescription = "Cycle start signal for robotic welding cell";
        const string expectedAlias = "WELD_START";
        const string expectedAddress = "DB1.DBX0.0";
        const string expectedNetType = "BOOL";
        const int expectedLength = 1;
        const int expectedIsActive = 1;
        const int expectedDirection = 1;
        const int expectedVariableGroupId = 10;
        const int expectedVariableSpecId = 1001;
        const int expectedTagStatus = 1;
        const string expectedNativeType = "BOOL";
        const string expectedValue = "true";
        const string expectedNativeAddress = "M0.0";

        // Act
        instance.VariableId = expectedVariableId;
        instance.MachineId = expectedMachineId;
        instance.PlcId = expectedPlcId;
        instance.Name = expectedName;
        instance.Description = expectedDescription;
        instance.Alias = expectedAlias;
        instance.Address = expectedAddress;
        instance.NetType = expectedNetType;
        instance.Length = expectedLength;
        instance.IsActive = expectedIsActive;
        instance.Direction = expectedDirection;
        instance.VariableGroupId = expectedVariableGroupId;
        instance.VariableSpecId = expectedVariableSpecId;
        instance.TagStatus = expectedTagStatus;
        instance.NativeType = expectedNativeType;
        instance.Value = expectedValue;
        instance.NativeAddress = expectedNativeAddress;

        // Assert
        instance.VariableId.ShouldBe(expectedVariableId);
        instance.MachineId.ShouldBe(expectedMachineId);
        instance.PlcId.ShouldBe(expectedPlcId);
        instance.Name.ShouldBe(expectedName);
        instance.Description.ShouldBe(expectedDescription);
        instance.Alias.ShouldBe(expectedAlias);
        instance.Address.ShouldBe(expectedAddress);
        instance.NetType.ShouldBe(expectedNetType);
        instance.Length.ShouldBe(expectedLength);
        instance.IsActive.ShouldBe(expectedIsActive);
        instance.Direction.ShouldBe(expectedDirection);
        instance.VariableGroupId.ShouldBe(expectedVariableGroupId);
        instance.VariableSpecId.ShouldBe(expectedVariableSpecId);
        instance.TagStatus.ShouldBe(expectedTagStatus);
        instance.NativeType.ShouldBe(expectedNativeType);
        instance.Value.ShouldBe(expectedValue);
        instance.NativeAddress.ShouldBe(expectedNativeAddress);
    }

    /// <summary>
    /// Executes Properties_WithVariousManufacturingScenarios_ShouldRetainValues operation.
    /// </summary>

    [Theory]
    [InlineData(2801, 201, 28, "ABB IRC5", "TemperatureSensor", "TEMP_01", "AI1", "REAL", 4)]
    [InlineData(3301, 301, 33, "Fanuc 31i-B", "CycleCounter", "CYC_CNT", "R100", "INT", 2)]
    [InlineData(4401, 401, 44, "Mitsubishi FX5U", "QualityStatus", "QUAL_OK", "M100", "BOOL", 1)]
    [InlineData(5501, 501, 55, "Schneider M580", "ProductionSpeed", "PROD_SPD", "MW200", "WORD", 2)]
    public void Properties_WithVariousManufacturingScenarios_ShouldRetainValues(
        int variableId, int machineId, int plcId, string name, string description, string alias, string address, string netType, int length)
    {
        // Arrange
        var instance = new VariablesView();

        // Act
        instance.VariableId = variableId;
        instance.MachineId = machineId;
        instance.PlcId = plcId;
        instance.Name = name;
        instance.Description = description;
        instance.Alias = alias;
        instance.Address = address;
        instance.NetType = netType;
        instance.Length = length;

        // Assert
        instance.VariableId.ShouldBe(variableId);
        instance.MachineId.ShouldBe(machineId);
        instance.PlcId.ShouldBe(plcId);
        instance.Name.ShouldBe(name);
        instance.Description.ShouldBe(description);
        instance.Alias.ShouldBe(alias);
        instance.Address.ShouldBe(address);
        instance.NetType.ShouldBe(netType);
        instance.Length.ShouldBe(length);
    }

    /// <summary>
    /// Executes EnumValue_WithValidEnumVariables_ShouldReturnCorrectEnumName operation.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="value">The value.</param>
    /// <param name="expectedEnumName">The expectedEnumName.</param>

    [Theory]
    [InlineData("CycleStatusPlc", "1", "NotStarted")]
    [InlineData("CycleStatusPlc", "2", "Started")]
    [InlineData("CycleStatusPlc", "4", "FinishedOk")]
    [InlineData("PartStatusPlc", "1", "Ok")]
    [InlineData("PartStatusPlc", "2", "nOK")]
    [InlineData("FlowStatusPlc", "1", "Created")]
    [InlineData("FlowStatusPlc", "2", "InProcess")]
    [InlineData("MachineTypePlc", "1", "Printer")]
    [InlineData("MachineTypePlc", "8", "Process")]
    [InlineData("WorkFlowTypePlc", "2", "Serial")]
    [InlineData("WorkFlowTypePlc", "32", "Final")]
    public void EnumValue_WithValidEnumVariables_ShouldReturnCorrectEnumName(string name, string value, string expectedEnumName)
    {
        // Using parameters: name, value, expectedEnumName
        _ = name; // xUnit1026 fix
        _ = value; // xUnit1026 fix
        _ = expectedEnumName; // xUnit1026 fix
        // Using parameters: name, value, expectedEnumName
        _ = name; // xUnit1026 fix
        _ = value; // xUnit1026 fix
        _ = expectedEnumName; // xUnit1026 fix
        // Using parameters: name, value, expectedEnumName
        _ = name; // xUnit1026 fix
        _ = value; // xUnit1026 fix
        _ = expectedEnumName; // xUnit1026 fix
        // Using parameters: name, value, expectedEnumName
        _ = name; // xUnit1026 fix
        _ = value; // xUnit1026 fix
        _ = expectedEnumName; // xUnit1026 fix
        // Using parameters: name, value, expectedEnumName
        _ = name; // xUnit1026 fix
        _ = value; // xUnit1026 fix
        _ = expectedEnumName; // xUnit1026 fix
        // Arrange
        var instance = new VariablesView
        {
            Name = name,
            Value = value
        };

        // Act
        var result = instance.EnumValue;

        // Assert
        result.ShouldBe(expectedEnumName);
    }

    /// <summary>
    /// Executes EnumValue_WithNonEnumVariables_ShouldReturnOriginalValue operation.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="value">The value.</param>
    /// <param name="expectedValue">The expectedValue.</param>

    [Theory]
    [InlineData("RegularVariable", "100", "100")]
    [InlineData("TemperatureSensor", "85.5", "85.5")]
    [InlineData("ProductionCounter", "1250", "1250")]
    [InlineData("QualityFlag", "true", "true")]
    public void EnumValue_WithNonEnumVariables_ShouldReturnOriginalValue(string name, string value, string expectedValue)
    {
        // Using parameters: name, value, expectedValue
        _ = name; // xUnit1026 fix
        _ = value; // xUnit1026 fix
        _ = expectedValue; // xUnit1026 fix
        // Using parameters: name, value, expectedValue
        _ = name; // xUnit1026 fix
        _ = value; // xUnit1026 fix
        _ = expectedValue; // xUnit1026 fix
        // Using parameters: name, value, expectedValue
        _ = name; // xUnit1026 fix
        _ = value; // xUnit1026 fix
        _ = expectedValue; // xUnit1026 fix
        // Using parameters: name, value, expectedValue
        _ = name; // xUnit1026 fix
        _ = value; // xUnit1026 fix
        _ = expectedValue; // xUnit1026 fix
        // Using parameters: name, value, expectedValue
        _ = name; // xUnit1026 fix
        _ = value; // xUnit1026 fix
        _ = expectedValue; // xUnit1026 fix
        // Arrange
        var instance = new VariablesView
        {
            Name = name,
            Value = value
        };

        // Act
        var result = instance.EnumValue;

        // Assert
        result.ShouldBe(expectedValue);
    }

    /// <summary>
    /// Executes EnumValue_WithNullValue_ShouldReturnEmptyString operation.
    /// </summary>

    [Fact]
    public void EnumValue_WithNullValue_ShouldReturnEmptyString()
    {
        // Arrange
        var instance = new VariablesView
        {
            Name = "TestVariable",
            Value = null!
        };

        // Act
        var result = instance.EnumValue;

        // Assert
        result.ShouldBe(string.Empty);
    }

    /// <summary>
    /// Executes ToDto_WithValidVariable_ShouldCreateCorrectVariablesView operation.
    /// </summary>

    [Fact]
    public void ToDto_WithValidVariable_ShouldCreateCorrectVariablesView()
    {
        // Arrange - Ford F-150 production line variable
        var variable = new Variable
        {
            VariableId = 1501,
            MachineId = 10001,
            PlcId = 15,
            Name = "CycleStatusPlc",
            Description = "Cycle status from PLC",
            Alias = "CYC_STAT",
            Address = "DB1.DBW10",
            NetType = "INT",
            Length = 2,
            IsActive = 1,
            Direction = 0,
            VariableGroupId = 10,
            VariableSpecId = 1001,
            TagStatus = 1,
            NativeType = "INT",
            Value = "4",
            NativeAddress = "MW10"
        };

        // Act
        var resultWrapper = VariablesView.ToDto(variable);

        // Assert
        resultWrapper.IsSuccess.ShouldBeTrue();
        resultWrapper.Value.ShouldNotBeNull();
        var result = resultWrapper.Value;
        result.ShouldNotBeNull();
        result.ShouldNotBeNull();
        result.ShouldNotBeNull();
        result.VariableId.ShouldBe(1501);
        result.MachineId.ShouldBe(10001);
        result.PlcId.ShouldBe(15);
        result.Name.ShouldBe("CycleStatusPlc");
        result.Description.ShouldBe("Cycle status from PLC");
        result.Alias.ShouldBe("CYC_STAT");
        result.Address.ShouldBe("DB1.DBW10");
        result.NetType.ShouldBe("INT");
        result.Length.ShouldBe(2);
        result.IsActive.ShouldBe(1);
        result.Direction.ShouldBe(0);
        result.VariableGroupId.ShouldBe(10);
        result.VariableSpecId.ShouldBe(1001);
        result.TagStatus.ShouldBe(1);
        result.NativeType.ShouldBe("INT");
        result.Value.ShouldBe("4");
        result.NativeAddress.ShouldBe("MW10");
        result.EnumValue.ShouldBe("FinishedOk"); // Enum conversion should work
    }

    /// <summary>
    /// Executes ToDto_WithNullVariable_ShouldReturnFailureResult operation.
    /// </summary>

    [Fact]
    public void ToDto_WithNullVariable_ShouldReturnFailureResult()
    {
        // Arrange
        Variable nullVariable = null!;

        // Act
        var result = VariablesView.ToDto(nullVariable);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Errors.ShouldNotBeNull();
        result.Errors.ShouldContain("Variable source cannot be null");
    }

    /// <summary>
    /// Executes ToDtoList_WithValidVariableList_ShouldCreateCorrectVariablesViewList operation.
    /// </summary>

    [Fact]
    public void ToDtoList_WithValidVariableList_ShouldCreateCorrectVariablesViewList()
    {
        // Arrange - Multiple manufacturing variables
        var variables = new List<Variable>
        {
            new() { VariableId = 1, Name = "CycleStatusPlc", Value = "2", MachineId = 10001 },
            new() { VariableId = 2, Name = "PartStatusPlc", Value = "1", MachineId = 10002 },
            new() { VariableId = 3, Name = "TemperatureSensor", Value = "85.5", MachineId = 10003 }
        };

        // Act
        var resultWrapper = VariablesView.ToDtoList(variables);

        // Assert
        resultWrapper.IsSuccess.ShouldBeTrue();
        resultWrapper.Value.ShouldNotBeNull();
        var result = resultWrapper.Value;
        result.ShouldNotBeNull();
        result.ShouldNotBeNull();
        result.ShouldNotBeNull();
        result.Count().ShouldBe(3);

        var resultList = result.ToList();
        resultList[0].Name.ShouldBe("CycleStatusPlc");
        resultList[0].EnumValue.ShouldBe("Started");
        resultList[1].Name.ShouldBe("PartStatusPlc");
        resultList[1].EnumValue.ShouldBe("Ok");
        resultList[2].Name.ShouldBe("TemperatureSensor");
        resultList[2].EnumValue.ShouldBe("85.5");
    }

    /// <summary>
    /// Executes ToDtoList_WithNullVariableList_ShouldReturnFailureResult operation.
    /// </summary>

    [Fact]
    public void ToDtoList_WithNullVariableList_ShouldReturnFailureResult()
    {
        // Arrange
        IEnumerable<Variable> nullVariableList = null!;

        // Act
        var result = VariablesView.ToDtoList(nullVariableList);

        // Assert
        //[Fix]
        //CLAUDE
        //Date: 22/08/2025
        //Reason: Pattern 11 Fix - Updated error message expectation to match actual VariablesView.ToDtoList() implementation
        result.IsFailure.ShouldBeTrue();
        result.Errors.ShouldNotBeNull();
        result.Errors.ShouldContain("Variable collection cannot be null");
    }

    /// <summary>
    /// Executes ToEntity_WithValidVariablesView_ShouldCreateCorrectVariable operation.
    /// </summary>

    [Fact]
    public void ToEntity_WithValidVariablesView_ShouldCreateCorrectVariable()
    {
        // Arrange - Electronics manufacturing scenario
        var variablesView = new VariablesView
        {
            VariableId = 8801,
            MachineId = 880,
            PlcId = 88,
            Name = "PCB_InspectionResult",
            Description = "PCB quality inspection result",
            Alias = "PCB_QUAL",
            Address = "DM1000",
            NetType = "DINT",
            Length = 4,
            IsActive = 1,
            Direction = 0,
            VariableGroupId = 88,
            VariableSpecId = 8801,
            TagStatus = 1,
            NativeType = "DWORD",
            Value = "1",
            NativeAddress = "DM1000"
        };

        // Act
        var resultWrapper = VariablesView.ToEntity(variablesView);

        // Assert
        resultWrapper.IsSuccess.ShouldBeTrue();
        resultWrapper.Value.ShouldNotBeNull();
        var result = resultWrapper.Value;
        result.ShouldNotBeNull();
        result.ShouldNotBeNull();
        result.ShouldNotBeNull();
        result.VariableId.ShouldBe(8801);
        result.MachineId.ShouldBe(880);
        result.PlcId.ShouldBe(88);
        result.Name.ShouldBe("PCB_InspectionResult");
        result.Description.ShouldBe("PCB quality inspection result");
        result.Alias.ShouldBe("PCB_QUAL");
        result.Address.ShouldBe("DM1000");
        result.NetType.ShouldBe("DINT");
        result.Length.ShouldBe(4);
        result.IsActive.ShouldBe(1);
        result.Direction.ShouldBe(0);
        result.VariableGroupId.ShouldBe(88);
        result.VariableSpecId.ShouldBe(8801);
        result.TagStatus.ShouldBe(1);
        result.NativeType.ShouldBe("DWORD");
        result.Value.ShouldBe("1");
        result.NativeAddress.ShouldBe("DM1000");
    }

    /// <summary>
    /// Executes ToEntity_WithNullVariablesView_ShouldReturnFailureResult operation.
    /// </summary>

    [Fact]
    public void ToEntity_WithNullVariablesView_ShouldReturnFailureResult()
    {
        // Arrange
        VariablesView nullVariablesView = null!;

        // Act
        var result = VariablesView.ToEntity(nullVariablesView);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Errors.ShouldNotBeNull();
        result.Errors.ShouldContain("VariablesView source cannot be null");
    }

    /// <summary>
    /// Executes Properties_WithPharmaceuticalManufacturingScenario_ShouldHandleComplexConfiguration operation.
    /// </summary>

    [Fact]
    public void Properties_WithPharmaceuticalManufacturingScenario_ShouldHandleComplexConfiguration()
    {
        // Arrange - Pharmaceutical tablet production with Siemens S7-1518F
        var instance = new VariablesView();

        // Act - cGMP compliant pharmaceutical manufacturing
        instance.VariableId = 9901;
        instance.MachineId = 990;
        instance.PlcId = 99;
        instance.Name = "TabletWeightSensor";
        instance.Description = "Tablet weight measurement for quality control";
        instance.Alias = "TAB_WEIGHT";
        instance.Address = "DB10.DBD100";
        instance.NetType = "REAL";
        instance.Length = 4;
        instance.IsActive = 1;
        instance.Direction = 0; // Read-only for safety
        instance.VariableGroupId = 99;
        instance.VariableSpecId = 9901;
        instance.TagStatus = 1;
        instance.NativeType = "REAL";
        instance.Value = "325.2"; // mg
        instance.NativeAddress = "MD100";

        // Assert
        instance.VariableId.ShouldBe(9901);
        instance.MachineId.ShouldBe(990);
        instance.PlcId.ShouldBe(99);
        instance.Name.ShouldBe("TabletWeightSensor");
        instance.Description.ShouldBe("Tablet weight measurement for quality control");
        instance.Alias.ShouldBe("TAB_WEIGHT");
        instance.Address.ShouldBe("DB10.DBD100");
        instance.NetType.ShouldBe("REAL");
        instance.Length.ShouldBe(4);
        instance.IsActive.ShouldBe(1);
        instance.Direction.ShouldBe(0);
        instance.VariableGroupId.ShouldBe(99);
        instance.VariableSpecId.ShouldBe(9901);
        instance.TagStatus.ShouldBe(1);
        instance.NativeType.ShouldBe("REAL");
        instance.Value.ShouldBe("325.2");
        instance.NativeAddress.ShouldBe("MD100");
        instance.EnumValue.ShouldBe("325.2"); // Non-enum variable returns original value
    }

    /// <summary>
    /// Executes RoundTrip_Conversion_ShouldMaintainDataIntegrity operation.
    /// </summary>

    [Fact]
    public void RoundTrip_Conversion_ShouldMaintainDataIntegrity()
    {
        // Arrange - Original Variable entity
        var originalVariable = new Variable
        {
            VariableId = 7701,
            MachineId = 770,
            PlcId = 77,
            Name = "MachineTypePlc",
            Description = "Machine type indicator",
            Alias = "MACH_TYPE",
            Address = "DB5.DBW50",
            NetType = "INT",
            Length = 2,
            IsActive = 1,
            Direction = 0,
            VariableGroupId = 77,
            VariableSpecId = 7701,
            TagStatus = 1,
            NativeType = "INT",
            Value = "8",
            NativeAddress = "MW50"
        };

        // Act - Convert to DTO and back to Entity
        var dtoWrapper = VariablesView.ToDto(originalVariable);
        dtoWrapper.IsSuccess.ShouldBeTrue();
        dtoWrapper.Value.ShouldNotBeNull();
        var dto = dtoWrapper.Value;

        var convertedVariableWrapper = VariablesView.ToEntity(dto);
        convertedVariableWrapper.IsSuccess.ShouldBeTrue();
        convertedVariableWrapper.Value.ShouldNotBeNull();
        var convertedVariable = convertedVariableWrapper.Value;
        convertedVariable.ShouldNotBeNull();
        convertedVariable.ShouldNotBeNull();

        // Assert - Verify data integrity
        convertedVariable.VariableId.ShouldBe(originalVariable.VariableId);
        convertedVariable.MachineId.ShouldBe(originalVariable.MachineId);
        convertedVariable.PlcId.ShouldBe(originalVariable.PlcId);
        convertedVariable.Name.ShouldBe(originalVariable.Name);
        convertedVariable.Description.ShouldBe(originalVariable.Description);
        convertedVariable.Alias.ShouldBe(originalVariable.Alias);
        convertedVariable.Address.ShouldBe(originalVariable.Address);
        convertedVariable.NetType.ShouldBe(originalVariable.NetType);
        convertedVariable.Length.ShouldBe(originalVariable.Length);
        convertedVariable.IsActive.ShouldBe(originalVariable.IsActive);
        convertedVariable.Direction.ShouldBe(originalVariable.Direction);
        convertedVariable.VariableGroupId.ShouldBe(originalVariable.VariableGroupId);
        convertedVariable.VariableSpecId.ShouldBe(originalVariable.VariableSpecId);
        convertedVariable.TagStatus.ShouldBe(originalVariable.TagStatus);
        convertedVariable.NativeType.ShouldBe(originalVariable.NativeType);
        convertedVariable.Value.ShouldBe(originalVariable.Value);
        convertedVariable.NativeAddress.ShouldBe(originalVariable.NativeAddress);

        // Verify enum conversion worked
        dto.EnumValue.ShouldBe("Process");
    }
}
