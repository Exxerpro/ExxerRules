namespace ExxerAI.Architecture.Tests;

/// <summary>
/// No-op logging scope implementation.
/// </summary>
internal sealed class NoOpScope : IDisposable
{
    public static readonly NoOpScope Instance = new();
    
    private NoOpScope() { }
    
    public void Dispose() { }
}