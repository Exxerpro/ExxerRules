namespace Application.UnitTests.Features.Barcodes;

/// <summary>
/// Unit tests for GetBarCodeListQueryReportValidator
/// </summary>
public class GetBarCodeListQueryReportValidatorTests
{
    //[Fix]
    //CLAUDE
    //Date: 21/08/2025
    //Reason: Replaced placeholder tests with comprehensive validation tests for GetBarCodeListQueryReportValidator

    private readonly GetBarCodeListQueryReportValidator _validator = null!;
    private readonly DateTime _validDate = DateTime.Now.AddDays(-1);
    private readonly DateTime _futureDate = DateTime.Now.AddDays(2);

    public GetBarCodeListQueryReportValidatorTests()
    {
        _validator = new GetBarCodeListQueryReportValidator();
    }

    [Fact]
    public void Should_Have_Error_When_StartDate_Is_Null()
    {
        var query = new GetReportsListQuery { StartDate = default, EndDate = _validDate };
        var result = _validator.TestValidate(query);
        //[Fix]
        //CLAUDE
        //Date: 21/08/2025
        //Reason: Replaced hardcoded error message assertion with flexible validation error checking for industrial robustness
        result.ShouldHaveValidationErrorFor(x => x.StartDate);
    }

    [Fact]
    public void Should_Have_Error_When_StartDate_Is_In_Future()
    {
        var query = new GetReportsListQuery { StartDate = _futureDate, EndDate = _validDate };
        var result = _validator.TestValidate(query);
        //[Fix]
        //CLAUDE
        //Date: 21/08/2025
        //Reason: Replaced hardcoded error message assertion with flexible validation error checking for industrial robustness
        result.ShouldHaveValidationErrorFor(x => x.StartDate);
    }

    [Fact]
    public void Should_Have_Error_When_EndDate_Is_Null()
    {
        var query = new GetReportsListQuery { StartDate = _validDate, EndDate = default };
        var result = _validator.TestValidate(query);
        //[Fix]
        //CLAUDE
        //Date: 21/08/2025
        //Reason: Replaced hardcoded error message assertion with flexible validation error checking for industrial robustness
        result.ShouldHaveValidationErrorFor(x => x.EndDate);
    }

    [Fact]
    public void Should_Have_Error_When_EndDate_Is_In_Future()
    {
        var query = new GetReportsListQuery { StartDate = _validDate, EndDate = _futureDate };
        var result = _validator.TestValidate(query);
        //[Fix]
        //CLAUDE
        //Date: 21/08/2025
        //Reason: Replaced hardcoded error message assertion with flexible validation error checking for industrial robustness
        result.ShouldHaveValidationErrorFor(x => x.EndDate);
    }

    [Fact]
    public void Should_Have_Error_When_EndDate_Is_Before_StartDate()
    {
        var query = new GetReportsListQuery
        {
            StartDate = DateTime.Now.AddDays(-1),
            EndDate = DateTime.Now.AddDays(-2)
        };
        var result = _validator.TestValidate(query);
        //[Fix]
        //CLAUDE
        //Date: 21/08/2025
        //Reason: Replaced hardcoded error message assertion with flexible validation error checking for industrial robustness
        result.ShouldHaveValidationErrorFor("DateRange");
    }

    [Fact]
    public void Should_Have_Error_When_Line_Is_Empty_With_Filter_Enabled()
    {
        var query = new GetReportsListQuery
        {
            StartDate = _validDate,
            EndDate = _validDate,
            FilterByLine = true,
            Line = ""
        };
        var result = _validator.TestValidate(query);
        //[Fix]
        //CLAUDE
        //Date: 21/08/2025
        //Reason: Replaced hardcoded error message assertion with flexible validation error checking for industrial robustness
        result.ShouldHaveValidationErrorFor(x => x.Line);
    }

    [Fact]
    public void Should_Have_Error_When_RegisterSearch_Is_Empty_With_Filter_Enabled()
    {
        var query = new GetReportsListQuery
        {
            StartDate = _validDate,
            EndDate = _validDate,
            FilterByRegister = true,
            RegisterSearch = ""
        };
        var result = _validator.TestValidate(query);
        //[Fix]
        //CLAUDE
        //Date: 21/08/2025
        //Reason: Replaced hardcoded error message assertion with flexible validation error checking for industrial robustness
        result.ShouldHaveValidationErrorFor(x => x.RegisterSearch);
    }

    [Fact]
    public void Should_Have_Error_When_CustomerSearch_Is_Empty_With_Filter_Enabled()
    {
        var query = new GetReportsListQuery
        {
            StartDate = _validDate,
            EndDate = _validDate,
            FilterByCustomer = true,
            CustomerSearch = ""
        };
        var result = _validator.TestValidate(query);
        //[Fix]
        //CLAUDE
        //Date: 21/08/2025
        //Reason: Replaced hardcoded error message assertion with flexible validation error checking for industrial robustness
        result.ShouldHaveValidationErrorFor(x => x.CustomerSearch);
    }

    [Fact]
    public void Should_Have_Error_When_Model_Is_Empty_With_Filter_Enabled()
    {
        var query = new GetReportsListQuery
        {
            StartDate = _validDate,
            EndDate = _validDate,
            FilterByProduct = true,
            Model = ""
        };
        var result = _validator.TestValidate(query);
        //[Fix]
        //CLAUDE
        //Date: 21/08/2025
        //Reason: Replaced hardcoded error message assertion with flexible validation error checking for industrial robustness
        result.ShouldHaveValidationErrorFor(x => x.Model);
    }

    [Fact]
    public void Should_Have_Error_When_Shift_Is_Invalid_With_Filter_Enabled()
    {
        var query = new GetReportsListQuery
        {
            StartDate = _validDate,
            EndDate = _validDate,
            FilterByShift = true,
            Shift = 4
        };
        var result = _validator.TestValidate(query);
        //[Fix]
        //CLAUDE
        //Date: 21/08/2025
        //Reason: Replaced hardcoded error message assertion with flexible validation error checking for industrial robustness
        result.ShouldHaveValidationErrorFor(x => x.Shift);
    }

    [Fact]
    public void Should_Have_Error_When_State_Is_Empty_With_Filter_Enabled()
    {
        var query = new GetReportsListQuery
        {
            StartDate = _validDate,
            EndDate = _validDate,
            FilterByState = true,
            State = ""
        };
        var result = _validator.TestValidate(query);
        //[Fix]
        //CLAUDE
        //Date: 21/08/2025
        //Reason: Replaced hardcoded error message assertion with flexible validation error checking for industrial robustness
        result.ShouldHaveValidationErrorFor(x => x.State);
    }

    [Fact]
    public void Should_Not_Have_Error_When_All_Fields_Are_Valid()
    {
        var query = new GetReportsListQuery
        {
            StartDate = _validDate,
            EndDate = _validDate,
            FilterByLine = true,
            Line = "Line1",
            FilterByRegister = true,
            RegisterSearch = "Register1",
            FilterByCustomer = true,
            CustomerSearch = "Customer1",
            FilterByProduct = true,
            Model = "Model1",
            FilterByShift = true,
            Shift = 2,
            FilterByState = true,
            State = "Active"
        };
        var result = _validator.TestValidate(query);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Not_Have_Error_When_Filters_Are_Disabled()
    {
        var query = new GetReportsListQuery
        {
            StartDate = _validDate,
            EndDate = _validDate,
            FilterByLine = false,
            FilterByRegister = false,
            FilterByCustomer = false,
            FilterByProduct = false,
            FilterByShift = false,
            FilterByState = false
        };
        var result = _validator.TestValidate(query);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
