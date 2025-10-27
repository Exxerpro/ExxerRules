using IndFusion.SemanticRag.Domain.Models;
using Shouldly;
using Xunit;

namespace IndFusion.SemanticRag.Tests.Unit.Domain.Models;

/// <summary>
/// Unit tests for Result functional type.
/// </summary>
public class ResultTests
{
    [Fact]
    public void Should_CreateSuccessfulResult_When_ValueIsProvided()
    {
        // Arrange
        var value = "test value";

        // Act
        var result = Result<string>.Success(value);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.IsFailure.ShouldBeFalse();
        result.Value.ShouldBe(value);
        result.Error.ShouldBeNull();
    }

    [Fact]
    public void Should_CreateFailedResult_When_ErrorIsProvided()
    {
        // Arrange
        var error = "test error";

        // Act
        var result = Result<string>.WithFailure(error);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(error);
    }

    [Fact]
    public void Should_ThrowException_When_AccessingValueOfFailedResult()
    {
        // Arrange
        var result = Result<string>.WithFailure("test error");

        // Act & Assert
        var exception = Should.Throw<InvalidOperationException>(() => result.Value);
        exception.Message.ShouldContain("Cannot access value of failed result");
        exception.Message.ShouldContain("test error");
    }

    [Fact]
    public void Should_ImplicitlyConvertValueToSuccessfulResult()
    {
        // Arrange
        var value = "test value";

        // Act
        Result<string> result = value;

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(value);
    }

    [Fact]
    public void Should_ImplicitlyConvertSuccessfulResultToValue()
    {
        // Arrange
        var result = Result<string>.Success("test value");

        // Act
        string value = result.Value!;

        // Assert
        value.ShouldBe("test value");
    }

    [Fact]
    public void Should_MapSuccessfulResult_When_ResultIsSuccess()
    {
        // Arrange
        var result = Result<int>.Success(5);
        Func<int, string> mapper = x => x.ToString();

        // Act
        var mappedResult = result.Map(mapper);

        // Assert
        mappedResult.IsSuccess.ShouldBeTrue();
        mappedResult.Value.ShouldBe("5");
    }

    [Fact]
    public void Should_NotMapFailedResult_When_ResultIsFailure()
    {
        // Arrange
        var result = Result<int>.WithFailure("test error");
        Func<int, string> mapper = x => x.ToString();

        // Act
        var mappedResult = result.Map(mapper);

        // Assert
        mappedResult.IsFailure.ShouldBeTrue();
        mappedResult.Error.ShouldBe("test error");
    }

    [Fact]
    public void Should_BindSuccessfulResult_When_ResultIsSuccess()
    {
        // Arrange
        var result = Result<int>.Success(5);
        Func<int, Result<string>> binder = x => Result<string>.Success(x.ToString());

        // Act
        var boundResult = result.Bind(binder);

        // Assert
        boundResult.IsSuccess.ShouldBeTrue();
        boundResult.Value.ShouldBe("5");
    }

    [Fact]
    public void Should_NotBindFailedResult_When_ResultIsFailure()
    {
        // Arrange
        var result = Result<int>.WithFailure("test error");
        Func<int, Result<string>> binder = x => Result<string>.Success(x.ToString());

        // Act
        var boundResult = result.Bind(binder);

        // Assert
        boundResult.IsFailure.ShouldBeTrue();
        boundResult.Error.ShouldBe("test error");
    }

    [Fact]
    public void Should_ConvertResultTToResult_When_ResultIsSuccess()
    {
        // Arrange
        var result = Result<string>.Success("test value");

        // Act
        var convertedResult = result.IsSuccess ? Result.Success() : Result.WithFailure(result.Error!);

        // Assert
        convertedResult.IsSuccess.ShouldBeTrue();
        convertedResult.Error.ShouldBeNull();
    }

    [Fact]
    public void Should_ConvertResultTToResult_When_ResultIsFailure()
    {
        // Arrange
        var result = Result<string>.WithFailure("test error");

        // Act
        var convertedResult = result.IsSuccess ? Result.Success() : Result.WithFailure(result.Error!);

        // Assert
        convertedResult.IsFailure.ShouldBeTrue();
        convertedResult.Error.ShouldBe("test error");
    }

    [Fact]
    public void Should_ValidateNotNullSuccessfully_When_AllParametersAreNotNull()
    {
        // Arrange
        var parameters = new[]
        {
            ((object?)"value1", "param1"),
            ((object?)"value2", "param2"),
            ((object?)"value3", "param3")
        };

        // Act
        var result = parameters.All(p => p.Item1 != null) 
            ? Result.Success()
            : Result.WithFailure("Parameter cannot be null");

        // Assert
        result.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public void Should_ValidateNotNullFailure_When_SomeParametersAreNull()
    {
        // Arrange
        var parameters = new[]
        {
            ((object?)"value1", "param1"),
            ((object?)null, "param2"),
            ((object?)"value3", "param3")
        };

        // Act
        var result = parameters.All(p => p.Item1 != null) 
            ? Result.Success()
            : Result.WithFailure("Parameter cannot be null");

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe("Null arguments: param2");
    }

    [Fact]
    public void Should_ValidateNotNullFailure_When_MultipleParametersAreNull()
    {
        // Arrange
        var parameters = new[]
        {
            ((object?)null, "param1"),
            ((object?)"value2", "param2"),
            ((object?)null, "param3")
        };

        // Act
        var result = parameters.All(p => p.Item1 != null) 
            ? Result.Success()
            : Result.WithFailure("Parameter cannot be null");

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe("Null arguments: param1, param3");
    }
}