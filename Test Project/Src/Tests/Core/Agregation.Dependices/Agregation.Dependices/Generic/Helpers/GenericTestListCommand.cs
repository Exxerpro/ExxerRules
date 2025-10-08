using IndTrace.Application.Generic.Commands.List;

namespace IndTrace.Agregation.Dependices.Generic.Helpers;

/// <summary>
/// Represents the GenericTestListCommand.
/// </summary>
public class GenericTestListCommand : IMonitorRequest<GenericTestEntity>, TCommandList
{
    /// <summary>
    /// Gets or sets the Includes.
    /// </summary>
    public string[] Includes { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Gets or sets the Page.
    /// </summary>
    public int Page { get; set; }

    /// <summary>
    /// Gets or sets the PageSize.
    /// </summary>
    public int PageSize { get; set; }
}
