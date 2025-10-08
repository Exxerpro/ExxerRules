namespace IndTrace.Domain.UnitTests.ProductsTests;

/// <summary>
/// Unit tests for Product domain entity
/// </summary>
public class ProductTests
{
    /// <summary>
    /// Executes Product_Product_WithAllRequiredProperties_ShouldCreateInstanceWithCorrectValues operation.
    /// </summary>
    [Fact]
    public void Product_Product_WithAllRequiredProperties_ShouldCreateInstanceWithCorrectValues()
    {
        // Arrange
        var productId = 1;
        var partNumber = "TEST123";
        var description = "Test Product";
        var customerId = 1;

        // Act
        var product = new Product
        {
            ProductId = productId,
            PartNumber = partNumber,
            Description = description,
            CustomerId = customerId
        };

        // Assert
        product.ShouldNotBeNull();
        product.ProductId.ShouldBe(productId);
        product.PartNumber.ShouldBe(partNumber);
        product.Description.ShouldBe(description);
        product.CustomerId.ShouldBe(customerId);
    }

    /// <summary>
    /// Executes Product_Product_WithDefaultConstructor_ShouldInitializeAllPropertiesToDefaultValues operation.
    /// </summary>

    [Fact]
    public void Product_Product_WithDefaultConstructor_ShouldInitializeAllPropertiesToDefaultValues()
    {
        // Act
        var product = new Product();

        // Assert
        product.ShouldNotBeNull();
        product.ProductId.ShouldBe(0);
        product.PartNumber.ShouldBeNull();
        product.ProductName.ShouldBeNull();
        product.IsActive.ShouldBe(0);
        product.Version.ShouldBe(0);
        product.CustomerPartNumber.ShouldBe(string.Empty);
        product.AliasPartNumber.ShouldBe(string.Empty);
        product.Description.ShouldBeNull();
        product.RuleId.ShouldBe(0);
        product.CustomerId.ShouldBe(0);
        product.LineId.ShouldBe(0);
        product.CustomerName.ShouldBe(string.Empty);
    }

    /// <summary>
    /// Executes ProductId_WhenSetToPositiveInteger_ShouldStoreValueCorrectly operation.
    /// </summary>

    [Fact]
    public void ProductId_WhenSetToPositiveInteger_ShouldStoreValueCorrectly()
    {
        // Arrange
        var product = new Product();
        var productId = 123;

        // Act
        product.ProductId = productId;

        // Assert
        product.ProductId.ShouldBe(productId);
    }

    /// <summary>
    /// Executes PartNumber_WhenSetToValidString_ShouldStoreValueCorrectly operation.
    /// </summary>

    [Fact]
    public void PartNumber_WhenSetToValidString_ShouldStoreValueCorrectly()
    {
        // Arrange
        var product = new Product();
        var partNumber = "TEST123";

        // Act
        product.PartNumber = partNumber;

        // Assert
        product.PartNumber.ShouldBe(partNumber);
    }

    /// <summary>
    /// Executes ProductName_WhenSetToString_ShouldStoreValueCorrectly operation.
    /// </summary>

    [Fact]
    public void ProductName_WhenSetToString_ShouldStoreValueCorrectly()
    {
        // Arrange
        var product = new Product();
        var productName = "Test Product";

        // Act
        product.ProductName = productName;

        // Assert
        product.ProductName.ShouldBe(productName);
    }

    /// <summary>
    /// Executes IsActive_WhenSetToOne_ShouldIndicateActiveStatus operation.
    /// </summary>

    [Fact]
    public void IsActive_WhenSetToOne_ShouldIndicateActiveStatus()
    {
        // Arrange
        var product = new Product();
        var isActive = 1;

        // Act
        product.IsActive = isActive;

        // Assert
        product.IsActive.ShouldBe(isActive);
    }

    /// <summary>
    /// Executes Version_WhenSetToInteger_ShouldStoreVersionNumber operation.
    /// </summary>

    [Fact]
    public void Version_WhenSetToInteger_ShouldStoreVersionNumber()
    {
        // Arrange
        var product = new Product();
        var version = 2;

        // Act
        product.Version = version;

        // Assert
        product.Version.ShouldBe(version);
    }

    /// <summary>
    /// Executes CustomerPartNumber_WhenSetToString_ShouldStoreCustomerSpecificPartNumber operation.
    /// </summary>

    [Fact]
    public void CustomerPartNumber_WhenSetToString_ShouldStoreCustomerSpecificPartNumber()
    {
        // Arrange
        var product = new Product();
        var customerPartNumber = "CUST123";

        // Act
        product.CustomerPartNumber = customerPartNumber;

        // Assert
        product.CustomerPartNumber.ShouldBe(customerPartNumber);
    }

    /// <summary>
    /// Executes AliasPartNumber_WhenSetToString_ShouldStoreAlternativePartNumber operation.
    /// </summary>

    [Fact]
    public void AliasPartNumber_WhenSetToString_ShouldStoreAlternativePartNumber()
    {
        // Arrange
        var product = new Product();
        var aliasPartNumber = "ALIAS123";

        // Act
        product.AliasPartNumber = aliasPartNumber;

        // Assert
        product.AliasPartNumber.ShouldBe(aliasPartNumber);
    }

    /// <summary>
    /// Executes Description_WhenSetToString_ShouldStoreProductDescription operation.
    /// </summary>

    [Fact]
    public void Description_WhenSetToString_ShouldStoreProductDescription()
    {
        // Arrange
        var product = new Product();
        var description = "Test product description";

        // Act
        product.Description = description;

        // Assert
        product.Description.ShouldBe(description);
    }

    /// <summary>
    /// Executes RuleId_WhenSetToInteger_ShouldStoreAssociatedRuleId operation.
    /// </summary>

    [Fact]
    public void RuleId_WhenSetToInteger_ShouldStoreAssociatedRuleId()
    {
        // Arrange
        var product = new Product();
        var ruleId = 456;

        // Act
        product.RuleId = ruleId;

        // Assert
        product.RuleId.ShouldBe(ruleId);
    }

    /// <summary>
    /// Executes CustomerId_WhenSetToInteger_ShouldStoreAssociatedCustomerId operation.
    /// </summary>

    [Fact]
    public void CustomerId_WhenSetToInteger_ShouldStoreAssociatedCustomerId()
    {
        // Arrange
        var product = new Product();
        var customerId = 789;

        // Act
        product.CustomerId = customerId;

        // Assert
        product.CustomerId.ShouldBe(customerId);
    }

    /// <summary>
    /// Executes LineId_WhenSetToInteger_ShouldStoreAssociatedLineId operation.
    /// </summary>

    [Fact]
    public void LineId_WhenSetToInteger_ShouldStoreAssociatedLineId()
    {
        // Arrange
        var product = new Product();
        var lineId = 101;

        // Act
        product.LineId = lineId;

        // Assert
        product.LineId.ShouldBe(lineId);
    }

    /// <summary>
    /// Executes CustomerName_WhenSetToString_ShouldStoreCustomerNameForDisplay operation.
    /// </summary>

    [Fact]
    public void CustomerName_WhenSetToString_ShouldStoreCustomerNameForDisplay()
    {
        // Arrange
        var product = new Product();
        var customerName = "Test Customer";

        // Act
        product.CustomerName = customerName;

        // Assert
        product.CustomerName.ShouldBe(customerName);
    }

    /// <summary>
    /// Executes Line_WhenSetToLineEntity_ShouldStoreAssociatedLineReference operation.
    /// </summary>

    [Fact]
    public void Line_WhenSetToLineEntity_ShouldStoreAssociatedLineReference()
    {
        // Arrange
        var product = new Product();
        var line = new Line { LineId = 1, Name = "Test Line" };

        // Act
        product.Line = line;

        // Assert
        product.Line.ShouldBe(line);
        product.Line.LineId.ShouldBe(1);
        product.Line.Name.ShouldBe("Test Line");
    }

    /// <summary>
    /// Executes Customer_WhenSetToCustomerEntity_ShouldStoreAssociatedCustomerReference operation.
    /// </summary>

    [Fact]
    public void Customer_WhenSetToCustomerEntity_ShouldStoreAssociatedCustomerReference()
    {
        // Arrange
        var product = new Product();
        var customer = new Customer { CustomerId = 1, Name = "Test Customer" };

        // Act
        product.Customer = customer;

        // Assert
        product.Customer.ShouldBe(customer);
        product.Customer.CustomerId.ShouldBe(1);
        product.Customer.Name.ShouldBe("Test Customer");
    }

    /// <summary>
    /// Executes Product_AsAuditableEntity_ShouldInheritAuditableEntityProperties operation.
    /// </summary>

    [Fact]
    public void Product_AsAuditableEntity_ShouldInheritAuditableEntityProperties()
    {
        // Arrange
        var product = new Product();

        // Act & Assert
        product.ShouldBeAssignableTo<AuditableEntity>();
    }

    /// <summary>
    /// Executes Product_WithCompleteConfiguration_ShouldMaintainAllPropertyValues operation.
    /// </summary>

    [Fact]
    public void Product_WithCompleteConfiguration_ShouldMaintainAllPropertyValues()
    {
        // Arrange
        var product = new Product
        {
            ProductId = 1,
            PartNumber = "TEST123",
            ProductName = "Test Product",
            IsActive = 1,
            Version = 2,
            CustomerPartNumber = "CUST123",
            AliasPartNumber = "ALIAS123",
            Description = "Test product description",
            RuleId = 456,
            CustomerId = 789,
            LineId = 101,
            CustomerName = "Test Customer",
            Line = new Line { LineId = 101, Name = "Test Line" },
            Customer = new Customer { CustomerId = 789, Name = "Test Customer" }
        };

        // Act & Assert
        product.ProductId.ShouldBe(1);
        product.PartNumber.ShouldBe("TEST123");
        product.ProductName.ShouldBe("Test Product");
        product.IsActive.ShouldBe(1);
        product.Version.ShouldBe(2);
        product.CustomerPartNumber.ShouldBe("CUST123");
        product.AliasPartNumber.ShouldBe("ALIAS123");
        product.Description.ShouldBe("Test product description");
        product.RuleId.ShouldBe(456);
        product.CustomerId.ShouldBe(789);
        product.LineId.ShouldBe(101);
        product.CustomerName.ShouldBe("Test Customer");
        product.Line.ShouldNotBeNull();
        product.Customer.ShouldNotBeNull();
    }

    /// <summary>
    /// Executes Product_Product_WithNullStringProperties_ShouldAcceptAndStoreNullValues operation.
    /// </summary>

    [Fact]
    public void Product_Product_WithNullStringProperties_ShouldAcceptAndStoreNullValues()
    {
        // Arrange
        var product = new Product
        {
            PartNumber = string.Empty,
            ProductName = string.Empty,
            CustomerPartNumber = string.Empty,
            AliasPartNumber = string.Empty,
            Description = string.Empty,
            CustomerName = string.Empty
        };

        // Act & Assert
        product.PartNumber.ShouldNotBeNull();
        product.ProductName.ShouldNotBeNull();
        product.CustomerPartNumber.ShouldNotBeNull();
        product.AliasPartNumber.ShouldNotBeNull();
        product.Description.ShouldNotBeNull();
        product.CustomerName.ShouldNotBeNull();
    }

    /// <summary>
    /// Executes Product_Product_WithEmptyStringProperties_ShouldAcceptAndStoreEmptyStrings operation.
    /// </summary>

    [Fact]
    public void Product_Product_WithEmptyStringProperties_ShouldAcceptAndStoreEmptyStrings()
    {
        // Arrange
        var product = new Product
        {
            PartNumber = "",
            ProductName = "",
            CustomerPartNumber = "",
            AliasPartNumber = "",
            Description = "",
            CustomerName = ""
        };

        // Act & Assert
        product.PartNumber.ShouldBe("");
        product.ProductName.ShouldBe("");
        product.CustomerPartNumber.ShouldBe("");
        product.AliasPartNumber.ShouldBe("");
        product.Description.ShouldBe("");
        product.CustomerName.ShouldBe("");
    }

    /// <summary>
    /// Executes Product_WithNegativeIdValues_ShouldAcceptNegativeIntegers operation.
    /// </summary>

    [Fact]
    public void Product_WithNegativeIdValues_ShouldAcceptNegativeIntegers()
    {
        // Arrange
        var product = new Product
        {
            ProductId = -1,
            RuleId = -2,
            CustomerId = -3,
            LineId = -4
        };

        // Act & Assert
        product.ProductId.ShouldBe(-1);
        product.RuleId.ShouldBe(-2);
        product.CustomerId.ShouldBe(-3);
        product.LineId.ShouldBe(-4);
    }

    /// <summary>
    /// Executes Product_WithMaxIntegerValues_ShouldHandleIntegerMaxValues operation.
    /// </summary>

    [Fact]
    public void Product_WithMaxIntegerValues_ShouldHandleIntegerMaxValues()
    {
        // Arrange
        var product = new Product
        {
            ProductId = int.MaxValue,
            RuleId = int.MaxValue - 1,
            CustomerId = int.MaxValue - 2,
            LineId = int.MaxValue - 3,
            Version = int.MaxValue - 4
        };

        // Act & Assert
        product.ProductId.ShouldBe(int.MaxValue);
        product.RuleId.ShouldBe(int.MaxValue - 1);
        product.CustomerId.ShouldBe(int.MaxValue - 2);
        product.LineId.ShouldBe(int.MaxValue - 3);
        product.Version.ShouldBe(int.MaxValue - 4);
    }

    /// <summary>
    /// Executes Product_CreatedWithoutParameters_ShouldHaveExpectedDefaultValues operation.
    /// </summary>

    [Fact]
    public void Product_CreatedWithoutParameters_ShouldHaveExpectedDefaultValues()
    {
        // Arrange & Act
        var product = new Product();

        // Assert
        product.ShouldNotBeNull();
        product.ProductId.ShouldBe(0);
        product.PartNumber.ShouldBeNull();
        product.Description.ShouldBeNull();
        product.CustomerId.ShouldBe(0);
    }

    /// <summary>
    /// Executes Product_WhenMultiplePropertiesUpdated_ShouldRetainAllAssignedValues operation.
    /// </summary>

    [Fact]
    public void Product_WhenMultiplePropertiesUpdated_ShouldRetainAllAssignedValues()
    {
        // Arrange
        var product = new Product();

        // Act
        product.ProductId = 123;
        product.PartNumber = "ABC123";
        product.Description = "Updated Description";
        product.CustomerId = 456;
        product.IsActive = 1;
        product.CreatedOn = DateTime.Now;
        product.ModifiedOn = DateTime.Now.AddHours(1);

        // Assert
        product.ProductId.ShouldBe(123);
        product.PartNumber.ShouldBe("ABC123");
        product.Description.ShouldBe("Updated Description");
        product.CustomerId.ShouldBe(456);
        product.IsActive.ShouldBe(1);
        product.CreatedOn.ShouldNotBe(default);
        product.ModifiedOn.ShouldNotBe(default);
    }

    /// <summary>
    /// Executes Product_WhenStringPropertiesSetToNull_ShouldAllowNullAssignment operation.
    /// </summary>

    [Fact]
    public void Product_WhenStringPropertiesSetToNull_ShouldAllowNullAssignment()
    {
        // Arrange
        var product = new Product
        {
            PartNumber = "TEST123",
            Description = "Test Description"
        };

        // Act
        product.PartNumber = string.Empty;
        product.Description = string.Empty;

        // Assert
        product.PartNumber.ShouldNotBeNull();
        product.Description.ShouldNotBeNull();
    }

    /// <summary>
    /// Executes IsActive_WhenSetToZero_ShouldIndicateInactiveStatus operation.
    /// </summary>

    [Fact]
    public void IsActive_WhenSetToZero_ShouldIndicateInactiveStatus()
    {
        // Arrange
        var product = new Product { IsActive = 1 };

        // Act
        product.IsActive = 0;

        // Assert
        product.IsActive.ShouldBe(0);
    }

    /// <summary>
    /// Executes CreatedOn_WhenSetToDateTime_ShouldStoreCreationTimestamp operation.
    /// </summary>

    [Fact]
    public void CreatedOn_WhenSetToDateTime_ShouldStoreCreationTimestamp()
    {
        // Arrange
        var product = new Product();
        var expectedDateTime = DateTime.Now;

        // Act
        product.CreatedOn = expectedDateTime;

        // Assert
        product.CreatedOn.ShouldBe(expectedDateTime);
    }

    /// <summary>
    /// Executes ModifiedOn_WhenSetToDateTime_ShouldStoreModificationTimestamp operation.
    /// </summary>

    [Fact]
    public void ModifiedOn_WhenSetToDateTime_ShouldStoreModificationTimestamp()
    {
        // Arrange
        var product = new Product();
        var expectedDateTime = DateTime.Now;

        // Act
        product.ModifiedOn = expectedDateTime;

        // Assert
        product.ModifiedOn.ShouldBe(expectedDateTime);
    }

    /// <summary>
    /// Executes CustomerId_WhenSetToZero_ShouldAcceptZeroAsValidValue operation.
    /// </summary>

    [Fact]
    public void CustomerId_WhenSetToZero_ShouldAcceptZeroAsValidValue()
    {
        // Arrange
        var product = new Product();

        // Act
        product.CustomerId = 0;

        // Assert
        product.CustomerId.ShouldBe(0);
    }

    /// <summary>
    /// Executes CustomerId_WhenSetToNegativeValue_ShouldAcceptNegativeInteger operation.
    /// </summary>

    [Fact]
    public void CustomerId_WhenSetToNegativeValue_ShouldAcceptNegativeInteger()
    {
        // Arrange
        var product = new Product();

        // Act
        product.CustomerId = -1;

        // Assert
        product.CustomerId.ShouldBe(-1);
    }

    /// <summary>
    /// Executes ProductId_WhenSetToZero_ShouldAcceptZeroAsValidValue operation.
    /// </summary>

    [Fact]
    public void ProductId_WhenSetToZero_ShouldAcceptZeroAsValidValue()
    {
        // Arrange
        var product = new Product();

        // Act
        product.ProductId = 0;

        // Assert
        product.ProductId.ShouldBe(0);
    }

    /// <summary>
    /// Executes ProductId_WhenSetToNegativeValue_ShouldAcceptNegativeInteger operation.
    /// </summary>

    [Fact]
    public void ProductId_WhenSetToNegativeValue_ShouldAcceptNegativeInteger()
    {
        // Arrange
        var product = new Product();

        // Act
        product.ProductId = -1;

        // Assert
        product.ProductId.ShouldBe(-1);
    }

    /// <summary>
    /// Executes PartNumber_WhenSetToEmptyString_ShouldStoreEmptyStringValue operation.
    /// </summary>

    [Fact]
    public void PartNumber_WhenSetToEmptyString_ShouldStoreEmptyStringValue()
    {
        // Arrange
        var product = new Product();

        // Act
        product.PartNumber = "";

        // Assert
        product.PartNumber.ShouldBe("");
    }

    /// <summary>
    /// Executes PartNumber_WhenSetToWhitespaceOnly_ShouldPreserveWhitespaceCharacters operation.
    /// </summary>

    [Fact]
    public void PartNumber_WhenSetToWhitespaceOnly_ShouldPreserveWhitespaceCharacters()
    {
        // Arrange
        var product = new Product();

        // Act
        product.PartNumber = "   ";

        // Assert
        product.PartNumber.ShouldBe("   ");
    }

    /// <summary>
    /// Executes Description_WhenSetToEmptyString_ShouldStoreEmptyStringValue operation.
    /// </summary>

    [Fact]
    public void Description_WhenSetToEmptyString_ShouldStoreEmptyStringValue()
    {
        // Arrange
        var product = new Product();

        // Act
        product.Description = "";

        // Assert
        product.Description.ShouldBe("");
    }

    /// <summary>
    /// Executes Description_WhenSetToWhitespaceOnly_ShouldPreserveWhitespaceCharacters operation.
    /// </summary>

    [Fact]
    public void Description_WhenSetToWhitespaceOnly_ShouldPreserveWhitespaceCharacters()
    {
        // Arrange
        var product = new Product();

        // Act
        product.Description = "   ";

        // Assert
        product.Description.ShouldBe("   ");
    }

    /// <summary>
    /// Executes IsActive_WhenSetToOne_ShouldRepresentActiveProductState operation.
    /// </summary>

    [Fact]
    public void IsActive_WhenSetToOne_ShouldRepresentActiveProductState()
    {
        // Arrange
        var product = new Product { IsActive = 1 };

        // Act & Assert
        product.IsActive.ShouldBe(1);
    }

    /// <summary>
    /// Executes IsActive_WhenSetToZero_ShouldRepresentInactiveProductState operation.
    /// </summary>

    [Fact]
    public void IsActive_WhenSetToZero_ShouldRepresentInactiveProductState()
    {
        // Arrange
        var product = new Product { IsActive = 0 };

        // Act & Assert
        product.IsActive.ShouldBe(0);
    }

    /// <summary>
    /// Executes PartNumber_WhenPopulated_ShouldNotBeNullOrEmpty operation.
    /// </summary>

    [Fact]
    public void PartNumber_WhenPopulated_ShouldNotBeNullOrEmpty()
    {
        // Arrange
        var product = new Product { PartNumber = "VALID123" };

        // Act & Assert
        product.PartNumber.ShouldNotBeNullOrEmpty();
    }

    /// <summary>
    /// Executes Description_WhenPopulated_ShouldNotBeNullOrEmpty operation.
    /// </summary>

    [Fact]
    public void Description_WhenPopulated_ShouldNotBeNullOrEmpty()
    {
        // Arrange
        var product = new Product { Description = "Valid Description" };

        // Act & Assert
        product.Description.ShouldNotBeNullOrEmpty();
    }

    /// <summary>
    /// Executes CustomerId_WhenSetToPositiveValue_ShouldBeGreaterThanZero operation.
    /// </summary>

    [Fact]
    public void CustomerId_WhenSetToPositiveValue_ShouldBeGreaterThanZero()
    {
        // Arrange
        var product = new Product { CustomerId = 1 };

        // Act & Assert
        product.CustomerId.ShouldBeGreaterThan(0);
    }

    /// <summary>
    /// Executes Timestamps_WhenBothCreatedAndModified_ShouldHaveModifiedAfterCreated operation.
    /// </summary>

    [Fact]
    public void Timestamps_WhenBothCreatedAndModified_ShouldHaveModifiedAfterCreated()
    {
        // Arrange
        var now = DateTime.Now;
        var product = new Product
        {
            CreatedOn = now,
            ModifiedOn = now.AddHours(1)
        };

        // Act & Assert
        product.CreatedOn.ShouldBe(now);
        product.ModifiedOn.ShouldBe(now.AddHours(1));
        product.ModifiedOn!.Value.ShouldBeGreaterThan(product.CreatedOn!.Value);
    }

    /// <summary>
    /// Executes Product_WhenNewlyCreated_ShouldHaveExpectedInitialState operation.
    /// </summary>

    [Fact]
    public void Product_WhenNewlyCreated_ShouldHaveExpectedInitialState()
    {
        // Arrange & Act
        var product = new Product();

        // Assert
        product.ProductId.ShouldBe(0);
        product.PartNumber.ShouldBeNull();
        product.Description.ShouldBeNull();
        product.CustomerId.ShouldBe(0);
        product.IsActive.ShouldBe(0);
        product.CreatedOn.ShouldNotBeNull();
        product.ModifiedOn.ShouldNotBeNull();
    }

    /// <summary>
    /// Executes Product_WithAllPropertiesConfigured_ShouldMaintainCompleteDataIntegrity operation.
    /// </summary>

    [Fact]
    public void Product_WithAllPropertiesConfigured_ShouldMaintainCompleteDataIntegrity()
    {
        // Arrange
        var now = DateTime.Now;
        var product = new Product
        {
            ProductId = 1,
            PartNumber = "COMPLETE123",
            Description = "Complete Product Description",
            CustomerId = 100,
            IsActive = 1,
            CreatedOn = now,
            ModifiedOn = now
        };

        // Act & Assert
        product.ProductId.ShouldBe(1);
        product.PartNumber.ShouldBe("COMPLETE123");
        product.Description.ShouldBe("Complete Product Description");
        product.CustomerId.ShouldBe(100);
        product.IsActive.ShouldBe(1);
        product.CreatedOn.ShouldBe(now);
        product.ModifiedOn.ShouldBe(now);
    }
}
