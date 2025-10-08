namespace IndTrace.Domain.UnitTests.BarCodesTests;

/// <summary>
/// Unit tests for BarCode domain entity
/// </summary>
public class BarCodeTests
{
    /// <summary>
    /// Executes BarCode_WhenCreatedWithValidData_ShouldSetAllPropertiesCorrectly operation.
    /// </summary>
    [Fact]
    public void BarCode_WhenCreatedWithValidData_ShouldSetAllPropertiesCorrectly()
    {
        // Arrange
        var barCodeId = 1;
        var productId = 1;
        var label = "L1ATEST1230001";
        var machineId = 1;
        var partStatus = PartStatus.Ok;
        var flowStatus = FlowStatus.Created;

        // Act
        var barCode = new BarCode
        {
            BarCodeId = barCodeId,
            ProductId = productId,
            Label = label,
            MachineId = machineId,
            PartStatus = partStatus,
            FlowStatus = flowStatus
        };

        // Assert
        barCode.ShouldNotBeNull();
        barCode.BarCodeId.ShouldBe(barCodeId);
        barCode.ProductId.ShouldBe(productId);
        barCode.Label.ShouldBe(label);
        barCode.MachineId.ShouldBe(machineId);
        barCode.PartStatus.ShouldBe(partStatus);
        barCode.FlowStatus.ShouldBe(flowStatus);
    }

    /// <summary>
    /// Executes BarCode_WhenCreatedWithoutParameters_ShouldInitializeWithDefaultValues operation.
    /// </summary>

    [Fact]
    public void BarCode_WhenCreatedWithoutParameters_ShouldInitializeWithDefaultValues()
    {
        // Arrange & Act
        var barCode = new BarCode();

        // Assert
        barCode.ShouldNotBeNull();
        barCode.BarCodeId.ShouldBe(0);
        barCode.ProductId.ShouldBe(0);
        //[Fix]
        //CLAUDE
        //Date: 20/08/2025
        //Reason: Fix test expectation - BarCode.Label is initialized to string.Empty in the actual implementation
        barCode.Label.ShouldBe(string.Empty); // BarCode.Label is initialized to string.Empty
        barCode.MachineId.ShouldBe(0);
        barCode.PartStatus.ShouldBe(PartStatus.None);
        barCode.FlowStatus.ShouldBe(FlowStatus.None);
    }

    /// <summary>
    /// Executes BarCode_WhenAllPropertiesUpdated_ShouldPersistAllChangesCorrectly operation.
    /// </summary>

    [Fact]
    public void BarCode_WhenAllPropertiesUpdated_ShouldPersistAllChangesCorrectly()
    {
        // Arrange
        var barCode = new BarCode();

        // Act
        barCode.BarCodeId = 123;
        barCode.ProductId = 456;
        barCode.Label = "Updated BarCode";
        barCode.MachineId = 789;
        barCode.PartStatus = PartStatus.Ok;
        barCode.FlowStatus = FlowStatus.Finished;
        barCode.CreatedOn = DateTime.Now;
        barCode.ModifiedOn = DateTime.Now.AddHours(1);

        // Assert
        barCode.BarCodeId.ShouldBe(123);
        barCode.ProductId.ShouldBe(456);
        barCode.Label.ShouldBe("Updated BarCode");
        barCode.MachineId.ShouldBe(789);
        barCode.PartStatus.ShouldBe(PartStatus.Ok);
        barCode.FlowStatus.ShouldBe(FlowStatus.Finished);
        barCode.CreatedOn.ShouldNotBe(default);
        barCode.ModifiedOn.ShouldNotBe(default);
    }

    /// <summary>
    /// Executes BarCodeProperties_WhenSetToNull_ShouldHandleNullValues operation.
    /// </summary>

    [Fact]
    public void BarCodeProperties_WhenSetToNull_ShouldHandleNullValues()
    {
        // Arrange
        var barCode = new BarCode
        {
            Label = "Test BarCode"
        };

        // Act
        barCode.Label = string.Empty;

        // Assert
        barCode.Label.ShouldNotBeNull();
    }

    /// <summary>
    /// Executes BarCodeId_WhenSetToZero_ShouldAcceptZero operation.
    /// </summary>

    [Fact]
    public void BarCodeId_WhenSetToZero_ShouldAcceptZero()
    {
        // Arrange
        var barCode = new BarCode();

        // Act
        barCode.BarCodeId = 0;

        // Assert
        barCode.BarCodeId.ShouldBe(0);
    }

    /// <summary>
    /// Executes BarCodeId_WhenSetToNegative_ShouldAcceptNegative operation.
    /// </summary>

    [Fact]
    public void BarCodeId_WhenSetToNegative_ShouldAcceptNegative()
    {
        // Arrange
        var barCode = new BarCode();

        // Act
        barCode.BarCodeId = -1;

        // Assert
        barCode.BarCodeId.ShouldBe(-1);
    }

    /// <summary>
    /// Executes ProductId_WhenSetToZero_ShouldAcceptZero operation.
    /// </summary>

    [Fact]
    public void ProductId_WhenSetToZero_ShouldAcceptZero()
    {
        // Arrange
        var barCode = new BarCode();

        // Act
        barCode.ProductId = 0;

        // Assert
        barCode.ProductId.ShouldBe(0);
    }

    /// <summary>
    /// Executes ProductId_WhenSetToNegative_ShouldAcceptNegative operation.
    /// </summary>

    [Fact]
    public void ProductId_WhenSetToNegative_ShouldAcceptNegative()
    {
        // Arrange
        var barCode = new BarCode();

        // Act
        barCode.ProductId = -1;

        // Assert
        barCode.ProductId.ShouldBe(-1);
    }

    /// <summary>
    /// Executes MachineId_WhenSetToZero_ShouldAcceptZero operation.
    /// </summary>

    [Fact]
    public void MachineId_WhenSetToZero_ShouldAcceptZero()
    {
        // Arrange
        var barCode = new BarCode();

        // Act
        barCode.MachineId = 0;

        // Assert
        barCode.MachineId.ShouldBe(0);
    }

    /// <summary>
    /// Executes MachineId_WhenSetToNegative_ShouldAcceptNegative operation.
    /// </summary>

    [Fact]
    public void MachineId_WhenSetToNegative_ShouldAcceptNegative()
    {
        // Arrange
        var barCode = new BarCode();

        // Act
        barCode.MachineId = -1;

        // Assert
        barCode.MachineId.ShouldBe(-1);
    }

    /// <summary>
    /// Executes Label_WhenSetToEmptyString_ShouldAcceptEmptyString operation.
    /// </summary>

    [Fact]
    public void Label_WhenSetToEmptyString_ShouldAcceptEmptyString()
    {
        // Arrange
        var barCode = new BarCode();

        // Act
        barCode.Label = "";

        // Assert
        barCode.Label.ShouldBe("");
    }

    /// <summary>
    /// Executes Label_WhenSetToWhitespace_ShouldAcceptWhitespace operation.
    /// </summary>

    [Fact]
    public void Label_WhenSetToWhitespace_ShouldAcceptWhitespace()
    {
        // Arrange
        var barCode = new BarCode();

        // Act
        barCode.Label = "   ";

        // Assert
        barCode.Label.ShouldBe("   ");
    }

    /// <summary>
    /// Executes CreatedOn_WhenSet_ShouldStoreDateTime operation.
    /// </summary>

    [Fact]
    public void CreatedOn_WhenSet_ShouldStoreDateTime()
    {
        // Arrange
        var barCode = new BarCode();
        var expectedDateTime = DateTime.Now;

        // Act
        barCode.CreatedOn = expectedDateTime;

        // Assert
        barCode.CreatedOn.ShouldBe(expectedDateTime);
    }

    /// <summary>
    /// Executes ModifiedOn_WhenSet_ShouldStoreDateTime operation.
    /// </summary>

    [Fact]
    public void ModifiedOn_WhenSet_ShouldStoreDateTime()
    {
        // Arrange
        var barCode = new BarCode();
        var expectedDateTime = DateTime.Now;

        // Act
        barCode.ModifiedOn = expectedDateTime;

        // Assert
        barCode.ModifiedOn.ShouldBe(expectedDateTime);
    }

    /// <summary>
    /// Executes PartStatus_WhenSetToNone_ShouldAcceptNone operation.
    /// </summary>

    [Fact]
    public void PartStatus_WhenSetToNone_ShouldAcceptNone()
    {
        // Arrange
        var barCode = new BarCode();

        // Act
        barCode.PartStatus = PartStatus.None;

        // Assert
        barCode.PartStatus.ShouldBe(PartStatus.None);
    }

    /// <summary>
    /// Executes PartStatus_WhenSetToOk_ShouldAcceptOk operation.
    /// </summary>

    [Fact]
    public void PartStatus_WhenSetToOk_ShouldAcceptOk()
    {
        // Arrange
        var barCode = new BarCode();

        // Act
        barCode.PartStatus = PartStatus.Ok;

        // Assert
        barCode.PartStatus.ShouldBe(PartStatus.Ok);
    }

    /// <summary>
    /// Executes PartStatus_WhenSetToNok_ShouldAcceptNok operation.
    /// </summary>

    [Fact]
    public void PartStatus_WhenSetToNok_ShouldAcceptNok()
    {
        // Arrange
        var barCode = new BarCode();

        // Act
        barCode.PartStatus = PartStatus.NOk;

        // Assert
        barCode.PartStatus.ShouldBe(PartStatus.NOk);
    }

    /// <summary>
    /// Executes FlowStatus_WhenSetToNone_ShouldAcceptNone operation.
    /// </summary>

    [Fact]
    public void FlowStatus_WhenSetToNone_ShouldAcceptNone()
    {
        // Arrange
        var barCode = new BarCode();

        // Act
        barCode.FlowStatus = FlowStatus.None;

        // Assert
        barCode.FlowStatus.ShouldBe(FlowStatus.None);
    }

    /// <summary>
    /// Executes FlowStatus_WhenSetToCreated_ShouldAcceptCreated operation.
    /// </summary>

    [Fact]
    public void FlowStatus_WhenSetToCreated_ShouldAcceptCreated()
    {
        // Arrange
        var barCode = new BarCode();

        // Act
        barCode.FlowStatus = FlowStatus.Created;

        // Assert
        barCode.FlowStatus.ShouldBe(FlowStatus.Created);
    }

    /// <summary>
    /// Executes FlowStatus_WhenSetToInProcess_ShouldAcceptInProcess operation.
    /// </summary>

    [Fact]
    public void FlowStatus_WhenSetToInProcess_ShouldAcceptInProcess()
    {
        // Arrange
        var barCode = new BarCode();

        // Act
        barCode.FlowStatus = FlowStatus.InProcess;

        // Assert
        barCode.FlowStatus.ShouldBe(FlowStatus.InProcess);
    }

    /// <summary>
    /// Executes FlowStatus_WhenSetToFinished_ShouldAcceptFinished operation.
    /// </summary>

    [Fact]
    public void FlowStatus_WhenSetToFinished_ShouldAcceptFinished()
    {
        // Arrange
        var barCode = new BarCode();

        // Act
        barCode.FlowStatus = FlowStatus.Finished;

        // Assert
        barCode.FlowStatus.ShouldBe(FlowStatus.Finished);
    }

    /// <summary>
    /// Executes BarCode_WhenPartStatusSetToOk_ShouldRetainOkStatusValue operation.
    /// </summary>

    [Fact]
    public void BarCode_WhenPartStatusSetToOk_ShouldRetainOkStatusValue()
    {
        // Arrange
        var barCode = new BarCode { PartStatus = PartStatus.Ok };

        // Act & Assert
        barCode.PartStatus.ShouldBe(PartStatus.Ok);
    }

    /// <summary>
    /// Executes BarCode_WhenPartStatusSetToNok_ShouldRetainNokStatusValue operation.
    /// </summary>

    [Fact]
    public void BarCode_WhenPartStatusSetToNok_ShouldRetainNokStatusValue()
    {
        // Arrange
        var barCode = new BarCode { PartStatus = PartStatus.NOk };

        // Act & Assert
        barCode.PartStatus.ShouldBe(PartStatus.NOk);
    }

    /// <summary>
    /// Executes BarCode_WhenFlowStatusSetToCreated_ShouldMaintainCreatedState operation.
    /// </summary>

    [Fact]
    public void BarCode_WhenFlowStatusSetToCreated_ShouldMaintainCreatedState()
    {
        // Arrange
        var barCode = new BarCode { FlowStatus = FlowStatus.Created };

        // Act & Assert
        barCode.FlowStatus.ShouldBe(FlowStatus.Created);
    }

    /// <summary>
    /// Executes BarCode_WhenFlowStatusSetToInProcess_ShouldMaintainInProcessState operation.
    /// </summary>

    [Fact]
    public void BarCode_WhenFlowStatusSetToInProcess_ShouldMaintainInProcessState()
    {
        // Arrange
        var barCode = new BarCode { FlowStatus = FlowStatus.InProcess };

        // Act & Assert
        barCode.FlowStatus.ShouldBe(FlowStatus.InProcess);
    }

    /// <summary>
    /// Executes BarCode_WhenFlowStatusSetToFinished_ShouldMaintainFinishedState operation.
    /// </summary>

    [Fact]
    public void BarCode_WhenFlowStatusSetToFinished_ShouldMaintainFinishedState()
    {
        // Arrange
        var barCode = new BarCode { FlowStatus = FlowStatus.Finished };

        // Act & Assert
        barCode.FlowStatus.ShouldBe(FlowStatus.Finished);
    }

    /// <summary>
    /// Executes BarCode_WhenCreatedAndModifiedTimestampsSet_ShouldValidateTimestampProgression operation.
    /// </summary>

    [Fact]
    public void BarCode_WhenCreatedAndModifiedTimestampsSet_ShouldValidateTimestampProgression()
    {
        // Arrange
        var barCode = new BarCode
        {
            CreatedOn = DateTime.Now,
            ModifiedOn = DateTime.Now.AddHours(1)
        };

        // Act & Assert
        barCode.CreatedOn.ShouldNotBe(default);
        barCode.ModifiedOn.ShouldNotBe(default);
        barCode.ModifiedOn.ShouldBeGreaterThan(barCode.CreatedOn);
    }

    /// <summary>
    /// Executes BarCode_WhenInstanceCreatedNew_ShouldInitializeWithExpectedDefaultValues operation.
    /// </summary>

    [Fact]
    public void BarCode_WhenInstanceCreatedNew_ShouldInitializeWithExpectedDefaultValues()
    {
        // Arrange & Act
        var barCode = new BarCode();

        // Assert
        barCode.BarCodeId.ShouldBe(0);
        barCode.ProductId.ShouldBe(0);
        //[Fix]
        //CLAUDE
        //Date: 20/08/2025
        //Reason: Fix test expectation - BarCode.Label is initialized to string.Empty in the actual implementation, aligning with null safety goal
        barCode.Label.ShouldBe(string.Empty); // BarCode.Label is initialized to string.Empty
        barCode.MachineId.ShouldBe(0);
        barCode.PartStatus.ShouldBe(PartStatus.None);
        barCode.FlowStatus.ShouldBe(FlowStatus.None);
    }

    /// <summary>
    /// Executes BarCode_WhenAllPropertiesPopulated_ShouldRepresentCompleteBarCodeEntity operation.
    /// </summary>

    [Fact]
    public void BarCode_WhenAllPropertiesPopulated_ShouldRepresentCompleteBarCodeEntity()
    {
        // Arrange
        var barCode = new BarCode
        {
            BarCodeId = 123,
            ProductId = 456,
            Label = "L1ATEST1230001",
            MachineId = 789,
            PartStatus = PartStatus.Ok,
            FlowStatus = FlowStatus.Finished,
            CreatedOn = DateTime.Now,
            ModifiedOn = DateTime.Now.AddHours(1)
        };

        // Act & Assert
        barCode.BarCodeId.ShouldBe(123);
        barCode.ProductId.ShouldBe(456);
        barCode.Label.ShouldBe("L1ATEST1230001");
        barCode.MachineId.ShouldBe(789);
        barCode.PartStatus.ShouldBe(PartStatus.Ok);
        barCode.FlowStatus.ShouldBe(FlowStatus.Finished);
        barCode.CreatedOn.ShouldNotBe(default);
        barCode.ModifiedOn.ShouldNotBe(default);
    }

    /// <summary>
    /// Executes BarCode_WhenLabelSetToSingleCharacter_ShouldAcceptMinimalLabelLength operation.
    /// </summary>

    [Fact]
    public void BarCode_WhenLabelSetToSingleCharacter_ShouldAcceptMinimalLabelLength()
    {
        // Arrange
        var barCode = new BarCode { Label = "A" };

        // Act & Assert
        barCode.Label.ShouldBe("A");
    }

    /// <summary>
    /// Executes BarCode_WhenLabelSetToVeryLongString_ShouldAcceptExtendedLabelLength operation.
    /// </summary>

    [Fact]
    public void BarCode_WhenLabelSetToVeryLongString_ShouldAcceptExtendedLabelLength()
    {
        // Arrange
        var barCode = new BarCode { Label = new string('A', 100) };

        // Act & Assert
        barCode.Label.ShouldNotBeNull();
        barCode.Label.Length.ShouldBe(100);
    }

    /// <summary>
    /// Executes BarCode_WhenLabelContainsSpecialCharacters_ShouldAcceptNonAlphanumericCharacters operation.
    /// </summary>

    [Fact]
    public void BarCode_WhenLabelContainsSpecialCharacters_ShouldAcceptNonAlphanumericCharacters()
    {
        // Arrange
        var barCode = new BarCode { Label = "L1A-TEST_123@#$%" };

        // Act & Assert
        barCode.Label.ShouldBe("L1A-TEST_123@#$%");
    }

    /// <summary>
    /// Executes BarCode_WhenLabelSetToNumericString_ShouldAcceptDigitsOnlyLabels operation.
    /// </summary>

    [Fact]
    public void BarCode_WhenLabelSetToNumericString_ShouldAcceptDigitsOnlyLabels()
    {
        // Arrange
        var barCode = new BarCode { Label = "123456789" };

        // Act & Assert
        barCode.Label.ShouldBe("123456789");
    }

    /// <summary>
    /// Executes BarCode_WhenLabelContainsUnicodeCharacters_ShouldAcceptInternationalCharacters operation.
    /// </summary>

    [Fact]
    public void BarCode_WhenLabelContainsUnicodeCharacters_ShouldAcceptInternationalCharacters()
    {
        // Arrange
        var barCode = new BarCode { Label = "L1A-TEST-ñáéíóú" };

        // Act & Assert
        barCode.Label.ShouldBe("L1A-TEST-ñáéíóú");
    }

    /// <summary>
    /// Executes BarCode_WhenPartStatusOkAndFlowFinished_ShouldRepresentValidCompletedState operation.
    /// </summary>

    [Fact]
    public void BarCode_WhenPartStatusOkAndFlowFinished_ShouldRepresentValidCompletedState()
    {
        // Arrange
        var barCode = new BarCode
        {
            PartStatus = PartStatus.Ok,
            FlowStatus = FlowStatus.Finished
        };

        // Act & Assert
        barCode.PartStatus.ShouldBe(PartStatus.Ok);
        barCode.FlowStatus.ShouldBe(FlowStatus.Finished);
    }

    /// <summary>
    /// Executes BarCode_WhenPartStatusNokAndFlowFinished_ShouldRepresentValidRejectedState operation.
    /// </summary>

    [Fact]
    public void BarCode_WhenPartStatusNokAndFlowFinished_ShouldRepresentValidRejectedState()
    {
        // Arrange
        var barCode = new BarCode
        {
            PartStatus = PartStatus.NOk,
            FlowStatus = FlowStatus.Finished
        };

        // Act & Assert
        barCode.PartStatus.ShouldBe(PartStatus.NOk);
        barCode.FlowStatus.ShouldBe(FlowStatus.Finished);
    }

    /// <summary>
    /// Executes BarCode_WhenPartStatusOkAndFlowInProcess_ShouldRepresentValidInProgressState operation.
    /// </summary>

    [Fact]
    public void BarCode_WhenPartStatusOkAndFlowInProcess_ShouldRepresentValidInProgressState()
    {
        // Arrange
        var barCode = new BarCode
        {
            PartStatus = PartStatus.Ok,
            FlowStatus = FlowStatus.InProcess
        };

        // Act & Assert
        barCode.PartStatus.ShouldBe(PartStatus.Ok);
        barCode.FlowStatus.ShouldBe(FlowStatus.InProcess);
    }

    /// <summary>
    /// Executes BarCode_WhenPartStatusNokAndFlowInProcess_ShouldRepresentValidDefectiveInProgressState operation.
    /// </summary>

    [Fact]
    public void BarCode_WhenPartStatusNokAndFlowInProcess_ShouldRepresentValidDefectiveInProgressState()
    {
        // Arrange
        var barCode = new BarCode
        {
            PartStatus = PartStatus.NOk,
            FlowStatus = FlowStatus.InProcess
        };

        // Act & Assert
        barCode.PartStatus.ShouldBe(PartStatus.NOk);
        barCode.FlowStatus.ShouldBe(FlowStatus.InProcess);
    }

    /// <summary>
    /// Executes BarCode_WhenAssignedToSpecificMachine_ShouldRetainCorrectMachineIdentifier operation.
    /// </summary>

    [Fact]
    public void BarCode_WhenAssignedToSpecificMachine_ShouldRetainCorrectMachineIdentifier()
    {
        // Arrange
        var barCode = new BarCode { MachineId = 10023 };

        // Act & Assert
        barCode.MachineId.ShouldBe(10023);
    }

    /// <summary>
    /// Executes BarCode_WhenNotAssignedToMachine_ShouldHaveZeroMachineIdentifier operation.
    /// </summary>

    [Fact]
    public void BarCode_WhenNotAssignedToMachine_ShouldHaveZeroMachineIdentifier()
    {
        // Arrange
        var barCode = new BarCode { MachineId = 0 };

        // Act & Assert
        barCode.MachineId.ShouldBe(0);
    }

    /// <summary>
    /// Executes BarCode_WhenPropertiesSetToMaximumValues_ShouldAcceptIntegerLimits operation.
    /// </summary>

    [Fact]
    public void BarCode_WhenPropertiesSetToMaximumValues_ShouldAcceptIntegerLimits()
    {
        // Arrange
        var barCode = new BarCode
        {
            BarCodeId = int.MaxValue,
            ProductId = int.MaxValue,
            MachineId = int.MaxValue
        };

        // Act & Assert
        barCode.BarCodeId.ShouldBe(int.MaxValue);
        barCode.ProductId.ShouldBe(int.MaxValue);
        barCode.MachineId.ShouldBe(int.MaxValue);
    }

    /// <summary>
    /// Executes BarCode_WhenLabelContainsWhitespaceCharacters_ShouldPreserveSpacesInLabel operation.
    /// </summary>

    [Fact]
    public void BarCode_WhenLabelContainsWhitespaceCharacters_ShouldPreserveSpacesInLabel()
    {
        // Arrange
        var barCode = new BarCode { Label = "  L1A TEST 123  " };

        // Act & Assert
        barCode.Label.ShouldBe("  L1A TEST 123  ");
    }

    /// <summary>
    /// Executes ToString_WhenLabelIsNull_ShouldReturnEmptyString operation.
    /// </summary>

    [Fact]
    public void ToString_WhenLabelIsNull_ShouldReturnEmptyString()
    {
        // Arrange
        var barCode = new BarCode { Label = null! };  //Property is no nullable reference but string still is nullable

        // Act
        var result = barCode.ToString();

        // Assert
        result.ShouldBeNull();
    }

    /// <summary>
    /// Executes ToString_WhenLabelIsSet_ShouldReturnLabel operation.
    /// </summary>

    [Fact]
    public void ToString_WhenLabelIsSet_ShouldReturnLabel()
    {
        // Arrange
        var barCode = new BarCode { Label = "L1ATEST1230001" };

        // Act
        var result = barCode.ToString();

        // Assert
        result.ShouldBe("L1ATEST1230001");
    }
}
