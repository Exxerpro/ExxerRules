namespace IndTrace.DataStore.Extensions;

/// <summary>
/// Provides string utility extension methods.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Cleans the input string by removing whitespace, newline, and non-visible characters.
    /// </summary>
    /// <param name="input">The input string to clean.</param>
    /// <returns>The cleaned string.</returns>
    //TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate string extensions logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
    public static string CleanString(this string input)
    {
        return string.IsNullOrEmpty(input) ? input :
            // Remove whitespace, newline characters, and non-visible characters
            new string(input.Where(c => !char.IsControl(c) && !char.IsWhiteSpace(c)).ToArray());
    }
}
