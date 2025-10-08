namespace Application.UnitTests.Features.Products;

/// <summary>
/// Unit tests for ProductDto
/// </summary>
public class ProductDtoTests
{
    /// <summary>
    /// Executes Constructor_WithDefaultConstructor_ShouldCreateInstanceWithDefaultValues operation.
    /// </summary>
    [Fact]
    public void Constructor_WithDefaultConstructor_ShouldCreateInstanceWithDefaultValues()
    {
        // Act
        var dto = new ProductDto();

        // Assert
        dto.ShouldNotBeNull();
        dto.ProductId.ShouldBe(0);
        dto.PartNumber.ShouldBe(string.Empty);
        dto.ProductName.ShouldBe(string.Empty);
        dto.IsActive.ShouldBe(0);
        dto.Version.ShouldBe(0);
        dto.CustomerPartNumber.ShouldBe(string.Empty);
        dto.CustomerId.ShouldBe(0);
        dto.CustomerName.ShouldBe(string.Empty);
        dto.CustomerLogo.ShouldBe(string.Empty);
        dto.AliasPartNumber.ShouldBe(string.Empty);
        dto.Description.ShouldBe(string.Empty);
        dto.CreatedBy.ShouldBe(string.Empty);
        dto.ModifiedBy.ShouldBe(string.Empty);
        dto.LineId.ShouldBe(0);
        dto.RuleId.ShouldBe(0);
        dto.Customer.ShouldNotBeNull();
        dto.CreatedOn.ShouldBe(default(DateTime));
        dto.ModifiedOn.ShouldBe(default(DateTime));
        dto.Machines.ShouldNotBeNull();
        dto.Machines.ShouldBeEmpty();
        dto.MachineNames.ShouldNotBeNull();
        dto.MachineNames.ShouldBeEmpty();
    }
    /// <summary>
    /// Executes Properties_WhenSet_ShouldReturnCorrectValues operation.
    /// </summary>

    [Fact]
    public void Properties_WhenSet_ShouldReturnCorrectValues()
    {
        // Arrange
        var dto = new ProductDto();
        var testDate = new DateTime(2024, 12, 25);
        var customer = new Customer { CustomerId = 100, Name = "Test Customer" };
        var line = new Line { LineId = 50, Name = "Production Line A" };
        var machines = new List<int> { 10, 20, 30 };
        var machineNames = new Dictionary<int, string> { { 10, "Machine A" }, { 20, "Machine B" } };

        // Act
        dto.ProductId = 12345;
        dto.PartNumber = "PART-ABC-123";
        dto.ProductName = "Industrial Widget";
        dto.IsActive = 1;
        dto.Version = 2;
        dto.CustomerPartNumber = "CUST-PART-456";
        dto.CustomerId = 100;
        dto.CustomerName = "ACME Manufacturing";
        dto.CustomerLogo = "acme-logo.png";
        dto.AliasPartNumber = "ALIAS-789";
        dto.Description = "High-precision industrial component";
        dto.CreatedBy = "john.doe";
        dto.ModifiedBy = "jane.smith";
        dto.LineId = 50;
        dto.RuleId = 2005;
        dto.Customer = customer;
        dto.CreatedOn = testDate;
        dto.ModifiedOn = testDate.AddDays(1);
        dto.Machines = machines;
        dto.MachineNames = machineNames;
        dto.Line = line;

        // Assert
        dto.ProductId.ShouldBe(12345);
        dto.PartNumber.ShouldBe("PART-ABC-123");
        dto.ProductName.ShouldBe("Industrial Widget");
        dto.IsActive.ShouldBe(1);
        dto.Version.ShouldBe(2);
        dto.CustomerPartNumber.ShouldBe("CUST-PART-456");
        dto.CustomerId.ShouldBe(100);
        dto.CustomerName.ShouldBe("ACME Manufacturing");
        dto.CustomerLogo.ShouldBe("acme-logo.png");
        dto.AliasPartNumber.ShouldBe("ALIAS-789");
        dto.Description.ShouldBe("High-precision industrial component");
        dto.CreatedBy.ShouldBe("john.doe");
        dto.ModifiedBy.ShouldBe("jane.smith");
        dto.LineId.ShouldBe(50);
        dto.RuleId.ShouldBe(2005);
        dto.Customer.ShouldBeSameAs(customer);
        dto.CreatedOn.ShouldBe(testDate);
        dto.ModifiedOn.ShouldBe(testDate.AddDays(1));
        dto.Machines.ShouldBeSameAs(machines);
        dto.MachineNames.ShouldBeSameAs(machineNames);
        dto.Line.ShouldBeSameAs(line);
    }

    // ToDto Static Method Tests
    /// <summary>
    /// Executes ToDto_WithNullProduct_ShouldReturnFailureResult operation.
    /// </summary>

    [Fact]
    public void ToDto_WithNullProduct_ShouldReturnFailureResult()
    {
        // Arrange
        Product? nullProduct = null!;

        // Act
        var result = ProductDto.ToDto(nullProduct!);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Errors.ShouldNotBeNull();
        result.Errors.ShouldContain("Parameter 'src' cannot be null");
    }
    /// <summary>
    /// Executes ToDto_WithValidProduct_ShouldMapAllProperties operation.
    /// </summary>

    [Fact]
    public void ToDto_WithValidProduct_ShouldMapAllProperties()
    {
        // Arrange
        var testDate = new DateTime(2024, 12, 25);
        var customer = new Customer { CustomerId = 100, Name = "Test Customer" };
        var line = new Line { LineId = 50, Name = "Production Line A" };

        var product = new Product
        {
            ProductId = 12345,
            PartNumber = "PART-ABC-123",
            ProductName = "Industrial Widget",
            IsActive = 1,
            Version = 2,
            CustomerPartNumber = "CUST-PART-456",
            CustomerId = 100,
            CustomerName = "ACME Manufacturing",
            AliasPartNumber = "ALIAS-789",
            Description = "High-precision industrial component",
            CreatedBy = "john.doe",
            ModifiedBy = "jane.smith",
            LineId = 50,
            RuleId = 2005,
            Customer = customer,
            CreatedOn = testDate,
            ModifiedOn = testDate.AddDays(1),
            Line = line
        };

        // Act
        var dtoWrapper = ProductDto.ToDto(product);

        // Assert
        dtoWrapper.IsSuccess.ShouldBeTrue();
        dtoWrapper.Value.ShouldNotBeNull();
        var dto = dtoWrapper.Value;
        dto.ShouldNotBeNull();
        dto.ShouldNotBeNull();
        dto.ShouldNotBeNull();
        dto.ProductId.ShouldBe(12345);
        dto.PartNumber.ShouldBe("PART-ABC-123");
        dto.ProductName.ShouldBe("Industrial Widget");
        dto.IsActive.ShouldBe(1);
        dto.Version.ShouldBe(2);
        dto.CustomerPartNumber.ShouldBe("CUST-PART-456");
        dto.CustomerId.ShouldBe(100);
        dto.CustomerName.ShouldBe("Test Customer"); // From Customer.Name
        dto.AliasPartNumber.ShouldBe("ALIAS-789");
        dto.Description.ShouldBe("High-precision industrial component");
        dto.CreatedBy.ShouldBe("john.doe");
        dto.ModifiedBy.ShouldBe("jane.smith");
        dto.LineId.ShouldBe(50);
        dto.RuleId.ShouldBe(2005);
        dto.Customer.ShouldBeSameAs(customer);
        dto.CreatedOn.ShouldBe(testDate);
        dto.ModifiedOn.ShouldBe(testDate.AddDays(1));
        dto.Line.ShouldBeSameAs(line);
    }
    /// <summary>
    /// Executes ToDto_WithNullCustomer_ShouldUseProductCustomerName operation.
    /// </summary>

    [Fact]
    public void ToDto_WithNullCustomer_ShouldUseProductCustomerName()
    {
        // Arrange
        var product = new Product
        {
            ProductId = 123,
            PartNumber = "TEST-PART",
            CustomerName = "Fallback Customer Name",
            Customer = null!
        };

        // Act
        var dtoWrapper = ProductDto.ToDto(product);

        // Assert
        dtoWrapper.IsSuccess.ShouldBeTrue();
        dtoWrapper.Value.ShouldNotBeNull();
        var dto = dtoWrapper.Value;
        dto.ShouldNotBeNull();
        dto.ShouldNotBeNull();
        dto.ShouldNotBeNull();
        dto.CustomerName.ShouldBe("Fallback Customer Name");
    }
    /// <summary>
    /// Executes ToDto_WithNullCustomerAndNullCustomerName_ShouldUseEmptyString operation.
    /// </summary>

    [Fact]
    public void ToDto_WithNullCustomerAndNullCustomerName_ShouldUseEmptyString()
    {
        // Arrange
        var product = new Product
        {
            ProductId = 123,
            PartNumber = "TEST-PART",
            CustomerName = null!,
            Customer = null!
        };

        // Act
        var dtoWrapper = ProductDto.ToDto(product);

        // Assert
        dtoWrapper.IsSuccess.ShouldBeTrue();
        dtoWrapper.Value.ShouldNotBeNull();
        var dto = dtoWrapper.Value;
        dto.ShouldNotBeNull();
        dto.ShouldNotBeNull();
        dto.ShouldNotBeNull();
        dto.CustomerName.ShouldBe(string.Empty);
    }
    /// <summary>
    /// Executes ToDto_WithNullCreatedOn_ShouldUseCurrentDateTime operation.
    /// </summary>

    [Fact]
    public void ToDto_WithNullCreatedOn_ShouldUseCurrentDateTime()
    {
        // Arrange
        var beforeCall = DateTime.Now.AddSeconds(-1);
        var product = new Product
        {
            ProductId = 123,
            PartNumber = "TEST-PART",
            CreatedOn = null
        };

        // Act
        var dtoWrapper = ProductDto.ToDto(product);

        // Assert
        dtoWrapper.IsSuccess.ShouldBeTrue();
        dtoWrapper.Value.ShouldNotBeNull();
        var dto = dtoWrapper.Value;
        var afterCall = DateTime.Now.AddSeconds(1);
        dto.CreatedOn.ShouldBeGreaterThan(beforeCall);
        dto.CreatedOn.ShouldBeLessThan(afterCall);
    }
    /// <summary>
    /// Executes ToDto_WithNullModifiedOn_ShouldUseCurrentDateTime operation.
    /// </summary>

    [Fact]
    public void ToDto_WithNullModifiedOn_ShouldUseCurrentDateTime()
    {
        // Arrange
        var beforeCall = DateTime.Now.AddSeconds(-1);
        var product = new Product
        {
            ProductId = 123,
            PartNumber = "TEST-PART",
            ModifiedOn = null
        };

        // Act
        var dtoWrapper = ProductDto.ToDto(product);

        // Assert
        dtoWrapper.IsSuccess.ShouldBeTrue();
        dtoWrapper.Value.ShouldNotBeNull();
        var dto = dtoWrapper.Value;
        var afterCall = DateTime.Now.AddSeconds(1);
        dto.ModifiedOn.ShouldBeGreaterThan(beforeCall);
        dto.ModifiedOn.ShouldBeLessThan(afterCall);
    }
    /// <summary>
    /// Executes ToDto_WithMinimalProduct_ShouldMapBasicProperties operation.
    /// </summary>

    [Fact]
    public void ToDto_WithMinimalProduct_ShouldMapBasicProperties()
    {
        // Arrange
        var product = new Product
        {
            ProductId = 1,
            PartNumber = "MIN-PART",
            ProductName = "Minimal Product"
        };

        // Act
        var dtoWrapper = ProductDto.ToDto(product);

        // Assert
        dtoWrapper.IsSuccess.ShouldBeTrue();
        dtoWrapper.Value.ShouldNotBeNull();
        var dto = dtoWrapper.Value;
        dto.ShouldNotBeNull();
        dto.ShouldNotBeNull();
        dto.ShouldNotBeNull();
        dto.ProductId.ShouldBe(1);
        dto.PartNumber.ShouldBe("MIN-PART");
        dto.ProductName.ShouldBe("Minimal Product");
        dto.CustomerName.ShouldBe(string.Empty);
    }


    // ToEntity Static Method Tests
    /// <summary>
    /// Executes ToEntity_WithNullProductDto_ShouldReturnFailureResult operation.
    /// </summary>

    [Fact]
    public void ToEntity_WithNullProductDto_ShouldReturnFailureResult()
    {
        // Arrange
        ProductDto? nullDto = null!;

        // Act
        var result = ProductDto.ToEntity(nullDto!);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Errors.ShouldNotBeNull();
        result.Errors.ShouldContain("Parameter 'src' cannot be null");
    }
    /// <summary>
    /// Executes ToEntity_WithValidProductDto_ShouldMapAllProperties operation.
    /// </summary>

    [Fact]
    public void ToEntity_WithValidProductDto_ShouldMapAllProperties()
    {
        // Arrange
        var customer = new Customer { CustomerId = 100, Name = "Test Customer" };
        var line = new Line { LineId = 50, Name = "Production Line A" };

        var dto = new ProductDto
        {
            ProductId = 12345,
            PartNumber = "PART-ABC-123",
            ProductName = "Industrial Widget",
            IsActive = 1,
            Version = 2,
            CustomerPartNumber = "CUST-PART-456",
            CustomerId = 100,
            CustomerName = "ACME Manufacturing",
            AliasPartNumber = "ALIAS-789",
            Description = "High-precision industrial component",
            CreatedBy = "john.doe",
            ModifiedBy = "jane.smith",
            LineId = 50,
            RuleId = 2005,
            Customer = customer,
            Line = line
        };

        // Act
        var entityWrapper = ProductDto.ToEntity(dto);

        // Assert
        entityWrapper.IsSuccess.ShouldBeTrue();
        entityWrapper.Value.ShouldNotBeNull();
        var entity = entityWrapper.Value;
        entity.ShouldNotBeNull();
        entity.ShouldNotBeNull();
        entity.ShouldNotBeNull();
        entity.ProductId.ShouldBe(12345);
        entity.PartNumber.ShouldBe("PART-ABC-123");
        entity.ProductName.ShouldBe("Industrial Widget");
        entity.IsActive.ShouldBe(1);
        entity.Version.ShouldBe(2);
        entity.CustomerPartNumber.ShouldBe("CUST-PART-456");
        entity.CustomerId.ShouldBe(100);
        entity.CustomerName.ShouldBe("ACME Manufacturing");
        entity.AliasPartNumber.ShouldBe("ALIAS-789");
        entity.Description.ShouldBe("High-precision industrial component");
        entity.CreatedBy.ShouldBe("john.doe");
        entity.ModifiedBy.ShouldBe("jane.smith");
        entity.LineId.ShouldBe(50);
        entity.RuleId.ShouldBe(2005);
        entity.Customer.ShouldBeSameAs(customer);
        entity.Line.ShouldBeSameAs(line);
    }
    /// <summary>
    /// Executes ToEntity_WithMinimalDto_ShouldMapBasicProperties operation.
    /// </summary>

    [Fact]
    public void ToEntity_WithMinimalDto_ShouldMapBasicProperties()
    {
        // Arrange
        var dto = new ProductDto
        {
            ProductId = 1,
            PartNumber = "MIN-PART",
            ProductName = "Minimal Product"
        };

        // Act
        var entityWrapper = ProductDto.ToEntity(dto);

        // Assert
        entityWrapper.IsSuccess.ShouldBeTrue();
        entityWrapper.Value.ShouldNotBeNull();
        var entity = entityWrapper.Value;
        entity.ShouldNotBeNull();
        entity.ShouldNotBeNull();
        entity.ShouldNotBeNull();
        entity.ProductId.ShouldBe(1);
        entity.PartNumber.ShouldBe("MIN-PART");
        entity.ProductName.ShouldBe("Minimal Product");
        entity.CustomerName.ShouldBe(string.Empty);
    }


    // Round-trip Conversion Tests
    /// <summary>
    /// Executes ToDto_ThenToEntity_ShouldPreserveBasicProperties operation.
    /// </summary>

    [Fact]
    public void ToDto_ThenToEntity_ShouldPreserveBasicProperties()
    {
        // Arrange
        var originalProduct = new Product
        {
            ProductId = 12345,
            PartNumber = "ROUND-TRIP-PART",
            ProductName = "Round Trip Product",
            IsActive = 1,
            Version = 3,
            CustomerPartNumber = "RT-CUST-PART",
            CustomerId = 200,
            CustomerName = "Round Trip Customer",
            AliasPartNumber = "RT-ALIAS",
            Description = "Round trip test product",
            CreatedBy = "test.user",
            ModifiedBy = "test.modifier",
            LineId = 75,
            RuleId = 3000
        };

        // Act
        var dtoWrapper = ProductDto.ToDto(originalProduct);
        dtoWrapper.IsSuccess.ShouldBeTrue();
        dtoWrapper.Value.ShouldNotBeNull();
        var dto = dtoWrapper.Value;

        var convertedEntityWrapper = ProductDto.ToEntity(dto);
        convertedEntityWrapper.IsSuccess.ShouldBeTrue();
        convertedEntityWrapper.Value.ShouldNotBeNull();
        var convertedEntity = convertedEntityWrapper.Value;
        convertedEntity.ShouldNotBeNull();
        convertedEntity.ShouldNotBeNull();

        // Assert
        convertedEntity.ProductId.ShouldBe(originalProduct.ProductId);
        convertedEntity.PartNumber.ShouldBe(originalProduct.PartNumber);
        convertedEntity.ProductName.ShouldBe(originalProduct.ProductName);
        convertedEntity.IsActive.ShouldBe(originalProduct.IsActive);
        convertedEntity.Version.ShouldBe(originalProduct.Version);
        convertedEntity.CustomerPartNumber.ShouldBe(originalProduct.CustomerPartNumber);
        convertedEntity.CustomerId.ShouldBe(originalProduct.CustomerId);
        convertedEntity.AliasPartNumber.ShouldBe(originalProduct.AliasPartNumber);
        convertedEntity.Description.ShouldBe(originalProduct.Description);
        convertedEntity.CreatedBy.ShouldBe(originalProduct.CreatedBy);
        convertedEntity.ModifiedBy.ShouldBe(originalProduct.ModifiedBy);
        convertedEntity.LineId.ShouldBe(originalProduct.LineId);
        convertedEntity.RuleId.ShouldBe(originalProduct.RuleId);
    }


    // Industrial Scenario Tests
    /// <summary>
    /// Executes ProductDto_WithIndustrialAutomotiveScenario_ShouldHandleComplexData operation.
    /// </summary>

    [Fact]
    public void ProductDto_WithIndustrialAutomotiveScenario_ShouldHandleComplexData()
    {
        // Arrange - Automotive manufacturing scenario
        var dto = new ProductDto();

        // Act - Set up automotive part data
        dto.ProductId = 501234;
        dto.PartNumber = "AUTO-ENGINE-BLOCK-V8-2024";
        dto.ProductName = "V8 Engine Block Assembly";
        dto.CustomerName = "Ford Motor Company";
        dto.Description = "High-performance V8 engine block for F-150 series";
        dto.IsActive = 1;
        dto.Version = 5;
        dto.LineId = 1001; // Engine assembly line
        dto.Machines = new List<int> { 2001, 2002, 2003, 2004 }; // CNC machines
        dto.MachineNames = new Dictionary<int, string>
        {
            { 2001, "CNC Milling Station A" },
            { 2002, "CNC Boring Machine B" },
            { 2003, "Surface Finishing Station C" },
            { 2004, "Quality Inspection Station D" }
        };

        // Assert - Verify automotive scenario
        dto.PartNumber.ShouldBe("AUTO-ENGINE-BLOCK-V8-2024");
        dto.ProductName.ShouldBe("V8 Engine Block Assembly");
        dto.Machines.Count().ShouldBe(4);
        dto.MachineNames.ContainsKey(2001).ShouldBeTrue();
        dto.MachineNames[2001].ShouldBe("CNC Milling Station A");
    }
    /// <summary>
    /// Executes ProductDto_WithIndustrialElectronicsScenario_ShouldHandleComplexData operation.
    /// </summary>

    [Fact]
    public void ProductDto_WithIndustrialElectronicsScenario_ShouldHandleComplexData()
    {
        // Arrange - Electronics manufacturing scenario
        var dto = new ProductDto();

        // Act - Set up electronics component data
        dto.ProductId = 700456;
        dto.PartNumber = "PCB-MAIN-CTRL-V3.2";
        dto.ProductName = "Main Control PCB Assembly";
        dto.CustomerName = "Siemens Industrial";
        dto.Description = "Primary control board for industrial automation systems";
        dto.IsActive = 1;
        dto.Version = 3;
        dto.LineId = 3001; // Electronics assembly line
        dto.Machines = new List<int> { 5001, 5002, 5003 }; // SMT machines
        dto.MachineNames = new Dictionary<int, string>
        {
            { 5001, "SMT Pick & Place Line 1" },
            { 5002, "Reflow Oven Station" },
            { 5003, "ICT Testing Station" }
        };

        // Assert - Verify electronics scenario
        dto.PartNumber.ShouldBe("PCB-MAIN-CTRL-V3.2");
        dto.ProductName.ShouldBe("Main Control PCB Assembly");
        dto.Machines.Count().ShouldBe(3);
        dto.MachineNames.ContainsKey(5002).ShouldBeTrue();
        dto.MachineNames[5002].ShouldBe("Reflow Oven Station");
    }

}
