namespace IndTrace.Domain.UnitTests.MachinesTests;

/// <summary>
/// Unit tests for MachinePlc domain entity
/// </summary>
public class MachinePlcTests
{
    /// <summary>
    /// Executes MachinePlc_Constructor_Default_ShouldCreateInstanceWithDefaultValues operation.
    /// </summary>
    [Fact]
    public void MachinePlc_Constructor_Default_ShouldCreateInstanceWithDefaultValues()
    {
        // Arrange & Act
        var machinePlc = new MachinePlc();

        // Assert
        machinePlc.ShouldNotBeNull();
        machinePlc.MachineId.ShouldBe(0);
        machinePlc.PlcId.ShouldBe(0);
        machinePlc.IsActive.ShouldBe(1);
    }

    /// <summary>
    /// Executes MachinePlc_WithAllRequiredProperties_ShouldCreateInstanceSuccessfully operation.
    /// </summary>

    [Fact]
    public void MachinePlc_WithAllRequiredProperties_ShouldCreateInstanceSuccessfully()
    {
        // Arrange
        var machineId = 100;
        var plcId = 200;
        var isActive = 1;

        // Act
        var machinePlc = new MachinePlc(machineId, plcId, isActive);

        // Assert
        machinePlc.ShouldNotBeNull();
        machinePlc.MachineId.ShouldBe(machineId);
        machinePlc.PlcId.ShouldBe(plcId);
        machinePlc.IsActive.ShouldBe(isActive);
    }

    /// <summary>
    /// Executes MachinePlc_WhenPropertiesAssigned_ShouldMaintainAllValues operation.
    /// </summary>

    [Fact]
    public void MachinePlc_WhenPropertiesAssigned_ShouldMaintainAllValues()
    {
        // Arrange
        var machinePlc = new MachinePlc();
        var machineId = 150;
        var plcId = 250;
        var isActive = 1;

        // Act
        machinePlc.MachineId = machineId;
        machinePlc.PlcId = plcId;
        machinePlc.IsActive = isActive;

        // Assert
        machinePlc.MachineId.ShouldBe(machineId);
        machinePlc.PlcId.ShouldBe(plcId);
        machinePlc.IsActive.ShouldBe(isActive);
    }

    /// <summary>
    /// Executes MachinePlcProperties_WhenSetToZero_ShouldAcceptZero operation.
    /// </summary>

    [Fact]
    public void MachinePlcProperties_WhenSetToZero_ShouldAcceptZero()
    {
        // Arrange
        var machinePlc = new MachinePlc();

        // Act
        machinePlc.MachineId = 0;
        machinePlc.PlcId = 0;
        machinePlc.IsActive = 0;

        // Assert
        machinePlc.MachineId.ShouldBe(0);
        machinePlc.PlcId.ShouldBe(0);
        machinePlc.IsActive.ShouldBe(0);
    }

    /// <summary>
    /// Executes MachinePlcProperties_WhenSetToNegative_ShouldAcceptNegative operation.
    /// </summary>

    [Fact]
    public void MachinePlcProperties_WhenSetToNegative_ShouldAcceptNegative()
    {
        // Arrange
        var machinePlc = new MachinePlc();

        // Act
        machinePlc.MachineId = -1;
        machinePlc.PlcId = -1;
        machinePlc.IsActive = 1;

        // Assert
        machinePlc.MachineId.ShouldBe(-1);
        machinePlc.PlcId.ShouldBe(-1);
        machinePlc.IsActive.ShouldBe(1);
    }

    /// <summary>
    /// Executes MachinePlcProperties_WhenSetToLargeValues_ShouldAcceptLargeValues operation.
    /// </summary>

    [Fact]
    public void MachinePlcProperties_WhenSetToLargeValues_ShouldAcceptLargeValues()
    {
        // Arrange
        var machinePlc = new MachinePlc();
        var largeValue = int.MaxValue;

        // Act
        machinePlc.MachineId = largeValue;
        machinePlc.PlcId = largeValue;
        machinePlc.IsActive = 1;

        // Assert
        machinePlc.MachineId.ShouldBe(largeValue);
        machinePlc.PlcId.ShouldBe(largeValue);
        machinePlc.IsActive.ShouldBe(1);
    }

    /// <summary>
    /// Executes MachinePlc_WhenMachinePlcIsCreated_ShouldHaveDefaultValues operation.
    /// </summary>

    [Fact]
    public void MachinePlc_WhenMachinePlcIsCreated_ShouldHaveDefaultValues()
    {
        // Arrange & Act
        var machinePlc = new MachinePlc();

        // Assert - Verify business logic defaults
        machinePlc.MachineId.ShouldBe(0, "Machine ID should default to 0");
        machinePlc.PlcId.ShouldBe(0, "PLC ID should default to 0");
        machinePlc.IsActive.ShouldBe(1, "IsActive should default to 1 (active)");
    }

    /// <summary>
    /// Executes MachinePlc_WhenMachinePlcIsConfigured_ShouldBeValid operation.
    /// </summary>

    [Fact]
    public void MachinePlc_WhenMachinePlcIsConfigured_ShouldBeValid()
    {
        // Arrange
        var machinePlc = new MachinePlc
        {
            MachineId = 100,
            PlcId = 100,
            IsActive = 1
        };

        // Act & Assert
        machinePlc.ShouldNotBeNull();
        machinePlc.MachineId.ShouldBe(100);
        machinePlc.PlcId.ShouldBe(100);
        machinePlc.IsActive.ShouldBe(1);
    }

    /// <summary>
    /// Executes MachinePlc_WhenMachinePlcIsActive_ShouldBeValid operation.
    /// </summary>

    [Fact]
    public void MachinePlc_WhenMachinePlcIsActive_ShouldBeValid()
    {
        // Arrange
        var machinePlc = new MachinePlc
        {
            IsActive = 1
        };

        // Act & Assert
        machinePlc.IsActive.ShouldBe(1);
    }

    /// <summary>
    /// Executes MachinePlc_WhenMachinePlcIsInactive_ShouldBeValid operation.
    /// </summary>

    [Fact]
    public void MachinePlc_WhenMachinePlcIsInactive_ShouldBeValid()
    {
        // Arrange
        var machinePlc = new MachinePlc
        {
            IsActive = 0
        };

        // Act & Assert
        machinePlc.IsActive.ShouldBe(0);
    }

    /// <summary>
    /// Executes MachinePlc_WhenMachinePlcHasSameMachineAndPlcIds_ShouldBeValid operation.
    /// </summary>

    [Fact]
    public void MachinePlc_WhenMachinePlcHasSameMachineAndPlcIds_ShouldBeValid()
    {
        // Arrange
        var machinePlc = new MachinePlc
        {
            MachineId = 10000,
            PlcId = 100
        };

        // Act & Assert
        machinePlc.MachineId.ShouldBe(10000);
        machinePlc.PlcId.ShouldBe(100);
    }

    /// <summary>
    /// Executes MachinePlc_WhenMachinePlcHasDifferentMachineAndPlcIds_ShouldBeValid operation.
    /// </summary>

    [Fact]
    public void MachinePlc_WhenMachinePlcHasDifferentMachineAndPlcIds_ShouldBeValid()
    {
        // Arrange
        var machinePlc = new MachinePlc
        {
            MachineId = 10000,
            PlcId = 200
        };

        // Act & Assert
        machinePlc.MachineId.ShouldBe(10000);
        machinePlc.PlcId.ShouldBe(200);
    }

    /// <summary>
    /// Executes MachinePlc_WhenMachinePlcHasZeroMachineId_ShouldBeValid operation.
    /// </summary>

    [Fact]
    public void MachinePlc_WhenMachinePlcHasZeroMachineId_ShouldBeValid()
    {
        // Arrange
        var machinePlc = new MachinePlc
        {
            MachineId = 0,
            PlcId = 100
        };

        // Act & Assert
        machinePlc.MachineId.ShouldBe(0);
        machinePlc.PlcId.ShouldBe(100);
    }

    /// <summary>
    /// Executes MachinePlc_WhenMachinePlcHasZeroPlcId_ShouldBeValid operation.
    /// </summary>

    [Fact]
    public void MachinePlc_WhenMachinePlcHasZeroPlcId_ShouldBeValid()
    {
        // Arrange
        var machinePlc = new MachinePlc
        {
            MachineId = 10000,
            PlcId = 0
        };

        // Act & Assert
        machinePlc.MachineId.ShouldBe(10000);
        machinePlc.PlcId.ShouldBe(0);
    }

    /// <summary>
    /// Executes MachinePlc_WhenMachinePlcHasNegativeMachineId_ShouldBeValid operation.
    /// </summary>

    [Fact]
    public void MachinePlc_WhenMachinePlcHasNegativeMachineId_ShouldBeValid()
    {
        // Arrange
        var machinePlc = new MachinePlc
        {
            MachineId = -1,
            PlcId = 100
        };

        // Act & Assert
        machinePlc.MachineId.ShouldBe(-1);
        machinePlc.PlcId.ShouldBe(100);
    }

    /// <summary>
    /// Executes MachinePlc_WhenMachinePlcHasNegativePlcId_ShouldBeValid operation.
    /// </summary>

    [Fact]
    public void MachinePlc_WhenMachinePlcHasNegativePlcId_ShouldBeValid()
    {
        // Arrange
        var machinePlc = new MachinePlc
        {
            MachineId = 10000,
            PlcId = -1
        };

        // Act & Assert
        machinePlc.MachineId.ShouldBe(10000);
        machinePlc.PlcId.ShouldBe(-1);
    }

    /// <summary>
    /// Executes MachinePlc_WhenMachinePlcHasLargeMachineId_ShouldBeValid operation.
    /// </summary>

    [Fact]
    public void MachinePlc_WhenMachinePlcHasLargeMachineId_ShouldBeValid()
    {
        // Arrange
        var machinePlc = new MachinePlc
        {
            MachineId = int.MaxValue,
            PlcId = 100
        };

        // Act & Assert
        machinePlc.MachineId.ShouldBe(int.MaxValue);
        machinePlc.PlcId.ShouldBe(100);
    }

    /// <summary>
    /// Executes MachinePlc_WhenMachinePlcHasLargePlcId_ShouldBeValid operation.
    /// </summary>

    [Fact]
    public void MachinePlc_WhenMachinePlcHasLargePlcId_ShouldBeValid()
    {
        // Arrange
        var machinePlc = new MachinePlc
        {
            MachineId = 10000,
            PlcId = int.MaxValue
        };

        // Act & Assert
        machinePlc.MachineId.ShouldBe(10000);
        machinePlc.PlcId.ShouldBe(int.MaxValue);
    }

    /// <summary>
    /// Executes MachinePlc_WhenMachinePlcHasLargeIsActive_ShouldBeValid operation.
    /// </summary>

    [Fact]
    public void MachinePlc_WhenMachinePlcHasLargeIsActive_ShouldBeValid()
    {
        // Arrange
        var machinePlc = new MachinePlc
        {
            MachineId = 10000,
            PlcId = 200,
            IsActive = 1
        };

        // Act & Assert
        machinePlc.MachineId.ShouldBe(10000);
        machinePlc.PlcId.ShouldBe(200);
        machinePlc.IsActive.ShouldBe(1);
    }

    /// <summary>
    /// Executes MachinePlc_WhenMachinePlcHasNegativeIsActive_ShouldBeValid operation.
    /// </summary>

    [Fact]
    public void MachinePlc_WhenMachinePlcHasNegativeIsActive_ShouldBeValid()
    {
        // Arrange
        var machinePlc = new MachinePlc
        {
            MachineId = 10000,
            PlcId = 200,
            IsActive = 0
        };

        // Act & Assert
        machinePlc.MachineId.ShouldBe(10000);
        machinePlc.PlcId.ShouldBe(200);
        machinePlc.IsActive.ShouldBe(0);
    }

    /// <summary>
    /// Executes MachinePlc_WhenValidParameters_ShouldSetAllProperties operation.
    /// </summary>

    [Fact]
    public void MachinePlc_WhenValidParameters_ShouldSetAllProperties()
    {
        // Arrange
        var machineId = 500;
        var plcId = 600;
        var isActive = 1;

        // Act
        var machinePlc = new MachinePlc(machineId, plcId, isActive);

        // Assert
        machinePlc.ShouldNotBeNull();
        machinePlc.MachineId.ShouldBe(machineId);
        machinePlc.PlcId.ShouldBe(plcId);
        machinePlc.IsActive.ShouldBe(isActive);
    }

    /// <summary>
    /// Executes MachinePlc_WhenZeroParameters_ShouldSetAllProperties operation.
    /// </summary>

    [Fact]
    public void MachinePlc_WhenZeroParameters_ShouldSetAllProperties()
    {
        // Arrange
        var machineId = 0;
        var plcId = 0;
        var isActive = 0;

        // Act
        var machinePlc = new MachinePlc(machineId, plcId, isActive);

        // Assert
        machinePlc.ShouldNotBeNull();
        machinePlc.MachineId.ShouldBe(machineId);
        machinePlc.PlcId.ShouldBe(plcId);
        machinePlc.IsActive.ShouldBe(isActive);
    }

    /// <summary>
    /// Executes MachinePlc_WhenNegativeParameters_ShouldSetAllProperties operation.
    /// </summary>

    [Fact]
    public void MachinePlc_WhenNegativeParameters_ShouldSetAllProperties()
    {
        // Arrange
        var machineId = -1;
        var plcId = -2;
        var isActive = 1;

        // Act
        var machinePlc = new MachinePlc(machineId, plcId, isActive);

        // Assert
        machinePlc.ShouldNotBeNull();
        machinePlc.MachineId.ShouldBe(machineId);
        machinePlc.PlcId.ShouldBe(plcId);
        machinePlc.IsActive.ShouldBe(isActive);
    }

    /// <summary>
    /// Executes MachinePlc_WhenLargeParameters_ShouldSetAllProperties operation.
    /// </summary>

    [Fact]
    public void MachinePlc_WhenLargeParameters_ShouldSetAllProperties()
    {
        // Arrange
        var machineId = int.MaxValue;
        var plcId = int.MaxValue - 1;
        var isActive = 0;

        // Act
        var machinePlc = new MachinePlc(machineId, plcId, isActive);

        // Assert
        machinePlc.ShouldNotBeNull();
        machinePlc.MachineId.ShouldBe(machineId);
        machinePlc.PlcId.ShouldBe(plcId);
        machinePlc.IsActive.ShouldBe(isActive);
    }
}
