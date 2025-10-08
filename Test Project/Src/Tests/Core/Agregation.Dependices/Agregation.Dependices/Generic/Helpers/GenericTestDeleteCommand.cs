using IndTrace.Application.Generic.Commands.Delete;

namespace IndTrace.Agregation.Dependices.Generic.Helpers;

/// <summary>
/// Represents the GenericTestDeleteCommand.
/// </summary>

public class GenericTestDeleteCommand : IDeleteCommand<GenericTestEntity>
{
    /// <summary>
    /// Gets or sets the id.
    /// </summary>
    public int Id { get; set; }
}
