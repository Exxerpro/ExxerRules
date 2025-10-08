namespace IndTrace.Domain.UnitTests.EnumTests;

/// <summary>
/// Unit tests for EnumModel - Base enumeration class for strongly-typed enumerations in manufacturing systems
/// </summary>
public class EnumModelTests
{
    /// <summary>
    /// Executes Enumeration_WhenDefaultParameters_ShouldCreateValidInstance operation.
    /// </summary>
    [Fact]
    public void Enumeration_WhenDefaultParameters_ShouldCreateValidInstance()
    {
        // Arrange & Act
        var enumModel = new TestEnumModel();

        // Assert
        enumModel.ShouldNotBeNull();
        enumModel.ShouldBeAssignableTo<IComparable>();
        enumModel.ShouldBeAssignableTo<IEnumModel>();
    }
    /// <summary>
    /// Executes Invalid_StaticProperty_ShouldReturnInvalidInstance operation.
    /// </summary>

    [Fact]
    public void Invalid_StaticProperty_ShouldReturnInvalidInstance()
    {
        // Arrange & Act
        var invalid = EnumModel.Invalid;

        // Assert
        invalid.ShouldNotBeNull();
        invalid.Value.ShouldBe(1);
        invalid.Name.ShouldBe("Invalid Value");
    }
    /// <summary>
    /// Executes Deconstruct_WhenCalled_ShouldReturnAllComponents operation.
    /// </summary>

    [Fact]
    public void Deconstruct_WhenCalled_ShouldReturnAllComponents()
    {
        // Arrange
        var partStatus = PartStatus.Ok;

        // Act
        var (value, name, displayName) = partStatus;

        // Assert
        value.ShouldBe(1);
        name.ShouldBe("Ok");
        displayName.ShouldBe("Ok");
    }
    /// <summary>
    /// Executes ImplicitConversion_ToInt_ShouldReturnValue operation.
    /// </summary>

    [Fact]
    public void ImplicitConversion_ToInt_ShouldReturnValue()
    {
        // Arrange
        var partStatus = PartStatus.Ok;

        // Act
        int value = partStatus;

        // Assert
        value.ShouldBe(1);
    }
    /// <summary>
    /// Executes ToString_WhenCalled_ShouldReturnDisplayNameOrName operation.
    /// </summary>

    [Fact]
    public void ToString_WhenCalled_ShouldReturnDisplayNameOrName()
    {
        // Arrange
        var partStatus = PartStatus.Ok;

        // Act
        var result = partStatus.ToString();

        // Assert
        result.ShouldBe("Ok");
    }
    /// <summary>
    /// Executes Exists_WhenCalledWithValues_ShouldReturnCorrectResult operation.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="expected">The expected.</param>

    [Theory]
    [InlineData(1, true)]    // Ok
    [InlineData(2, true)]    // NOk
    [InlineData(4, true)]    // Restored
    [InlineData(999, false)] // Non-existent
    public void Exists_WhenCalledWithValues_ShouldReturnCorrectResult(int value, bool expected)
    {
        // Arrange & Act
        var result = EnumModel.Exists<PartStatus>(value);

        // Assert
        result.ShouldBe(expected);
    }
    /// <summary>
    /// Executes GetAll_WhenCalled_ShouldReturnAllInstancesOfType operation.
    /// </summary>

    [Fact]
    public void GetAll_WhenCalled_ShouldReturnAllInstancesOfType()
    {
        // Arrange & Act
        var allPartStatuses = EnumModel.GetAll<PartStatus>().ToList();

        // Assert
        allPartStatuses.ShouldNotBeEmpty();
        allPartStatuses.ShouldContain(ps => ps.Name == "Ok");
        allPartStatuses.ShouldContain(ps => ps.Name == "nOK");
        allPartStatuses.ShouldContain(ps => ps.Name == "Restored");
        allPartStatuses.ShouldContain(ps => ps.Name == "Rejected");
        allPartStatuses.ShouldContain(ps => ps.Name == "Scrap");
    }
    /// <summary>
    /// Executes ToLookUpTable_Generic_ShouldReturnLookupTableList operation.
    /// </summary>

    [Fact]
    public void ToLookUpTable_Generic_ShouldReturnLookupTableList()
    {
        // Arrange & Act
        var lookupTables = EnumModel.ToLookUpTable<PartStatusEntity, PartStatus>();

        // Assert
        lookupTables.ShouldNotBeEmpty();
        lookupTables.ShouldAllBe(lt => lt != null);
        lookupTables.ShouldContain(lt => lt.Name == "Ok");
    }
    /// <summary>
    /// Executes ToLookUpTable_NonGeneric_ShouldReturnEnumLookUpTableList operation.
    /// </summary>

    [Fact]
    public void ToLookUpTable_NonGeneric_ShouldReturnEnumLookUpTableList()
    {
        // Arrange & Act
        var lookupTables = EnumModel.ToLookUpTable<PartStatus>();

        // Assert
        lookupTables.ShouldNotBeEmpty();
        lookupTables.ShouldAllBe(lt => lt != null);
        lookupTables.ShouldContain(lt => lt.Name == "Ok");
    }
    /// <summary>
    /// Executes Equals_WithSameInstance_ShouldReturnTrue operation.
    /// </summary>

    [Fact]
    public void Equals_WithSameInstance_ShouldReturnTrue()
    {
        // Arrange
        var partStatus1 = PartStatus.Ok;
        var partStatus2 = PartStatus.Ok;

        // Act & Assert
        partStatus1.Equals(partStatus2).ShouldBeTrue();
    }
    /// <summary>
    /// Executes Equals_WithDifferentValues_ShouldReturnFalse operation.
    /// </summary>

    [Fact]
    public void Equals_WithDifferentValues_ShouldReturnFalse()
    {
        // Arrange
        var partStatus1 = PartStatus.Ok;
        var partStatus2 = PartStatus.NOk;

        // Act & Assert
        partStatus1.Equals(partStatus2).ShouldBeFalse();
    }
    /// <summary>
    /// Executes Equals_WithNullObject_ShouldReturnFalse operation.
    /// </summary>

    [Fact]
    public void Equals_WithNullObject_ShouldReturnFalse()
    {
        // Arrange
        var partStatus = PartStatus.Ok;

        // Act & Assert
        partStatus.Equals(null).ShouldBeFalse();
    }
    /// <summary>
    /// Executes GetHashCode_WithSameValues_ShouldReturnSameHashCode operation.
    /// </summary>

    [Fact]
    public void GetHashCode_WithSameValues_ShouldReturnSameHashCode()
    {
        // Arrange
        var partStatus1 = PartStatus.Ok;
        var partStatus2 = PartStatus.Ok;

        // Act
        var hash1 = partStatus1.GetHashCode();
        var hash2 = partStatus2.GetHashCode();

        // Assert
        hash1.ShouldBe(hash2);
    }
    /// <summary>
    /// Executes AbsoluteDifference_BetweenValues_ShouldReturnCorrectDifference operation.
    /// </summary>

    [Fact]
    public void AbsoluteDifference_BetweenValues_ShouldReturnCorrectDifference()
    {
        // Arrange
        var partStatus1 = PartStatus.Ok;       // Value = 1
        var partStatus2 = PartStatus.Restored; // Value = 4

        // Act
        var difference = EnumModel.AbsoluteDifference(partStatus1, partStatus2);

        // Assert
        difference.ShouldBe(3);
    }
    /// <summary>
    /// Executes FromValue_WithValidValues_ShouldReturnCorrectInstance operation.
    /// </summary>
    /// <param name="value">The value.</param>

    [Theory]
    [InlineData(1)]     // Ok
    [InlineData(2)]     // NOk
    [InlineData(4)]     // Restored
    [InlineData(8)]     // Rejected
    [InlineData(512)]   // Scrap
    public void FromValue_WithValidValues_ShouldReturnCorrectInstance(int value)
    {
        // Arrange & Act
        var result = EnumModel.FromValue<PartStatus>(value);

        // Assert
        result.ShouldNotBeNull();
        result.Value.ShouldBe(value);
    }
    /// <summary>
    /// Executes FromValue_WithInvalidValue_ShouldReturnInvalidInstance operation.
    /// </summary>

    [Fact]
    public void FromValue_WithInvalidValue_ShouldReturnInvalidInstance()
    {
        // Arrange & Act
        var result = EnumModel.FromValue<PartStatus>(999);

        // Assert
        result.ShouldNotBeNull();
        result.Name.ShouldBe("Invalid Value");
    }
    /// <summary>
    /// Executes FromName_WithValidNames_ShouldReturnCorrectInstance operation.
    /// </summary>
    /// <param name="name">The name.</param>

    [Theory]
    [InlineData("Ok")]
    [InlineData("nOK")]
    [InlineData("Restored")]
    [InlineData("Rejected")]
    [InlineData("Scrap")]
    public void FromName_WithValidNames_ShouldReturnCorrectInstance(string name)
    {
        // Arrange & Act
        var result = EnumModel.FromName<PartStatus>(name);

        // Assert
        result.ShouldNotBeNull();
        result.Name.ShouldBe(name);
    }
    /// <summary>
    /// Executes FromName_WithInvalidName_ShouldReturnInvalidInstance operation.
    /// </summary>

    [Fact]
    public void FromName_WithInvalidName_ShouldReturnInvalidInstance()
    {
        // Arrange & Act
        var result = EnumModel.FromName<PartStatus>("NonExistentStatus");

        // Assert
        result.ShouldNotBeNull();
        result.Name.ShouldBe("Invalid Value");
    }
    /// <summary>
    /// Executes FromDisplayName_WithValidDisplayName_ShouldReturnCorrectInstance operation.
    /// </summary>

    [Fact]
    public void FromDisplayName_WithValidDisplayName_ShouldReturnCorrectInstance()
    {
        // Arrange & Act
        var result = EnumModel.FromDisplayName<PartStatus>("Ok");

        // Assert
        result.ShouldNotBeNull();
        result.DisplayName.ShouldBe("Ok");
    }
    /// <summary>
    /// Executes FromDisplayName_WithInvalidDisplayName_ShouldReturnInvalidInstance operation.
    /// </summary>

    [Fact]
    public void FromDisplayName_WithInvalidDisplayName_ShouldReturnInvalidInstance()
    {
        // Arrange & Act
        var result = EnumModel.FromDisplayName<PartStatus>("NonExistentDisplayName");

        // Assert
        result.ShouldNotBeNull();
        result.Name.ShouldBe("Invalid Value");
    }
    /// <summary>
    /// Executes FromValue_WithNullableInt_ShouldHandleCorrectly operation.
    /// </summary>
    /// <param name="value">The value.</param>

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(null)]
    public void FromValue_WithNullableInt_ShouldHandleCorrectly(int? value)
    {
        // Arrange & Act
        var result = EnumModel.FromValue<PartStatus>(value);

        // Assert
        result.ShouldNotBeNull();
        if (value.HasValue && EnumModel.Exists<PartStatus>(value.Value))
        {
            result.Value.ShouldBe(value.Value);
        }
        else
        {
            result.Name.ShouldBe("Invalid Value");
        }
    }
    /// <summary>
    /// Executes InvalidValue_WhenCalled_ShouldReturnInvalidInstance operation.
    /// </summary>

    [Fact]
    public void InvalidValue_WhenCalled_ShouldReturnInvalidInstance()
    {
        // Arrange & Act
        var result = EnumModel.InvalidValue<PartStatus>();

        // Assert
        result.ShouldNotBeNull();
        result.Name.ShouldBe("Invalid Value");
    }
    /// <summary>
    /// Executes CompareTo_WithSameType_ShouldCompareByValue operation.
    /// </summary>

    [Fact]
    public void CompareTo_WithSameType_ShouldCompareByValue()
    {
        // Arrange
        var partStatus1 = PartStatus.Ok;       // Value = 1
        var partStatus2 = PartStatus.Restored; // Value = 4

        // Act
        var comparison = partStatus1.CompareTo(partStatus2);

        // Assert
        comparison.ShouldBeLessThan(0);
    }
    /// <summary>
    /// Executes CompareTo_WithNull_ShouldReturnDefault operation.
    /// </summary>

    [Fact]
    public void CompareTo_WithNull_ShouldReturnDefault()
    {
        // Arrange
        var partStatus = PartStatus.Ok;

        // Act
        var comparison = partStatus.CompareTo(null);

        // Assert
        comparison.ShouldBe(0);
    }
    /// <summary>
    /// Executes ManufacturingWorkflow_WithDifferentPartStatuses_ShouldMaintainOrder operation.
    /// </summary>
    /// <param name="value1">The value1.</param>
    /// <param name="value2">The value2.</param>

    [Theory]
    [InlineData(1, 2)]    // Ok vs NOk
    [InlineData(2, 4)]    // NOk vs Restored
    [InlineData(4, 8)]    // Restored vs Rejected
    public void ManufacturingWorkflow_WithDifferentPartStatuses_ShouldMaintainOrder(int value1, int value2)
    {
        // Arrange
        var status1 = EnumModel.FromValue<PartStatus>(value1);
        var status2 = EnumModel.FromValue<PartStatus>(value2);

        // Act
        var comparison = status1.CompareTo(status2);

        // Assert
        comparison.ShouldBeLessThan(0);
        status1.Value.ShouldBeLessThan(status2.Value);
    }
    /// <summary>
    /// Executes QualityControlScenario_WithMultipleStatuses_ShouldHandleCorrectly operation.
    /// </summary>

    [Fact]
    public void QualityControlScenario_WithMultipleStatuses_ShouldHandleCorrectly()
    {
        // Arrange
        var okPart = PartStatus.Ok;
        var nokPart = PartStatus.NOk;
        var restoredPart = PartStatus.Restored;
        var rejectedPart = PartStatus.Rejected;
        var scrapPart = PartStatus.Scrap;

        // Act & Assert - Manufacturing quality control workflow
        okPart.Value.ShouldBe(1);
        nokPart.Value.ShouldBe(2);
        restoredPart.Value.ShouldBe(4);
        rejectedPart.Value.ShouldBe(8);
        scrapPart.Value.ShouldBe(512);

        // Verify quality progression
        okPart.CompareTo(nokPart).ShouldBeLessThan(0);
        nokPart.CompareTo(restoredPart).ShouldBeLessThan(0);
        restoredPart.CompareTo(rejectedPart).ShouldBeLessThan(0);
        rejectedPart.CompareTo(scrapPart).ShouldBeLessThan(0);
    }

    /// <summary>
    /// Test enumeration for testing purposes
    /// </summary>
    private class TestEnumModel : EnumModel
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="base(">The base(.</param>
        public TestEnumModel() : base()
        {
        }
    }
}
