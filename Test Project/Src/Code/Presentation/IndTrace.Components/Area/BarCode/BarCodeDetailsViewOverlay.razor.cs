using IndTrace.Application.BarCodes.Queries.GetBarCodeDetail;
using IndTrace.UI.Models.BarCodes;
using Microsoft.AspNetCore.Components;

namespace IndTrace.Components.Area.BarCode;

/// <summary>
/// Component for displaying an overlay with barcode details.
/// </summary>
public partial class BarCodeDetailsViewOverlay
{
    /// <summary>
    /// Gets or sets the barcode detail view model.
    /// </summary>
    [Parameter]
    public BarCodeDetailVm? BarCodeDetail { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the overlay is visible.
    /// </summary>
    [Parameter]
    public bool IsVisible { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked when the overlay is closed.
    /// </summary>
    [Parameter]
    public EventCallback OnClose { get; set; }
}
