using System.Text.RegularExpressions;

namespace IndTrace.Domain.UnitTests.BarCodesTests;

public static class ValidationAssert
{
    public static readonly Regex InvalidStatusPattern = new(@"Status Invalid.*cycle status Started",
        RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    public static readonly Regex NotNextMachinePattern = new(@"not.*next.*machine",
        RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    public static readonly Regex StatusInvalidPattern = new(@"Status Invalid.*cycle status Started",
        RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    public static void ShouldContainErrorMatching(this Result result, Regex pattern, string? description = null)
    {
        result.ShouldNotBeNull("Validation result must not be null");
        result.IsFailure.ShouldBeTrue("Validation result should indicate failure");

        result.Errors.Any(e => pattern.IsMatch(e))
            .ShouldBeTrue(description ?? $"Expected an error matching pattern: {pattern}");
    }
}
