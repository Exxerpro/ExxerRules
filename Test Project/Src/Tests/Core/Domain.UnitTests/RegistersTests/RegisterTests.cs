namespace IndTrace.Domain.UnitTests.RegistersTests;

/// <summary>
/// Unit tests for Register domain entity
/// </summary>
public class RegisterTests
{
    /// <summary>
    /// Executes Register_WithAllRequiredProperties_ShouldCreateInstanceSuccessfully operation.
    /// </summary>
    [Fact]
    public void Register_WithAllRequiredProperties_ShouldCreateInstanceSuccessfully()
    {
        // Arrange
        var registerId = 1;
        var name = "Test Register";
        var description = "Test Register Description";
        var machineId = 100;
        var variableId = 200;
        var cycleId = 300;
        var value = "Test Value";
        var dataType = "String";
        var statusValueId = 400;
        var timeStamp = DateTime.Now;

        // Act
        var register = new Register
        {
            RegisterId = registerId,
            Name = name,
            Description = description,
            MachineId = machineId,
            VariableId = variableId,
            CycleId = cycleId,
            Value = value,
            DataType = dataType,
            StatusValueId = statusValueId,
            TimeStamp = timeStamp
        };

        // Assert
        register.ShouldNotBeNull();
        register.RegisterId.ShouldBe(registerId);
        register.Name.ShouldBe(name);
        register.Description.ShouldBe(description);
        register.MachineId.ShouldBe(machineId);
        register.VariableId.ShouldBe(variableId);
        register.CycleId.ShouldBe(cycleId);
        register.Value.ShouldBe(value);
        register.DataType.ShouldBe(dataType);
        register.StatusValueId.ShouldBe(statusValueId);
        register.TimeStamp.ShouldBe(timeStamp);
    }

    /// <summary>
    /// Executes Register_Constructor_ShouldCreateRegisterWithDefaultValues operation.
    /// </summary>

    [Fact]
    public void Register_Constructor_ShouldCreateRegisterWithDefaultValues()
    {
        // Act
        var register = new Register();

        // Assert
        register.ShouldNotBeNull();
        register.RegisterId.ShouldBe(0);
        register.Name.ShouldBe(string.Empty);
        register.Description.ShouldBe(string.Empty);
        register.MachineId.ShouldBe(0);
        register.VariableId.ShouldBe(0);
        register.CycleId.ShouldBe(0);
        register.Value.ShouldBe(string.Empty);
        register.DataType.ShouldBe(string.Empty);
        register.StatusValueId.ShouldBe(0);
        register.TimeStamp.ShouldBe(default);
    }

    /// <summary>
    /// Executes Register_AllProperties_ShouldBeSettableAndGettable operation.
    /// </summary>

    [Fact]
    public void Register_AllProperties_ShouldBeSettableAndGettable()
    {
        // Arrange
        var register = new Register();
        var now = DateTime.UtcNow;

        // Act
        register.RegisterId = 1;
        register.Name = "Test Register";
        register.Description = "Test Description";
        register.MachineId = 2;
        register.VariableId = 3;
        register.CycleId = 4;
        register.Value = "123.45";
        register.DataType = "float";
        register.StatusValueId = 5;
        register.TimeStamp = now;

        // Assert
        register.RegisterId.ShouldBe(1);
        register.Name.ShouldBe("Test Register");
        register.Description.ShouldBe("Test Description");
        register.MachineId.ShouldBe(2);
        register.VariableId.ShouldBe(3);
        register.CycleId.ShouldBe(4);
        register.Value.ShouldBe("123.45");
        register.DataType.ShouldBe("float");
        register.StatusValueId.ShouldBe(5);
        register.TimeStamp.ShouldBe(now);
    }

    /// <summary>
    /// Executes Register_WithNullAndEmptyStrings_WithEdgeCases_ShouldHandleWithoutErrors operation.
    /// </summary>

    [Fact]
    public void Register_WithNullAndEmptyStrings_WithEdgeCases_ShouldHandleWithoutErrors()
    {
        // Arrange
        var register = new Register
        {
            Name = null!,
            Description = null!,
            Value = null!,
            DataType = null!
        };

        // Act & Assert
        register.Name.ShouldBeNull();
        register.Description.ShouldBeNull();
        register.Value.ShouldBeNull();
        register.DataType.ShouldBeNull();
    }

    /// <summary>
    /// Executes Register_WithNegativeAndLargeValues_WithEdgeCases_ShouldHandleWithoutErrors operation.
    /// </summary>

    [Fact]
    public void Register_WithNegativeAndLargeValues_WithEdgeCases_ShouldHandleWithoutErrors()
    {
        // Arrange
        var register = new Register
        {
            RegisterId = -1,
            MachineId = int.MaxValue,
            VariableId = -2,
            CycleId = int.MinValue,
            StatusValueId = int.MaxValue
        };

        // Act & Assert
        register.RegisterId.ShouldBe(-1);
        register.MachineId.ShouldBe(int.MaxValue);
        register.VariableId.ShouldBe(-2);
        register.CycleId.ShouldBe(int.MinValue);
        register.StatusValueId.ShouldBe(int.MaxValue);
    }

    /// <summary>
    /// Executes Register_WithDefaultConstructor_ShouldInitializeToExpectedDefaults operation.
    /// </summary>

    [Fact]
    public void Register_WithDefaultConstructor_ShouldInitializeToExpectedDefaults()
    {
        // Arrange & Act
        var register = new Register();

        // Assert
        register.ShouldNotBeNull();
        register.RegisterId.ShouldBe(0);
        register.Name.ShouldBe(string.Empty);
        register.Description.ShouldBe(string.Empty);
        register.MachineId.ShouldBe(0);
        register.VariableId.ShouldBe(0);
        register.CycleId.ShouldBe(0);
        register.Value.ShouldBe(string.Empty);
        register.DataType.ShouldBe(string.Empty);
        register.StatusValueId.ShouldBe(0);
        register.TimeStamp.ShouldBe(default);
    }

    /// <summary>
    /// Executes Register_WhenPropertiesAssigned_ShouldMaintainAllValues operation.
    /// </summary>

    [Fact]
    public void Register_WhenPropertiesAssigned_ShouldMaintainAllValues()
    {
        // Arrange
        var register = new Register();
        var registerId = 2;
        var name = "Updated Register";
        var description = "Updated Description";
        var machineId = 150;
        var variableId = 250;
        var cycleId = 350;
        var value = "Updated Value";
        var dataType = "Integer";
        var statusValueId = 450;
        var timeStamp = DateTime.Now.AddHours(1);

        // Act
        register.RegisterId = registerId;
        register.Name = name;
        register.Description = description;
        register.MachineId = machineId;
        register.VariableId = variableId;
        register.CycleId = cycleId;
        register.Value = value;
        register.DataType = dataType;
        register.StatusValueId = statusValueId;
        register.TimeStamp = timeStamp;

        // Assert
        register.RegisterId.ShouldBe(registerId);
        register.Name.ShouldBe(name);
        register.Description.ShouldBe(description);
        register.MachineId.ShouldBe(machineId);
        register.VariableId.ShouldBe(variableId);
        register.CycleId.ShouldBe(cycleId);
        register.Value.ShouldBe(value);
        register.DataType.ShouldBe(dataType);
        register.StatusValueId.ShouldBe(statusValueId);
        register.TimeStamp.ShouldBe(timeStamp);
    }

    /// <summary>
    /// Executes RegisterProperties_WhenSetToNull_ShouldHandleNullValues operation.
    /// </summary>

    [Fact]
    public void RegisterProperties_WhenSetToNull_ShouldHandleNullValues()
    {
        // Arrange
        var register = new Register
        {
            Name = "TEST",
            Description = "TEST DESCRIPTION",
            Value = "TEST VALUE"
        };

        // Act
        register.Name = null!;
        register.Description = null!;
        register.Value = null!;

        // Assert
        register.Name.ShouldBeNull();
        register.Description.ShouldBeNull();
        register.Value.ShouldBeNull();
    }

    /// <summary>
    /// Executes RegisterId_WhenSetToZero_ShouldAcceptZero operation.
    /// </summary>

    [Fact]
    public void RegisterId_WhenSetToZero_ShouldAcceptZero()
    {
        // Arrange
        var register = new Register();

        // Act
        register.RegisterId = 0;

        // Assert
        register.RegisterId.ShouldBe(0);
    }

    /// <summary>
    /// Executes RegisterId_WhenSetToNegative_ShouldAcceptNegative operation.
    /// </summary>

    [Fact]
    public void RegisterId_WhenSetToNegative_ShouldAcceptNegative()
    {
        // Arrange
        var register = new Register();

        // Act
        register.RegisterId = -1;

        // Assert
        register.RegisterId.ShouldBe(-1);
    }

    /// <summary>
    /// Executes Name_WhenSetToEmptyString_ShouldAcceptEmptyString operation.
    /// </summary>

    [Fact]
    public void Name_WhenSetToEmptyString_ShouldAcceptEmptyString()
    {
        // Arrange
        var register = new Register();

        // Act
        register.Name = string.Empty;

        // Assert
        register.Name.ShouldBe(string.Empty);
    }

    /// <summary>
    /// Executes Description_WhenSetToEmptyString_ShouldAcceptEmptyString operation.
    /// </summary>

    [Fact]
    public void Description_WhenSetToEmptyString_ShouldAcceptEmptyString()
    {
        // Arrange
        var register = new Register();

        // Act
        register.Description = string.Empty;

        // Assert
        register.Description.ShouldBe(string.Empty);
    }

    /// <summary>
    /// Executes Value_WhenSetToNull_ShouldAcceptNull operation.
    /// </summary>

    [Fact]
    public void Value_WhenSetToNull_ShouldAcceptNull()
    {
        // Arrange
        var register = new Register();

        // Act
        register.Value = null!;

        // Assert
        register.Value.ShouldBeNull();
    }

    /// <summary>
    /// Executes DataType_WhenSetToEmptyString_ShouldAcceptEmptyString operation.
    /// </summary>

    [Fact]
    public void DataType_WhenSetToEmptyString_ShouldAcceptEmptyString()
    {
        // Arrange
        var register = new Register();

        // Act
        register.DataType = string.Empty;

        // Assert
        register.DataType.ShouldBe(string.Empty);
    }

    /// <summary>
    /// Executes Register_WhenRegisterIsCreated_ShouldHaveDefaultValues operation.
    /// </summary>

    [Fact]
    public void Register_WhenRegisterIsCreated_ShouldHaveDefaultValues()
    {
        // Arrange & Act
        var register = new Register();

        // Assert
        register.ShouldNotBeNull();
        register.RegisterId.ShouldBe(0);
        register.Name.ShouldBe(string.Empty);
        register.Description.ShouldBe(string.Empty);
        register.MachineId.ShouldBe(0);
        register.VariableId.ShouldBe(0);
        register.CycleId.ShouldBe(0);
        register.Value.ShouldBe(string.Empty);
        register.DataType.ShouldBe(string.Empty);
        register.StatusValueId.ShouldBe(0);
        register.TimeStamp.ShouldBe(default);
    }

    /// <summary>
    /// Executes Register_WhenRegisterIsConfigured_ShouldBeValid operation.
    /// </summary>

    [Fact]
    public void Register_WhenRegisterIsConfigured_ShouldBeValid()
    {
        // Arrange
        var timeStamp = DateTime.Now;
        var register = new Register
        {
            RegisterId = 1,
            Name = "Production Register",
            Description = "Production Register Description",
            MachineId = 10000,
            VariableId = 200,
            CycleId = 300,
            Value = "Production Value",
            DataType = "String",
            StatusValueId = 400,
            TimeStamp = timeStamp
        };

        // Act & Assert
        register.ShouldNotBeNull();
        register.RegisterId.ShouldBe(1);
        register.Name.ShouldBe("Production Register");
        register.Description.ShouldBe("Production Register Description");
        register.MachineId.ShouldBe(10000);
        register.VariableId.ShouldBe(200);
        register.CycleId.ShouldBe(300);
        register.Value.ShouldBe("Production Value");
        register.DataType.ShouldBe("String");
        register.StatusValueId.ShouldBe(400);
        register.TimeStamp.ShouldBe(timeStamp);
    }

    /// <summary>
    /// Executes Register_WhenRegisterHasNullValue_ShouldBeValid operation.
    /// </summary>

    [Fact]
    public void Register_WhenRegisterHasNullValue_ShouldBeValid()
    {
        // Arrange
        var register = new Register
        {
            RegisterId = 1,
            Name = "Null Value Register",
            Description = "Register with null value",
            MachineId = 10000,
            VariableId = 200,
            CycleId = 300,
            Value = null!,
            DataType = "String",
            StatusValueId = 400,
            TimeStamp = DateTime.Now
        };

        // Act & Assert
        register.ShouldNotBeNull();
        register.Value.ShouldBeNull();
    }

    /// <summary>
    /// Executes Register_WhenRegisterHasLargeIds_ShouldBeValid operation.
    /// </summary>

    [Fact]
    public void Register_WhenRegisterHasLargeIds_ShouldBeValid()
    {
        // Arrange
        var register = new Register
        {
            RegisterId = 999999,
            Name = "Large ID Register",
            Description = "Register with large IDs",
            MachineId = 999999,
            VariableId = 999999,
            CycleId = 999999,
            Value = "Large ID Value",
            DataType = "Integer",
            StatusValueId = 999999,
            TimeStamp = DateTime.Now
        };

        // Act & Assert
        register.ShouldNotBeNull();
        register.RegisterId.ShouldBe(999999);
        register.MachineId.ShouldBe(999999);
        register.VariableId.ShouldBe(999999);
        register.CycleId.ShouldBe(999999);
        register.StatusValueId.ShouldBe(999999);
    }

    /// <summary>
    /// Executes Register_WhenRegisterHasNegativeIds_ShouldBeValid operation.
    /// </summary>

    [Fact]
    public void Register_WhenRegisterHasNegativeIds_ShouldBeValid()
    {
        // Arrange
        var register = new Register
        {
            RegisterId = -1,
            Name = "Negative ID Register",
            Description = "Register with negative IDs",
            MachineId = -1,
            VariableId = -1,
            CycleId = -1,
            Value = "Negative ID Value",
            DataType = "Integer",
            StatusValueId = -1,
            TimeStamp = DateTime.Now
        };

        // Act & Assert
        register.ShouldNotBeNull();
        register.RegisterId.ShouldBe(-1);
        register.MachineId.ShouldBe(-1);
        register.VariableId.ShouldBe(-1);
        register.CycleId.ShouldBe(-1);
        register.StatusValueId.ShouldBe(-1);
    }
}
