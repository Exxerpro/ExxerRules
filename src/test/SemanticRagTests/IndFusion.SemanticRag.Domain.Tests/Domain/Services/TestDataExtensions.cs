namespace IndFusion.SemanticRag.Domain.Tests.Domain.Services;

/// <summary>
/// Extension methods for test data creation.
/// </summary>
public static class TestDataExtensions
{
    public static byte[] ToUtf8Bytes(this string text) => System.Text.Encoding.UTF8.GetBytes(text);
}