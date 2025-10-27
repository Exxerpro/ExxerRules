using IndFusion.SemanticRag.Domain.ValueObjects;
using Shouldly;
using Xunit;

namespace IndFusion.SemanticRag.Tests.Unit.Domain.ValueObjects;

/// <summary>
/// Unit tests for SearchQuery value object.
/// </summary>
public class SearchQueryTests
{
    [Fact]
    public void Should_CreateSearchQuery_When_ValidParametersProvided()
    {
        // Arrange
        var text = "test query";
        var limit = 20;
        var threshold = 0.5f;
        var filters = new Dictionary<string, object> { ["type"] = "code" };

        // Act
        var query = new SearchQuery(text, limit, threshold, filters);

        // Assert
        query.Text.ShouldBe(text);
        query.Limit.ShouldBe(limit);
        query.Threshold.ShouldBe(threshold);
        query.Filters.ShouldBe(filters);
    }

    [Fact]
    public void Should_CreateSearchQuery_When_OnlyTextProvided()
    {
        // Arrange
        var text = "test query";

        // Act
        var query = new SearchQuery(text);

        // Assert
        query.Text.ShouldBe(text);
        query.Limit.ShouldBe(10); // Default value
        query.Threshold.ShouldBe(0.0f); // Default value
        query.Filters.ShouldBeNull(); // Default value
    }

    [Fact]
    public void Should_ValidateSuccessfully_When_ValidQuery()
    {
        // Arrange
        var query = CreateValidQuery();

        // Act
        var result = query.Validate();

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.IsFailure.ShouldBeFalse();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Should_ValidateFailure_When_TextIsNullOrEmptyOrWhitespace(string? text)
    {
        // Arrange
        var query = new SearchQuery(text!);

        // Act
        var result = query.Validate();

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error!.ShouldContain("cannot be null, empty, or whitespace");
    }

    [Fact]
    public void Should_ValidateFailure_When_TextExceedsMaxLength()
    {
        // Arrange
        var longText = new string('a', 1001); // 1001 characters
        var query = new SearchQuery(longText);

        // Act
        var result = query.Validate();

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error!.ShouldContain("cannot exceed 1000 characters");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-10)]
    public void Should_ValidateFailure_When_LimitIsZeroOrNegative(int limit)
    {
        // Arrange
        var query = new SearchQuery("test query", limit);

        // Act
        var result = query.Validate();

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error!.ShouldContain("must be greater than zero");
    }

    [Fact]
    public void Should_ValidateFailure_When_LimitExceedsMaxValue()
    {
        // Arrange
        var query = new SearchQuery("test query", 1001);

        // Act
        var result = query.Validate();

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error!.ShouldContain("cannot exceed 1000");
    }

    [Theory]
    [InlineData(-0.1f)]
    [InlineData(1.1f)]
    [InlineData(2.0f)]
    [InlineData(-1.0f)]
    public void Should_ValidateFailure_When_ThresholdIsOutOfRange(float threshold)
    {
        // Arrange
        var query = new SearchQuery("test query", 10, threshold);

        // Act
        var result = query.Validate();

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error!.ShouldContain("must be between 0.0 and 1.0");
    }

    [Theory]
    [InlineData(0.0f)]
    [InlineData(0.5f)]
    [InlineData(1.0f)]
    public void Should_ValidateSuccessfully_When_ThresholdIsInRange(float threshold)
    {
        // Arrange
        var query = new SearchQuery("test query", 10, threshold);

        // Act
        var result = query.Validate();

        // Assert
        result.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public void Should_CreateSuccessfully_When_ValidParametersProvided()
    {
        // Arrange
        var text = "test query";
        var limit = 20;
        var threshold = 0.5f;
        var filters = new Dictionary<string, object> { ["type"] = "code" };

        // Act
        var result = SearchQuery.Create(text, limit, threshold, filters);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Text.ShouldBe(text);
        result.Value.Limit.ShouldBe(limit);
        result.Value.Threshold.ShouldBe(threshold);
        result.Value.Filters.ShouldBe(filters);
    }

    [Fact]
    public void Should_CreateSuccessfully_When_OnlyTextProvided()
    {
        // Arrange
        var text = "test query";

        // Act
        var result = SearchQuery.Create(text);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Text.ShouldBe(text);
        result.Value.Limit.ShouldBe(10);
        result.Value.Threshold.ShouldBe(0.0f);
        result.Value.Filters.ShouldBeNull();
    }

    [Fact]
    public void Should_CreateFailure_When_InvalidParametersProvided()
    {
        // Arrange
        var text = "";

        // Act
        var result = SearchQuery.Create(text);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error!.ShouldNotBeNullOrEmpty();
    }

    [Fact]
    public void Should_GetFilterValue_When_FilterExists()
    {
        // Arrange
        var filters = new Dictionary<string, object> { ["type"] = "code", ["count"] = 42 };
        var query = new SearchQuery("test query", 10, 0.5f, filters);

        // Act
        var typeValue = query.GetFilter<string>("type");
        var countValue = query.GetFilter<int>("count");

        // Assert
        typeValue.ShouldBe("code");
        countValue.ShouldBe(42);
    }

    [Fact]
    public void Should_ReturnDefaultValue_When_FilterDoesNotExist()
    {
        // Arrange
        var filters = new Dictionary<string, object> { ["type"] = "code" };
        var query = new SearchQuery("test query", 10, 0.5f, filters);

        // Act
        var value = query.GetFilter<string>("nonexistent");

        // Assert
        value.ShouldBeNull();
    }

    [Fact]
    public void Should_ReturnDefaultValue_When_FiltersIsNull()
    {
        // Arrange
        var query = new SearchQuery("test query");

        // Act
        var value = query.GetFilter<string>("type");

        // Assert
        value.ShouldBeNull();
    }

    [Fact]
    public void Should_ReturnDefaultValue_When_FilterTypeMismatch()
    {
        // Arrange
        var filters = new Dictionary<string, object> { ["count"] = "not-a-number" };
        var query = new SearchQuery("test query", 10, 0.5f, filters);

        // Act
        var value = query.GetFilter<int>("count");

        // Assert
        value.ShouldBe(0); // Default value for int
    }

    [Fact]
    public void Should_CheckFilterExists_When_FilterExists()
    {
        // Arrange
        var filters = new Dictionary<string, object> { ["type"] = "code" };
        var query = new SearchQuery("test query", 10, 0.5f, filters);

        // Act
        var exists = query.HasFilter("type");

        // Assert
        exists.ShouldBeTrue();
    }

    [Fact]
    public void Should_CheckFilterExists_When_FilterDoesNotExist()
    {
        // Arrange
        var filters = new Dictionary<string, object> { ["type"] = "code" };
        var query = new SearchQuery("test query", 10, 0.5f, filters);

        // Act
        var exists = query.HasFilter("nonexistent");

        // Assert
        exists.ShouldBeFalse();
    }

    [Fact]
    public void Should_CheckFilterExists_When_FiltersIsNull()
    {
        // Arrange
        var query = new SearchQuery("test query");

        // Act
        var exists = query.HasFilter("type");

        // Assert
        exists.ShouldBeFalse();
    }

    [Fact]
    public void Should_ReturnStringRepresentation_When_ToStringCalled()
    {
        // Arrange
        var query = new SearchQuery("test query", 20, 0.5f);

        // Act
        var result = query.ToString();

        // Assert
        result.ShouldContain("SearchQuery: \"test query\"");
        result.ShouldContain("Limit: 20");
        result.ShouldContain("Threshold: 0.5");
    }

    private static SearchQuery CreateValidQuery()
    {
        return new SearchQuery("test query", 10, 0.5f);
    }
}
