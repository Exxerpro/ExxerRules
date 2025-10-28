namespace ExxerAI.Architecture.Tests;

/// <summary>
/// Test language model entity for behavioral tests.
/// </summary>
public sealed class LanguageModel
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Provider { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}