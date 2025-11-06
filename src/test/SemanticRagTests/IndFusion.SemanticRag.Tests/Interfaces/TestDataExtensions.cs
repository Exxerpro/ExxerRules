namespace IndFusion.SemanticRag.Tests.Interfaces;

/// <summary>
/// Extension methods for test data creation.
/// </summary>
public static class TestDataExtensions
{
    /// <summary>
    /// Converts a string to UTF-8 encoded bytes.
    /// </summary>
    /// <param name="text">The text to convert.</param>
    /// <returns>The UTF-8 encoded bytes.</returns>
    public static byte[] ToUtf8Bytes(this string text) => System.Text.Encoding.UTF8.GetBytes(text);
}