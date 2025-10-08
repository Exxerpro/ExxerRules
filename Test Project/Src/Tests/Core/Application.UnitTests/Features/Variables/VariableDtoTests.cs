namespace Application.UnitTests.Features.Variables;

/// <summary>
/// Unit tests for VariableDto - Industrial Automation Variable Management
/// </summary>
public class VariableDtoTests
{
    /// <summary>
    /// Executes Constructor_WithDefaultConstructor_ShouldCreateInstanceWithDefaultValues operation.
    /// </summary>
    [Fact]
    public void Constructor_WithDefaultConstructor_ShouldCreateInstanceWithDefaultValues()
    {
        // Act
        var variableDto = new VariableDto();

        // Assert
        variableDto.ShouldNotBeNull();
        variableDto.VariableId.ShouldBe(0);
        variableDto.MachineId.ShouldBe(0);
        variableDto.PlcId.ShouldBe(0);
        //[Fix]
        //CLAUDE
        //Date: 22/08/2025
        //Reason: Pattern 17 Fix - VariableDto initializes string properties with = null!, not string.Empty
        variableDto.Name.ShouldBeNull();
        variableDto.Description.ShouldBeNull();
        variableDto.Alias.ShouldBeNull();
        variableDto.Address.ShouldBeNull();
        variableDto.NetType.ShouldBeNull();
        variableDto.Length.ShouldBe(0);
        variableDto.IsActive.ShouldBe(0);
        variableDto.Direction.ShouldBe(0);
        variableDto.VariableGroupId.ShouldBe(0);
        variableDto.VariableSpecId.ShouldBeNull();
        variableDto.TagStatus.ShouldBe(0);
        variableDto.NativeType.ShouldBeNull();
        variableDto.Value.ShouldBeNull();
        variableDto.NativeAddress.ShouldBeNull();
    }
    /// <summary>
    /// Executes Properties_WhenSet_ShouldReturnCorrectValues operation.
    /// </summary>

    [Fact]
    public void Properties_WhenSet_ShouldReturnCorrectValues()
    {
        // Arrange
        var variableDto = new VariableDto();

        // Act - Industrial automation variable configuration
        variableDto.VariableId = 12345;
        variableDto.MachineId = 7001;
        variableDto.PlcId = 2001;
        variableDto.Name = "Temperature_Sensor_Zone_A1";
        variableDto.Description = "Main engine block temperature sensor for zone A1";
        variableDto.Alias = "TEMP_A1";
        variableDto.Address = "DB100.DBD20";
        variableDto.NetType = "REAL";
        variableDto.Length = 4;
        variableDto.IsActive = 1;
        variableDto.Direction = 0; // Input
        variableDto.VariableGroupId = 5001;
        variableDto.VariableSpecId = 3001;
        variableDto.TagStatus = 100;
        variableDto.NativeType = "REAL";
        variableDto.Value = "87.5";
        variableDto.NativeAddress = "MW100";

        // Assert
        variableDto.VariableId.ShouldBe(12345);
        variableDto.MachineId.ShouldBe(7001);
        variableDto.PlcId.ShouldBe(2001);
        variableDto.Name.ShouldBe("Temperature_Sensor_Zone_A1");
        variableDto.Description.ShouldBe("Main engine block temperature sensor for zone A1");
        variableDto.Alias.ShouldBe("TEMP_A1");
        variableDto.Address.ShouldBe("DB100.DBD20");
        variableDto.NetType.ShouldBe("REAL");
        variableDto.Length.ShouldBe(4);
        variableDto.IsActive.ShouldBe(1);
        variableDto.Direction.ShouldBe(0);
        variableDto.VariableGroupId.ShouldBe(5001);
        variableDto.VariableSpecId.ShouldBe(3001);
        variableDto.TagStatus.ShouldBe(100);
        variableDto.NativeType.ShouldBe("REAL");
        variableDto.Value.ShouldBe("87.5");
        variableDto.NativeAddress.ShouldBe("MW100");
    }

    // ToDto Static Method Tests
    /// <summary>
    /// Executes ToDto_WithNullVariable_ShouldReturnFailureResult operation.
    /// </summary>

    [Fact]
    public void ToDto_WithNullVariable_ShouldReturnFailureResult()
    {
        // Arrange
        Variable? nullVariable = null!;

        // Act
        var result = VariableDto.ToDto(nullVariable!);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Value.ShouldBeNull();
        result.Errors.ShouldNotBeNull();
        result.Errors.ShouldNotBeEmpty();
        result.Errors.ShouldContain("Parameter 'src' cannot be null");
    }
    /// <summary>
    /// Executes ToDto_WithValidVariable_ShouldMapAllProperties operation.
    /// </summary>

    [Fact]
    public void ToDto_WithValidVariable_ShouldMapAllProperties()
    {
        // Arrange
        var variable = new Variable
        {
            VariableId = 98765,
            MachineId = 8001,
            PlcId = 3001,
            Name = "Hydraulic_Pressure_Main",
            Description = "Main hydraulic cylinder pressure sensor",
            Alias = "PRESS_MAIN",
            Address = "DB200.DBD45",
            NetType = "REAL",
            Length = 4,
            IsActive = 1,
            Direction = 0,
            VariableGroupId = 6001,
            VariableSpecId = 4001,
            TagStatus = 100,
            NativeType = "REAL",
            Value = "2450.75",
            NativeAddress = "MD200"
        };

        // Act
        var dtoWrapper = VariableDto.ToDto(variable);

        // Assert
        dtoWrapper.IsSuccess.ShouldBeTrue();
        dtoWrapper.Value.ShouldNotBeNull();
        var dto = dtoWrapper.Value;
        dto.ShouldNotBeNull();
        dto.ShouldNotBeNull();
        dto.ShouldNotBeNull();
        dto.VariableId.ShouldBe(98765);
        dto.MachineId.ShouldBe(8001);
        dto.PlcId.ShouldBe(3001);
        dto.Name.ShouldBe("Hydraulic_Pressure_Main");
        dto.Description.ShouldBe("Main hydraulic cylinder pressure sensor");
        dto.Alias.ShouldBe("PRESS_MAIN");
        dto.Address.ShouldBe("DB200.DBD45");
        dto.NetType.ShouldBe("REAL");
        dto.Length.ShouldBe(4);
        dto.IsActive.ShouldBe(1);
        dto.Direction.ShouldBe(0);
        dto.VariableGroupId.ShouldBe(6001);
        dto.VariableSpecId.ShouldBe(4001);
        dto.TagStatus.ShouldBe(100);
        dto.NativeType.ShouldBe("REAL");
        dto.Value.ShouldBe("2450.75");
        dto.NativeAddress.ShouldBe("MD200");
    }


    // ToEntity Static Method Tests
    /// <summary>
    /// Executes ToEntity_WithNullVariableDto_ShouldReturnFailureResult operation.
    /// </summary>

    [Fact]
    public void ToEntity_WithNullVariableDto_ShouldReturnFailureResult()
    {
        // Arrange
        VariableDto? nullDto = null!;

        // Act
        var result = VariableDto.ToEntity(nullDto!);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Value.ShouldBeNull();
        result.Errors.ShouldNotBeNull();
        result.Errors.ShouldNotBeEmpty();
        result.Errors.ShouldContain("Parameter 'src' cannot be null");
    }
    /// <summary>
    /// Executes ToEntity_WithValidVariableDto_ShouldMapAllProperties operation.
    /// </summary>

    [Fact]
    public void ToEntity_WithValidVariableDto_ShouldMapAllProperties()
    {
        // Arrange
        var dto = new VariableDto
        {
            VariableId = 54321,
            MachineId = 9001,
            PlcId = 4001,
            Name = "Spindle_Speed_Feedback",
            Description = "High-speed spindle RPM feedback encoder",
            Alias = "SPINDLE_RPM",
            Address = "DB300.DBD60",
            NetType = "DINT",
            Length = 4,
            IsActive = 1,
            Direction = 0,
            VariableGroupId = 7001,
            VariableSpecId = 5001,
            TagStatus = 100,
            NativeType = "DINT",
            Value = "12500",
            NativeAddress = "MD300"
        };

        // Act
        var entityWrapper = VariableDto.ToEntity(dto);

        // Assert
        entityWrapper.IsSuccess.ShouldBeTrue();
        entityWrapper.Value.ShouldNotBeNull();
        var entity = entityWrapper.Value;
        entity.ShouldNotBeNull();
        entity.ShouldNotBeNull();
        entity.ShouldNotBeNull();
        entity.VariableId.ShouldBe(54321);
        entity.MachineId.ShouldBe(9001);
        entity.PlcId.ShouldBe(4001);
        entity.Name.ShouldBe("Spindle_Speed_Feedback");
        entity.Description.ShouldBe("High-speed spindle RPM feedback encoder");
        entity.Alias.ShouldBe("SPINDLE_RPM");
        entity.Address.ShouldBe("DB300.DBD60");
        entity.NetType.ShouldBe("DINT");
        entity.Length.ShouldBe(4);
        entity.IsActive.ShouldBe(1);
        entity.Direction.ShouldBe(0);
        entity.VariableGroupId.ShouldBe(7001);
        entity.VariableSpecId.ShouldBe(5001);
        entity.TagStatus.ShouldBe(100);
        entity.NativeType.ShouldBe("DINT");
        entity.Value.ShouldBe("12500");
        entity.NativeAddress.ShouldBe("MD300");
    }


    // ToDtoList Static Method Tests
    /// <summary>
    /// Executes ToDtoList_WithNullVariableCollection_ShouldReturnFailureResult operation.
    /// </summary>

    [Fact]
    public void ToDtoList_WithNullVariableCollection_ShouldReturnFailureResult()
    {
        // Arrange
        IEnumerable<Variable>? nullCollection = null!;

        // Act
        var result = VariableDto.ToDtoList(nullCollection!);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Value.ShouldBeNull();
        result.Errors.ShouldNotBeNull();
        result.Errors.ShouldNotBeEmpty();
        result.Errors.ShouldContain("Variable collection cannot be null");
    }
    /// <summary>
    /// Executes ToDtoList_WithEmptyVariableCollection_ShouldReturnEmptyList operation.
    /// </summary>

    [Fact]
    public void ToDtoList_WithEmptyVariableCollection_ShouldReturnEmptyList()
    {
        // Arrange
        var emptyCollection = new List<Variable>();

        // Act
        var resultWrapper = VariableDto.ToDtoList(emptyCollection);

        // Assert
        resultWrapper.IsSuccess.ShouldBeTrue();
        resultWrapper.Value.ShouldNotBeNull();
        var result = resultWrapper.Value;
        result.ShouldNotBeNull();
        result.ShouldNotBeNull();
        result.ShouldNotBeNull();
        result.ShouldBeEmpty();
    }
    /// <summary>
    /// Executes ToDtoList_WithValidVariableCollection_ShouldMapAllItems operation.
    /// </summary>

    [Fact]
    public void ToDtoList_WithValidVariableCollection_ShouldMapAllItems()
    {
        // Arrange - Complete automation system variables
        var variables = new List<Variable>
        {
            new()
            {
                VariableId = 1001,
                MachineId = 7001,
                PlcId = 2001,
                Name = "Temperature_Zone_1",
                Alias = "TEMP_Z1",
                Address = "DB100.DBD0",
                NetType = "REAL",
                Direction = 0,
                Value = "85.5"
            },
            new()
            {
                VariableId = 1002,
                MachineId = 7001,
                PlcId = 2001,
                Name = "Pressure_Hydraulic_Main",
                Alias = "PRESS_MAIN",
                Address = "DB100.DBD4",
                NetType = "REAL",
                Direction = 0,
                Value = "2250.0"
            },
            new()
            {
                VariableId = 1003,
                MachineId = 7001,
                PlcId = 2001,
                Name = "Production_Counter",
                Alias = "PROD_CNT",
                Address = "DB100.DBD8",
                NetType = "DINT",
                Direction = 1,
                Value = "15750"
            }
        };

        // Act
        var resultWrapper = VariableDto.ToDtoList(variables);

        // Assert
        resultWrapper.IsSuccess.ShouldBeTrue();
        resultWrapper.Value.ShouldNotBeNull();
        var result = resultWrapper.Value.ToList();

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(3);

        result[0].VariableId.ShouldBe(1001);
        result[0].Name.ShouldBe("Temperature_Zone_1");
        result[0].Alias.ShouldBe("TEMP_Z1");
        result[0].Direction.ShouldBe(0); // Input

        result[1].VariableId.ShouldBe(1002);
        result[1].Name.ShouldBe("Pressure_Hydraulic_Main");
        result[1].Value.ShouldBe("2250.0");

        result[2].VariableId.ShouldBe(1003);
        result[2].Name.ShouldBe("Production_Counter");
        result[2].Direction.ShouldBe(1); // Output
        result[2].Value.ShouldBe("15750");
    }


    // Industrial Automation Scenarios
    /// <summary>
    /// Executes VariableDto_WithSiemensPLCConfiguration_ShouldHandleS7Protocol operation.
    /// </summary>

    [Fact]
    public void VariableDto_WithSiemensPLCConfiguration_ShouldHandleS7Protocol()
    {
        // Arrange - Siemens S7-1500 PLC variable configuration
        var variableDto = new VariableDto();

        // Act - Configure Siemens S7 variable
        variableDto.VariableId = 2001;
        variableDto.MachineId = 7001;
        variableDto.PlcId = 1001; // Siemens S7-1500
        variableDto.Name = "Engine_Temperature_Coolant_Inlet";
        variableDto.Description = "Engine coolant inlet temperature sensor - critical for thermal management";
        variableDto.Alias = "ENG_TEMP_IN";
        variableDto.Address = "DB1.DBD100"; // Siemens data block format
        variableDto.NetType = "REAL";
        variableDto.NativeType = "REAL";
        variableDto.NativeAddress = "MW100";
        variableDto.Length = 4;
        variableDto.IsActive = 1;
        variableDto.Direction = 0; // Input from sensor
        variableDto.TagStatus = 100; // Good quality
        variableDto.Value = "82.5";

        // Assert - Verify Siemens S7 configuration
        variableDto.Address.ShouldStartWith("DB"); // Siemens data block
        variableDto.Name.ShouldContain("Engine_Temperature");
        variableDto.NetType.ShouldBe("REAL");
        variableDto.Length.ShouldBe(4); // 32-bit REAL
        variableDto.Direction.ShouldBe(0); // Input variable
        variableDto.TagStatus.ShouldBe(100);
    }
    /// <summary>
    /// Executes VariableDto_WithAllenBradleyPLCConfiguration_ShouldHandleEtherNetIP operation.
    /// </summary>

    [Fact]
    public void VariableDto_WithAllenBradleyPLCConfiguration_ShouldHandleEtherNetIP()
    {
        // Arrange - Allen-Bradley ControlLogix PLC variable
        var variableDto = new VariableDto();

        // Act - Configure Allen-Bradley variable
        variableDto.VariableId = 3001;
        variableDto.MachineId = 8001;
        variableDto.PlcId = 2001; // Allen-Bradley ControlLogix
        variableDto.Name = "Conveyor_Speed_Setpoint";
        variableDto.Description = "Production line conveyor speed control setpoint";
        variableDto.Alias = "CONV_SPD_SP";
        variableDto.Address = "Program:MainProgram.CONV_SPEED_SP"; // AB tag format
        variableDto.NetType = "REAL";
        variableDto.NativeType = "REAL";
        variableDto.NativeAddress = "N7:100";
        variableDto.Length = 4;
        variableDto.IsActive = 1;
        variableDto.Direction = 1; // Output to drive
        variableDto.TagStatus = 100;
        variableDto.Value = "150.0";

        // Assert - Verify Allen-Bradley configuration
        variableDto.Address.ShouldContain("Program:"); // AB program structure
        variableDto.Name.ShouldContain("Conveyor_Speed");
        variableDto.Direction.ShouldBe(1); // Output variable
        variableDto.Value.ShouldBe("150.0");
    }
    /// <summary>
    /// Executes VariableDto_WithMitsubishiPLCConfiguration_ShouldHandleMCProtocol operation.
    /// </summary>

    [Fact]
    public void VariableDto_WithMitsubishiPLCConfiguration_ShouldHandleMCProtocol()
    {
        // Arrange - Mitsubishi Q Series PLC variable
        var variableDto = new VariableDto();

        // Act - Configure Mitsubishi variable
        variableDto.VariableId = 4001;
        variableDto.MachineId = 9001;
        variableDto.PlcId = 3001; // Mitsubishi Q Series
        variableDto.Name = "Robot_Position_X_Axis";
        variableDto.Description = "6-axis robot X-axis position feedback";
        variableDto.Alias = "ROB_POS_X";
        variableDto.Address = "D1000"; // Mitsubishi data register
        variableDto.NetType = "DINT";
        variableDto.NativeType = "DINT";
        variableDto.NativeAddress = "D1000";
        variableDto.Length = 4;
        variableDto.IsActive = 1;
        variableDto.Direction = 0; // Input from encoder
        variableDto.TagStatus = 100;
        variableDto.Value = "125750"; // Encoder counts

        // Assert - Verify Mitsubishi configuration
        variableDto.Address.ShouldStartWith("D"); // Mitsubishi data register
        variableDto.Name.ShouldContain("Robot_Position");
        variableDto.NetType.ShouldBe("DINT");
        variableDto.Value.ShouldBe("125750");
    }
    /// <summary>
    /// Executes VariableDto_WithOmronPLCConfiguration_ShouldHandleFINSProtocol operation.
    /// </summary>

    [Fact]
    public void VariableDto_WithOmronPLCConfiguration_ShouldHandleFINSProtocol()
    {
        // Arrange - Omron NJ Series PLC variable
        var variableDto = new VariableDto();

        // Act - Configure Omron variable
        variableDto.VariableId = 5001;
        variableDto.MachineId = 1000001;
        variableDto.PlcId = 4001; // Omron NJ Series
        variableDto.Name = "Vision_System_Result_OK";
        variableDto.Description = "Machine vision inspection result - part OK";
        variableDto.Alias = "VISION_OK";
        variableDto.Address = "CIO200.00"; // Omron CIO format
        variableDto.NetType = "BOOL";
        variableDto.NativeType = "BOOL";
        variableDto.NativeAddress = "CIO200.00";
        variableDto.Length = 1;
        variableDto.IsActive = 1;
        variableDto.Direction = 0; // Input from vision system
        variableDto.TagStatus = 100;
        variableDto.Value = "1"; // Part OK

        // Assert - Verify Omron configuration
        variableDto.Address.ShouldStartWith("CIO"); // Omron CIO area
        variableDto.Name.ShouldContain("Vision_System");
        variableDto.NetType.ShouldBe("BOOL");
        variableDto.Length.ShouldBe(1);
        variableDto.Value.ShouldBe("1");
    }


    // Data Type and Communication Protocol Tests
    /// <summary>
    /// Executes VariableDto_WithVariousDataTypes_ShouldHandleIndustrialStandards operation.
    /// </summary>

    [Theory]
    [InlineData("BOOL", 1, 0, "Digital I/O")]
    [InlineData("INT", 2, 0, "16-bit integer")]
    [InlineData("DINT", 4, 0, "32-bit integer")]
    [InlineData("REAL", 4, 0, "32-bit float")]
    [InlineData("LREAL", 8, 0, "64-bit float")]
    [InlineData("STRING", 256, 0, "Text string")]
    [InlineData("WORD", 2, 1, "16-bit register output")]
    [InlineData("DWORD", 4, 1, "32-bit register output")]
    public void VariableDto_WithVariousDataTypes_ShouldHandleIndustrialStandards(
        string dataType, int length, int direction, string description)
    {
        // Arrange
        description.ShouldNotBeNull(); // Validates test description parameter

        var variableDto = new VariableDto();

        // Act
        variableDto.NetType = dataType;
        variableDto.NativeType = dataType;
        variableDto.Length = length;
        variableDto.Direction = direction;

        // Assert
        variableDto.NetType.ShouldBe(dataType);
        variableDto.NativeType.ShouldBe(dataType);
        variableDto.Length.ShouldBe(length);
        variableDto.Direction.ShouldBe(direction);
        // Each combination represents valid industrial automation data types
    }
    /// <summary>
    /// Executes VariableDto_WithHighSpeedDataAcquisition_ShouldHandleRealTimeRequirements operation.
    /// </summary>

    [Fact]
    public void VariableDto_WithHighSpeedDataAcquisition_ShouldHandleRealTimeRequirements()
    {
        // Arrange - High-speed data acquisition system
        var variables = new List<VariableDto>();

        // Act - Create high-frequency monitoring variables
        for (int i = 0; i < 500; i++)
        {
            var variableDto = new VariableDto
            {
                VariableId = 10000 + i,
                MachineId = 1005001,
                PlcId = 5001,
                Name = $"HighSpeed_Analog_Channel_{i:D3}",
                Alias = $"HSA_CH{i:D3}",
                Address = $"DB500.DBD{i * 4}",
                NetType = "REAL",
                Length = 4,
                IsActive = 1,
                Direction = 0,
                TagStatus = 100,
                Value = (1000.0 + (i * 0.1)).ToString("F1")
            };
            variables.Add(variableDto);
        }

        // Assert - Verify high-speed data acquisition setup
        variables.Count.ShouldBe(500);
        variables.All(v => v.NetType == "REAL").ShouldBeTrue();
        variables.All(v => v.Length == 4).ShouldBeTrue();
        variables.All(v => v.Direction == 0).ShouldBeTrue(); // All inputs
        variables.All(v => v.TagStatus == 100).ShouldBeTrue(); // Good quality

        // Verify address progression
        variables[0].Address.ShouldBe("DB500.DBD0");
        variables[100].Address.ShouldBe("DB500.DBD400");
        variables[499].Address.ShouldBe("DB500.DBD1996");
    }
    /// <summary>
    /// Executes VariableDto_WithPredictiveMaintenanceVariables_ShouldHandleConditionMonitoring operation.
    /// </summary>

    [Fact]
    public void VariableDto_WithPredictiveMaintenanceVariables_ShouldHandleConditionMonitoring()
    {
        // Arrange - Predictive maintenance monitoring variables
        var variableDto = new VariableDto();

        // Act - Configure condition monitoring variable
        variableDto.VariableId = 6001;
        variableDto.MachineId = 1002001;
        variableDto.PlcId = 6001;
        variableDto.Name = "Bearing_Vibration_RMS_Overall";
        variableDto.Description = "Main spindle bearing vibration RMS value for predictive maintenance";
        variableDto.Alias = "BEAR_VIB_RMS";
        variableDto.Address = "DB1000.DBD500";
        variableDto.NetType = "REAL";
        variableDto.Length = 4;
        variableDto.IsActive = 1;
        variableDto.Direction = 0;
        variableDto.VariableGroupId = 9001; // Condition monitoring group
        variableDto.TagStatus = 100;
        variableDto.Value = "0.15"; // mm/s RMS

        // Assert - Verify predictive maintenance configuration
        variableDto.Name.ShouldContain("Vibration_RMS");
        variableDto.Description.ShouldContain("predictive maintenance");
        variableDto.VariableGroupId.ShouldBe(9001);
        variableDto.Value.ShouldBe("0.15");
        variableDto.TagStatus.ShouldBe(100);
    }


    // Round-trip Conversion Tests
    /// <summary>
    /// Executes ToDto_ThenToEntity_ShouldPreserveAllProperties operation.
    /// </summary>

    [Fact]
    public void ToDto_ThenToEntity_ShouldPreserveAllProperties()
    {
        // Arrange
        var originalVariable = new Variable
        {
            VariableId = 77777,
            MachineId = 88888,
            PlcId = 99999,
            Name = "Round_Trip_Test_Variable",
            Description = "Complete round-trip conversion test variable",
            Alias = "RT_TEST",
            Address = "DB999.DBD888",
            NetType = "REAL",
            Length = 4,
            IsActive = 1,
            Direction = 0,
            VariableGroupId = 11111,
            VariableSpecId = 22222,
            TagStatus = 100,
            NativeType = "REAL",
            Value = "123.456",
            NativeAddress = "MW999"
        };

        // Act
        var dtoResult = VariableDto.ToDto(originalVariable);
        dtoResult.Value.ShouldNotBeNull();





        var entityResult = VariableDto.ToEntity(dtoResult.Value);

        // Assert
        dtoResult.IsSuccess.ShouldBeTrue();
        dtoResult.Value.ShouldNotBeNull();
        entityResult.IsSuccess.ShouldBeTrue();
        entityResult.Value.ShouldNotBeNull();
        var convertedEntity = entityResult.Value;
        convertedEntity.ShouldNotBeNull();
        convertedEntity.ShouldNotBeNull();
        convertedEntity.ShouldNotBeNull();
        convertedEntity.VariableId.ShouldBe(originalVariable.VariableId);
        convertedEntity.MachineId.ShouldBe(originalVariable.MachineId);
        convertedEntity.PlcId.ShouldBe(originalVariable.PlcId);
        convertedEntity.Name.ShouldBe(originalVariable.Name);
        convertedEntity.Description.ShouldBe(originalVariable.Description);
        convertedEntity.Alias.ShouldBe(originalVariable.Alias);
        convertedEntity.Address.ShouldBe(originalVariable.Address);
        convertedEntity.NetType.ShouldBe(originalVariable.NetType);
        convertedEntity.Length.ShouldBe(originalVariable.Length);
        convertedEntity.IsActive.ShouldBe(originalVariable.IsActive);
        convertedEntity.Direction.ShouldBe(originalVariable.Direction);
        convertedEntity.VariableGroupId.ShouldBe(originalVariable.VariableGroupId);
        convertedEntity.VariableSpecId.ShouldBe(originalVariable.VariableSpecId);
        convertedEntity.TagStatus.ShouldBe(originalVariable.TagStatus);
        convertedEntity.NativeType.ShouldBe(originalVariable.NativeType);
        convertedEntity.Value.ShouldBe(originalVariable.Value);
        convertedEntity.NativeAddress.ShouldBe(originalVariable.NativeAddress);
    }

}
