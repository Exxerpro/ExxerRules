using IndTrace.Application.Generic.Commands.Detail;

namespace IndTrace.Aggregation.BoundedTests.Generic.Helpers;

/// <summary>
/// Represents the GenericTestDetailQuery.
/// </summary>

public class GenericTestDetailQuery : IDetailQuery<GenericTestEntity>
{
    /// <summary>
    /// Gets or sets the EntitieId.
    /// </summary>
    public int Id { get; set; }
}
