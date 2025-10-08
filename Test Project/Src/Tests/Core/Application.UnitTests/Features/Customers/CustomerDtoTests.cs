namespace Application.UnitTests.Features.Customers;

/// <summary>
/// Unit tests for CustomerDto data transfer object.
/// Tests the industrial customer management system.
/// </summary>
public class CustomerDtoTests
{
    /// <summary>
    /// Executes CustomerDto_Constructor_ShouldInitializeWithDefaults operation.
    /// </summary>
    [Fact]
    public void CustomerDto_Constructor_ShouldInitializeWithDefaults()
    {
        // Arrange & Act
        var customerDto = new CustomerDto();

        // Assert
        customerDto.CustomerId.ShouldBe(0);
        customerDto.Name.ShouldBe(string.Empty);
        customerDto.IsActive.ShouldBe(true);
        customerDto.HasProductRunningOnLine.ShouldBe(true);
        customerDto.CreatedBy.ShouldBe(string.Empty);
        customerDto.CreatedOn.ShouldBeNull();
        customerDto.ModifiedBy.ShouldBe(string.Empty);
        customerDto.ModifiedOn.ShouldBeNull();
    }

    /// <summary>
    /// Executes CustomerDto_Properties_ShouldAllowGetAndSet operation.
    /// </summary>

    [Fact]
    public void CustomerDto_Properties_ShouldAllowGetAndSet()
    {
        // Arrange
        var customerDto = new CustomerDto();
        var testDate = DateTime.Now;

        // Act
        customerDto.CustomerId = 1001;
        customerDto.Name = "Valeo Automotive Systems";
        customerDto.IsActive = true;
        customerDto.HasProductRunningOnLine = false;
        customerDto.CreatedBy = "admin@indtrace.com";
        customerDto.CreatedOn = testDate;
        customerDto.ModifiedBy = "user@indtrace.com";
        customerDto.ModifiedOn = testDate.AddDays(1);

        // Assert
        customerDto.CustomerId.ShouldBe(1001);
        customerDto.Name.ShouldBe("Valeo Automotive Systems");
        customerDto.IsActive.ShouldBe(true);
        customerDto.HasProductRunningOnLine.ShouldBe(false);
        customerDto.CreatedBy.ShouldBe("admin@indtrace.com");
        customerDto.CreatedOn.ShouldBe(testDate);
        customerDto.ModifiedBy.ShouldBe("user@indtrace.com");
        customerDto.ModifiedOn.ShouldBe(testDate.AddDays(1));
    }

    /// <summary>
    /// Executes CustomerDto_WithVariousValues_ShouldSetCorrectly operation.
    /// </summary>

    [Theory]
    [InlineData(2001, "BMW Manufacturing", true, true)]
    [InlineData(2002, "Mercedes-Benz Production", false, false)]
    [InlineData(2003, "Audi Assembly Plant", true, false)]
    [InlineData(2004, "Ford Motor Company", false, true)]
    public void CustomerDto_WithVariousValues_ShouldSetCorrectly(
        int customerId, string name, bool isActive, bool hasProductRunning)
    {
        // Arrange
        var customerDto = new CustomerDto();

        // Act
        customerDto.CustomerId = customerId;
        customerDto.Name = name;
        customerDto.IsActive = isActive;
        customerDto.HasProductRunningOnLine = hasProductRunning;

        // Assert
        customerDto.CustomerId.ShouldBe(customerId);
        customerDto.Name.ShouldBe(name);
        customerDto.IsActive.ShouldBe(isActive);
        customerDto.HasProductRunningOnLine.ShouldBe(hasProductRunning);
    }

    /// <summary>
    /// Executes CustomerDto_ToDto_ShouldConvertFromEntityCorrectly operation.
    /// </summary>

    [Fact]
    public void CustomerDto_ToDto_ShouldConvertFromEntityCorrectly()
    {
        // Arrange
        var customer = new Customer
        {
            CustomerId = 3001,
            Name = "Tesla Factory",
            IsActive = true
        };

        // Act
        var customerDtoWrapper = CustomerDto.ToDto(customer);

        // Assert
        customerDtoWrapper.IsSuccess.ShouldBeTrue();
        customerDtoWrapper.Value.ShouldNotBeNull();
        var customerDto = customerDtoWrapper.Value;
        customerDto.ShouldNotBeNull();
        customerDto.ShouldNotBeNull();
        customerDto.ShouldNotBeNull();
        customerDto.CustomerId.ShouldBe(3001);
        customerDto.Name.ShouldBe("Tesla Factory");
        customerDto.IsActive.ShouldBe(true);
        customerDto.HasProductRunningOnLine.ShouldBe(true); // Default value
    }

    /// <summary>
    /// Executes CustomerDto_ToEntity_ShouldConvertToDomainEntityCorrectly operation.
    /// </summary>

    [Fact]
    public void CustomerDto_ToEntity_ShouldConvertToDomainEntityCorrectly()
    {
        // Arrange
        var customerDto = new CustomerDto
        {
            CustomerId = 4001,
            Name = "Honda Manufacturing",
            IsActive = false,
            HasProductRunningOnLine = true,
            CreatedBy = "system@indtrace.com",
            CreatedOn = DateTime.Now
        };

        // Act
        var customerWrapper = CustomerDto.ToEntity(customerDto);

        // Assert
        customerWrapper.IsSuccess.ShouldBeTrue();
        customerWrapper.Value.ShouldNotBeNull();
        var customer = customerWrapper.Value;
        customer.ShouldNotBeNull();
        customer.ShouldNotBeNull();
        customer.ShouldNotBeNull();
        customer.CustomerId.ShouldBe(4001);
        customer.Name.ShouldBe("Honda Manufacturing");
        customer.IsActive.ShouldBe(false);
    }

    /// <summary>
    /// Executes CustomerDto_ToDto_WithNullEntity_ShouldReturnFailureResult operation.
    /// </summary>

    [Fact]
    public void CustomerDto_ToDto_WithNullEntity_ShouldReturnFailureResult()
    {
        // Arrange
        Customer? nullCustomer = null!;

        // Act
        var result = CustomerDto.ToDto(nullCustomer!);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Errors.ShouldNotBeNull();
        result.Errors.Count().ShouldBeGreaterThan(0);
    }

    /// <summary>
    /// Executes CustomerDto_ToEntity_WithNullDto_ShouldReturnFailureResult operation.
    /// </summary>

    [Fact]
    public void CustomerDto_ToEntity_WithNullDto_ShouldReturnFailureResult()
    {
        // Arrange
        CustomerDto? nullDto = null!;

        // Act
        var result = CustomerDto.ToEntity(nullDto!);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Errors.ShouldNotBeNull();
        result.Errors.Count().ShouldBeGreaterThan(0);
    }

    /// <summary>
    /// Executes CustomerDto_ToDtoList_ShouldConvertListCorrectly operation.
    /// </summary>

    [Fact]
    public void CustomerDto_ToDtoList_ShouldConvertListCorrectly()
    {
        // Arrange
        var customers = new List<Customer>
        {
            new() { CustomerId = 5001, Name = "Nissan Plant", IsActive = true },
            new() { CustomerId = 5002, Name = "Toyota Factory", IsActive = false }
        };

        // Act
        var customerDtosWrapper = CustomerDto.ToDtoList(customers);

        // Assert
        customerDtosWrapper.IsSuccess.ShouldBeTrue();
        customerDtosWrapper.Value.ShouldNotBeNull();
        var customerDtos = customerDtosWrapper.Value.ToList();

        // Assert
        customerDtos.Count.ShouldBe(2);
        customerDtos[0].CustomerId.ShouldBe(5001);
        customerDtos[0].Name.ShouldBe("Nissan Plant");
        customerDtos[1].CustomerId.ShouldBe(5002);
        customerDtos[1].Name.ShouldBe("Toyota Factory");
    }

    /// <summary>
    /// Executes CustomerDto_ToEntityList_ShouldConvertListCorrectly operation.
    /// </summary>

    [Fact]
    public void CustomerDto_ToEntityList_ShouldConvertListCorrectly()
    {
        // Arrange
        var customerDtos = new List<CustomerDto>
        {
            new() { CustomerId = 6001, Name = "Volvo Trucks", IsActive = true },
            new() { CustomerId = 6002, Name = "Scania Manufacturing", IsActive = false }
        };

        // Act
        var customersWrapper = CustomerDto.ToEntityList(customerDtos);

        // Assert
        customersWrapper.IsSuccess.ShouldBeTrue();
        customersWrapper.Value.ShouldNotBeNull();
        var customers = customersWrapper.Value.ToList();

        // Assert
        customers.Count.ShouldBe(2);
        customers[0].CustomerId.ShouldBe(6001);
        customers[0].Name.ShouldBe("Volvo Trucks");
        customers[1].CustomerId.ShouldBe(6002);
        customers[1].Name.ShouldBe("Scania Manufacturing");
    }

    /// <summary>
    /// Executes CustomerDto_RoundTripConversion_ShouldMaintainDataIntegrity operation.
    /// </summary>

    [Fact]
    public void CustomerDto_RoundTripConversion_ShouldMaintainDataIntegrity()
    {
        // Arrange
        var originalCustomer = new Customer
        {
            CustomerId = 7001,
            Name = "Porsche Manufacturing",
            IsActive = true
        };

        // Act
        var dtoWrapper = CustomerDto.ToDto(originalCustomer);
        dtoWrapper.IsSuccess.ShouldBeTrue();
        dtoWrapper.Value.ShouldNotBeNull();
        var dto = dtoWrapper.Value;

        var convertedBackCustomerWrapper = CustomerDto.ToEntity(dto);

        // Assert
        convertedBackCustomerWrapper.IsSuccess.ShouldBeTrue();
        convertedBackCustomerWrapper.Value.ShouldNotBeNull();
        var convertedBackCustomer = convertedBackCustomerWrapper.Value;
        convertedBackCustomer.ShouldNotBeNull();
        convertedBackCustomer.ShouldNotBeNull();
        convertedBackCustomer.ShouldNotBeNull();
        convertedBackCustomer.CustomerId.ShouldBe(originalCustomer.CustomerId);
        convertedBackCustomer.Name.ShouldBe(originalCustomer.Name);
        convertedBackCustomer.IsActive.ShouldBe(originalCustomer.IsActive);
    }

    /// <summary>
    /// Executes CustomerDto_WithIndustrialScenarios_ShouldHandleCorrectly operation.
    /// </summary>

    [Theory]
    [InlineData("Automotive OEM", true, true)]
    [InlineData("Electronics Manufacturer", false, false)]
    [InlineData("Pharmaceutical Company", true, false)]
    [InlineData("Food Processing Plant", false, true)]
    public void CustomerDto_WithIndustrialScenarios_ShouldHandleCorrectly(
        string companyName, bool isActive, bool hasProductRunning)
    {
        // Arrange & Act
        var customerDto = new CustomerDto
        {
            CustomerId = 8001,
            Name = companyName,
            IsActive = isActive,
            HasProductRunningOnLine = hasProductRunning,
            CreatedBy = "production.manager@company.com",
            CreatedOn = DateTime.UtcNow
        };

        // Assert
        customerDto.Name.ShouldBe(companyName);
        customerDto.IsActive.ShouldBe(isActive);
        customerDto.HasProductRunningOnLine.ShouldBe(hasProductRunning);
        customerDto.CreatedBy.ShouldNotBeNull();
        customerDto.CreatedOn.ShouldNotBeNull();
    }
}
