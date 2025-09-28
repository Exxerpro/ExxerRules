namespace IndFusion.Mcp.Tests;

/// <summary>
/// Tests for move-method helper utilities ensuring unique member naming.
/// </summary>
public class MoveMethodsHelpersTests
{
    /// <summary>
    /// Returns the base member name when not already used.
    /// </summary>
    [Fact]
    public void GenerateAccessMemberName_UnusedName_ReturnsBaseName()
    {
        var existing = new HashSet<string> { "field1", "field2" };

        var result = MoveMethodAst.GenerateAccessMemberName(existing, "TargetClass");

        Assert.Equal("_targetClass", result);
        Assert.DoesNotContain(result, existing);
    }

    /// <summary>
    /// Appends numeric suffixes to produce a unique member name.
    /// </summary>
    [Fact]
    public void GenerateAccessMemberName_ExistingNamesForceNumericSuffixes_ReturnsUniqueName()
    {
        var existing = new HashSet<string> { "_targetClass", "_targetClass1" };

        var result = MoveMethodAst.GenerateAccessMemberName(existing, "TargetClass");

        Assert.Equal("_targetClass2", result);
        Assert.DoesNotContain(result, existing);
    }
}
