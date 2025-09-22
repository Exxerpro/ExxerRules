namespace IndFusion.Mcp.Tests;

/// <summary>
/// Tests for parsing and validating line/column range strings.
/// </summary>
public class ExxerFactoringHelpersRangeTests
{
    /// <summary>
    /// Parses a valid range string into expected numeric components.
    /// </summary>
    [Fact]
    public void TryParseRange_ValidFormat_ReturnsParsedValues()
    {
        var success = ExxerFactoringHelpers.TryParseRange("2:3-4:5", out var sl, out var sc, out var el, out var ec);

        Assert.True(success);
        Assert.Equal(2, sl);
        Assert.Equal(3, sc);
        Assert.Equal(4, el);
        Assert.Equal(5, ec);
    }

    /// <summary>
    /// Returns false when the range string is malformed.
    /// </summary>
    [Theory]
    [InlineData("invalid")]
    [InlineData("1:2:3-4:5")]
    [InlineData("1:2-4")]
    public void TryParseRange_InvalidFormat_ReturnsFalse(string range)
    {
        var result = ExxerFactoringHelpers.TryParseRange(range, out _, out _, out _, out _);
        Assert.False(result);
    }

    /// <summary>
    /// Fails validation when any component is negative.
    /// </summary>
    [Fact]
    public void ValidateRange_NegativeValues_ReturnsError()
    {
        var text = SourceText.From("line1\nline2");
        var valid = ExxerFactoringHelpers.ValidateRange(text, -1, 1, 1, 2, out var error);

        Assert.False(valid);
        Assert.Equal("Error: Range values must be positive", error);
    }

    /// <summary>
    /// Fails validation when end precedes start.
    /// </summary>
    [Fact]
    public void ValidateRange_ReversedRange_ReturnsError()
    {
        var text = SourceText.From("line1\nline2");
        var valid = ExxerFactoringHelpers.ValidateRange(text, 2, 5, 1, 4, out var error);

        Assert.False(valid);
        Assert.Equal("Error: Range start must precede end", error);
    }

    /// <summary>
    /// Fails validation when the range exceeds file length.
    /// </summary>
    [Fact]
    public void ValidateRange_ExceedsFileLength_ReturnsError()
    {
        var text = SourceText.From("line1\nline2");
        var valid = ExxerFactoringHelpers.ValidateRange(text, 1, 1, text.Lines.Count + 1, 1, out var error);

        Assert.False(valid);
        Assert.Equal("Error: Range exceeds file length", error);
    }
}
